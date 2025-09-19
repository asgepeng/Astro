using Astro.Cryptography;
using Astro.Winform.Classes;
using Astro.Binaries;
using Astro.Data;
using System.Data.Common;

namespace Astro.Winform.Forms
{
    public partial class SqlExecuteForm : Form
    {
        private readonly IDBClient db = My.Application.CreateDBAccess();
        public SqlExecuteForm()
        {
            InitializeComponent();
        }

        private async void richTextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.F5)
            {
                var commandText = richTextBox1.Text.Trim();
                var parameters = new DbParameter[]
                {
                    db.CreateParameter("userid", My.Application.GetCurrentLocationID(), System.Data.DbType.Int16),
                    db.CreateParameter("location", My.Application.GetCurrentLocationID(), System.Data.DbType.Int16)
                };
                this.dataGridView1.DataSource = await db.ExecuteDataTableAsync(commandText, parameters);
            }
        }
    }
}
