using Astro.Models.Transactions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Astro.Winform.Forms
{
    public partial class CostsForm : Form
    {
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HTCAPTION = 0x2;

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        private void HandleMouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(this.Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
            }
        }
        public CostsForm()
        {
            InitializeComponent();
            MouseDown += HandleMouseDown;
        }
        public CostCollection Costs { get; } = new CostCollection();
        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            foreach (var item in this.Costs)
            {
                if (string.IsNullOrWhiteSpace(item.Name) || item.Value <= 0)
                {
                    MessageBox.Show("Keterangan tidak boleh kosong, nilai tidak boleh lebih kecil atau sama dengan nol", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            costBindingSource.AddNew();
        }

        private void CostsForm_Load(object sender, EventArgs e)
        {
            this.costBindingSource.DataSource = this.Costs;
        }
    }
}
