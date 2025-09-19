using Astro.Winform.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astro.Winform.Extensions
{
    public static class ControlExtensions
    {
        private static readonly ToolTip toolTip = new ToolTip()
        {
            ToolTipIcon = ToolTipIcon.Error,
            ToolTipTitle = "Error",
            ForeColor = Color.Red
        };
        private static readonly ToolTip standardToolTip = new ToolTip()
        {
            BackColor = Color.AliceBlue
        };
        public static void ShowError(this Control control, string message)
        {
            toolTip.Show(message, control, 0, control.Height, 3000);
            control.Focus();
        }
        public static void ShowToolTip(this Control control, Point location, string message)
        {
            standardToolTip.Show(message, control, location, 3000);
        }
        public static void HideToolTip(this Control control)
        {
            standardToolTip.Hide(control);
        }
        public static void InsertIconColumn(this DataGridView grid, Image? img)
        {
            var iconColumn = new DataGridViewImageColumn()
            {
                Resizable = DataGridViewTriState.False,
                Width = 30,
                Image = img
            };
            grid.Columns.Insert(0, iconColumn);
        }
        public static void ApplyGridStyle(this DataGridView grid)
        {
            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.SteelBlue;
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = grid.ColumnHeadersDefaultCellStyle.BackColor;
            grid.ColumnHeadersDefaultCellStyle.SelectionForeColor = grid.ColumnHeadersDefaultCellStyle.SelectionForeColor;
            grid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font(grid.ColumnHeadersDefaultCellStyle.Font, FontStyle.Bold);
            grid.DefaultCellStyle.Padding = new Padding(3);
            grid.EnableHeadersVisualStyles = false;
            grid.BorderStyle = BorderStyle.None;
        }
        public static DialogResult OpenChildWindow(this Form mainForm, Control control, DockStyle dock = DockStyle.Right)
        {
            using (var tform = new TransparentfForm(mainForm))
            {
                control.BackColor = Color.FromArgb(250, 250, 250);
                if (dock == DockStyle.None)
                {
                    var x = (int)((tform.Width - control.Width) / 2);
                    var y = (int)((tform.Height - control.Height) / 2);
                    control.Location = new Point(x, y);
                }
                else control.Dock = dock;
                tform.ControlToDisplay = control;
                return tform.ShowDialog();
            }
        }
    }
}
