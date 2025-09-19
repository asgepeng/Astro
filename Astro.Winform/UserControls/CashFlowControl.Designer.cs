namespace Astro.Winform.UserControls
{
    partial class CashFlowControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            dGrid1 = new Controls.DGrid();
            ((System.ComponentModel.ISupportInitialize)dGrid1).BeginInit();
            SuspendLayout();
            // 
            // dGrid1
            // 
            dGrid1.AllowUserToAddRows = false;
            dGrid1.AllowUserToDeleteRows = false;
            dGrid1.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = Color.FromArgb(244, 244, 244);
            dataGridViewCellStyle1.ForeColor = SystemColors.ControlText;
            dGrid1.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            dGrid1.BackgroundColor = Color.White;
            dGrid1.BorderStyle = BorderStyle.None;
            dGrid1.CellBorderStyle = DataGridViewCellBorderStyle.None;
            dGrid1.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dGrid1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = SystemColors.Window;
            dataGridViewCellStyle2.Font = new Font("Segoe UI", 9.75F);
            dataGridViewCellStyle2.ForeColor = SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = Color.FromArgb(255, 255, 192);
            dataGridViewCellStyle2.SelectionForeColor = SystemColors.ControlText;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.False;
            dGrid1.DefaultCellStyle = dataGridViewCellStyle2;
            dGrid1.Dock = DockStyle.Fill;
            dGrid1.EnableHeadersVisualStyles = false;
            dGrid1.Location = new Point(0, 57);
            dGrid1.MultiSelect = false;
            dGrid1.Name = "dGrid1";
            dGrid1.ReadOnly = true;
            dGrid1.RowHeadersVisible = false;
            dGrid1.RowTemplate.Height = 30;
            dGrid1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dGrid1.Size = new Size(370, 515);
            dGrid1.TabIndex = 0;
            // 
            // CashFlowControl
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(dGrid1);
            Name = "CashFlowControl";
            Padding = new Padding(0, 57, 0, 57);
            Size = new Size(370, 629);
            Load += CashFlowControl_Load;
            ((System.ComponentModel.ISupportInitialize)dGrid1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Controls.DGrid dGrid1;
    }
}
