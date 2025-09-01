using Astro.Drawing.Extensions;
using Astro.Winform.Classes;
using Astro.Winform.Controls;
using Astro.Winform.Forms;
using System.Collections.ObjectModel;
using System.Data;
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
            public Rectangle Bounds => new Rectangle(this.Location, this.Size);
            public int X => this.Location.X;
            public int Y => this.Location.Y;
            public int Width => this.Size.Width;
            public int Height => this.Size.Height;
            public Action? OnClick = null;
            public Color BackColor { get; set; } = Color.Blue;
            public Color HoveredBackColor { get; set; } = Color.DarkBlue;
            public Font Font { get; set; } = new Font("Segoe UI", 9.75F, FontStyle.Regular);
            public bool Hovered { get; private set; }
            public bool MouseHit { get; set; }
            public void OnMouseMove(Point location)
            {
                Hovered = new Rectangle(this.Location, this.Size).Contains(location);
            }
            public void ResetHover() => Hovered = false;
            public void Draw(Graphics g)
            {
                using (var brush = new SolidBrush(this.MouseHit ? Color.SteelBlue : this.Hovered ? this.HoveredBackColor : this.BackColor))
                {
                    var sf = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    g.DrawRoundedRectangle(new Rectangle(this.Location, this.Size), 5, brush);
                    TextRenderer.DrawText(
                        g,
                        this.Text,
                        this.Font,
                        new Rectangle(this.Location, this.Size),
                        Color.White,
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis
                    );
                }
            }
        }
        public class ButtonCollection : Collection<Button>
        {
            public Button Add(string text)
            {
                var button = new Button()
                {
                    Text = text,
                    Location = this.Count > 0 ? new Point(this[this.Count - 1].X + this[this.Count - 1].Width + 5, 0) : new Point(10, 0)
                };
                var w = TextRenderer.MeasureText(button.Text, button.Font).Width + 16;
                button.Size = new Size(w, 30);
                base.Add(button);
                return button;
            }
            public void Draw (Graphics g)
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                foreach (var button in this)
                {
                    button.Draw(g);
                }
            }
        }
        #endregion
        private BindingSource BindingSource { get; } = new BindingSource();
        private ButtonCollection buttons = new ButtonCollection();
        private Point _mouseHitPoint;
        private bool _mouseHit = false;
        public ListingControl(SideBarPanel.Menu _menu)
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.AllPaintingInWmPaint
                | ControlStyles.UserPaint
                | ControlStyles.ResizeRedraw
                | ControlStyles.OptimizedDoubleBuffer, true);
            this.UpdateStyles();
            var refreshButton = this.buttons.Add("\u21BA Refresh");
            refreshButton.OnClick = async () =>
            {
                await LoadDataAsync();
            };
            this.Menu = _menu;
            if (this.Menu.AllowAdd)
            {
                var add = this.buttons.Add("\u2795 Baru");
                add.OnClick = async () =>
                {
                    await this.OpenObjectDetail();
                };
            }
            if (this.Menu.AllowDelete)
            {
                var del = this.buttons.Add("\u274C Hapus");
                del.OnClick = async () =>
                {
                    await DeleteRecordAsync();
                };
            }
            switch (this.Menu.Type)
            {
                case ListingData.Users:
                    GridHelpers.InitializeDataGridColumns(this.dataGridView1, new DataTableColumnInfo[]
                    {
                        new DataTableColumnInfo("Nama Pegawai", "fullname", 320, DataGridViewContentAlignment.MiddleLeft, ""),
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
                        new DataTableColumnInfo("Tanggal Dibuat", "createddate", 120, DataGridViewContentAlignment.MiddleRight, "dd/MM/yyyy HH:mm")
                    }, this.BindingSource);
                    this.dataGridView1.Columns[0].DefaultCellStyle.BackColor = Color.FromArgb(244,244,244);

                    var manageButton = this.buttons.Add("Manage Kategori");
                    manageButton.OnClick = () =>
                    {
                        var form = new ListCategoryForm();
                        form.ShowDialog();
                    };
                    var unitButton = this.buttons.Add("Manage Satuan");
                    unitButton.OnClick = () =>
                    {
                        var form = new ListUnitForm();
                        form.ShowDialog();
                    };
                    break;
                case ListingData.Suppliers:
                case ListingData.Customers:
                    var headerText = this.Menu.Type == ListingData.Suppliers ? "Nama Supplier" : "Nama Pelanggan";
                    GridHelpers.InitializeDataGridColumns(this.dataGridView1, new DataTableColumnInfo[]
                    {
                        new DataTableColumnInfo("Kode", "contactid", 70, DataGridViewContentAlignment.MiddleCenter, "00000"),
                        new DataTableColumnInfo(headerText, "name", 300),
                        new DataTableColumnInfo("Alamat", "address", 300),
                        new DataTableColumnInfo("Telepon", "phonenumber", 120),
                        new DataTableColumnInfo("Dibuat Oleh", "creator", 180),
                        new DataTableColumnInfo("Dibuat Tanggal", "createdDate", 120, DataGridViewContentAlignment.MiddleRight, "dd-MM-yyyy HH:mm")
                    }, this.BindingSource);
                    this.dataGridView1.Columns[0].DefaultCellStyle.BackColor = Color.FromArgb(244, 244, 244);
                    break;
                case ListingData.Employee:
                    GridHelpers.InitializeDataGridColumns(this.dataGridView1, new DataTableColumnInfo[]
                    {
                        //new DataTableColumnInfo("ID", "id", 60, DataGridViewContentAlignment.MiddleCenter, "00000"),
                        new DataTableColumnInfo("Nama Pegawai", "fullname", 300),
                        new DataTableColumnInfo("Alamat", "address", 300),
                        new DataTableColumnInfo("Telepon", "phone", 120),
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
                        new DataTableColumnInfo("Provider Name", "name", 180),
                        new DataTableColumnInfo("Created By", "fullname", 200),
                        new DataTableColumnInfo("Created Date", "createddate", 120, DataGridViewContentAlignment.MiddleRight, "dd-MM-yyyy HH:mm"),
                        new DataTableColumnInfo("Last Modified", "aditeddate", 120, DataGridViewContentAlignment.MiddleRight, "dd-MM-yyyy HH:mm")
                    }, this.BindingSource);
                    var accountButton = this.buttons.Add("Account Provider");
                    accountButton.OnClick = () =>
                    {
                        var form = new ListAccountProviderForm();
                        form.ShowDialog();
                    };
                    break;
            }
        }
        public SideBarPanel.Menu Menu { get; }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            foreach (var button in buttons)
            {
                button.OnMouseMove(e.Location);
            }
            base.OnMouseMove(e);
            Invalidate();
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            foreach (var button in this.buttons)
            {
                if (button.MouseHit && button.Bounds.Contains(e.Location))
                {
                    if (button.OnClick != null) button.OnClick.Invoke();
                    button.MouseHit = false;
                }
            }
            base.OnMouseUp(e);
            Invalidate();
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            foreach (var button in this.buttons)
            {
                button.MouseHit = button.Bounds.Contains(e.Location);
            }
            base.OnMouseDown(e);
            Invalidate();
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            foreach (var button in buttons)
            {
                button.ResetHover();
            }
            base.OnMouseLeave(e);
            Invalidate();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            buttons.Draw(e.Graphics);
        }
        internal async Task LoadDataAsync()
        {
            using (var stream = await WClient.PostStreamAsync(this.Menu.URL, new byte[] { 0x00, 0x00 }))
            using (var reader = new Streams.Reader(stream))
            {
                if (stream.Length == 0) return;

                var result = reader.ReadByte();
                if (result == 0x00)
                {
                    this.BindingSource.DataSource = null;
                    return;
                }
                this.BindingSource.DataSource = reader.ReadDataTable();
            }
        }
        private async void ListingControl_Load(object sender, EventArgs e)
        {
            await LoadDataAsync();
        }
        private async Task OpenObjectDetail(object? id = null)
        {
            Control? control = null;
            switch (this.Menu.Type)
            {
                case ListingData.Roles:
                    control = new RoleForm();
                    break;
                case ListingData.Products:
                    control = new ProductForm();
                    break;
                case ListingData.Customers:
                case ListingData.Suppliers:
                    control = new ContactForm()
                    {
                        ContactType = this.Menu.Type == ListingData.Suppliers ? (short)0 : (short)1,
                        Text = this.Menu.Type == ListingData.Suppliers ? "Supplier" : "Customer"
                    };
                    break;
                case ListingData.Employee:
                    control = new EmployeeForm();
                    break;
                case ListingData.Accounts:
                    control = new AccountForm();
                    break;
            }
            if (control != null)
            {
                var mainForm = this.FindForm();
                if (mainForm is null) return;

                using (OverlayForm form = new OverlayForm())
                {
                    control.Tag = id;
                    control.Dock = DockStyle.Right;

                    form.StartPosition = FormStartPosition.Manual;
                    form.Size = mainForm.Size;
                    form.Location = mainForm.Location;
                    form.Padding = new Padding(1, 1, 1, 1);
                    form.Controls.Add(control);
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        await LoadDataAsync();
                    }
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
                    await this.LoadDataAsync();
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
