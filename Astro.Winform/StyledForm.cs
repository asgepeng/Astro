using Astro.Drawing.Extensions;
using Astro.Forms.Controls;
using Astro.Winform.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Astro.Winform
{
    public partial class StyledForm : Form
    {
        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HTCAPTION = 0x2;
        private const int HTCLIENT = 1;
        private const int WM_RBUTTONUP = 0x0205;

        private const uint TPM_LEFTALIGN = 0x0000;
        private const uint TPM_RETURNCMD = 0x0100;
        private const uint TPM_RIGHTBUTTON = 0x0002;
        private const uint WM_SYSCOMMAND = 0x0112;

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

            switch (m.Msg)
            {
                case WM_GETMINMAXINFO:
                    WmGetMinMaxInfo(m.HWnd, m.LParam);
                    return;
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
                        var hButtonLoc = this.ClientSize.Width - 144;
                        if (pos.Y <= 40 && pos.X > hButtonLoc)
                        {
                            this.topNavigator.ResetHover();
                            if (pos.X > hButtonLoc)
                            {
                                if (pos.X <= hButtonLoc + 48)
                                {
                                    this.WindowState = FormWindowState.Minimized;
                                    return;
                                }
                                else if (pos.X <= hButtonLoc + 96)
                                {
                                    ToggleMaximize();
                                    return;
                                }
                                else if (pos.X <= hButtonLoc + 144)
                                {
                                    this.Close();
                                    return;
                                }
                            }
                            ReleaseCapture();
                            SendMessage(this.Handle, 0xA1, 0x2, 0);
                            return;
                        }
                        break;
                    }

                case WM_NCHITTEST:
                    {
                        Point cursor = this.PointToClient(Cursor.Position);
                        if (cursor.Y <= 40 && cursor.Y > RESIZE_HANDLE_SIZE &&
                            cursor.X > RESIZE_HANDLE_SIZE && cursor.X < this.ClientSize.Width - RESIZE_HANDLE_SIZE)
                        {
                            m.Result = (IntPtr)HTCLIENT;
                            return;
                        }
                        // ?? Resize hit area
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
            const int MONITOR_DEFAULTTONEAREST = 0x00000002;

            IntPtr monitor = MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);

            if (monitor != IntPtr.Zero)
            {
                MONITORINFO monitorInfo = new MONITORINFO();
                monitorInfo.cbSize = Marshal.SizeOf(typeof(MONITORINFO));
                GetMonitorInfo(monitor, ref monitorInfo);

                MINMAXINFO mmi = Marshal.PtrToStructure<MINMAXINFO>(lParam);

                RECT work = monitorInfo.rcWork;
                RECT monitorArea = monitorInfo.rcMonitor;

                mmi.ptMaxPosition.x = work.Left - monitorArea.Left;
                mmi.ptMaxPosition.y = work.Top - monitorArea.Top;

                mmi.ptMaxSize.x = work.Right - work.Left;
                mmi.ptMaxSize.y = (work.Bottom - work.Top) - 1;

                Marshal.StructureToPtr(mmi, lParam, true);
            }
        }


        private ToolTip toolTipGlobal;
        private SideBar leftSideBar;
        private Header topNavigator;
        private VirtualControlCollection VirtualControls { get; } = new VirtualControlCollection();
        public StyledForm()
        {
            InitializeComponent();
            toolTipGlobal = new ToolTip();
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.UserPaint |
                          ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.OptimizedDoubleBuffer |
                          ControlStyles.StandardDoubleClick, true);
            this.UpdateStyles();

            //Top Navigator
            this.topNavigator = new Header();

            //Navigator
            this.leftSideBar = new SideBar();
            this.leftSideBar.Items.Add("Dashboard", global::Astro.Winform.Properties.Resources.db);
            this.leftSideBar.Items.Add("Product", global::Astro.Winform.Properties.Resources.icons8_product_24);
            this.leftSideBar.Items.Add("Customer", global::Astro.Winform.Properties.Resources.icustomer);
            this.leftSideBar.SelectedIndex = 0;

            this.VirtualControls.Add(topNavigator);
            this.VirtualControls.Add(leftSideBar);
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            var login = new LoginForm();
            if (login.ShowDialog() == DialogResult.OK)
            {
                this.WindowState = FormWindowState.Maximized;
                ApplyShadow();
            }
            else
            {
                this.Close();
            }
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Button == MouseButtons.Left && e.Y < 40)
            {
                ReleaseCapture();
                SendMessage(this.Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
            }
        }
        private void ToggleMaximize()
        {
            if (this.WindowState == FormWindowState.Maximized)
                this.WindowState = FormWindowState.Normal;
            else
                this.WindowState = FormWindowState.Maximized;
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            this.topNavigator.IsMaximized = this.WindowState == FormWindowState.Maximized;
            foreach (var item in this.VirtualControls)
            {
                item.Draw(e.Graphics);
            }
        }
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
            e.Graphics.DrawRectangle(Pens.Gainsboro, new Rectangle(0, 0, this.ClientSize.Width, this.ClientSize.Height));
            var rect = new Rectangle(47, 40, this.ClientSize.Width - 47, this.ClientSize.Height - 40);
            using (Brush fillColor = new SolidBrush(Color.White))
            using (Pen pen = new Pen(Color.Gainsboro))
            {
                e.Graphics.DrawTopLeftRoundedRectangle(rect, 8, fillColor, pen);
            }
        }
        protected override void OnResize(EventArgs e)
        {
            if (leftSideBar != null) leftSideBar.MainFormResize(this.ClientSize);
            if (topNavigator != null) topNavigator.MainFormResize(this.ClientSize);
            this.Invalidate();
            base.OnResize(e);
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (leftSideBar.Bounds.Contains(e.Location))
            {
                var anyHovered = leftSideBar.GetHoveredItem(e.Location);
                if (anyHovered != null)
                {
                    if (anyHovered.Hovered)
                    {
                        this.toolTipGlobal.ToolTipTitle = anyHovered.Text;
                        this.toolTipGlobal.Show(anyHovered.Text, this, new Point(anyHovered.Bounds.X + anyHovered.Bounds.Width + 10, anyHovered.Bounds.Y));
                    }
                    this.Invalidate(this.leftSideBar.Bounds);
                }
                if (!leftSideBar.Items.AnyHoveredItem())
                {
                    this.toolTipGlobal.Hide(this);
                }
            }
            else if (topNavigator.Bounds.Contains(e.Location))
            {
                var buttonHovered = topNavigator.GetHoveredButton(e.Location);
                this.Invalidate(topNavigator.Bounds);
            }
        }
        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (leftSideBar.Bounds.Contains(e.Location))
            {
                var selected = leftSideBar.GetSelectedItem(e.Location);
                if (selected != null)
                {
                    this.Invalidate(leftSideBar.Bounds);
                    if (selected.Index == 0)
                    {
                        if (this.Controls.ContainsKey("ucDashboard"))
                        {
                            var ucDashboard = this.Controls.Find("ucDashboard", true);
                            if (ucDashboard != null && ucDashboard.Length > 0) ucDashboard[0].BringToFront();
                        }
                        else
                        {
                            var ucProduct = new DashboardUserControl();
                            ucProduct.Name = "ucDashboard";
                            ucProduct.Location = new Point(48, 48);
                            ucProduct.Size = new Size(this.ClientSize.Width - 48, this.ClientSize.Height - 48);
                            ucProduct.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom | AnchorStyles.Left;
                            this.Controls.Add(ucProduct);
                            ucProduct.BringToFront();
                        }
                    }
                    else if (selected.Index == 1)
                    {
                        if (this.Controls.ContainsKey("ucProduct"))
                        {
                            var ucProduct = this.Controls.Find("ucProduct", true);
                            if (ucProduct != null && ucProduct.Length > 0) ucProduct[0].BringToFront();
                        }
                        else
                        {
                            var ucProduct = new ListingControl();
                            ucProduct.Name = "ucProduct";
                            ucProduct.Location = new Point(48, 48);
                            ucProduct.Size = new Size(this.ClientSize.Width - 48, this.ClientSize.Height - 48);
                            ucProduct.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom | AnchorStyles.Left;
                            this.Controls.Add(ucProduct);
                        }
                    }
                }
            }
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            this.topNavigator.ResetHover();
        }
    }
}
