using System;
using System.Drawing;
using System.Windows.Forms;

namespace Reservify_Techfusion
{
    public partial class BookingCard : UserControl
    {
        public event EventHandler EditBookingClicked;
        public event EventHandler BookingSelected;
        public event EventHandler DeleteBookingClicked;

        public int BookingIndex { get; set; }
        public bool IsSelected { get; private set; }
        public int BookingID { get; private set; } // Added BookingID property

        public string VenueName => labelVenueName.Text;
        public string VenueCategory => labelvenueCategory.Text;
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public string StartTime => labelStartTime.Text;
        public string EndTime => labelEndTime.Text;
        public int VenueCapacity => int.Parse(labelCapacity.Text);

        public BookingCard(string venueCategory, string venueName, DateTime startDate, DateTime endDate, string startTime, string endTime, int capacity, int bookingID)
        {
            InitializeComponent();
            SetBookingDetails(venueCategory, venueName, startDate, endDate, startTime, endTime, capacity);
            BookingID = bookingID; // Store the BookingID

            // Button click event for editing
            buttonEditBooking.Click += (sender, e) =>
            {
                HighlightCard(); // Highlight this card
                EditBookingClicked?.Invoke(this, EventArgs.Empty); // Raise the event
            };

            // Button click event for deleting
            buttonDeleteBooking.Click += (sender, e) =>
            {
                DeleteBookingClicked?.Invoke(this, EventArgs.Empty); // Raise the delete event
            };
        }

        private void SetBookingDetails(string venueCategory, string venueName, DateTime startDate, DateTime endDate, string startTime, string endTime, int capacity)
        {
            labelvenueCategory.Text = venueCategory;
            labelVenueName.Text = venueName;
            StartDate = startDate;
            EndDate = endDate;
            labelStartDate.Text = startDate.ToShortDateString();
            labelEndDate.Text = endDate.ToShortDateString();
            labelStartTime.Text = startTime;
            labelEndTime.Text = endTime;
            labelCapacity.Text = capacity.ToString();
        }

        public void HighlightCard()
        {
            this.BackColor = Color.DarkTurquoise; // Highlight color
            buttonEditBooking.BackColor = Color.DeepSkyBlue;
        }

        public void Deselect()
        {
            IsSelected = false; // Set selection state to false
            this.BackColor = Color.DeepSkyBlue; // Reset to default control color
            buttonEditBooking.BackColor = DefaultBackColor;
        }

        public void SelectCard()
        {
            IsSelected = true; // Mark as selected
            HighlightCard(); // Highlight the card
        }
    }
}
