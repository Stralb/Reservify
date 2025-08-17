using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Reservify_Techfusion
{
    public partial class MakeBookingForm : Form
    {
        private OleDbConnection con;
        private Timer flickerTimer;
        private int flickerCount; // Count of flicker cycles
        private const int FlickerCycles = 6; // Number of flicker changes
        private const int FlickerInterval = 100; // Time in milliseconds for each flicker
        private Timer successTimer = new Timer();

        private Timer highlightButtonTimer = new Timer();
        private int highlightCount = 0;
        private const int highlightLimit = 10; // Number of highlights
        private const int highlightInterval = 200; // Interval for highlighting in milliseconds


        public MakeBookingForm()
        {
            InitializeComponent();
           
            AddTimeOptions(); // Populate time options after the components are initialized

            string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Reservify.accdb");
            con = new OleDbConnection($@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={dbPath};Persist Security Info=False;");

            flickerTimer = new Timer();
            flickerTimer.Interval = FlickerInterval; // Set interval for flicker
            flickerTimer.Tick += FlickerTimer_Tick;

          
            SetCapacitySliderRange();

            //PrintAllBookingRecords();

            LoadVenueNames();
            LoadVenueCategorys();
            LoadAvailableVenues(); // Load available venues on form load

            LoadUserBookings(); // Load user bookings when the form is initialized

            // Subscribe to the events
            trackBarCapacity.ValueChanged += TrackBarCapacity_ValueChanged;
            comboBoxVenueCategory.SelectedIndexChanged += ComboBoxVenueCategory_SelectedIndexChanged;
            comboBoxVenueName.SelectedIndexChanged += ComboBoxVenueName_SelectedIndexChanged;
            //dateTimePickerstart.ValueChanged += ComboBoxVenueCategory_SelectedIndexChanged;
            //dateTimePickerend.ValueChanged += ComboBoxVenueCategory_SelectedIndexChanged;

            // Subscribe to the MonthCalendar DateChanged event
            monthCalendar.DateSelected += MonthCalendar_DateChanged;
        }

      

        private void HighlightValidationError(Panel panel)
        {
            Color originalColor = panel.BackColor; // Store the original color
            panel.BackColor = Color.PaleVioletRed; // Change to red for feedback

            // Start the flicker timer
            flickerCount = 0; // Reset flicker count
            flickerTimer.Tag = new Tuple<Panel, Color>(panel, originalColor); // Store the panel and original color reference
            flickerTimer.Start();
        }

        // Timer tick event for flickering effect
        private void FlickerTimer_Tick(object sender, EventArgs e)
        {
            if (flickerTimer.Tag is Tuple<Panel, Color> data)
            {
                var panel = data.Item1;
                var originalColor = data.Item2;

                // Toggle the panel's color
                if (flickerCount % 2 == 0)
                {
                    panel.BackColor = Color.Red; // Set to red
                }
                else
                {
                    panel.BackColor = originalColor; // Revert to original color
                }

                flickerCount++;

                // Stop the timer after the flicker cycles
                if (flickerCount >= FlickerCycles)
                {
                    panel.BackColor = originalColor; // Ensure it ends with the original color
                    flickerTimer.Stop(); // Stop the timer
                    flickerTimer.Tag = null; // Clear the tag
                }
            }
        }


        private void MonthCalendar_DateChanged(object sender, DateRangeEventArgs e)
        {
            // Optionally, you can display the selected date
            DateTime selectedDate = monthCalendar.SelectionStart;
            Console.WriteLine($"Selected Date: {selectedDate.ToShortDateString()}");

            // Reload available venues based on the new date
            LoadAvailableVenues();
        }


        private void TrackBarCapacity_ValueChanged(object sender, EventArgs e)
        {
            this.labelSelectedCapacity.Text = this.trackBarCapacity.Value.ToString();

            LoadAvailableVenues(); // Reload venues based on the new capacity
        }

        private void SetCapacitySliderRange()
        {
            try
            {
                con.Open(); // Open the connection

                // Query to get the min and max capacity from the Venue table
                string query = "SELECT MIN(VenueCapacity) AS MinCapacity, MAX(VenueCapacity) AS MaxCapacity FROM Venue";

                using (var cmd = new OleDbCommand(query, con))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int minCapacity = reader.IsDBNull(0) ? 0 : reader.GetInt32(0);
                            int maxCapacity = reader.IsDBNull(1) ? 0 : reader.GetInt32(1);

                            // Set the trackBar's minimum and maximum values
                            trackBarCapacity.Minimum = minCapacity;
                            trackBarCapacity.Maximum = maxCapacity;

                            // Optionally, set the initial value of the trackBar
                            trackBarCapacity.Value = minCapacity; // Start at min capacity
                            labelSelectedCapacity.Text = minCapacity.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error retrieving capacity range: " + ex.Message);
                MessageBox.Show("An error occurred while setting the capacity range.");
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close(); // Ensure the connection is closed
                }
            }
        }



        private void ComboBoxVenueCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadAvailableVenues(); // Reload venues based on the new category selection
        }

        private void ComboBoxVenueName_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadAvailableVenues(); // Call to update available venues based on selected VenueName
        }



        private void InitializeDataGridViewColumns(DataTable dataTable)
        {
            dataGridViewVenues.Columns.Clear(); // Clear existing columns

            // Add columns based on the DataTable's structure
            foreach (DataColumn column in dataTable.Columns)
            {
                dataGridViewVenues.Columns.Add(column.ColumnName, column.ColumnName);
            }
        }



        private void LoadVenueNames()
        {
            try
            {
                con.Open();
                string query = "SELECT VenueName FROM VenueName";
                using (var cmd = new OleDbCommand(query, con))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        comboBoxVenueName.Items.Clear(); // Clear existing items
                        comboBoxVenueName.Items.Add("None"); // Add "None" option
                        while (reader.Read())
                        {
                            comboBoxVenueName.Items.Add(reader["VenueName"].ToString());
                        }
                        comboBoxVenueName.SelectedIndex = 0; // Select "None" by default
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading venue names: " + ex.Message);
            }
            finally
            {
                con.Close(); // Close the connection
            }
        }

        private void LoadVenueCategorys()
        {
            try
            {
                con.Open();
                string query = "SELECT VenueCategoryName FROM VenueCategory";
                using (var cmd = new OleDbCommand(query, con))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        comboBoxVenueCategory.Items.Clear(); // Clear existing items
                        comboBoxVenueCategory.Items.Add("None"); // Add "None" option
                        while (reader.Read())
                        {
                            comboBoxVenueCategory.Items.Add(reader["VenueCategoryName"].ToString());
                        }
                        comboBoxVenueCategory.SelectedIndex = 0; // Select "None" by default
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading venue categories: " + ex.Message);
            }
            finally
            {
                con.Close(); // Close the connection
            }
        }


        private void LoadAvailableVenues()
        {
            LoadUserBookings();
            DataTable dataTable = new DataTable();
            DateTime selectedStartDate = monthCalendar.SelectionStart;
            DateTime selectedEndDate = monthCalendar.SelectionEnd; // Include end date selection

            // Ensure DateTimePickers have valid times selected
            if (dateTimePickerstart.Value == null || dateTimePickerend.Value == null)
            {
                MessageBox.Show("Please select valid start and end times.");
                return;
            }

            // Retrieve selected times from the DateTimePickers
            string startTimeStr = dateTimePickerstart.Value.ToString("HH:mm");
            string endTimeStr = dateTimePickerend.Value.ToString("HH:mm");

            // Retrieve the selected VenueName and VenueCategory
            string selectedVenueName = comboBoxVenueName.SelectedItem?.ToString();
            string selectedVenueCategory = comboBoxVenueCategory.SelectedItem?.ToString();

            // Construct the base query
            string query = $@"
SELECT v.VenueID, vn.VenueName, vc.VenueCategoryName, v.VenueCapacity
FROM (Venue AS v
INNER JOIN VenueName AS vn ON v.VenueNameID = vn.VenueNameID)
INNER JOIN VenueCategory AS vc ON v.VenueCategoryID = vc.VenueCategoryID
WHERE v.VenueCapacity >= {trackBarCapacity.Value} AND
v.VenueID NOT IN (
    SELECT VenueID
    FROM Booking
    WHERE (
        (StartDate <= #{selectedEndDate.ToString("MM/dd/yyyy")}# AND EndDate >= #{selectedStartDate.ToString("MM/dd/yyyy")}#) AND
        (
            (StartTime < '{endTimeStr}' AND EndTime > '{startTimeStr}') OR
            (StartTime >= '{startTimeStr}' AND EndTime <= '{endTimeStr}') OR
            (StartTime < '{startTimeStr}' AND EndTime > '{endTimeStr}') 
        )
    )
)";
            Console.WriteLine("Executing query: " + query);

            // Add condition for VenueName only if it is selected
            if (!string.IsNullOrEmpty(selectedVenueName) && selectedVenueName != "None")
            {
                query += $" AND vn.VenueName = '{selectedVenueName}'";
            }

            // Add condition for VenueCategory only if it is selected
            if (!string.IsNullOrEmpty(selectedVenueCategory) && selectedVenueCategory != "None")
            {
                query += $" AND vc.VenueCategoryName = '{selectedVenueCategory}'";
            }

            try
            {
                // Check if the connection is already open
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                    Console.WriteLine("Database connection opened successfully.");
                }

                using (var cmd = new OleDbCommand(query, con))
                {
                    Console.WriteLine("Executing query...");
                    using (var reader = cmd.ExecuteReader())
                    {
                        dataTable.Load(reader); // Load data into DataTable

                        if (dataTable.Rows.Count > 0)
                        {
                            InitializeDataGridViewColumns(dataTable);
                            dataGridViewVenues.Rows.Clear();
                            availableVenues.Clear();
                            // Populate the DataGridView with rows
                            foreach (DataRow row in dataTable.Rows)
                            {
                                int rowIndex = dataGridViewVenues.Rows.Add();
                                dataGridViewVenues.Rows[rowIndex].Cells["VenueID"].Value = row["VenueID"];
                                dataGridViewVenues.Rows[rowIndex].Cells["VenueName"].Value = row["VenueName"];
                                dataGridViewVenues.Rows[rowIndex].Cells["VenueCategoryName"].Value = row["VenueCategoryName"];
                                dataGridViewVenues.Rows[rowIndex].Cells["VenueCapacity"].Value = row["VenueCapacity"];

                                Venue venue = new Venue
                                {
                                    VenueID = (int)row["VenueID"],
                                    VenueName = (string)row["VenueName"],
                                    VenueCategory = (string)row["VenueCategoryName"],
                                    VenueCapacity = (int)row["VenueCapacity"]
                                };
                                availableVenues.Add(venue); // Add to the list

                            }

                            Console.WriteLine($"{dataTable.Rows.Count} venues loaded.");
                        }
                        else
                        {
                            Console.WriteLine("No venues available.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading available venues: " + ex.Message);
                MessageBox.Show("An error occurred while loading available venues. Please try again.");
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                    Console.WriteLine("Database connection closed.");
                }

                PopulateVenuesPanel(panelVenues);
            }
        }


        // Declare the list to hold available venues
        private List<Venue> availableVenues = new List<Venue>();




        private void PopulateVenuesPanel(Panel panel)
        {
            panel.Controls.Clear();
            panel.Size = new Size(300, 480);

            if (availableVenues.Count == 0)
            {
                // Create a label to display the message
                Label noVenuesLabel = new Label
                {
                    Text = "No alternative venues available.",
                    AutoSize = true,
                    ForeColor = Color.Red,
                    Location = new Point(10, 10)
                };

                panel.Controls.Add(noVenuesLabel);
            }
            else
            {
                int yOffset = 0; // Y offset for positioning cards

                for (int i = 0; i < availableVenues.Count; i++)
                {
                    var venue = availableVenues[i];

                    var venueCard = new VenueCard(venue.VenueID, venue.VenueName, venue.VenueCategory, venue.VenueCapacity)
                    {
                        VenueIndex = i // Set the index for the card
                    };

                    // Load the image for the venue card
                    LoadImageForVenue(venueCard);

                    venueCard.SetButtonText("Select Venue"); // Set your desired text here

                    venueCard.SelectClicked += (sender, e) =>
                    {
                        // Deselect all other venue cards but keep their colors
                        DeselectAllVenueCards(panel);

                        // Highlight the selected card
                        venueCard.SelectCard(); // Highlight this card

                        // Select the corresponding venue in the grid
                        SelectVenueInGrid(venueCard.VenueIndex);

                        // Highlight the Match Me button
                        PulseButton(buttonMatchMe); // Pass the Match Me button to highlight
                    };

                    venueCard.Location = new Point(10, yOffset);
                    panel.Controls.Add(venueCard);

                    yOffset += venueCard.Height + 20; // Maintain spacing between cards
                }

                // Set the AutoScroll property
                panel.AutoScroll = (yOffset > panel.Height);
            }
        }


        private Timer pulseTimer;
        private const int PulseInterval = 250; // Interval in milliseconds for pulsing
        private const int PulseDuration = 2000; // Total duration in milliseconds (2 seconds)
        private int pulseCount = 0;
        private int pulseLimit; // Calculate the number of pulses based on duration and interval
        private bool isOriginalColor = true;

        private void PulseButton(System.Windows.Forms.Button button)
        {
            if (pulseTimer == null)
            {
                pulseTimer = new Timer();
                pulseTimer.Interval = PulseInterval; // Set pulse interval
                pulseTimer.Tick += PulseTimer_Tick;

                pulseLimit = PulseDuration / PulseInterval; // Calculate how many times to pulse
            }

            // Start pulsing
            pulseCount = 0; // Reset the pulse count
            button.BackColor = Color.Gold; // Change to pulse color
            pulseTimer.Start(); // Start the timer
        }

        private void PulseTimer_Tick(object sender, EventArgs e)
        {
            if (pulseCount < pulseLimit)
            {
                // Toggle button color
                if (isOriginalColor)
                    buttonMatchMe.BackColor = SystemColors.Control; // Revert to original color
                else
                    buttonMatchMe.BackColor = Color.Gold; 

                isOriginalColor = !isOriginalColor; // Toggle the state
                pulseCount++; // Increment pulse count
            }
            else
            {
                pulseTimer.Stop(); // Stop the timer after pulsing is done
                buttonMatchMe.BackColor = SystemColors.Highlight; // Ensure original color is set
                isOriginalColor = true; // Reset the original color flag
            }
        }






        private void LoadImageForVenue(VenueCard venueCard)
        {
            byte[] imageBytes = GetVenueImage(venueCard.VenueId); // Get image bytes for the venue

            if (imageBytes != null)
            {
                using (var ms = new MemoryStream(imageBytes))
                {
                    try
                    {
                        System.Drawing.Image image = System.Drawing.Image.FromStream(ms);
                        venueCard.VenueImage = image; // Update the VenueCard's image
                        venueCard.VenueImageBox.Image = image; // Display in PictureBox
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error loading image from stream: " + ex.Message);
                        venueCard.VenueImageBox.Image = null; // Or set a default image
                    }
                }
            }
            else
            {
                venueCard.VenueImageBox.Image = null; // Or set a default image
            }
        }

        private byte[] GetVenueImage(int venueId)
        {
            byte[] imageBytes = null;

            try
            {
                con.Open(); // Open the connection
                string query = "SELECT VenueImage FROM Venue WHERE VenueID = ?";

                using (var cmd = new OleDbCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("?", venueId);
                    imageBytes = cmd.ExecuteScalar() as byte[];
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error retrieving image: " + ex.Message);
            }
            finally
            {
                con.Close(); // Ensure the connection is closed
            }

            return imageBytes;
        }



        private void DeselectAllVenueCards(Panel panel)
        {
            foreach (var control in panel.Controls)
            {
                if (control is VenueCard venueCard)
                {
                    venueCard.Deselect(); // Deselect each card without changing its color
                }
            }
        }

        private void SelectVenueInGrid(int index)
        {
            if (index >= 0 && index < dataGridViewVenues.Rows.Count)
            {
                dataGridViewVenues.ClearSelection(); // Clear previous selections
                dataGridViewVenues.Rows[index].Selected = true; // Select the row
                dataGridViewVenues.CurrentCell = dataGridViewVenues.Rows[index].Cells[0]; // Focus on the first cell
            }
        }


        private void LoadUserBookings()
        {
            DataTable dataTable = new DataTable(); // Create a DataTable to hold the results
            int userID = Convert.ToInt32(UserSession.GetUserId()); // Get the current user ID

            try
            {
                // Check if the connection is already open
                if (con.State != ConnectionState.Open)
                {
                    con.Open(); // Open the connection
                }

                // SQL query to get bookings for the specific user
                string query = @"
        SELECT 
            b.BookingID, 
            (SELECT vn.VenueName FROM VenueName AS vn WHERE vn.VenueNameID = (SELECT v.VenueNameID FROM Venue AS v WHERE v.VenueID = b.VenueID)) AS VenueName,
            (SELECT vc.VenueCategoryName FROM VenueCategory AS vc WHERE vc.VenueCategoryID = (SELECT v.VenueCategoryID FROM Venue AS v WHERE v.VenueID = b.VenueID)) AS VenueCategoryName,
            b.StartDate, 
            b.EndDate, 
            b.StartTime, 
            b.EndTime
        FROM Booking AS b
        WHERE b.UserID = @UserID"; // Use parameterized query for safety

                using (var cmd = new OleDbCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@UserID", userID); // Add parameter

                    using (var reader = cmd.ExecuteReader())
                    {
                        // Clear existing rows in the DataGridView
                        dataGridViewBookings.Rows.Clear();
                        dataGridViewBookings.Columns.Clear();

                        // Define the columns for the DataGridView
                        dataGridViewBookings.Columns.Add("BookingID", "Booking ID");
                        dataGridViewBookings.Columns.Add("VenueName", "Venue Name");
                        dataGridViewBookings.Columns.Add("VenueCategoryName", "Venue Category");
                        dataGridViewBookings.Columns.Add("StartDate", "Start Date");
                        dataGridViewBookings.Columns.Add("EndDate", "End Date");
                        dataGridViewBookings.Columns.Add("StartTime", "Start Time");
                        dataGridViewBookings.Columns.Add("EndTime", "End Time");

                        // Load data into DataTable
                        if (reader.HasRows)
                        {
                            dataTable.Load(reader); // Load data into DataTable

                            // Add rows to the DataGridView
                            foreach (DataRow row in dataTable.Rows)
                            {
                                int rowIndex = dataGridViewBookings.Rows.Add();
                                dataGridViewBookings.Rows[rowIndex].Cells["BookingID"].Value = row["BookingID"];
                                dataGridViewBookings.Rows[rowIndex].Cells["VenueName"].Value = row["VenueName"];
                                dataGridViewBookings.Rows[rowIndex].Cells["VenueCategoryName"].Value = row["VenueCategoryName"];
                                dataGridViewBookings.Rows[rowIndex].Cells["StartDate"].Value = row["StartDate"];
                                dataGridViewBookings.Rows[rowIndex].Cells["EndDate"].Value = row["EndDate"];
                                dataGridViewBookings.Rows[rowIndex].Cells["StartTime"].Value = row["StartTime"];
                                dataGridViewBookings.Rows[rowIndex].Cells["EndTime"].Value = row["EndTime"];
                            }
                        }
                        // If no bookings are found, the DataGridView will remain empty
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading user bookings: " + ex.Message);
                MessageBox.Show("An error occurred while loading your bookings. Please try again.");
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close(); // Ensure the connection is closed
                }
            }
        }


        private void InitializeDataGridViewBookingColumns(DataTable dataTable)
        {
            dataGridViewBookings.Columns.Clear(); // Clear existing columns

            // Add columns based on the DataTable's structure
            foreach (DataColumn column in dataTable.Columns)
            {
                dataGridViewBookings.Columns.Add(column.ColumnName, column.ColumnName);
            }

            // Optionally hide certain columns (e.g., BookingID) if needed
            dataGridViewBookings.Columns["BookingID"].Visible = false; // Hide BookingID
        }

        private void PrintAllBookingRecords()
        {
            try
            {
                con.Open(); // Open the connection
                Console.WriteLine("Database connection opened successfully.");

                // SQL query to get all records from the Booking table
                string query = "SELECT * FROM Booking";

                using (var cmd = new OleDbCommand(query, con))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows) // Check if there are records
                        {
                            Console.WriteLine("Booking Records:");
                            while (reader.Read()) // Loop through all records
                            {
                                // Print all fields for each record
                                Console.WriteLine($"Booking ID: {reader["BookingID"]}");
                                Console.WriteLine($"User ID: {reader["UserID"]}");
                                Console.WriteLine($"Venue ID: {reader["VenueID"]}");
                                Console.WriteLine($"Event Type: {reader["EventTypeID"]}");
                                Console.WriteLine($"Start Date: {reader["StartDate"]}");
                                Console.WriteLine($"End Date: {reader["EndDate"]}");
                                Console.WriteLine($"Start Time: {reader["StartTime"]}");
                                Console.WriteLine($"End Time: {reader["EndTime"]}");
                                Console.WriteLine("-------------------------------"); // Separator for clarity
                            }
                        }
                        else
                        {
                            Console.WriteLine("No records found in the Booking table.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error retrieving booking records: " + ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close(); // Ensure the connection is closed
                    Console.WriteLine("Database connection closed.");
                }
            }
        }


        
        public void ButtonMatchMe_Click(object sender, EventArgs e)
        {
            // Initialize a list to keep track of empty fields
            List<string> emptyFields = new List<string>();

            // Check for empty fields and add corresponding messages
            // Remove checks for ComboBoxes and check DateTimePickers instead
            if (dateTimePickerstart.Value == null || dateTimePickerstart.Value == DateTime.MinValue)
                emptyFields.Add("Start Time");
            if (dateTimePickerend.Value == null || dateTimePickerend.Value == DateTime.MinValue)
                emptyFields.Add("End Time");
            if (dataGridViewVenues.SelectedRows.Count == 0)
                emptyFields.Add("Venue (select a venue from the grid)");

            // If there are any empty fields, show a message and return
            if (emptyFields.Count > 0)
            {
                HighlightValidationError(panelVenues); // Assuming you have a panel to highlight

               // MessageBox.Show("Please fill in the following fields:\n- " + string.Join("\n- ", emptyFields));
                return;
            }

            try
            {
                con.Open(); // Open the connection

                // Retrieve the UserID from the session
                int userID = Convert.ToInt32(UserSession.GetUserId());

                // Retrieve the selected row from the DataGridView
                DataGridViewRow selectedRow = dataGridViewVenues.SelectedRows[0];

                // Assuming the selected row contains VenueID and EventTypeID directly
                int venueID = Convert.ToInt32(selectedRow.Cells["VenueID"].Value); // Assuming "VenueID" is the column name
                int eventType = 0; // Assuming "EventTypeID" is also in the selected row

                // Retrieve the selected date(s) from the MonthCalendar
                DateTime startDate = monthCalendar.SelectionStart.Date;
                DateTime endDate = monthCalendar.SelectionEnd.Date;

                // If only one date is selected, both start and end dates are the same
                if (startDate.Date == endDate.Date)
                {
                    endDate = startDate;
                }

                // Format start and end times from DateTimePickers
                string startTime = dateTimePickerstart.Value.ToString("HH:mm");
                string endTime = dateTimePickerend.Value.ToString("HH:mm");

                // Debugging output
                Console.WriteLine($"UserID: {userID}, VenueID: {venueID}, EventType: {eventType}, StartDate: {startDate}, EndDate: {endDate}, StartTime: {startTime}, EndTime: {endTime}");

                // SQL query to insert a new booking
                string query = $"INSERT INTO Booking (UserID, VenueID, EventTypeID, StartDate, EndDate, StartTime, EndTime) " +
                               $"VALUES ({userID}, {venueID}, {eventType}, #{startDate}#, #{endDate}#, '{startTime}', '{endTime}')";

                using (var cmd = new OleDbCommand(query, con))
                {
                    // Execute the insert command
                    int rowsAffected = cmd.ExecuteNonQuery();

                    // Check if the booking was successfully added
                    if (rowsAffected > 0)
                    {
                        HighlightSuccess(panelVenues);
                        MessageBox.Show("Booking added successfully!");
                        LoadUserBookings(); // Refresh the booking grid after adding a booking
                    }
                    else
                    {
                        MessageBox.Show("Failed to add booking.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error adding booking: " + ex.Message);
              //  MessageBox.Show("An error occurred while adding the booking.");
            }
            finally
            {
                LoadAvailableVenues();
                if (con.State == ConnectionState.Open)
                {
                    con.Close(); // Ensure the connection is closed
                }
            }
        }

        private void HighlightSuccess(Panel panel)
        {
            Color originalColor = panel.BackColor; // Store the original color
            panel.BackColor = Color.Green; // Change to green for success feedback

            // Start a timer to revert the color
            successTimer.Interval = 2000; // Time in milliseconds to stay green
            successTimer.Tag = panel; // Store the panel reference
            successTimer.Start();
        }

        // Timer tick event to revert color back to original
        private void SuccessTimer_Tick(object sender, EventArgs e)
        {
            if (successTimer.Tag is Panel panel)
            {
                panel.BackColor = SystemColors.Control; // Revert to the original color
                successTimer.Stop(); // Stop the timer
                successTimer.Tag = null; // Clear the tag
            }
        }






        // Assuming the GetVenueID and GetVenueCategoryID methods remain the same
        private int GetVenueID(string venueName)
        {
            int venueID = 0;

            try
            {
                con.Open(); // Open the connection

                // SQL query to retrieve VenueID based on VenueName
                string query = $"SELECT VenueNameID FROM VenueName WHERE VenueName = '{venueName}'";

                using (var cmd = new OleDbCommand(query, con))
                {
                    var result = cmd.ExecuteScalar(); // Execute the query and get the first result

                    if (result != null)
                    {
                        venueID = Convert.ToInt32(result); // Convert the result to an integer
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error retrieving VenueID: " + ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close(); // Ensure the connection is closed
                }
            }

            return venueID;
        }


        private int GetVenueCategoryID(string venueCategoryName)
        {
            int venueCategoryID = 0;

            try
            {
                con.Open(); // Open the connection

                // SQL query to retrieve VenueCategoryID based on VenueCategoryName
                string query = $"SELECT VenueCategoryID FROM VenueCategory WHERE VenueCategoryName = '{venueCategoryName}'";

                using (var cmd = new OleDbCommand(query, con))
                {
                    var result = cmd.ExecuteScalar(); // Execute the query and get the first result

                    if (result != null)
                    {
                        venueCategoryID = Convert.ToInt32(result); // Convert the result to an integer
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error retrieving VenueCategoryID: " + ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close(); // Ensure the connection is closed
                }
            }

            return venueCategoryID;
        }








        private void AddTimeOptions()
        {
            // Get the current time
            DateTime now = DateTime.Now;

            // Set the DateTimePickers to the current time
            dateTimePickerstart.Value = new DateTime(now.Year, now.Month, now.Day, now.Hour, (now.Minute / 5) * 5, 0); // Rounded to nearest 5 minutes
            dateTimePickerend.Value = dateTimePickerstart.Value.AddHours(1); // Set end time to 1 hour later

            // Optionally, you can disable past times if needed
            dateTimePickerstart.MinDate = now;
            dateTimePickerend.MinDate = now;
        }



        private void dataGridViewVenues_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void labelHomeRedirect_Click(object sender, EventArgs e)
        {
            var mainMenu = new MainMenuForm();
            mainMenu.Show();
            this.Hide(); // Hide update profile form
        }

        private void pictureBoxReset_Click(object sender, EventArgs e)
        {
            // Reset the TrackBar for capacity
            trackBarCapacity.Value = trackBarCapacity.Minimum;

            // Reset the ComboBoxes to their default state (first item or "None")
            comboBoxVenueCategory.SelectedIndex = 0; // Assuming "None" is the first item
            comboBoxVenueName.SelectedIndex = 0; // Assuming "None" is the first item

            // Reset the DateTimePickers to the current date
            dateTimePickerstart.Value = DateTime.Now;
            dateTimePickerend.Value = DateTime.Now.AddHours(1); // Assuming you want the end time to be one hour after the start time

            // Reset the MonthCalendar selection
            monthCalendar.SetDate(DateTime.Now); // Reset to the current date

            // Reset the panel's background color if it was changed due to validation
            panelVenues.BackColor = SystemColors.Control; // Reset to default color

            // Reload the available venues to ensure they reflect any potential changes
            PopulateVenuesPanel(panelVenues);

            // Clear any other user selections or states as necessary
            // ClearUserSelections(); // Create a method for any specific resets if needed

            // Highlight the "Match Me" button for feedback
          //  PulseButton(buttonMatchMe); // Call the highlight method for the button
        }


    }
}
