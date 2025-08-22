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
            this.grid.DataSource = bs;
            this.grid.AutoGenerateColumns = false;
            this.grid.MultiSelect = false;
            this.commandText = cmd;
        }
        private async void ListingPopUpForm_Load(object sender, EventArgs e)
        {
            var key = guid.ToByteArray();
            var encrypted = Encryption.Encrypt(commandText, key);
            using (var writer = new IO.Writer())
            {
                writer.WriteGuid(guid);
                writer.WriteString(encrypted);

                using (var stream = await WClient.PostStreamAsync("/api/sql", writer.ToArray()))
                using (var reader = new IO.Reader(stream))
                {
                    var result = reader.ReadByte();
                    if (result == 1)
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
                    }
                    else if (result == 2)
                    {
                        var message = reader.ReadString();
                        MessageBox.Show(message);
                    }

                    this.searchTextBox.Enabled = true;
                    this.grid.Enabled = true;
                    this.okButton.Enabled = true;
                }
            }
            this.searchTextBox.Focus();
        }
        internal void AddColumn(string headerText, string propertyName, int width, DataGridViewContentAlignment alignment = DataGridViewContentAlignment.MiddleLeft, string format = "")
        {
            var colname = "col_" + this.grid.Columns.Count.ToString();
            var index = this.grid.Columns.Add(colname, headerText);

            var column = this.grid.Columns[index];
            column.DataPropertyName = propertyName;
            column.Width = width;
            column.DefaultCellStyle.Alignment = alignment;
            column.DefaultCellStyle.Format = format;
        }

        private void OKButtonClicked(object sender, EventArgs e)
        {
            if (bs.Current is null)
            {
                MessageBox.Show("There is no one selected", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            this.DialogResult = DialogResult.OK;
            this.Close();

        }

        private void GridDoubleClicked(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            this.okButton.PerformClick();
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

        private void ApplyFilter(object sender, EventArgs e)
        {
            this.bs.Filter = CreateFilter(this.searchTextBox.Text);
        }

        private void SearchTextBoxKeyPressed(object sender, KeyPressEventArgs e)
        {
            e.FilterAlphaNumeric();
        }

        private void SearchTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Up) this.bs.MovePrevious();
            if (e.KeyData == Keys.Down) this.bs.MoveNext();
            if (e.KeyData == Keys.Enter) this.okButton.PerformClick();
        }
    }
}
