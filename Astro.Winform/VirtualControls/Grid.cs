namespace Astro.Forms.Controls
{
    public class Grid : VirtualControl
    {
        public string[] Columns { get; set; } = Array.Empty<string>();
        public List<string[]> Rows { get; set; } = new List<string[]>();

        public int RowHeight { get; set; } = 24;
        public int HeaderHeight { get; set; } = 28;
        public int ColumnWidth { get; set; } = 120;
        public Font Font { get; set; } = SystemFonts.DefaultFont;
        public Color BackColor { get; set; } = Color.White;
        public Color GridColor { get; set; } = Color.LightGray;
        public Color HeaderColor { get; set; } = Color.LightGray;
        public Color ForeColor { get; set; } = Color.Black;

        public int ScrollOffset { get; set; } = 0;
        public Rectangle Bounds { get; set; }

        internal override void Draw(Graphics g)
        {
            g.SetClip(Bounds);
            g.Clear(BackColor);

            int width = Bounds.Width;
            int xOffset = Bounds.X;
            int yOffset = Bounds.Y;

            // Gambar header
            for (int col = 0; col < Columns.Length; col++)
            {
                Rectangle headerRect = new Rectangle(
                    xOffset + col * ColumnWidth,
                    yOffset,
                    ColumnWidth,
                    HeaderHeight
                );

                using (Brush b = new SolidBrush(HeaderColor))
                    g.FillRectangle(b, headerRect);

                g.DrawRectangle(Pens.DarkGray, headerRect);
                TextRenderer.DrawText(g, Columns[col], Font, headerRect, ForeColor,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            }

            // Gambar rows
            int startY = yOffset + HeaderHeight - ScrollOffset;
            for (int row = 0; row < Rows.Count; row++)
            {
                int y = startY + row * RowHeight;

                if (y + RowHeight < yOffset || y > yOffset + Bounds.Height)
                    continue; // Skip if not visible

                for (int col = 0; col < Columns.Length; col++)
                {
                    Rectangle cellRect = new Rectangle(
                        xOffset + col * ColumnWidth,
                        y,
                        ColumnWidth,
                        RowHeight
                    );

                    g.FillRectangle(Brushes.White, cellRect);
                    g.DrawRectangle(new Pen(GridColor), cellRect);

                    string text = (col < Rows[row].Length) ? Rows[row][col] : "";
                    TextRenderer.DrawText(g, text, Font, cellRect, ForeColor,
                        TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
                }
            }
        }

        public void OnMouseWheel(MouseEventArgs e)
        {
            int maxVisibleRows = (Bounds.Height - HeaderHeight) / RowHeight;
            int maxScroll = Math.Max(0, Rows.Count - maxVisibleRows);

            ScrollOffset -= Math.Sign(e.Delta) * RowHeight; // scroll 1 row per gerakan
            ScrollOffset = Math.Max(0, Math.Min(ScrollOffset, maxScroll * RowHeight));
        }
    }
}
