using System.Drawing.Drawing2D;

namespace Astro.Drawing.Extensions
{
    internal static class GraphicsExtensions
    {
        internal static void DrawRoundedRectangle(this Graphics g, Rectangle rect, int cornerRadius, Brush fillColor, Pen? borderColor = null)
        {
            using (GraphicsPath path = new GraphicsPath())
            {
                int diameter = cornerRadius * 2;

                path.StartFigure();
                path.AddArc(rect.Left, rect.Top, diameter, diameter, 180, 90); // kiri atas
                path.AddArc(rect.Right - diameter, rect.Top, diameter, diameter, 270, 90); // kanan atas
                path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90); // kanan bawah
                path.AddArc(rect.Left, rect.Bottom - diameter, diameter, diameter, 90, 90); // kiri bawah
                path.CloseFigure();

                g.FillPath(fillColor, path);
                if (borderColor != null)
                {
                    g.DrawPath(borderColor, path);
                }
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
