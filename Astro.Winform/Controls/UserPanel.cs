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
                <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" width="24" height="24">
                  <g fill="none" stroke="#000000" stroke-width="1.6" stroke-linecap="round" stroke-linejoin="round">
                    <circle cx="12" cy="8" r="3.2"/>
                    <path d="M4 19c0-3.3 3.6-5.8 8-5.8s8 2.5 8 5.8"/>
                  </g>
                </svg>
                """.ToImage(30,30);
            this._email = """
                <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 16 16" width="16" height="16">
                  <path d="M1.5 4.2h13v7.6a1 1 0 0 1-1 1H2.5a1 1 0 0 1-1-1V4.2z" fill="#FFFFFF" stroke="#222222" stroke-width="0.9" stroke-linejoin="round"/>
                  <path d="M2 5.1L8 9.1l6-4" fill="none" stroke="#222222" stroke-width="0.9" stroke-linecap="round" stroke-linejoin="round"/>
                </svg>
                """.ToImage(30,30);
            this._cashflow = """
                <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" width="24" height="24">
                  <rect x="3" y="6" width="18" height="12" rx="2" 
                        fill="#FFFFFF" stroke="#2E7D32" stroke-width="1.5"/>
                  <circle cx="12" cy="12" r="2.5" fill="#2E7D32"/>
                  <path d="M7 17 L7 9 L5 11" 
                        fill="none" stroke="#1976D2" stroke-width="1.6" stroke-linecap="round" stroke-linejoin="round"/>
                  <path d="M17 7 L17 15 L19 13" 
                        fill="none" stroke="#D32F2F" stroke-width="1.6" stroke-linecap="round" stroke-linejoin="round"/>
                </svg>
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
