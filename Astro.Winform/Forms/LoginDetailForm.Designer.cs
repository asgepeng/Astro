namespace Astro.Winform.Forms
{
    partial class LoginDetailForm
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
            comboBox1 = new ComboBox();
            label1 = new Label();
            checkBox1 = new CheckBox();
            checkBox2 = new CheckBox();
            label2 = new Label();
            label3 = new Label();
            passwordTextBox = new TextBox();
            passwordLabel = new Label();
            confirmPassweordLabel = new Label();
            confirmPasswordTextBox = new TextBox();
            button1 = new Button();
            resetPasswordLink = new LinkLabel();
            checkedListBox1 = new CheckedListBox();
            label4 = new Label();
            SuspendLayout();
            // 
            // comboBox1
            // 
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox1.FormattingEnabled = true;
            comboBox1.Location = new Point(25, 95);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(331, 25);
            comboBox1.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(25, 75);
            label1.Name = "label1";
            label1.Size = new Size(78, 17);
            label1.TabIndex = 1;
            label1.Text = "Login untuk:";
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            checkBox1.Location = new Point(25, 269);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(228, 21);
            checkBox1.TabIndex = 2;
            checkBox1.Text = "Gunakan kadaluwarsa kata sandi";
            checkBox1.UseVisualStyleBackColor = true;
            // 
            // checkBox2
            // 
            checkBox2.AutoSize = true;
            checkBox2.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            checkBox2.Location = new Point(25, 357);
            checkBox2.Name = "checkBox2";
            checkBox2.Size = new Size(129, 21);
            checkBox2.TabIndex = 3;
            checkBox2.Text = "Lockout Enabled";
            checkBox2.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            label2.Location = new Point(25, 293);
            label2.Name = "label2";
            label2.Size = new Size(331, 61);
            label2.TabIndex = 4;
            label2.Text = "Jika dicentang: kata sandi pengguna memiliki masa berlaku dan harus diganti setelah melewati batas waktu yang ditentukan.";
            // 
            // label3
            // 
            label3.Location = new Point(25, 381);
            label3.Name = "label3";
            label3.Size = new Size(331, 55);
            label3.TabIndex = 5;
            label3.Text = "Jika dicentang: akun akan otomatis dikunci setelah beberapa kali percobaan login gagal, sesuai pengaturan sistem.";
            // 
            // passwordTextBox
            // 
            passwordTextBox.Location = new Point(25, 483);
            passwordTextBox.Name = "passwordTextBox";
            passwordTextBox.PasswordChar = '*';
            passwordTextBox.Size = new Size(331, 25);
            passwordTextBox.TabIndex = 6;
            passwordTextBox.UseSystemPasswordChar = true;
            // 
            // passwordLabel
            // 
            passwordLabel.AutoSize = true;
            passwordLabel.Location = new Point(25, 463);
            passwordLabel.Name = "passwordLabel";
            passwordLabel.Size = new Size(69, 17);
            passwordLabel.TabIndex = 7;
            passwordLabel.Text = "Kata sandi";
            // 
            // confirmPassweordLabel
            // 
            confirmPassweordLabel.AutoSize = true;
            confirmPassweordLabel.Location = new Point(25, 516);
            confirmPassweordLabel.Name = "confirmPassweordLabel";
            confirmPassweordLabel.Size = new Size(133, 17);
            confirmPassweordLabel.TabIndex = 9;
            confirmPassweordLabel.Text = "Konfirmasi kata sandi";
            // 
            // confirmPasswordTextBox
            // 
            confirmPasswordTextBox.Location = new Point(25, 536);
            confirmPasswordTextBox.Name = "confirmPasswordTextBox";
            confirmPasswordTextBox.PasswordChar = '*';
            confirmPasswordTextBox.Size = new Size(331, 25);
            confirmPasswordTextBox.TabIndex = 8;
            confirmPasswordTextBox.UseSystemPasswordChar = true;
            // 
            // button1
            // 
            button1.Location = new Point(25, 604);
            button1.Name = "button1";
            button1.Size = new Size(331, 30);
            button1.TabIndex = 10;
            button1.Text = "Simpan";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // resetPasswordLink
            // 
            resetPasswordLink.AutoSize = true;
            resetPasswordLink.Location = new Point(25, 446);
            resetPasswordLink.Name = "resetPasswordLink";
            resetPasswordLink.Size = new Size(100, 17);
            resetPasswordLink.TabIndex = 11;
            resetPasswordLink.TabStop = true;
            resetPasswordLink.Text = "Reset Password";
            resetPasswordLink.Visible = false;
            // 
            // checkedListBox1
            // 
            checkedListBox1.FormattingEnabled = true;
            checkedListBox1.Items.AddRange(new object[] { "MAESTRO BENDO", "MAESTRO BESOLE", "MAESTRO ALBAIK", "MAESTRO FOTOKOPI" });
            checkedListBox1.Location = new Point(25, 153);
            checkedListBox1.Name = "checkedListBox1";
            checkedListBox1.Size = new Size(331, 104);
            checkedListBox1.TabIndex = 12;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(25, 133);
            label4.Name = "label4";
            label4.Size = new Size(160, 17);
            label4.TabIndex = 13;
            label4.Text = "Cabang yang bisa diakses";
            // 
            // LoginDetailForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(label4);
            Controls.Add(checkedListBox1);
            Controls.Add(resetPasswordLink);
            Controls.Add(button1);
            Controls.Add(confirmPassweordLabel);
            Controls.Add(confirmPasswordTextBox);
            Controls.Add(passwordLabel);
            Controls.Add(passwordTextBox);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(checkBox2);
            Controls.Add(checkBox1);
            Controls.Add(label1);
            Controls.Add(comboBox1);
            Name = "LoginDetailForm";
            Size = new Size(380, 650);
            Load += LoginDetailForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ComboBox comboBox1;
        private Label label1;
        private CheckBox checkBox1;
        private CheckBox checkBox2;
        private Label label2;
        private Label label3;
        private TextBox passwordTextBox;
        private Label passwordLabel;
        private Label confirmPassweordLabel;
        private TextBox confirmPasswordTextBox;
        private Button button1;
        private LinkLabel resetPasswordLink;
        private CheckedListBox checkedListBox1;
        private Label label4;
    }
}
