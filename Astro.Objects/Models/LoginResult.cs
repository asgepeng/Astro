using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astro.Models
{
    public enum LoginResult
    {
        Success, AccountLocked, InvalidCredential, NotFound, CreateTokenFailed, PasswordExpired
    }
}
