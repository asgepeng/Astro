using Astro.Drawing.Extensions;

namespace Astro.Forms.Controls
{
    internal class SideBar : VirtualControl
    {
        int _selectedIndex = -1;
        public SideBar()
        {
            Bounds = new Rectangle(0, 48, 48, 0);
            Items = new SideBarItemCollection(this);
        }
        internal Rectangle Bounds { get; private set; }
        internal Color BackColor { get; set; } = Color.FromArgb(244, 244, 244);
        internal int SelectedIndex
        {
            get { return _selectedIndex; }
            set { _selectedIndex = value; Items[_selectedIndex].Selected = true; }
        }
        internal override void Draw(Graphics g)
        {
            foreach (var item in Items)
            {
                item.Draw(g);
            }
        }
        internal SideBarItemCollection Items { get; }
        internal void MainFormResize(Size clientSize)
        {
            Bounds = new Rectangle(0, 48, 48, clientSize.Height - 48);
        }
        internal SideBarItem? GetHoveredItem(Point p)
        {
            foreach (var item in Items)
            {
                if (item.Hovered != item.Bounds.Contains(p))
                {
                    item.ReverseHover();
                    return item;
                }
            }
            return null;
        }
        internal SideBarItem? GetSelectedItem(Point p)
        {
            int selectedIndex = -1, currentSelectedIndex = -1;
            for (int i=0; i< this.Items.Count; i++)
            {
                if (Items[i].Selected)
                {
                    selectedIndex = i;
                }
                if (Items[i].Bounds.Contains(p))
                {
                    if (!Items[i].Selected)
                    {
                        currentSelectedIndex = i;
                    }
                }
            }
            if (currentSelectedIndex == -1) return null;
            if (selectedIndex == currentSelectedIndex) return null;
            
            Items[selectedIndex].Selected = false;
            Items[currentSelectedIndex].Selected = true;
            return Items[currentSelectedIndex];
        }
    }

    internal class SideBarItem : VirtualControl
    {
        internal SideBarItem(string text, Image image, Point location, int index)
        {
            Text = text;
            Image = image;
            Description = "";
            Bounds = new Rectangle(location, new Size(32, 32));
            Hovered = false;
            Index = index;
        }
        internal string Text { get; }
        internal string Description { get; set; }
        internal Image Image { get; }
        internal int Index { get; }
        internal bool Hovered { get; private set; }
        internal bool Selected { get; set; }
        internal Rectangle Bounds { get; set; }
        internal void ReverseHover() => Hovered = !Hovered;
        internal override void Draw(Graphics g)
        {
            if (Hovered || Selected)
            {
                g.DrawRoundedRectangle(Bounds, 10, new SolidBrush(Color.FromArgb(220, 230, 240)));
            }
            if (Selected)
            {
                g.FillRectangle(Brushes.Red, new Rectangle(Bounds.X, Bounds.Y + 10, 3, 12));
            }
            g.DrawImage(Image, Bounds.X + 6, Bounds.Y + 6, 20, 20);
        }
    }
    internal class SideBarItemCollection : List<SideBarItem>
    {
        public SideBarItemCollection(SideBar owner)
        {
            Parent = owner;
        }
        internal SideBar Parent { get; }
        internal void Add(string text, Image image)
        {
            int x = Parent.Bounds.X + 8, y = Parent.Bounds.Y + 8;
            if (Count > 0)
            {
                int lastIndex = Count - 1;
                y = this[lastIndex].Bounds.Y + this[lastIndex].Bounds.Height + 4;
            }
            var item = new SideBarItem(text, image, new Point(x, y), this.Count);
            Add(item);
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
