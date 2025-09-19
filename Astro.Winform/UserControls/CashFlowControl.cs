using Astro.Cryptography;
using Astro.Binaries;
using Astro.Winform.Classes;
using WinformApp.Data;
using Astro.Data;

namespace Astro.Winform.UserControls
{
    public partial class CashFlowControl : XPanel
    {
        private readonly IDBClient db = My.Application.CreateDBAccess();
        private readonly BindingSource bs = new BindingSource();
        public CashFlowControl()
        {
            InitializeComponent();
            this.Text = "Cash Flow";
            GridHelpers.InitializeDataGridColumns(this.dGrid1, new DataTableColumnInfo[]
            {
                new DataTableColumnInfo("Tanggal", "cashflowdate", 110, DataGridViewContentAlignment.MiddleRight, "dd-MM-yy HH:mm"),
                new DataTableColumnInfo("Account Name", "accountname", 300),
                new DataTableColumnInfo("Debt", "deb", 100, DataGridViewContentAlignment.MiddleRight, "N0"),
                new DataTableColumnInfo("Credit", "cre", 100, DataGridViewContentAlignment.MiddleRight, "N0")
            }, this.bs);
        }

        private async void CashFlowControl_Load(object sender, EventArgs e)
        {
            var commandText = """
                SELECT c.cashflowdate, 
                CASE WHEN c.accounttype = 1 THEN e.fullname WHEN c.accounttype = 2 THEN CONCAT(ap.name, ' - ', a.accountname) ELSE '' END AS accountname, 
                CASE WHEN c.amount < 0 THEN c.amount * -1 ELSE 0 END AS deb,
                CASE WHEN c.amount > 0 THEN c.amount ELSE 0 END AS cre
                FROM cashflows AS c
                LEFT JOIN accounts AS a ON c.accountid = a.accountid AND c.accounttype = 2
                LEFT JOIN accountproviders AS ap ON a.providerid = ap.providerid
                LEFT JOIN employees AS e ON c.accountid = e.employeeid AND c.accounttype = 1
                ORDER BY c.cashflowdate             
                """;
            this.bs.DataSource = await db.ExecuteDataTableAsync(commandText);
        }
    }
}
