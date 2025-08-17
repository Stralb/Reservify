using System;
using System.Data.OleDb;
using System.Windows.Forms;

namespace Reservify_Techfusion
{
    public partial class MainMenuForm : Form
    {
        private OleDbConnection con; // Use the existing connection

        public MainMenuForm()
        {
            InitializeComponent();
            SetConnectionString();
            LoadUserName();
        }

        private void SetConnectionString()
        {
            string dbPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Reservify.accdb");
            con = new OleDbConnection($@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={dbPath};Persist Security Info=False;");
        }

        private string name;
        private void LoadUserName()
        {
            int userId = UserSession.CurrentUserId; // Assuming this holds the current user's ID
            if (userId <= 0)
            {
                MessageBox.Show("No user is currently logged in.");
                return;
            }

            using (con)
            {
                try
                {
                    con.Open();
                    // Join User and UserType tables to get the UserTypeName
                    string query = @"
                SELECT U.FirstName, U.LastName, UT.UserTypeName 
                FROM [User] AS U 
                INNER JOIN [UserType] AS UT ON U.UserTypeID = UT.UserTypeID 
                WHERE U.UserID = @UserID";

                    using (var cmd = new OleDbCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@UserID", userId);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string firstName = reader["FirstName"].ToString();
                                string lastName = reader["LastName"].ToString();
                                string userTypeName = reader["UserTypeName"].ToString();

                                this.labelWelcome.Text = $"Welcome Home, {firstName}!"; // Update welcome label

                                // Check user type and set button states accordingly
                                if (userTypeName.Equals("Lecturer", StringComparison.OrdinalIgnoreCase) ||
                                    userTypeName.Equals("Student", StringComparison.OrdinalIgnoreCase))
                                {
                                    buttonSupport.Enabled = false; // Disable support button for lecturers and students
                                    //buttonMakeBooking.Enabled = false; // Disable Make Booking button
                                }
                                else if (userTypeName.Equals("Admin", StringComparison.OrdinalIgnoreCase))
                                {
                                    buttonMakeBooking.Enabled = false;
                                    buttonManageBookings.Text = "Update Venues";
                                    buttonSupport.Enabled = true; // Enable support button for admins
                                }

                                // Change Manage Bookings button text to Update Venues
                                name = userTypeName;
                            }
                            else
                            {
                                MessageBox.Show("User not found.");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading user data: " + ex.Message);
                }
            }
        }

        private void buttonLogout_Click(object sender, EventArgs e)
        {
            RegistrationForm Register = new RegistrationForm();
            Register.Show();
            this.Hide(); // Hide main menu
        }

        private void buttonManageBookings_Click(object sender, EventArgs e)
        {

            if (name.Equals("Admin", StringComparison.OrdinalIgnoreCase))
            {
                ImageUploadForm img = new ImageUploadForm();
                img.Show();
                this.Hide(); // Hide the main menu
            }
            else
            {
                ManageBookingsForm manage = new ManageBookingsForm();
                manage.Show();
                this.Hide(); // Hide the main menu
            }
                        
        }



        private void buttonMakeBooking_Click(object sender, EventArgs e)
        {
            MakeBookingForm Book = new MakeBookingForm();
            Book.Show();
            this.Hide(); // Hide the main menu
        }

      
        private void buttonProfile_Click(object sender, EventArgs e)
        {
            UpdateProfileForm Update = new UpdateProfileForm();
            Update.Show();
            this.Hide(); // Hide the main menu
        }

        private void buttonSupport_Click(object sender, EventArgs e)
        {

           // Redirect to UpdateProfiles when clicking the updated button
            UpdateProfiles updateProfiles = new UpdateProfiles();
            updateProfiles.Show();
            this.Hide(); // Hide main menu
        }

      
    }
}
