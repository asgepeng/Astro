using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PointOfSale.Drawing
{
    internal static class DrawingHelper
    {
        internal static void DrawRoundedRectangle(Graphics g, Rectangle rect, int cornerRadius, Brush fillColor)
        {
            using (GraphicsPath path = new GraphicsPath())
            {
                path.StartFigure();
                path.AddArc(rect.Left, rect.Top, cornerRadius, cornerRadius, 180, 90);
                path.AddArc(rect.Right - cornerRadius, rect.Top, cornerRadius, cornerRadius, 270, 90);
                path.AddArc(rect.Right - cornerRadius, rect.Bottom - cornerRadius, cornerRadius, cornerRadius, 0, 90);
                path.AddArc(rect.Left, rect.Bottom - cornerRadius, cornerRadius, cornerRadius, 90, 90);
                path.CloseFigure();

                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.FillPath(fillColor, path);
            }
        }
        internal static void DrawRoundedHalf(Graphics g, Rectangle bounds, int radius)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            GraphicsPath path = new GraphicsPath();
            path.StartFigure();
            path.AddArc(bounds.Right - radius, bounds.Y, radius, radius, 270, 90);
            path.CloseFigure();
            path.CloseFigure();

            using (Pen pen = new Pen(Color.Black))
            {
                g.DrawPath(pen, path);
            }
        }
    }
}
