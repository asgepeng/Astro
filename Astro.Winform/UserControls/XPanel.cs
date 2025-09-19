using Astro.Drawing.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astro.Winform.UserControls
{
    public class XPanel : UserControl
    {
        public XPanel()
        {
            this.InitializeComponent();
            this.SetStyle(ControlStyles.AllPaintingInWmPaint
                | ControlStyles.UserPaint
                | ControlStyles.ResizeRedraw
                | ControlStyles.OptimizedDoubleBuffer, true);
            this.UpdateStyles();
        }
        public Image? Icon { get; set; }
        private void InitializeComponent()
        {
            SuspendLayout();
            // 
            // XPanel
            // 
            BackColor = Color.FromArgb(250, 250, 250);
            BorderStyle = BorderStyle.FixedSingle;
            Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Name = "XPanel";
            Padding = new Padding(10, 50, 10, 50);
            Size = new Size(397, 576);
            ResumeLayout(false);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var x = 15;
            var y = 10;
            if (Icon != null)
            {
                var imgRext = new Rectangle(x, y, 32, 32);
                e.Graphics.DrawImage(this.Icon, imgRext);
                x += 37;
            }
            var rect = new Rectangle(x, y, this.Width - 10, 32);
            TextFormatFlags flags = TextFormatFlags.EndEllipsis;
            TextRenderer.DrawText(e.Graphics, this.Text, My.Application.TitleFont, rect, My.Application.TitleForeColor, flags);
        }
    }
}
