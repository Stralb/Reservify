namespace Reservify_Techfusion
{
    partial class VenueCard
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.buttonChangeToVenue = new System.Windows.Forms.Button();
            this.labelVenueName = new System.Windows.Forms.Label();
            this.labelVenueCategory = new System.Windows.Forms.Label();
            this.labelVenueCapacity = new System.Windows.Forms.Label();
            this.pictureBoxVenue = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxVenue)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonChangeToVenue
            // 
            this.buttonChangeToVenue.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold);
            this.buttonChangeToVenue.Location = new System.Drawing.Point(8, 273);
            this.buttonChangeToVenue.Margin = new System.Windows.Forms.Padding(4);
            this.buttonChangeToVenue.Name = "buttonChangeToVenue";
            this.buttonChangeToVenue.Size = new System.Drawing.Size(348, 49);
            this.buttonChangeToVenue.TabIndex = 1;
            this.buttonChangeToVenue.Text = "Change To Venue";
            this.buttonChangeToVenue.UseVisualStyleBackColor = true;
            // 
            // labelVenueName
            // 
            this.labelVenueName.AutoSize = true;
            this.labelVenueName.Font = new System.Drawing.Font("Arial", 11.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelVenueName.Location = new System.Drawing.Point(92, 0);
            this.labelVenueName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelVenueName.Name = "labelVenueName";
            this.labelVenueName.Size = new System.Drawing.Size(65, 22);
            this.labelVenueName.TabIndex = 2;
            this.labelVenueName.Text = "label1";
            // 
            // labelVenueCategory
            // 
            this.labelVenueCategory.AutoSize = true;
            this.labelVenueCategory.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold);
            this.labelVenueCategory.Location = new System.Drawing.Point(4, 50);
            this.labelVenueCategory.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelVenueCategory.Name = "labelVenueCategory";
            this.labelVenueCategory.Size = new System.Drawing.Size(65, 22);
            this.labelVenueCategory.TabIndex = 3;
            this.labelVenueCategory.Text = "label1";
            // 
            // labelVenueCapacity
            // 
            this.labelVenueCapacity.AutoSize = true;
            this.labelVenueCapacity.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold);
            this.labelVenueCapacity.Location = new System.Drawing.Point(4, 25);
            this.labelVenueCapacity.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelVenueCapacity.Name = "labelVenueCapacity";
            this.labelVenueCapacity.Size = new System.Drawing.Size(65, 22);
            this.labelVenueCapacity.TabIndex = 4;
            this.labelVenueCapacity.Text = "label2";
            this.labelVenueCapacity.UseMnemonic = false;
            // 
            // pictureBoxVenue
            // 
            this.pictureBoxVenue.Location = new System.Drawing.Point(8, 76);
            this.pictureBoxVenue.Margin = new System.Windows.Forms.Padding(4);
            this.pictureBoxVenue.Name = "pictureBoxVenue";
            this.pictureBoxVenue.Size = new System.Drawing.Size(348, 193);
            this.pictureBoxVenue.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxVenue.TabIndex = 24;
            this.pictureBoxVenue.TabStop = false;
            // 
            // VenueCard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DodgerBlue;
            this.Controls.Add(this.pictureBoxVenue);
            this.Controls.Add(this.labelVenueCapacity);
            this.Controls.Add(this.labelVenueCategory);
            this.Controls.Add(this.labelVenueName);
            this.Controls.Add(this.buttonChangeToVenue);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "VenueCard";
            this.Size = new System.Drawing.Size(361, 326);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxVenue)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonChangeToVenue;
        private System.Windows.Forms.Label labelVenueName;
        private System.Windows.Forms.Label labelVenueCategory;
        private System.Windows.Forms.Label labelVenueCapacity;
        private System.Windows.Forms.PictureBox pictureBoxVenue;
    }
}
