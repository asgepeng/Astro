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
    public partial class WebPageForm : Form
    {
        public WebPageForm()
        {
            InitializeComponent();
        }

        private async void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            await webView21.EnsureCoreWebView2Async();
            webView21.CoreWebView2.Navigate("https://chatgpt.com/");
        }
    }
}
