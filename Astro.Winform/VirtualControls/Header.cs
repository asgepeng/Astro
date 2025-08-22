namespace Astro.Forms.Controls
{
    internal class Header : VirtualControl
    {
        private AccountButton accountButton;
        internal Header()
        {
            Bounds = new Rectangle(0, 0, 0, 40);
            accountButton = new AccountButton();
            accountButton.Size = new Size(32, 32);

            Buttons[0] = new VirtualButton("–") { ID = 1 };
            Buttons[1] = new VirtualButton("❒") { ID = 2 };
            Buttons[2] = new VirtualButton("✕") { ID = 3 };

        }
        internal Color BackColor { get; set; } = Color.FromArgb(244, 244, 244);
        internal Rectangle Bounds { get; private set; }
        internal string Text { get; set; } = "Enterprise Resource Management System";
        internal Font Font { get; set; } = new Font("Segoe UI", 11.75F, FontStyle.Bold);
        internal Font ButtonFont { get; set; } = new Font("Segoe UI", 9.75F, FontStyle.Regular);
        internal VirtualButton[] Buttons { get; set; } = new VirtualButton[3];
        internal void ResetHover()
        {
            foreach (var b in this.Buttons) b.Hovered = false;
        }
        internal bool IsMaximized { get; set; } = true;
        internal override void Draw(Graphics g)
        {            
            g.DrawImage(global::Astro.Winform.Properties.Resources.homecolor, 12, 8, 24, 24);
            g.DrawString(Text, Font, Brushes.DarkBlue, new Rectangle(48, 0, 600, 40), new StringFormat() { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center });
            //g.DrawRoundedRectangle(new Rectangle(accountButton.Location, accountButton.Size), 32, Brushes.LightGray);
            var sf = new StringFormat()
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
            for (int i = 0; i < this.Buttons.Length; i++)
            {
                var color = this.Buttons[i].Hovered ? i == 2 ? Color.White : Color.Black : Color.Black;
                this.Buttons[1].Text = IsMaximized ? "❒": "☐";
                if (this.Buttons[i].Hovered)
                {
                    var backColor = this.Buttons[i].Hovered ? i == 2 ? Color.Red : Color.LightGray : Color.LightGray;
                    using (Brush bColor = new SolidBrush(backColor))
                    {
                        g.FillRectangle(bColor, this.Buttons[i].Bounds);
                    }
                }
                using (Brush textBrush = new SolidBrush(color))
                {
                    g.DrawString(this.Buttons[i].Text, ButtonFont, textBrush, this.Buttons[i].Bounds, sf);
                }
            }
        }
        internal VirtualButton? GetHoveredButton(Point location)
        {
            VirtualButton? hovered = null;
            foreach (var b in this.Buttons)
            {
                b.Hovered = b.Bounds.Contains(location);
                if (b.Hovered) hovered = b;
            }
            return hovered;
        }
        internal VirtualButton? GetClickedButton(Point location)
        {
            foreach (var b in this.Buttons)
            {
                if (b.Bounds.Contains(location)) return b;
            }
            return null;
        }
        internal void MainFormResize(Size clientSize)
        {
            Bounds = new Rectangle(0, 0, clientSize.Width, 40);
            accountButton.Location = new Point(clientSize.Width - 48, 4);

            Buttons[0].Location = new Point(clientSize.Width - 144, 0);
            Buttons[1].Location = new Point(clientSize.Width - 96, 0);
            Buttons[2].Location = new Point(clientSize.Width - 48, 0);
        }
    }
    internal class AccountButton
    {
        internal Size Size { get; set; }
        internal Point Location { get; set; }
        internal bool Hovered { get; private set; }
    }
    internal class VirtualButton
    {
        internal VirtualButton(string text)
        {
            Location = new Point(0, 0);
            Size = new Size(48, 32);
            Text = text;
        }
        internal short ID { get; set; } = 0;
        internal Rectangle Bounds => new Rectangle(this.Location, this.Size);
        internal string Text { get; set; }
        internal Point Location { get; set; }
        internal Size Size { get; set; }
        internal bool Hovered { get; set; }
    }
}
