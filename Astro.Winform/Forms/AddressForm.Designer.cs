namespace Astro.Winform.Forms
{
    partial class AddressForm
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
            label1 = new Label();
            typeComboBox = new ComboBox();
            label2 = new Label();
            streetTextBox = new TextBox();
            stateComboBox = new ComboBox();
            label3 = new Label();
            cityComboBox = new ComboBox();
            label4 = new Label();
            districtComboBox = new ComboBox();
            label5 = new Label();
            label6 = new Label();
            zipCodeTextBox = new TextBox();
            button1 = new Button();
            primaryCheckBox = new CheckBox();
            villageComboBox = new ComboBox();
            label7 = new Label();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(87, 17);
            label1.TabIndex = 0;
            label1.Text = "Address Type";
            // 
            // typeComboBox
            // 
            typeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            typeComboBox.FormattingEnabled = true;
            typeComboBox.Items.AddRange(new object[] { "Home", "Office" });
            typeComboBox.Location = new Point(12, 29);
            typeComboBox.Name = "typeComboBox";
            typeComboBox.Size = new Size(310, 25);
            typeComboBox.TabIndex = 1;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 57);
            label2.Name = "label2";
            label2.Size = new Size(94, 17);
            label2.TabIndex = 2;
            label2.Text = "Street Address";
            // 
            // streetTextBox
            // 
            streetTextBox.Location = new Point(12, 77);
            streetTextBox.Multiline = true;
            streetTextBox.Name = "streetTextBox";
            streetTextBox.ScrollBars = ScrollBars.Both;
            streetTextBox.Size = new Size(310, 60);
            streetTextBox.TabIndex = 3;
            // 
            // countryComboBox
            // 
            stateComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            stateComboBox.FormattingEnabled = true;
            stateComboBox.Location = new Point(12, 160);
            stateComboBox.Name = "countryComboBox";
            stateComboBox.Size = new Size(310, 25);
            stateComboBox.TabIndex = 5;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(12, 140);
            label3.Name = "label3";
            label3.Size = new Size(55, 17);
            label3.TabIndex = 4;
            label3.Text = "Propinsi";
            // 
            // stateComboBox
            // 
            cityComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            cityComboBox.FormattingEnabled = true;
            cityComboBox.Location = new Point(12, 208);
            cityComboBox.Name = "stateComboBox";
            cityComboBox.Size = new Size(310, 25);
            cityComboBox.TabIndex = 7;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(12, 188);
            label4.Name = "label4";
            label4.Size = new Size(111, 17);
            label4.TabIndex = 6;
            label4.Text = "Kabupaten / Kota";
            // 
            // cityComboBox
            // 
            districtComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            districtComboBox.FormattingEnabled = true;
            districtComboBox.Location = new Point(12, 256);
            districtComboBox.Name = "cityComboBox";
            districtComboBox.Size = new Size(310, 25);
            districtComboBox.TabIndex = 9;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(12, 236);
            label5.Name = "label5";
            label5.Size = new Size(72, 17);
            label5.TabIndex = 8;
            label5.Text = "Kecamatan";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(12, 339);
            label6.Name = "label6";
            label6.Size = new Size(64, 17);
            label6.TabIndex = 10;
            label6.Text = "Kode Pos";
            // 
            // zipCodeTextBox
            // 
            zipCodeTextBox.Location = new Point(12, 359);
            zipCodeTextBox.Name = "zipCodeTextBox";
            zipCodeTextBox.Size = new Size(310, 25);
            zipCodeTextBox.TabIndex = 12;
            // 
            // button1
            // 
            button1.Location = new Point(12, 417);
            button1.Name = "button1";
            button1.Size = new Size(100, 30);
            button1.TabIndex = 13;
            button1.Text = "💾 Save";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // primaryCheckBox
            // 
            primaryCheckBox.AutoSize = true;
            primaryCheckBox.Location = new Point(12, 390);
            primaryCheckBox.Name = "primaryCheckBox";
            primaryCheckBox.Size = new Size(123, 21);
            primaryCheckBox.TabIndex = 14;
            primaryCheckBox.Text = "Primary Address";
            primaryCheckBox.UseVisualStyleBackColor = true;
            // 
            // villageComboBox
            // 
            villageComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            villageComboBox.FormattingEnabled = true;
            villageComboBox.Location = new Point(12, 304);
            villageComboBox.Name = "villageComboBox";
            villageComboBox.Size = new Size(310, 25);
            villageComboBox.TabIndex = 16;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(12, 284);
            label7.Name = "label7";
            label7.Size = new Size(108, 17);
            label7.TabIndex = 15;
            label7.Text = "Desa / Kelurahan";
            // 
            // AddressForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(250, 250, 250);
            ClientSize = new Size(334, 459);
            Controls.Add(villageComboBox);
            Controls.Add(label7);
            Controls.Add(primaryCheckBox);
            Controls.Add(button1);
            Controls.Add(zipCodeTextBox);
            Controls.Add(label6);
            Controls.Add(districtComboBox);
            Controls.Add(label5);
            Controls.Add(cityComboBox);
            Controls.Add(label4);
            Controls.Add(stateComboBox);
            Controls.Add(label3);
            Controls.Add(streetTextBox);
            Controls.Add(label2);
            Controls.Add(typeComboBox);
            Controls.Add(label1);
            Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "AddressForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Address";
            Load += AddressForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private ComboBox typeComboBox;
        private Label label2;
        private TextBox streetTextBox;
        private ComboBox stateComboBox;
        private Label label3;
        private ComboBox cityComboBox;
        private Label label4;
        private ComboBox districtComboBox;
        private Label label5;
        private Label label6;
        private TextBox zipCodeTextBox;
        private Button button1;
        private CheckBox primaryCheckBox;
        private ComboBox villageComboBox;
        private Label label7;
    }
}