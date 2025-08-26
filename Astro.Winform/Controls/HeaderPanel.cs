using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astro.Winform.Controls
{
    public class HeaderPanel : Control
    {
        public HeaderPanel()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                  ControlStyles.AllPaintingInWmPaint |
                  ControlStyles.ResizeRedraw |
                  ControlStyles.UserPaint, true);
        }
    }
}
