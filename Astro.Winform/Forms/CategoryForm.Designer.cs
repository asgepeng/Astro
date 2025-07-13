namespace Astro.Winform.Forms
{
    partial class CategoryForm
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
            textBox1 = new TextBox();
            button1 = new Button();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 27);
            label1.Name = "label1";
            label1.Size = new Size(100, 17);
            label1.TabIndex = 0;
            label1.Text = "Category Name";
            // 
            // textBox1
            // 
            textBox1.Location = new Point(12, 47);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(314, 25);
            textBox1.TabIndex = 1;
            // 
            // button1
            // 
            button1.Location = new Point(241, 102);
            button1.Name = "button1";
            button1.Size = new Size(85, 32);
            button1.TabIndex = 3;
            button1.Text = "💾 Save";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // CategoryForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(338, 152);
            Controls.Add(button1);
            Controls.Add(textBox1);
            Controls.Add(label1);
            Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "CategoryForm";
            ShowIcon = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Product Category Detail";
            Load += HandleFormLoad;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private TextBox textBox1;
        private Button button1;
    }
}