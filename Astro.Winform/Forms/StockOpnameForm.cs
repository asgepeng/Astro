using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Astro.Winform.Forms
{
    public partial class StockOpnameForm : UserControl
    {
        public StockOpnameForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var commandText = """
                SELECT p.product_id, p.product_name, p.product_sku, c.category_name, p.stock, u.unit_name, p.price
                FROM products AS p
                INNER JOIN categories As c ON p.category_id = c.category_id
                INNER JOIN units AS u ON p.unit_id = u.unit_id
                WHERE p.is_deleted = false
                """;
            var form = new ListingPopUpForm(commandText)
            {
                Size = new Size(800, 400)
            };
            form.AddColumn("Product Name", "product_name", 230);
            form.AddColumn("SKU", "product_sku", 100);
            form.AddColumn("Category", "category_name", 150);
            form.AddColumn("Stock", "stock", 100, DataGridViewContentAlignment.MiddleRight);
            form.AddColumn("Unit", "unit_name", 100);
            form.AddColumn("Price", "price", 100, DataGridViewContentAlignment.MiddleRight, "N0");
            if (form.ShowDialog() == DialogResult.OK)
            {
                this.textBox2.Text = form.SelectedRow[1].ToString();
                this.textBox3.Focus();
            }
        }
    }
}
