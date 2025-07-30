using Astro.Cryptography;
using Astro.Winform.Classes;
using Astro.Winform.Helpers;
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
    public partial class ListingPopUpForm : Form
    {
        private readonly string commandText;
        private readonly Guid guid = Guid.NewGuid();
        private readonly BindingSource bs;

        private List<string> stringColumns = new List<string>();
        public ListingPopUpForm(string cmd)
        {
            InitializeComponent();
            this.bs = new BindingSource();
            this.dataGridView1.DataSource = bs;
            this.dataGridView1.AutoGenerateColumns = false;
            this.commandText = cmd;
        }
        private async void ListingPopUpForm_Load(object sender, EventArgs e)
        {
            var key = guid.ToByteArray();
            var encrypted = SimpleEncryption.Encrypt(commandText, key);
            using (var writer = new IO.Writer())
            {
                writer.WriteGuid(guid);
                writer.WriteString(encrypted);

                using (var stream = await HttpClientSingleton.PostStreamAsync("/api/sql", writer.ToArray()))
                using (var reader = new IO.Reader(stream))
                {
                    var table = reader.ReadDataTable();
                    foreach (DataColumn col in table.Columns)
                    {
                        if (col.DataType.Equals(typeof(string)))
                        {
                            stringColumns.Add(col.ColumnName);
                        }
                    }
                    this.bs.DataSource = table;
                    this.textBox1.Enabled = true;
                    this.dataGridView1.Enabled = true;
                    this.button1.Enabled = true;
                }
            }
        }
        internal void AddColumn(string headerText, string propertyName, int width, DataGridViewContentAlignment alignment = DataGridViewContentAlignment.MiddleLeft, string format = "")
        {
            var colname = "col_" + this.dataGridView1.Columns.Count.ToString();
            var index = this.dataGridView1.Columns.Add(colname, headerText);

            var column = this.dataGridView1.Columns[index];
            column.DataPropertyName = propertyName;
            column.Width = width;
            column.DefaultCellStyle.Alignment = alignment;
            column.DefaultCellStyle.Format = format;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (bs.Current is null)
            {
                MessageBox.Show("There is no one selected", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            this.DialogResult = DialogResult.OK;
            this.Close();

        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            this.button1.PerformClick();
        }

        public DataRow SelectedRow
        {
            get
            {
                return ((DataRowView)this.bs.Current).Row;
            }
        }

        private string CreateFilter(string keyword)
        {
            var saveKeyword = keyword.Replace("'", "''");
            var columns = new string[stringColumns.Count];
            for (int i = 0; i < this.stringColumns.Count; i++)
            {
                columns[i] = stringColumns[i] + " LIKE '%" + saveKeyword + "%'";
            }
            return string.Join(" OR ", columns);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            this.bs.Filter = CreateFilter(this.textBox1.Text);
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.FilterAlphaNumeric();
        }
    }
}
