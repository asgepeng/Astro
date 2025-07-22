namespace PointOfSale
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            toolTipGlobal = new ToolTip(components);
            ms = new MenuStrip();
            statusStrip1 = new StatusStrip();
            navigator = new BindingNavigator(components);
            countLabel = new ToolStripLabel();
            refreshButton = new ToolStripButton();
            addNewButton = new ToolStripButton();
            deleteButton = new ToolStripButton();
            spr1 = new ToolStripSeparator();
            firstRowButton = new ToolStripButton();
            previousRow = new ToolStripButton();
            posTextBox = new ToolStripTextBox();
            spr2 = new ToolStripSeparator();
            nextRowButton = new ToolStripButton();
            lastRecordButton = new ToolStripButton();
            spr3 = new ToolStripSeparator();
            categortButton = new ToolStripButton();
            unitButton = new ToolStripButton();
            searchTextBox = new ToolStripTextBox();
            searchLabel = new ToolStripLabel();
            apButton = new ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)navigator).BeginInit();
            navigator.SuspendLayout();
            SuspendLayout();
            // 
            // toolTipGlobal
            // 
            toolTipGlobal.ToolTipIcon = ToolTipIcon.Info;
            // 
            // ms
            // 
            ms.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            ms.Location = new Point(0, 0);
            ms.Name = "ms";
            ms.Padding = new Padding(6, 4, 0, 4);
            ms.Size = new Size(1185, 24);
            ms.TabIndex = 0;
            ms.Text = "menuStrip1";
            // 
            // statusStrip1
            // 
            statusStrip1.Location = new Point(0, 428);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(1185, 22);
            statusStrip1.TabIndex = 2;
            statusStrip1.Text = "statusStrip1";
            // 
            // navigator
            // 
            navigator.AddNewItem = null;
            navigator.BackColor = SystemColors.ControlLightLight;
            navigator.CountItem = countLabel;
            navigator.DeleteItem = null;
            navigator.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            navigator.GripMargin = new Padding(0);
            navigator.GripStyle = ToolStripGripStyle.Hidden;
            navigator.ImageScalingSize = new Size(24, 24);
            navigator.Items.AddRange(new ToolStripItem[] { refreshButton, addNewButton, deleteButton, spr1, firstRowButton, previousRow, posTextBox, countLabel, spr2, nextRowButton, lastRecordButton, spr3, categortButton, unitButton, apButton, searchTextBox, searchLabel });
            navigator.Location = new Point(0, 24);
            navigator.MoveFirstItem = firstRowButton;
            navigator.MoveLastItem = lastRecordButton;
            navigator.MoveNextItem = nextRowButton;
            navigator.MovePreviousItem = previousRow;
            navigator.Name = "navigator";
            navigator.Padding = new Padding(6, 4, 0, 4);
            navigator.PositionItem = posTextBox;
            navigator.Size = new Size(1185, 39);
            navigator.TabIndex = 4;
            navigator.Text = "menuStrip1";
            // 
            // countLabel
            // 
            countLabel.Name = "countLabel";
            countLabel.Size = new Size(39, 28);
            countLabel.Text = "of {0}";
            // 
            // refreshButton
            // 
            refreshButton.Image = Astro.Winform.Properties.Resources.reload;
            refreshButton.ImageTransparentColor = Color.Magenta;
            refreshButton.Name = "refreshButton";
            refreshButton.Size = new Size(98, 28);
            refreshButton.Text = "Refresh All";
            refreshButton.Click += HandleMdiChildActivate;
            // 
            // addNewButton
            // 
            addNewButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            addNewButton.Image = Astro.Winform.Properties.Resources.plusblack;
            addNewButton.ImageTransparentColor = Color.Magenta;
            addNewButton.Name = "addNewButton";
            addNewButton.Size = new Size(28, 28);
            addNewButton.Text = "Add New";
            addNewButton.Click += HandleNewButtonClicked;
            // 
            // deleteButton
            // 
            deleteButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            deleteButton.Image = Astro.Winform.Properties.Resources.deleteblack;
            deleteButton.ImageTransparentColor = Color.Magenta;
            deleteButton.Name = "deleteButton";
            deleteButton.Size = new Size(28, 28);
            deleteButton.Text = "Delete Record";
            deleteButton.Click += HandleDeleteButtonClicked;
            // 
            // spr1
            // 
            spr1.Name = "spr1";
            spr1.Size = new Size(6, 31);
            // 
            // firstRowButton
            // 
            firstRowButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            firstRowButton.Image = Astro.Winform.Properties.Resources.first_page;
            firstRowButton.ImageTransparentColor = Color.Magenta;
            firstRowButton.Name = "firstRowButton";
            firstRowButton.Size = new Size(28, 28);
            firstRowButton.Text = "First Record";
            // 
            // previousRow
            // 
            previousRow.DisplayStyle = ToolStripItemDisplayStyle.Image;
            previousRow.Image = Astro.Winform.Properties.Resources.back__2_;
            previousRow.ImageTransparentColor = Color.Magenta;
            previousRow.Name = "previousRow";
            previousRow.Size = new Size(28, 28);
            previousRow.Text = "Previous Record";
            // 
            // posTextBox
            // 
            posTextBox.Name = "posTextBox";
            posTextBox.Size = new Size(80, 31);
            posTextBox.Text = "0";
            // 
            // spr2
            // 
            spr2.Name = "spr2";
            spr2.Size = new Size(6, 31);
            // 
            // nextRowButton
            // 
            nextRowButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            nextRowButton.Image = Astro.Winform.Properties.Resources.right_arrow;
            nextRowButton.ImageTransparentColor = Color.Magenta;
            nextRowButton.Name = "nextRowButton";
            nextRowButton.Size = new Size(28, 28);
            nextRowButton.Text = "Next Record";
            // 
            // lastRecordButton
            // 
            lastRecordButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            lastRecordButton.Image = Astro.Winform.Properties.Resources.last_page;
            lastRecordButton.ImageTransparentColor = Color.Magenta;
            lastRecordButton.Name = "lastRecordButton";
            lastRecordButton.Size = new Size(28, 28);
            lastRecordButton.Text = "Last Record";
            // 
            // spr3
            // 
            spr3.Name = "spr3";
            spr3.Size = new Size(6, 31);
            // 
            // categortButton
            // 
            categortButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            categortButton.Image = Astro.Winform.Properties.Resources.data;
            categortButton.ImageTransparentColor = Color.Magenta;
            categortButton.Name = "categortButton";
            categortButton.Size = new Size(28, 28);
            categortButton.Text = "Category";
            categortButton.Click += HandleCategoryButtonClicked;
            // 
            // unitButton
            // 
            unitButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            unitButton.Image = Astro.Winform.Properties.Resources.db;
            unitButton.ImageTransparentColor = Color.Magenta;
            unitButton.Name = "unitButton";
            unitButton.Size = new Size(28, 28);
            unitButton.Text = "Units";
            unitButton.Click += unitButton_Click;
            // 
            // searchTextBox
            // 
            searchTextBox.Alignment = ToolStripItemAlignment.Right;
            searchTextBox.BackColor = Color.FromArgb(224, 224, 224);
            searchTextBox.BorderStyle = BorderStyle.FixedSingle;
            searchTextBox.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            searchTextBox.MaxLength = 50;
            searchTextBox.Name = "searchTextBox";
            searchTextBox.Size = new Size(100, 31);
            searchTextBox.KeyDown += ApplyFilter;
            // 
            // searchLabel
            // 
            searchLabel.Alignment = ToolStripItemAlignment.Right;
            searchLabel.Name = "searchLabel";
            searchLabel.Size = new Size(69, 28);
            searchLabel.Text = "🔍 Search";
            // 
            // apButton
            // 
            apButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            apButton.Image = Astro.Winform.Properties.Resources.cashflow;
            apButton.ImageTransparentColor = Color.Magenta;
            apButton.Name = "apButton";
            apButton.Size = new Size(28, 28);
            apButton.Text = "Account Providers";
            apButton.Click += OpenAccountProviderForm;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1185, 450);
            Controls.Add(navigator);
            Controls.Add(statusStrip1);
            Controls.Add(ms);
            IsMdiContainer = true;
            MainMenuStrip = ms;
            Name = "MainForm";
            Text = "Form1";
            FormClosing += MainForm_FormClosing;
            Load += MainForm_Load;
            MdiChildActivate += HandleMdiChildActivate;
            ((System.ComponentModel.ISupportInitialize)navigator).EndInit();
            navigator.ResumeLayout(false);
            navigator.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ToolTip toolTipGlobal;
        private MenuStrip ms;
        private StatusStrip statusStrip1;
        private BindingNavigator navigator;
        private ToolStripButton refreshButton;
        private ToolStripButton addNewButton;
        private ToolStripButton firstRowButton;
        private ToolStripButton previousRow;
        private ToolStripTextBox posTextBox;
        private ToolStripLabel countLabel;
        private ToolStripButton nextRowButton;
        private ToolStripButton lastRecordButton;
        private ToolStripButton deleteButton;
        private ToolStripSeparator spr1;
        private ToolStripSeparator spr2;
        private ToolStripSeparator spr3;
        private ToolStripButton categortButton;
        private ToolStripButton unitButton;
        private ToolStripTextBox searchTextBox;
        private ToolStripLabel searchLabel;
        private ToolStripButton apButton;
    }
}
