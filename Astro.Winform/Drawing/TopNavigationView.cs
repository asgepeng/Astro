using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PointOfSale.Drawing
{
    internal class TopNavigationView : VirtualControl
    {
        private AccountButton accountButton;
        internal TopNavigationView()
        {
            Bounds = new Rectangle(0, 0, 0, 48);
            accountButton = new AccountButton();
            accountButton.Size = new Size(32, 32);
        }
        internal Color BackColor { get; set; } = Color.FromArgb(244, 244, 244);
        internal Rectangle Bounds { get; private set; }
        internal string Text { get; set; } = "Point Of Sale Application";
        internal Font Font { get; set; } = new Font("Segoe UI", 12.75F, FontStyle.Bold);
        internal override void Draw(Graphics g)
        {
            g.FillRectangle(new SolidBrush(BackColor), Bounds);
            g.DrawLine(Pens.LightGray, 48, Bounds.Height, Bounds.Width, Bounds.Height);
            g.DrawImage(global::Astro.Winform.Properties.Resources.homecolor, 12, 12, 24, 24);
            g.DrawString(this.Text, this.Font, Brushes.DarkBlue, new Rectangle(50, 0, 300, 48), new StringFormat() { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center});
            DrawingHelper.DrawRoundedRectangle(g, new Rectangle(this.accountButton.Location, this.accountButton.Size), 32, Brushes.LightGray);

        }
        internal void MainFormResize(Size clientSize)
        {
            this.Bounds = new Rectangle(0, 0, clientSize.Width, 48);
            this.accountButton.Location = new Point(clientSize.Width - 48, 8);
        }
    }
    internal class AccountButton
    {
        internal Size Size { get; set; }
        internal Point Location { get; set; }
        internal bool Hovered { get; private set; }
    }
}
