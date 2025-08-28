using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astro.Winform.Controls
{
    public class FlatTabControl : TabControl
    {
        public Color ActiveColor { get; set; } = Color.DeepSkyBlue;
        public Color InactiveColor { get; set; } = Color.LightGray;
        public Color TextColor { get; set; } = Color.Black;
        public FlatTabControl()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                 ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);

            DrawMode = TabDrawMode.OwnerDrawFixed;
            SizeMode = TabSizeMode.Normal;
            ItemSize = new Size(120, 40);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.Clear(Parent.BackColor);
            for (int i = 0; i < TabPages.Count; i++)
            {
                Rectangle rect = GetTabRect(i);
                bool selected = (i == SelectedIndex);

                

                

                // garis highlight bawah untuk tab aktif
                if (selected)
                {
                    using (SolidBrush b = new SolidBrush(Color.White))
                    //e.Graphics.FillRectangle(b, rect);
                    using (Pen p = new Pen(ActiveColor, 3))
                        e.Graphics.DrawLine(p, rect.Left + 5, rect.Bottom - 2, rect.Right - 5, rect.Bottom - 2);
                }
                // text
                TextRenderer.DrawText(
                    e.Graphics,
                    TabPages[i].Text,
                    Font,
                    rect,
                    TextColor,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
                );
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // kosong: biar gak gambar border 3D bawaan
        }
    }
}
