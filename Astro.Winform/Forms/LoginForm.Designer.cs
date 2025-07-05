namespace Astro.Winform.Forms
{
    partial class LoginForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginForm));
            label1 = new Label();
            usernameTextBox = new TextBox();
            passwordTextBox = new TextBox();
            label2 = new Label();
            button1 = new Button();
            button2 = new Button();
            pictureBox1 = new PictureBox();
            label3 = new Label();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(35, 199);
            label1.Name = "label1";
            label1.Size = new Size(67, 17);
            label1.TabIndex = 0;
            label1.Text = "Username";
            // 
            // usernameTextBox
            // 
            usernameTextBox.BackColor = Color.Lavender;
            usernameTextBox.Location = new Point(35, 219);
            usernameTextBox.Name = "usernameTextBox";
            usernameTextBox.Size = new Size(280, 25);
            usernameTextBox.TabIndex = 1;
            // 
            // passwordTextBox
            // 
            passwordTextBox.BackColor = Color.Lavender;
            passwordTextBox.Location = new Point(35, 272);
            passwordTextBox.Name = "passwordTextBox";
            passwordTextBox.PasswordChar = '*';
            passwordTextBox.Size = new Size(280, 25);
            passwordTextBox.TabIndex = 3;
            passwordTextBox.UseSystemPasswordChar = true;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(35, 252);
            label2.Name = "label2";
            label2.Size = new Size(70, 17);
            label2.TabIndex = 2;
            label2.Text = "Passsword";
            // 
            // button1
            // 
            button1.BackColor = SystemColors.HotTrack;
            button1.FlatAppearance.BorderSize = 0;
            button1.FlatStyle = FlatStyle.Flat;
            button1.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            button1.ForeColor = Color.White;
            button1.Location = new Point(178, 325);
            button1.Name = "button1";
            button1.Size = new Size(137, 32);
            button1.TabIndex = 4;
            button1.Text = "🔑 Login";
            button1.UseVisualStyleBackColor = false;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.BackColor = Color.Red;
            button2.FlatAppearance.BorderSize = 0;
            button2.FlatStyle = FlatStyle.Flat;
            button2.Font = new Font("Segoe UI Emoji", 9.75F);
            button2.ForeColor = Color.White;
            button2.Location = new Point(35, 325);
            button2.Name = "button2";
            button2.Size = new Size(137, 32);
            button2.TabIndex = 5;
            button2.Text = "❌ Batal";
            button2.UseVisualStyleBackColor = false;
            button2.Click += button2_Click;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(35, 96);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(280, 48);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 6;
            pictureBox1.TabStop = false;
            // 
            // label3
            // 
            label3.ForeColor = SystemColors.ControlDarkDark;
            label3.Location = new Point(29, 405);
            label3.Name = "label3";
            label3.Size = new Size(282, 74);
            label3.TabIndex = 7;
            label3.Text = "Copyright © 2025 Havas Media";
            label3.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // LoginForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(348, 498);
            ControlBox = false;
            Controls.Add(label3);
            Controls.Add(pictureBox1);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(passwordTextBox);
            Controls.Add(label2);
            Controls.Add(usernameTextBox);
            Controls.Add(label1);
            Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "LoginForm";
            StartPosition = FormStartPosition.CenterScreen;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private TextBox usernameTextBox;
        private TextBox passwordTextBox;
        private Label label2;
        private Button button1;
        private Button button2;
        private PictureBox pictureBox1;
        private Label label3;
    }
}