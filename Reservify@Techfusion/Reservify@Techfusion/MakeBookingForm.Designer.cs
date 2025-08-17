using System;
using System.Windows.Forms;

namespace Reservify_Techfusion
{
    partial class MakeBookingForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MakeBookingForm));
            this.labelChooseDate = new System.Windows.Forms.Label();
            this.monthCalendar = new System.Windows.Forms.MonthCalendar();
            this.labelCapacity = new System.Windows.Forms.Label();
            this.trackBarCapacity = new System.Windows.Forms.TrackBar();
            this.labelSelectedCapacity = new System.Windows.Forms.Label();
            this.labelVenueCategory = new System.Windows.Forms.Label();
            this.comboBoxVenueCategory = new System.Windows.Forms.ComboBox();
            this.labelVenueName = new System.Windows.Forms.Label();
            this.comboBoxVenueName = new System.Windows.Forms.ComboBox();
            this.buttonMatchMe = new System.Windows.Forms.Button();
            this.dataGridViewVenues = new System.Windows.Forms.DataGridView();
            this.labelStartTime = new System.Windows.Forms.Label();
            this.labelEndTime = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.dataGridViewBookings = new System.Windows.Forms.DataGridView();
            this.label4 = new System.Windows.Forms.Label();
            this.labelHomeRedirect = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.pictureBoxReset = new System.Windows.Forms.PictureBox();
            this.dateTimePickerend = new System.Windows.Forms.DateTimePicker();
            this.dateTimePickerstart = new System.Windows.Forms.DateTimePicker();
            this.panelVenues = new System.Windows.Forms.Panel();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.trackBarCapacity)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewVenues)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewBookings)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxReset)).BeginInit();
            this.SuspendLayout();
            // 
            // labelChooseDate
            // 
            this.labelChooseDate.AutoSize = true;
            this.labelChooseDate.Location = new System.Drawing.Point(124, 26);
            this.labelChooseDate.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelChooseDate.Name = "labelChooseDate";
            this.labelChooseDate.Size = new System.Drawing.Size(136, 22);
            this.labelChooseDate.TabIndex = 0;
            this.labelChooseDate.Text = "Choose Date:";
            // 
            // monthCalendar
            // 
            this.monthCalendar.Location = new System.Drawing.Point(71, 44);
            this.monthCalendar.Margin = new System.Windows.Forms.Padding(12, 11, 12, 11);
            this.monthCalendar.Name = "monthCalendar";
            this.monthCalendar.TabIndex = 1;
            // 
            // labelCapacity
            // 
            this.labelCapacity.AutoSize = true;
            this.labelCapacity.Location = new System.Drawing.Point(137, 363);
            this.labelCapacity.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelCapacity.Name = "labelCapacity";
            this.labelCapacity.Size = new System.Drawing.Size(98, 22);
            this.labelCapacity.TabIndex = 2;
            this.labelCapacity.Text = "Capacity:";
            // 
            // trackBarCapacity
            // 
            this.trackBarCapacity.Location = new System.Drawing.Point(5, 337);
            this.trackBarCapacity.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.trackBarCapacity.Maximum = 100;
            this.trackBarCapacity.Minimum = 1;
            this.trackBarCapacity.Name = "trackBarCapacity";
            this.trackBarCapacity.Size = new System.Drawing.Size(440, 56);
            this.trackBarCapacity.TabIndex = 3;
            this.trackBarCapacity.Value = 1;
            this.trackBarCapacity.ValueChanged += new System.EventHandler(this.TrackBarCapacity_ValueChanged);
            // 
            // labelSelectedCapacity
            // 
            this.labelSelectedCapacity.AutoSize = true;
            this.labelSelectedCapacity.Location = new System.Drawing.Point(241, 363);
            this.labelSelectedCapacity.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelSelectedCapacity.Name = "labelSelectedCapacity";
            this.labelSelectedCapacity.Size = new System.Drawing.Size(21, 22);
            this.labelSelectedCapacity.TabIndex = 4;
            this.labelSelectedCapacity.Text = "1";
            // 
            // labelVenueCategory
            // 
            this.labelVenueCategory.AutoSize = true;
            this.labelVenueCategory.Location = new System.Drawing.Point(-4, 416);
            this.labelVenueCategory.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelVenueCategory.Name = "labelVenueCategory";
            this.labelVenueCategory.Size = new System.Drawing.Size(165, 22);
            this.labelVenueCategory.TabIndex = 5;
            this.labelVenueCategory.Text = "Venue Category:";
            // 
            // comboBoxVenueCategory
            // 
            this.comboBoxVenueCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxVenueCategory.Location = new System.Drawing.Point(163, 412);
            this.comboBoxVenueCategory.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.comboBoxVenueCategory.Name = "comboBoxVenueCategory";
            this.comboBoxVenueCategory.Size = new System.Drawing.Size(265, 30);
            this.comboBoxVenueCategory.TabIndex = 6;
            // 
            // labelVenueName
            // 
            this.labelVenueName.AutoSize = true;
            this.labelVenueName.Location = new System.Drawing.Point(16, 470);
            this.labelVenueName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelVenueName.Name = "labelVenueName";
            this.labelVenueName.Size = new System.Drawing.Size(132, 22);
            this.labelVenueName.TabIndex = 7;
            this.labelVenueName.Text = "Venue Name:";
            // 
            // comboBoxVenueName
            // 
            this.comboBoxVenueName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxVenueName.Location = new System.Drawing.Point(163, 466);
            this.comboBoxVenueName.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.comboBoxVenueName.Name = "comboBoxVenueName";
            this.comboBoxVenueName.Size = new System.Drawing.Size(265, 30);
            this.comboBoxVenueName.TabIndex = 8;
            // 
            // buttonMatchMe
            // 
            this.buttonMatchMe.BackColor = System.Drawing.SystemColors.Highlight;
            this.buttonMatchMe.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonMatchMe.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.buttonMatchMe.Location = new System.Drawing.Point(29, 506);
            this.buttonMatchMe.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonMatchMe.Name = "buttonMatchMe";
            this.buttonMatchMe.Size = new System.Drawing.Size(273, 48);
            this.buttonMatchMe.TabIndex = 11;
            this.buttonMatchMe.Text = "Reservify Venue";
            this.buttonMatchMe.UseVisualStyleBackColor = false;
            this.buttonMatchMe.Click += new System.EventHandler(this.ButtonMatchMe_Click);
            // 
            // dataGridViewVenues
            // 
            this.dataGridViewVenues.ColumnHeadersHeight = 29;
            this.dataGridViewVenues.Location = new System.Drawing.Point(512, 673);
            this.dataGridViewVenues.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.dataGridViewVenues.Name = "dataGridViewVenues";
            this.dataGridViewVenues.RowHeadersWidth = 51;
            this.dataGridViewVenues.Size = new System.Drawing.Size(539, 122);
            this.dataGridViewVenues.TabIndex = 12;
            this.dataGridViewVenues.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewVenues_CellContentClick);
            // 
            // labelStartTime
            // 
            this.labelStartTime.AutoSize = true;
            this.labelStartTime.Location = new System.Drawing.Point(16, 255);
            this.labelStartTime.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelStartTime.Name = "labelStartTime";
            this.labelStartTime.Size = new System.Drawing.Size(112, 22);
            this.labelStartTime.TabIndex = 9;
            this.labelStartTime.Text = "Start Time:";
            // 
            // labelEndTime
            // 
            this.labelEndTime.AutoSize = true;
            this.labelEndTime.Location = new System.Drawing.Point(259, 255);
            this.labelEndTime.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelEndTime.Name = "labelEndTime";
            this.labelEndTime.Size = new System.Drawing.Size(105, 22);
            this.labelEndTime.TabIndex = 10;
            this.labelEndTime.Text = "End Time:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(508, 645);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(82, 16);
            this.label3.TabIndex = 19;
            this.label3.Text = "My Matches!";
            // 
            // dataGridViewBookings
            // 
            this.dataGridViewBookings.ColumnHeadersHeight = 29;
            this.dataGridViewBookings.Location = new System.Drawing.Point(36, 673);
            this.dataGridViewBookings.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.dataGridViewBookings.Name = "dataGridViewBookings";
            this.dataGridViewBookings.RowHeadersWidth = 51;
            this.dataGridViewBookings.Size = new System.Drawing.Size(453, 122);
            this.dataGridViewBookings.TabIndex = 20;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(15, 645);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(88, 16);
            this.label4.TabIndex = 21;
            this.label4.Text = "My Bookings!";
            // 
            // labelHomeRedirect
            // 
            this.labelHomeRedirect.AutoSize = true;
            this.labelHomeRedirect.Cursor = System.Windows.Forms.Cursors.Hand;
            this.labelHomeRedirect.ForeColor = System.Drawing.Color.Blue;
            this.labelHomeRedirect.Location = new System.Drawing.Point(49, 558);
            this.labelHomeRedirect.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelHomeRedirect.Name = "labelHomeRedirect";
            this.labelHomeRedirect.Size = new System.Drawing.Size(354, 22);
            this.labelHomeRedirect.TabIndex = 22;
            this.labelHomeRedirect.Text = "Return Home? Click here to go home!";
            this.labelHomeRedirect.Click += new System.EventHandler(this.labelHomeRedirect_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.pictureBoxReset);
            this.groupBox1.Controls.Add(this.dateTimePickerend);
            this.groupBox1.Controls.Add(this.dateTimePickerstart);
            this.groupBox1.Controls.Add(this.labelCapacity);
            this.groupBox1.Controls.Add(this.labelSelectedCapacity);
            this.groupBox1.Controls.Add(this.monthCalendar);
            this.groupBox1.Controls.Add(this.labelHomeRedirect);
            this.groupBox1.Controls.Add(this.labelChooseDate);
            this.groupBox1.Controls.Add(this.trackBarCapacity);
            this.groupBox1.Controls.Add(this.labelVenueCategory);
            this.groupBox1.Controls.Add(this.comboBoxVenueCategory);
            this.groupBox1.Controls.Add(this.labelVenueName);
            this.groupBox1.Controls.Add(this.buttonMatchMe);
            this.groupBox1.Controls.Add(this.comboBoxVenueName);
            this.groupBox1.Controls.Add(this.labelStartTime);
            this.groupBox1.Controls.Add(this.labelEndTime);
            this.groupBox1.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold);
            this.groupBox1.Location = new System.Drawing.Point(16, 15);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Size = new System.Drawing.Size(456, 591);
            this.groupBox1.TabIndex = 24;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Find Venue";
            // 
            // pictureBoxReset
            // 
            this.pictureBoxReset.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBoxReset.BackgroundImage")));
            this.pictureBoxReset.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBoxReset.Location = new System.Drawing.Point(337, 506);
            this.pictureBoxReset.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pictureBoxReset.Name = "pictureBoxReset";
            this.pictureBoxReset.Size = new System.Drawing.Size(77, 58);
            this.pictureBoxReset.TabIndex = 28;
            this.pictureBoxReset.TabStop = false;
            this.pictureBoxReset.Click += new System.EventHandler(this.pictureBoxReset_Click);
            // 
            // dateTimePickerend
            // 
            this.dateTimePickerend.CustomFormat = "HH:mm";
            this.dateTimePickerend.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePickerend.Location = new System.Drawing.Point(245, 274);
            this.dateTimePickerend.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.dateTimePickerend.Name = "dateTimePickerend";
            this.dateTimePickerend.ShowUpDown = true;
            this.dateTimePickerend.Size = new System.Drawing.Size(168, 29);
            this.dateTimePickerend.TabIndex = 24;
            // 
            // dateTimePickerstart
            // 
            this.dateTimePickerstart.CustomFormat = "HH:mm";
            this.dateTimePickerstart.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePickerstart.Location = new System.Drawing.Point(20, 274);
            this.dateTimePickerstart.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.dateTimePickerstart.Name = "dateTimePickerstart";
            this.dateTimePickerstart.ShowUpDown = true;
            this.dateTimePickerstart.Size = new System.Drawing.Size(172, 29);
            this.dateTimePickerstart.TabIndex = 23;
            // 
            // panelVenues
            // 
            this.panelVenues.AutoScroll = true;
            this.panelVenues.Location = new System.Drawing.Point(532, 15);
            this.panelVenues.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panelVenues.Name = "panelVenues";
            this.panelVenues.Size = new System.Drawing.Size(400, 591);
            this.panelVenues.TabIndex = 27;
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // MakeBookingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(932, 607);
            this.Controls.Add(this.panelVenues);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.dataGridViewBookings);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.dataGridViewVenues);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "MakeBookingForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Make Booking";
            ((System.ComponentModel.ISupportInitialize)(this.trackBarCapacity)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewVenues)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewBookings)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxReset)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }


        private System.Windows.Forms.Label labelChooseDate;
        private System.Windows.Forms.MonthCalendar monthCalendar;
        private System.Windows.Forms.Label labelCapacity;
        private System.Windows.Forms.TrackBar trackBarCapacity;
        private System.Windows.Forms.Label labelSelectedCapacity;
        private System.Windows.Forms.Label labelVenueCategory;
        private System.Windows.Forms.ComboBox comboBoxVenueCategory;
        private System.Windows.Forms.Label labelVenueName; // Label for venue name
        private System.Windows.Forms.ComboBox comboBoxVenueName; // ComboBox for venue names
        private System.Windows.Forms.Button buttonMatchMe;
        private System.Windows.Forms.DataGridView dataGridViewVenues;
        private System.Windows.Forms.Label labelStartTime;
        private System.Windows.Forms.Label labelEndTime;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DataGridView dataGridViewBookings;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label labelHomeRedirect;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DateTimePicker dateTimePickerstart;
        private System.Windows.Forms.DateTimePicker dateTimePickerend;
        private Panel panelVenues;
        private ImageList imageList1;
        private PictureBox pictureBoxReset;
    }
}
