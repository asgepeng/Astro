using Astro.Cryptography;
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
    public partial class SqlExecuteForm : Form
    {
        public SqlExecuteForm()
        {
            InitializeComponent();
        }

        private async void richTextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.F5)
            {
                var commandText = richTextBox1.Text.Trim();
                var guid = Guid.NewGuid();
                var key = guid.ToByteArray();
                var encrypted = SimpleEncryption.Encrypt(commandText, key);
                using (var writer = new IO.Writer())
                {
                    writer.WriteGuid(guid);
                    writer.WriteString(encrypted);

                    using (var stream = await HttpClientSingleton.PostStreamAsync("/api/sql", writer.ToArray()))
                    using (var reader = new IO.Reader(stream))
                    {
                        var result = reader.ReadByte();
                        if (result == 1)
                        {
                            var table = reader.ReadDataTable();
                            dataGridView1.DataSource = table;
                        }
                        else if (result == 2)
                        {
                            var message = reader.ReadString();
                            MessageBox.Show(message);
                        }
                    }
                }
            }
        }
    }
}
