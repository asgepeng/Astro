namespace Astro.Winform.Forms
{
    partial class PhoneForm
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
            zipCodeTextBox = new TextBox();
            label6 = new Label();
            typeComboBox = new ComboBox();
            label1 = new Label();
            textBox1 = new TextBox();
            label2 = new Label();
            checkBox1 = new CheckBox();
            button1 = new Button();
            SuspendLayout();
            // 
            // zipCodeTextBox
            // 
            zipCodeTextBox.Location = new Point(12, 77);
            zipCodeTextBox.Name = "zipCodeTextBox";
            zipCodeTextBox.Size = new Size(178, 25);
            zipCodeTextBox.TabIndex = 16;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(12, 57);
            label6.Name = "label6";
            label6.Size = new Size(96, 17);
            label6.TabIndex = 15;
            label6.Text = "Phone Number";
            // 
            // typeComboBox
            // 
            typeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            typeComboBox.FormattingEnabled = true;
            typeComboBox.Items.AddRange(new object[] { "Home", "Celluler", "Whatsapp" });
            typeComboBox.Location = new Point(12, 29);
            typeComboBox.Name = "typeComboBox";
            typeComboBox.Size = new Size(310, 25);
            typeComboBox.TabIndex = 14;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(75, 17);
            label1.TabIndex = 13;
            label1.Text = "Phone Type";
            // 
            // textBox1
            // 
            textBox1.Location = new Point(196, 77);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(126, 25);
            textBox1.TabIndex = 18;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(196, 57);
            label2.Name = "label2";
            label2.Size = new Size(68, 17);
            label2.TabIndex = 17;
            label2.Text = "Phone Ext.";
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Location = new Point(12, 108);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(124, 21);
            checkBox1.TabIndex = 19;
            checkBox1.Text = "Is Primary Phone";
            checkBox1.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            button1.Location = new Point(12, 153);
            button1.Name = "button1";
            button1.Size = new Size(87, 30);
            button1.TabIndex = 20;
            button1.Text = "💾 Save";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // PhoneForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(334, 195);
            Controls.Add(button1);
            Controls.Add(checkBox1);
            Controls.Add(textBox1);
            Controls.Add(label2);
            Controls.Add(zipCodeTextBox);
            Controls.Add(label6);
            Controls.Add(typeComboBox);
            Controls.Add(label1);
            Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "PhoneForm";
            ShowIcon = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Add Phone";
            Load += PhoneForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox zipCodeTextBox;
        private Label label6;
        private ComboBox typeComboBox;
        private Label label1;
        private TextBox textBox1;
        private Label label2;
        private CheckBox checkBox1;
        private Button button1;
    }
}