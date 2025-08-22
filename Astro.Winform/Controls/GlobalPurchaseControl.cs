using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astro.Winform.Controls
{
    public class GlobalPurchaseControl : Control
    {
        public GlobalPurchaseControl()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.UserPaint |
                          ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.OptimizedDoubleBuffer |
                          ControlStyles.StandardDoubleClick, true);
            this.UpdateStyles();
        }
        private Font TitleFont { get; set; } = new Font("Segoe UI", 15.75F, FontStyle.Regular);
        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            var titleRect = new Rectangle(10, 0, this.Width - 20, 40);
            var sf = new StringFormat() { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
            g.DrawString("Pembelian", this.TitleFont, Brushes.Black, titleRect, sf);
            using (var pens = new Pen(new SolidBrush(Color.FromArgb(240, 240, 240))))
            {
                g.DrawLine(pens, new Point(0, 40), new Point(this.Width, 40));
            }
            base.OnPaint(e);
        }

        private void InitializeComponent()
        {
            SuspendLayout();
            // 
            // GlobalPurchaseControl
            // 
            BackColor = Color.White;
            ResumeLayout(false);
        }
    }
}
