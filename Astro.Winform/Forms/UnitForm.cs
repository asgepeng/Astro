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
    public partial class UnitForm : Form
    {
        public UnitForm()
        {
            InitializeComponent();
        }
        public Unit? Unit { get; set; }

        private void HandleFormLoad(object sender, EventArgs e)
        {
            if (Unit != null)
            {
                this.textBox1.Text = Unit.Name;
            }
        }

        private async void HandleSaveButtonClicked(object sender, EventArgs e)
        {
            if (Unit is null) return;
            if (string.IsNullOrWhiteSpace(this.textBox1.Text))
            {
                MessageBox.Show("Unit name cannot be empty.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Unit.Name = this.textBox1.Text.Trim();
            var json = Unit.Id > 0 ? await WClient.PutAsync("/data/units", Unit.ToString()) : await WClient.PostAsync("/data/units", Unit.ToString());
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
