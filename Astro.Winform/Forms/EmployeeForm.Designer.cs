using Astro.Winform.Controls;

namespace Astro.Winform.Forms
{
    partial class EmployeeForm
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
            checkBox1 = new CheckBox();
            groupBox1 = new GroupBox();
            label2 = new Label();
            villageComboBox = new ComboBox();
            label7 = new Label();
            districtComboBox = new ComboBox();
            label3 = new Label();
            cityComboBox = new ComboBox();
            label4 = new Label();
            provinceComboBox = new ComboBox();
            label6 = new Label();
            zipCodeTextBox = new TextBox();
            label5 = new Label();
            streetTextBox = new TextBox();
            label16 = new Label();
            phoneTextBox = new TextBox();
            label13 = new Label();
            emailTextBox = new TextBox();
            label17 = new Label();
            comboBox7 = new ComboBox();
            label12 = new Label();
            textBox6 = new TextBox();
            label15 = new Label();
            label10 = new Label();
            maritalComboBox = new ComboBox();
            label9 = new Label();
            sexComboBox = new ComboBox();
            bodDatePicker = new DateTimePicker();
            bopTextBox = new TextBox();
            label8 = new Label();
            fullnameTextBox = new TextBox();
            label1 = new Label();
            tabPage2 = new TabPage();
            saveButton = new Button();
            formLabel = new Label();
            panel1 = new Panel();
            label11 = new Label();
            comboBox1 = new ComboBox();
            label14 = new Label();
            comboBox2 = new ComboBox();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            groupBox1.SuspendLayout();
            tabPage2.SuspendLayout();
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
            tabControl1.Padding = new Point(10, 6);
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(400, 727);
            tabControl1.TabIndex = 0;
            tabControl1.TextColor = Color.Black;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(checkBox1);
            tabPage1.Controls.Add(groupBox1);
            tabPage1.Controls.Add(label16);
            tabPage1.Controls.Add(phoneTextBox);
            tabPage1.Controls.Add(label13);
            tabPage1.Controls.Add(emailTextBox);
            tabPage1.Controls.Add(label17);
            tabPage1.Controls.Add(comboBox7);
            tabPage1.Controls.Add(label12);
            tabPage1.Controls.Add(textBox6);
            tabPage1.Controls.Add(label15);
            tabPage1.Controls.Add(label10);
            tabPage1.Controls.Add(maritalComboBox);
            tabPage1.Controls.Add(label9);
            tabPage1.Controls.Add(sexComboBox);
            tabPage1.Controls.Add(bodDatePicker);
            tabPage1.Controls.Add(bopTextBox);
            tabPage1.Controls.Add(label8);
            tabPage1.Controls.Add(fullnameTextBox);
            tabPage1.Controls.Add(label1);
            tabPage1.Location = new Point(4, 44);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(392, 679);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "ℹ️ Umum";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Location = new Point(15, 482);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(52, 21);
            checkBox1.TabIndex = 70;
            checkBox1.Text = "Aktif";
            checkBox1.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(villageComboBox);
            groupBox1.Controls.Add(label7);
            groupBox1.Controls.Add(districtComboBox);
            groupBox1.Controls.Add(label3);
            groupBox1.Controls.Add(cityComboBox);
            groupBox1.Controls.Add(label4);
            groupBox1.Controls.Add(provinceComboBox);
            groupBox1.Controls.Add(label6);
            groupBox1.Controls.Add(zipCodeTextBox);
            groupBox1.Controls.Add(label5);
            groupBox1.Controls.Add(streetTextBox);
            groupBox1.Location = new Point(15, 230);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(360, 237);
            groupBox1.TabIndex = 69;
            groupBox1.TabStop = false;
            groupBox1.Text = "Alamat Lengkap";
            // 
            // label2
            // 
            label2.Location = new Point(6, 21);
            label2.Name = "label2";
            label2.Size = new Size(114, 50);
            label2.TabIndex = 57;
            label2.Text = "Nama Jalan / Lingkungan";
            // 
            // villageComboBox
            // 
            villageComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            villageComboBox.FormattingEnabled = true;
            villageComboBox.Location = new Point(150, 170);
            villageComboBox.Name = "villageComboBox";
            villageComboBox.Size = new Size(204, 25);
            villageComboBox.TabIndex = 67;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(6, 173);
            label7.Name = "label7";
            label7.Size = new Size(108, 17);
            label7.TabIndex = 68;
            label7.Text = "Kelurahan / Desa";
            // 
            // districtComboBox
            // 
            districtComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            districtComboBox.FormattingEnabled = true;
            districtComboBox.Location = new Point(150, 139);
            districtComboBox.Name = "districtComboBox";
            districtComboBox.Size = new Size(204, 25);
            districtComboBox.TabIndex = 65;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(6, 204);
            label3.Name = "label3";
            label3.Size = new Size(64, 17);
            label3.TabIndex = 59;
            label3.Text = "Kode Pos";
            // 
            // cityComboBox
            // 
            cityComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            cityComboBox.FormattingEnabled = true;
            cityComboBox.Location = new Point(150, 108);
            cityComboBox.Name = "cityComboBox";
            cityComboBox.Size = new Size(204, 25);
            cityComboBox.TabIndex = 63;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(6, 80);
            label4.Name = "label4";
            label4.Size = new Size(55, 17);
            label4.TabIndex = 62;
            label4.Text = "Propinsi";
            // 
            // provinceComboBox
            // 
            provinceComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            provinceComboBox.FormattingEnabled = true;
            provinceComboBox.Location = new Point(150, 77);
            provinceComboBox.Name = "provinceComboBox";
            provinceComboBox.Size = new Size(204, 25);
            provinceComboBox.TabIndex = 61;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(6, 142);
            label6.Name = "label6";
            label6.Size = new Size(72, 17);
            label6.TabIndex = 66;
            label6.Text = "Kecamatan";
            // 
            // zipCodeTextBox
            // 
            zipCodeTextBox.Location = new Point(150, 201);
            zipCodeTextBox.Name = "zipCodeTextBox";
            zipCodeTextBox.Size = new Size(107, 25);
            zipCodeTextBox.TabIndex = 60;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(6, 111);
            label5.Name = "label5";
            label5.Size = new Size(111, 17);
            label5.TabIndex = 64;
            label5.Text = "Kota / Kabupaten";
            // 
            // streetTextBox
            // 
            streetTextBox.Location = new Point(150, 21);
            streetTextBox.Multiline = true;
            streetTextBox.Name = "streetTextBox";
            streetTextBox.ScrollBars = ScrollBars.Both;
            streetTextBox.Size = new Size(204, 50);
            streetTextBox.TabIndex = 58;
            // 
            // label16
            // 
            label16.AutoSize = true;
            label16.Location = new Point(260, 174);
            label16.Name = "label16";
            label16.Size = new Size(100, 17);
            label16.TabIndex = 56;
            label16.Text = "Nomor Telepon";
            // 
            // phoneTextBox
            // 
            phoneTextBox.Location = new Point(261, 194);
            phoneTextBox.MaxLength = 16;
            phoneTextBox.Name = "phoneTextBox";
            phoneTextBox.Size = new Size(114, 25);
            phoneTextBox.TabIndex = 55;
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Location = new Point(12, 174);
            label13.Name = "label13";
            label13.Size = new Size(83, 17);
            label13.TabIndex = 54;
            label13.Text = "Alamat Email";
            // 
            // emailTextBox
            // 
            emailTextBox.Location = new Point(15, 194);
            emailTextBox.MaxLength = 16;
            emailTextBox.Name = "emailTextBox";
            emailTextBox.Size = new Size(240, 25);
            emailTextBox.TabIndex = 53;
            // 
            // label17
            // 
            label17.AutoSize = true;
            label17.Location = new Point(126, 506);
            label17.Name = "label17";
            label17.Size = new Size(92, 17);
            label17.TabIndex = 46;
            label17.Text = "Role / Jabatan";
            // 
            // comboBox7
            // 
            comboBox7.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox7.FormattingEnabled = true;
            comboBox7.Items.AddRange(new object[] { "Laki-laki", "Perempuan" });
            comboBox7.Location = new Point(129, 526);
            comboBox7.Name = "comboBox7";
            comboBox7.Size = new Size(246, 25);
            comboBox7.TabIndex = 45;
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Location = new Point(12, 506);
            label12.Name = "label12";
            label12.Size = new Size(96, 17);
            label12.TabIndex = 44;
            label12.Text = "Tanggal Masuk";
            // 
            // textBox6
            // 
            textBox6.BackColor = Color.White;
            textBox6.Location = new Point(15, 526);
            textBox6.MaxLength = 16;
            textBox6.Name = "textBox6";
            textBox6.ReadOnly = true;
            textBox6.Size = new Size(108, 25);
            textBox6.TabIndex = 43;
            // 
            // label15
            // 
            label15.AutoSize = true;
            label15.Location = new Point(260, 68);
            label15.Name = "label15";
            label15.Size = new Size(83, 17);
            label15.TabIndex = 42;
            label15.Text = "Tempat Lahir";
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new Point(195, 121);
            label10.Name = "label10";
            label10.Size = new Size(110, 17);
            label10.TabIndex = 35;
            label10.Text = "Status Pernikahan";
            // 
            // maritalComboBox
            // 
            maritalComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            maritalComboBox.FormattingEnabled = true;
            maritalComboBox.Items.AddRange(new object[] { "Single", "Maried", "Divorce" });
            maritalComboBox.Location = new Point(198, 141);
            maritalComboBox.Name = "maritalComboBox";
            maritalComboBox.Size = new Size(177, 25);
            maritalComboBox.TabIndex = 34;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(12, 121);
            label9.Name = "label9";
            label9.Size = new Size(86, 17);
            label9.TabIndex = 33;
            label9.Text = "Jenis Kelamin";
            // 
            // sexComboBox
            // 
            sexComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            sexComboBox.FormattingEnabled = true;
            sexComboBox.Items.AddRange(new object[] { "Laki-laki", "Perempuan" });
            sexComboBox.Location = new Point(15, 141);
            sexComboBox.Name = "sexComboBox";
            sexComboBox.Size = new Size(177, 25);
            sexComboBox.TabIndex = 32;
            // 
            // bodDatePicker
            // 
            bodDatePicker.CustomFormat = "dd/MM/yyyy";
            bodDatePicker.Format = DateTimePickerFormat.Custom;
            bodDatePicker.Location = new Point(261, 88);
            bodDatePicker.Name = "bodDatePicker";
            bodDatePicker.Size = new Size(114, 25);
            bodDatePicker.TabIndex = 30;
            // 
            // bopTextBox
            // 
            bopTextBox.Location = new Point(15, 88);
            bopTextBox.Name = "bopTextBox";
            bopTextBox.Size = new Size(240, 25);
            bopTextBox.TabIndex = 29;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(12, 68);
            label8.Name = "label8";
            label8.Size = new Size(83, 17);
            label8.TabIndex = 28;
            label8.Text = "Tempat Lahir";
            // 
            // fullnameTextBox
            // 
            fullnameTextBox.Location = new Point(15, 35);
            fullnameTextBox.Name = "fullnameTextBox";
            fullnameTextBox.Size = new Size(360, 25);
            fullnameTextBox.TabIndex = 15;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 15);
            label1.Name = "label1";
            label1.Size = new Size(96, 17);
            label1.TabIndex = 14;
            label1.Text = "Nama Lengkap";
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(label14);
            tabPage2.Controls.Add(comboBox2);
            tabPage2.Controls.Add(label11);
            tabPage2.Controls.Add(comboBox1);
            tabPage2.Location = new Point(4, 44);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(392, 679);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "💳 Payroll";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // saveButton
            // 
            saveButton.BackColor = Color.Blue;
            saveButton.FlatAppearance.BorderSize = 0;
            saveButton.FlatStyle = FlatStyle.Flat;
            saveButton.ForeColor = Color.White;
            saveButton.Location = new Point(19, 10);
            saveButton.Name = "saveButton";
            saveButton.Size = new Size(360, 30);
            saveButton.TabIndex = 1;
            saveButton.Text = "Simpan";
            saveButton.UseVisualStyleBackColor = false;
            // 
            // formLabel
            // 
            formLabel.Dock = DockStyle.Top;
            formLabel.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point, 0);
            formLabel.Location = new Point(0, 0);
            formLabel.Name = "formLabel";
            formLabel.Padding = new Padding(5, 0, 0, 0);
            formLabel.Size = new Size(400, 40);
            formLabel.TabIndex = 2;
            formLabel.Text = "Detail Pegawai";
            formLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // panel1
            // 
            panel1.Controls.Add(saveButton);
            panel1.Dock = DockStyle.Bottom;
            panel1.Location = new Point(0, 717);
            panel1.Name = "panel1";
            panel1.Size = new Size(400, 50);
            panel1.TabIndex = 3;
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Location = new Point(14, 15);
            label11.Name = "label11";
            label11.Size = new Size(137, 17);
            label11.TabIndex = 35;
            label11.Text = "Methode Pembayaran";
            // 
            // comboBox1
            // 
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox1.FormattingEnabled = true;
            comboBox1.Items.AddRange(new object[] { "Laki-laki", "Perempuan" });
            comboBox1.Location = new Point(15, 35);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(237, 25);
            comboBox1.TabIndex = 34;
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Location = new Point(258, 15);
            label14.Name = "label14";
            label14.Size = new Size(94, 17);
            label14.TabIndex = 37;
            label14.Text = "Setiap Tanggal";
            // 
            // comboBox2
            // 
            comboBox2.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox2.FormattingEnabled = true;
            comboBox2.Items.AddRange(new object[] { "Laki-laki", "Perempuan" });
            comboBox2.Location = new Point(259, 35);
            comboBox2.Name = "comboBox2";
            comboBox2.Size = new Size(116, 25);
            comboBox2.TabIndex = 36;
            // 
            // EmployeeForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(250, 250, 250);
            BorderStyle = BorderStyle.FixedSingle;
            Controls.Add(panel1);
            Controls.Add(tabControl1);
            Controls.Add(formLabel);
            Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Name = "EmployeeForm";
            Size = new Size(400, 767);
            Load += LoadEmployeeData;
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage1.PerformLayout();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            tabPage2.ResumeLayout(false);
            tabPage2.PerformLayout();
            panel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private TabPage tabPage1;
        private TabPage tabPage2;
        private Label label10;
        private ComboBox maritalComboBox;
        private Label label9;
        private ComboBox sexComboBox;
        private DateTimePicker bodDatePicker;
        private TextBox bopTextBox;
        private Label label8;
        private TextBox fullnameTextBox;
        private Label label1;
        private Button saveButton;
        private Label formLabel;
        private Panel panel1;
        private FlatTabControl tabControl1;
        private Label label15;
        private Label label12;
        private TextBox textBox6;
        private Label label17;
        private ComboBox comboBox7;
        private Label label16;
        private TextBox phoneTextBox;
        private Label label13;
        private TextBox emailTextBox;
        private Label label7;
        private ComboBox villageComboBox;
        private Label label6;
        private ComboBox districtComboBox;
        private Label label5;
        private ComboBox cityComboBox;
        private Label label4;
        private ComboBox provinceComboBox;
        private TextBox zipCodeTextBox;
        private Label label3;
        private TextBox streetTextBox;
        private Label label2;
        private GroupBox groupBox1;
        private CheckBox checkBox1;
        private Label label14;
        private ComboBox comboBox2;
        private Label label11;
        private ComboBox comboBox1;
    }
}