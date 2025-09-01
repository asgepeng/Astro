using Astro.Winform.Controls;

namespace Astro.Winform.Forms
{
    partial class AccountForm
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
            tabControl1 = new FlatTabControl();
            tabPage1 = new TabPage();
            label4 = new Label();
            accountProviderComboBox = new ComboBox();
            label3 = new Label();
            accountTypeComboBox = new ComboBox();
            label2 = new Label();
            accountNumberTextBox = new TextBox();
            label1 = new Label();
            accountNameTextBox = new TextBox();
            tabPage2 = new TabPage();
            button1 = new Button();
            label5 = new Label();
            panel1 = new Panel();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // tabControl1
            // 
            tabControl1.ActiveColor = Color.DeepSkyBlue;
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Dock = DockStyle.Fill;
            tabControl1.DrawMode = TabDrawMode.OwnerDrawFixed;
            tabControl1.InactiveColor = Color.LightGray;
            tabControl1.ItemSize = new Size(120, 40);
            tabControl1.Location = new Point(0, 40);
            tabControl1.Name = "tabControl1";
            tabControl1.Padding = new Point(6, 6);
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(400, 431);
            tabControl1.TabIndex = 0;
            tabControl1.TextColor = Color.Black;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(label4);
            tabPage1.Controls.Add(accountProviderComboBox);
            tabPage1.Controls.Add(label3);
            tabPage1.Controls.Add(accountTypeComboBox);
            tabPage1.Controls.Add(label2);
            tabPage1.Controls.Add(accountNumberTextBox);
            tabPage1.Controls.Add(label1);
            tabPage1.Controls.Add(accountNameTextBox);
            tabPage1.Location = new Point(4, 44);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(392, 383);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "ℹ️ General";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(13, 182);
            label4.Margin = new Padding(0, 0, 3, 0);
            label4.Name = "label4";
            label4.Size = new Size(92, 17);
            label4.TabIndex = 8;
            label4.Text = "Penyedia Akun";
            // 
            // accountProviderComboBox
            // 
            accountProviderComboBox.DisplayMember = "Name";
            accountProviderComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            accountProviderComboBox.FormattingEnabled = true;
            accountProviderComboBox.Location = new Point(16, 202);
            accountProviderComboBox.Name = "accountProviderComboBox";
            accountProviderComboBox.Size = new Size(360, 25);
            accountProviderComboBox.TabIndex = 7;
            accountProviderComboBox.ValueMember = "Id";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(13, 129);
            label3.Margin = new Padding(0, 0, 3, 0);
            label3.Name = "label3";
            label3.Size = new Size(68, 17);
            label3.TabIndex = 6;
            label3.Text = "Jenis Akun";
            // 
            // accountTypeComboBox
            // 
            accountTypeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            accountTypeComboBox.FormattingEnabled = true;
            accountTypeComboBox.Items.AddRange(new object[] { "Bank", "E-Wallet", "E-Money" });
            accountTypeComboBox.Location = new Point(16, 149);
            accountTypeComboBox.Name = "accountTypeComboBox";
            accountTypeComboBox.Size = new Size(360, 25);
            accountTypeComboBox.TabIndex = 5;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 76);
            label2.Margin = new Padding(0, 0, 3, 0);
            label2.Name = "label2";
            label2.Size = new Size(107, 17);
            label2.TabIndex = 3;
            label2.Text = "Nomor Rekening";
            // 
            // accountNumberTextBox
            // 
            accountNumberTextBox.Location = new Point(15, 96);
            accountNumberTextBox.Name = "accountNumberTextBox";
            accountNumberTextBox.Size = new Size(360, 25);
            accountNumberTextBox.TabIndex = 4;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 23);
            label1.Margin = new Padding(0, 0, 3, 0);
            label1.Name = "label1";
            label1.Size = new Size(129, 17);
            label1.TabIndex = 1;
            label1.Text = "Atas Nama Rekening";
            // 
            // accountNameTextBox
            // 
            accountNameTextBox.Location = new Point(15, 43);
            accountNameTextBox.Name = "accountNameTextBox";
            accountNameTextBox.Size = new Size(360, 25);
            accountNameTextBox.TabIndex = 2;
            // 
            // tabPage2
            // 
            tabPage2.Location = new Point(4, 44);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(392, 383);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "🕒 Histories";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            button1.BackColor = Color.Blue;
            button1.FlatAppearance.BorderSize = 0;
            button1.FlatStyle = FlatStyle.Flat;
            button1.ForeColor = Color.White;
            button1.Location = new Point(19, 15);
            button1.Name = "button1";
            button1.Size = new Size(359, 30);
            button1.TabIndex = 1;
            button1.Text = "💾 Save";
            button1.UseVisualStyleBackColor = false;
            button1.Click += button1_Click;
            // 
            // label5
            // 
            label5.Dock = DockStyle.Top;
            label5.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label5.Location = new Point(0, 0);
            label5.Name = "label5";
            label5.Padding = new Padding(5, 0, 0, 0);
            label5.Size = new Size(400, 40);
            label5.TabIndex = 2;
            label5.Text = "Detil Rekening";
            label5.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // panel1
            // 
            panel1.Controls.Add(button1);
            panel1.Dock = DockStyle.Bottom;
            panel1.Location = new Point(0, 416);
            panel1.Name = "panel1";
            panel1.Size = new Size(400, 55);
            panel1.TabIndex = 3;
            // 
            // AccountForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(250, 250, 250);
            BorderStyle = BorderStyle.FixedSingle;
            Controls.Add(panel1);
            Controls.Add(tabControl1);
            Controls.Add(label5);
            Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Name = "AccountForm";
            Size = new Size(400, 471);
            Load += AccountForm_Load;
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage1.PerformLayout();
            panel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private TabPage tabPage1;
        private Label label4;
        private ComboBox accountProviderComboBox;
        private Label label3;
        private ComboBox accountTypeComboBox;
        private Label label2;
        private TextBox accountNumberTextBox;
        private Label label1;
        private TextBox accountNameTextBox;
        private TabPage tabPage2;
        private Button button1;
        private Label label5;
        private Panel panel1;
        private FlatTabControl tabControl1;
    }
}