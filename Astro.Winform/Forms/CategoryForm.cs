using Astro.Models;
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
    public partial class CategoryForm : Form
    {
        public CategoryForm()
        {
            InitializeComponent();
        }
        public Category? Category { get; set; }

        private void HandleFormLoad(object sender, EventArgs e)
        {
            if (Category != null)
            {
                this.textBox1.Text = Category.Name;
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            if (Category is null) return;
            if (string.IsNullOrWhiteSpace(this.textBox1.Text))
            {
                MessageBox.Show("Category name cannot be empty.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Category.Name = this.textBox1.Text.Trim();
            var json = Category.Id > 0 ? await WClient.PutAsync("/data/categories", Category.ToString()) : await WClient.PostAsync("/data/categories", Category.ToString());
            var commondResult = CommonResult.Create(json);
            if (commondResult != null)
            {
                if (commondResult.Success)
                {
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show(commondResult.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
