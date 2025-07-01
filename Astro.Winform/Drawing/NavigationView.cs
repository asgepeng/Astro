using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PointOfSale.Drawing
{
    internal class NavigationView : VirtualControl
    {
        int _selectedIndex = -1;
        public NavigationView()
        {
            Bounds = new Rectangle(0, 48, 48, 0);
            Items = new NavigationItemCollection(this);
        }
        internal Rectangle Bounds { get; private set; }
        internal Color BackColor { get; set; } = Color.FromArgb(244, 244, 244);
        internal int SelectedIndex
        {
            get { return _selectedIndex; }
            set { _selectedIndex = value; this.Items[_selectedIndex].Selected = true; }
        }
        internal override void Draw(Graphics g)
        {
            g.FillRectangle(new SolidBrush(BackColor), Bounds);
            g.DrawLine(Pens.LightGray, Bounds.X + Bounds.Width, Bounds.Y, Bounds.X + Bounds.Width, Bounds.Y + Bounds.Height);
            foreach (var item in this.Items)
            {
                item.Draw(g);
            }
        }
        internal NavigationItemCollection Items { get; }
        internal void MainFormResize(Size clientSize)
        {
            Bounds = new Rectangle(0, 48, 48, clientSize.Height - 48);
        }
        internal NavigationItem? GetHoveredItem(Point p)
        {
            foreach (var item in this.Items)
            {
                if (item.Hovered != item.Bounds.Contains(p))
                {
                    item.ReverseHover();
                    return item;
                }
            }
            return null;
        }
        internal bool SelectedChanged(Point p)
        {
            var anySelected = false;
            foreach (var item in this.Items)
            {
                item.Selected = item.Bounds.Contains(p);
                if (item.Selected) anySelected = true;
            }
            return anySelected;
        }
    }

    internal class NavigationItem : VirtualControl
    {
        internal NavigationItem(string text, Image image, Point location)
        {
            Text = text;
            Image = image;
            Description = "";
            Bounds = new Rectangle(location, new Size(32, 32));
            Hovered = false;
        }
        internal string Text { get; }
        internal string Description { get; set; }
        internal Image Image { get; }
        internal bool Hovered { get; private set; }
        internal bool Selected { get; set; }
        internal Rectangle Bounds { get; set; }
        internal void ReverseHover() => Hovered = !Hovered;
        internal override void Draw(Graphics g)
        {
            if (Hovered || Selected)
            {
                DrawingHelper.DrawRoundedRectangle(g, Bounds, 8, Brushes.Silver);
            }
            if (Selected)
            {
                g.FillRectangle(Brushes.Blue, new Rectangle(Bounds.X, Bounds.Y + 10, 3, 12));
            }
            g.DrawImage(this.Image, Bounds.X + 6, Bounds.Y + 6, 20, 20);
        }
    }
    internal class NavigationItemCollection : List <NavigationItem>
    {
        public NavigationItemCollection(NavigationView owner)
        {
            Parent = owner;
        }
        internal NavigationView Parent { get; }
        internal void Add(string text, Image image)
        {
            int x = Parent.Bounds.X + 8, y = Parent.Bounds.Y + 8;
            if (this.Count > 0)
            {
                int lastIndex = this.Count - 1;
                y = this[lastIndex].Bounds.Y + this[lastIndex].Bounds.Height + 4;
            }
            var item = new NavigationItem(text, image, new Point(x, y));
            base.Add(item);
        }
        internal bool AnyHoveredItem()
        {
            foreach (var item in this)
            {
                if (item.Hovered) return true;
            }
            return false;
        }
    }
}
