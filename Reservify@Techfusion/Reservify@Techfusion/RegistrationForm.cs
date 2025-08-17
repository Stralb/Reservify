using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reservify_Techfusion
{
    public partial class RegistrationForm : Form
    {
        private OleDbConnection con;
        public RegistrationForm()
        {
            InitializeComponent();

            textBoxFirstName.TextChanged += TextBox_UppercaseFirstLetter;
            textBoxLastName.TextChanged += TextBox_UppercaseFirstLetter;
            maskedTextBoxEmail.TextChanged += TextBox_UppercaseFirstLetter;
            maskedTextBoxContactNumber.TextChanged += TextBox_UppercaseFirstLetter;
            string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Reservify.accdb");
            con = new OleDbConnection($@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={dbPath};Persist Security Info=False;");

            LoadUserTypes(); // Load user types into the combo box
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
                        comboBoxUserType.Items.Clear(); // Clear existing items
                        while (reader.Read())
                        {
                            comboBoxUserType.Items.Add(reader["UserTypeName"].ToString());
                        }
                    }
                }

               
                // Set the selected index to the first valid option
                if (comboBoxUserType.Items.Count > 0)
                    comboBoxUserType.SelectedIndex = 0;

                // Enable drawing
                comboBoxUserType.DrawMode = DrawMode.OwnerDrawFixed;
                comboBoxUserType.DrawItem += new DrawItemEventHandler(comboBoxUserType_DrawItem);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading user types: " + ex.Message);
            }
            finally
            {
                con.Close(); // Close the connection
            }
        }



        private void comboBoxUserType_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground(); // Clear the background

            // Ensure there are items in the ComboBox and the index is valid
            if (e.Index < 0 || e.Index >= comboBoxUserType.Items.Count)
            {
                return; // Exit if the index is out of range
            }

            // Check if the item is the last one
            if (e.Index == comboBoxUserType.Items.Count - 1)
            {
                // Draw the disabled item in gray
                e.Graphics.DrawString(comboBoxUserType.Items[e.Index].ToString(),
                                      e.Font,
                                      Brushes.Gray, // Disabled color
                                      e.Bounds);
            }
            else
            {
                // Draw normal items
                e.Graphics.DrawString(comboBoxUserType.Items[e.Index].ToString(),
                                      e.Font,
                                      Brushes.Black, // Normal color
                                      e.Bounds);
            }

            // Check if the item is selected
            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                // Avoid default blue highlight
                e.Graphics.FillRectangle(new SolidBrush(Color.LightGray), e.Bounds);
                e.Graphics.DrawString(comboBoxUserType.Items[e.Index].ToString(),
                                      e.Font,
                                      Brushes.Black, // Color for selected item
                                      e.Bounds);
            }

            e.DrawFocusRectangle(); // Draw focus rectangle if needed
        }

        private void buttonRegister_Click(object sender, EventArgs e)
        {
            // Validate inputs
            if (!ValidateInputs())
            {
                return; // Exit if validation fails
            }

            try
            {
                con.Open();

                // Check if the email already exists
                string checkEmailQuery = "SELECT COUNT(*) FROM [User] WHERE Email = @Email"; // Use brackets for reserved words
                using (var checkCmd = new OleDbCommand(checkEmailQuery, con))
                {
                    checkCmd.Parameters.AddWithValue("@Email", maskedTextBoxEmail.Text.Trim());
                    int emailExists = (int)checkCmd.ExecuteScalar();

                    if (emailExists > 0)
                    {
                        MessageBox.Show("An account with this email already exists.");
                        return; // Exit if the email is already registered
                    }
                }

                // Hash the password before storing
                string hashedPassword = HashPassword(textBoxPassword.Text.Trim());
                // Get UserTypeID based on the selected index
                int userTypeId = comboBoxUserType.SelectedIndex + 1; // Assuming index starts at 0, adjust to match DB

                // Format phone number for database
                string contactNumber = maskedTextBoxContactNumber.Text.Trim();

                // Remove spaces and parentheses for database storage
                contactNumber = contactNumber.Replace(" ", "").Replace("(", "").Replace(")", "");

                if (contactNumber.StartsWith("0"))
                {
                    contactNumber = "+27" + contactNumber.Substring(1); // Replace leading 0 with +27
                }
                else if (!contactNumber.StartsWith("+27"))
                {
                    MessageBox.Show("Contact number must start with +27.");
                    return;
                }

                // Insert new user
                string query = "INSERT INTO [User] ([FirstName], [LastName], [Email], [Password], [ContactNumber], [UserTypeID]) VALUES (@FirstName, @LastName, @Email, @Password, @ContactNumber, @UserTypeID)";
                using (var cmd = new OleDbCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@FirstName", textBoxFirstName.Text.Trim());
                    cmd.Parameters.AddWithValue("@LastName", textBoxLastName.Text.Trim());
                    cmd.Parameters.AddWithValue("@Email", maskedTextBoxEmail.Text.Trim());
                    cmd.Parameters.AddWithValue("@Password", hashedPassword); // Use hashed password
                    cmd.Parameters.AddWithValue("@ContactNumber", maskedTextBoxContactNumber.Text.Trim());
                    cmd.Parameters.AddWithValue("@UserTypeID", userTypeId); // Use correct UserTypeID

                    int result = cmd.ExecuteNonQuery();
                    if (result > 0)
                    {
                        string getIdQuery = "SELECT UserID FROM [User] WHERE Email = @Email AND Password = @Password";
                        using (var idCmd = new OleDbCommand(getIdQuery, con))
                        {
                            idCmd.Parameters.AddWithValue("@Email", maskedTextBoxEmail.Text.Trim());
                            idCmd.Parameters.AddWithValue("@Password", hashedPassword); // Use hashed password
                            int userId = Convert.ToInt32(idCmd.ExecuteScalar());
                            UserSession.SetUserId(userId); // Save the user ID in the UserSession
                        }


                        MessageBox.Show("Registration successful!");

                        // Clear fields after successful registration
                        textBoxFirstName.Clear();
                        textBoxLastName.Clear();
                        maskedTextBoxEmail.Clear();
                        textBoxPassword.Clear();
                        textBoxConfirmPassword.Clear();
                        maskedTextBoxContactNumber.Clear();
                        comboBoxUserType.SelectedIndex = -1; // Reset dropdown
                        labelPasswordStrength.Text = string.Empty;

                        MainMenuForm mainMenu = new MainMenuForm();
                        mainMenu.Show();
                        this.Hide(); // Hide registration form
                    }
                    else
                    {
                        MessageBox.Show("Registration failed. Please try again.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        private string HashPassword(string password)
        {
            // Simple example of hashing (you can use a more secure method)
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
                string.IsNullOrWhiteSpace(maskedTextBoxEmail.Text) ||
                string.IsNullOrWhiteSpace(textBoxPassword.Text) ||
                string.IsNullOrWhiteSpace(textBoxConfirmPassword.Text) ||
                string.IsNullOrWhiteSpace(maskedTextBoxContactNumber.Text) ||
                comboBoxUserType.SelectedItem == null)
            {
                MessageBox.Show("All fields must be filled out.");
                return false;
            }

            if (!IsValidEmail(maskedTextBoxEmail.Text))
            {
                MessageBox.Show("Invalid email format.");
                return false;
            }

            if (!IsValidPhoneNumber(maskedTextBoxContactNumber.Text))
            {
                MessageBox.Show("Contact number must start with +27 followed by 9 digits.");
                return false;
            }

            if (textBoxPassword.Text != textBoxConfirmPassword.Text)
            {
                MessageBox.Show("Passwords do not match.");
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
            // This regex checks for the format +27 (0) 000 000 000
            return Regex.IsMatch(phoneNumber, @"^\+27 \(0\) \d{3} \d{3} \d{3}$");
        }


        private void labelLoginRedirect_Click(object sender, EventArgs e)
        {
            // Logic to open the login form
            var loginForm = new LoginForm();
            loginForm.Show();
            this.Hide(); // Optionally hide the registration form
        }


        private void textBoxPassword_TextChanged(object sender, EventArgs e)
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


        private void buttonTogglePassword_Click(object sender, EventArgs e)
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

        private void TextBox_UppercaseFirstLetter(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox != null && textBox.Text.Length > 0)
            {
                // Capitalize the first letter and concatenate the rest of the text
                string newText = char.ToUpper(textBox.Text[0]) + textBox.Text.Substring(1);

                // Prevent recursion by removing the event handler temporarily
                textBox.TextChanged -= TextBox_UppercaseFirstLetter;

                textBox.Text = newText;
                textBox.SelectionStart = textBox.Text.Length; // Set cursor to the end

                // Reattach the event handler
                textBox.TextChanged += TextBox_UppercaseFirstLetter;
            }
        }

        private void groupBoxRegistration_Enter(object sender, EventArgs e)
        {

        }
    }
}
