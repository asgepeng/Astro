using Astro.Winform.Controls;

namespace Astro.Winform.Forms
{
    partial class ListCategoryForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            dataGridView1 = new DGrid();
            col_01 = new DataGridViewTextBoxColumn();
            col_02 = new DataGridViewTextBoxColumn();
            col_03 = new DataGridViewTextBoxColumn();
            button1 = new Button();
            button2 = new Button();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // dataGridView1
            // 
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.AllowUserToResizeRows = false;
            dataGridView1.BackgroundColor = Color.White;
            dataGridView1.BorderStyle = BorderStyle.Fixed3D;
            dataGridView1.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dataGridView1.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dataGridView1.ColumnHeadersHeight = 30;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridView1.Columns.AddRange(new DataGridViewColumn[] { col_01, col_02, col_03 });
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = SystemColors.Window;
            dataGridViewCellStyle2.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            dataGridViewCellStyle2.ForeColor = SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = SystemColors.InactiveBorder;
            dataGridViewCellStyle2.SelectionForeColor = SystemColors.ControlText;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.False;
            dataGridView1.DefaultCellStyle = dataGridViewCellStyle2;
            dataGridView1.Dock = DockStyle.Fill;
            dataGridView1.GridColor = Color.FromArgb(244, 244, 244);
            dataGridView1.Location = new Point(0, 50);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.ReadOnly = true;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.RowTemplate.Height = 30;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.Size = new Size(500, 448);
            dataGridView1.TabIndex = 1;
            dataGridView1.CellDoubleClick += HandleGridCellDoubleCLicked;
            // 
            // col_01
            // 
            col_01.DataPropertyName = "name";
            col_01.HeaderText = "Category Name";
            col_01.Name = "col_01";
            col_01.ReadOnly = true;
            col_01.Width = 250;
            // 
            // col_02
            // 
            col_02.DataPropertyName = "createddate";
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle1.Format = "dd-MM-yyyy HH:mm";
            col_02.DefaultCellStyle = dataGridViewCellStyle1;
            col_02.HeaderText = "Date Added";
            col_02.Name = "col_02";
            col_02.ReadOnly = true;
            col_02.Width = 120;
            // 
            // col_03
            // 
            col_03.DataPropertyName = "fullname";
            col_03.HeaderText = "Added By";
            col_03.Name = "col_03";
            col_03.ReadOnly = true;
            col_03.Width = 200;
            // 
            // button1
            // 
            button1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            button1.Location = new Point(3, 504);
            button1.Name = "button1";
            button1.Size = new Size(103, 32);
            button1.TabIndex = 2;
            button1.Text = "📝 Add New";
            button1.UseVisualStyleBackColor = true;
            button1.Click += HandleSaveButtonClicked;
            // 
            // button2
            // 
            button2.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            button2.Location = new Point(112, 504);
            button2.Name = "button2";
            button2.Size = new Size(88, 32);
            button2.TabIndex = 3;
            button2.Text = "❌ Delete";
            button2.UseVisualStyleBackColor = true;
            button2.Click += HandleDeleteButtonClicked;
            // 
            // ListCategoryForm
            // 
            BackColor = Color.FromArgb(250, 250, 250);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(dataGridView1);
            Name = "ListCategoryForm";
            Size = new Size(500, 548);
            Load += HandleFormLoad;
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
        }

        #endregion
        private Button button1;
        private Button button2;
        private DataGridViewTextBoxColumn col_01;
        private DataGridViewTextBoxColumn col_02;
        private DataGridViewTextBoxColumn col_03;
        private DGrid dataGridView1;
    }
}