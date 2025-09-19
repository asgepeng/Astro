using Astro.Winform.Controls;

namespace Astro.Winform.UserControls
{
    partial class PurchaseControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle10 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle4 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle5 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle6 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle7 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle8 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle9 = new DataGridViewCellStyle();
            grid = new DGrid();
            idDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            nameDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn1 = new DataGridViewTextBoxColumn();
            quantityDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            unitDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            priceDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            discountDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            nettPriceDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            totalDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            Delete = new DataGridViewButtonColumn();
            purchaseItemBindingSource = new BindingSource(components);
            label2 = new Label();
            dateTimePicker1 = new DateTimePicker();
            label3 = new Label();
            supplierNameTextBox = new TextBox();
            label4 = new Label();
            barcodeTextBox = new TextBox();
            grandTotalLabel = new Label();
            button2 = new Button();
            subTotalTextBox = new TextBox();
            label7 = new Label();
            discountTextBox = new TextBox();
            label8 = new Label();
            costTextBox = new TextBox();
            label10 = new Label();
            textBox7 = new TextBox();
            label11 = new Label();
            label12 = new Label();
            comboBox2 = new ComboBox();
            paidAmountTextBox = new TextBox();
            label13 = new Label();
            button3 = new Button();
            button4 = new Button();
            refundTextBox = new TextBox();
            label14 = new Label();
            button5 = new Button();
            textBox4 = new TextBox();
            label5 = new Label();
            button7 = new Button();
            button8 = new Button();
            button1 = new Button();
            button9 = new Button();
            grandTotalTextBox = new TextBox();
            label15 = new Label();
            textBox2 = new TextBox();
            label16 = new Label();
            supplierIdTextBox = new TextBox();
            label17 = new Label();
            ((System.ComponentModel.ISupportInitialize)grid).BeginInit();
            ((System.ComponentModel.ISupportInitialize)purchaseItemBindingSource).BeginInit();
            SuspendLayout();
            // 
            // grid
            // 
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = Color.FromArgb(244, 244, 244);
            dataGridViewCellStyle1.ForeColor = SystemColors.ControlText;
            grid.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            grid.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            grid.AutoGenerateColumns = false;
            grid.BackgroundColor = Color.White;
            grid.BorderStyle = BorderStyle.None;
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            grid.ColumnHeadersHeight = 30;
            grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            grid.Columns.AddRange(new DataGridViewColumn[] { idDataGridViewTextBoxColumn, nameDataGridViewTextBoxColumn, dataGridViewTextBoxColumn1, quantityDataGridViewTextBoxColumn, unitDataGridViewTextBoxColumn, priceDataGridViewTextBoxColumn, discountDataGridViewTextBoxColumn, nettPriceDataGridViewTextBoxColumn, totalDataGridViewTextBoxColumn, Delete });
            grid.DataSource = purchaseItemBindingSource;
            dataGridViewCellStyle10.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle10.BackColor = SystemColors.Window;
            dataGridViewCellStyle10.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            dataGridViewCellStyle10.ForeColor = SystemColors.ControlText;
            dataGridViewCellStyle10.SelectionBackColor = SystemColors.ButtonFace;
            dataGridViewCellStyle10.SelectionForeColor = SystemColors.ControlText;
            dataGridViewCellStyle10.WrapMode = DataGridViewTriState.False;
            grid.DefaultCellStyle = dataGridViewCellStyle10;
            grid.EnableHeadersVisualStyles = false;
            grid.GridColor = Color.FromArgb(244, 244, 244);
            grid.Location = new Point(-1, 162);
            grid.MultiSelect = false;
            grid.Name = "grid";
            grid.ReadOnly = true;
            grid.RowHeadersVisible = false;
            grid.RowTemplate.Height = 30;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.Size = new Size(1254, 374);
            grid.TabIndex = 6;
            grid.CellContentClick += GridCellContentClick;
            grid.CellEndEdit += GridCellEndEdit;
            grid.RowsAdded += GridRowsAdded;
            grid.RowsRemoved += GridRowsRemoved;
            // 
            // idDataGridViewTextBoxColumn
            // 
            idDataGridViewTextBoxColumn.DataPropertyName = "Id";
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = Color.FromArgb(250, 250, 250);
            dataGridViewCellStyle2.Format = "00000";
            idDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle2;
            idDataGridViewTextBoxColumn.HeaderText = "Kode";
            idDataGridViewTextBoxColumn.Name = "idDataGridViewTextBoxColumn";
            idDataGridViewTextBoxColumn.ReadOnly = true;
            idDataGridViewTextBoxColumn.Width = 60;
            // 
            // nameDataGridViewTextBoxColumn
            // 
            nameDataGridViewTextBoxColumn.DataPropertyName = "Name";
            nameDataGridViewTextBoxColumn.HeaderText = "Nama Barang / Produk";
            nameDataGridViewTextBoxColumn.Name = "nameDataGridViewTextBoxColumn";
            nameDataGridViewTextBoxColumn.ReadOnly = true;
            nameDataGridViewTextBoxColumn.Width = 350;
            // 
            // dataGridViewTextBoxColumn1
            // 
            dataGridViewTextBoxColumn1.DataPropertyName = "Sku";
            dataGridViewTextBoxColumn1.HeaderText = "Sku / Barcode";
            dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            dataGridViewTextBoxColumn1.ReadOnly = true;
            dataGridViewTextBoxColumn1.Width = 150;
            // 
            // quantityDataGridViewTextBoxColumn
            // 
            quantityDataGridViewTextBoxColumn.DataPropertyName = "Quantity";
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleRight;
            quantityDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle3;
            quantityDataGridViewTextBoxColumn.HeaderText = "Quantity";
            quantityDataGridViewTextBoxColumn.Name = "quantityDataGridViewTextBoxColumn";
            quantityDataGridViewTextBoxColumn.ReadOnly = true;
            quantityDataGridViewTextBoxColumn.Width = 80;
            // 
            // unitDataGridViewTextBoxColumn
            // 
            unitDataGridViewTextBoxColumn.DataPropertyName = "Unit";
            dataGridViewCellStyle4.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = Color.FromArgb(250, 250, 250);
            dataGridViewCellStyle4.Format = "N0";
            unitDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle4;
            unitDataGridViewTextBoxColumn.HeaderText = "Unit";
            unitDataGridViewTextBoxColumn.Name = "unitDataGridViewTextBoxColumn";
            unitDataGridViewTextBoxColumn.ReadOnly = true;
            unitDataGridViewTextBoxColumn.Width = 80;
            // 
            // priceDataGridViewTextBoxColumn
            // 
            priceDataGridViewTextBoxColumn.DataPropertyName = "Price";
            dataGridViewCellStyle5.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle5.Format = "N0";
            priceDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle5;
            priceDataGridViewTextBoxColumn.HeaderText = "Harga";
            priceDataGridViewTextBoxColumn.Name = "priceDataGridViewTextBoxColumn";
            priceDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // discountDataGridViewTextBoxColumn
            // 
            discountDataGridViewTextBoxColumn.DataPropertyName = "Discount";
            dataGridViewCellStyle6.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle6.Format = "N0";
            discountDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle6;
            discountDataGridViewTextBoxColumn.HeaderText = "Diskon";
            discountDataGridViewTextBoxColumn.Name = "discountDataGridViewTextBoxColumn";
            discountDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // nettPriceDataGridViewTextBoxColumn
            // 
            nettPriceDataGridViewTextBoxColumn.DataPropertyName = "NettPrice";
            dataGridViewCellStyle7.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle7.BackColor = Color.FromArgb(250, 250, 250);
            dataGridViewCellStyle7.Format = "N0";
            nettPriceDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle7;
            nettPriceDataGridViewTextBoxColumn.HeaderText = "Harga Bersih";
            nettPriceDataGridViewTextBoxColumn.Name = "nettPriceDataGridViewTextBoxColumn";
            nettPriceDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // totalDataGridViewTextBoxColumn
            // 
            totalDataGridViewTextBoxColumn.DataPropertyName = "Total";
            dataGridViewCellStyle8.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle8.BackColor = Color.FromArgb(250, 250, 250);
            dataGridViewCellStyle8.Format = "N0";
            totalDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle8;
            totalDataGridViewTextBoxColumn.HeaderText = "Total";
            totalDataGridViewTextBoxColumn.Name = "totalDataGridViewTextBoxColumn";
            totalDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // Delete
            // 
            Delete.DataPropertyName = "Delete";
            dataGridViewCellStyle9.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle9.BackColor = Color.FromArgb(250, 250, 250);
            Delete.DefaultCellStyle = dataGridViewCellStyle9;
            Delete.HeaderText = "Action";
            Delete.Name = "Delete";
            Delete.ReadOnly = true;
            Delete.Resizable = DataGridViewTriState.False;
            Delete.Text = "Hapus";
            Delete.UseColumnTextForButtonValue = true;
            Delete.Width = 70;
            // 
            // purchaseItemBindingSource
            // 
            purchaseItemBindingSource.DataSource = typeof(Models.Transactions.PurchaseItem);
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(522, 62);
            label2.Name = "label2";
            label2.Size = new Size(54, 17);
            label2.TabIndex = 1;
            label2.Text = "Tanggal";
            // 
            // dateTimePicker1
            // 
            dateTimePicker1.CustomFormat = "dd/MM/yyyy HH:mm";
            dateTimePicker1.Format = DateTimePickerFormat.Custom;
            dateTimePicker1.Location = new Point(597, 59);
            dateTimePicker1.Name = "dateTimePicker1";
            dateTimePicker1.Size = new Size(139, 25);
            dateTimePicker1.TabIndex = 2;
            dateTimePicker1.ValueChanged += PurchaseDateChanged;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(10, 88);
            label3.Name = "label3";
            label3.Size = new Size(56, 17);
            label3.TabIndex = 3;
            label3.Text = "Supplier";
            // 
            // supplierNameTextBox
            // 
            supplierNameTextBox.BackColor = Color.White;
            supplierNameTextBox.Location = new Point(158, 85);
            supplierNameTextBox.Name = "supplierNameTextBox";
            supplierNameTextBox.ReadOnly = true;
            supplierNameTextBox.Size = new Size(348, 25);
            supplierNameTextBox.TabIndex = 4;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(11, 134);
            label4.Name = "label4";
            label4.Size = new Size(87, 17);
            label4.TabIndex = 7;
            label4.Text = "Scan Barcode";
            // 
            // barcodeTextBox
            // 
            barcodeTextBox.BackColor = SystemColors.Info;
            barcodeTextBox.Location = new Point(107, 131);
            barcodeTextBox.Name = "barcodeTextBox";
            barcodeTextBox.Size = new Size(187, 25);
            barcodeTextBox.TabIndex = 8;
            barcodeTextBox.KeyDown += BarcodeTextBoxKeyDown;
            // 
            // grandTotalLabel
            // 
            grandTotalLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            grandTotalLabel.BackColor = Color.AliceBlue;
            grandTotalLabel.BorderStyle = BorderStyle.FixedSingle;
            grandTotalLabel.Font = new Font("Segoe UI", 24F, FontStyle.Regular, GraphicsUnit.Point, 0);
            grandTotalLabel.ForeColor = Color.FromArgb(0, 0, 192);
            grandTotalLabel.Location = new Point(901, 99);
            grandTotalLabel.Name = "grandTotalLabel";
            grandTotalLabel.Size = new Size(338, 58);
            grandTotalLabel.TabIndex = 9;
            grandTotalLabel.Text = "0";
            grandTotalLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // button2
            // 
            button2.BackColor = Color.Red;
            button2.FlatAppearance.BorderColor = Color.FromArgb(192, 0, 0);
            button2.FlatStyle = FlatStyle.Flat;
            button2.Font = new Font("Segoe UI Emoji", 9F);
            button2.ForeColor = Color.White;
            button2.Location = new Point(533, 85);
            button2.Name = "button2";
            button2.Size = new Size(25, 25);
            button2.TabIndex = 12;
            button2.Text = "❌";
            button2.UseVisualStyleBackColor = false;
            button2.Click += ResetSupplierButtonClicked;
            // 
            // subTotalTextBox
            // 
            subTotalTextBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            subTotalTextBox.BackColor = Color.FromArgb(250, 250, 250);
            subTotalTextBox.Location = new Point(107, 553);
            subTotalTextBox.Name = "subTotalTextBox";
            subTotalTextBox.ReadOnly = true;
            subTotalTextBox.Size = new Size(187, 25);
            subTotalTextBox.TabIndex = 14;
            // 
            // label7
            // 
            label7.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label7.AutoSize = true;
            label7.Location = new Point(11, 556);
            label7.Name = "label7";
            label7.Size = new Size(62, 17);
            label7.TabIndex = 13;
            label7.Text = "Sub Total";
            // 
            // discountTextBox
            // 
            discountTextBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            discountTextBox.BackColor = Color.FromArgb(250, 250, 250);
            discountTextBox.Location = new Point(107, 579);
            discountTextBox.Name = "discountTextBox";
            discountTextBox.ReadOnly = true;
            discountTextBox.Size = new Size(187, 25);
            discountTextBox.TabIndex = 16;
            // 
            // label8
            // 
            label8.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label8.AutoSize = true;
            label8.Location = new Point(10, 579);
            label8.Name = "label8";
            label8.Size = new Size(47, 17);
            label8.TabIndex = 15;
            label8.Text = "Diskon";
            // 
            // costTextBox
            // 
            costTextBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            costTextBox.BackColor = Color.FromArgb(250, 250, 250);
            costTextBox.Location = new Point(107, 605);
            costTextBox.Name = "costTextBox";
            costTextBox.ReadOnly = true;
            costTextBox.Size = new Size(161, 25);
            costTextBox.TabIndex = 20;
            // 
            // label10
            // 
            label10.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label10.AutoSize = true;
            label10.Location = new Point(10, 607);
            label10.Name = "label10";
            label10.Size = new Size(38, 17);
            label10.TabIndex = 19;
            label10.Text = "Biaya";
            // 
            // textBox7
            // 
            textBox7.BackColor = Color.AliceBlue;
            textBox7.Location = new Point(319, 59);
            textBox7.Name = "textBox7";
            textBox7.ReadOnly = true;
            textBox7.Size = new Size(187, 25);
            textBox7.TabIndex = 22;
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Location = new Point(269, 62);
            label11.Name = "label11";
            label11.Size = new Size(43, 17);
            label11.TabIndex = 21;
            label11.Text = "Faktur";
            // 
            // label12
            // 
            label12.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label12.AutoSize = true;
            label12.Location = new Point(322, 634);
            label12.Name = "label12";
            label12.Size = new Size(102, 17);
            label12.TabIndex = 24;
            label12.Text = "Pembayaran Via";
            // 
            // comboBox2
            // 
            comboBox2.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            comboBox2.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox2.FormattingEnabled = true;
            comboBox2.Location = new Point(430, 631);
            comboBox2.Name = "comboBox2";
            comboBox2.Size = new Size(318, 25);
            comboBox2.TabIndex = 23;
            // 
            // paidAmountTextBox
            // 
            paidAmountTextBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            paidAmountTextBox.BackColor = SystemColors.Info;
            paidAmountTextBox.Font = new Font("Segoe UI", 24F, FontStyle.Regular, GraphicsUnit.Point, 0);
            paidAmountTextBox.Location = new Point(430, 579);
            paidAmountTextBox.Multiline = true;
            paidAmountTextBox.Name = "paidAmountTextBox";
            paidAmountTextBox.Size = new Size(318, 50);
            paidAmountTextBox.TabIndex = 25;
            paidAmountTextBox.Text = "0";
            paidAmountTextBox.TextChanged += textBox8_TextChanged;
            paidAmountTextBox.KeyPress += textBox8_KeyPress;
            // 
            // label13
            // 
            label13.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label13.AutoSize = true;
            label13.Location = new Point(340, 579);
            label13.Name = "label13";
            label13.Size = new Size(84, 17);
            label13.TabIndex = 26;
            label13.Text = "Jumlah Bayar";
            // 
            // button3
            // 
            button3.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            button3.Enabled = false;
            button3.Font = new Font("Segoe UI Emoji", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            button3.Location = new Point(754, 579);
            button3.Name = "button3";
            button3.Size = new Size(115, 77);
            button3.TabIndex = 27;
            button3.Text = "💾 Save";
            button3.UseVisualStyleBackColor = true;
            button3.Click += SaveButtonClicked;
            // 
            // button4
            // 
            button4.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            button4.Location = new Point(875, 579);
            button4.Name = "button4";
            button4.Size = new Size(77, 77);
            button4.TabIndex = 28;
            button4.Text = "Baru";
            button4.UseVisualStyleBackColor = true;
            button4.Click += button4_Click;
            // 
            // refundTextBox
            // 
            refundTextBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            refundTextBox.BackColor = Color.White;
            refundTextBox.Location = new Point(430, 657);
            refundTextBox.Name = "refundTextBox";
            refundTextBox.ReadOnly = true;
            refundTextBox.Size = new Size(187, 25);
            refundTextBox.TabIndex = 30;
            // 
            // label14
            // 
            label14.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label14.AutoSize = true;
            label14.Location = new Point(334, 660);
            label14.Name = "label14";
            label14.Size = new Size(90, 17);
            label14.TabIndex = 29;
            label14.Text = "Uang Kembali";
            // 
            // button5
            // 
            button5.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            button5.FlatAppearance.BorderColor = Color.FromArgb(192, 192, 255);
            button5.FlatStyle = FlatStyle.Flat;
            button5.Location = new Point(269, 605);
            button5.Name = "button5";
            button5.Size = new Size(25, 25);
            button5.TabIndex = 31;
            button5.Text = "...";
            button5.UseVisualStyleBackColor = true;
            button5.Click += ButtonCostClicked;
            // 
            // textBox4
            // 
            textBox4.BackColor = Color.AliceBlue;
            textBox4.Location = new Point(107, 59);
            textBox4.Name = "textBox4";
            textBox4.ReadOnly = true;
            textBox4.Size = new Size(113, 25);
            textBox4.TabIndex = 33;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(10, 62);
            label5.Name = "label5";
            label5.Size = new Size(71, 17);
            label5.TabIndex = 32;
            label5.Text = "Nomor PO";
            // 
            // button7
            // 
            button7.BackColor = Color.FromArgb(250, 250, 250);
            button7.FlatAppearance.BorderColor = Color.FromArgb(192, 192, 255);
            button7.FlatStyle = FlatStyle.Flat;
            button7.Location = new Point(321, 131);
            button7.Name = "button7";
            button7.Size = new Size(103, 25);
            button7.TabIndex = 35;
            button7.Text = "Produk Baru";
            button7.UseVisualStyleBackColor = false;
            button7.Click += NewProductButtonClicked;
            // 
            // button8
            // 
            button8.BackColor = SystemColors.ButtonFace;
            button8.FlatAppearance.BorderColor = Color.FromArgb(192, 192, 255);
            button8.FlatStyle = FlatStyle.Flat;
            button8.Font = new Font("Segoe UI Emoji", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            button8.ForeColor = SystemColors.ControlDarkDark;
            button8.Location = new Point(295, 131);
            button8.Name = "button8";
            button8.Size = new Size(25, 25);
            button8.TabIndex = 36;
            button8.Text = "🔍";
            button8.UseVisualStyleBackColor = false;
            button8.Click += button8_Click;
            // 
            // button1
            // 
            button1.BackColor = Color.WhiteSmoke;
            button1.FlatAppearance.BorderColor = Color.FromArgb(192, 192, 255);
            button1.FlatStyle = FlatStyle.Flat;
            button1.Font = new Font("Segoe UI Emoji", 9F);
            button1.ForeColor = SystemColors.ControlDarkDark;
            button1.Location = new Point(507, 85);
            button1.Name = "button1";
            button1.Size = new Size(25, 25);
            button1.TabIndex = 5;
            button1.Text = "🔍";
            button1.UseVisualStyleBackColor = false;
            button1.Click += SupplierButtonClicked;
            // 
            // button9
            // 
            button9.BackColor = SystemColors.ButtonFace;
            button9.FlatAppearance.BorderColor = Color.FromArgb(192, 192, 255);
            button9.FlatStyle = FlatStyle.Flat;
            button9.Font = new Font("Segoe UI Emoji", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            button9.ForeColor = SystemColors.ControlDarkDark;
            button9.Location = new Point(221, 59);
            button9.Name = "button9";
            button9.Size = new Size(25, 25);
            button9.TabIndex = 37;
            button9.Text = "🔍";
            button9.UseVisualStyleBackColor = false;
            button9.Click += button9_Click;
            // 
            // grandTotalTextBox
            // 
            grandTotalTextBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            grandTotalTextBox.BackColor = Color.FromArgb(250, 250, 250);
            grandTotalTextBox.Location = new Point(107, 631);
            grandTotalTextBox.Name = "grandTotalTextBox";
            grandTotalTextBox.ReadOnly = true;
            grandTotalTextBox.Size = new Size(187, 25);
            grandTotalTextBox.TabIndex = 39;
            // 
            // label15
            // 
            label15.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label15.AutoSize = true;
            label15.Location = new Point(11, 634);
            label15.Name = "label15";
            label15.Size = new Size(76, 17);
            label15.TabIndex = 38;
            label15.Text = "Grand Total";
            // 
            // textBox2
            // 
            textBox2.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            textBox2.BackColor = Color.White;
            textBox2.Location = new Point(430, 553);
            textBox2.Name = "textBox2";
            textBox2.ReadOnly = true;
            textBox2.Size = new Size(187, 25);
            textBox2.TabIndex = 41;
            textBox2.Text = "0";
            // 
            // label16
            // 
            label16.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label16.AutoSize = true;
            label16.Location = new Point(327, 556);
            label16.Name = "label16";
            label16.Size = new Size(97, 17);
            label16.TabIndex = 40;
            label16.Text = "Potong Piutang";
            // 
            // supplierIdTextBox
            // 
            supplierIdTextBox.Location = new Point(107, 85);
            supplierIdTextBox.Name = "supplierIdTextBox";
            supplierIdTextBox.ReadOnly = true;
            supplierIdTextBox.Size = new Size(50, 25);
            supplierIdTextBox.TabIndex = 42;
            // 
            // label17
            // 
            label17.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            label17.Font = new Font("Segoe UI", 15.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label17.ForeColor = Color.FromArgb(0, 0, 192);
            label17.Location = new Point(937, 59);
            label17.Name = "label17";
            label17.Size = new Size(302, 40);
            label17.TabIndex = 43;
            label17.Text = "Total Belanja";
            label17.TextAlign = ContentAlignment.MiddleRight;
            // 
            // PurchaseControl
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(250, 250, 250);
            Controls.Add(grandTotalLabel);
            Controls.Add(label17);
            Controls.Add(supplierIdTextBox);
            Controls.Add(textBox2);
            Controls.Add(label16);
            Controls.Add(grandTotalTextBox);
            Controls.Add(label15);
            Controls.Add(button9);
            Controls.Add(button8);
            Controls.Add(button7);
            Controls.Add(textBox4);
            Controls.Add(label5);
            Controls.Add(button5);
            Controls.Add(refundTextBox);
            Controls.Add(label14);
            Controls.Add(button4);
            Controls.Add(button3);
            Controls.Add(label13);
            Controls.Add(paidAmountTextBox);
            Controls.Add(label12);
            Controls.Add(comboBox2);
            Controls.Add(textBox7);
            Controls.Add(label11);
            Controls.Add(costTextBox);
            Controls.Add(label10);
            Controls.Add(discountTextBox);
            Controls.Add(label8);
            Controls.Add(subTotalTextBox);
            Controls.Add(label7);
            Controls.Add(button2);
            Controls.Add(barcodeTextBox);
            Controls.Add(label4);
            Controls.Add(grid);
            Controls.Add(button1);
            Controls.Add(supplierNameTextBox);
            Controls.Add(label3);
            Controls.Add(dateTimePicker1);
            Controls.Add(label2);
            Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Name = "PurchaseControl";
            Size = new Size(1252, 700);
            Load += BaseFormLoad;
            ((System.ComponentModel.ISupportInitialize)grid).EndInit();
            ((System.ComponentModel.ISupportInitialize)purchaseItemBindingSource).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private DataGridViewTextBoxColumn productIdDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn productNameDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn productUnitDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn cOGSDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn nettProfitDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn skuDataGridViewTextBoxColumn;
        private BindingSource purchaseItemBindingSource;
        private Label label2;
        private DateTimePicker dateTimePicker1;
        private Label label3;
        private TextBox supplierNameTextBox;
        private Label label4;
        private TextBox barcodeTextBox;
        private Label grandTotalLabel;
        private Button button2;
        private TextBox subTotalTextBox;
        private Label label7;
        private TextBox discountTextBox;
        private Label label8;
        private TextBox costTextBox;
        private Label label10;
        private TextBox textBox7;
        private Label label11;
        private Label label12;
        private ComboBox comboBox2;
        private TextBox paidAmountTextBox;
        private Label label13;
        private Button button3;
        private Button button4;
        private TextBox refundTextBox;
        private Label label14;
        private Button button5;
        private TextBox textBox4;
        private Label label5;
        private Button button7;
        private Button button8;
        private Button button1;
        private Button button9;
        private TextBox grandTotalTextBox;
        private Label label15;
        private TextBox textBox2;
        private Label label16;
        private TextBox supplierIdTextBox;
        private Astro.Winform.Controls.DGrid grid;
        private Label label17;
        private DataGridViewTextBoxColumn idDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn nameDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private DataGridViewTextBoxColumn quantityDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn unitDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn priceDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn discountDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn nettPriceDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn totalDataGridViewTextBoxColumn;
        private DataGridViewButtonColumn Delete;
    }
}
