using System;
using System.Drawing;
using System.Windows.Forms;

namespace Reservify_Techfusion
{
    public partial class VenueCard : UserControl
    {
        public event EventHandler SelectClicked;
        public event EventHandler<ImageUploadEventArgs> ImageUploadRequested; // New event for image upload

        public bool IsSelected { get; private set; }
        public int VenueIndex { get; set; }

        // Property to store VenueId
        public int VenueId { get; set; }


        // Property to determine if we're in the image upload form context
        public bool IsImageUploadForm { get; set; } = false;

        public VenueCard(int venueId, string venueName, string venueCategory, int venueCapacity)
        {
            InitializeComponent();
            VenueId = venueId; // Set the VenueId
            SetVenueDetails(venueName, venueCategory, venueCapacity);
            buttonChangeToVenue.Click += ButtonChangeToVenue_Click; // Add event handler for button
        }

        private void SetVenueDetails(string venueName, string venueCategory, int venueCapacity)
        {
            labelVenueName.Text = venueName;
            labelVenueCategory.Text = $"Category:   {venueCategory}";
            labelVenueCapacity.Text = $"Capacity:   {venueCapacity}";
        }

        protected virtual void OnSelectClicked()
        {
            SelectClicked?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnImageUploadRequested()
        {
            ImageUploadRequested?.Invoke(this, new ImageUploadEventArgs { VenueId = this.VenueId });
        }

        public void SelectCard()
        {
            IsSelected = true;
            this.BackColor = Color.DeepSkyBlue; // Highlight color
            buttonChangeToVenue.BackColor = Color.DodgerBlue;
        }

        public void Deselect()
        {
            IsSelected = false;
            this.BackColor = Color.DodgerBlue; // Default control color
            buttonChangeToVenue.BackColor = DefaultBackColor;
        }

        public void SetButtonText(string text)
        {
            buttonChangeToVenue.Text = text; // Set the button text
        }

        public PictureBox VenueImageBox => pictureBoxVenue; // Access to the PictureBox

        public Image VenueImage
        {
            get => pictureBoxVenue.Image; // Get the image from the PictureBox
            set => pictureBoxVenue.Image = value; // Set the image directly to the PictureBox
        }

        private void ButtonChangeToVenue_Click(object sender, EventArgs e)
        {
            OnSelectClicked(); // Trigger selection event
            if (IsImageUploadForm) // Check if we are in the image upload form context
            {
                OnImageUploadRequested(); // Raise the image upload request event
            }
        }
    }

    public class ImageUploadEventArgs : EventArgs
    {
        public int VenueId { get; set; }
    }
}
