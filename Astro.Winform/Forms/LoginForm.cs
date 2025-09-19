using Astro.Data;
using Astro.Models;
using Astro.Services;
using Mysqlx;
using MySqlX.XDevAPI.Common;
using System.Data;

namespace Astro.Winform.Forms
{
    public partial class LoginForm : Form
    {
        private readonly IDBClient db;
        public LoginForm()
        {
            InitializeComponent();
            db = My.Application.CreateDBAccess();
        }
        void DisableControls(bool disabled)
        {
            if (disabled) this.Cursor = Cursors.WaitCursor;
            else this.Cursor = Cursors.Default;

            foreach (Control control in this.Controls)
            {
                control.Enabled = !disabled;
            }
        }

        private async void LoginButtonClicked(object sender, EventArgs e)
        {
            this.DisableControls(true);
            if (this.usernameTextBox.Text.Trim() == "")
            {
                MessageBox.Show("Username tidak boleh kosong", "Username", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.usernameTextBox.Focus();
                return;
            }
            if (this.passwordTextBox.Text == "")
            {
                MessageBox.Show("Password tidak boleh kosong", "Password", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.passwordTextBox.Focus();
                return;
            }
            var credential = new Credential(this.usernameTextBox.Text.Trim(), this.passwordTextBox.Text);
            if (string.IsNullOrEmpty(credential.Username) || string.IsNullOrEmpty(credential.Password))
            {
                MessageBox.Show("Username atau password salah", "Invalid Accsess", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            var ipv4 = new byte[] { 0x00, 0x00, 0x00, 0x00 };
            var loginInfo = await LoginService.GetLoginInfoAsync(credential.Username, db);
            if (loginInfo is null)
            {
                await LoginService.CreateLoginHistory(null, LoginResult.NotFound, db, ipv4);
                MessageBox.Show("Username atau password salah", "Invalid Accsess", MessageBoxButtons.OK, MessageBoxIcon.Error); ;
                return;
            }
            if (loginInfo.IsLockedOut())
            {
                await LoginService.CreateLoginHistory(loginInfo, LoginResult.AccountLocked, db, ipv4);
                MessageBox.Show("Akun terkunci", "OK", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            var credentialVerified = loginInfo.VerifyPassword(credential.Password);
            if (!credentialVerified)
            {
                if (loginInfo.LockoutEnabled && await LoginService.IncrementAccessFailedCountAsync(loginInfo, db))
                {
                    await LoginService.CreateLoginHistory(loginInfo, LoginResult.AccountLocked, db, ipv4);
                    MessageBox.Show("Your account has been temporarily locked due to multiple failed login attempts.");
                    return;
                }
                else
                {
                    await LoginService.CreateLoginHistory(loginInfo, LoginResult.InvalidCredential, db, ipv4);
                    MessageBox.Show("Username atau password salah", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            if (loginInfo.IsPasswordExpired())
            {
                await LoginService.CreateLoginHistory(loginInfo, LoginResult.PasswordExpired, db, ipv4);
                MessageBox.Show("Password expired");
                return;
            }
            var commandText = """
                SELECT e.employeeid, e.fullname, e.roleid, r.name AS rolename
                FROM employees AS e
                INNER JOIN roles AS r ON e.roleid = r.roleid
                WHERE employeeid = @id
                """;
            await db.ExecuteReaderAsync(async reader =>
            {
                if (await reader.ReadAsync())
                {
                    My.Application.User = new UserInfo(reader.GetInt16(0), reader.GetString(1), new Role()
                    {
                        Id = reader.GetInt16(2),
                        Name = reader.GetString(3)
                    });
                }
            }, commandText, db.CreateParameter("id", loginInfo.EmployeeId, DbType.Int16));

            if (My.Application.User != null)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            this.usernameTextBox.Text = "asgepeng@gmail.com";
            this.passwordTextBox.Text = "Power123...";
            this.button1.PerformClick();
        }
    }
}
