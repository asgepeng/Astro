using Astro.Forms.Controls;
using Astro.Winform.Classes;
using Astro.Winform.Controls;
using Astro.Winform.Extensions;
using Svg;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Astro.Drawing.Extensions;

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

            const int RESIZE_HANDLE_SIZE = 3;
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

                        if (pos.Y <= 24 && pos.X > hButtonLoc)
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

                        if (pos.Y <= 24 && pos.X > hButtonLoc)
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
                this.WindowState = FormWindowState.Normal;
            else
                this.WindowState = FormWindowState.Maximized;
        }
        #endregion Winform
        #region Members
        private Point _mousePoint;
        private readonly Font _controlFont = new Font("Segoe MDL2 Assets", 7.75F, FontStyle.Regular);
        private readonly Font _labelFont = new Font("Segoe UI", 17.75F, FontStyle.Regular);
        private readonly StringFormat _center = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
        #endregion
        public SPAForm()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.AllPaintingInWmPaint
                    | ControlStyles.UserPaint
                    | ControlStyles.ResizeRedraw
                    | ControlStyles.OptimizedDoubleBuffer, true);
            this.UpdateStyles();
            this.userPanel1.OnAccountButtonClick = this.HandleOnUserPanelClick;
        }
        private async void SPAForm_Load(object sender, EventArgs e)
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
            using (var stream = await WClient.GetStreamAsync("/auth/permissions"))
            using (var reader = new Astro.Streams.Reader(stream))
            {
                var sectionLength = reader.ReadInt32();
                while (sectionLength > 0)
                {
                    var sectionID = reader.ReadInt16();
                    var sectionName = reader.ReadString();
                    var parent = new Models.Section() { Title = sectionName };
                    var group = sideBarPanel.Groups.Add(sectionName);
                    int menuLength = reader.ReadInt32();
                    while (menuLength > 0)
                    {
                        var menuID = reader.ReadInt16();
                        var menuTitle = reader.ReadString();
                        var svg = reader.ReadString();
                        
                        var allowAdd = reader.ReadBoolean();
                        var allowEdit = reader.ReadBoolean();
                        var allowUpdate = reader.ReadBoolean();
                        var allowDelete = reader.ReadBoolean();

                        var menu = group.Items.Add(menuID, menuTitle);
                        menu.SetUserAccess(allowAdd, allowEdit, allowUpdate, allowDelete);
                        if (!string.IsNullOrWhiteSpace(svg))
                        {
                            menu.Image = svg.ToImage();
                        }
                        switch (menu.Id)
                        {
                            case 1:
                                menu.Type = ListingData.Products;
                                menu.URL = "/data/products";
                                break;
                            case 3:
                                menu.Type = ListingData.Customers;
                                menu.URL = "/data/customers";
                                break;
                            case 2:
                                menu.Type = ListingData.Suppliers;
                                menu.URL = "/data/suppliers";
                                break;
                            case 4:
                                menu.Type = ListingData.Employee;
                                menu.URL = "/data/employees";
                                break;
                            case 5:
                                menu.Type = ListingData.Accounts;
                                menu.URL = "/data/accounts";
                                break;
                            case 21:
                                menu.Type = ListingData.Roles;
                                menu.URL = "/data/roles";
                                break;
                        }
                        menuLength--;
                    }
                    sectionLength--;
                }
                this.sideBarPanel.RecalculateContent();
            }
            this.WindowState = FormWindowState.Maximized;
            ApplyShadow();
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
            var textRect = new Rectangle(this.sideBarPanel.Width +  this.Padding.Left + 8, 0, 220, this.Padding.Top);
            var sf = new StringFormat() { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
            e.Graphics.DrawString(this.Text, _labelFont, Brushes.Black, textRect);

            //draw control buttons
            int x = this.ClientRectangle.Width - 126, y = 0, width = 40, height = 24;
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
                    e.Graphics.DrawString(chrs[i], _controlFont, Brushes.White, r, _center);
                }
                else
                {
                    var cindex = i == 1 && this.WindowState == FormWindowState.Maximized ? 3 : i;
                    e.Graphics.DrawString(chrs[cindex], _controlFont, Brushes.Black, r, _center);
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
        private async void sideBarPanel_MouseClick(object sender, MouseEventArgs e)
        {
            if (sideBarPanel.SelectedItem != null)
            {
                this.Text = sideBarPanel.SelectedItem.Title;
                var headerArea = new Rectangle(0, 0, this.ClientSize.Width, 40);
                this.Invalidate(headerArea);
                Control? ctrl = GetControl(sideBarPanel.SelectedItem.Id.ToString());
                if (ctrl != null)
                {
                    ctrl.BringToFront();
                    if (ctrl is ListingControl listing)
                    {
                        await listing.LoadDataAsync();
                    }
                    return;
                }
                switch (sideBarPanel.SelectedItem.Id)
                {
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                    case 6:
                    case 10:
                    case 21:
                        ctrl = new ListingControl(sideBarPanel.SelectedItem);
                        break;
                }
                if (ctrl != null)
                {
                    ctrl.Name = sideBarPanel.SelectedItem.Id.ToString();
                    ctrl.Dock = DockStyle.Fill;
                    this.mainPanel.Controls.Add(ctrl);
                    ctrl.BringToFront();
                }
            }
        }
        private void HandleOnUserPanelClick(int code)
        {
            var overlay = new OverlayForm();
            overlay.StartPosition = FormStartPosition.Manual;
            overlay.Size = this.Size;

            var panel = new Panel();
            panel.BorderStyle = BorderStyle.FixedSingle;
            panel.BackColor = Color.FromArgb(250, 250, 250);
            panel.Size = new Size(300, 200);
            panel.Dock = DockStyle.Right;
            overlay.Controls.Add(panel);
            overlay.ShowDialog(this);
        }
    }
}
