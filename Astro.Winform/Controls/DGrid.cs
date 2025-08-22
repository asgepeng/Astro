using Astro.Forms.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astro.Winform.Controls
{
    internal class DGrid : DataGridView
    {
        protected override bool ShowFocusCues
        {
            get { return true; }
        }
        internal DGrid()
        {
            DataGridViewCellStyle dataGridViewCellStyle9 = new DataGridViewCellStyle();
            this.AllowUserToAddRows = false;
            this.AllowUserToDeleteRows = false;
            this.AllowUserToResizeRows = false;
            this.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.AutoGenerateColumns = false;
            this.BackgroundColor = Color.White;
            this.BorderStyle = BorderStyle.Fixed3D;
            this.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            this.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            this.ColumnHeadersHeight = 26;
            this.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            
            dataGridViewCellStyle9.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle9.BackColor = SystemColors.Window;
            dataGridViewCellStyle9.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            dataGridViewCellStyle9.ForeColor = SystemColors.ControlText;
            dataGridViewCellStyle9.SelectionBackColor = SystemColors.ButtonFace;
            dataGridViewCellStyle9.SelectionForeColor = SystemColors.ControlText;
            dataGridViewCellStyle9.WrapMode = DataGridViewTriState.False;

            this.DefaultCellStyle = dataGridViewCellStyle9;
            this.GridColor = Color.FromArgb(244, 244, 244);
            this.RowHeadersVisible = false;
            this.RowTemplate.Height = 26;
            this.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }
    }
}
