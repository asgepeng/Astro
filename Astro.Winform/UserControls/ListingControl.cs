using Astro.DataTables;
using Astro.Drawing.Extensions;
using Astro.Winform.Classes;
using Astro.Winform.Controls;
using Astro.Winform.Forms;
using Astro.Winform.Tables;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using WinformApp.Data;

namespace Astro.Forms.Controls
{

    public partial class ListingControl : UserControl
    {
        #region Button
        public class Button
        {
            public Size Size { get; set; } = new Size(80, 30);
            public string Text { get; set; } = "";
            public Point Location { get; set; } = new Point(10, 0);
            public Rectangle Bounds
            {
                get
                {
                    return new Rectangle(this.Location, this.Size);
                }
            }
            public Color BackColor { get; set; } = Color.Navy;
            public Color HoveredBackColor { get; set; } = Color.Blue;
            public Font Font { get; set; } = new Font("Segoe UI", 9.75F, FontStyle.Regular);
            public bool Hovered { get; private set; }
            public bool MouseHit { get; set; }
            public void OnMouseMove(Point location)
            {
                Hovered = new Rectangle(this.Location, this.Size).Contains(location);
            }
            public void Draw(Graphics g)
            {
                using (var brush = new SolidBrush(this.MouseHit ? Color.Red : this.Hovered ? this.HoveredBackColor : this.BackColor))
                {
                    var sf = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    g.DrawRoundedRectangle(new Rectangle(this.Location, this.Size), 10, brush);
                    g.DrawString(this.Text, this.Font, Brushes.White, new Rectangle(this.Location, this.Size), sf);
                }
            }
        }
        #endregion
        private BindingSource BindingSource { get; } = new BindingSource();
        private ListingControl.Button? addButton = null, deleteButton = null;
        private Point _mouseHitPoint;
        private bool _mouseHit = false;
        public ListingControl(SideBarPanel.Menu _menu)
        {
            InitializeComponent();
            this.Menu = _menu;
            if (this.Menu.AllowAdd)
            {
                this.addButton = new Button();
                this.addButton.Text = "+ Baru";
            }
            if (this.Menu.AllowDelete)
            {
                this.deleteButton = new Button();
                this.deleteButton.Text = "Hapus Terpilih";
                this.deleteButton.Size = new Size(150, 30);
                if (this.addButton != null)
                {
                    this.deleteButton.Location = new Point(this.addButton.Location.X + this.addButton.Size.Width + 5, this.addButton.Location.Y);
                }
            }
            this.SetStyle(ControlStyles.AllPaintingInWmPaint
                | ControlStyles.UserPaint
                | ControlStyles.ResizeRedraw
                | ControlStyles.OptimizedDoubleBuffer, true);
            this.UpdateStyles();
            switch (this.Menu.Type)
            {
                case ListingData.Users:
                    GridHelpers.InitializeDataGridColumns(this.dataGridView1, new DataTableColumnInfo[]
                    {
                        new DataTableColumnInfo("Name", "fullname", 320, DataGridViewContentAlignment.MiddleLeft, ""),
                        new DataTableColumnInfo("Email Address", "email", 250, DataGridViewContentAlignment.MiddleLeft, ""),
                        new DataTableColumnInfo("Role", "role_name", 120),
                        new DataTableColumnInfo("Created By", "creator", 150),
                        new DataTableColumnInfo("Created Date", "createddate", 120, DataGridViewContentAlignment.MiddleRight, "dd/MM/yyyy HH:mm")
                    }, this.BindingSource);
                    break;
                case ListingData.Roles:
                    GridHelpers.InitializeDataGridColumns(this.dataGridView1, new DataTableColumnInfo[]
                    {
                        new DataTableColumnInfo("Role Name", "name", 300),
                        new DataTableColumnInfo("Created By", "creator", 200),
                        new DataTableColumnInfo("Created Date", "createddate", 120, DataGridViewContentAlignment.MiddleRight, "dd/MM/yyyy HH:mm")
                    }, this.BindingSource);
                    break;
                case ListingData.Products:
                    GridHelpers.InitializeDataGridColumns(this.dataGridView1, new DataTableColumnInfo[]
                    {
                        new DataTableColumnInfo("Kode", "productid", 70, DataGridViewContentAlignment.MiddleCenter, "00000"),
                        new DataTableColumnInfo("Nama Produk", "name", 300),
                        new DataTableColumnInfo("SKU", "sku", 100),
                        new DataTableColumnInfo("Kategori", "categoryname", 100),
                        new DataTableColumnInfo("Stok", "stock", 80, DataGridViewContentAlignment.MiddleRight, "N0"),
                        new DataTableColumnInfo("Satuan", "unitname", 80),
                        new DataTableColumnInfo("Harga", "price", 120, DataGridViewContentAlignment.MiddleRight, "N0"),
                        new DataTableColumnInfo("Dibuat Oleh", "fullname", 200),
                        new DataTableColumnInfo("Dibuat Tggl", "createddate", 120, DataGridViewContentAlignment.MiddleRight, "dd/MM/yyyy HH:mm")
                    }, this.BindingSource);
                    this.dataGridView1.Columns[0].DefaultCellStyle.BackColor = Color.FromArgb(244,244,244);
                    break;
                case ListingData.Suppliers:
                case ListingData.Customers:
                    var headerText = this.Menu.Type == ListingData.Suppliers ? "Supplier Name" : "Customer Name";
                    GridHelpers.InitializeDataGridColumns(this.dataGridView1, new DataTableColumnInfo[]
                    {
                        new DataTableColumnInfo("Kode", "contactid", 70, DataGridViewContentAlignment.MiddleCenter, "00000"),
                        new DataTableColumnInfo(headerText, "name", 300),
                        new DataTableColumnInfo("Alamat", "address", 300),
                        new DataTableColumnInfo("Telepon", "phonenumber", 120),
                        new DataTableColumnInfo("Created By", "creator", 180),
                        new DataTableColumnInfo("Created At", "createdDate", 120, DataGridViewContentAlignment.MiddleRight, "dd-MM-yyyy HH:mm")
                    }, this.BindingSource);
                    this.dataGridView1.Columns[0].DefaultCellStyle.BackColor = Color.FromArgb(244, 244, 244);
                    break;
                case ListingData.Employee:
                    GridHelpers.InitializeDataGridColumns(this.dataGridView1, new DataTableColumnInfo[]
                    {
                        //new DataTableColumnInfo("ID", "id", 60, DataGridViewContentAlignment.MiddleCenter, "00000"),
                        new DataTableColumnInfo("Nama Pegawai", "fullname", 300),
                        new DataTableColumnInfo("Alamat", "address", 300),
                        new DataTableColumnInfo("Phone Number", "phone", 120),
                        new DataTableColumnInfo("Jabatan", "rolename", 200),
                        new DataTableColumnInfo("Created By", "creator", 180),
                        new DataTableColumnInfo("Created At", "createdDate", 120, DataGridViewContentAlignment.MiddleRight, "dd-MM-yyyy HH:mm")
                    }, this.BindingSource);
                    break;
                case ListingData.Accounts:
                    GridHelpers.InitializeDataGridColumns(this.dataGridView1, new DataTableColumnInfo[]
                    {
                        new DataTableColumnInfo("Account Name", "accountName", 200),
                        new DataTableColumnInfo("Account Number", "accountNumber", 200),
                        new DataTableColumnInfo("Account Type", "accountType", 120),
                        new DataTableColumnInfo("Provider Name", "providerName", 180),
                        new DataTableColumnInfo("Created By", "createdBy", 200),
                        new DataTableColumnInfo("Created Date", "createdDate", 120, DataGridViewContentAlignment.MiddleRight, "dd-MM-yyyy HH:mm"),
                        new DataTableColumnInfo("Last Modified", "lastModified", 120, DataGridViewContentAlignment.MiddleRight, "dd-MM-yyyy HH:mm")
                    }, this.BindingSource);
                    break;
            }
        }
        public SideBarPanel.Menu Menu { get; }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (this.Menu.AllowAdd && this.addButton != null)
            {
                this.addButton.OnMouseMove(e.Location);
            }
            if (this.Menu.AllowDelete && this.deleteButton != null)
            {
                this.deleteButton.OnMouseMove(e.Location);
            }
            this.Invalidate();
        }
        protected async override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (this.addButton != null && this.addButton.MouseHit && this.addButton.Bounds.Contains(e.Location))
            {
                await OpenObjectDetail();
            }
            if (this.deleteButton != null && this.deleteButton.MouseHit && this.deleteButton.Bounds.Contains(e.Location))
            {
                await DeleteRecordAsync();
            }
            if (this.addButton != null) this.addButton.MouseHit = false;
            if (this.deleteButton != null) this.deleteButton.MouseHit = false;
            Invalidate();
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (this.addButton != null && this.addButton.Bounds.Contains(e.Location))
            {
                this.addButton.MouseHit = true;
                this.Invalidate();
                return;
            }
            if (this.deleteButton != null && this.deleteButton.Bounds.Contains(e.Location))
            {
                this.deleteButton.MouseHit = true;
                this.Invalidate();
            }
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (this.addButton != null) this.addButton.OnMouseMove(new Point(-1, -1));
            if (this.deleteButton != null) this.deleteButton.OnMouseMove(new Point(-1, -1));
            Invalidate();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            var sf = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            if (Menu.AllowAdd && this.addButton != null)
            {
                this.addButton.Draw(e.Graphics);
            }
            if (Menu.AllowDelete && this.deleteButton != null)
            {
                this.deleteButton.Draw(e.Graphics);
            }
        }
        internal async Task ReloadDataTable()
        {
            using (var stream = await WClient.GetStreamAsync(this.Menu.URL))
            using (var reader = new Streams.Reader(stream))
            {
                var result = reader.ReadByte();
                if (result == 0x00)
                {
                    this.BindingSource.DataSource = null;
                    return;
                }
                if (stream.Length > 0)
                {
                    this.BindingSource.DataSource = reader.ReadDataTable();
                }
            }
        }
        private async void ListingControl_Load(object sender, EventArgs e)
        {
            await ReloadDataTable();
        }
        private async Task OpenObjectDetail(object? id = null)
        {
            Form? form = null;
            switch (this.Menu.Type)
            {
                case ListingData.Roles:
                    form = new RoleForm();
                    break;
                case ListingData.Products:
                    form = new ProductForm();
                    break;
                case ListingData.Customers:
                case ListingData.Suppliers:
                    form = new ContactForm();
                    form.Text = this.Menu.Type == ListingData.Customers ? "Supplier" : "Customer";
                    break;
                case ListingData.Employee:
                    form = new EmployeeForm();
                    break;
                case ListingData.Accounts:
                    form = new AccountForm();
                    break;
            }
            if (form != null)
            {
                form.Tag = id;
                if (form.ShowDialog() == DialogResult.OK)
                {
                    await this.ReloadDataTable();
                }
            }
        }
        private async Task DeleteRecordAsync()
        {
            if (this.BindingSource.Current != null)
            {
                if (MessageBox.Show("Anda yakin akan menghapus record ini secara permanen dari database?", "Konfirmasi Hapus", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    var row = (DataRowView)this.BindingSource.Current;
                    var id = row[0]?.ToString();
                    var result = await WClient.DeleteAsync(this.Menu.URL + "/" + id);
                    await this.ReloadDataTable();
                }
            }
        }
        private async void dataGridView1_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (this.BindingSource.Current is null) return;

            if (this.Menu.AllowEdit)
            {
                var id = ((DataRowView)this.BindingSource.Current)[0];
                await OpenObjectDetail(id);
            }
        }
    }
}
