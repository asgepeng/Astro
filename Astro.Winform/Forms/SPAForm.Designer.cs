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
            userPanel1 = new Controls.UserPanel();
            SuspendLayout();
            // 
            // sideBarPanel
            // 
            sideBarPanel.BackColor = Color.Navy;
            sideBarPanel.BoldFont = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            sideBarPanel.Dock = DockStyle.Left;
            sideBarPanel.Font = new Font("Segoe UI", 9.75F);
            sideBarPanel.ForeColor = Color.White;
            sideBarPanel.GroupHeight = 48;
            sideBarPanel.ItemHeight = 32;
            sideBarPanel.Location = new Point(3, 40);
            sideBarPanel.Name = "sideBarPanel";
            sideBarPanel.ScrollBarColor = Color.FromArgb(192, 192, 255);
            sideBarPanel.SelectedItemColor = Color.DarkGray;
            sideBarPanel.Size = new Size(220, 557);
            sideBarPanel.TabIndex = 0;
            sideBarPanel.Text = "sideBarPanel1";
            // 
            // mainPanel
            // 
            mainPanel.Dock = DockStyle.Fill;
            mainPanel.Location = new Point(223, 40);
            mainPanel.Name = "mainPanel";
            mainPanel.Size = new Size(940, 557);
            mainPanel.TabIndex = 1;
            // 
            // userPanel1
            // 
            userPanel1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            userPanel1.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            userPanel1.ForeColor = Color.Navy;
            userPanel1.Location = new Point(634, 5);
            userPanel1.Name = "userPanel1";
            userPanel1.Size = new Size(388, 30);
            userPanel1.TabIndex = 2;
            userPanel1.Text = "userPanel1";
            // 
            // SPAForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(250, 250, 250);
            ClientSize = new Size(1166, 600);
            Controls.Add(userPanel1);
            Controls.Add(mainPanel);
            Controls.Add(sideBarPanel);
            Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            FormBorderStyle = FormBorderStyle.None;
            MinimumSize = new Size(1000, 600);
            Name = "SPAForm";
            Padding = new Padding(3, 40, 3, 3);
            Text = "SPAForm";
            Load += SPAForm_Load;
            ResumeLayout(false);
        }

        #endregion

        private Controls.SideBarPanel sideBarPanel;
        private Panel mainPanel;
        private Controls.UserPanel userPanel1;
    }
}