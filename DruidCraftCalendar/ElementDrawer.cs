using System;
using SkiaSharp;
using Xamarin.Forms;
using SkiaSharp.Views.Forms;

namespace DruidCraftCalendar
{
    public class ElementDrawer
    {
        public static CircleShape GetPegPointTemplate(SKImageInfo info)
        {
            var pegPoint = new CircleShape();
            pegPoint.Radius = (info.Width / 100) * (float)0.7;
            pegPoint.FillColor = Color.FromRgb(90,90,90);
            return pegPoint;
        }

        public static CircleShape[] CreatePegPoints(SKImageInfo info, SKCanvas canvas, CircleShape ring, int points, double offset = 0)
        {
            CircleShape[] pegPoints = new CircleShape[points];
            for (var i = 0; i < points; i++)
            {
                float x = Convert.ToSingle(((info.Width) / 2) + ring.Radius * Math.Sin(Math.PI + offset + (2 * Math.PI * i / points)));
                float y = Convert.ToSingle(((info.Width) / 2) + ring.Radius * Math.Cos(Math.PI + offset + (2 * Math.PI * i / points)));
                var point = GetPegPointTemplate(info);
                point.x = x;
                point.y = y;
                pegPoints[i] = point;
            }
            return pegPoints;
        }

        public static CircleShape CreateRing(SKImageInfo info, SKCanvas canvas, float radius, Color color, bool draw, bool fill = false)
        {
            var ring = new CircleShape();
            ring.Radius = radius;
            ring.OutlineColor = color;

            SKPaint paint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = ring.OutlineColor.ToSKColor(),
                StrokeWidth = 2
            };
            if (fill == true)
                paint.Style = SKPaintStyle.Fill;

            if (draw)
                canvas.DrawCircle(((info.Width) / 2), (info.Width) / 2, ring.Radius, paint);
            return ring;
        }
    }
}
