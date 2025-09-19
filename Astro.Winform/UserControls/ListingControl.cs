using Astro.Drawing.Extensions;
using Astro.Winform.Classes;
using Astro.Winform.Controls;
using Astro.Winform.Extensions;
using Astro.Winform.Forms;
using Astro.Models;
using System.Collections.ObjectModel;
using System.Data;
using WinformApp.Data;
using Astro.Extensions;
using Astro.Winform.UserControls;
using Astro.Data;
using System.Data.Common;

namespace Astro.Forms.Controls
{
    public partial class ListingControl : UserControl
    {
        #region Button
        public class Button
        {
            public Size Size { get; set; } = new Size(40, 40);
            public string Text { get; set; } = "";
            public Point Location { get; set; } = new Point(10, 0);
            public Image? Image { get; set; }
            public Rectangle Bounds => new Rectangle(this.Location, this.Size);
            public int X => this.Location.X;
            public int Y => this.Location.Y;
            public int Width => this.Size.Width;
            public int Height => this.Size.Height;
            public Action? OnClick = null;
            public Color BackColor { get; set; } = Color.Blue;
            public Color HoveredBackColor { get; set; } = Color.DarkBlue;
            public Font Font { get; set; } = new Font("Segoe UI", 9.75F, FontStyle.Regular);
            public bool Hovered { get; internal set; }
            public bool MouseHit { get; set; }
            public void OnMouseMove(Point location)
            {
                Hovered = new Rectangle(this.Location, this.Size).Contains(location);
            }
            public void Draw(Graphics g)
            {
                if (Image != null)
                {
                    if (this.Hovered || this.MouseHit)
                    {
                        g.DrawRoundedRectangle(new Rectangle(this.Location, this.Size), 5, this.MouseHit ? Brushes.Blue :Brushes.Lavender);
                    }
                    g.DrawImage(this.Image, this.Bounds.Left + 2, this.Bounds.Top + 2, this.Bounds.Height - 4, this.Bounds.Height - 4);
                }
                else
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
        }
        public class ButtonCollection : Collection<Button>
        {
            public Button Add(string text)
            {
                var button = new Button()
                {
                    Text = text,
                    Location = this.Count > 0 ? new Point(this[this.Count - 1].X + this[this.Count - 1].Width + 5, 10) : new Point(10, 10)
                };
                var w = TextRenderer.MeasureText(button.Text, button.Font).Width + 16;
                button.Size = new Size(w, 32);
                base.Add(button);
                return button;
            }
            public Button Add(string text, Image image)
            {
                var button = new Button()
                {
                    Text = text,
                    Location = this.Count > 0 ? new Point(this[this.Count - 1].X + this[this.Count - 1].Width + 2, 10) : new Point(10, 10),
                    Image = image
                };
                button.Size = new Size(32, 32);
                base.Add(button);
                return button;
            }
            public void Draw(Graphics g)
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                foreach (var button in this)
                {
                    button.Draw(g);
                }
            }
            public bool AnyHoveredItem { get; private set; }
            public Button? GetHoveredButton(Point loc)
            {
                Button? button = null;
                foreach (var item in this)
                {
                    if (item.Bounds.Contains(loc))
                    {
                        if (!item.Hovered)
                        {
                            item.Hovered = true;
                            button = item;
                        }
                    }
                    else
                    {
                        item.Hovered = false;
                    }
                    if (this.AnyHoveredItem == false && item.Hovered) this.AnyHoveredItem = true;
                }
                return button;
            }
            public void ResetHover()
            {
                foreach (var item in this)
                {
                    item.Hovered = false;
                }
                AnyHoveredItem = false;
            }
        }
        #endregion
        private BindingSource BindingSource { get; } = new BindingSource();
        private ButtonCollection buttons = new ButtonCollection();
        private Point _mouseHitPoint;
        private List<string> filters = new List<string>();
        private bool _useDateRange = false;
        private DateRange _dateRange = new DateRange();
        private readonly IDBClient db;
        public ListingControl(SideBarPanel.Menu _menu)
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.AllPaintingInWmPaint
                | ControlStyles.UserPaint
                | ControlStyles.ResizeRedraw
                | ControlStyles.OptimizedDoubleBuffer, true);
            this.UpdateStyles();
            this.db = My.Application.CreateDBAccess();
            var refreshButton = this.buttons.Add("Refresh All", """
                <svg viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg"><g id="SVGRepo_bgCarrier" stroke-width="0"></g><g id="SVGRepo_tracerCarrier" stroke-linecap="round" stroke-linejoin="round"></g><g id="SVGRepo_iconCarrier"> <path d="M7.37756 11.6296H6.62756H7.37756ZM7.37756 12.5556L6.81609 13.0528C6.95137 13.2056 7.14306 13.2966 7.34695 13.3049C7.55084 13.3133 7.74932 13.2382 7.89662 13.0969L7.37756 12.5556ZM9.51905 11.5414C9.81805 11.2547 9.82804 10.7799 9.54137 10.4809C9.2547 10.182 8.77994 10.172 8.48095 10.4586L9.51905 11.5414ZM6.56148 10.5028C6.28686 10.1927 5.81286 10.1639 5.50277 10.4385C5.19267 10.7131 5.16391 11.1871 5.43852 11.4972L6.56148 10.5028ZM14.9317 9.0093C15.213 9.31337 15.6875 9.33184 15.9915 9.05055C16.2956 8.76927 16.3141 8.29476 16.0328 7.9907L14.9317 9.0093ZM12.0437 6.25C9.05802 6.25 6.62756 8.653 6.62756 11.6296H8.12756C8.12756 9.49251 9.87531 7.75 12.0437 7.75V6.25ZM6.62756 11.6296L6.62756 12.5556H8.12756L8.12756 11.6296H6.62756ZM7.89662 13.0969L9.51905 11.5414L8.48095 10.4586L6.85851 12.0142L7.89662 13.0969ZM7.93904 12.0583L6.56148 10.5028L5.43852 11.4972L6.81609 13.0528L7.93904 12.0583ZM16.0328 7.9907C15.0431 6.9209 13.6212 6.25 12.0437 6.25V7.75C13.1879 7.75 14.2154 8.23504 14.9317 9.0093L16.0328 7.9907Z" fill="#1C274C"></path> <path d="M16.6188 11.4443L17.1795 10.9462C17.044 10.7937 16.8523 10.703 16.6485 10.6949C16.4447 10.6868 16.2464 10.7621 16.0993 10.9034L16.6188 11.4443ZM14.4805 12.4581C14.1817 12.745 14.1722 13.2198 14.4591 13.5185C14.746 13.8173 15.2208 13.8269 15.5195 13.54L14.4805 12.4581ZM17.4393 13.4972C17.7144 13.8068 18.1885 13.8348 18.4981 13.5597C18.8078 13.2846 18.8358 12.8106 18.5607 12.5009L17.4393 13.4972ZM9.04688 15.0047C8.76342 14.7027 8.28879 14.6876 7.98675 14.9711C7.68472 15.2545 7.66966 15.7292 7.95312 16.0312L9.04688 15.0047ZM11.9348 17.7499C14.9276 17.7499 17.3688 15.3496 17.3688 12.3703H15.8688C15.8688 14.5047 14.1158 16.2499 11.9348 16.2499V17.7499ZM17.3688 12.3703V11.4443H15.8688V12.3703H17.3688ZM16.0993 10.9034L14.4805 12.4581L15.5195 13.54L17.1383 11.9853L16.0993 10.9034ZM16.0581 11.9425L17.4393 13.4972L18.5607 12.5009L17.1795 10.9462L16.0581 11.9425ZM7.95312 16.0312C8.94543 17.0885 10.3635 17.7499 11.9348 17.7499V16.2499C10.792 16.2499 9.76546 15.7704 9.04688 15.0047L7.95312 16.0312Z" fill="#1C274C"></path> <circle opacity="0.5" cx="12" cy="12" r="10" stroke="#1C274C" stroke-width="1.5"></circle> </g></svg>
                """.ToImage(32, 32));
            refreshButton.OnClick = async () =>
            {
                await LoadDataAsync();
            };

            this.Menu = _menu;
            if (this.Menu.AllowAdd)
            {
                var add = this.buttons.Add("Tambah Record Baru", """
                    <svg viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg"><g id="SVGRepo_bgCarrier" stroke-width="0"></g><g id="SVGRepo_tracerCarrier" stroke-linecap="round" stroke-linejoin="round"></g><g id="SVGRepo_iconCarrier"> <circle opacity="0.5" cx="12" cy="12" r="10" stroke="#1C274C" stroke-width="1.5"></circle> <path d="M15 12L12 12M12 12L9 12M12 12L12 9M12 12L12 15" stroke="#1C274C" stroke-width="1.5" stroke-linecap="round"></path> </g></svg>
                    """.ToImage(32, 32));
                add.OnClick = async () =>
                {
                    await this.OpenObjectDetail();
                };
            }
            if (this.Menu.AllowDelete)
            {
                var del = this.buttons.Add("Hapus Record", """
                    <svg viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg"><g id="SVGRepo_bgCarrier" stroke-width="0"></g><g id="SVGRepo_tracerCarrier" stroke-linecap="round" stroke-linejoin="round"></g><g id="SVGRepo_iconCarrier"> <path opacity="0.5" d="M16 4C18.175 4.01211 19.3529 4.10856 20.1213 4.87694C21 5.75562 21 7.16983 21 9.99826V15.9983C21 18.8267 21 20.2409 20.1213 21.1196C19.2426 21.9983 17.8284 21.9983 15 21.9983H9C6.17157 21.9983 4.75736 21.9983 3.87868 21.1196C3 20.2409 3 18.8267 3 15.9983V9.99826C3 7.16983 3 5.75562 3.87868 4.87694C4.64706 4.10856 5.82497 4.01211 8 4" stroke="#1C274C" stroke-width="1.5"></path> <path d="M8 3.5C8 2.67157 8.67157 2 9.5 2H14.5C15.3284 2 16 2.67157 16 3.5V4.5C16 5.32843 15.3284 6 14.5 6H9.5C8.67157 6 8 5.32843 8 4.5V3.5Z" stroke="#1C274C" stroke-width="1.5"></path> <path d="M14.5 11L9.50004 16M9.50002 11L14.5 16" stroke="#1C274C" stroke-width="1.5" stroke-linecap="round"></path> </g></svg>
                    """.ToImage(32, 32));
                del.OnClick = async () =>
                {
                    await DeleteRecordAsync();
                };
            }
            this.buttons.Add("Export Excel", """
                <svg viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg"><g id="SVGRepo_bgCarrier" stroke-width="0"></g><g id="SVGRepo_tracerCarrier" stroke-linecap="round" stroke-linejoin="round"></g><g id="SVGRepo_iconCarrier"> <path opacity="0.5" d="M17 9.00195C19.175 9.01406 20.3529 9.11051 21.1213 9.8789C22 10.7576 22 12.1718 22 15.0002V16.0002C22 18.8286 22 20.2429 21.1213 21.1215C20.2426 22.0002 18.8284 22.0002 16 22.0002H8C5.17157 22.0002 3.75736 22.0002 2.87868 21.1215C2 20.2429 2 18.8286 2 16.0002L2 15.0002C2 12.1718 2 10.7576 2.87868 9.87889C3.64706 9.11051 4.82497 9.01406 7 9.00195" stroke="#1C274C" stroke-width="1.5" stroke-linecap="round"></path> <path d="M12 2L12 15M12 15L9 11.5M12 15L15 11.5" stroke="#1C274C" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"></path> </g></svg>
                """.ToImage(32, 32));
            switch (this.Menu.Type)
            {
                case ListingData.Users:
                    GridHelpers.InitializeDataGridColumns(this.grid, new DataTableColumnInfo[]
                    {
                        new DataTableColumnInfo("Nama Pegawai", "fullname", 500, DataGridViewContentAlignment.MiddleLeft, ""),
                        new DataTableColumnInfo("Created By", "creator", 150),
                        new DataTableColumnInfo("Created Date", "createddate", 120, DataGridViewContentAlignment.MiddleRight, "dd/MM/yyyy HH:mm")
                    }, this.BindingSource);
                    break;
                case ListingData.Roles:
                    GridHelpers.InitializeDataGridColumns(this.grid, new DataTableColumnInfo[]
                    {
                        new DataTableColumnInfo("Role Name", "name", 300),
                        new DataTableColumnInfo("Created By", "creator", 200),
                        new DataTableColumnInfo("Created Date", "createddate", 120, DataGridViewContentAlignment.MiddleRight, "dd/MM/yyyy HH:mm")
                    }, this.BindingSource);
                    break;
                case ListingData.Products:
                    GridHelpers.InitializeDataGridColumns(this.grid, new DataTableColumnInfo[]
                    {
                        //new DataTableColumnInfo("Kode", "productid", 70, DataGridViewContentAlignment.MiddleCenter, "00000"),
                        new DataTableColumnInfo("Nama Produk", "name", 300),
                        new DataTableColumnInfo("SKU", "sku", 100),
                        new DataTableColumnInfo("Kategori", "categoryname", 100),
                        new DataTableColumnInfo("Stok", "stock", 80, DataGridViewContentAlignment.MiddleRight, "N0"),
                        new DataTableColumnInfo("Satuan", "unitname", 80),
                        new DataTableColumnInfo("Harga", "price", 120, DataGridViewContentAlignment.MiddleRight, "N0"),
                        new DataTableColumnInfo("Dibuat Oleh", "fullname", 200),
                        new DataTableColumnInfo("Tanggal Dibuat", "createddate", 120, DataGridViewContentAlignment.MiddleRight, "dd/MM/yyyy HH:mm")
                    }, this.BindingSource);
                    //this.dataGridView1.Columns[0].DefaultCellStyle.BackColor = Color.FromArgb(244,244,244);

                    var manageButton = this.buttons.Add("Kategori Produk");
                    manageButton.OnClick = async () =>
                    {
                        await OpenChildWindow(new ListCategoryForm(), DockStyle.None);
                    };
                    var unitButton = this.buttons.Add("Satuan");
                    unitButton.OnClick = async () =>
                    {
                        await OpenChildWindow(new ListUnitForm());
                    };
                    break;
                case ListingData.Suppliers:
                case ListingData.Customers:
                    var headerText = this.Menu.Type == ListingData.Suppliers ? "Nama Supplier" : "Nama Pelanggan";
                    GridHelpers.InitializeDataGridColumns(this.grid, new DataTableColumnInfo[]
                    {
                        new DataTableColumnInfo(headerText, "name", 300),
                        new DataTableColumnInfo("Alamat", "address", 300),
                        new DataTableColumnInfo("Telepon", "phonenumber", 120),
                        new DataTableColumnInfo("Dibuat Oleh", "fullname", 180),
                        new DataTableColumnInfo("Dibuat Tanggal", "createdDate", 120, DataGridViewContentAlignment.MiddleRight, "dd-MM-yyyy HH:mm")
                    }, this.BindingSource);
                    break;
                case ListingData.Employee:
                    GridHelpers.InitializeDataGridColumns(this.grid, new DataTableColumnInfo[]
                    {
                        //new DataTableColumnInfo("ID", "id", 60, DataGridViewContentAlignment.MiddleCenter, "00000"),
                        new DataTableColumnInfo("Nama Pegawai", "fullname", 300),
                        new DataTableColumnInfo("Alamat", "streetaddress", 300),
                        new DataTableColumnInfo("Telepon", "phone", 120),
                        new DataTableColumnInfo("Jabatan", "rolename", 200),
                        new DataTableColumnInfo("Created By", "creator", 180),
                        new DataTableColumnInfo("Created At", "createdDate", 120, DataGridViewContentAlignment.MiddleRight, "dd-MM-yyyy HH:mm")
                    }, this.BindingSource);
                    break;
                case ListingData.Accounts:
                    GridHelpers.InitializeDataGridColumns(this.grid, new DataTableColumnInfo[]
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
                    accountButton.OnClick = async () =>
                    {
                        await OpenChildWindow(new ListAccountProviderForm());
                    };
                    break;
                case ListingData.Purchases:
                    GridHelpers.InitializeDataGridColumns(this.grid, new DataTableColumnInfo[]
                    {
                        new DataTableColumnInfo("Faktur", "invoicenumber", 180),
                        new DataTableColumnInfo("Tanggal", "purchasedate", 100, DataGridViewContentAlignment.MiddleRight, "dd/MM/yy HH:mm"),
                        new DataTableColumnInfo("Supplier", "suppliername", 200),
                        new DataTableColumnInfo("Status", "status", 110),
                        new DataTableColumnInfo("Nilai", "grandtotal", 100, DataGridViewContentAlignment.MiddleRight, "N0"),
                        new DataTableColumnInfo("Hutang", "ap", 120, DataGridViewContentAlignment.MiddleRight, "N0"),
                        new DataTableColumnInfo("Dibuat Oleh", "creator", 150),
                        new DataTableColumnInfo("Tanggal Dibuat", "createddate", 120, DataGridViewContentAlignment.MiddleRight, "dd/MM/yy HH:mm:ss")
                    }, this.BindingSource);
                    this._useDateRange = true;
                    var plabel = new Label()
                    {
                        Location = new Point(150, 18),
                        Text = "Periode",
                        AutoSize = true
                    };
                    var startDate = new DateTimePicker()
                    {
                        Size = new Size(100, 25),
                        Location = new Point(210, 15),
                        CustomFormat = "dd/MM/yyyy",
                        Format = DateTimePickerFormat.Custom,
                        Value = _dateRange.StartDate
                    };
                    var endDate = new DateTimePicker()
                    {
                        Size = new Size(100, 25),
                        Location = new Point(315, 15),
                        CustomFormat = "dd/MM/yyyy",
                        Format = DateTimePickerFormat.Custom,
                        Value = _dateRange.EndDate
                    };
                    startDate.CloseUp += async (sender, e) =>
                    {
                        this._dateRange.StartDate = startDate.Value;
                        await this.LoadDataAsync();
                    };
                    endDate.CloseUp += async (sender, e) =>
                    {
                        this._dateRange.EndDate = endDate.Value;
                        await this.LoadDataAsync();
                    };
                    this.Controls.Add(plabel);
                    this.Controls.Add(startDate);
                    this.Controls.Add(endDate);
                    break;
            }
            this.grid.InsertIconColumn(this.Menu.Image);
            this.grid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.grid.ColumnHeadersDefaultCellStyle.Font = new Font(this.Font, FontStyle.Bold);
            this.grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = this.grid.ColumnHeadersDefaultCellStyle.BackColor;
            this.grid.MouseUp += (sender, e) =>
            {
                if (e.Button == MouseButtons.Right)
                {
                    var hit = grid.HitTest(e.X, e.Y);
                    if (hit.Type == DataGridViewHitTestType.ColumnHeader)
                    {
                        contextMenuStrip1.Show(grid, e.Location);
                    }
                }
            };
        }
        private async Task OpenChildWindow(Control control, DockStyle dock = DockStyle.Right)
        {
            var mainForm = this.FindForm();
            if (mainForm is null) return;

            using (var tform = new TransparentfForm(mainForm))
            {
                control.BackColor = Color.FromArgb(250, 250, 250);
                if (dock == DockStyle.None)
                {
                    var x = (int)((tform.Width - control.Width) / 2);
                    var y = (int)((tform.Height - control.Height) / 2);
                    control.Location = new Point(x, y);
                }
                else control.Dock = dock;
                tform.ControlToDisplay = control;
                if (tform.ShowDialog(this) == DialogResult.OK)
                {
                    await this.LoadDataAsync();
                }
            }
        }
        public SideBarPanel.Menu Menu { get; }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            var button = this.buttons.GetHoveredButton(e.Location);
            if (button != null)
            {
                Invalidate();
                this.ShowToolTip(new Point(button.Location.X + button.Width + 5, button.Location.Y + 5), button.Text);
            }
            else
            {
                if (!this.buttons.AnyHoveredItem) this.HideToolTip();
            }
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
            this.buttons.ResetHover();
            this.HideToolTip();
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
            if (string.IsNullOrWhiteSpace(this.Menu.SQLSelect)) return;

            var mainForm = this.FindForm() as SPAForm;
            if (mainForm is null) return;

            mainForm.ShowLoadingOverlay();
            try
            {
                var parameters = new DbParameter[]
                {
                    db.CreateParameter("location", My.Application.GetCurrentLocationID(), DbType.Int16),
                    db.CreateParameter("start", this._dateRange.StartDate.ToUniversalTime(), DbType.DateTime),
                    db.CreateParameter("end", this._dateRange.EndDate.ToUniversalTime(), DbType.DateTime)
                };
                var table = await db.ExecuteDataTableAsync(this.Menu.SQLSelect, parameters);
                this.BindingSource.DataSource = table;
                if (this.filters.Count == 0)
                {
                    foreach (DataColumn col in table.Columns)
                    {
                        if (col.DataType == typeof(string))
                        {
                            filters.Add(col.ColumnName);
                        }
                    }
                }
            }
            finally
            {
                mainForm.HideLoadingOverlay();

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
                        Text = this.Menu.Type == ListingData.Suppliers ? "Supplier" : "Customer",
                        Icon = this.Menu.Image
                    };
                    break;
                case ListingData.Employee:
                    control = new EmployeeForm();
                    break;
                case ListingData.Accounts:
                    control = new AccountForm();
                    break;
                case ListingData.Users:
                    control = new LoginDetailForm()
                    {
                        Icon = this.Menu.Image
                    };
                    break;
                case ListingData.Purchases:
                    control = new PurchaseControl()
                    {
                        Icon = this.Menu.Image
                    };
                    break;
            }
            if (control != null)
            {
                control.Tag = id;
                var dock = this.Menu.Type == ListingData.Users 
                    || this.Menu.Type == ListingData.Suppliers 
                    || this.Menu.Type == ListingData.Customers
                    || this.Menu.Type == ListingData.Purchases ? DockStyle.None : DockStyle.Right;
                await OpenChildWindow(control, dock);
            }
        }
        private async Task DeleteRecordAsync()
        {
            if (this.BindingSource.Current != null)
            {
                if (MessageBox.Show("Anda yakin akan menghapus record ini secara permanen dari database?", "Konfirmasi Hapus", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (this.Menu.SQLDelete.Length > 0)
                    {
                        if (await (db.ExecuteNonQueryAsync(this.Menu.SQLDelete, db.CreateParameter("id", ((DataRowView)this.BindingSource.Current)[0]))))
                        {
                            await this.LoadDataAsync();
                        }
                    }
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

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                var search = this.textBox1.Text.Trim().Replace("'", "''");
                if (string.IsNullOrWhiteSpace(search))
                {
                    this.BindingSource.RemoveFilter();
                    return;
                }
                var cfilters = new string[this.filters.Count];
                for (int i=0; i < filters.Count; i++)
                {
                    cfilters[i] = "[" + filters[i] + "] LIKE '%" + search + "%'";
                }
                this.BindingSource.Filter = string.Join(" OR ", cfilters);
            }
        }
    }
}
