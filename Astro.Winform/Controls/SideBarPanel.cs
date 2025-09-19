using Astro.Drawing.Extensions;
using Astro.Models;

using System.Collections.ObjectModel;

namespace Astro.Winform.Controls
{
    public class SideBarPanel : Control
    {
        #region Nested Class
        public class GroupMenu
        {
            public GroupMenu()
            {
                Items = new MenuCollection(this);
            }
            private bool _collapsed = false;
            public string Title { get; set; } = "";
            public MenuCollection Items { get; }
            public void Expand() => _collapsed = false;
            public void Collapse() => _collapsed = true;
            public bool IsCollapsed => _collapsed;
            public Rectangle Bounds { get; set; }
            public bool Hovered { get; set; }
            public bool Selected { get; set; }
        }
        public class GroupCollection : Collection<GroupMenu>
        {
            public int Height { get; private set; } = 0;
            public void Calculate()
            {
                Height = 0;
                foreach (var item in this)
                    Height += item.IsCollapsed ? 50 : 50 + item.Items.Count * 40;
            }
            public GroupMenu Add(string title)
            {
                var group = new GroupMenu() { Title = title };
                this.Add(group);
                return group;
            }
        }
        public class Menu
        {
            public Menu(GroupMenu group) => Group = group;
            public short Id { get; set; }
            public string Title { get; set; } = "";
            public GroupMenu Group { get; }
            public Image? Image { get; set; }
            public bool Selected { get; set; }
            public bool Hovered { get; set; }
            public bool MouseHit { get; set; } = false;
            public Rectangle Bounds { get; set; }
            public string SQLSelect { get; set; } = string.Empty;
            public string SQLDelete { get; set; } = string.Empty;
            public ListingData Type { get; set; }
            public void SetUserAccess(bool add, bool edit, bool update, bool delete)
            {
                AllowAdd = add;
                AllowEdit = edit;
                AllowEdit = update;
                AllowDelete = delete;
            }
            public bool AllowEdit { get; private set; }
            public bool AllowAdd { get; private set; }
            public bool AllowUpdate { get; private set; }
            public bool AllowDelete { get; private set; }
        }
        public class MenuCollection : Collection<Menu>
        {
            public MenuCollection(GroupMenu group) => Group = group;
            public GroupMenu Group { get; }
            public Menu Add(short id, string title)
            {
                var item = new Menu(Group) { Id = id, Title = title };
                this.Add(item);
                return item;
            }
        }
        #endregion Nested Class
        public SideBarPanel()
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint
                    | ControlStyles.UserPaint
                    | ControlStyles.ResizeRedraw
                    | ControlStyles.OptimizedDoubleBuffer, true);
            this.UpdateStyles();

            this.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular);
            this.BoldFont = new Font(this.Font, FontStyle.Bold);

            this.MouseWheel += SideBarPanel_MouseWheel;
        }

        public Color ScrollBarColor { get; set; } = Color.LightSkyBlue;
        public Color SelectedItemColor { get; set; } = Color.DarkGray;
        public Font BoldFont { get; set; }
        public int ItemHeight { get; set; } = 40;
        public int GroupHeight { get; set; } = 50;
        public GroupCollection Groups { get; } = new GroupCollection();

        private int _scrollOffsetY = 0;
        private int _contentHeight = 0;
        private Point _mousePoint = new Point(-1, -1);

        // scrollbar dragging
        private bool _dragging = false;
        private int _dragOffsetY;

        private int ScrollBarWidth => 6;
        private int GetContentHeight()
        {
            var h = 10;
            foreach (var group in this.Groups)
            {
                if (group.IsCollapsed)
                {
                    h += this.GroupHeight;
                }
                else
                {
                    h += this.GroupHeight + (group.Items.Count * this.ItemHeight);
                }
            }
            return h + 10;
        }
        public void RecalculateContent()
        {
            _contentHeight = this.GetContentHeight();
            if (_contentHeight <= this.ClientSize.Height)
            {
                _scrollOffsetY = 0;
            }

            var width = this.ClientSize.Width - 20;
            int x = 10, y = 10 - _scrollOffsetY;
            foreach (var group in this.Groups)
            {
                group.Bounds = new Rectangle(x, y + 2, width, this.GroupHeight - 4);
                y += this.GroupHeight;
                if (!group.IsCollapsed)
                {
                    foreach (var item in group.Items)
                    {
                        item.Bounds = new Rectangle(x, y + 1, width, this.ItemHeight - 2);
                        y += this.ItemHeight;
                    }
                }
            }
            this.Invalidate();
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            this._mousePoint = new Point(-1, -1);
            foreach (var g in this.Groups)
            {
                foreach (var i in g.Items)
                {
                    i.MouseHit = false;
                }
            }
            this.Invalidate();
        }
        public Menu? SelectedItem { get; private set; }
        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            if (this.BackgroundImage != null)
            {
                pevent.Graphics.DrawImage(this.BackgroundImage, pevent.ClipRectangle);
            }
            else
            {
                using (var brush = new SolidBrush(this.BackColor))
                {
                    pevent.Graphics.FillRectangle(brush, pevent.ClipRectangle);
                }
            }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            foreach (var g in Groups)
            {            
                //if (g.Bounds.Contains(_mousePoint)) e.Graphics.FillRectangle(Brushes.LightGreen, g.Bounds);
                if (g.Selected)
                {
                    var rect = new Rectangle(g.Bounds.X + g.Bounds.Width - 15, g.Bounds.Y + 11, 10, 10);
                    e.Graphics.FillEllipse(Brushes.LightYellow, rect);
                    e.Graphics.DrawEllipse(Pens.YellowGreen, rect);
                }
                // Group title (pakai TextRenderer)
                TextRenderer.DrawText(e.Graphics, g.Title.ToUpper(), this.BoldFont, g.Bounds, this.ForeColor, TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
                if (!g.IsCollapsed)
                {
                    foreach (var m in g.Items)
                    {
                        if (!m.Selected && m.MouseHit)
                        {
                            using (var selectedItemBrush = new SolidBrush(Color.FromArgb(100, Color.SkyBlue)))
                            {
                                e.Graphics.DrawRoundedRectangle(m.Bounds, 4, selectedItemBrush);
                            }
                        }
                        else if (m.Selected || m.Bounds.Contains(_mousePoint))
                        {
                            using (var selectedItemBrush = new SolidBrush(Color.FromArgb(100, Color.Black)))
                            {
                                e.Graphics.DrawRoundedRectangle(m.Bounds, 4, selectedItemBrush);
                            }
                        }

                        if (m.Selected)
                            e.Graphics.FillRectangle(
                                Brushes.LightSkyBlue,
                                new Rectangle(m.Bounds.X - 2, m.Bounds.Y + 6, 4, m.Bounds.Height - 12)
                            );

                        if (m.Image != null)
                        {
                            var imgPadding = (int)(m.Bounds.Height - 20) / 2;
                            var imgRect = new Rectangle(m.Bounds.X + 5, m.Bounds.Y + imgPadding, 20, 20);
                            e.Graphics.DrawImage(m.Image, imgRect);
                        }

                        var textRect = new Rectangle(m.Bounds.X + 30, m.Bounds.Y, m.Bounds.Width - 30, m.Bounds.Height);
                        TextRenderer.DrawText(e.Graphics, m.Title, this.Font, textRect, this.ForeColor, TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

                    }
                }
                else
                {
                    e.Graphics.DrawLine(Pens.Lavender, g.Bounds.X, g.Bounds.Y + g.Bounds.Height, g.Bounds.X + g.Bounds.Width, g.Bounds.Y + g.Bounds.Height);
                }
            }

            if (_contentHeight > this.Height)
            {
                if (e.ClipRectangle.Contains(_mousePoint))
                    DrawScrollBar(e.Graphics);
            }
        }
        private void DrawScrollBar(Graphics g)
        {
            float ratio = (float)this.Height / _contentHeight;
            int thumbHeight = Math.Max(20, (int)(this.Height * ratio));

            int maxScroll = Math.Max(1, _contentHeight - this.Height);
            int thumbY = (int)((float)_scrollOffsetY / maxScroll * (this.Height - thumbHeight));

            Rectangle thumbRect = new Rectangle(this.Width - ScrollBarWidth, thumbY, ScrollBarWidth, thumbHeight);
            using (Brush thumbBrush = new SolidBrush(this.ScrollBarColor))
                g.FillRectangle(thumbBrush, thumbRect);
        }

        private void SideBarPanel_MouseWheel(object? sender, MouseEventArgs e)
        {
            if (_contentHeight <= this.Height) return;

            int scrollAmount = this.ItemHeight;
            _scrollOffsetY = e.Delta < 0
                ? Math.Min(_scrollOffsetY + scrollAmount, _contentHeight - this.Height)
                : Math.Max(_scrollOffsetY - scrollAmount, 0);
            RecalculateContent();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (_contentHeight <= this.Height) return;

            float ratio = (float)this.Height / _contentHeight;
            int thumbHeight = Math.Max(20, (int)(this.Height * ratio));
            int maxScroll = Math.Max(1, _contentHeight - this.Height);

            int thumbY = (int)((float)_scrollOffsetY / maxScroll * (this.Height - thumbHeight));
            Rectangle thumbRect = new Rectangle(this.Width - ScrollBarWidth, thumbY, ScrollBarWidth, thumbHeight);

            if (thumbRect.Contains(e.Location))
            {
                _dragging = true;
                _dragOffsetY = e.Y - thumbRect.Y;
            }
            else
            {
                foreach (var g in this.Groups)
                {
                    foreach(var item in g.Items)
                    {
                        item.MouseHit = item.Bounds.Contains(e.Location);
                    }
                }
                Invalidate();
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (_dragging && _contentHeight > this.Height)
            {
                int thumbHeight = Math.Max(20, (int)(this.Height * (float)this.Height / _contentHeight));
                int newY = e.Y - _dragOffsetY;

                newY = Math.Max(0, Math.Min(newY, this.Height - thumbHeight));

                float ratio = (float)newY / (this.Height - thumbHeight);
                _scrollOffsetY = (int)(ratio * (_contentHeight - this.Height));
                RecalculateContent();
            }
            else
            {
                this._mousePoint = e.Location;
                Invalidate();
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            _dragging = false;

            var loc = e.Location;
            Menu? selectedItem = null;
            this.SelectedItem = null;
            foreach (var group in this.Groups)
            {
                if (group.Bounds.Contains(loc))
                {
                    if (group.IsCollapsed)
                    {
                        group.Expand();
                        RecalculateContent();
                        return;
                    }
                    else
                    {
                        group.Collapse();
                        RecalculateContent();
                        return;
                    }
                }
                foreach (var item in group.Items)
                {
                    if (item.Selected)
                    {
                        selectedItem = item;
                    }
                    else
                    {
                        if (!group.IsCollapsed && item.Bounds.Contains(loc) && item.MouseHit)
                        {
                            item.Selected = true;
                            this.SelectedItem = item;
                        }
                    }
                }
            }
            if (this.SelectedItem != null)
            {
                this.SelectedItem.Group.Selected = true;
                if (selectedItem != null)
                {
                    if (!selectedItem.Group.Equals(this.SelectedItem.Group))
                    {
                        selectedItem.Group.Selected = false;
                    }
                    selectedItem.Selected = false;
                    if (OnItemClicked != null)
                    {
                        OnItemClicked.Invoke(SelectedItem);
                    }
                }
                this.Invalidate();
            }
        }
        public Func<Menu, Task>? OnItemClicked = null;
    }
}
