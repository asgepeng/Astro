using Astro.Winform.Controls;

namespace Astro.Winform.Forms
{
    partial class ContactForm
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ContactForm));
            tabControl1 = new FlatTabControl();
            tabPage1 = new TabPage();
            deleteButton = new Button();
            listView1 = new ListView();
            columnHeader1 = new ColumnHeader();
            columnHeader2 = new ColumnHeader();
            columnHeader3 = new ColumnHeader();
            imageList1 = new ImageList(components);
            label6 = new Label();
            textBox3 = new TextBox();
            label5 = new Label();
            textBox2 = new TextBox();
            editButton = new Button();
            addButton = new Button();
            label2 = new Label();
            label1 = new Label();
            textBox1 = new TextBox();
            tabPage2 = new TabPage();
            tabPage3 = new TabPage();
            button1 = new Button();
            contextMenuStrip1 = new ContextMenuStrip(components);
            addressToolStripMenuItem = new ToolStripMenuItem();
            phoneToolStripMenuItem = new ToolStripMenuItem();
            emailToolStripMenuItem = new ToolStripMenuItem();
            label3 = new Label();
            panel1 = new Panel();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            contextMenuStrip1.SuspendLayout();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // tabControl1
            // 
            tabControl1.ActiveColor = Color.DeepSkyBlue;
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Controls.Add(tabPage3);
            tabControl1.Dock = DockStyle.Fill;
            tabControl1.DrawMode = TabDrawMode.OwnerDrawFixed;
            tabControl1.InactiveColor = Color.LightGray;
            tabControl1.ItemSize = new Size(120, 40);
            tabControl1.Location = new Point(0, 40);
            tabControl1.Name = "tabControl1";
            tabControl1.Padding = new Point(6, 6);
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(400, 607);
            tabControl1.TabIndex = 0;
            tabControl1.TextColor = Color.Black;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(deleteButton);
            tabPage1.Controls.Add(listView1);
            tabPage1.Controls.Add(label6);
            tabPage1.Controls.Add(textBox3);
            tabPage1.Controls.Add(label5);
            tabPage1.Controls.Add(textBox2);
            tabPage1.Controls.Add(editButton);
            tabPage1.Controls.Add(addButton);
            tabPage1.Controls.Add(label2);
            tabPage1.Controls.Add(label1);
            tabPage1.Controls.Add(textBox1);
            tabPage1.Location = new Point(4, 44);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(392, 559);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "ℹ️ General";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // deleteButton
            // 
            deleteButton.BackColor = Color.FromArgb(230, 230, 230);
            deleteButton.Enabled = false;
            deleteButton.FlatAppearance.BorderColor = Color.FromArgb(192, 192, 255);
            deleteButton.FlatStyle = FlatStyle.Flat;
            deleteButton.Font = new Font("Segoe MDL2 Assets", 9.75F);
            deleteButton.ForeColor = SystemColors.ControlText;
            deleteButton.Location = new Point(81, 91);
            deleteButton.Name = "deleteButton";
            deleteButton.Size = new Size(30, 30);
            deleteButton.TabIndex = 18;
            deleteButton.Text = "❌";
            deleteButton.UseVisualStyleBackColor = false;
            deleteButton.Click += deleteButton_Click;
            // 
            // listView1
            // 
            listView1.Alignment = ListViewAlignment.SnapToGrid;
            listView1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            listView1.Columns.AddRange(new ColumnHeader[] { columnHeader1, columnHeader2, columnHeader3 });
            listView1.GridLines = true;
            listView1.LabelWrap = false;
            listView1.LargeImageList = imageList1;
            listView1.Location = new Point(15, 127);
            listView1.Name = "listView1";
            listView1.Size = new Size(363, 378);
            listView1.TabIndex = 2;
            listView1.TileSize = new Size(350, 56);
            listView1.UseCompatibleStateImageBehavior = false;
            listView1.View = View.Tile;
            listView1.ItemActivate += listView1_ItemActivate;
            listView1.SelectedIndexChanged += listView1_SelectedIndexChanged;
            // 
            // imageList1
            // 
            imageList1.ColorDepth = ColorDepth.Depth32Bit;
            imageList1.ImageStream = (ImageListStreamer)resources.GetObject("imageList1.ImageStream");
            imageList1.TransparentColor = Color.Transparent;
            imageList1.Images.SetKeyName(0, "ihome.png");
            imageList1.Images.SetKeyName(1, "ioffice.png");
            imageList1.Images.SetKeyName(2, "iseluler.png");
            imageList1.Images.SetKeyName(3, "celluler.png");
            imageList1.Images.SetKeyName(4, "iwhatsapp.png");
            imageList1.Images.SetKeyName(5, "email.png");
            // 
            // label6
            // 
            label6.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label6.AutoSize = true;
            label6.Location = new Point(200, 508);
            label6.Name = "label6";
            label6.Size = new Size(55, 17);
            label6.TabIndex = 17;
            label6.Text = "AR Limit";
            // 
            // textBox3
            // 
            textBox3.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            textBox3.Location = new Point(200, 528);
            textBox3.Name = "textBox3";
            textBox3.Size = new Size(178, 25);
            textBox3.TabIndex = 16;
            // 
            // label5
            // 
            label5.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label5.AutoSize = true;
            label5.Location = new Point(12, 508);
            label5.Name = "label5";
            label5.Size = new Size(74, 17);
            label5.TabIndex = 15;
            label5.Text = "Credit Limit";
            // 
            // textBox2
            // 
            textBox2.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            textBox2.Location = new Point(12, 528);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(178, 25);
            textBox2.TabIndex = 14;
            // 
            // editButton
            // 
            editButton.BackColor = Color.FromArgb(230, 230, 230);
            editButton.Enabled = false;
            editButton.FlatAppearance.BorderColor = Color.FromArgb(192, 192, 255);
            editButton.FlatStyle = FlatStyle.Flat;
            editButton.Font = new Font("Segoe MDL2 Assets", 9.75F);
            editButton.ForeColor = SystemColors.ControlText;
            editButton.Location = new Point(48, 91);
            editButton.Name = "editButton";
            editButton.Size = new Size(30, 30);
            editButton.TabIndex = 9;
            editButton.Text = "✏️";
            editButton.UseVisualStyleBackColor = false;
            editButton.Click += listView1_ItemActivate;
            // 
            // addButton
            // 
            addButton.BackColor = Color.FromArgb(230, 230, 230);
            addButton.FlatAppearance.BorderColor = Color.FromArgb(192, 192, 255);
            addButton.FlatStyle = FlatStyle.Flat;
            addButton.Font = new Font("Segoe MDL2 Assets", 9.75F);
            addButton.ForeColor = SystemColors.ControlText;
            addButton.Location = new Point(15, 91);
            addButton.Name = "addButton";
            addButton.Size = new Size(30, 30);
            addButton.TabIndex = 8;
            addButton.Text = "➕";
            addButton.UseVisualStyleBackColor = false;
            addButton.Click += button2_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 71);
            label2.Name = "label2";
            label2.Size = new Size(52, 17);
            label2.TabIndex = 4;
            label2.Text = "Contact";
            // 
            // label1
            // 
            label1.Location = new Point(12, 10);
            label1.Name = "label1";
            label1.Size = new Size(108, 25);
            label1.TabIndex = 1;
            label1.Text = "Customer Name";
            label1.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(15, 38);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(363, 25);
            textBox1.TabIndex = 0;
            // 
            // tabPage2
            // 
            tabPage2.Location = new Point(4, 44);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(392, 559);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "🕒 Transaction Histories";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // tabPage3
            // 
            tabPage3.Location = new Point(4, 44);
            tabPage3.Name = "tabPage3";
            tabPage3.Padding = new Padding(3);
            tabPage3.Size = new Size(392, 559);
            tabPage3.TabIndex = 2;
            tabPage3.Text = "\U0001f9fe Account Payable";
            tabPage3.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            button1.BackColor = Color.Blue;
            button1.FlatAppearance.BorderColor = Color.FromArgb(192, 192, 255);
            button1.FlatStyle = FlatStyle.Flat;
            button1.ForeColor = Color.White;
            button1.Location = new Point(16, 17);
            button1.Name = "button1";
            button1.Size = new Size(366, 32);
            button1.TabIndex = 1;
            button1.Text = "Simpan";
            button1.UseVisualStyleBackColor = false;
            button1.Click += button1_Click;
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.Items.AddRange(new ToolStripItem[] { addressToolStripMenuItem, phoneToolStripMenuItem, emailToolStripMenuItem });
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new Size(149, 70);
            // 
            // addressToolStripMenuItem
            // 
            addressToolStripMenuItem.Name = "addressToolStripMenuItem";
            addressToolStripMenuItem.Size = new Size(148, 22);
            addressToolStripMenuItem.Text = "Address";
            addressToolStripMenuItem.Click += addressToolStripMenuItem_Click;
            // 
            // phoneToolStripMenuItem
            // 
            phoneToolStripMenuItem.Name = "phoneToolStripMenuItem";
            phoneToolStripMenuItem.Size = new Size(148, 22);
            phoneToolStripMenuItem.Text = "Phone";
            phoneToolStripMenuItem.Click += button7_Click;
            // 
            // emailToolStripMenuItem
            // 
            emailToolStripMenuItem.Name = "emailToolStripMenuItem";
            emailToolStripMenuItem.Size = new Size(148, 22);
            emailToolStripMenuItem.Text = "Email Address";
            emailToolStripMenuItem.Click += button5_Click;
            // 
            // label3
            // 
            label3.Dock = DockStyle.Top;
            label3.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label3.Location = new Point(0, 0);
            label3.Name = "label3";
            label3.Padding = new Padding(5, 0, 0, 0);
            label3.Size = new Size(400, 40);
            label3.TabIndex = 2;
            label3.Text = "Pelanggan";
            label3.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // panel1
            // 
            panel1.Controls.Add(button1);
            panel1.Dock = DockStyle.Bottom;
            panel1.Location = new Point(0, 647);
            panel1.Name = "panel1";
            panel1.Size = new Size(400, 64);
            panel1.TabIndex = 19;
            // 
            // ContactForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(250, 250, 250);
            BorderStyle = BorderStyle.FixedSingle;
            Controls.Add(tabControl1);
            Controls.Add(label3);
            Controls.Add(panel1);
            Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Name = "ContactForm";
            Size = new Size(400, 711);
            Load += ContactForm_Load;
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage1.PerformLayout();
            contextMenuStrip1.ResumeLayout(false);
            panel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private TabPage tabPage1;
        private TabPage tabPage2;
        private ListView listView1;
        private Label label1;
        private TextBox textBox1;
        private TabPage tabPage3;
        private Button button1;
        private Label label2;
        private Button editButton;
        private Button addButton;
        private Label label6;
        private TextBox textBox3;
        private Label label5;
        private TextBox textBox2;
        private ImageList imageList1;
        private ColumnHeader columnHeader1;
        private ColumnHeader columnHeader2;
        private ColumnHeader columnHeader3;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem addressToolStripMenuItem;
        private ToolStripMenuItem phoneToolStripMenuItem;
        private ToolStripMenuItem emailToolStripMenuItem;
        private Button deleteButton;
        private FlatTabControl tabControl1;
        private Label label3;
        private Panel panel1;
    }
}