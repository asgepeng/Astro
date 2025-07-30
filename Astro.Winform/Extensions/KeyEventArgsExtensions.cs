using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astro.Winform.Helpers
{
    internal static class KeyEventArgsExtensions
    {
        internal static void FilterOnlyNumber(this KeyPressEventArgs e)
        {
            var code = Convert.ToInt32(e.KeyChar);
            if (!(code >= 48 && code <= 57) && code != 8)
            {
                e.Handled = true;
            }
        }
        internal static void FilterAlphaNumeric(this KeyPressEventArgs e)
        {
            char c = e.KeyChar;

            if (!char.IsLetterOrDigit(c) && c != 8) // 8 = Backspace
            {
                e.Handled = true;
            }
        }

    }
}
