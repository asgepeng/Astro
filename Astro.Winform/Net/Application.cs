using Astro.Data;
using Astro.Models;

namespace My
{
    internal static class Application
    {
        internal static string ApiUrl { get; set; } = "";
        internal static string ApiToken { get; set; } = "";
        internal static UserInfo? User { get; set; } = null;
        internal static List<Branch> AccessableBranches { get; } = new List<Branch>();
        internal static Branch? Current { get; set; }
        internal static readonly Font TitleFont = new Font("Segoe UI", 17.75F, FontStyle.Regular);
        internal static readonly Font EmojiFont = new Font("Segoe MDL2 Assets", 7.75F, FontStyle.Regular);
        internal static readonly Color TitleForeColor  = Color.Black;
        internal static IDBClient CreateDBAccess() => new PgDbClient("Host=localhost;Database=pos;Username=posuser;Password=Orogin@k-66171");
        internal static short GetCurrentUserID() => My.Application.User is null ? (short)0 : My.Application.User.Id;
        internal static short GetCurrentLocationID() => (short)1;
    }
}
