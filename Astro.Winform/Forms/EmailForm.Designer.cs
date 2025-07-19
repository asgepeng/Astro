namespace Astro.Winform.Forms
{
    partial class EmailForm
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
            button1 = new Button();
            checkBox1 = new CheckBox();
            zipCodeTextBox = new TextBox();
            label6 = new Label();
            typeComboBox = new ComboBox();
            label1 = new Label();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new Point(12, 153);
            button1.Name = "button1";
            button1.Size = new Size(87, 30);
            button1.TabIndex = 28;
            button1.Text = "💾 Save";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Location = new Point(12, 108);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(104, 19);
            checkBox1.TabIndex = 27;
            checkBox1.Text = "Is Primary Mail";
            checkBox1.UseVisualStyleBackColor = true;
            // 
            // zipCodeTextBox
            // 
            zipCodeTextBox.Location = new Point(12, 77);
            zipCodeTextBox.Name = "zipCodeTextBox";
            zipCodeTextBox.Size = new Size(310, 23);
            zipCodeTextBox.TabIndex = 24;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(12, 57);
            label6.Name = "label6";
            label6.Size = new Size(81, 15);
            label6.TabIndex = 23;
            label6.Text = "Email Address";
            // 
            // typeComboBox
            // 
            typeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            typeComboBox.FormattingEnabled = true;
            typeComboBox.Items.AddRange(new object[] { "Office", "Personal" });
            typeComboBox.Location = new Point(12, 29);
            typeComboBox.Name = "typeComboBox";
            typeComboBox.Size = new Size(310, 23);
            typeComboBox.TabIndex = 22;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(63, 15);
            label1.TabIndex = 21;
            label1.Text = "Email Type";
            // 
            // EmailForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(334, 196);
            Controls.Add(button1);
            Controls.Add(checkBox1);
            Controls.Add(zipCodeTextBox);
            Controls.Add(label6);
            Controls.Add(typeComboBox);
            Controls.Add(label1);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "EmailForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Email";
            Load += EmailForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button button1;
        private CheckBox checkBox1;
        private TextBox zipCodeTextBox;
        private Label label6;
        private ComboBox typeComboBox;
        private Label label1;
    }
}