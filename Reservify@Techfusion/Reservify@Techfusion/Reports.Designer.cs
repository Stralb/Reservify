using Microsoft.Reporting.WinForms;
using System;
using System.Data;
using System.Windows.Forms;

namespace Reservify_Techfusion
{
    partial class Reports : Form
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblStartDate = new System.Windows.Forms.Label();
            this.dtpStartDate = new System.Windows.Forms.DateTimePicker();
            this.lblEndDate = new System.Windows.Forms.Label();
            this.dtpEndDate = new System.Windows.Forms.DateTimePicker();
            this.lblVenueName = new System.Windows.Forms.Label();
            this.txtVenueName = new System.Windows.Forms.TextBox();
            this.lblBookingStatus = new System.Windows.Forms.Label();
            this.cmbBookingStatus = new System.Windows.Forms.ComboBox();
            this.lblUserType = new System.Windows.Forms.Label();
            this.cmbUserType = new System.Windows.Forms.ComboBox();
            this.btnGenerateReport = new System.Windows.Forms.Button();
            this.reportViewer = new Microsoft.Reporting.WinForms.ReportViewer();
            this.dataGridViewVenues = new System.Windows.Forms.DataGridView();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewVenues)).BeginInit();
            this.SuspendLayout();
            // 
            // lblStartDate
            // 
            this.lblStartDate.AutoSize = true;
            this.lblStartDate.Location = new System.Drawing.Point(12, 28);
            this.lblStartDate.Name = "lblStartDate";
            this.lblStartDate.Size = new System.Drawing.Size(58, 13);
            this.lblStartDate.TabIndex = 0;
            this.lblStartDate.Text = "Start Date:";
            // 
            // dtpStartDate
            // 
            this.dtpStartDate.Location = new System.Drawing.Point(103, 22);
            this.dtpStartDate.Name = "dtpStartDate";
            this.dtpStartDate.Size = new System.Drawing.Size(200, 20);
            this.dtpStartDate.TabIndex = 1;
            // 
            // lblEndDate
            // 
            this.lblEndDate.AutoSize = true;
            this.lblEndDate.Location = new System.Drawing.Point(12, 54);
            this.lblEndDate.Name = "lblEndDate";
            this.lblEndDate.Size = new System.Drawing.Size(55, 13);
            this.lblEndDate.TabIndex = 2;
            this.lblEndDate.Text = "End Date:";
            // 
            // dtpEndDate
            // 
            this.dtpEndDate.Location = new System.Drawing.Point(103, 51);
            this.dtpEndDate.Name = "dtpEndDate";
            this.dtpEndDate.Size = new System.Drawing.Size(200, 20);
            this.dtpEndDate.TabIndex = 3;
            // 
            // lblVenueName
            // 
            this.lblVenueName.AutoSize = true;
            this.lblVenueName.Location = new System.Drawing.Point(12, 85);
            this.lblVenueName.Name = "lblVenueName";
            this.lblVenueName.Size = new System.Drawing.Size(72, 13);
            this.lblVenueName.TabIndex = 4;
            this.lblVenueName.Text = "Venue Name:";
            // 
            // txtVenueName
            // 
            this.txtVenueName.Location = new System.Drawing.Point(103, 82);
            this.txtVenueName.Name = "txtVenueName";
            this.txtVenueName.Size = new System.Drawing.Size(200, 20);
            this.txtVenueName.TabIndex = 5;
            // 
            // lblBookingStatus
            // 
            this.lblBookingStatus.AutoSize = true;
            this.lblBookingStatus.Location = new System.Drawing.Point(11, 115);
            this.lblBookingStatus.Name = "lblBookingStatus";
            this.lblBookingStatus.Size = new System.Drawing.Size(82, 13);
            this.lblBookingStatus.TabIndex = 6;
            this.lblBookingStatus.Text = "Booking Status:";
            // 
            // cmbBookingStatus
            // 
            this.cmbBookingStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBookingStatus.FormattingEnabled = true;
            this.cmbBookingStatus.Items.AddRange(new object[] {
            "All",
            "Confirmed",
            "Pending",
            "Cancelled"});
            this.cmbBookingStatus.Location = new System.Drawing.Point(102, 112);
            this.cmbBookingStatus.Name = "cmbBookingStatus";
            this.cmbBookingStatus.Size = new System.Drawing.Size(200, 21);
            this.cmbBookingStatus.TabIndex = 7;
            // 
            // lblUserType
            // 
            this.lblUserType.AutoSize = true;
            this.lblUserType.Location = new System.Drawing.Point(11, 149);
            this.lblUserType.Name = "lblUserType";
            this.lblUserType.Size = new System.Drawing.Size(59, 13);
            this.lblUserType.TabIndex = 8;
            this.lblUserType.Text = "User Type:";
            // 
            // cmbUserType
            // 
            this.cmbUserType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbUserType.FormattingEnabled = true;
            this.cmbUserType.Items.AddRange(new object[] {
            "All",
            "Student",
            "Corporate",
            "Guest"});
            this.cmbUserType.Location = new System.Drawing.Point(102, 146);
            this.cmbUserType.Name = "cmbUserType";
            this.cmbUserType.Size = new System.Drawing.Size(200, 21);
            this.cmbUserType.TabIndex = 9;
            // 
            // btnGenerateReport
            // 
            this.btnGenerateReport.Location = new System.Drawing.Point(36, 180);
            this.btnGenerateReport.Name = "btnGenerateReport";
            this.btnGenerateReport.Size = new System.Drawing.Size(267, 30);
            this.btnGenerateReport.TabIndex = 10;
            this.btnGenerateReport.Text = "Generate Report";
            this.btnGenerateReport.UseVisualStyleBackColor = true;
            this.btnGenerateReport.Click += new System.EventHandler(this.btnGenerateReport_Click);
            // 
            // reportViewer
            // 
            this.reportViewer.Location = new System.Drawing.Point(15, 218);
            this.reportViewer.Name = "reportViewer";
            this.reportViewer.ServerReport.BearerToken = null;
            this.reportViewer.Size = new System.Drawing.Size(669, 146);
            this.reportViewer.TabIndex = 11;
            // 
            // dataGridViewVenues
            // 
            this.dataGridViewVenues.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewVenues.Location = new System.Drawing.Point(320, 25);
            this.dataGridViewVenues.Name = "dataGridViewVenues";
            this.dataGridViewVenues.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewVenues.Size = new System.Drawing.Size(364, 185);
            this.dataGridViewVenues.TabIndex = 13;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(447, 10);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(93, 13);
            this.label3.TabIndex = 20;
            this.label3.Text = "Matching Venues!";
            // 
            // Reports
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(697, 376);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.dataGridViewVenues);
            this.Controls.Add(this.reportViewer);
            this.Controls.Add(this.btnGenerateReport);
            this.Controls.Add(this.cmbUserType);
            this.Controls.Add(this.lblUserType);
            this.Controls.Add(this.cmbBookingStatus);
            this.Controls.Add(this.lblBookingStatus);
            this.Controls.Add(this.txtVenueName);
            this.Controls.Add(this.lblVenueName);
            this.Controls.Add(this.dtpEndDate);
            this.Controls.Add(this.lblEndDate);
            this.Controls.Add(this.dtpStartDate);
            this.Controls.Add(this.lblStartDate);
            this.Name = "Reports";
            this.Text = "Venue Booking Report";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewVenues)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void btnGenerateReport_Click(object sender, EventArgs e)
        {
            // Check if a venue is selected
            if (dataGridViewVenues.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a venue from the list.");
                return;
            }

            // Get selected venue details
            var selectedVenue = dataGridViewVenues.SelectedRows[0].Cells["VenueName"].Value.ToString();

            // Set report parameters
            var startDate = dtpStartDate.Value;
            var endDate = dtpEndDate.Value;
            var bookingStatus = cmbBookingStatus.SelectedItem.ToString();
            var userType = cmbUserType.SelectedItem.ToString();

            // Configure the report viewer
            reportViewer.LocalReport.DataSources.Clear();
            reportViewer.LocalReport.ReportEmbeddedResource = "Reservify_Techfusion.VenueBookingReport.rdlc"; // Replace with your report file

            // Add parameters to the report
            ReportParameter[] parameters = new ReportParameter[]
            {
                new ReportParameter("StartDate", startDate.ToShortDateString()),
                new ReportParameter("EndDate", endDate.ToShortDateString()),
                new ReportParameter("VenueName", selectedVenue),
                new ReportParameter("BookingStatus", bookingStatus),
                new ReportParameter("UserType", userType)
            };
            reportViewer.LocalReport.SetParameters(parameters);

            // Refresh the report
            reportViewer.RefreshReport();
        }

        // Method to populate the DataGridView with matching venues
        private void LoadMatchingVenues()
        {
            // Simulating loading data (replace with actual data fetching logic)
            DataTable venues = new DataTable();
            venues.Columns.Add("VenueName");
            venues.Columns.Add("Location");

            // Example data
            venues.Rows.Add("Venue A", "Location A");
            venues.Rows.Add("Venue B", "Location B");
            venues.Rows.Add("Venue C", "Location C");

            dataGridViewVenues.DataSource = venues;
        }

        private void Reports_Load(object sender, EventArgs e)
        {
            LoadMatchingVenues();
        }

        #region Windows Form Designer generated code
        private System.Windows.Forms.Label lblStartDate;
        private System.Windows.Forms.DateTimePicker dtpStartDate;
        private System.Windows.Forms.Label lblEndDate;
        private System.Windows.Forms.DateTimePicker dtpEndDate;
        private System.Windows.Forms.Label lblVenueName;
        private System.Windows.Forms.TextBox txtVenueName;
        private System.Windows.Forms.Label lblBookingStatus;
        private System.Windows.Forms.ComboBox cmbBookingStatus;
        private System.Windows.Forms.Label lblUserType;
        private System.Windows.Forms.ComboBox cmbUserType;
        private System.Windows.Forms.Button btnGenerateReport;
        private Microsoft.Reporting.WinForms.ReportViewer reportViewer;
        #endregion

        private DataGridView dataGridViewVenues;
        private Label label3;
    }
}
