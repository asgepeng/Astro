using System.Drawing.Drawing2D;

namespace Astro.Drawing.Extensions
{
    internal static class GraphicsExtensions
    {
        internal static void DrawRoundedRectangle(this Graphics g, Rectangle rect, int cornerRadius, Brush fillColor)
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
        internal static void DrawRoundedHalf(this Graphics g, Rectangle bounds, int radius)
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
        internal static void DrawTopLeftRoundedRectangle(this Graphics g, Rectangle rect, int radius, Brush fillBrush, Pen borderPen)
        {
            using (GraphicsPath path = new GraphicsPath())
            {
                int diameter = radius * 2;
                path.AddArc(rect.Left, rect.Top, diameter, diameter, 180, 90);
                path.AddLine(rect.Left + radius, rect.Top, rect.Right, rect.Top);
                path.AddLine(rect.Right, rect.Top, rect.Right, rect.Bottom);
                path.AddLine(rect.Right, rect.Bottom, rect.Left, rect.Bottom);
                path.AddLine(rect.Left, rect.Bottom, rect.Left, rect.Top + radius);
                path.CloseFigure();
                g.FillPath(fillBrush, path);
                g.DrawPath(borderPen, path);
            }
        }
    }
}
