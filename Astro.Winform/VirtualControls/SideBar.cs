using Astro.Binaries;
using Astro.Drawing.Extensions;
using Astro.Models;
using Astro.Winform.Controls;
using Astro.Winform.Extensions;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Data;
using Astro.Data;
using DocumentFormat.OpenXml.Vml.Spreadsheet;

namespace Astro.Forms.Controls
{
    internal class SideBar : VirtualControl
    {
        internal bool IsDragging { get; private set; } = false;
        internal bool Pressed { get; private set; }

        private int _dragOffsetY;
        public SideBar()
        {
            Groups = new SideBarItemGroupCollection(this);
            this.HoveredColor = Color.FromArgb(128, Color.White);
        }
        internal Point Location
        {
            get { return Bounds.Location; }
            set { Bounds = new Rectangle(value, this.Bounds.Size); }
        }
        internal Size Size
        {
            get { return Bounds.Size; }
            set { Bounds = new Rectangle(this.Bounds.Location, value); }
        }
        internal int Width
        {
            get { return Bounds.Width; }
            set { Bounds = new Rectangle(Bounds.X, Bounds.Y, value, Bounds.Height); }
        }
        internal int Height
        {
            get { return Bounds.Height; }
            set { Bounds = new Rectangle(Bounds.X, Bounds.Y, Bounds.Width, value); }
        }
        internal Font Font { get; set; } = new Font("Segoe UI", 9.75F, FontStyle.Regular);
        internal Font GroupFont { get; set; } = new Font("Segoe UI", 9.75F, FontStyle.Bold);
        internal Color ForeColor { get; set; } = SystemColors.Window;
        internal int ItemHeight { get; set; } = 32;
        internal Padding Padding { get; set; } = new Padding(8);
        internal Size ContentSize { get; private set; }
        internal Point ScrollOffset { get; set; }
        internal Color ScrollBarColor { get; set; } = Color.SkyBlue;
        internal Rectangle Bounds { get; private set; }
        internal Color BackColor { get; set; } = Color.Navy;
        internal Color HoveredColor { get; set; }
        internal Color SelectedColor { get; set; }
        internal bool ScrollBarVisible { get; set; }
        internal void DrawBackground(Graphics g)
        {
            using (var brush = new SolidBrush(this.BackColor))
            {
                g.FillRectangle(brush, this.Bounds);
            }
        }
        internal override void Draw(Graphics g)
        {
            foreach (var item in Groups)
            {
                item.Draw(g);
            }
            if (ScrollBarVisible) DrawScrollBar(g);
        }
        private void DrawScrollBar(Graphics g)
        {
            var thumbRect = GetThumbRectangle();
            using (Brush thumbBrush = new SolidBrush(this.ScrollBarColor))
            {
                g.FillRectangle(thumbBrush, thumbRect);
            }
        }
        internal SideBarItem? SelectedItem { get; private set; }
        internal SideBarItemGroupCollection Groups { get; }
        internal async Task LoadMenuAsync(IDBClient db)
        {
            My.Application.User = new UserInfo(1, "Administrator", new Role() { Id = 1, Name = "Admin" });
            var home = this.Groups.Add("HOME");
            var dashboard = home.Items.Add(0, "Dashboard");
            dashboard.Image = """
                    <svg width="24" height="24" viewBox="0 0 24 24" fill="none"
                         xmlns="http://www.w3.org/2000/svg">
                      <rect x="3"  y="3"  width="8" height="8" rx="2" stroke="#fff" stroke-width="2"/>
                      <rect x="13" y="3"  width="8" height="5" rx="2" stroke="#fff" stroke-width="2"/>
                      <rect x="13" y="10" width="8" height="11" rx="2" stroke="#fff" stroke-width="2"/>
                      <rect x="3"  y="13" width="8" height="8" rx="2" stroke="#fff" stroke-width="2"/>
                    </svg>                
                    """.ToImage();
            dashboard.Selected = true;
            this.SelectedItem = dashboard;

            var commandText = """
                    SELECT s.sectionid, s.title, m.menuid, m.title AS menu_title, m.icon, rtm.allowcreate, rtm.allowread, rtm.allowupdate, rtm.allowdelete
                    FROM rolemenus AS rtm 
                    INNER JOIN menus m ON rtm.menuid = m.menuid 
                    INNER JOIN sections s ON m.sectionid = s.sectionid
                    WHERE m.disabled = false AND rtm.roleid = @roleId
                    ORDER BY s.sectionid, m.menuid
                    """;
            await db.ExecuteReaderAsync(async (DbDataReader reader) =>
            {
                SideBarItemGroup? group = null;

                var currentSectionId = 0;

                while (await reader.ReadAsync())
                {
                    var sectionId = reader.GetInt16(0);
                    if (sectionId != currentSectionId || group is null)
                    {
                        currentSectionId = sectionId;
                        group = Groups.Add(reader.GetString(1).Replace("&", "&&"));
                    }
                    var menu = group.Items.Add(reader.GetInt16(2), reader.GetString(3));
                    //menu.SetUserAccess(reader.GetBoolean(5), reader.GetBoolean(6), reader.GetBoolean(7), reader.GetBoolean(8));
                    var svg = reader.GetString(4);
                    if (!string.IsNullOrWhiteSpace(svg))
                    {
                        menu.Image = svg.ToImage();
                    }
                    switch (menu.ID)
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
            this.ArrangeLayout();
        }
        internal SideBarItem? HoveredItem { get; private set; }
        internal SideBarItem? PressedItem { get; private set; }
        internal SideBarItem? RelesedItem { get; private set; }
        internal void Resize(Size mainFormSize)
        {
            this.Height = mainFormSize.Height;
            this.ArrangeLayout();
        }
        internal void ArrangeLayout()
        {
            var x = this.Location.X + Padding.Left;
            var y = this.Location.Y + Padding.Top - this.ScrollOffset.Y;
            var w = this.Width - Padding.Left - Padding.Right;
            var h = this.ItemHeight;
            foreach (var group in this.Groups)
            {
                group.Bounds = new Rectangle(x, y, w, 48);
                y += 48;
                if (group.Expanded)
                {
                    foreach (var item in group.Items)
                    {
                        item.Bounds = new Rectangle(x, y, w, h);
                        y += this.ItemHeight;
                    }
                }
            }
            y += Padding.Bottom + this.ScrollOffset.Y;
            this.ContentSize = new Size(this.Width, y);
        }
        private Rectangle GetThumbRectangle()
        {
            int ScrollBarWidth = 8;
            float ratio = (float)this.Height / this.ContentSize.Height;
            int thumbHeight = Math.Max(20, (int)(this.Height * ratio)) - this.Bounds.Y - 28;

            int maxScroll = Math.Max(1, this.ContentSize.Height - this.Height);
            int thumbY = (int)((float)(this.ScrollOffset.Y) / maxScroll * (this.Height - thumbHeight));
            thumbY += this.Bounds.Y;

            return new Rectangle(this.Width - ScrollBarWidth, thumbY, ScrollBarWidth, thumbHeight);
        }
        internal void MouseWheel(MouseEventArgs e)
        {
            if (this.ContentSize.Height <= this.Height) return;

            int scrollAmount = this.ItemHeight;
            int x = 0, y = this.ScrollOffset.Y;
            y = e.Delta < 0
                ? Math.Min(this.ScrollOffset.Y + scrollAmount, this.ContentSize.Height - this.Height)
                : Math.Max(this.ScrollOffset.Y - scrollAmount, 0);
            this.ScrollOffset = new Point(x, y);
            this.ArrangeLayout();
            foreach (var group in this.Groups)
            {
                group.Hovered = group.Bounds.Contains(e.Location);
                foreach (var item in group.Items)
                {
                    item.Hovered = item.Bounds.Contains(e.Location);
                }
            }
        }
        internal bool MouseDown(MouseEventArgs e)
        {
            if (this.ContentSize.Height <= this.Height) return false;

            var thumbRect = GetThumbRectangle();
            if (thumbRect.Contains(e.Location))
            {
                this.IsDragging = true;
                _dragOffsetY = e.Y - thumbRect.Y;
                return true;
            }
            else
            {
                bool anyPressed = false;
                foreach (var group in this.Groups)
                {
                    if (group.Bounds.Contains(e.Location))
                    {
                        group.Pressed = true;
                        anyPressed = true;
                    }
                    foreach (var item in group.Items)
                    {
                        if (!item.Selected && item.Bounds.Contains(e.Location))
                        {
                            item.Pressed = true;
                            anyPressed = true;
                        }                        
                    }
                }
                return anyPressed;
            }
        }
        internal void MouseUp(MouseEventArgs e) 
        {
            if (IsDragging) IsDragging = false;
            if (Pressed) Pressed = false;
        }
        internal SideBarItem? ItemClicked(MouseEventArgs e)
        {
            SideBarItem? sideBarItem = null;
            foreach (var group in this.Groups)
            {
                if (group.Pressed && group.Bounds.Contains(e.Location))
                {
                    group.Pressed = false;
                }
                foreach (var item in group.Items)
                {
                    if (item.Pressed && item.Bounds.Contains(e.Location) && group.Expanded)
                    {
                        if (SelectedItem != null && item == SelectedItem) return null;
                        else
                        {
                            if (SelectedItem != null) SelectedItem.Selected = false;
                            item.Selected = true;
                            SelectedItem = item;
                            sideBarItem = SelectedItem;
                        }
                    }
                    item.Pressed = false;
                }
            }
            Pressed = false;
            return sideBarItem;
        }
        internal bool MouseMove(MouseEventArgs e)
        {
            this.ScrollBarVisible = true;
            if (IsDragging && this.ContentSize.Height > this.Height)
            {
                int thumbHeight = Math.Max(20, (int)(this.Height * (float)this.Height / this.ContentSize.Height));
                int newY = e.Y - _dragOffsetY;

                newY = Math.Max(0, Math.Min(newY, this.Height - thumbHeight));

                float ratio = (float)newY / (this.Height - thumbHeight);
                int y = (int)(ratio * (this.ContentSize.Height - this.Height));
                this.ScrollOffset = new Point(0, y);
                ArrangeLayout();
                return true;
            }
            else
            {
                bool anyHovered = false;
                SideBarItem? hovered = null;
                SideBarItem? currentHovered = null;
                foreach (var group in this.Groups)
                {
                    group.Hovered = group.Bounds.Contains(e.Location);
                    if (group.Hovered) anyHovered = true;

                    foreach (var item in group.Items)
                    {
                        if (item.Hovered) hovered = item;
                        item.Hovered = item.Bounds.Contains(e.Location);
                        if (item.Hovered)
                        {
                            currentHovered = item;
                            anyHovered = true;
                        }
                    }
                }
                if (hovered is null && currentHovered != null) return true;
                if (hovered != null && currentHovered != null && hovered == currentHovered) return false;
                return anyHovered;
            }
        }
        internal void MouseLeave()
        {
            this.Pressed = false;
            foreach (var group in this.Groups)
            {
                group.Hovered = false;
                foreach (var item in group.Items)
                {
                    item.Hovered = false;
                    item.Pressed = false;
                }
            }
            this.ScrollBarVisible = false;
        }
        internal void PerformClick()
        {

        }
        internal Action<SideBarItem>? OnItemClick = null;
    }

    internal class SideBarItemGroup : VirtualControl
    {
        internal SideBar SideBar { get; }
        internal string Title { get; set; }
        internal Rectangle Bounds { get; set; }
        internal bool Hovered { get; set; }
        internal bool Selected { get; set; }
        internal bool Pressed { get; set; }
        internal SideBarItemCollection Items { get; }
        internal bool Expanded { get; set; } = true;
        internal SideBarItemGroup(SideBar owner, string title)
        {
            this.SideBar = owner;
            this.Title = title;
            this.Items = new SideBarItemCollection(this);
        }
        internal override void Draw(Graphics g)
        {
            if (this.Hovered)
            {
                var rect = new Rectangle(this.Bounds.Right - 16 - this.SideBar.Padding.Right, this.Bounds.Top + 16, 16, 16);
                g.DrawImage(global::Astro.Winform.Properties.Resources.right_arrow, rect);
            }
            TextRenderer.DrawText(g, this.Title, this.SideBar.GroupFont, this.Bounds, this.SideBar.ForeColor, TextFormatFlags.VerticalCenter |TextFormatFlags.EndEllipsis);
            if (this.Expanded)
            {
                foreach (var item in this.Items)
                {
                    var wh = item.Bounds.Height - 10;
                    var imgRect = new Rectangle(item.Bounds.Left + 6, item.Bounds.Top + 5, wh, wh);
                    if (item.Hovered || item.Selected || item.Pressed)
                    {
                        var color = item.Pressed ? Color.Orange : this.SideBar.HoveredColor;
                        using (var brush = new SolidBrush(color))
                        {
                            g.FillRectangle(brush, item.Bounds);
                        }
                        if (item.Selected)
                        {
                            using (var pen = new Pen(Brushes.SkyBlue, 4))
                            {
                                g.DrawLine(pen, item.Bounds.Left, item.Bounds.Top + 3, item.Bounds.Left, item.Bounds.Bottom - 3);
                            }
                        }
                    }

                    if (item.Image != null) g.DrawImage(item.Image, imgRect);

                    var textRect = new Rectangle(item.Bounds.Left + wh + 15, item.Bounds.Y, item.Bounds.Width - wh - 10, item.Bounds.Height);
                    TextRenderer.DrawText(g, item.Title, this.SideBar.Font, textRect, this.SideBar.ForeColor, TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
                }
            }
        }
    }
    internal class SideBarItemGroupCollection : Collection<SideBarItemGroup>
    {
        private SideBar SideBar { get; }
        internal SideBarItemGroupCollection(SideBar parent)
        {
            SideBar = parent;
        }
        internal SideBarItemGroup Add(string text)
        {
            var group = new SideBarItemGroup(this.SideBar, text);
            Add(group);
            return group;
        }
    }
    internal class SideBarItem
    {
        internal SideBarItem(short id, string title, Image? image = null)
        {
            ID = id;
            Title = title;
        }
        internal short ID { get; }
        internal string Title { get; }
        internal Image? Image { get; set; }
        internal Image? SmallImage { get; set; }
        internal bool Hovered { get; set; }
        internal bool Pressed { get; set; }
        internal bool Selected { get; set; }
        internal Rectangle Bounds { get; set; }
        internal void ReverseHover() => Hovered = !Hovered;

        internal ListingData Type;
        internal string SQLSelect { get; set; } = "";
        internal string SQLDelete { get; set; } = "";
    }
    internal class SideBarItemCollection : Collection<SideBarItem>
    {
        public SideBarItemCollection(SideBarItemGroup parent)
        {
            Group = parent;
        }
        internal SideBarItemGroup Group { get; }
        internal SideBarItem Add(short id, string title, Image? image = null)
        {
            var item = new SideBarItem(id, title, image);
            Add(item);
            return item;
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
