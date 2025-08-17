namespace Reservify_Techfusion
{
    partial class ImageUploadForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImageUploadForm));
            this.panelVenues = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.pictureBoxReset = new System.Windows.Forms.PictureBox();
            this.labelCapacity = new System.Windows.Forms.Label();
            this.labelSelectedCapacity = new System.Windows.Forms.Label();
            this.labelHomeRedirect = new System.Windows.Forms.Label();
            this.trackBarCapacity = new System.Windows.Forms.TrackBar();
            this.labelVenueCategory = new System.Windows.Forms.Label();
            this.comboBoxVenueCategory = new System.Windows.Forms.ComboBox();
            this.labelVenueName = new System.Windows.Forms.Label();
            this.comboBoxVenueName = new System.Windows.Forms.ComboBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxReset)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarCapacity)).BeginInit();
            this.SuspendLayout();
            // 
            // panelVenues
            // 
            this.panelVenues.AutoScroll = true;
            this.panelVenues.Location = new System.Drawing.Point(378, 12);
            this.panelVenues.Name = "panelVenues";
            this.panelVenues.Size = new System.Drawing.Size(300, 480);
            this.panelVenues.TabIndex = 27;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.pictureBoxReset);
            this.groupBox1.Controls.Add(this.labelCapacity);
            this.groupBox1.Controls.Add(this.labelSelectedCapacity);
            this.groupBox1.Controls.Add(this.labelHomeRedirect);
            this.groupBox1.Controls.Add(this.trackBarCapacity);
            this.groupBox1.Controls.Add(this.labelVenueCategory);
            this.groupBox1.Controls.Add(this.comboBoxVenueCategory);
            this.groupBox1.Controls.Add(this.labelVenueName);
            this.groupBox1.Controls.Add(this.comboBoxVenueName);
            this.groupBox1.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(340, 290);
            this.groupBox1.TabIndex = 28;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Find Venue";
            // 
            // pictureBoxReset
            // 
            this.pictureBoxReset.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBoxReset.BackgroundImage")));
            this.pictureBoxReset.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBoxReset.Location = new System.Drawing.Point(264, 213);
            this.pictureBoxReset.Name = "pictureBoxReset";
            this.pictureBoxReset.Size = new System.Drawing.Size(58, 47);
            this.pictureBoxReset.TabIndex = 27;
            this.pictureBoxReset.TabStop = false;
            this.pictureBoxReset.Click += new System.EventHandler(this.pictureBoxReset_Click);
            // 
            // labelCapacity
            // 
            this.labelCapacity.AutoSize = true;
            this.labelCapacity.Location = new System.Drawing.Point(98, 63);
            this.labelCapacity.Name = "labelCapacity";
            this.labelCapacity.Size = new System.Drawing.Size(72, 18);
            this.labelCapacity.TabIndex = 2;
            this.labelCapacity.Text = "Capacity:";
            // 
            // labelSelectedCapacity
            // 
            this.labelSelectedCapacity.AutoSize = true;
            this.labelSelectedCapacity.Location = new System.Drawing.Point(176, 63);
            this.labelSelectedCapacity.Name = "labelSelectedCapacity";
            this.labelSelectedCapacity.Size = new System.Drawing.Size(16, 18);
            this.labelSelectedCapacity.TabIndex = 4;
            this.labelSelectedCapacity.Text = "1";
            // 
            // labelHomeRedirect
            // 
            this.labelHomeRedirect.AutoSize = true;
            this.labelHomeRedirect.Cursor = System.Windows.Forms.Cursors.Hand;
            this.labelHomeRedirect.ForeColor = System.Drawing.Color.Blue;
            this.labelHomeRedirect.Location = new System.Drawing.Point(29, 263);
            this.labelHomeRedirect.Name = "labelHomeRedirect";
            this.labelHomeRedirect.Size = new System.Drawing.Size(274, 18);
            this.labelHomeRedirect.TabIndex = 22;
            this.labelHomeRedirect.Text = "Return Home? Click here to go home!";
            this.labelHomeRedirect.Click += new System.EventHandler(this.labelHomeRedirect_Click);
            // 
            // trackBarCapacity
            // 
            this.trackBarCapacity.Location = new System.Drawing.Point(10, 31);
            this.trackBarCapacity.Maximum = 100;
            this.trackBarCapacity.Minimum = 1;
            this.trackBarCapacity.Name = "trackBarCapacity";
            this.trackBarCapacity.Size = new System.Drawing.Size(330, 45);
            this.trackBarCapacity.TabIndex = 3;
            this.trackBarCapacity.Value = 1;
            // 
            // labelVenueCategory
            // 
            this.labelVenueCategory.AutoSize = true;
            this.labelVenueCategory.Location = new System.Drawing.Point(-3, 123);
            this.labelVenueCategory.Name = "labelVenueCategory";
            this.labelVenueCategory.Size = new System.Drawing.Size(124, 18);
            this.labelVenueCategory.TabIndex = 5;
            this.labelVenueCategory.Text = "Venue Category:";
            // 
            // comboBoxVenueCategory
            // 
            this.comboBoxVenueCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxVenueCategory.Location = new System.Drawing.Point(122, 120);
            this.comboBoxVenueCategory.Name = "comboBoxVenueCategory";
            this.comboBoxVenueCategory.Size = new System.Drawing.Size(200, 26);
            this.comboBoxVenueCategory.TabIndex = 6;
            // 
            // labelVenueName
            // 
            this.labelVenueName.AutoSize = true;
            this.labelVenueName.Location = new System.Drawing.Point(6, 182);
            this.labelVenueName.Name = "labelVenueName";
            this.labelVenueName.Size = new System.Drawing.Size(100, 18);
            this.labelVenueName.TabIndex = 7;
            this.labelVenueName.Text = "Venue Name:";
            // 
            // comboBoxVenueName
            // 
            this.comboBoxVenueName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxVenueName.Location = new System.Drawing.Point(122, 179);
            this.comboBoxVenueName.Name = "comboBoxVenueName";
            this.comboBoxVenueName.Size = new System.Drawing.Size(200, 26);
            this.comboBoxVenueName.TabIndex = 8;
            // 
            // ImageUploadForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(677, 490);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.panelVenues);
            this.Name = "ImageUploadForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ImageUploadForm";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxReset)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarCapacity)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelVenues;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label labelCapacity;
        private System.Windows.Forms.Label labelSelectedCapacity;
        private System.Windows.Forms.Label labelHomeRedirect;
        private System.Windows.Forms.TrackBar trackBarCapacity;
        private System.Windows.Forms.Label labelVenueCategory;
        private System.Windows.Forms.ComboBox comboBoxVenueCategory;
        private System.Windows.Forms.Label labelVenueName;
        private System.Windows.Forms.ComboBox comboBoxVenueName;
        private System.Windows.Forms.PictureBox pictureBoxReset;
    }
}