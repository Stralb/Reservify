using System;
using System.Drawing;
using System.Windows.Forms;

public class TransparentLabel : Label
{
    public TransparentLabel()
    {
        this.BackColor = Color.Transparent; // Set the default background to transparent
    }

    protected override void OnPaintBackground(PaintEventArgs e)
    {
        // Do nothing here to keep the background transparent
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        // Draw the text without a background
        using (SolidBrush textBrush = new SolidBrush(this.ForeColor))
        {
            e.Graphics.DrawString(this.Text, this.Font, textBrush, 0, 0);
        }
    }
}
