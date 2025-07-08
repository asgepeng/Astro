namespace Astro.Winform.Forms
{
    partial class ChangePasswordForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChangePasswordForm));
            passwordTextBox = new TextBox();
            label2 = new Label();
            newPasswordTextBox = new TextBox();
            label1 = new Label();
            confirmPasswordTextBox = new TextBox();
            label3 = new Label();
            button2 = new Button();
            button1 = new Button();
            label4 = new Label();
            pictureBox1 = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // passwordTextBox
            // 
            passwordTextBox.BackColor = Color.Lavender;
            passwordTextBox.Location = new Point(27, 195);
            passwordTextBox.Name = "passwordTextBox";
            passwordTextBox.PasswordChar = '*';
            passwordTextBox.Size = new Size(280, 25);
            passwordTextBox.TabIndex = 0;
            passwordTextBox.UseSystemPasswordChar = true;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(27, 175);
            label2.Name = "label2";
            label2.Size = new Size(120, 17);
            label2.TabIndex = 4;
            label2.Text = "Current Password *";
            // 
            // newPasswordTextBox
            // 
            newPasswordTextBox.BackColor = Color.Lavender;
            newPasswordTextBox.Location = new Point(27, 243);
            newPasswordTextBox.Name = "newPasswordTextBox";
            newPasswordTextBox.PasswordChar = '*';
            newPasswordTextBox.Size = new Size(280, 25);
            newPasswordTextBox.TabIndex = 1;
            newPasswordTextBox.UseSystemPasswordChar = true;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(27, 223);
            label1.Name = "label1";
            label1.Size = new Size(109, 17);
            label1.TabIndex = 6;
            label1.Text = "New Passsword *";
            // 
            // confirmPasswordTextBox
            // 
            confirmPasswordTextBox.BackColor = Color.Lavender;
            confirmPasswordTextBox.Location = new Point(27, 291);
            confirmPasswordTextBox.Name = "confirmPasswordTextBox";
            confirmPasswordTextBox.PasswordChar = '*';
            confirmPasswordTextBox.Size = new Size(280, 25);
            confirmPasswordTextBox.TabIndex = 2;
            confirmPasswordTextBox.UseSystemPasswordChar = true;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(27, 271);
            label3.Name = "label3";
            label3.Size = new Size(153, 17);
            label3.TabIndex = 8;
            label3.Text = "Confirm New Password *";
            // 
            // button2
            // 
            button2.BackColor = Color.Red;
            button2.FlatAppearance.BorderSize = 0;
            button2.FlatStyle = FlatStyle.Flat;
            button2.Font = new Font("Segoe UI Emoji", 9.75F);
            button2.ForeColor = Color.White;
            button2.Location = new Point(27, 345);
            button2.Name = "button2";
            button2.Size = new Size(137, 32);
            button2.TabIndex = 4;
            button2.Text = "❌ Batal";
            button2.UseVisualStyleBackColor = false;
            button2.Click += CloseForm;
            // 
            // button1
            // 
            button1.BackColor = SystemColors.HotTrack;
            button1.FlatAppearance.BorderSize = 0;
            button1.FlatStyle = FlatStyle.Flat;
            button1.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            button1.ForeColor = Color.White;
            button1.Location = new Point(170, 345);
            button1.Name = "button1";
            button1.Size = new Size(137, 32);
            button1.TabIndex = 3;
            button1.Text = "🔑 Update";
            button1.UseVisualStyleBackColor = false;
            button1.Click += button1_Click;
            // 
            // label4
            // 
            label4.ForeColor = SystemColors.ControlDarkDark;
            label4.Location = new Point(18, 408);
            label4.Name = "label4";
            label4.Size = new Size(282, 44);
            label4.TabIndex = 12;
            label4.Text = "Copyright © 2025 Havas Media";
            label4.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(27, 39);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(280, 100);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 13;
            pictureBox1.TabStop = false;
            // 
            // ChangePasswordForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(334, 461);
            Controls.Add(pictureBox1);
            Controls.Add(label4);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(confirmPasswordTextBox);
            Controls.Add(label3);
            Controls.Add(newPasswordTextBox);
            Controls.Add(label1);
            Controls.Add(passwordTextBox);
            Controls.Add(label2);
            Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ChangePasswordForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Change Password";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox passwordTextBox;
        private Label label2;
        private TextBox newPasswordTextBox;
        private Label label1;
        private TextBox confirmPasswordTextBox;
        private Label label3;
        private Button button2;
        private Button button1;
        private Label label4;
        private PictureBox pictureBox1;
    }
}