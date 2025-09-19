using Astro.Drawing.Extensions;
using Astro.Winform.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astro.Winform.Controls
{
    public class UserPanel : Control
    {
        Image? _avatar;
        Image? _email;
        Image? _cashflow;

        Rectangle _cashflowRect = new Rectangle();
        Rectangle _emailRect = new Rectangle();
        Rectangle _userRect = new Rectangle();
        Rectangle _textRect = new Rectangle();
        Rectangle _nameTag = new Rectangle();
        Point _mouseLoc;
        bool _mouseHit = false;
        Point _mousehitLoc;

        string _userName = "";
        bool _isSet = false;
        public UserPanel()
        {
            this.InitializeComponent();
            this.SetStyle(ControlStyles.AllPaintingInWmPaint
                 | ControlStyles.UserPaint
                 | ControlStyles.ResizeRedraw
                 | ControlStyles.OptimizedDoubleBuffer, true);
            this.UpdateStyles();
            this._avatar = """
                <svg viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg"><g id="SVGRepo_bgCarrier" stroke-width="0"></g><g id="SVGRepo_tracerCarrier" stroke-linecap="round" stroke-linejoin="round"></g><g id="SVGRepo_iconCarrier"> <circle opacity="0.5" cx="12" cy="9" r="3" stroke="#1C274C" stroke-width="1.5"></circle> <circle cx="12" cy="12" r="10" stroke="#1C274C" stroke-width="1.5"></circle> <path opacity="0.5" d="M17.9691 20C17.81 17.1085 16.9247 15 11.9999 15C7.07521 15 6.18991 17.1085 6.03076 20" stroke="#1C274C" stroke-width="1.5" stroke-linecap="round"></path> </g></svg>
                """.ToImage(48,48);
            this._email = """
                <svg viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg"><g id="SVGRepo_bgCarrier" stroke-width="0"></g><g id="SVGRepo_tracerCarrier" stroke-linecap="round" stroke-linejoin="round"></g><g id="SVGRepo_iconCarrier"> <path d="M12 22C17.5228 22 22 17.5228 22 12C22 6.47715 17.5228 2 12 2C6.47715 2 2 6.47715 2 12C2 13.5997 2.37562 15.1116 3.04346 16.4525C3.22094 16.8088 3.28001 17.2161 3.17712 17.6006L2.58151 19.8267C2.32295 20.793 3.20701 21.677 4.17335 21.4185L6.39939 20.8229C6.78393 20.72 7.19121 20.7791 7.54753 20.9565C8.88837 21.6244 10.4003 22 12 22Z" stroke="#1C274C" stroke-width="1.5"></path> <path opacity="0.5" d="M8 10.5H16" stroke="#1C274C" stroke-width="1.5" stroke-linecap="round"></path> <path opacity="0.5" d="M8 14H13.5" stroke="#1C274C" stroke-width="1.5" stroke-linecap="round"></path> </g></svg>
                """.ToImage(30,30);
            this._cashflow = """
                <svg viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg"><g id="SVGRepo_bgCarrier" stroke-width="0"></g><g id="SVGRepo_tracerCarrier" stroke-linecap="round" stroke-linejoin="round"></g><g id="SVGRepo_iconCarrier"> <path opacity="0.5" d="M18.6667 12C20.5513 11.7213 22 10.0457 22 8.02036C22 5.79998 20.2589 4 18.1111 4H5.88889C3.74112 4 2 5.79998 2 8.02036C2 10.0457 3.44873 11.7213 5.33333 12" stroke="#1C274C" stroke-width="1.5"></path> <path d="M12 7V14M12 14L14 11.6667M12 14L10 11.6667" stroke="#1C274C" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"></path> <path d="M5 11C5 9.11438 5 8.17157 5.58579 7.58579C6.17157 7 7.11438 7 9 7H15C16.8856 7 17.8284 7 18.4142 7.58579C19 8.17157 19 9.11438 19 11V17C19 18.8856 19 19.8284 18.4142 20.4142C17.8284 21 16.8856 21 15 21H9C7.11438 21 6.17157 21 5.58579 20.4142C5 19.8284 5 18.8856 5 17V11Z" stroke="#1C274C" stroke-width="1.5"></path> <path opacity="0.5" d="M5 18H19" stroke="#1C274C" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"></path> </g></svg>
                """.ToImage(30,30);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (!_isSet)
            {
                if (My.Application.User != null)
                {
                    _userName = My.Application.User?.Name ?? "";
                    var w = TextRenderer.MeasureText(_userName, this.Font).Width + 4;
                    _textRect = new Rectangle(e.ClipRectangle.Width - w, 0, w, e.ClipRectangle.Height);
                    _userRect = new Rectangle(_textRect.Left - e.ClipRectangle.Height - 4, 0, e.ClipRectangle.Height, e.ClipRectangle.Height);
                    _nameTag = new Rectangle(_userRect.X, _userRect.Y, _userRect.Width + 4 + _textRect.Width, _userRect.Height);
                    _emailRect = new Rectangle(_userRect.Left - _userRect.Width - 4, 0, _userRect.Width, _userRect.Height);
                    _cashflowRect = new Rectangle(_emailRect.Left - _emailRect.Width - 4, 0, _emailRect.Width, _emailRect.Height);
                    var curw = this.Width;
                    this.Size = new Size(_nameTag.Width + 10 + (this._userRect.Width * 2), this.Height);
                    var diff = curw - this.Width;

                    _textRect.Location = new Point(_textRect.Left - diff, _textRect.Top);
                    _userRect.Location = new Point(_userRect.Left - diff, _userRect.Top);
                    _nameTag.Location = new Point(_nameTag.Left - diff, _nameTag.Top);
                    _emailRect.Location = new Point(_emailRect.Left - diff, _emailRect.Top);
                    _cashflowRect.Location = new Point(_cashflowRect.Left - diff, _cashflowRect.Top);
                    this.Location = new Point(this.Location.X + diff, this.Location.Y);
                    _isSet = true;
                }
            }
            else
            {
                if (_nameTag.Contains(_mouseLoc))
                {
                    e.Graphics.DrawRoundedRectangle(_nameTag, (int)(_userRect.Height / 2), (_mouseHit && _nameTag.Contains(_mousehitLoc) ? Brushes.Red : Brushes.Lavender));
                }
                using (var brush = new SolidBrush(this.ForeColor))
                {
                    e.Graphics.DrawString(this._userName, this.Font, brush, _textRect, new StringFormat() { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center });
                }
                if (_avatar != null)
                {
                    var h = (int)_userRect.Height - 4;
                    e.Graphics.DrawImage(_avatar, _userRect.Left + 2, _userRect.Top + 2, h, h);
                }
                if (_emailRect.Contains(_mouseLoc)) e.Graphics.DrawRoundedRectangle(_emailRect, (int)(_emailRect.Height / 2), _mouseHit && _emailRect.Contains(_mousehitLoc) ? Brushes.Red : Brushes.Lavender);
                if (_email != null)
                {
                    var h = (int)_emailRect.Height - 4;
                    e.Graphics.DrawImage(_email, _emailRect.Left + 2, _emailRect.Top + 2, h, h);
                }
                if (_cashflowRect.Contains(_mouseLoc)) e.Graphics.DrawRoundedRectangle(_cashflowRect, (int)(_cashflowRect.Height / 2), _mouseHit && _cashflowRect.Contains(_mousehitLoc) ? Brushes.Red : Brushes.Lavender);
                if (_cashflow != null)
                {
                    var h = (int)_emailRect.Height - 4;
                    e.Graphics.DrawImage(_cashflow, _cashflowRect.Left + 2, _cashflowRect.Top + 2, h, h);
                }
            }
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            _mouseLoc = e.Location;
            Invalidate();
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _mouseLoc = new Point(-1, -1);
            _mouseHit = false;
            Invalidate();
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            this._mouseHit = true;
            _mousehitLoc = e.Location;
            Invalidate();
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (this.OnAccountButtonClick != null)
            {
                if (this._nameTag.Contains(_mousehitLoc) && this._nameTag.Contains(e.Location))
                {
                    this.OnAccountButtonClick.Invoke(1);
                }
                else if (this._emailRect.Contains(_mousehitLoc) && this._emailRect.Contains(e.Location))
                {
                    this.OnAccountButtonClick.Invoke(2);
                }
                else if (this._cashflowRect.Contains(_mousehitLoc) && this._cashflowRect.Contains(e.Location))
                {
                    this.OnAccountButtonClick.Invoke(3);
                }
            }
            this._mouseHit = false;
            Invalidate();
        }
        public Action<int>? OnAccountButtonClick = null;
        private void InitializeComponent()
        {
            SuspendLayout();
            // 
            // UserPanel
            // 
            Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            ForeColor = Color.Navy;
            ResumeLayout(false);
        }
    }
}
