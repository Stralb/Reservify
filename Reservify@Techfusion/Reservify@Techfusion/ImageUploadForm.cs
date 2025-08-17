using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Reservify_Techfusion
{
    public partial class ImageUploadForm : Form
    {
        private Timer flickerTimer;
        private int flickerCount; // Count of flicker cycles
        private const int FlickerCycles = 6; // Number of flicker changes
        private const int FlickerInterval = 100; // Time in milliseconds for each flicker
        private Timer successTimer = new Timer();
        private OleDbConnection con;
        public int VenueId { get; set; }
        private VenueCard selectedVenueCard; // Track the selected venue card

        public ImageUploadForm()
        {
            InitializeComponent();
            // Initialize connection string and load venues
            string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Reservify.accdb");
            con = new OleDbConnection($@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={dbPath};Persist Security Info=False;");

            flickerTimer = new Timer();
            flickerTimer.Interval = FlickerInterval; // Set interval for flicker
            flickerTimer.Tick += FlickerTimer_Tick;

            successTimer.Tick += SuccessTimer_Tick;

            SetCapacitySliderRange();
            LoadVenueNames();
            LoadVenueCategorys();
            //this.Shown += ImageUploadForm_Load;
            //PopulateVenuesPanel(panelVenues);

            trackBarCapacity.ValueChanged += TrackBarCapacity_ValueChanged;
            comboBoxVenueCategory.SelectedIndexChanged += ComboBoxVenueCategory_SelectedIndexChanged;
            comboBoxVenueName.SelectedIndexChanged += ComboBoxVenueCategory_SelectedIndexChanged;
        }
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

        private void TrackBarCapacity_ValueChanged(object sender, EventArgs e)
        {
            this.labelSelectedCapacity.Text = this.trackBarCapacity.Value.ToString();
            PopulateVenuesPanel(panelVenues);
        }

        private void ComboBoxVenueCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateVenuesPanel(panelVenues); // Reload venues based on the new category selection
        }


        private void PopulateVenuesPanel(Panel panel)
        {
            venues.Clear();
            panel.Controls.Clear();
            panel.Size = new Size(300, 480);

            // Fetch available venues from the database
            var availableVenues = GetAvailableVenues().Select(venue => new
            {
                VenueID = venue.VenueID,
                VenueName = venue.VenueName,
                VenueCategory = venue.VenueCategory,
                VenueCapacity = venue.VenueCapacity
            }).ToArray();

            int yOffset = 10; // Initial offset for card positioning

            foreach (var venue in availableVenues)
            {
                var venueCard = new VenueCard(venue.VenueID, venue.VenueName, venue.VenueCategory, venue.VenueCapacity)
                {
                    VenueIndex = 0, // Example index
                    IsImageUploadForm = true // Set this property to true
                };

                venueCard.SetButtonText("Upload Image");

                // Load the image for the venue card
                LoadImageForVenue(venueCard);

                // Subscribe to the ImageUploadRequested event
                venueCard.ImageUploadRequested += VenueCard_ImageUploadRequested;

                venueCard.SelectClicked += (s, e) =>
                {
                    DeselectAllVenueCards(panel);
                    venueCard.SelectCard(); // Highlight this card
                    selectedVenueCard = venueCard; // Track the selected venue card
                };

                panel.Controls.Add(venueCard);
                venueCard.Location = new Point(10, yOffset);
                yOffset += venueCard.Height + 20; // Maintain spacing
            }

            panel.AutoScroll = (yOffset > panel.Height); // Enable scrolling if needed
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
                con.Open();
                string query = "SELECT VenueImage FROM Venue WHERE VenueID = ?";

                using (var cmd = new OleDbCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("?", venueId);
                    var result = cmd.ExecuteScalar();

                    if (result != null)
                    {
                        imageBytes = result as byte[];
                        if (imageBytes != null && imageBytes.Length > 0)
                        {
                            Console.WriteLine("Image retrieved successfully.");
                        }
                        else
                        {
                            Console.WriteLine("No image found for VenueID: " + venueId);
                        }
                    }
                    else
                    {
                        Console.WriteLine("No result returned for VenueID: " + venueId);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error retrieving image: " + ex.Message);
            }
            finally
            {
                con.Close();
            }

            return imageBytes;
        }



        private void LoadImageForSelectedVenue(int venueId)
        {
            Console.WriteLine("Loading image for VenueID: " + venueId);
            byte[] imageBytes = GetVenueImage(venueId);

            if (imageBytes != null)
            {
                using (var ms = new MemoryStream(imageBytes))
                {
                    try
                    {
                        Image image = Image.FromStream(ms);
                        selectedVenueCard.VenueImage = image;
                        selectedVenueCard.VenueImageBox.Image = image;
                        Console.WriteLine("Image loaded successfully.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error loading image from stream: " + ex.Message);
                        selectedVenueCard.VenueImageBox.Image = null; // Or set a default image
                    }
                }
            }
            else
            {
                Console.WriteLine("No image bytes found for VenueID: " + venueId);
                selectedVenueCard.VenueImageBox.Image = null; // Or set a default image
            }
        }




        private void VenueCard_ImageUploadRequested(object sender, ImageUploadEventArgs e)
        {
            // Set the selectedVenueCard based on the VenueId from the event args
            selectedVenueCard = sender as VenueCard;

            // Call the method to upload the image
            buttonUploadImage_Click(this, EventArgs.Empty);
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

        private List<Venue> venues = new List<Venue>();


        private List<Venue> GetAvailableVenues()
        {
            var venues = new List<Venue>();
            con.Open(); // Open the connection

            // Retrieve the selected VenueName and VenueCategory from the combo boxes
            string selectedVenueName = comboBoxVenueName.SelectedItem?.ToString();
            string selectedVenueCategory = comboBoxVenueCategory.SelectedItem?.ToString();
            int minCapacity = trackBarCapacity.Value; // Get the slider value

            // Construct the base query
            string query = @"
    SELECT 
        VenueID, 
        VenueNameID, 
        VenueCategoryID, 
        VenueCapacity 
    FROM 
        Venue 
    WHERE 
        VenueCategoryID IS NOT NULL";

            // Add conditions based on user selections
            if (minCapacity > 0) // Assuming the slider's minimum value is 0
            {
                query += $" AND VenueCapacity >= {minCapacity}";
            }

            if (!string.IsNullOrEmpty(selectedVenueName) && selectedVenueName != "None")
            {
                query += $" AND VenueNameID IN (SELECT VenueNameID FROM VenueName WHERE VenueName = '{selectedVenueName}')";
            }

            if (!string.IsNullOrEmpty(selectedVenueCategory) && selectedVenueCategory != "None")
            {
                query += $" AND VenueCategoryID IN (SELECT VenueCategoryID FROM VenueCategory WHERE VenueCategoryName = '{selectedVenueCategory}')";
            }

            using (var cmd = new OleDbCommand(query, con))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read()) // Read each record
                    {
                        var venueID = reader["VenueID"].ToString();
                        var venueNameID = reader["VenueNameID"].ToString();
                        var venueCategoryID = reader["VenueCategoryID"].ToString();
                        var venueCapacity = reader.GetInt32(reader.GetOrdinal("VenueCapacity"));

                        // Fetch VenueName using VenueNameID
                        string venueName = GetVenueNameByID(venueNameID, con);
                        // Fetch VenueCategory using VenueCategoryID
                        string venueCategory = GetVenueCategoryByID(venueCategoryID, con);

                        var venue = new Venue
                        {
                            VenueID = int.Parse(venueID),
                            VenueName = venueName,
                            VenueCategory = venueCategory,
                            VenueCapacity = venueCapacity
                        };
                        venues.Add(venue);
                    }
                }
            }

            con.Close(); // Close the connection
            return venues;
        }



        private string GetVenueNameByID(string venueNameID, OleDbConnection connection)
        {
            string name = string.Empty;
            string query = "SELECT VenueName FROM VenueName WHERE VenueNameID = ?";
            using (var cmd = new OleDbCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@VenueNameID", venueNameID);
                name = cmd.ExecuteScalar()?.ToString() ?? string.Empty;
            }
            return name;
        }

        private string GetVenueCategoryByID(string venueCategoryID, OleDbConnection connection)
        {
            string category = string.Empty;
            string query = "SELECT VenueCategoryName FROM VenueCategory WHERE VenueCategoryID = ?";
            using (var cmd = new OleDbCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@VenueCategoryID", venueCategoryID);
                category = cmd.ExecuteScalar()?.ToString() ?? string.Empty;
            }
            return category;
        }



        private void DeselectAllVenueCards(Panel panel)
        {
            foreach (Control control in panel.Controls)
            {
                if (control is VenueCard venueCard)
                {
                    venueCard.Deselect(); // Deselect each card
                }
            }
        }

        private void buttonUploadImage_Click(object sender, EventArgs e)
        {
            if (selectedVenueCard == null)
            {
                Console.WriteLine("Please select a venue first.");
                return;
            }

            // Open file dialog to select an image
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp|All Files|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string imagePath = openFileDialog.FileName;

                    try
                    {
                        // Load the image and display it in the selected VenueCard's PictureBox
                        Image image = Image.FromFile(imagePath);
                        selectedVenueCard.VenueImage = image; // Set the image in the VenueCard

                        // Save or replace the image in the database
                        SaveVenueImage(selectedVenueCard.VenueId, image);

                        Console.WriteLine("Image uploaded successfully!");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error loading image: " + ex.Message);
                    }
                }
                else
                {
                    Console.WriteLine("Image upload canceled.");
                }
            }
        }


        private void SaveVenueImage(int venueId, Image image)
        {
            try
            {
                con.Open(); // Open the connection

                // Convert the image to a byte array
                byte[] imageBytes;
                using (var ms = new MemoryStream())
                {
                    image.Save(ms, System.Drawing.Imaging.ImageFormat.Png); // Save as PNG or any format
                    imageBytes = ms.ToArray();
                }

                // Check if an image already exists for the given VenueID
                string checkQuery = "SELECT COUNT(*) FROM Venue WHERE VenueID = ?";
                using (var checkCmd = new OleDbCommand(checkQuery, con))
                {
                    checkCmd.Parameters.AddWithValue("?", venueId); // Correct parameter usage
                    int count = (int)checkCmd.ExecuteScalar();

                    if (count > 0) // Update existing image
                    {
                        string updateQuery = @"
                    UPDATE Venue 
                    SET VenueImage = ? 
                    WHERE VenueID = ?";

                        using (var updateCmd = new OleDbCommand(updateQuery, con))
                        {
                            HighlightSuccess(panelVenues);
                            updateCmd.Parameters.AddWithValue("?", imageBytes); // Image byte array
                            updateCmd.Parameters.AddWithValue("?", venueId); // Venue ID

                            int rowsAffected = updateCmd.ExecuteNonQuery();
                            Console.WriteLine(rowsAffected > 0 ? "Image updated successfully!" : "No records updated.");
                        }
                    }
                    else // If it doesn't exist, insert a new record
                    {
                        string insertQuery = @"
                    INSERT INTO Venue (VenueID, VenueImage) 
                    VALUES (?, ?)";

                        using (var insertCmd = new OleDbCommand(insertQuery, con))
                        {
                            insertCmd.Parameters.AddWithValue("?", venueId); // Venue ID
                            insertCmd.Parameters.AddWithValue("?", imageBytes); // Image byte array

                            int rowsInserted = insertCmd.ExecuteNonQuery();
                            Console.WriteLine(rowsInserted > 0 ? "Image inserted successfully!" : "Failed to insert image.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving image: " + ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close(); // Close the connection
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


        private void SetCapacitySliderRange()
        {
            PopulateVenuesPanel(panelVenues);
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

        private void labelHomeRedirect_Click(object sender, EventArgs e)
        {
            var mainMenu = new MainMenuForm();
            mainMenu.Show();
            this.Hide(); // Hide update profile form
        }

        private void pictureBoxReset_Click(object sender, EventArgs e)
        {
            // Reset the selected venue card
            selectedVenueCard = null; // Clear the selected venue

            // Clear text fields if any (assuming you have text boxes for venue details)
            // textBoxVenueName.Clear();
            // textBoxVenueCategory.Clear();
            // textBoxVenueCapacity.Clear();

            // Reset combo boxes
            comboBoxVenueName.SelectedIndex = 0; // Reset to "None"
            comboBoxVenueCategory.SelectedIndex = 0; // Reset to "None"

            // Reset the capacity slider
            trackBarCapacity.Value = trackBarCapacity.Minimum; // Reset to minimum capacity
            labelSelectedCapacity.Text = trackBarCapacity.Minimum.ToString(); // Update label
            panelVenues.BackColor = SystemColors.Control; // Reset to default color
            // Optionally, clear images or refresh the venue panel
            panelVenues.Controls.Clear(); // Clear existing venue cards
         //   LoadVenueNames(); // Reload venue names
          //  LoadVenueCategorys(); // Reload venue categories
            PopulateVenuesPanel(panelVenues); // Refresh venue panel display
        }

    }
}
