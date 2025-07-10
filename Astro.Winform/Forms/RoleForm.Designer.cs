namespace Astro.Winform.Forms
{
    partial class RoleForm
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
            components = new System.ComponentModel.Container();
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            saveButton = new Button();
            label1 = new Label();
            rolenameTextBox = new TextBox();
            tabControl1 = new TabControl();
            tabPage1 = new TabPage();
            dataGridView1 = new DataGridView();
            nameDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            allowCreateDataGridViewCheckBoxColumn = new DataGridViewCheckBoxColumn();
            allowReadDataGridViewCheckBoxColumn = new DataGridViewCheckBoxColumn();
            allowEditDataGridViewCheckBoxColumn = new DataGridViewCheckBoxColumn();
            allowDeleteDataGridViewCheckBoxColumn = new DataGridViewCheckBoxColumn();
            permissionBindingSource = new BindingSource(components);
            tabPage2 = new TabPage();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)permissionBindingSource).BeginInit();
            SuspendLayout();
            // 
            // saveButton
            // 
            saveButton.Location = new Point(12, 475);
            saveButton.Name = "saveButton";
            saveButton.Size = new Size(75, 23);
            saveButton.TabIndex = 0;
            saveButton.Text = "button1";
            saveButton.UseVisualStyleBackColor = true;
            saveButton.Click += HandleSaveButtonClicked;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 15);
            label1.Name = "label1";
            label1.Size = new Size(73, 17);
            label1.TabIndex = 1;
            label1.Text = "Role Name";
            // 
            // rolenameTextBox
            // 
            rolenameTextBox.Location = new Point(132, 12);
            rolenameTextBox.Name = "rolenameTextBox";
            rolenameTextBox.Size = new Size(387, 25);
            rolenameTextBox.TabIndex = 2;
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Location = new Point(12, 51);
            tabControl1.Name = "tabControl1";
            tabControl1.Padding = new Point(6, 6);
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(776, 418);
            tabControl1.TabIndex = 3;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(dataGridView1);
            tabPage1.Location = new Point(4, 32);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(768, 382);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "🛡️ Permission";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // dataGridView1
            // 
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.AllowUserToResizeRows = false;
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.BackgroundColor = Color.White;
            dataGridView1.BorderStyle = BorderStyle.Fixed3D;
            dataGridView1.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridView1.Columns.AddRange(new DataGridViewColumn[] { nameDataGridViewTextBoxColumn, allowCreateDataGridViewCheckBoxColumn, allowReadDataGridViewCheckBoxColumn, allowEditDataGridViewCheckBoxColumn, allowDeleteDataGridViewCheckBoxColumn });
            dataGridView1.DataSource = permissionBindingSource;
            dataGridView1.Dock = DockStyle.Fill;
            dataGridView1.GridColor = Color.FromArgb(230, 230, 230);
            dataGridView1.Location = new Point(3, 3);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.Size = new Size(762, 376);
            dataGridView1.TabIndex = 0;
            // 
            // nameDataGridViewTextBoxColumn
            // 
            nameDataGridViewTextBoxColumn.DataPropertyName = "Name";
            dataGridViewCellStyle1.BackColor = SystemColors.ButtonFace;
            nameDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle1;
            nameDataGridViewTextBoxColumn.HeaderText = "Name";
            nameDataGridViewTextBoxColumn.Name = "nameDataGridViewTextBoxColumn";
            nameDataGridViewTextBoxColumn.ReadOnly = true;
            nameDataGridViewTextBoxColumn.Width = 250;
            // 
            // allowCreateDataGridViewCheckBoxColumn
            // 
            allowCreateDataGridViewCheckBoxColumn.DataPropertyName = "AllowCreate";
            allowCreateDataGridViewCheckBoxColumn.HeaderText = "Allow Create";
            allowCreateDataGridViewCheckBoxColumn.Name = "allowCreateDataGridViewCheckBoxColumn";
            // 
            // allowReadDataGridViewCheckBoxColumn
            // 
            allowReadDataGridViewCheckBoxColumn.DataPropertyName = "AllowRead";
            allowReadDataGridViewCheckBoxColumn.HeaderText = "Allow Read";
            allowReadDataGridViewCheckBoxColumn.Name = "allowReadDataGridViewCheckBoxColumn";
            // 
            // allowEditDataGridViewCheckBoxColumn
            // 
            allowEditDataGridViewCheckBoxColumn.DataPropertyName = "AllowEdit";
            allowEditDataGridViewCheckBoxColumn.HeaderText = "Allow Edit";
            allowEditDataGridViewCheckBoxColumn.Name = "allowEditDataGridViewCheckBoxColumn";
            // 
            // allowDeleteDataGridViewCheckBoxColumn
            // 
            allowDeleteDataGridViewCheckBoxColumn.DataPropertyName = "AllowDelete";
            allowDeleteDataGridViewCheckBoxColumn.HeaderText = "Allow Delete";
            allowDeleteDataGridViewCheckBoxColumn.Name = "allowDeleteDataGridViewCheckBoxColumn";
            // 
            // permissionBindingSource
            // 
            permissionBindingSource.DataSource = typeof(Models.Permission);
            // 
            // tabPage2
            // 
            tabPage2.Location = new Point(4, 30);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(768, 384);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "🕒 Change Log";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // RoleForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 510);
            Controls.Add(tabControl1);
            Controls.Add(rolenameTextBox);
            Controls.Add(label1);
            Controls.Add(saveButton);
            Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Name = "RoleForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "RoleForm";
            Load += HandleFormLoad;
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ((System.ComponentModel.ISupportInitialize)permissionBindingSource).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button saveButton;
        private Label label1;
        private TextBox rolenameTextBox;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private DataGridView dataGridView1;
        private BindingSource permissionBindingSource;
        private DataGridViewTextBoxColumn nameDataGridViewTextBoxColumn;
        private DataGridViewCheckBoxColumn allowCreateDataGridViewCheckBoxColumn;
        private DataGridViewCheckBoxColumn allowReadDataGridViewCheckBoxColumn;
        private DataGridViewCheckBoxColumn allowEditDataGridViewCheckBoxColumn;
        private DataGridViewCheckBoxColumn allowDeleteDataGridViewCheckBoxColumn;
    }
}