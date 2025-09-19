using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Astro.Forms.Controls
{
    public partial class DashboardUserControl : UserControl
    {
        public DashboardUserControl()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.AllPaintingInWmPaint
                | ControlStyles.UserPaint
                | ControlStyles.ResizeRedraw
                | ControlStyles.OptimizedDoubleBuffer, true);
            this.UpdateStyles();
        }
        StringFormat sf1 = new StringFormat()
        {
            Alignment = StringAlignment.Near,
            LineAlignment = StringAlignment.Near
        };
        StringFormat sf2 = new StringFormat()
        {
            Alignment = StringAlignment.Far,
            LineAlignment = StringAlignment.Far
        };
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var rectWidth = this.Width / 4 - 5;
            var rectHeight = 100;

            var x = 10;
            var y = 10;
            var colors = new Color[] { Color.Red, Color.Orange, Color.Blue, Color.Green };
            var texts = new string[] { "Penjualan", "Pembelian", "Pemasukan", "Pengeluaran" };
            var amount = new long[] { 1900192, 2562661, 288280, 1720000 };
            for (int i=0; i < 4; i++)
            {
                using (var brush =new SolidBrush(colors[i]))
                {
                    var r = new Rectangle(x, y, rectWidth, rectHeight);
                    e.Graphics.FillRectangle(brush, r);
                    e.Graphics.DrawString(texts[i], this.Font, Brushes.White, r, sf1);
                    e.Graphics.DrawString(amount[i].ToString("N0"), My.Application.TitleFont, Brushes.White, r, sf2);
                    x += rectWidth + 5;
                }
            }
        }
    }
}
