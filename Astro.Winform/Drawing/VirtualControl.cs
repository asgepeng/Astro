using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PointOfSale.Drawing
{
    public abstract class VirtualControl
    {
        internal abstract void Draw(Graphics g);
    }

    public class VirtualControlCollection : List<VirtualControl>
    {
        internal void Draw(Graphics g)
        {
            foreach( var item in this)
            {
                item.Draw(g);
            }
        }
    }
}
