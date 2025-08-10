namespace Astro.Winform.Forms
{
    partial class ProductForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProductForm));
            tabPage1 = new TabPage();
            il = new ImageList(components);
            label9 = new Label();
            stockTextBox = new TextBox();
            button4 = new Button();
            button3 = new Button();
            button1 = new Button();
            productImage = new PictureBox();
            isactiveCheckBox = new CheckBox();
            label11 = new Label();
            maxstockTextBox = new TextBox();
            label10 = new Label();
            minstockTextBox = new TextBox();
            label7 = new Label();
            unitComboBox = new ComboBox();
            button6 = new Button();
            loginButton = new Button();
            label6 = new Label();
            categoryComboBox = new ComboBox();
            basicpriceTextBox = new TextBox();
            label3 = new Label();
            priceTextBox = new TextBox();
            label2 = new Label();
            descriptionTextBox = new TextBox();
            label1 = new Label();
            nameTextBox = new TextBox();
            skuTextBox = new TextBox();
            skuLabel = new Label();
            button5 = new Button();
            commanTabPage = new TabPage();
            button10 = new Button();
            button9 = new Button();
            button8 = new Button();
            button7 = new Button();
            button2 = new Button();
            label4 = new Label();
            tabControl = new TabControl();
            tabPage2 = new TabPage();
            ((System.ComponentModel.ISupportInitialize)productImage).BeginInit();
            commanTabPage.SuspendLayout();
            tabControl.SuspendLayout();
            SuspendLayout();
            // 
            // tabPage1
            // 
            tabPage1.ImageIndex = 2;
            tabPage1.Location = new Point(4, 31);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(971, 450);
            tabPage1.TabIndex = 1;
            tabPage1.Text = "Kartu Stok";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // il
            // 
            il.ColorDepth = ColorDepth.Depth32Bit;
            il.ImageStream = (ImageListStreamer)resources.GetObject("il.ImageStream");
            il.TransparentColor = Color.Transparent;
            il.Images.SetKeyName(0, "ready-stock (1).png");
            il.Images.SetKeyName(1, "info.png");
            il.Images.SetKeyName(2, "sim-card.png");
            il.Images.SetKeyName(3, "folder.png");
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(19, 289);
            label9.Name = "label9";
            label9.Size = new Size(86, 17);
            label9.TabIndex = 106;
            label9.Text = "Stok Tersedia";
            // 
            // stockTextBox
            // 
            stockTextBox.Location = new Point(150, 283);
            stockTextBox.MaxLength = 15;
            stockTextBox.Name = "stockTextBox";
            stockTextBox.Size = new Size(120, 25);
            stockTextBox.TabIndex = 105;
            // 
            // button4
            // 
            button4.Location = new Point(757, 401);
            button4.Name = "button4";
            button4.Size = new Size(96, 32);
            button4.TabIndex = 104;
            button4.Text = "Save Image As";
            button4.UseVisualStyleBackColor = true;
            button4.Click += button4_Click;
            // 
            // button3
            // 
            button3.Location = new Point(859, 401);
            button3.Name = "button3";
            button3.Size = new Size(43, 32);
            button3.TabIndex = 103;
            button3.Text = "▶";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // button1
            // 
            button1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button1.ImageIndex = 3;
            button1.Location = new Point(1577, 404);
            button1.Name = "button1";
            button1.Size = new Size(142, 32);
            button1.TabIndex = 101;
            button1.Text = "Tambah Gambar";
            button1.TextImageRelation = TextImageRelation.ImageBeforeText;
            button1.UseVisualStyleBackColor = true;
            // 
            // productImage
            // 
            productImage.BorderStyle = BorderStyle.FixedSingle;
            productImage.Location = new Point(576, 20);
            productImage.Name = "productImage";
            productImage.Size = new Size(375, 375);
            productImage.SizeMode = PictureBoxSizeMode.Zoom;
            productImage.TabIndex = 20;
            productImage.TabStop = false;
            // 
            // isactiveCheckBox
            // 
            isactiveCheckBox.AutoSize = true;
            isactiveCheckBox.Location = new Point(150, 391);
            isactiveCheckBox.Name = "isactiveCheckBox";
            isactiveCheckBox.Size = new Size(95, 21);
            isactiveCheckBox.TabIndex = 11;
            isactiveCheckBox.Text = "Aktif (dijual)";
            isactiveCheckBox.UseVisualStyleBackColor = true;
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Location = new Point(310, 369);
            label11.Name = "label11";
            label11.Size = new Size(92, 17);
            label11.TabIndex = 26;
            label11.Text = "Stok Maksimal";
            // 
            // maxstockTextBox
            // 
            maxstockTextBox.Location = new Point(430, 360);
            maxstockTextBox.MaxLength = 15;
            maxstockTextBox.Name = "maxstockTextBox";
            maxstockTextBox.Size = new Size(120, 25);
            maxstockTextBox.TabIndex = 9;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new Point(310, 343);
            label10.Name = "label10";
            label10.Size = new Size(83, 17);
            label10.TabIndex = 24;
            label10.Text = "Stok Minimal";
            // 
            // minstockTextBox
            // 
            minstockTextBox.Location = new Point(430, 334);
            minstockTextBox.MaxLength = 15;
            minstockTextBox.Name = "minstockTextBox";
            minstockTextBox.Size = new Size(120, 25);
            minstockTextBox.TabIndex = 8;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(310, 289);
            label7.Name = "label7";
            label7.Size = new Size(93, 17);
            label7.TabIndex = 16;
            label7.Text = "Satuan barang";
            // 
            // unitComboBox
            // 
            unitComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            unitComboBox.Location = new Point(430, 283);
            unitComboBox.Name = "unitComboBox";
            unitComboBox.Size = new Size(120, 25);
            unitComboBox.TabIndex = 4;
            // 
            // button6
            // 
            button6.BackColor = Color.Red;
            button6.FlatAppearance.BorderSize = 0;
            button6.FlatStyle = FlatStyle.Flat;
            button6.ForeColor = Color.White;
            button6.Location = new Point(248, 517);
            button6.Name = "button6";
            button6.Size = new Size(173, 32);
            button6.TabIndex = 11;
            button6.Text = "Kategori Produk";
            button6.UseVisualStyleBackColor = false;
            // 
            // loginButton
            // 
            loginButton.BackColor = SystemColors.HotTrack;
            loginButton.FlatAppearance.BorderSize = 0;
            loginButton.FlatStyle = FlatStyle.Flat;
            loginButton.ForeColor = Color.White;
            loginButton.Location = new Point(13, 517);
            loginButton.Name = "loginButton";
            loginButton.Size = new Size(103, 32);
            loginButton.TabIndex = 9;
            loginButton.Text = "💾 Simpan";
            loginButton.UseVisualStyleBackColor = false;
            loginButton.Click += loginButton_Click;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(19, 164);
            label6.Name = "label6";
            label6.Size = new Size(58, 17);
            label6.TabIndex = 14;
            label6.Text = "Kategori";
            // 
            // categoryComboBox
            // 
            categoryComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            categoryComboBox.Location = new Point(150, 158);
            categoryComboBox.Name = "categoryComboBox";
            categoryComboBox.Size = new Size(400, 25);
            categoryComboBox.TabIndex = 3;
            // 
            // basicpriceTextBox
            // 
            basicpriceTextBox.Location = new Point(150, 334);
            basicpriceTextBox.MaxLength = 15;
            basicpriceTextBox.Name = "basicpriceTextBox";
            basicpriceTextBox.Size = new Size(120, 25);
            basicpriceTextBox.TabIndex = 100;
            basicpriceTextBox.TabStop = false;
            basicpriceTextBox.Enter += BasicPriceTextBoxEnter;
            basicpriceTextBox.Leave += BasicPriceTextBoxLeave;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(16, 369);
            label3.Name = "label3";
            label3.Size = new Size(87, 17);
            label3.TabIndex = 8;
            label3.Text = "Harga Satuan";
            // 
            // priceTextBox
            // 
            priceTextBox.Location = new Point(150, 360);
            priceTextBox.MaxLength = 15;
            priceTextBox.Name = "priceTextBox";
            priceTextBox.Size = new Size(120, 25);
            priceTextBox.TabIndex = 6;
            priceTextBox.Enter += BasicPriceTextBoxEnter;
            priceTextBox.Leave += BasicPriceTextBoxLeave;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(16, 78);
            label2.Name = "label2";
            label2.Size = new Size(61, 17);
            label2.TabIndex = 6;
            label2.Text = "Deskripsi";
            // 
            // descriptionTextBox
            // 
            descriptionTextBox.Location = new Point(150, 83);
            descriptionTextBox.Multiline = true;
            descriptionTextBox.Name = "descriptionTextBox";
            descriptionTextBox.ScrollBars = ScrollBars.Both;
            descriptionTextBox.Size = new Size(400, 69);
            descriptionTextBox.TabIndex = 2;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(16, 52);
            label1.Name = "label1";
            label1.Size = new Size(88, 17);
            label1.TabIndex = 4;
            label1.Text = "Nama Barang";
            // 
            // nameTextBox
            // 
            nameTextBox.Location = new Point(150, 52);
            nameTextBox.Name = "nameTextBox";
            nameTextBox.Size = new Size(400, 25);
            nameTextBox.TabIndex = 1;
            // 
            // skuTextBox
            // 
            skuTextBox.Location = new Point(150, 20);
            skuTextBox.Name = "skuTextBox";
            skuTextBox.Size = new Size(400, 25);
            skuTextBox.TabIndex = 0;
            // 
            // skuLabel
            // 
            skuLabel.AutoSize = true;
            skuLabel.Location = new Point(16, 26);
            skuLabel.Name = "skuLabel";
            skuLabel.Size = new Size(32, 17);
            skuLabel.TabIndex = 3;
            skuLabel.Text = "SKU";
            // 
            // button5
            // 
            button5.BackColor = Color.Red;
            button5.FlatAppearance.BorderSize = 0;
            button5.FlatStyle = FlatStyle.Flat;
            button5.ForeColor = Color.White;
            button5.Location = new Point(122, 517);
            button5.Name = "button5";
            button5.Size = new Size(120, 32);
            button5.TabIndex = 10;
            button5.Text = "Tutup";
            button5.UseVisualStyleBackColor = false;
            // 
            // commanTabPage
            // 
            commanTabPage.Controls.Add(button10);
            commanTabPage.Controls.Add(button9);
            commanTabPage.Controls.Add(button8);
            commanTabPage.Controls.Add(button7);
            commanTabPage.Controls.Add(label9);
            commanTabPage.Controls.Add(stockTextBox);
            commanTabPage.Controls.Add(button4);
            commanTabPage.Controls.Add(button3);
            commanTabPage.Controls.Add(button2);
            commanTabPage.Controls.Add(button1);
            commanTabPage.Controls.Add(productImage);
            commanTabPage.Controls.Add(isactiveCheckBox);
            commanTabPage.Controls.Add(label11);
            commanTabPage.Controls.Add(maxstockTextBox);
            commanTabPage.Controls.Add(label10);
            commanTabPage.Controls.Add(minstockTextBox);
            commanTabPage.Controls.Add(label7);
            commanTabPage.Controls.Add(unitComboBox);
            commanTabPage.Controls.Add(label6);
            commanTabPage.Controls.Add(categoryComboBox);
            commanTabPage.Controls.Add(label4);
            commanTabPage.Controls.Add(basicpriceTextBox);
            commanTabPage.Controls.Add(label3);
            commanTabPage.Controls.Add(priceTextBox);
            commanTabPage.Controls.Add(label2);
            commanTabPage.Controls.Add(descriptionTextBox);
            commanTabPage.Controls.Add(label1);
            commanTabPage.Controls.Add(nameTextBox);
            commanTabPage.Controls.Add(skuTextBox);
            commanTabPage.Controls.Add(skuLabel);
            commanTabPage.ImageIndex = 1;
            commanTabPage.Location = new Point(4, 31);
            commanTabPage.Name = "commanTabPage";
            commanTabPage.Padding = new Padding(3);
            commanTabPage.Size = new Size(971, 450);
            commanTabPage.TabIndex = 0;
            commanTabPage.Text = "Umum";
            commanTabPage.UseVisualStyleBackColor = true;
            // 
            // button10
            // 
            button10.Location = new Point(715, 401);
            button10.Name = "button10";
            button10.Size = new Size(34, 32);
            button10.TabIndex = 110;
            button10.Text = "x";
            button10.UseVisualStyleBackColor = true;
            button10.Click += button10_Click;
            // 
            // button9
            // 
            button9.Location = new Point(576, 401);
            button9.Name = "button9";
            button9.Size = new Size(43, 32);
            button9.TabIndex = 109;
            button9.Text = "|◀";
            button9.UseVisualStyleBackColor = true;
            button9.Click += button9_Click;
            // 
            // button8
            // 
            button8.Location = new Point(908, 401);
            button8.Name = "button8";
            button8.Size = new Size(43, 32);
            button8.TabIndex = 108;
            button8.Text = "▶|";
            button8.UseVisualStyleBackColor = true;
            button8.Click += button8_Click;
            // 
            // button7
            // 
            button7.Location = new Point(675, 401);
            button7.Name = "button7";
            button7.Size = new Size(34, 32);
            button7.TabIndex = 107;
            button7.Text = "+";
            button7.UseVisualStyleBackColor = true;
            button7.Click += button7_Click;
            // 
            // button2
            // 
            button2.Location = new Point(626, 401);
            button2.Name = "button2";
            button2.Size = new Size(43, 32);
            button2.TabIndex = 102;
            button2.Text = "◀";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(16, 340);
            label4.Name = "label4";
            label4.Size = new Size(83, 17);
            label4.TabIndex = 10;
            label4.Text = "Harga Pokok";
            // 
            // tabControl
            // 
            tabControl.Controls.Add(commanTabPage);
            tabControl.Controls.Add(tabPage1);
            tabControl.Controls.Add(tabPage2);
            tabControl.HotTrack = true;
            tabControl.ImageList = il;
            tabControl.Location = new Point(13, 12);
            tabControl.Multiline = true;
            tabControl.Name = "tabControl";
            tabControl.Padding = new Point(10, 3);
            tabControl.SelectedIndex = 0;
            tabControl.Size = new Size(979, 485);
            tabControl.TabIndex = 8;
            // 
            // tabPage2
            // 
            tabPage2.Location = new Point(4, 31);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(971, 450);
            tabPage2.TabIndex = 2;
            tabPage2.Text = "Inventory";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // ProductForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1005, 561);
            Controls.Add(button6);
            Controls.Add(loginButton);
            Controls.Add(button5);
            Controls.Add(tabControl);
            Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ProductForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "ProductForm";
            Load += ProductForm_Load;
            ((System.ComponentModel.ISupportInitialize)productImage).EndInit();
            commanTabPage.ResumeLayout(false);
            commanTabPage.PerformLayout();
            tabControl.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private TabPage tabPage1;
        private ImageList il;
        private Label label9;
        private TextBox stockTextBox;
        private Button button4;
        private Button button3;
        private Button button1;
        private PictureBox productImage;
        private CheckBox isactiveCheckBox;
        private Label label11;
        private TextBox maxstockTextBox;
        private Label label10;
        private TextBox minstockTextBox;
        private Label label7;
        private ComboBox unitComboBox;
        private Button button6;
        private Button loginButton;
        private Label label6;
        private ComboBox categoryComboBox;
        private TextBox basicpriceTextBox;
        private Label label3;
        private TextBox priceTextBox;
        private Label label2;
        private TextBox descriptionTextBox;
        private Label label1;
        private TextBox nameTextBox;
        private TextBox skuTextBox;
        private Label skuLabel;
        private Button button5;
        private TabPage commanTabPage;
        private Label label4;
        private TabControl tabControl;
        private Button button7;
        private Button button9;
        private Button button8;
        private Button button2;
        private Button button10;
        private TabPage tabPage2;
    }
}