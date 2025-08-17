using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Reservify_Techfusion
{
    public partial class UpdateProfiles : Form
    {
        // Local variables to hold initial user data
        private string initialFirstName;
        private string initialLastName;
        private string initialEmail;
        private string initialContactNumber;
        private int initialUserTypeId;

        private Timer flickerTimer;
        private int flickerCount; // Count of flicker cycles
        private const int FlickerCycles = 6; // Number of flicker changes
        private const int FlickerInterval = 100; // Time in milliseconds for each flicker
        private Timer successTimer = new Timer();

        private OleDbConnection con;

        public UpdateProfiles()
        {
            InitializeComponent();
            textBoxFirstName.TextChanged += TextBox_UppercaseFirstLetter;
            textBoxLastName.TextChanged += TextBox_UppercaseFirstLetter;
            textBoxEmail.TextChanged += TextBox_UppercaseFirstLetter;
            maskedTextBoxContactNumber.TextChanged += TextBox_UppercaseFirstLetter;

            string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Reservify.accdb");
            con = new OleDbConnection($@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={dbPath};Persist Security Info=False;");


            flickerTimer = new Timer();
            flickerTimer.Interval = FlickerInterval; // Set interval for flicker
            flickerTimer.Tick += FlickerTimer_Tick;

            successTimer.Tick += SuccessTimer_Tick;

            LoadUserTypes();
            textBoxFirstName.TextChanged += (s, e) => FilterUsers();
            textBoxLastName.TextChanged += (s, e) => FilterUsers();
            textBoxEmail.TextChanged += (s, e) => FilterUsers();
            maskedTextBoxContactNumber.TextChanged += (s, e) => FilterUsers();
            comboBoxUserType.SelectedIndexChanged += (s, e) => FilterUsers();


            PopulateUserProfilesPanel(panelProfiles, GetUsersFromDatabase());
        }

       

        private void TextBox_UppercaseFirstLetter(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null && textBox.Text.Length > 0)
            {
                string newText = char.ToUpper(textBox.Text[0]) + textBox.Text.Substring(1);
                textBox.TextChanged -= TextBox_UppercaseFirstLetter;
                textBox.Text = newText;
                textBox.SelectionStart = textBox.Text.Length;
                textBox.TextChanged += TextBox_UppercaseFirstLetter;
            }
        }




        private void FilterUsers()
        {
            // Get the input from the filter fields
            string firstNameFilter = textBoxFirstName.Text.Trim();
            string lastNameFilter = textBoxLastName.Text.Trim();
            string emailFilter = textBoxEmail.Text.Trim();

            // Normalize contact number by removing non-digit characters
            string contactNumberFilter = new string(maskedTextBoxContactNumber.Text.Where(char.IsDigit).ToArray());

            // Get the selected UserTypeID from the combo box
            int? selectedUserTypeId = comboBoxUserType.SelectedIndex >= 0 ? comboBoxUserType.SelectedIndex + 1 : (int?)null;

            // Clear the panel before repopulating
            panelProfiles.Controls.Clear();

            // Fetch filtered users from the database
            List<User> users = GetFilteredUsersFromDatabase(firstNameFilter, lastNameFilter, emailFilter, contactNumberFilter, selectedUserTypeId);

            // Populate the panel with the filtered users
            PopulateUserProfilesPanel(panelProfiles, users);
        }





        private List<User> GetFilteredUsersFromDatabase(string firstName, string lastName, string email, string contactNumber, int? userTypeId)
        {
            List<User> users = new List<User>();
            try
            {
                con.Open();

                // Start building the query
                var query = new StringBuilder("SELECT UserID, FirstName, LastName, Email, ContactNumber, UserTypeID FROM [User] WHERE 1=1");

                // Add filters based on input
                if (!string.IsNullOrEmpty(firstName))
                    query.Append(" AND FirstName LIKE ?");

                if (!string.IsNullOrEmpty(lastName))
                    query.Append(" AND LastName LIKE ?");

                if (!string.IsNullOrEmpty(email))
                    query.Append(" AND Email LIKE ?");

                if (!string.IsNullOrEmpty(contactNumber))
                    query.Append(" AND ContactNumber LIKE ?");

                if (userTypeId.HasValue)
                    query.Append(" AND UserTypeID = ?");

                using (var cmd = new OleDbCommand(query.ToString(), con))
                {
                    int parameterIndex = 1;

                    // Set the parameters
                    if (!string.IsNullOrEmpty(firstName))
                        cmd.Parameters.AddWithValue("?", $"%{firstName}%");

                    if (!string.IsNullOrEmpty(lastName))
                        cmd.Parameters.AddWithValue("?", $"%{lastName}%");

                    if (!string.IsNullOrEmpty(email))
                        cmd.Parameters.AddWithValue("?", $"%{email}%");

                    if (!string.IsNullOrEmpty(contactNumber))
                        cmd.Parameters.AddWithValue("?", $"%{contactNumber}%");

                    if (userTypeId.HasValue)
                        cmd.Parameters.AddWithValue("?", userTypeId.Value);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var user = new User
                            {
                                UserId = reader.GetInt32(reader.GetOrdinal("UserID")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                Email = reader.GetString(reader.GetOrdinal("Email")),
                                ContactNumber = reader.GetString(reader.GetOrdinal("ContactNumber")),
                                UserTypeId = reader.GetInt32(reader.GetOrdinal("UserTypeID"))
                            };
                            users.Add(user);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error retrieving filtered users: " + ex.Message); // Print error to console
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
            return users;
        }

        private void LoadUserTypes()
        {
            try
            {
                con.Open();
                string query = "SELECT UserTypeID, UserTypeName FROM UserType"; // Include UserTypeID

                using (var cmd = new OleDbCommand(query, con))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        comboBoxUserType.Items.Clear();
                        while (reader.Read())
                        {
                            // Create a new instance of a class to hold UserTypeID and Name
                            comboBoxUserType.Items.Add(new UserType
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1)
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading user types: " + ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }

        // Define a class to hold UserType information
        public class UserType
        {
            public int Id { get; set; }
            public string Name { get; set; }

            public override string ToString() => Name; // Override ToString to display the Name
        }






        private void labelHomeRedirect_Click(object sender, EventArgs e)
        {
            var mainMenu = new MainMenuForm();
            mainMenu.Show();
            this.Hide(); // Hide update profile form
        }
        private int lastSelectedUserId = -1; // To track the last selected user's ID

        private void PopulateUserProfilesPanel(Panel panel, List<User> users)
        {
            panel.Controls.Clear(); // Clear any existing controls
            panel.Size = new Size(317, 409);
            int yOffset = 10; // Initial offset for card positioning

            foreach (var user in users)
            {
                string userType = GetUserTypeById(user.UserTypeId);

                var userProfileCard = new UserProfileCard(user.UserId, user.FirstName, user.LastName, user.Email, user.ContactNumber, userType);

                // Highlight the last selected user card if it matches
                if (user.UserId == lastSelectedUserId)
                {
                    userProfileCard.BackColor = Color.DeepSkyBlue; // Highlight color for the last selected user
                }

                userProfileCard.SelectClicked += (s, e) =>
                {
                    lastSelectedUserId = userProfileCard.UserId; // Store the last selected UserId
                    LoadUserData(lastSelectedUserId); // Load user data
                                                      // Set user details in text boxes
                    textBoxFirstName.Text = user.FirstName;
                    textBoxLastName.Text = user.LastName;
                    textBoxEmail.Text = user.Email;
                    maskedTextBoxContactNumber.Text = user.ContactNumber;
                    comboBoxUserType.SelectedItem = userType; // Assuming userType is available in the combo box

                    // Reset color for all cards
                    foreach (Control ctrl in panel.Controls)
                    {
                        if (ctrl is UserProfileCard card)
                        {
                            card.BackColor = SystemColors.Control; // Default color
                        }
                    }
                    userProfileCard.BackColor = Color.DeepSkyBlue; // Highlight the selected card
                };

                // Handle delete button click
                userProfileCard.DeleteClicked += (s, e) =>
                {
                    OnUserDeleteClicked(userProfileCard, users); // Pass the users list for deletion
                };

                // Add the user profile card to the panel
                panel.Controls.Add(userProfileCard);
                userProfileCard.Location = new Point(10, yOffset); // Set position
                yOffset += userProfileCard.Height + 10; // Maintain spacing between cards
            }

            panel.AutoScroll = (yOffset > panel.Height); // Enable scrolling if needed
        }

        private void OnUserDeleteClicked(UserProfileCard userCard, List<User> users)
        {
            if (userCard != null)
            {
                // Show confirmation dialog
                var result = MessageBox.Show(
                    "Are you sure you want to delete this user?",
                    "Confirm Deletion",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                // If the user clicked "Yes", proceed with deletion
                if (result == DialogResult.Yes)
                {
                    // Remove the card from the panel
                    panelProfiles.Controls.Remove(userCard);
                    userCard.Dispose(); // Dispose of the card if no longer needed

                    // Delete from the user list
                    var userToRemove = users.FirstOrDefault(u => u.UserId == userCard.UserId);
                    if (userToRemove != null)
                    {
                        users.Remove(userToRemove);
                    }

                    // Delete from the database using the existing connection
                    DeleteUserFromDatabase(userCard.UserId); // Pass the UserId for deletion

                    // Refresh the panel to reflect the changes
                    PopulateUserProfilesPanel(panelProfiles, users); // Re-populate the panel
                }
            }
        }




        private void DeleteUserFromDatabase(int userId)
        {
            string query = "DELETE FROM [User] WHERE UserID = @UserID"; // Use brackets for reserved keywords

            try
            {
                // Open the connection
                con.Open();

                using (var command = new OleDbCommand(query, con))
                {
                    command.Parameters.AddWithValue("@UserID", userId);
                    command.ExecuteNonQuery(); // Execute the delete command
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting user: " + ex.Message);
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


        private List<User> GetUsersFromDatabase()
        {
            List<User> users = new List<User>();
            try
            {
                con.Open();
                string query = "SELECT UserID, FirstName, LastName, Email, ContactNumber, UserTypeID FROM [User]"; // Use brackets to avoid conflicts with reserved words
                using (var cmd = new OleDbCommand(query, con))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var user = new User
                            {
                                UserId = reader.GetInt32(reader.GetOrdinal("UserID")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                Email = reader.GetString(reader.GetOrdinal("Email")),
                                ContactNumber = reader.GetString(reader.GetOrdinal("ContactNumber")),
                                UserTypeId = reader.GetInt32(reader.GetOrdinal("UserTypeID"))
                            };
                            users.Add(user);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error retrieving users: " + ex.Message); // Print error to console
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
            return users;
        }


        private string GetUserTypeById(int userTypeId)
        {
            string userType = string.Empty;
            try
            {
                con.Open();
                string query = "SELECT UserTypeName FROM UserType WHERE UserTypeID = ?";
                using (var cmd = new OleDbCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("?", userTypeId);
                    userType = cmd.ExecuteScalar()?.ToString() ?? "Unknown"; // Default to "Unknown" if not found
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error retrieving user type: " + ex.Message); // Print error to console
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
            return userType;
        }

        private void buttonTogglePasswordUpdate_Click(object sender, EventArgs e)
        {
            if (textBoxPassword.PasswordChar == '*')
            {
                textBoxPassword.PasswordChar = '\0'; // Show password
                textBoxConfirmPassword.PasswordChar = '\0'; // Show confirm password
            }
            else
            {
                textBoxPassword.PasswordChar = '*'; // Hide password
                textBoxConfirmPassword.PasswordChar = '*'; // Hide confirm password
            }
        }

        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs())
            {
                return;
            }

            var updates = new List<string>();
            var parameters = new List<OleDbParameter>();

            // Get the UserId from the currently selected UserProfileCard
            int selectedUserId = lastSelectedUserId; // Create a method to get this ID
            //MessageBox.Show(lastSelectedUserId.ToString());
            if (selectedUserId < 0)
            {
                HighlightValidationError(panelProfiles); // Assuming you have a panel to highlight

               MessageBox.Show("No user is selected for update.");
                return;
            }

            string currentFirstName = textBoxFirstName.Text.Trim();
            string currentLastName = textBoxLastName.Text.Trim();
            string currentEmail = textBoxEmail.Text.Trim();
            string currentContactNumber = maskedTextBoxContactNumber.Text.Trim();

            // Get the selected UserTypeID from the ComboBox
            int currentUserTypeId = comboBoxUserType.SelectedItem is UserType selectedUserType ? selectedUserType.Id : -1;

            string newPassword = textBoxPassword.Text.Trim();

            if (currentFirstName != initialFirstName)
            {
                updates.Add("[FirstName] = @FirstName");
                parameters.Add(new OleDbParameter("@FirstName", currentFirstName));
            }

            if (currentLastName != initialLastName)
            {
                updates.Add("[LastName] = @LastName");
                parameters.Add(new OleDbParameter("@LastName", currentLastName));
            }

            if (currentEmail != initialEmail)
            {
                updates.Add("[Email] = @Email");
                parameters.Add(new OleDbParameter("@Email", currentEmail));
            }

            if (currentContactNumber != initialContactNumber)
            {
                updates.Add("[ContactNumber] = @ContactNumber");
                parameters.Add(new OleDbParameter("@ContactNumber", currentContactNumber));
            }

            if (currentUserTypeId != initialUserTypeId)
            {
                updates.Add("[UserTypeID] = @UserTypeID");
                parameters.Add(new OleDbParameter("@UserTypeID", currentUserTypeId));
            }

            if (!string.IsNullOrWhiteSpace(newPassword))
            {
                if (newPassword != textBoxConfirmPassword.Text.Trim())
                {
                    MessageBox.Show("Passwords do not match.");
                    return;
                }
                string hashedPassword = HashPassword(newPassword);
                updates.Add("[Password] = @Password");
                parameters.Add(new OleDbParameter("@Password", hashedPassword));
            }

            if (updates.Count > 0)
            {
                try
                {
                    con.Open();
                    string updateQuery = "UPDATE [User] SET " + string.Join(", ", updates) + " WHERE [UserID] = @UserID";
                    parameters.Add(new OleDbParameter("@UserID", selectedUserId)); // Use selectedUserId here

                    using (var cmd = new OleDbCommand(updateQuery, con))
                    {
                        cmd.Parameters.AddRange(parameters.ToArray());
                        int result = cmd.ExecuteNonQuery();
                        if (result > 0)
                        {
                            Console.WriteLine("Profile updated successfully!");
                            HighlightSuccess(panelProfiles);
                           // MessageBox.Show("Profile updated successfully!"); // Show message to user
                        }
                        else
                        {
                            Console.WriteLine("No changes detected.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
                finally
                {
                    if (con != null && con.State == ConnectionState.Open)
                    {
                        con.Close();
                    }
                }
            }
            else
            {
                Console.WriteLine("No data was provided to be updated.");
            }
            PopulateUserProfilesPanel(panelProfiles, GetUsersFromDatabase());
            // Load user data again after update
            //LoadUserData(selectedUserId); // Load data using the selected user's ID
        }

        private void textBoxPasswordUpdate_TextChanged(object sender, EventArgs e)
        {
            string password = textBoxPassword.Text;
            string strength = GetPasswordStrength(password);
            labelPasswordStrength.Text = strength;

            if (strength == "Weak Encryption")
                labelPasswordStrength.ForeColor = Color.Red;
            else if (strength == "Medium Encryption")
                labelPasswordStrength.ForeColor = Color.Orange;
            else if (strength == "Strong Encryption")
                labelPasswordStrength.ForeColor = Color.Green;
        }

        private string GetPasswordStrength(string password)
        {
            bool hasUpperCase = password.Any(char.IsUpper);
            bool hasLowerCase = password.Any(char.IsLower);
            bool hasDigit = password.Any(char.IsDigit);
            bool hasSpecialChar = password.Any(c => !char.IsLetterOrDigit(c));

            int strengthScore = Convert.ToInt32(hasUpperCase) + Convert.ToInt32(hasLowerCase) +
                                Convert.ToInt32(hasDigit) + Convert.ToInt32(hasSpecialChar);

            switch (strengthScore)
            {
                case 0:
                case 1:
                    return "Weak Encryption";
                case 2:
                    return "Medium Encryption";
                case 3:
                case 4:
                    return "Strong Encryption";
                default:
                    return "Weak Encryption";
            }
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



        private void HighlightSuccess(Panel panel)
        {
            Color originalColor = panel.BackColor; // Store the original color
            panel.BackColor = Color.DeepSkyBlue; // Change to green for success feedback

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


        private void HighlightUpdatedUserProfileCard(int userId)
        {
            foreach (Control control in panelProfiles.Controls)
            {
                if (control is UserProfileCard userProfileCard && userProfileCard.UserId == userId)
                {
                    userProfileCard.SelectCard(); // Change the card color to indicate selection
                    break;
                }
            }
        }



        // Helper method to get the selected user's ID
        private int GetSelectedUserId()
        {
            // Logic to get the currently selected UserProfileCard and return its UserId
            foreach (Control control in panelProfiles.Controls)
            {
                if (control is UserProfileCard userProfileCard && userProfileCard.IsSelected)
                {
                    return userProfileCard.UserId;
                }
            }
            return -1; // Return -1 if no user is selected
        }


        private void LoadUserData(int userId)
        {
            if (userId <= 0)
            {
                Console.WriteLine("Invalid user ID.");
                return;
            }

            using (var connection = new OleDbConnection(con.ConnectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT FirstName, LastName, Email, ContactNumber, UserTypeID FROM [User] WHERE UserID = @UserID";

                    using (var cmd = new OleDbCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@UserID", userId);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                initialFirstName = textBoxFirstName.Text = reader["FirstName"].ToString();
                                initialLastName = textBoxLastName.Text = reader["LastName"].ToString();
                                initialEmail = textBoxEmail.Text = reader["Email"].ToString();
                                initialContactNumber = maskedTextBoxContactNumber.Text = reader["ContactNumber"].ToString();

                                if (initialContactNumber.StartsWith("0"))
                                {
                                    initialContactNumber = "+27 (0) " + initialContactNumber.Substring(1);
                                }

                                maskedTextBoxContactNumber.Text = initialContactNumber;
                                initialUserTypeId = Convert.ToInt32(reader["UserTypeID"]);
                                comboBoxUserType.SelectedIndex = initialUserTypeId - 1;
                               // comboBoxUserType.Enabled = false;
                            }
                            else
                            {
                                Console.WriteLine("User not found.");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error loading user data: " + ex.Message);
                }
            }
        }


        private string HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(textBoxFirstName.Text) ||
                string.IsNullOrWhiteSpace(textBoxLastName.Text) ||
                string.IsNullOrWhiteSpace(textBoxEmail.Text) ||
                string.IsNullOrWhiteSpace(maskedTextBoxContactNumber.Text) ||
                comboBoxUserType.SelectedItem == null)
            {
                HighlightValidationError(panelProfiles);
                //MessageBox.Show("All fields must be filled out.");
                return false;
            }

            if (!IsValidEmail(textBoxEmail.Text))
            {
                MessageBox.Show("Invalid email format.");
                return false;
            }

            if (!IsValidPhoneNumber(maskedTextBoxContactNumber.Text))
            {
                MessageBox.Show("Contact number must start with +27 followed by 9 digits.");
                return false;
            }

            return true;
        }

        private bool IsValidEmail(string email)
        {
            return email.Contains("@") && email.IndexOf("@") < email.Length - 1;
        }

        private bool IsValidPhoneNumber(string phoneNumber)
        {
            return Regex.IsMatch(phoneNumber, @"^\+27 \(0\) \d{3} \d{3} \d{3}$");
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            // Reset text boxes to empty
            textBoxFirstName.Text = string.Empty;
            textBoxLastName.Text = string.Empty;
            textBoxEmail.Text = string.Empty;
            maskedTextBoxContactNumber.Text = string.Empty;

            // Reset the combo box to its default (first item or empty)
            comboBoxUserType.SelectedIndex = -1; // Or set to 0 if you want to select the first item

            // Optionally reset other controls, like password fields
            textBoxPassword.Text = string.Empty;
            textBoxConfirmPassword.Text = string.Empty;

            labelPasswordStrength.Text = string.Empty;

            // Clear the last selected user ID
            lastSelectedUserId = -1; // Reset to default value
            // Reset the panel's background color if it was changed due to validation
            panelProfiles.BackColor = SystemColors.Control; // Reset to default color

            PopulateUserProfilesPanel(panelProfiles, GetUsersFromDatabase());

        }

        private void buttonTogglePassword_Click(object sender, EventArgs e)
        {

            if (textBoxPassword.PasswordChar == '*')
            {
                textBoxPassword.PasswordChar = '\0';
                textBoxConfirmPassword.PasswordChar = '\0';
            }
            else
            {
                textBoxPassword.PasswordChar = '*';
                textBoxConfirmPassword.PasswordChar = '*';
            }
        }

        
    }

    public class User
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string ContactNumber { get; set; }
        public int UserTypeId { get; set; }
    }
}
