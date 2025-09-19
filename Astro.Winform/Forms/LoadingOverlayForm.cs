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
    public partial class LoadingOverlayForm : Form
    {
        public LoadingOverlayForm(Form form)
        {
            InitializeComponent();
            this.Size = form.Size;
            this.Location = form.Location;
        }
    }
}
