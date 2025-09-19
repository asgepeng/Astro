using Astro.Forms.Controls;
using Astro.Winform.Classes;
using Astro.Winform.Extensions;
using Astro.Binaries;
using System.Runtime.InteropServices;
using Astro.Winform.Controls;
using Astro.Models;
using Astro.Winform.UserControls;
using System.Data.Common;
using System.Data;
using Astro.Data;

namespace Astro.Winform.Forms
{
    public partial class SPAForm : Form
    {
        #region Winform
        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HTCAPTION = 0x2;
        private const int HTCLIENT = 1;
        private const int WM_RBUTTONUP = 0x0205;

        private const uint TPM_LEFTALIGN = 0x0000;
        private const uint TPM_RETURNCMD = 0x0100;
        private const uint TPM_RIGHTBUTTON = 0x0002;
        private const uint WM_SYSCOMMAND = 0x0112;

        private const int AW_SLIDE = 0x00040000;
        private const int AW_HIDE = 0x00010000;
        private const int AW_BLEND = 0x00080000;
        private const int AW_HOR_NEGATIVE = 0x00000002;
        private const int AW_VER_NEGATIVE = 0x00000008;

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        [DllImport("user32.dll")]
        private static extern int TrackPopupMenu(IntPtr hMenu, uint uFlags, int x, int y, int nReserved, IntPtr hWnd, IntPtr prcRect);
        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();
        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("dwmapi.dll")]
        public static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS pMarInset);
        [DllImport("dwmapi.dll")]
        public static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        [DllImport("dwmapi.dll")]
        public static extern int DwmIsCompositionEnabled(ref int pfEnabled);
        [DllImport("user32.dll")]
        private static extern IntPtr MonitorFromWindow(IntPtr handle, int flags);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);
        [DllImport("user32.dll")]
        private static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll")]
        private static extern bool AnimateWindow(IntPtr hwnd, int dwTime, int dwFlags);
        [DllImport("user32.dll")]
        private static extern IntPtr WindowFromPoint(POINT pt);

        private enum HitButton { None, Minimize, Maximize, Close }
        private HitButton hitButton = HitButton.None;

        public struct MARGINS
        {
            public int cxLeftWidth;
            public int cxRightWidth;
            public int cyTopHeight;
            public int cyBottomHeight;
        }
        private void ApplyShadow()
        {
            int enabled = 0;
            DwmIsCompositionEnabled(ref enabled);
            if (enabled == 1)
            {
                int val = 2;
                DwmSetWindowAttribute(this.Handle, 2, ref val, sizeof(int));

                MARGINS margins = new MARGINS()
                {
                    cxLeftWidth = 1,
                    cxRightWidth = 1,
                    cyTopHeight = 1,
                    cyBottomHeight = 1
                };
                DwmExtendFrameIntoClientArea(this.Handle, ref margins);
            }
        }
        private void ShowSystemMenu(Point screenPoint)
        {
            IntPtr hMenu = GetSystemMenu(this.Handle, false);
            if (hMenu != IntPtr.Zero)
            {
                int x = screenPoint.X;
                int y = screenPoint.Y;

                // Menu di posisi kursor, flag pakai TPM_RETURNCMD agar dapat command
                int command = (int)TrackPopupMenu(hMenu,
                    TPM_LEFTALIGN | TPM_RIGHTBUTTON | TPM_RETURNCMD,
                    x, y, 0, this.Handle, IntPtr.Zero);

                if (command != 0)
                {
                    // Kirim command ke window
                    PostMessage(this.Handle, WM_SYSCOMMAND, (IntPtr)command, IntPtr.Zero);
                }
            }
        }
        int RESIZE_HANDLE_SIZE = 3;
        protected override void WndProc(ref Message m)
        {
            const int WM_GETMINMAXINFO = 0x0024;
            const int WM_LBUTTONDBLCLK = 0x0203;
            const int WM_LBUTTONDOWN = 0x0201;
            const int WM_NCHITTEST = 0x0084;

            const int HTCLIENT = 1;
            const int HTBOTTOMRIGHT = 17;
            const int HTLEFT = 10;
            const int HTRIGHT = 11;
            const int HTTOP = 12;
            const int HTTOPLEFT = 13;
            const int HTTOPRIGHT = 14;
            const int HTBOTTOM = 15;
            const int HTBOTTOMLEFT = 16;

            
            const int WM_LBUTTONUP = 0x0202;

            switch (m.Msg)
            {
                case WM_GETMINMAXINFO:
                    WmGetMinMaxInfo(m.HWnd, m.LParam);
                    return;
                case WM_LBUTTONUP:
                    {
                        Point pos = this.PointToClient(Cursor.Position);
                        var hButtonLoc = this.ClientSize.Width - 126;

                        if (pos.Y <= this.Padding.Top && pos.X > hButtonLoc)
                        {
                            if (hitButton == HitButton.Minimize && pos.X <= hButtonLoc + 40)
                                this.WindowState = FormWindowState.Minimized;
                            else if (hitButton == HitButton.Maximize && pos.X >= hButtonLoc + 43 && pos.X <= hButtonLoc + 83)
                                ToggleMaximize();
                            else if (hitButton == HitButton.Close && pos.X >= hButtonLoc + 86 && pos.X <= hButtonLoc + 126)
                                this.Close();
                        }

                        hitButton = HitButton.None;
                        return;
                    }
                case WM_RBUTTONUP:
                    {
                        Point pos = this.PointToClient(Cursor.Position);
                        if (pos.Y <= 40)
                        {
                            ShowSystemMenu(Cursor.Position);
                            return;
                        }
                        break;
                    }
                case WM_LBUTTONDBLCLK:
                    {
                        Point pos = this.PointToClient(Cursor.Position);
                        if (pos.Y <= 40 && pos.X < this.ClientSize.Width - 144)
                        {
                            ToggleMaximize();
                            return;
                        }
                        break;
                    }
                case WM_LBUTTONDOWN:
                    {
                        Point pos = this.PointToClient(Cursor.Position);
                        var hButtonLoc = this.ClientSize.Width - 126;

                        if (pos.Y <= this.Padding.Top && pos.X > hButtonLoc)
                        {
                            if (pos.X <= hButtonLoc + 40)
                                hitButton = HitButton.Minimize;
                            else if (pos.X >= hButtonLoc + 43 && pos.X <= hButtonLoc + 83)
                                hitButton = HitButton.Maximize;
                            else if (pos.X >= hButtonLoc + 86 && pos.X <= hButtonLoc + 126)
                                hitButton = HitButton.Close;

                            return; // jangan eksekusi dulu, tunggu MouseUp
                        }

                        // drag move window
                        ReleaseCapture();
                        SendMessage(this.Handle, 0xA1, 0x2, 0);
                        return;
                    }

                case WM_NCHITTEST:
                    {
                        Point cursor = this.PointToClient(Cursor.Position);

                        if (cursor.X <= RESIZE_HANDLE_SIZE)
                        {
                            if (cursor.Y <= RESIZE_HANDLE_SIZE)
                                m.Result = (IntPtr)HTTOPLEFT;
                            else if (cursor.Y >= this.ClientSize.Height - RESIZE_HANDLE_SIZE)
                                m.Result = (IntPtr)HTBOTTOMLEFT;
                            else
                                m.Result = (IntPtr)HTLEFT;
                            return;
                        }
                        else if (cursor.X >= this.ClientSize.Width - RESIZE_HANDLE_SIZE)
                        {
                            if (cursor.Y <= RESIZE_HANDLE_SIZE)
                                m.Result = (IntPtr)HTTOPRIGHT;
                            else if (cursor.Y >= this.ClientSize.Height - RESIZE_HANDLE_SIZE)
                                m.Result = (IntPtr)HTBOTTOMRIGHT;
                            else
                                m.Result = (IntPtr)HTRIGHT;
                            return;
                        }
                        else if (cursor.Y <= RESIZE_HANDLE_SIZE)
                        {
                            m.Result = (IntPtr)HTTOP;
                            return;
                        }
                        else if (cursor.Y >= this.ClientSize.Height - RESIZE_HANDLE_SIZE)
                        {
                            m.Result = (IntPtr)HTBOTTOM;
                            return;
                        }

                        if (cursor.Y <= 40)
                        {
                            m.Result = (IntPtr)HTCLIENT;
                            return;
                        }

                        break;
                    }

            }
            base.WndProc(ref m);
        }
        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int x;
            public int y;
        }
        [StructLayout(LayoutKind.Sequential)]
        private struct MINMAXINFO
        {
            public POINT ptReserved;
            public POINT ptMaxSize;
            public POINT ptMaxPosition;
            public POINT ptMinTrackSize;
            public POINT ptMaxTrackSize;
        }
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct MONITORINFO
        {
            public int cbSize;
            public RECT rcMonitor;
            public RECT rcWork;
            public int dwFlags;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left, Top, Right, Bottom;
        }
        private void WmGetMinMaxInfo(IntPtr hwnd, IntPtr lParam)
        {
            MINMAXINFO mmi = Marshal.PtrToStructure<MINMAXINFO>(lParam);

            // pastikan max size & position sesuai monitor aktif
            Screen screen = Screen.FromHandle(hwnd);
            Rectangle workingArea = screen.WorkingArea;
            Rectangle monitorArea = screen.Bounds;

            mmi.ptMaxPosition.x = workingArea.Left - monitorArea.Left;
            mmi.ptMaxPosition.y = workingArea.Top - monitorArea.Top;
            mmi.ptMaxSize.x = workingArea.Width;
            mmi.ptMaxSize.y = workingArea.Height;

            // optional: set min size biar gak bisa terlalu kecil
            mmi.ptMinTrackSize.x = this.MinimumSize.Width;
            mmi.ptMinTrackSize.y = this.MinimumSize.Height;

            Marshal.StructureToPtr(mmi, lParam, true);
        }

        private void ToggleMaximize()
        {
            if (this.WindowState == FormWindowState.Maximized)
            {
                this.WindowState = FormWindowState.Normal;
                this.RESIZE_HANDLE_SIZE = 3;
            }
            else
            {
                this.WindowState = FormWindowState.Maximized;
                this.RESIZE_HANDLE_SIZE = 0;
            }
        }
        #endregion Winform
        #region Members
        private Point _mousePoint;
        private LoadingOverlayForm _loadingOverlay;
        private readonly StringFormat _center = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
        private readonly IDBClient db;
        #endregion
        public SPAForm()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.AllPaintingInWmPaint
                    | ControlStyles.UserPaint
                    | ControlStyles.ResizeRedraw
                    | ControlStyles.OptimizedDoubleBuffer, true);
            this.UpdateStyles();
            _loadingOverlay = new LoadingOverlayForm(this);
            this.userPanel1.OnAccountButtonClick = this.HandleOnUserPanelClick;
            this.db = My.Application.CreateDBAccess();
        }
        internal void ShowLoadingOverlay()
        {
            if (_loadingOverlay is null)
            {
                _loadingOverlay = new LoadingOverlayForm(this);
            }
            else if (_loadingOverlay.IsDisposed)
            {
                _loadingOverlay = new LoadingOverlayForm(this);
            }

            _loadingOverlay.Size = this.Size;
            _loadingOverlay.Location = this.Location;
            _loadingOverlay.Show(this);
        }
        internal void HideLoadingOverlay()
        {
            _loadingOverlay.Hide();
        }
        private async void SPAForm_Load(object sender, EventArgs e)
        {
            using (var loginDialog = new LoginForm())
            {
                if (loginDialog.ShowDialog(this) == DialogResult.OK)
                {
                    var home = sideBarPanel.Groups.Add("HOME");
                    home.Selected = true;
                    var dashboard = home.Items.Add(0, "Dashboard");
                    dashboard.Selected = true;
                    dashboard.Image = """
                    <svg width="24" height="24" viewBox="0 0 24 24" fill="none"
                         xmlns="http://www.w3.org/2000/svg">
                      <rect x="3"  y="3"  width="8" height="8" rx="2" stroke="#fff" stroke-width="2"/>
                      <rect x="13" y="3"  width="8" height="5" rx="2" stroke="#fff" stroke-width="2"/>
                      <rect x="13" y="10" width="8" height="11" rx="2" stroke="#fff" stroke-width="2"/>
                      <rect x="3"  y="13" width="8" height="8" rx="2" stroke="#fff" stroke-width="2"/>
                    </svg>                
                    """.ToImage();

                    var commandText = """
                    SELECT s.sectionid, s.title, m.menuid, m.title AS menu_title, m.icon, rtm.allowcreate, rtm.allowread, rtm.allowupdate, rtm.allowdelete
                    FROM rolemenus AS rtm 
                    INNER JOIN menus m ON rtm.menuid = m.menuid 
                    INNER JOIN sections s ON m.sectionid = s.sectionid
                    WHERE m.disabled = false AND rtm.roleid = @roleId
                    ORDER BY s.sectionid, m.menuid
                    """;
                    var listMenu = new ListMenu();
                    using (var writer = new BinaryDataWriter())
                    {
                        await db.ExecuteReaderAsync(async (DbDataReader reader) =>
                        {
                            SideBarPanel.GroupMenu? group = null;     

                            var currentSectionId = 0;

                            while (await reader.ReadAsync())
                            {
                                var sectionId = reader.GetInt16(0);
                                if (sectionId != currentSectionId || group is null)
                                {
                                    currentSectionId = sectionId;
                                    group = sideBarPanel.Groups.Add(reader.GetString(1).Replace("&", "&&"));
                                }
                                var menu = group.Items.Add(reader.GetInt16(2), reader.GetString(3));
                                menu.SetUserAccess(reader.GetBoolean(5), reader.GetBoolean(6), reader.GetBoolean(7), reader.GetBoolean(8));
                                var svg = reader.GetString(4);
                                if (!string.IsNullOrWhiteSpace(svg))
                                {
                                    menu.Image = svg.ToImage();
                                }
                                switch (menu.Id)
                                {
                                    case 1:
                                        menu.Type = ListingData.Products;
                                        menu.SQLSelect = """
                                        SELECT p.productid, p.name, p.sku, c.name AS categoryname, i.stock, u.name AS unitname, i.price, e.fullname, p.createddate
                                        FROM products AS p
                                            INNER JOIN categories AS c ON p.categoryid = c.categoryid
                                            INNER JOIN units AS u ON p.unitid = u.unitid
                                            INNER JOIN inventories AS i ON p.productid = i.productid AND i.locationid = 1
                                            INNER JOIN employees AS e ON p.creatorid = e.employeeid
                                        WHERE p.isdeleted = false
                                        ORDER BY p.name
                                        """;
                                        menu.SQLDelete = """
                                        UPDATE products SET isdeleted = true
                                        WHERE productid = @id
                                        """;
                                        break;
                                    case 3:
                                        menu.Type = ListingData.Customers;
                                        menu.SQLSelect = """
                                        SELECT c.contactid, 
                                            c.name, 
                                            COALESCE(ca.streetaddress || ' ' || v.name || ' ' || d.name || ' ' || cty.name || ', ' || ca.zipcode, '') AS address,
                                            COALESCE(p.phonenumber, '') as phonenumber, 
                                            creator.fullname, 
                                            c.createddate
                                        FROM contacts c
                                        LEFT JOIN addresses ca 
                                            ON c.contactid = ca.contactid AND ca.isprimary = true
                                        LEFT JOIN villages v 
                                            ON ca.villageid = v.villageid
                                        LEFT JOIN districts d 
                                            ON v.districtid = d.districtid
                                        LEFT JOIN cities cty 
                                            ON d.cityid = cty.cityid
                                        LEFT JOIN phones p 
                                            ON c.contactid = p.contactid AND p.isprimary = true
                                        INNER JOIN employees creator 
                                            ON c.creatorid = creator.employeeid
                                        WHERE c.isdeleted = false 
                                          AND c.contacttype = 1
                                        ORDER BY c.name;
                                        """;
                                        menu.SQLDelete = """
                                        UPDATE contacts
                                        SET isdeleted = true
                                        WHERE contactid = @id
                                        """;
                                        break;
                                    case 2:
                                        menu.Type = ListingData.Suppliers;
                                        menu.SQLSelect = """
                                        SELECT c.contactid, 
                                            c.name, 
                                            COALESCE(ca.streetaddress || ' ' || v.name || ' ' || d.name || ' ' || cty.name || ', ' || ca.zipcode, '') AS address,
                                            COALESCE(p.phonenumber, '') as phonenumber, 
                                            creator.fullname, 
                                            c.createddate
                                        FROM contacts c
                                        LEFT JOIN addresses ca 
                                            ON c.contactid = ca.contactid AND ca.isprimary = true
                                        LEFT JOIN villages v 
                                           ON ca.villageid = v.villageid
                                        LEFT JOIN districts d 
                                               ON v.districtid = d.districtid
                                        LEFT JOIN cities cty 
                                               ON d.cityid = cty.cityid
                                        LEFT JOIN phones p 
                                               ON c.contactid = p.contactid AND p.isprimary = true
                                        INNER JOIN employees creator 
                                               ON c.creatorid = creator.employeeid
                                        WHERE c.isdeleted = false 
                                          AND c.contacttype = 0
                                        ORDER BY c.name;
                                        """;
                                        menu.SQLDelete = """
                                        UPDATE contacts
                                        SET isdeleted = true
                                        WHERE contactid = @Id
                                        """;
                                        break;
                                    case 4:
                                        menu.Type = ListingData.Employee;
                                        menu.SQLSelect = """
                                        SELECT e.employeeid, e.fullname, e.streetaddress, e.email, e.phone, r.name AS rolename, CASE e.creatorid WHEN 0 THEN 'System' ELSE c.fullname END AS creator, e.createddate
                                        FROM employees AS e
                                        INNER JOIN roles AS r ON e.roleid = r.roleid
                                        LEFT JOIN employees AS c ON e.creatorid = c.employeeid
                                        WHERE e.isdeleted = false AND e.employeeid > 1
                                        """;
                                        menu.SQLDelete = """
                                        UPDATE employees
                                        SET isdeleted = true
                                        WHERE employeeid = @id
                                        """;
                                        break;
                                    case 5:
                                        menu.Type = ListingData.Accounts;
                                        menu.SQLSelect = """
                                        SELECT acc.accountid, acc.accountname, acc.accountnumber, 
                                        CASE ap.providertype WHEN 1 THEN 'Bank' WHEN 2 THEN 'E-Wallet' WHEN 3 THEN 'E-Money' ELSE '-' END AS accounttype, 
                                        ap.name, u.fullname, acc.createddate, acc.editeddate
                                        FROM accounts AS acc
                                        INNER JOIN accountproviders AS ap ON acc.providerid = ap.providerid
                                        INNER JOIN employees AS u ON acc.creatorid = u.employeeid
                                        WHERE acc.isdeleted = false
                                        """;
                                        menu.SQLDelete = """
                                        UPDATE accounts
                                        SET isdeleted = true
                                        WHERE accountid = @id
                                        """;
                                        break;
                                    case 12:
                                        menu.Type = ListingData.Purchases;
                                        menu.SQLSelect = """
                                        SELECT p.purchaseid, p.invoicenumber, s.name AS suppliername, p.purchasedate, p.grandtotal, p.grandtotal - p.totalpaid AS ap, 
                                            CASE p.status WHEN 2 THEN 'Lunas' WHEN 1 THEN 'Bayar Sebagian' ELSE 'Belum dibayar' END AS status, e.fullname AS creator, p.createddate
                                        FROM purchases AS p
                                        LEFT JOIN contacts AS s ON p.supplierid = s.contactid
                                        INNER JOIN employees AS e ON p.creatorid = e.employeeid
                                        WHERE p.purchasedate BETWEEN @start AND @end
                                        AND p.locationid = @location
                                        """;
                                        break;
                                    case 20:
                                        menu.Type = ListingData.Users;
                                        menu.SQLSelect = """
                                        SELECT e.employeeid, e.fullname, CASE l.creatorid WHEN 0 THEN 'System' ELSE creator.fullname END AS creator, l.createddate
                                        FROM logins AS l
                                        INNER JOIN employees AS e ON l.employeeid = e.employeeid
                                        LEFT JOIN employees AS creator ON l.creatorid = creator.employeeid
                                        WHERE e.isdeleted = false AND e.employeeid > 1
                                        """;
                                        menu.SQLDelete = """
                                        DELETE FROM logins
                                        WHERE employeeid = @id
                                        """;
                                        break;
                                    case 21:
                                        menu.Type = ListingData.Roles;
                                        menu.SQLSelect = """
                                        SELECT r.roleid, r.name, CASE WHEN r.creatorid = 0 THEN 'System' else c.fullname END AS creator, r.createddate
                                        FROM roles AS r
                                        LEFT JOIN employees AS c ON r.creatorid = c.employeeid
                                        WHERE r.roleid > 1
                                        """;
                                        menu.SQLDelete = """

                                        """;
                                        break;
                                }
                            }

                        }, commandText, db.CreateParameter("roleId", (short)1, DbType.Int16));
                        this.sideBarPanel.OnItemClicked = this.OnSideBarMenuClicked;
                        this.sideBarPanel.RecalculateContent();

                        this.Text = "Dashboard";
                        var dbcontrol = new DashboardUserControl();
                        dbcontrol.Name = "0";
                        dbcontrol.Dock = DockStyle.Fill;
                        this.mainPanel.Controls.Add(dbcontrol);
                    }
                    this.WindowState = FormWindowState.Maximized;
                    this.RESIZE_HANDLE_SIZE = 0;
                    ApplyShadow();
                }
            }
        }
        #region Protected Area
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Button == MouseButtons.Left && e.Y < 40)
            {
                ReleaseCapture();
                SendMessage(this.Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
            }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var brandRect = new Rectangle(0, 0, this.sideBarPanel.Width + this.Padding.Left, e.ClipRectangle.Width);
            var image = global::Astro.Winform.Properties.Resources.logo_bizmate;
            var imageRectangle = new Rectangle(15, 8, 100, this.Padding.Top - 20);
            using (var backBrush = new SolidBrush(this.sideBarPanel.BackColor))
            {
                e.Graphics.FillRectangle(backBrush, brandRect);
            }
            e.Graphics.DrawImage(image, imageRectangle);
            var textRect = new Rectangle(this.sideBarPanel.Width +  this.Padding.Left + 5, -3, 220, this.Padding.Top + 3);
            TextFormatFlags flags = TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis;
            TextRenderer.DrawText(e.Graphics, this.Text, My.Application.TitleFont, textRect, My.Application.TitleForeColor, flags);

            //draw control buttons
            int x = this.ClientRectangle.Width - 126, y = 0, width = 40, height = this.Padding.Top;
            var chrs = new string[]{ "\uE921", "\uE922", "\uE8BB", "\uE923" };
            for (int i = 0; i < 3; i++)
            {
                var r = new Rectangle(x, y, width, height);
                if (r.Contains(_mousePoint))
                {
                    using (var brush = new SolidBrush(i == 2 ? Color.Red : Color.LightGray))
                    {
                        e.Graphics.FillRectangle(brush, r);
                    }
                }
                if (i == 2 && r.Contains(_mousePoint))
                {
                    e.Graphics.DrawString(chrs[i], My.Application.EmojiFont, Brushes.White, r, _center);
                }
                else
                {
                    var cindex = i == 1 && this.WindowState == FormWindowState.Maximized ? 3 : i;
                    e.Graphics.DrawString(chrs[cindex], My.Application.EmojiFont, Brushes.Black, r, _center);
                }
                x += 43;
            }
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _mousePoint = new Point(-1, -1);
            Invalidate();
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            var headerArea = new Rectangle(this.sideBarPanel.Width, 0, this.ClientSize.Width - this.sideBarPanel.Width, this.Padding.Top);
            if (headerArea.Contains(e.Location))
            {
                this._mousePoint = e.Location;
                this.Invalidate(headerArea);
            }
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // tambahkan animasi close
            AnimateWindow(this.Handle, 300, AW_BLEND | AW_HIDE);
            base.OnFormClosing(e);
        }
        #endregion Protected Area
        private Control? GetControl(string key)
        {
            var ctrls = this.mainPanel.Controls.Find(key, false);
            if (ctrls.Length > 0) return ctrls[0];
            return null;
        }
        private async Task OnSideBarMenuClicked(SideBarPanel.Menu menu)
        {
            this.Text = menu.Title;
            var headerArea = new Rectangle(0, 0, this.ClientSize.Width, 40);
            this.Invalidate(headerArea);
            Control? ctrl = GetControl(menu.Id.ToString());
            if (ctrl != null)
            {
                ctrl.BringToFront();
                if (ctrl is ListingControl listing)
                {
                    await listing.LoadDataAsync();
                }
                return;
            }
            switch (menu.Id)
            {
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 9:
                case 10:
                case 12:
                case 20:
                case 21:
                    ctrl = new ListingControl(menu);
                    break;
                case 6:
                    ctrl = new StockOpnameForm();
                    break;
            }
            if (ctrl != null)
            {
                ctrl.Name = menu.Id.ToString();
                ctrl.Dock = DockStyle.Fill;
                this.mainPanel.Controls.Add(ctrl);
                ctrl.BringToFront();
            }
        }
        private void HandleOnUserPanelClick(int code)
        {
            if (code == 3)
            {
                using (var sqlform = new SqlExecuteForm())
                {
                    sqlform.ShowDialog();
                }
                return;
            }
            var overlay = new OverlayForm(this);
            overlay.StartPosition = FormStartPosition.Manual;
            overlay.Size = this.Size;

            var panel = new CashFlowControl();
            panel.BorderStyle = BorderStyle.FixedSingle;
            panel.BackColor = Color.FromArgb(250, 250, 250);
            panel.Size = new Size(700, 350);
            panel.Dock = DockStyle.Right;
            overlay.Controls.Add(panel);
            overlay.ShowDialog(this);
        }
    }
}
