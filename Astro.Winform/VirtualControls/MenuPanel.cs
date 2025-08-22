using Astro.Drawing.Extensions;
using Astro.Models;
using PointOfSale.Drawing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astro.Forms.Controls
{
    internal class MenuPanel : VirtualControl
    {
        private int _selectedIndex = -1;
        internal override void Draw(Graphics g)
        {
            if (this.SelectedSection != null)
            {
                var section = this.SelectedSection;
                var titleRect = new Rectangle(Bounds.X + 5, Bounds.Y, Bounds.Width - 10, 40);
                var sf = new StringFormat() { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
                TextRenderer.DrawText(
                    g,
                    section.Title,
                    this.TitleFont,
                    titleRect,
                    Color.Black,
                    TextFormatFlags.Default | TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak
                );

                using (var pen = new Pen(new SolidBrush(Color.Gainsboro)))
                {
                    foreach (var item in this.SelectedSection.Items)
                    {
                        if (item.Selected)
                        {
                            g.DrawRoundedRectangle(new Rectangle(new Point(item.Bounds.X + 1, item.Bounds.Y + 1), item.Bounds.Size), 10, Brushes.Silver);
                            g.DrawRoundedRectangle(item.Bounds, 10, Brushes.White);
                        }
                        else
                        {
                            if (item.Hovered)
                                g.DrawRoundedRectangle(item.Bounds, 10, Brushes.Silver);
                        }
                        g.DrawImage(global::Astro.Winform.Properties.Resources.icons8_product_24, item.Bounds.X + 12, item.Bounds.Y + 12, 16F, 16F);
                        g.DrawString(item.Title, this.Font, Brushes.Black, new Rectangle(item.Bounds.X + 40, item.Bounds.Y, item.Bounds.Width - 40, item.Bounds.Height), sf);
                    }
                }
            }
        }
        internal int Width { get; set; } = 200;
        internal Font TitleFont { get; set; } = new Font("Segoe UI", 15.75F, FontStyle.Regular);
        internal Font Font { get; set; } = new Font("Segoe UI", 9.75F, FontStyle.Regular);
        internal Rectangle Bounds { get; private set; }
        internal ListSection Items { get; set; } = new ListSection();
        internal int SelectedIndex
        {
            get
            {
                return _selectedIndex;
            }
            set
            {
                if (value >=0 && value < this.Items.Count)
                {
                    this._selectedIndex = value;
                    this.SelectedSection = this.Items[_selectedIndex];
                }
                else
                {
                    _selectedIndex = -1;
                    SelectedSection = null;
                }
            }
        }
        internal Section? SelectedSection { get; private set; } = null;
        internal void MainFormResize(Size parentSize)
        {
            this.Bounds = new Rectangle(48, 48, Width, parentSize.Height - 48);
        }
        internal void OnMouseMove(Point location)
        {
            if (this.SelectedSection != null)
            {
                foreach (var item in this.SelectedSection.Items)
                {
                    item.Hovered = item.Bounds.Contains(location);
                }
            }
        }
        internal Menu? GetClickedItem(Point location)
        {
            if (this.SelectedSection is null) return null;

            int selectedIndex = -1, currentSelectedIndex = -1;
            for (int i = 0; i < this.SelectedSection.Items.Count; i++)
            {
                if (this.SelectedSection.Items[i].Selected)
                {
                    selectedIndex = i;
                }
                if (this.SelectedSection.Items[i].Bounds.Contains(location))
                {
                    if (!this.SelectedSection.Items[i].Selected)
                    {
                        currentSelectedIndex = i;
                    }
                }
            }
            if (currentSelectedIndex == -1) return null;
            if (selectedIndex == currentSelectedIndex) return null;

            if (selectedIndex >=0) this.SelectedSection.Items[selectedIndex].Selected = false;
            this.SelectedSection.Items[currentSelectedIndex].Selected = true;
            return this.SelectedSection.Items[currentSelectedIndex];
        }
        internal Menu? GetSelectedItem()
        {
            if (this.SelectedSection is null) return null;
            foreach (var item in this.SelectedSection.Items)
            {
                if (item.Selected) return item;
            }
            return null;
        }
    }

    internal class XMenuItem : VirtualControl
    {
        internal string Text { get; set; } = "";
        internal Rectangle Bounds { get; set; }
        internal bool Hovered { get; set; } = false;
        internal Image Image { get; set; } = global::Astro.Winform.Properties.Resources.data;
        internal bool Selected { get; set; } = false;
        internal override void Draw(Graphics g)
        {
            using (var pen = new Pen(new SolidBrush(Color.Gainsboro)))
            {
                g.DrawRectangle(pen, Bounds);
            }
        }
    }
}
