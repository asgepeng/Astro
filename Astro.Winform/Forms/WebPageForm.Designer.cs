namespace Astro.Winform.Forms
{
    partial class WebPageForm
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
            textBox2 = new TextBox();
            label2 = new Label();
            textBox3 = new TextBox();
            label3 = new Label();
            tabControl1 = new TabControl();
            tabPage1 = new TabPage();
            textBox4 = new TextBox();
            label4 = new Label();
            tabPage2 = new TabPage();
            webView21 = new Microsoft.Web.WebView2.WinForms.WebView2();
            tabPage3 = new TabPage();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)webView21).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(11, 23);
            label1.Name = "label1";
            label1.Size = new Size(32, 17);
            label1.TabIndex = 0;
            label1.Text = "Title";
            // 
            // textBox1
            // 
            textBox1.Location = new Point(120, 20);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(400, 25);
            textBox1.TabIndex = 1;
            // 
            // textBox2
            // 
            textBox2.Location = new Point(120, 51);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(400, 25);
            textBox2.TabIndex = 3;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(11, 54);
            label2.Name = "label2";
            label2.Size = new Size(99, 17);
            label2.TabIndex = 2;
            label2.Text = "Navigation Title";
            // 
            // textBox3
            // 
            textBox3.Location = new Point(120, 82);
            textBox3.Name = "textBox3";
            textBox3.Size = new Size(400, 25);
            textBox3.TabIndex = 5;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(11, 85);
            label3.Name = "label3";
            label3.Size = new Size(71, 17);
            label3.TabIndex = 4;
            label3.Text = "Image URL";
            // 
            // tabControl1
            // 
            tabControl1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Controls.Add(tabPage3);
            tabControl1.Location = new Point(12, 12);
            tabControl1.Name = "tabControl1";
            tabControl1.Padding = new Point(6, 6);
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(1108, 485);
            tabControl1.TabIndex = 6;
            tabControl1.SelectedIndexChanged += tabControl1_SelectedIndexChanged;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(textBox4);
            tabPage1.Controls.Add(label4);
            tabPage1.Controls.Add(textBox1);
            tabPage1.Controls.Add(textBox3);
            tabPage1.Controls.Add(label1);
            tabPage1.Controls.Add(label3);
            tabPage1.Controls.Add(label2);
            tabPage1.Controls.Add(textBox2);
            tabPage1.Location = new Point(4, 32);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(1100, 449);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "General";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // textBox4
            // 
            textBox4.Location = new Point(120, 113);
            textBox4.Name = "textBox4";
            textBox4.Size = new Size(400, 25);
            textBox4.TabIndex = 7;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(11, 116);
            label4.Name = "label4";
            label4.Size = new Size(30, 17);
            label4.TabIndex = 6;
            label4.Text = "Link";
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(webView21);
            tabPage2.Location = new Point(4, 30);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(1100, 451);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Content";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // webView21
            // 
            webView21.AllowExternalDrop = true;
            webView21.CreationProperties = null;
            webView21.DefaultBackgroundColor = Color.White;
            webView21.Dock = DockStyle.Fill;
            webView21.Location = new Point(3, 3);
            webView21.Name = "webView21";
            webView21.Size = new Size(1094, 445);
            webView21.TabIndex = 7;
            webView21.ZoomFactor = 1D;
            // 
            // tabPage3
            // 
            tabPage3.Location = new Point(4, 30);
            tabPage3.Name = "tabPage3";
            tabPage3.Padding = new Padding(3);
            tabPage3.Size = new Size(1100, 451);
            tabPage3.TabIndex = 2;
            tabPage3.Text = "Meta Data";
            tabPage3.UseVisualStyleBackColor = true;
            // 
            // WebPageForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1132, 509);
            Controls.Add(tabControl1);
            Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Name = "WebPageForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "WebPageForm";
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage1.PerformLayout();
            tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)webView21).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Label label1;
        private TextBox textBox1;
        private TextBox textBox2;
        private Label label2;
        private TextBox textBox3;
        private Label label3;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TextBox textBox4;
        private Label label4;
        private TabPage tabPage2;
        private TabPage tabPage3;
        private Microsoft.Web.WebView2.WinForms.WebView2 webView21;
    }
}