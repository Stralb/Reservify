using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.IO;
using System.Linq;
using System.Data;

namespace Reservify_Techfusion
{
    public partial class UpdateProfileForm : Form
    {
        private OleDbConnection con;

        // Local variables to hold initial user data
        private string initialFirstName;
        private string initialLastName;
        private string initialEmail;
        private string initialContactNumber;
        private int initialUserTypeId;

        public UpdateProfileForm()
        {
            InitializeComponent();
            textBoxFirstName.TextChanged += TextBox_UppercaseFirstLetter;
            textBoxLastName.TextChanged += TextBox_UppercaseFirstLetter;
            textBoxEmail.TextChanged += TextBox_UppercaseFirstLetter;
            maskedTextBoxContactNumber.TextChanged += TextBox_UppercaseFirstLetter;

            string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Reservify.accdb");
            con = new OleDbConnection($@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={dbPath};Persist Security Info=False;");

            LoadUserTypes();
            LoadUserData();
        }

        private void LoadUserTypes()
        {
            try
            {
                con.Open();
                string query = "SELECT UserTypeName FROM UserType";

                using (var cmd = new OleDbCommand(query, con))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        comboBoxUserType.Items.Clear();
                        while (reader.Read())
                        {
                            comboBoxUserType.Items.Add(reader["UserTypeName"].ToString());
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
                if (con != null && con.State == ConnectionState.Open)
                {
                    con.Close();
                }
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
            int userId = UserSession.CurrentUserId;

            string currentFirstName = textBoxFirstName.Text.Trim();
            string currentLastName = textBoxLastName.Text.Trim();
            string currentEmail = textBoxEmail.Text.Trim();
            string currentContactNumber = maskedTextBoxContactNumber.Text.Trim();
            int currentUserTypeId = comboBoxUserType.SelectedIndex + 1;
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
                    parameters.Add(new OleDbParameter("@UserID", userId));

                    using (var cmd = new OleDbCommand(updateQuery, con))
                    {
                        cmd.Parameters.AddRange(parameters.ToArray());
                        int result = cmd.ExecuteNonQuery();
                        if (result > 0)
                        {
                            Console.WriteLine("Profile updated successfully!");
                            MessageBox.Show("Profile updated successfully!"); // Show message to user
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

            // Load user data again after update
            LoadUserData();
        }

        private void LoadUserData()
        {
            int userId = UserSession.CurrentUserId;

            if (userId <= 0)
            {
                Console.WriteLine("No user is currently logged in.");
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
                                comboBoxUserType.Enabled = false;
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


        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(textBoxFirstName.Text) ||
                string.IsNullOrWhiteSpace(textBoxLastName.Text) ||
                string.IsNullOrWhiteSpace(textBoxEmail.Text) ||
                string.IsNullOrWhiteSpace(maskedTextBoxContactNumber.Text) ||
                comboBoxUserType.SelectedItem == null)
            {
                MessageBox.Show("All fields must be filled out.");
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

        private void labelHomeRedirect_Click(object sender, EventArgs e)
        {
            var mainMenu = new MainMenuForm();
            mainMenu.Show();
            this.Hide();
        }

        private string HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
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

        private void buttonTogglePasswordUpdate_Click(object sender, EventArgs e)
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
    }
}
