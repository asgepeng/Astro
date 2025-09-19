using Astro.Data;
using Astro.Models;
using System.Data;
using System.Data.Common;
using System.Net.Http;

namespace Astro.Services
{
    public class LoginService
    {
        public static async Task<Login?> GetLoginInfoAsync(string login, IDBClient db)
        {
            Login? loginInfo = null;
            var commandText = """
                UPDATE logins
                SET accessfailedcount = 0,
                    lockoutend = NULL
                WHERE lockoutend < CURRENT_TIMESTAMP;
                SELECT
                    l.employeeid,
                    e.fullname,
                    e.roleid,
                    l.passwordhash,
                    l.accessfailedcount,
                    l.lockoutenabled,
                    l.lockoutend,
                    l.usepasswordexpiration,
                    l.passwordexpirationdate
                FROM
                    employees AS e
                	INNER JOIN logins AS l ON e.employeeid = l.employeeid
                WHERE (e.email = @login OR e.phone = @login)
                    AND e.isdeleted = false;
                """;
            await db.ExecuteReaderAsync(async (DbDataReader reader) =>
            {
                if (await reader.ReadAsync())
                {
                    loginInfo = Login.Create(reader);
                }
            }, commandText, db.CreateParameter("login", login));
            return loginInfo;
        }
        public static async Task CreateLoginHistory(Login? loginInfo, LoginResult loginResult, IDBClient db, byte[] ipv4)
        {
            var commandText = """
                INSERT INTO login_attempts
                    (userid, ipv4, useragent, issuccess, notes)
                VALUES
                    (@userId, @ipv4, @useragent, @success, @notes);
                """;
            var getDescription = string (LoginResult result) =>
            {
                switch (loginResult)
                {
                    case LoginResult.Success: return "Login success";
                    case LoginResult.AccountLocked: return "Account locked";
                    case LoginResult.InvalidCredential: return "Invalid Credential";
                    case LoginResult.NotFound: return "Not registered account";
                    case LoginResult.CreateTokenFailed: return "Token generation failed";
                    case LoginResult.PasswordExpired: return "Password expired";
                    default: return "";
                }
            };
            var notes = getDescription(loginResult);
            var userId = loginInfo is null ? 0 : loginInfo.EmployeeId;
            var parameters = new DbParameter[]
            {
                db.CreateParameter("userId", userId, DbType.Int16),
                db.CreateParameter("ipv4", ipv4, DbType.Binary),
                db.CreateParameter("success", loginResult == LoginResult.Success, DbType.Boolean),
                db.CreateParameter("useragent", "", DbType.AnsiString),
                db.CreateParameter("notes", notes, DbType.AnsiString)
            };

            await db.ExecuteNonQueryAsync(commandText, parameters);
        }
        public static async Task<bool> IncrementAccessFailedCountAsync(Login loginInfo, IDBClient db)
        {
            var commandText = """
                UPDATE users
                SET
                    access_failed_count = access_failed_count + 1,
                    lockout_end = CASE WHEN (access_failed_count + 1) >= @threshold THEN CURRENT_TIMESTAMP + INTERVAL '5 minutes' ELSE NULL END
                WHERE 
                    user_id = @userId
                RETURNING access_failed_count, lockout_end;
                """;
            var parameters = new DbParameter[]
            {
                db.CreateParameter("userId", loginInfo.EmployeeId, DbType.Int16),
                db.CreateParameter("threshold", 3, DbType.Int32)
            };
            await db.ExecuteReaderAsync(async (DbDataReader reader) =>
            {
                if (await reader.ReadAsync())
                {
                    loginInfo.AccessFailedCount = reader.GetInt16(0);
                    loginInfo.LockoutEnd = !reader.IsDBNull(1) ? reader.GetDateTime(1) : null;
                }
            },
            commandText, parameters);

            return loginInfo.HasExceededFailedAttempts(3);
        }
    }
}
