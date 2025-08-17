using System;
using System.Data.OleDb;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Reservify_Techfusion
{
    public partial class LoginForm : Form
    {
        private OleDbConnection con;

        public LoginForm()
        {
            InitializeComponent();
            textBoxEmail.TextChanged += TextBox_UppercaseFirstLetter;
            string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Reservify.accdb");
            con = new OleDbConnection($@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={dbPath};Persist Security Info=False;");
        }

            private void buttonLogin_Click(object sender, EventArgs e)
        {
            string email = textBoxEmail.Text.Trim();
            string password = textBoxPassword.Text.Trim();

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please enter both email and password.");
                return;
            }

            if (!IsValidEmail(email))
            {
                MessageBox.Show("Invalid email format.");
                return;
            }

            try
            {
                con.Open();

                // Hash the entered password to compare with the stored hash
                string hashedPassword = HashPassword(password);

                // Check if the email and password match
                string query = "SELECT UserID FROM [User] WHERE Email = @Email AND Password = @Password";
                using (var cmd = new OleDbCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Password", hashedPassword);

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        // Successful login
                        int userId = Convert.ToInt32(result);
                        UserSession.SetUserId(userId); // Save the user ID

                        //MessageBox.Show("Login successful!");

                        // Clear fields after successful login
                        textBoxEmail.Clear();
                        textBoxPassword.Clear();

                        MainMenuForm mainMenu = new MainMenuForm();
                        mainMenu.Show();
                        this.Hide(); // Hide login form
                    }
                    else
                    {
                        MessageBox.Show("Invalid email or password.");
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
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }

        private bool IsValidEmail(string email)
        {
            // Simple regex for email validation
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        private void labelRegister_Click(object sender, EventArgs e)
        {
            var registrationForm = new RegistrationForm();
            registrationForm.Show();
            this.Hide(); // Optionally hide the login form
        }

        private void buttonTogglePassword_Click(object sender, EventArgs e)
        {
            if (textBoxPassword.PasswordChar == '*')
            {
                textBoxPassword.PasswordChar = '\0'; // Show password
                buttonTogglePassword.Text = "🔒"; // Change icon to lock
            }
            else
            {
                textBoxPassword.PasswordChar = '*'; // Hide password
                buttonTogglePassword.Text = "👁️"; // Change icon to eye
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
    }
}