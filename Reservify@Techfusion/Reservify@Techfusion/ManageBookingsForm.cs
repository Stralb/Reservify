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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Reservify_Techfusion
{
    public partial class ManageBookingsForm : Form
    {
        private OleDbConnection con;
        private Timer updateTimer;
        private Timer flickerTimer;
        private int flickerCount; // Count of flicker cycles
        private const int FlickerCycles = 6; // Number of flicker changes
        private const int FlickerInterval = 100; // Time in milliseconds for each flicker
        private Timer successTimer = new Timer();

        public ManageBookingsForm()
        {
            InitializeComponent();

            AddTimeOptions(); // Populate time options after the components are initialized

            string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Reservify.accdb");
            con = new OleDbConnection($@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={dbPath};Persist Security Info=False;");

            flickerTimer = new Timer();
            flickerTimer.Interval = FlickerInterval; // Set interval for flicker
            flickerTimer.Tick += FlickerTimer_Tick;
            successTimer.Tick += SuccessTimer_Tick;
            SetCapacitySliderRange();

            PrintAllBookingRecords();

            LoadVenueNames();
            LoadVenueCategorys();
            LoadAvailableVenues(); // Load available venues on form load

            LoadUserBookings(); // Load user bookings when the form is initialized


            updateTimer = new Timer();
            updateTimer.Interval = 999999999; // milliseconds
            updateTimer.Tick += UpdateTimer_Tick;

            // Subscribe to the events
            trackBarCapacity.ValueChanged += TrackBarCapacity_ValueChanged;
            comboBoxVenueCategory.SelectedIndexChanged += ComboBoxVenueCategory_SelectedIndexChanged;
            comboBoxVenueName.SelectedIndexChanged += ComboBoxVenueName_SelectedIndexChanged;
            dateTimePickerstart.ValueChanged += ComboBoxVenueCategory_SelectedIndexChanged;
            dateTimePickerend.ValueChanged += ComboBoxVenueCategory_SelectedIndexChanged;

            // Subscribe to the MonthCalendar DateChanged event
            monthCalendar.DateSelected += MonthCalendar_DateChanged;

            dataGridViewBookings.CellClick += DataGridViewBookings_CellClick;
           
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


        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            updateTimer.Stop(); // Stop the timer

            // Place your code here that should run after the user is done
        }


        private void DataGridViewBookings_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Ensure that the clicked cell is not a header cell
            if (e.RowIndex >= 0)
            {
                // Get the selected row
                DataGridViewRow selectedRow = dataGridViewBookings.Rows[e.RowIndex];

                // Retrieve data from the selected row
                string venueName = selectedRow.Cells["VenueName"].Value.ToString();
                string venueCategoryName = selectedRow.Cells["VenueCategoryName"].Value.ToString();
                DateTime startDate = Convert.ToDateTime(selectedRow.Cells["StartDate"].Value);
                DateTime endDate = Convert.ToDateTime(selectedRow.Cells["EndDate"].Value);
                string startTime = selectedRow.Cells["StartTime"].Value.ToString(); // HH:mm format
                string endTime = selectedRow.Cells["EndTime"].Value.ToString();     // HH:mm format
                int venueCapacity = Convert.ToInt32(selectedRow.Cells["VenueCapacity"].Value); // Get the VenueCapacity

                // Set the MonthCalendar to focus on the start date
                monthCalendar.SetDate(startDate); // This sets the display date to the start date of the booking
                monthCalendar.SetSelectionRange(startDate, endDate); // Set the selected range

                // Set the selected items in the combo boxes
                comboBoxVenueName.SelectedItem = venueName;
                comboBoxVenueCategory.SelectedItem = venueCategoryName;

                try
                {
                    // Combine the date from MonthCalendar with the start and end times
                    DateTime newStartTime = new DateTime(startDate.Year, startDate.Month, startDate.Day,
                        int.Parse(startTime.Split(':')[0]), int.Parse(startTime.Split(':')[1]), 0);

                    DateTime newEndTime = new DateTime(startDate.Year, startDate.Month, startDate.Day,
                        int.Parse(endTime.Split(':')[0]), int.Parse(endTime.Split(':')[1]), 0);

                    // Set the DateTimePickers to the calculated times
                    dateTimePickerstart.Value = newStartTime;
                    dateTimePickerend.Value = newEndTime;

                    // Set the capacity slider to the venue capacity
                    trackBarCapacity.Value = venueCapacity; // Set the slider value to the capacity
                }
                catch (ArgumentOutOfRangeException)
                {
                    MessageBox.Show("The selected time is out of range. Please check your data.");
                }
                catch (FormatException)
                {
                    MessageBox.Show("Error parsing the time. Please check the format.");
                }

                // Keep the focus on the clicked row

                LoadAvailableVenues(); // Reload venues based on the new category selection
                dataGridViewBookings.CurrentCell = dataGridViewBookings.Rows[e.RowIndex].Cells[0]; // Focus on the first cell of the clicked row
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
               

        // Declare the list to hold available venues
        private List<Venue> availableVenues = new List<Venue>();

        private void LoadAvailableVenues()
        {
            availableVenues.Clear();
            // Save the currently selected index for both grids
            int selectedIndexVenues = dataGridViewVenues.SelectedRows.Count > 0 ? dataGridViewVenues.SelectedRows[0].Index : -1;
            int selectedIndexBookings = dataGridViewBookings.SelectedRows.Count > 0 ? dataGridViewBookings.SelectedRows[0].Index : -1;

            //LoadUserBookings();
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
            string baseQuery = $@"
SELECT v.VenueID, vn.VenueName, vc.VenueCategoryName, v.VenueCapacity
FROM (Venue AS v
INNER JOIN VenueName AS vn ON v.VenueNameID = vn.VenueNameID)
INNER JOIN VenueCategory AS vc ON v.VenueCategoryID = vc.VenueCategoryID
WHERE v.VenueCapacity >= {trackBarCapacity.Value} AND
v.VenueID NOT IN (
    SELECT VenueID
    FROM Booking
    WHERE (
        (StartDate <= #{selectedEndDate:MM/dd/yyyy}# AND EndDate >= #{selectedStartDate:MM/dd/yyyy}#) AND
        (
            (StartTime < '{endTimeStr}' AND EndTime > '{startTimeStr}') OR
            (StartTime >= '{startTimeStr}' AND EndTime <= '{endTimeStr}') OR
            (StartTime < '{startTimeStr}' AND EndTime > '{endTimeStr}') 
        )
    )
)";

            Console.WriteLine("Executing base query: " + baseQuery);

            // Add conditions for VenueName and VenueCategory
            if (!string.IsNullOrEmpty(selectedVenueName) && selectedVenueName != "None")
            {
                baseQuery += $" AND vn.VenueName = '{selectedVenueName}'";
            }

            if (!string.IsNullOrEmpty(selectedVenueCategory) && selectedVenueCategory != "None")
            {
                baseQuery += $" AND vc.VenueCategoryName = '{selectedVenueCategory}'";
            }

            try
            {
                // Check if the connection is already open
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                    Console.WriteLine("Database connection opened successfully.");
                }

                using (var cmd = new OleDbCommand(baseQuery, con))
                {
                    Console.WriteLine("Executing base query...");
                    using (var reader = cmd.ExecuteReader())
                    {
                        dataTable.Load(reader); // Load data into DataTable

                        // Clear previous rows
                        dataGridViewVenues.Rows.Clear();

                        if (dataTable.Rows.Count > 0)
                        {
                            InitializeDataGridViewColumns(dataTable);
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
                            Console.WriteLine("No venues available. Trying relaxed search...");

                            // Fewer than 2 records, try without VenueName
                            string relaxedQuery = baseQuery.Replace($" AND vn.VenueName = '{selectedVenueName}'", "");

                            using (var relaxedCmd = new OleDbCommand(relaxedQuery, con))
                            {
                                using (var relaxedReader = relaxedCmd.ExecuteReader())
                                {
                                    dataTable.Clear(); // Clear previous results
                                    dataTable.Load(relaxedReader); // Load relaxed data into DataTable

                                    if (dataTable.Rows.Count > 0)
                                    {
                                        InitializeDataGridViewColumns(dataTable);
                                        availableVenues.Clear();
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

                                        Console.WriteLine($"{dataTable.Rows.Count} venues loaded after relaxed search.");
                                    }
                                    else
                                    {
                                        Console.WriteLine("No venues available after relaxed search. Trying without VenueCategory...");

                                        // No records found, try without VenueCategory
                                        string finalQuery = relaxedQuery.Replace($" AND vc.VenueCategoryName = '{selectedVenueCategory}'", "");

                                        using (var finalCmd = new OleDbCommand(finalQuery, con))
                                        {
                                            using (var finalReader = finalCmd.ExecuteReader())
                                            {
                                                dataTable.Clear(); // Clear previous results
                                                dataTable.Load(finalReader); // Load final data into DataTable

                                                if (dataTable.Rows.Count > 0)
                                                {
                                                    InitializeDataGridViewColumns(dataTable);
                                                    availableVenues.Clear();

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

                                                    Console.WriteLine($"{dataTable.Rows.Count} venues loaded after final search.");
                                                }
                                                else
                                                {
                                                    Console.WriteLine("No venues available after final search.");
                                                }
                                                // After loading available venues, populate the venue panel
                                               

                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading available venues: " + ex.Message);
              //  MessageBox.Show("An error occurred while loading available venues. Please try again.");
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                    Console.WriteLine("Database connection closed.");
                }

                // Restore the selected index for the venues grid
                if (selectedIndexVenues >= 0 && selectedIndexVenues < dataGridViewVenues.Rows.Count)
                {
                    dataGridViewVenues.Rows[selectedIndexVenues].Selected = true;
                    dataGridViewVenues.CurrentCell = dataGridViewVenues.Rows[selectedIndexVenues].Cells[0];
                }

                // Restore the selected index for the bookings grid
                if (selectedIndexBookings >= 0 && selectedIndexBookings < dataGridViewBookings.Rows.Count)
                {
                    dataGridViewBookings.Rows[selectedIndexBookings].Selected = true;
                    dataGridViewBookings.CurrentCell = dataGridViewBookings.Rows[selectedIndexBookings].Cells[0];
                }

                PopulateVenuesPanel(panelVenues);
            }
        }



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

                    venueCard.SelectClicked += (sender, e) =>
                    {
                        // Deselect all other venue cards but keep their colors
                        DeselectAllVenueCards(panel);

                        // Highlight the selected card
                        venueCard.SelectCard(); // Highlight this card

                        // Select the corresponding venue in the grid
                        SelectVenueInGrid(venueCard.VenueIndex);

                        PulseButton(buttonMatchMe);
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
                    buttonMatchMe.BackColor = Color.Gold; // Pulse color

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
                        Image image = Image.FromStream(ms);
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
       
                         
        private List<Booking> userBookings = new List<Booking>();


        private void LoadUserBookings()
        {
            DataTable dataTable = new DataTable(); // Create a DataTable to hold the results
            int userID = Convert.ToInt32(UserSession.GetUserId()); // Get the current user ID

            try
            {
                // Open the connection if not already open
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }

                // Corrected SQL query for Access
                string query = @"
SELECT 
    b.BookingID, 
    vn.VenueName,
    vc.VenueCategoryName,
    b.StartDate, 
    b.EndDate, 
    b.StartTime, 
    b.EndTime,
    v.VenueCapacity
FROM 
    ((Booking AS b
    INNER JOIN Venue AS v ON b.VenueID = v.VenueID)
    INNER JOIN VenueName AS vn ON v.VenueNameID = vn.VenueNameID)
    INNER JOIN VenueCategory AS vc ON v.VenueCategoryID = vc.VenueCategoryID
WHERE 
    b.UserID = @UserID"; // Use parameterized query for safety


                using (var cmd = new OleDbCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@UserID", userID); // Add parameter

                    using (var reader = cmd.ExecuteReader())
                    {
                        // Clear existing rows and columns in the DataGridView
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
                        dataGridViewBookings.Columns.Add("VenueCapacity", "Venue Capacity"); // New column for VenueCapacity

                        // Load data into DataTable
                        if (reader.HasRows)
                        {
                            dataTable.Load(reader); // Load data into DataTable
                            userBookings.Clear(); // Clear previous bookings

                            // Add rows to the DataGridView
                            foreach (DataRow row in dataTable.Rows)
                            {
                                int rowIndex = dataGridViewBookings.Rows.Add();
                                dataGridViewBookings.Rows[rowIndex].Cells["BookingID"].Value = Convert.ToInt32(row["BookingID"]);
                                dataGridViewBookings.Rows[rowIndex].Cells["VenueName"].Value = row["VenueName"].ToString();
                                dataGridViewBookings.Rows[rowIndex].Cells["VenueCategoryName"].Value = row["VenueCategoryName"].ToString();
                                dataGridViewBookings.Rows[rowIndex].Cells["StartDate"].Value = Convert.ToDateTime(row["StartDate"]).ToString("yyyy/MM/dd"); // Use Convert
                                dataGridViewBookings.Rows[rowIndex].Cells["EndDate"].Value = Convert.ToDateTime(row["EndDate"]).ToString("yyyy/MM/dd"); // Use Convert
                                dataGridViewBookings.Rows[rowIndex].Cells["StartTime"].Value = Convert.ToDateTime(row["StartTime"]).TimeOfDay; // Use Convert
                                dataGridViewBookings.Rows[rowIndex].Cells["EndTime"].Value = Convert.ToDateTime(row["EndTime"]).TimeOfDay; // Use Convert
                                dataGridViewBookings.Rows[rowIndex].Cells["VenueCapacity"].Value = Convert.ToInt32(row["VenueCapacity"]); // Use Convert

                                // Create and add booking object
                                Booking booking = new Booking
                                {
                                    BookingID = Convert.ToInt32(row["BookingID"]),
                                    VenueName = row["VenueName"].ToString(),
                                    VenueCategory = row["VenueCategoryName"].ToString(),
                                    StartDate = Convert.ToDateTime(row["StartDate"]).Date, // Ensure only date part is used
                                    EndDate = Convert.ToDateTime(row["EndDate"]).Date, // Ensure only date part is used
                                    StartTime = Convert.ToDateTime(row["StartTime"]).TimeOfDay, // Use Convert
                                    EndTime = Convert.ToDateTime(row["EndTime"]).TimeOfDay, // Use Convert
                                    VenueCapacity = Convert.ToInt32(row["VenueCapacity"])
                                };
                                userBookings.Add(booking); // Add to the list
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading user bookings: " + ex.Message);
                // You might want to show a message box here for the user
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close(); // Ensure the connection is closed
                }

                PopulateBookingsPanel(panelBookings); // Refresh the bookings panel
            }
        }




        private void PopulateBookingsPanel(Panel panel)
        {
            panel.Controls.Clear();
            panel.Size = new Size(293 + 30, 480); // Set a fixed height

            // Reset padding and margin
            panel.Padding = new Padding(0);
            panel.Margin = new Padding(0);

            if (userBookings.Count == 0)
            {
                // Create a label to display the message
                Label noBookingsLabel = new Label
                {
                    Text = "You don't have any bookings yet.",
                    AutoSize = true,
                    ForeColor = Color.Gray,
                    Location = new Point(10, 10)
                };

                panel.Controls.Add(noBookingsLabel);
            }
            else
            {
                int yOffset = 10; // Start with an initial yOffset for the first card
                int xOffset = 10; // Fixed xOffset to position the cards

                // Add booking cards
                foreach (var booking in userBookings)
                {
                    var bookingCard = CreateBookingCard(booking, yOffset);
                    bookingCard.Location = new Point(xOffset, yOffset); // Set location explicitly
                    panel.Controls.Add(bookingCard);
                    yOffset += bookingCard.Height + 10; // Maintain spacing between cards
                }

                // Enable scrollability if needed
                panel.AutoScroll = (yOffset > panel.Height);
            }
        }







        private BookingCard CreateBookingCard(Booking booking, int yOffset)
        {
            var bookingCard = new BookingCard(
                booking.VenueCategory,
                booking.VenueName,
                booking.StartDate,
                booking.EndDate,
                booking.StartTime.ToString(@"hh\:mm"),
                booking.EndTime.ToString(@"hh\:mm"),
                booking.VenueCapacity,
                booking.BookingID // Pass the BookingID here
            )
            {
                BookingIndex = userBookings.IndexOf(booking) // Set the index for later reference
            };

            // Subscribe to events
            bookingCard.EditBookingClicked += BookingCard_EditClicked;
            bookingCard.DeleteBookingClicked += OnBookingDeleteClicked; // Subscribe to delete event
            bookingCard.Location = new Point(10, yOffset);

            return bookingCard;
        }



        private void OnBookingDeleteClicked(object sender, EventArgs e)
        {
            BookingCard bookingCard = sender as BookingCard;
            if (bookingCard != null)
            {
                // Show confirmation dialog
                var result = MessageBox.Show(
                    "Are you sure you want to delete this booking?",
                    "Confirm Deletion",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                // If the user clicked "Yes", proceed with deletion
                if (result == DialogResult.Yes)
                {
                    // Remove the card from the panel
                    panelBookings.Controls.Remove(bookingCard);
                    bookingCard.Dispose(); // Dispose of the card if no longer needed

                    // Delete from the userBookings list
                    var bookingToRemove = userBookings[bookingCard.BookingIndex];
                    userBookings.Remove(bookingToRemove);

                    // Delete from the database using the existing connection
                    DeleteBookingFromDatabase(bookingCard.BookingID); // Pass the existing connection

                    // Refresh the panel to reflect the changes
                    PopulateBookingsPanel(panelBookings);
                }
            }
        }



        private void DeleteBookingFromDatabase(int bookingID)
        {

            string query = "DELETE FROM Booking WHERE BookingID = @BookingID";

            try
            {
                // Open the connection
                con.Open();

                using (var command = new OleDbCommand(query, con))
                {
                    HighlightSuccess(panelBookings);
                    command.Parameters.AddWithValue("@BookingID", bookingID);
                    command.ExecuteNonQuery(); // Execute the delete command
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting booking: " + ex.Message);
            }
            finally
            {
                // Ensure the connection is closed
                if (con.State == System.Data.ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }





        private void SelectBookingInGrid(int index)
        {
            if (index >= 0 && index < dataGridViewBookings.Rows.Count)
            {
                dataGridViewBookings.ClearSelection(); // Clear previous selections
                dataGridViewBookings.Rows[index].Selected = true; // Select the row
                dataGridViewBookings.CurrentCell = dataGridViewBookings.Rows[index].Cells[0]; // Focus on the first cell
            }
        }


        private void BookingCard_EditClicked(object sender, EventArgs e)
        {
            BookingCard selectedCard = sender as BookingCard;
            if (selectedCard != null)
            {
                // Deselect all other booking cards
                foreach (var control in panelBookings.Controls)
                {
                    if (control is BookingCard card && card != selectedCard)
                    {
                        card.Deselect(); // Deselect each card except the clicked one
                    }
                }

                // Highlight the selected card
                selectedCard.SelectCard(); // Use the method to handle selection and highlighting

                // Set the MonthCalendar to focus on the start date
                monthCalendar.SetDate(selectedCard.StartDate);
                monthCalendar.SetSelectionRange(selectedCard.StartDate, selectedCard.EndDate);

                // Ensure combo boxes are populated correctly
                comboBoxVenueName.SelectedItem = selectedCard.VenueName; // Match by the string
                comboBoxVenueCategory.SelectedItem = selectedCard.VenueCategory; // Match by the string

                try
                {
                    // Combine the date from MonthCalendar with the start and end times
                    DateTime newStartTime = new DateTime(
                        selectedCard.StartDate.Year,
                        selectedCard.StartDate.Month,
                        selectedCard.StartDate.Day,
                        int.Parse(selectedCard.StartTime.Split(':')[0]),
                        int.Parse(selectedCard.StartTime.Split(':')[1]),
                        0);

                    DateTime newEndTime = new DateTime(
                        selectedCard.EndDate.Year,
                        selectedCard.EndDate.Month,
                        selectedCard.EndDate.Day,
                        int.Parse(selectedCard.EndTime.Split(':')[0]),
                        int.Parse(selectedCard.EndTime.Split(':')[1]),
                        0);

                    // Set the DateTimePickers to the calculated times
                    dateTimePickerstart.Value = newStartTime;
                    dateTimePickerend.Value = newEndTime;

                    // Set the capacity slider to the venue capacity
                    trackBarCapacity.Value = selectedCard.VenueCapacity;
                }
                catch (ArgumentOutOfRangeException)
                {
                    MessageBox.Show("The selected time is out of range. Please check your data.");
                }
                catch (FormatException)
                {
                    MessageBox.Show("Error parsing the time. Please check the format.");
                }

                // Select the corresponding booking in the DataGridView
                SelectBookingInGrid(selectedCard.BookingIndex);

                // Load venues based on the new category selection
                LoadAvailableVenues();
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

        private void ButtonMatchMe_Click(object sender, EventArgs e)
        {
            // Validate fields
            if (!ValidateFields(out string validationMessage))
            {
               // MessageBox.Show(validationMessage);

                // Highlight the relevant panel for feedback (replace `panel` with your actual panel reference)
                HighlightValidationError(panelBookings); // Assuming you have a panel to highlight

                return;
            }

            int selectedIndex = dataGridViewBookings.SelectedRows[0].Index;

            try
            {
                con.Open();

                // Get selected booking details
                DataGridViewRow selectedBookingRow = dataGridViewBookings.SelectedRows[0];
                int bookingID = Convert.ToInt32(selectedBookingRow.Cells["BookingID"].Value);

                // Retrieve current VenueID from the booking
                int currentVenueID = GetCurrentVenueID(bookingID);

                // Retrieve new booking details
                DateTime startDate = monthCalendar.SelectionStart;
                DateTime endDate = monthCalendar.SelectionEnd;
                string startTime = dateTimePickerstart.Value.ToString("HH:mm");
                string endTime = dateTimePickerend.Value.ToString("HH:mm");

                // Check availability of the new criteria
                if (!CheckAvailability(currentVenueID, startDate, endDate, startTime, endTime, bookingID))
                {
                    MessageBox.Show("The selected criteria are too specific. Please choose a different venue.");

                    // Load available venues based on relaxed criteria
                    var availableVenues = GetAvailableVenues(startDate, endDate, startTime, endTime);

                    if (availableVenues.Count > 0)
                    {
                        ShowAvailableVenues(availableVenues);
                    }
                    else
                    {
                        MessageBox.Show("No venues available for the selected date and time.");
                    }
                    return;
                }

                // Check if a new venue is selected
                int selectedVenueID = GetSelectedVenueID();
                if (selectedVenueID != -1) // -1 indicates no venue is selected
                {
                    currentVenueID = selectedVenueID; // Update currentVenueID to the new selected venue
                }

                // Proceed to update the booking with the current venue
                UpdateBooking(bookingID, currentVenueID, startDate, endDate, startTime, endTime);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error updating booking: " + ex.ToString());
              //  MessageBox.Show("An error occurred while updating the booking: " + ex.Message);
            }
            finally
            {
                LoadAvailableVenues();
                if (con.State == ConnectionState.Open)
                    con.Close();

                // Restore selected index
                if (selectedIndex >= 0 && selectedIndex < dataGridViewBookings.Rows.Count)
                {
                    dataGridViewBookings.Rows[selectedIndex].Selected = true;
                    dataGridViewBookings.CurrentCell = dataGridViewBookings.Rows[selectedIndex].Cells[0];
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

        private int GetSelectedVenueID()
        {
            // Check if a venue is selected in the dataGridViewVenues
            if (dataGridViewVenues.SelectedRows.Count > 0)
            {
                return Convert.ToInt32(dataGridViewVenues.SelectedRows[0].Cells["VenueID"].Value);
            }
            return -1; // Indicate no selection
        }

        private int GetCurrentVenueID(int bookingID)
        {
            string query = $"SELECT VenueID FROM Booking WHERE BookingID = {bookingID}";

            using (var cmd = new OleDbCommand(query, con))
            {
                return (int)cmd.ExecuteScalar(); // Returns the VenueID
            }
        }

        private bool ValidateFields(out string validationMessage)
        {
            var emptyFields = new List<string>();

            if (dateTimePickerstart.Value == null || dateTimePickerstart.Value == DateTime.MinValue)
                emptyFields.Add("Start Time");
            if (dateTimePickerend.Value == null || dateTimePickerend.Value == DateTime.MinValue)
                emptyFields.Add("End Time");
            if (dataGridViewBookings.SelectedRows.Count == 0)
                emptyFields.Add("Select a booking to update");

            validationMessage = emptyFields.Count > 0
                ? "Please fill in the following fields:\n- " + string.Join("\n- ", emptyFields)
                : null;

            return emptyFields.Count == 0;
        }

        private bool CheckAvailability(int venueID, DateTime startDate, DateTime endDate, string startTime, string endTime, int bookingID)
        {
            string availabilityQuery = $@"
SELECT COUNT(*)
FROM Booking
WHERE VenueID = {venueID} AND
(
    (StartDate <= #{endDate:MM/dd/yyyy}# AND EndDate >= #{startDate:MM/dd/yyyy}#) AND
    (
        (StartTime < '{endTime}' AND EndTime > '{startTime}') OR
        (StartTime >= '{startTime}' AND EndTime <= '{endTime}') OR
        (StartTime < '{startTime}' AND EndTime > '{endTime}')
    )
) AND BookingID <> {bookingID}"; // Exclude current booking

            using (var availabilityCmd = new OleDbCommand(availabilityQuery, con))
            {
                int conflictingBookings = (int)availabilityCmd.ExecuteScalar();
                return conflictingBookings == 0; // Return true if no conflicts
            }
        }

        private List<int> GetAvailableVenues(DateTime startDate, DateTime endDate, string startTime, string endTime)
        {
            List<int> availableVenues = new List<int>();

            // Check with current criteria first
            string query = $@"
SELECT VenueID 
FROM Venue 
WHERE VenueID NOT IN (
    SELECT VenueID 
    FROM Booking 
    WHERE 
    (StartDate <= #{endDate:MM/dd/yyyy}# AND EndDate >= #{startDate:MM/dd/yyyy}#) AND 
    (
        (StartTime < '{endTime}' AND EndTime > '{startTime}') OR 
        (StartTime >= '{startTime}' AND EndTime <= '{endTime}') OR 
        (StartTime < '{startTime}' AND EndTime > '{endTime}')
    )
)";

            using (var cmd = new OleDbCommand(query, con))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        availableVenues.Add(reader.GetInt32(0)); // Add VenueID
                    }
                }
            }

            return availableVenues;
        }

        private void ShowAvailableVenues(List<int> availableVenues)
        {
            if (availableVenues.Count > 0)
            {
                string venuesList = string.Join("\n", availableVenues.Select(v => $"Venue ID: {v}"));
                MessageBox.Show($"Available Venues:\n{venuesList}\n\nSelect one from the venue grid to update your booking.");
            }
        }

        private void UpdateBooking(int bookingID, int venueID, DateTime startDate, DateTime endDate, string startTime, string endTime)
        {
            string updateQuery = $@"
UPDATE Booking SET 
VenueID = {venueID}, 
StartDate = #{startDate:MM/dd/yyyy}#, 
EndDate = #{endDate:MM/dd/yyyy}#, 
StartTime = '{startTime}', 
EndTime = '{endTime}' 
WHERE BookingID = {bookingID}";

            using (var cmd = new OleDbCommand(updateQuery, con))
            {
                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    //MessageBox.Show("Booking updated successfully!");
                    HighlightSuccess(panelBookings); // Highlight panel in green for success
                    LoadUserBookings(); // Refresh the booking grid after updating
                }
                else
                {
                    MessageBox.Show("Failed to update booking.");
                }
            }
        }

        private void AddTimeOptions()
        {
            // Get the current time
            DateTime now = DateTime.Now;

            // Set the DateTimePickers to the current time
            dateTimePickerstart.Value = new DateTime(now.Year, now.Month, now.Day, now.Hour, (now.Minute / 5) * 5, 0); // Rounded to nearest 5 minutes
            dateTimePickerend.Value = dateTimePickerstart.Value.AddHours(1); // Set end time to 1 hour later

            // Allow selecting any date and time by not setting MinDate
            dateTimePickerstart.MinDate = DateTime.MinValue; // Allow past dates
            dateTimePickerend.MinDate = DateTime.MinValue;   // Allow past dates
        }


        private void labelHomeRedirect_Click(object sender, EventArgs e)
        {
            var mainMenu = new MainMenuForm();
            mainMenu.Show();
            this.Hide(); // Hide update profile form
        }

        private void ManageBookingsForm_Load(object sender, EventArgs e)
        {

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

            // Optionally, reload the available venues to ensure they reflect any potential changes
            //LoadAvailableVenues();
            PopulateVenuesPanel(panelVenues);

            panelVenues.BackColor = SystemColors.Control; // Reset to default color
            // Clear any other user selections or states as necessary
            //ClearUserSelections(); // Create a method for any specific resets if needed
        }
    }

    public class Venue
    {
        public int VenueID { get; set; }
        public string VenueName { get; set; }
        public string VenueCategory { get; set; }
        public int VenueCapacity { get; set; }
    }

    public class Booking
    {
        public int BookingID { get; set; }
        public string VenueCategory { get; set; }
        public string VenueName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int VenueCapacity { get; set; }

        // Other properties as needed
    }

}

