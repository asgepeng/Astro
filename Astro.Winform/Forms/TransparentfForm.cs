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
    public partial class TransparentfForm : Form
    {
        public TransparentfForm(Form owner)
        {
            InitializeComponent();
            this.Location = owner.Location;
            this.Size = owner.Size;
        }
        public Control? ControlToDisplay { get; set; }

        private void timer1_Tick(object sender, EventArgs e)
        {

        }

        private void TransparentfForm_Load(object sender, EventArgs e)
        {
            if (this.ControlToDisplay != null)
            {
                Task.Run(() =>
                {
                    var overlay = new OverlayForm(this);
                    overlay.Controls.Add(this.ControlToDisplay);
                    this.DialogResult = overlay.ShowDialog(this);
                    this.Close();
                });
            }
            else
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }
    }
}
