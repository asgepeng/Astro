namespace Astro.Winform.Forms
{
    partial class SPAForm
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
            sideBarPanel = new Controls.SideBarPanel();
            mainPanel = new Panel();
            SuspendLayout();
            // 
            // sideBarPanel
            // 
            sideBarPanel.BackColor = Color.Navy;
            sideBarPanel.BoldFont = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            sideBarPanel.Dock = DockStyle.Left;
            sideBarPanel.Font = new Font("Segoe UI", 9.75F);
            sideBarPanel.ForeColor = Color.AliceBlue;
            sideBarPanel.GroupHeight = 48;
            sideBarPanel.ItemHeight = 32;
            sideBarPanel.Location = new Point(0, 40);
            sideBarPanel.Name = "sideBarPanel";
            sideBarPanel.ScrollBarColor = Color.SlateGray;
            sideBarPanel.SelectedItemColor = Color.DarkGray;
            sideBarPanel.Size = new Size(220, 527);
            sideBarPanel.TabIndex = 0;
            sideBarPanel.Text = "sideBarPanel1";
            sideBarPanel.Click += sideBarPanel_Click;
            sideBarPanel.MouseClick += sideBarPanel_MouseClick;
            // 
            // mainPanel
            // 
            mainPanel.Dock = DockStyle.Fill;
            mainPanel.Location = new Point(220, 40);
            mainPanel.Name = "mainPanel";
            mainPanel.Size = new Size(580, 527);
            mainPanel.TabIndex = 1;
            // 
            // SPAForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(250, 250, 250);
            ClientSize = new Size(800, 567);
            Controls.Add(mainPanel);
            Controls.Add(sideBarPanel);
            Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            FormBorderStyle = FormBorderStyle.None;
            Name = "SPAForm";
            Padding = new Padding(0, 40, 0, 0);
            Text = "SPAForm";
            WindowState = FormWindowState.Maximized;
            Load += SPAForm_Load;
            ResumeLayout(false);
        }

        #endregion

        private Controls.SideBarPanel sideBarPanel;
        private Panel mainPanel;
    }
}