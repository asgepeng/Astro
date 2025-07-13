using Astro.Models;
using Astro.ViewModels;
using Astro.Winform.Classes;
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
    public partial class ProductForm : Form
    {
        public ProductForm()
        {
            InitializeComponent();
        }
        public ProductViewModel? Model { get; set; } = null;

        private void ProductForm_Load(object sender, EventArgs e)
        {
            if (this.Model != null)
            {
                this.categoryComboBox.DataSource = this.Model.Categories;
                this.unitComboBox.DataSource = this.Model.Units;
                if (this.Model.Product != null)
                {
                    this.skuTextBox.Text = this.Model.Product.Sku;
                    this.nameTextBox.Text = this.Model.Product.Name;
                    this.descriptionTextBox.Text = this.Model.Product.Description;
                    this.stockTextBox.Text = this.Model.Product.Stock.ToString();
                    this.basicpriceTextBox.Text = this.Model.Product.CostAverage.ToString();
                    this.priceTextBox.Text = this.Model.Product.Price.ToString();
                    this.minstockTextBox.Text = this.Model.Product.MinStock.ToString();
                    this.maxstockTextBox.Text = this.Model.Product.MaxStock.ToString();
                    this.categoryComboBox.SelectedItem = this.Model.Categories.FirstOrDefault(c => c.Id == this.Model.Product.Category);
                    this.unitComboBox.SelectedItem = this.Model.Units.FirstOrDefault(u => u.Id == this.Model.Product.Unit);
                }
            }
            this.unitComboBox.DisplayMember = "Text";
            this.unitComboBox.ValueMember = "Id";

            this.categoryComboBox.DisplayMember = "Text";
            this.categoryComboBox.ValueMember = "Id";
        }

        private async void loginButton_Click(object sender, EventArgs e)
        {
            if (this.Model is null) return;

            if (string.IsNullOrWhiteSpace(this.nameTextBox.Text))
            {
                MessageBox.Show("Product name is required", "Product name", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                nameTextBox.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(this.skuTextBox.Text))
            {
                MessageBox.Show("Sku is required", "Sku", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                skuTextBox.Focus();
                return;
            }
            if (categoryComboBox.SelectedItem is null)
            {
                MessageBox.Show("Product category is not selected", "Category", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                categoryComboBox.Focus();
                return;
            }
            if (unitComboBox.SelectedItem is null)
            {
                MessageBox.Show("Unit not yet selected", "Unit", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                unitComboBox.Focus();
                return;                
            }

            var product = this.Model.Product != null ? this.Model.Product : new Product();
            product.Name = this.nameTextBox.Text.Trim();
            product.Description = this.descriptionTextBox.Text.Trim();
            product.Sku  = this.skuTextBox.Text.Trim();
            product.Category = (short)((Option)this.categoryComboBox.SelectedItem).Id;
            product.Unit = (short)((Option)this.unitComboBox.SelectedItem).Id;
            int.TryParse(stockTextBox.Text, out int productStock);
            product.Stock = productStock;
            long.TryParse(priceTextBox.Text, out long productPrice);
            product.Price = productPrice;
            long.TryParse(basicpriceTextBox.Text, out long productBasicPrice);
            product.CostAverage = productBasicPrice;
            short.TryParse(minstockTextBox.Text, out short minStock);
            short.TryParse(maxstockTextBox.Text, out short maxStock);
            product.MinStock = minStock;
            product.MaxStock = maxStock;

            var result = product.ID > 0 ? await HttpClientSingleton.PutAsync("/data/products", product.ToString()) : await HttpClientSingleton.PostAsync("/data/products", product.ToString());
            var commonResult = CommonResult.Create(result);
            if (commonResult != null)
            {
                if (commonResult.Success)
                {
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show(commonResult.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
