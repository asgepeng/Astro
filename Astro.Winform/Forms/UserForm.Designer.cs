namespace Astro.Winform.Forms
{
    partial class UserForm
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
            firstnameTextBox = new TextBox();
            lastnameTextBox = new TextBox();
            label2 = new Label();
            roleComboBox = new ComboBox();
            label3 = new Label();
            emailTextBox = new TextBox();
            label4 = new Label();
            phoneTextBox = new TextBox();
            label5 = new Label();
            dobTextBox = new DateTimePicker();
            label6 = new Label();
            sexComboBox = new ComboBox();
            label7 = new Label();
            label8 = new Label();
            maritalComboBox = new ComboBox();
            streetAddressTextBox = new TextBox();
            label9 = new Label();
            label10 = new Label();
            countryComboBox = new ComboBox();
            label11 = new Label();
            stateComboBox = new ComboBox();
            zipCodeTextBox = new TextBox();
            label12 = new Label();
            label13 = new Label();
            cityComboBox = new ComboBox();
            button1 = new Button();
            button2 = new Button();
            useExpirationCheckBox = new CheckBox();
            label14 = new Label();
            passwordExpirationDate = new DateTimePicker();
            lockoutEnableCheckBox = new CheckBox();
            button3 = new Button();
            listView1 = new ListView();
            columnHeader1 = new ColumnHeader();
            columnHeader2 = new ColumnHeader();
            columnHeader3 = new ColumnHeader();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(71, 17);
            label1.TabIndex = 0;
            label1.Text = "First Name";
            // 
            // firstnameTextBox
            // 
            firstnameTextBox.Location = new Point(12, 29);
            firstnameTextBox.Name = "firstnameTextBox";
            firstnameTextBox.Size = new Size(253, 25);
            firstnameTextBox.TabIndex = 1;
            // 
            // lastnameTextBox
            // 
            lastnameTextBox.Location = new Point(271, 29);
            lastnameTextBox.Name = "lastnameTextBox";
            lastnameTextBox.Size = new Size(253, 25);
            lastnameTextBox.TabIndex = 3;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(271, 9);
            label2.Name = "label2";
            label2.Size = new Size(70, 17);
            label2.TabIndex = 2;
            label2.Text = "Last Name";
            // 
            // roleComboBox
            // 
            roleComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            roleComboBox.FormattingEnabled = true;
            roleComboBox.Location = new Point(12, 78);
            roleComboBox.Name = "roleComboBox";
            roleComboBox.Size = new Size(512, 25);
            roleComboBox.TabIndex = 4;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(12, 57);
            label3.Name = "label3";
            label3.Size = new Size(34, 17);
            label3.TabIndex = 5;
            label3.Text = "Role";
            // 
            // emailTextBox
            // 
            emailTextBox.Location = new Point(12, 126);
            emailTextBox.Name = "emailTextBox";
            emailTextBox.Size = new Size(253, 25);
            emailTextBox.TabIndex = 7;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(12, 106);
            label4.Name = "label4";
            label4.Size = new Size(91, 17);
            label4.TabIndex = 6;
            label4.Text = "Email Address";
            // 
            // phoneTextBox
            // 
            phoneTextBox.Location = new Point(12, 174);
            phoneTextBox.Name = "phoneTextBox";
            phoneTextBox.Size = new Size(253, 25);
            phoneTextBox.TabIndex = 9;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(12, 154);
            label5.Name = "label5";
            label5.Size = new Size(96, 17);
            label5.TabIndex = 8;
            label5.Text = "Phone Number";
            // 
            // dobTextBox
            // 
            dobTextBox.CustomFormat = "dd/MM/yyyy";
            dobTextBox.Format = DateTimePickerFormat.Custom;
            dobTextBox.Location = new Point(12, 222);
            dobTextBox.Name = "dobTextBox";
            dobTextBox.Size = new Size(253, 25);
            dobTextBox.TabIndex = 10;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(12, 202);
            label6.Name = "label6";
            label6.Size = new Size(83, 17);
            label6.TabIndex = 11;
            label6.Text = "Date Of Birth";
            // 
            // sexComboBox
            // 
            sexComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            sexComboBox.FormattingEnabled = true;
            sexComboBox.Items.AddRange(new object[] { "Male", "Female" });
            sexComboBox.Location = new Point(271, 222);
            sexComboBox.Name = "sexComboBox";
            sexComboBox.Size = new Size(253, 25);
            sexComboBox.TabIndex = 12;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(271, 202);
            label7.Name = "label7";
            label7.Size = new Size(28, 17);
            label7.TabIndex = 13;
            label7.Text = "Sex";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(12, 250);
            label8.Name = "label8";
            label8.Size = new Size(88, 17);
            label8.TabIndex = 15;
            label8.Text = "Marital Status";
            // 
            // maritalComboBox
            // 
            maritalComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            maritalComboBox.FormattingEnabled = true;
            maritalComboBox.Items.AddRange(new object[] { "Single", "Maried", "Divorce" });
            maritalComboBox.Location = new Point(12, 270);
            maritalComboBox.Name = "maritalComboBox";
            maritalComboBox.Size = new Size(253, 25);
            maritalComboBox.TabIndex = 14;
            // 
            // streetAddressTextBox
            // 
            streetAddressTextBox.Location = new Point(12, 318);
            streetAddressTextBox.Name = "streetAddressTextBox";
            streetAddressTextBox.Size = new Size(512, 25);
            streetAddressTextBox.TabIndex = 17;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(12, 298);
            label9.Name = "label9";
            label9.Size = new Size(94, 17);
            label9.TabIndex = 16;
            label9.Text = "Street Address";
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new Point(12, 348);
            label10.Name = "label10";
            label10.Size = new Size(53, 17);
            label10.TabIndex = 19;
            label10.Text = "Country";
            // 
            // countryComboBox
            // 
            countryComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            countryComboBox.FormattingEnabled = true;
            countryComboBox.Location = new Point(12, 368);
            countryComboBox.Name = "countryComboBox";
            countryComboBox.Size = new Size(253, 25);
            countryComboBox.TabIndex = 18;
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Location = new Point(271, 348);
            label11.Name = "label11";
            label11.Size = new Size(107, 17);
            label11.TabIndex = 21;
            label11.Text = "State or Province";
            // 
            // stateComboBox
            // 
            stateComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            stateComboBox.FormattingEnabled = true;
            stateComboBox.Location = new Point(271, 368);
            stateComboBox.Name = "stateComboBox";
            stateComboBox.Size = new Size(253, 25);
            stateComboBox.TabIndex = 20;
            // 
            // zipCodeTextBox
            // 
            zipCodeTextBox.Location = new Point(271, 418);
            zipCodeTextBox.Name = "zipCodeTextBox";
            zipCodeTextBox.Size = new Size(96, 25);
            zipCodeTextBox.TabIndex = 23;
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Location = new Point(271, 398);
            label12.Name = "label12";
            label12.Size = new Size(61, 17);
            label12.TabIndex = 22;
            label12.Text = "Zip Code";
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Location = new Point(12, 398);
            label13.Name = "label13";
            label13.Size = new Size(29, 17);
            label13.TabIndex = 25;
            label13.Text = "City";
            // 
            // cityComboBox
            // 
            cityComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            cityComboBox.FormattingEnabled = true;
            cityComboBox.Location = new Point(12, 418);
            cityComboBox.Name = "cityComboBox";
            cityComboBox.Size = new Size(253, 25);
            cityComboBox.TabIndex = 24;
            // 
            // button1
            // 
            button1.Location = new Point(271, 126);
            button1.Name = "button1";
            button1.Size = new Size(253, 26);
            button1.TabIndex = 26;
            button1.Text = "Edit Login";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.Location = new Point(271, 173);
            button2.Name = "button2";
            button2.Size = new Size(253, 26);
            button2.TabIndex = 27;
            button2.Text = "Verify Phone Number";
            button2.UseVisualStyleBackColor = true;
            // 
            // useExpirationCheckBox
            // 
            useExpirationCheckBox.AutoSize = true;
            useExpirationCheckBox.Location = new Point(12, 466);
            useExpirationCheckBox.Name = "useExpirationCheckBox";
            useExpirationCheckBox.Size = new Size(171, 21);
            useExpirationCheckBox.TabIndex = 28;
            useExpirationCheckBox.Text = "Use Password Expiration";
            useExpirationCheckBox.UseVisualStyleBackColor = true;
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Location = new Point(12, 491);
            label14.Name = "label14";
            label14.Size = new Size(157, 17);
            label14.TabIndex = 30;
            label14.Text = "Password Expiration Date";
            // 
            // passwordExpirationDate
            // 
            passwordExpirationDate.CustomFormat = "dd/MM/yyyy HH:mm";
            passwordExpirationDate.Format = DateTimePickerFormat.Custom;
            passwordExpirationDate.Location = new Point(12, 511);
            passwordExpirationDate.Name = "passwordExpirationDate";
            passwordExpirationDate.Size = new Size(154, 25);
            passwordExpirationDate.TabIndex = 29;
            // 
            // lockoutEnableCheckBox
            // 
            lockoutEnableCheckBox.AutoSize = true;
            lockoutEnableCheckBox.Location = new Point(271, 466);
            lockoutEnableCheckBox.Name = "lockoutEnableCheckBox";
            lockoutEnableCheckBox.Size = new Size(115, 21);
            lockoutEnableCheckBox.TabIndex = 31;
            lockoutEnableCheckBox.Text = "Lockout enable";
            lockoutEnableCheckBox.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            button3.Location = new Point(12, 548);
            button3.Name = "button3";
            button3.Size = new Size(154, 35);
            button3.TabIndex = 32;
            button3.Text = "Simpan";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // listView1
            // 
            listView1.Columns.AddRange(new ColumnHeader[] { columnHeader1, columnHeader2, columnHeader3 });
            listView1.Location = new Point(575, 84);
            listView1.Name = "listView1";
            listView1.Size = new Size(500, 179);
            listView1.TabIndex = 33;
            listView1.UseCompatibleStateImageBehavior = false;
            listView1.View = View.Details;
            // 
            // UserForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1105, 595);
            Controls.Add(listView1);
            Controls.Add(button3);
            Controls.Add(lockoutEnableCheckBox);
            Controls.Add(label14);
            Controls.Add(passwordExpirationDate);
            Controls.Add(useExpirationCheckBox);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(label13);
            Controls.Add(cityComboBox);
            Controls.Add(zipCodeTextBox);
            Controls.Add(label12);
            Controls.Add(label11);
            Controls.Add(stateComboBox);
            Controls.Add(label10);
            Controls.Add(countryComboBox);
            Controls.Add(streetAddressTextBox);
            Controls.Add(label9);
            Controls.Add(label8);
            Controls.Add(maritalComboBox);
            Controls.Add(label7);
            Controls.Add(sexComboBox);
            Controls.Add(label6);
            Controls.Add(dobTextBox);
            Controls.Add(phoneTextBox);
            Controls.Add(label5);
            Controls.Add(emailTextBox);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(roleComboBox);
            Controls.Add(lastnameTextBox);
            Controls.Add(label2);
            Controls.Add(firstnameTextBox);
            Controls.Add(label1);
            Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "UserForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "User";
            Load += UserForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private TextBox firstnameTextBox;
        private TextBox lastnameTextBox;
        private Label label2;
        private ComboBox roleComboBox;
        private Label label3;
        private TextBox emailTextBox;
        private Label label4;
        private TextBox phoneTextBox;
        private Label label5;
        private DateTimePicker dobTextBox;
        private Label label6;
        private ComboBox sexComboBox;
        private Label label7;
        private Label label8;
        private ComboBox maritalComboBox;
        private TextBox streetAddressTextBox;
        private Label label9;
        private Label label10;
        private ComboBox countryComboBox;
        private Label label11;
        private ComboBox stateComboBox;
        private TextBox zipCodeTextBox;
        private Label label12;
        private Label label13;
        private ComboBox cityComboBox;
        private Button button1;
        private Button button2;
        private CheckBox useExpirationCheckBox;
        private Label label14;
        private DateTimePicker passwordExpirationDate;
        private CheckBox lockoutEnableCheckBox;
        private Button button3;
        private ListView listView1;
        private ColumnHeader columnHeader1;
        private ColumnHeader columnHeader2;
        private ColumnHeader columnHeader3;
    }
}