using Astro.Winform.Controls;

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
            tabPage1 = new TabPage();
            label9 = new Label();
            stockTextBox = new TextBox();
            button1 = new Button();
            isactiveCheckBox = new CheckBox();
            label11 = new Label();
            maxstockTextBox = new TextBox();
            label10 = new Label();
            minstockTextBox = new TextBox();
            label7 = new Label();
            unitComboBox = new ComboBox();
            saveButton = new Button();
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
            commanTabPage = new TabPage();
            marginTextBox = new TextBox();
            label13 = new Label();
            label4 = new Label();
            label8 = new Label();
            taxFactorNumericUpDown = new NumericUpDown();
            taxableCheckBox = new CheckBox();
            label5 = new Label();
            typeComboBox = new ComboBox();
            tabControl = new FlatTabControl();
            tabPage2 = new TabPage();
            button10 = new Button();
            button9 = new Button();
            button8 = new Button();
            button7 = new Button();
            button4 = new Button();
            button3 = new Button();
            button2 = new Button();
            productImage = new PictureBox();
            panel1 = new Panel();
            titleLabel = new Label();
            commanTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)taxFactorNumericUpDown).BeginInit();
            tabControl.SuspendLayout();
            tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)productImage).BeginInit();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // tabPage1
            // 
            tabPage1.ImageIndex = 2;
            tabPage1.Location = new Point(4, 44);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(392, 668);
            tabPage1.TabIndex = 1;
            tabPage1.Text = "Kartu Stok";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(12, 302);
            label9.Margin = new Padding(0, 0, 3, 0);
            label9.Name = "label9";
            label9.Size = new Size(33, 17);
            label9.TabIndex = 106;
            label9.Text = "Stok";
            // 
            // stockTextBox
            // 
            stockTextBox.Location = new Point(15, 322);
            stockTextBox.MaxLength = 15;
            stockTextBox.Name = "stockTextBox";
            stockTextBox.ReadOnly = true;
            stockTextBox.Size = new Size(117, 25);
            stockTextBox.TabIndex = 7;
            stockTextBox.TabStop = false;
            // 
            // button1
            // 
            button1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button1.ImageIndex = 3;
            button1.Location = new Point(998, 404);
            button1.Name = "button1";
            button1.Size = new Size(142, 32);
            button1.TabIndex = 101;
            button1.Text = "Tambah Gambar";
            button1.TextImageRelation = TextImageRelation.ImageBeforeText;
            button1.UseVisualStyleBackColor = true;
            // 
            // isactiveCheckBox
            // 
            isactiveCheckBox.AutoSize = true;
            isactiveCheckBox.Location = new Point(17, 500);
            isactiveCheckBox.Name = "isactiveCheckBox";
            isactiveCheckBox.Size = new Size(95, 21);
            isactiveCheckBox.TabIndex = 9;
            isactiveCheckBox.Text = "Aktif (dijual)";
            isactiveCheckBox.UseVisualStyleBackColor = true;
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Location = new Point(135, 438);
            label11.Name = "label11";
            label11.Size = new Size(92, 17);
            label11.TabIndex = 26;
            label11.Text = "Stok Maksimal";
            // 
            // maxstockTextBox
            // 
            maxstockTextBox.Location = new Point(137, 458);
            maxstockTextBox.MaxLength = 15;
            maxstockTextBox.Name = "maxstockTextBox";
            maxstockTextBox.Size = new Size(117, 25);
            maxstockTextBox.TabIndex = 8;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new Point(13, 438);
            label10.Name = "label10";
            label10.Size = new Size(83, 17);
            label10.TabIndex = 24;
            label10.Text = "Stok Minimal";
            // 
            // minstockTextBox
            // 
            minstockTextBox.Location = new Point(17, 458);
            minstockTextBox.MaxLength = 15;
            minstockTextBox.Name = "minstockTextBox";
            minstockTextBox.Size = new Size(116, 25);
            minstockTextBox.TabIndex = 7;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(133, 302);
            label7.Margin = new Padding(0, 0, 3, 0);
            label7.Name = "label7";
            label7.Size = new Size(47, 17);
            label7.TabIndex = 16;
            label7.Text = "Satuan";
            // 
            // unitComboBox
            // 
            unitComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            unitComboBox.Location = new Point(134, 322);
            unitComboBox.Name = "unitComboBox";
            unitComboBox.Size = new Size(241, 25);
            unitComboBox.TabIndex = 5;
            // 
            // saveButton
            // 
            saveButton.BackColor = Color.Blue;
            saveButton.FlatAppearance.BorderColor = Color.FromArgb(192, 192, 255);
            saveButton.FlatStyle = FlatStyle.Flat;
            saveButton.ForeColor = Color.White;
            saveButton.Location = new Point(19, 14);
            saveButton.Name = "saveButton";
            saveButton.Size = new Size(360, 32);
            saveButton.TabIndex = 12;
            saveButton.Text = "+ Tambahkan Produk";
            saveButton.UseVisualStyleBackColor = false;
            saveButton.Click += loginButton_Click;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(12, 196);
            label6.Name = "label6";
            label6.Size = new Size(58, 17);
            label6.TabIndex = 14;
            label6.Text = "Kategori";
            // 
            // categoryComboBox
            // 
            categoryComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            categoryComboBox.Location = new Point(15, 216);
            categoryComboBox.Name = "categoryComboBox";
            categoryComboBox.Size = new Size(360, 25);
            categoryComboBox.TabIndex = 3;
            // 
            // basicpriceTextBox
            // 
            basicpriceTextBox.Location = new Point(16, 405);
            basicpriceTextBox.MaxLength = 15;
            basicpriceTextBox.Name = "basicpriceTextBox";
            basicpriceTextBox.ReadOnly = true;
            basicpriceTextBox.Size = new Size(117, 25);
            basicpriceTextBox.TabIndex = 8;
            basicpriceTextBox.TabStop = false;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(135, 385);
            label3.Margin = new Padding(0, 0, 3, 0);
            label3.Name = "label3";
            label3.Size = new Size(70, 17);
            label3.TabIndex = 8;
            label3.Text = "Harga Jual";
            // 
            // priceTextBox
            // 
            priceTextBox.Location = new Point(137, 405);
            priceTextBox.MaxLength = 15;
            priceTextBox.Name = "priceTextBox";
            priceTextBox.Size = new Size(117, 25);
            priceTextBox.TabIndex = 6;
            priceTextBox.TextChanged += priceTextBox_TextChanged;
            priceTextBox.Enter += BasicPriceTextBoxEnter;
            priceTextBox.Leave += BasicPriceTextBoxLeave;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 121);
            label2.Name = "label2";
            label2.Size = new Size(61, 17);
            label2.TabIndex = 6;
            label2.Text = "Deskripsi";
            // 
            // descriptionTextBox
            // 
            descriptionTextBox.Location = new Point(15, 141);
            descriptionTextBox.Multiline = true;
            descriptionTextBox.Name = "descriptionTextBox";
            descriptionTextBox.ScrollBars = ScrollBars.Both;
            descriptionTextBox.Size = new Size(360, 47);
            descriptionTextBox.TabIndex = 2;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 15);
            label1.Name = "label1";
            label1.Size = new Size(88, 17);
            label1.TabIndex = 4;
            label1.Text = "Nama Barang";
            label1.Click += label1_Click;
            // 
            // nameTextBox
            // 
            nameTextBox.Location = new Point(15, 35);
            nameTextBox.Name = "nameTextBox";
            nameTextBox.Size = new Size(360, 25);
            nameTextBox.TabIndex = 0;
            // 
            // skuTextBox
            // 
            skuTextBox.Location = new Point(16, 88);
            skuTextBox.Name = "skuTextBox";
            skuTextBox.Size = new Size(359, 25);
            skuTextBox.TabIndex = 1;
            // 
            // skuLabel
            // 
            skuLabel.AutoSize = true;
            skuLabel.Location = new Point(12, 68);
            skuLabel.Name = "skuLabel";
            skuLabel.Size = new Size(32, 17);
            skuLabel.TabIndex = 3;
            skuLabel.Text = "SKU";
            // 
            // commanTabPage
            // 
            commanTabPage.Controls.Add(marginTextBox);
            commanTabPage.Controls.Add(label13);
            commanTabPage.Controls.Add(basicpriceTextBox);
            commanTabPage.Controls.Add(priceTextBox);
            commanTabPage.Controls.Add(label4);
            commanTabPage.Controls.Add(label9);
            commanTabPage.Controls.Add(label3);
            commanTabPage.Controls.Add(label8);
            commanTabPage.Controls.Add(taxFactorNumericUpDown);
            commanTabPage.Controls.Add(minstockTextBox);
            commanTabPage.Controls.Add(label10);
            commanTabPage.Controls.Add(taxableCheckBox);
            commanTabPage.Controls.Add(maxstockTextBox);
            commanTabPage.Controls.Add(label5);
            commanTabPage.Controls.Add(label11);
            commanTabPage.Controls.Add(typeComboBox);
            commanTabPage.Controls.Add(stockTextBox);
            commanTabPage.Controls.Add(button1);
            commanTabPage.Controls.Add(isactiveCheckBox);
            commanTabPage.Controls.Add(label7);
            commanTabPage.Controls.Add(unitComboBox);
            commanTabPage.Controls.Add(label6);
            commanTabPage.Controls.Add(categoryComboBox);
            commanTabPage.Controls.Add(label2);
            commanTabPage.Controls.Add(descriptionTextBox);
            commanTabPage.Controls.Add(label1);
            commanTabPage.Controls.Add(nameTextBox);
            commanTabPage.Controls.Add(skuTextBox);
            commanTabPage.Controls.Add(skuLabel);
            commanTabPage.ImageIndex = 1;
            commanTabPage.Location = new Point(4, 44);
            commanTabPage.Name = "commanTabPage";
            commanTabPage.Padding = new Padding(3);
            commanTabPage.Size = new Size(392, 668);
            commanTabPage.TabIndex = 0;
            commanTabPage.Text = "ℹ️ Umum";
            commanTabPage.UseVisualStyleBackColor = true;
            // 
            // marginTextBox
            // 
            marginTextBox.Location = new Point(259, 405);
            marginTextBox.MaxLength = 15;
            marginTextBox.Name = "marginTextBox";
            marginTextBox.ReadOnly = true;
            marginTextBox.Size = new Size(117, 25);
            marginTextBox.TabIndex = 119;
            marginTextBox.TabStop = false;
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Location = new Point(256, 385);
            label13.Margin = new Padding(0, 0, 3, 0);
            label13.Name = "label13";
            label13.Size = new Size(50, 17);
            label13.TabIndex = 118;
            label13.Text = "Margin";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(13, 385);
            label4.Margin = new Padding(0, 0, 3, 0);
            label4.Name = "label4";
            label4.Size = new Size(99, 17);
            label4.TabIndex = 10;
            label4.Text = "Harga rata-rata";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(259, 503);
            label8.Name = "label8";
            label8.Size = new Size(61, 17);
            label8.TabIndex = 115;
            label8.Text = "Nilai DPP";
            // 
            // taxFactorNumericUpDown
            // 
            taxFactorNumericUpDown.Location = new Point(260, 523);
            taxFactorNumericUpDown.Name = "taxFactorNumericUpDown";
            taxFactorNumericUpDown.Size = new Size(115, 25);
            taxFactorNumericUpDown.TabIndex = 11;
            // 
            // taxableCheckBox
            // 
            taxableCheckBox.AutoSize = true;
            taxableCheckBox.Location = new Point(17, 527);
            taxableCheckBox.Name = "taxableCheckBox";
            taxableCheckBox.Size = new Size(165, 21);
            taxableCheckBox.TabIndex = 10;
            taxableCheckBox.Text = "Produk dikenakan pajak";
            taxableCheckBox.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(12, 249);
            label5.Name = "label5";
            label5.Size = new Size(78, 17);
            label5.TabIndex = 112;
            label5.Text = "Tipe Produk";
            // 
            // typeComboBox
            // 
            typeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            typeComboBox.Items.AddRange(new object[] { "Barang", "Jasa (Service)" });
            typeComboBox.Location = new Point(15, 269);
            typeComboBox.Name = "typeComboBox";
            typeComboBox.Size = new Size(360, 25);
            typeComboBox.TabIndex = 4;
            // 
            // tabControl
            // 
            tabControl.ActiveColor = Color.DeepSkyBlue;
            tabControl.Controls.Add(commanTabPage);
            tabControl.Controls.Add(tabPage1);
            tabControl.Controls.Add(tabPage2);
            tabControl.Dock = DockStyle.Fill;
            tabControl.DrawMode = TabDrawMode.OwnerDrawFixed;
            tabControl.HotTrack = true;
            tabControl.InactiveColor = Color.LightGray;
            tabControl.ItemSize = new Size(120, 40);
            tabControl.Location = new Point(0, 40);
            tabControl.Multiline = true;
            tabControl.Name = "tabControl";
            tabControl.Padding = new Point(10, 6);
            tabControl.SelectedIndex = 0;
            tabControl.Size = new Size(400, 716);
            tabControl.TabIndex = 8;
            tabControl.TextColor = Color.Black;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(button10);
            tabPage2.Controls.Add(button9);
            tabPage2.Controls.Add(button8);
            tabPage2.Controls.Add(button7);
            tabPage2.Controls.Add(button4);
            tabPage2.Controls.Add(button3);
            tabPage2.Controls.Add(button2);
            tabPage2.Controls.Add(productImage);
            tabPage2.Location = new Point(4, 44);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(392, 668);
            tabPage2.TabIndex = 2;
            tabPage2.Text = "Gambar";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // button10
            // 
            button10.FlatAppearance.BorderColor = Color.FromArgb(192, 192, 255);
            button10.FlatStyle = FlatStyle.Flat;
            button10.Font = new Font("Segoe UI Symbol", 9.75F);
            button10.Location = new Point(110, 212);
            button10.Name = "button10";
            button10.Size = new Size(28, 26);
            button10.TabIndex = 118;
            button10.Text = "x";
            button10.UseVisualStyleBackColor = true;
            // 
            // button9
            // 
            button9.FlatAppearance.BorderColor = Color.FromArgb(192, 192, 255);
            button9.FlatStyle = FlatStyle.Flat;
            button9.Font = new Font("Segoe UI Symbol", 9.75F);
            button9.ForeColor = Color.MidnightBlue;
            button9.Location = new Point(14, 212);
            button9.Name = "button9";
            button9.Size = new Size(26, 26);
            button9.TabIndex = 117;
            button9.Text = "⏮";
            button9.UseVisualStyleBackColor = true;
            // 
            // button8
            // 
            button8.FlatAppearance.BorderColor = Color.FromArgb(192, 192, 255);
            button8.FlatStyle = FlatStyle.Flat;
            button8.Font = new Font("Segoe UI Symbol", 9.75F);
            button8.ForeColor = Color.MidnightBlue;
            button8.Location = new Point(201, 253);
            button8.Name = "button8";
            button8.Size = new Size(26, 26);
            button8.TabIndex = 116;
            button8.Text = "⏭";
            button8.UseVisualStyleBackColor = true;
            // 
            // button7
            // 
            button7.FlatAppearance.BorderColor = Color.FromArgb(192, 192, 255);
            button7.FlatStyle = FlatStyle.Flat;
            button7.Font = new Font("Segoe UI Symbol", 9.75F);
            button7.Location = new Point(78, 212);
            button7.Name = "button7";
            button7.Size = new Size(26, 26);
            button7.TabIndex = 115;
            button7.Text = "+";
            button7.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            button4.FlatAppearance.BorderColor = Color.FromArgb(192, 192, 255);
            button4.FlatStyle = FlatStyle.Flat;
            button4.Location = new Point(23, 253);
            button4.Name = "button4";
            button4.Size = new Size(114, 26);
            button4.TabIndex = 114;
            button4.Text = "Save Image As";
            button4.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            button3.FlatAppearance.BorderColor = Color.FromArgb(192, 192, 255);
            button3.FlatStyle = FlatStyle.Flat;
            button3.Font = new Font("Segoe UI Symbol", 9.75F);
            button3.ForeColor = Color.MidnightBlue;
            button3.Location = new Point(169, 253);
            button3.Name = "button3";
            button3.Size = new Size(26, 25);
            button3.TabIndex = 113;
            button3.Text = "⏩";
            button3.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            button2.FlatAppearance.BorderColor = Color.FromArgb(192, 192, 255);
            button2.FlatStyle = FlatStyle.Flat;
            button2.Font = new Font("Segoe UI Symbol", 9.75F);
            button2.ForeColor = Color.MidnightBlue;
            button2.Location = new Point(46, 212);
            button2.Name = "button2";
            button2.Size = new Size(26, 26);
            button2.TabIndex = 112;
            button2.Text = "⏪";
            button2.UseVisualStyleBackColor = true;
            // 
            // productImage
            // 
            productImage.BorderStyle = BorderStyle.FixedSingle;
            productImage.Location = new Point(14, 6);
            productImage.Name = "productImage";
            productImage.Size = new Size(213, 200);
            productImage.SizeMode = PictureBoxSizeMode.Zoom;
            productImage.TabIndex = 111;
            productImage.TabStop = false;
            // 
            // panel1
            // 
            panel1.Controls.Add(saveButton);
            panel1.Dock = DockStyle.Bottom;
            panel1.Location = new Point(0, 700);
            panel1.Name = "panel1";
            panel1.Size = new Size(400, 56);
            panel1.TabIndex = 9;
            // 
            // titleLabel
            // 
            titleLabel.Dock = DockStyle.Top;
            titleLabel.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point, 0);
            titleLabel.Location = new Point(0, 0);
            titleLabel.Name = "titleLabel";
            titleLabel.Padding = new Padding(5, 0, 0, 0);
            titleLabel.Size = new Size(400, 40);
            titleLabel.TabIndex = 10;
            titleLabel.Text = "Tambah Produk";
            titleLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // ProductForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(250, 250, 250);
            BorderStyle = BorderStyle.FixedSingle;
            Controls.Add(panel1);
            Controls.Add(tabControl);
            Controls.Add(titleLabel);
            Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Name = "ProductForm";
            Size = new Size(400, 756);
            Load += ProductForm_Load;
            commanTabPage.ResumeLayout(false);
            commanTabPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)taxFactorNumericUpDown).EndInit();
            tabControl.ResumeLayout(false);
            tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)productImage).EndInit();
            panel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private TabPage tabPage1;
        private Label label9;
        private TextBox stockTextBox;
        private Button button1;
        private CheckBox isactiveCheckBox;
        private Label label11;
        private TextBox maxstockTextBox;
        private Label label10;
        private TextBox minstockTextBox;
        private Label label7;
        private ComboBox unitComboBox;
        private Button saveButton;
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
        private TabPage commanTabPage;
        private Label label4;
        private Label label5;
        private ComboBox typeComboBox;
        private CheckBox taxableCheckBox;
        private Label label8;
        private NumericUpDown taxFactorNumericUpDown;
        private Panel panel1;
        private Label titleLabel;
        private FlatTabControl tabControl;
        private TabPage tabPage2;
        private Button button10;
        private Button button9;
        private Button button8;
        private Button button7;
        private Button button4;
        private Button button3;
        private Button button2;
        private PictureBox productImage;
        private TextBox marginTextBox;
        private Label label13;
    }
}