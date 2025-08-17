using System;
using System.Drawing;
using System.Windows.Forms;

namespace Reservify_Techfusion
{
    public partial class UserProfileCard : UserControl
    {
        public event EventHandler SelectClicked;
        public event EventHandler DeleteClicked; // Event for delete action

        public bool IsSelected { get; private set; }
        public int UserIndex { get; set; }
        public int UserId { get; set; } // Property to store UserId

        public UserProfileCard(int userId, string firstName, string lastName, string email, string contactNumber, string userType)
        {
            InitializeComponent();
            UserId = userId; // Set the UserId
            SetUserProfileDetails(firstName, lastName, email, contactNumber, userType);

            buttonSelectUser.Click += ButtonSelectUser_Click; // Add event handler for select button
            buttonDeleteUser.Click += ButtonDeleteUser_Click; // Add event handler for delete button
        }

        private void SetUserProfileDetails(string firstName, string lastName, string email, string contactNumber, string userType)
        {
            labelFirstName.Text = $"First Name: {firstName}";
            labelLastName.Text = $"Last Name: {lastName}";
            labelEmail.Text = $"Email: {email}";
            labelContactNumber.Text = $"Contact: {contactNumber}";
            labelUserType.Text = $"User Type: {userType}";
        }

        protected virtual void OnSelectClicked()
        {
            SelectClicked?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnDeleteClicked() // Method to raise delete event
        {
            DeleteClicked?.Invoke(this, EventArgs.Empty);
        }

        public void SelectCard()
        {
            IsSelected = true;
            this.BackColor = Color.Green; // Highlight color
            buttonSelectUser.BackColor = Color.DarkTurquoise;
        }

        public void Deselect()
        {
            IsSelected = false;
            this.BackColor = DefaultBackColor; // Default control color
            buttonSelectUser.BackColor = DefaultBackColor;
        }

        private void ButtonSelectUser_Click(object sender, EventArgs e)
        {
            OnSelectClicked(); // Trigger selection event
        }

        private void ButtonDeleteUser_Click(object sender, EventArgs e)
        {
            OnDeleteClicked(); // Trigger delete event
        }
    }
}
