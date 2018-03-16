using System;
using DruidCraftCalendar.Rings;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;


namespace DruidCraftCalendar
{
    public class Calendar
    {
        public static void DrawCalendar(SKImageInfo info, SKCanvas canvas, INodesModel nodes, ISunModel sun, IMoonModel moon, IMetonicYearModel year, IMonthModel month, IDayModel day, ISunCountModel sunCount)
        {
            ElementDrawer.CreateRing(info,canvas, GetWidthValueFromPercentage(info, (float)41), new Color(0, 0, 0), true);
            DrawNodeRing(info, canvas, nodes);
            DrawSunRing(info, canvas, sun);
            DrawMoonRing(info, canvas, moon);
            ElementDrawer.CreateRing(info, canvas, GetWidthValueFromPercentage(info, (float)34), new Color(0, 0, 0), true);
            BuildDayRings(info, canvas, day, month);
            DrawSunCountRing(info, canvas, sunCount);
            DrawMonthRing(info, canvas, month);
            DrawYearRing(info, canvas, year);
        }

        public static float GetWidthValueFromPercentage(SKImageInfo info, float v)
        {
            var val = (info.Width / 100) * v;
            return val;
        }

        private static void BuildDayRings(SKImageInfo info, SKCanvas canvas, IDayModel day, IMonthModel month)
        {
            float outerRingPercentageWidth = 31;
            float innerRingPercentageWidth = 26;

            var arcPainter = new SKPaint
            {
                Color = SKColors.Black,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 2
            };

            var dayLabelPainter = new SKPaint
            {
                Color = Color.FromRgb(0, 0, 0).ToSKColor(),
                Style = SKPaintStyle.StrokeAndFill,
                StrokeWidth = 2,
                TextSize = GetWidthValueFromPercentage(info, 2.5f)
            };

            info = BuildOuterDayRing(info, canvas, outerRingPercentageWidth, arcPainter);
            info = BuildInnerDayRing(info, canvas, innerRingPercentageWidth, arcPainter);

            CircleShape[] outerRingPegPoints = GetOuterPegPoints(info, canvas, outerRingPercentageWidth);
            CircleShape[] innerRingPegPoints = GetInnerPegPoints(info, canvas, innerRingPercentageWidth);

            ConnectDayRingCrossover(canvas, arcPainter, outerRingPegPoints, innerRingPegPoints);
            CreateDayLabels(info, canvas, arcPainter, dayLabelPainter, outerRingPegPoints, innerRingPegPoints);
            SetActiveDayPeg(info, canvas, day, month, outerRingPegPoints, innerRingPegPoints);
        }

        private static void SetActiveDayPeg(SKImageInfo info, SKCanvas canvas, IDayModel day, IMonthModel month, CircleShape[] outerRingPegPoints, CircleShape[] innerRingPegPoints)
        {
            ColourActiveDayPeg(day, month, outerRingPegPoints, innerRingPegPoints);
            DrawDayPegPoints(info, canvas, outerRingPegPoints, innerRingPegPoints);
        }

        private static void DrawDayPegPoints(SKImageInfo info, SKCanvas canvas, CircleShape[] outerRingPegPoints, CircleShape[] innerRingPegPoints)
        {
            DrawOutterRingPegPoints(canvas, outerRingPegPoints);
            DrawInnerRingPegPoints(info, canvas, innerRingPegPoints);
        }

        private static void DrawInnerRingPegPoints(SKImageInfo info, SKCanvas canvas, CircleShape[] innerRingPegPoints)
        {
            for (int i = 0; i <= innerRingPegPoints.Length - 1; i++)
            {
                DrawPegPointsWithOutOfPlaceFirstPoint(info, canvas, innerRingPegPoints, i);

            }
        }

        private static void DrawOutterRingPegPoints(SKCanvas canvas, CircleShape[] outerRingPegPoints)
        {
            for (int i = 0; i <= outerRingPegPoints.Length - 1; i++)
            {
                DrawAllButFirstPegPoint(canvas, outerRingPegPoints, i);
            }
        }

        private static void DrawPegPointsWithOutOfPlaceFirstPoint(SKImageInfo info, SKCanvas canvas, CircleShape[] innerRingPegPoints, int i)
        {
            if (i == 0)
            {
                canvas.DrawCircle(innerRingPegPoints[i].x, innerRingPegPoints[i].y - GetWidthValueFromPercentage(info, (float)1.7), innerRingPegPoints[i].Radius, GetPointPainter(innerRingPegPoints[i]));
            }
            else
            {
                canvas.DrawCircle(innerRingPegPoints[i].x, innerRingPegPoints[i].y, innerRingPegPoints[i].Radius, GetPointPainter(innerRingPegPoints[i]));
            }
        }

        private static void DrawAllButFirstPegPoint(SKCanvas canvas, CircleShape[] outerRingPegPoints, int i)
        {
            if (i != 0)
                canvas.DrawCircle(outerRingPegPoints[i].x, outerRingPegPoints[i].y, outerRingPegPoints[i].Radius, GetPointPainter(outerRingPegPoints[i]));
        }

        private static void ColourActiveDayPeg(IDayModel day, IMonthModel month, CircleShape[] outerRingPegPoints, CircleShape[] innerRingPegPoints)
        {
            if (!day.IsShortMonth() || (month.Get() == 11 || month.Get() == 10))
            {
                if (day.Get() == 1)
                {
                    innerRingPegPoints[day.Get() - 1].FillColor = new Color(255, 0, 0);
                }
                else
                {
                    outerRingPegPoints[day.Get() - 1].FillColor = new Color(255, 0, 0);
                }


            }
            else
            {
                innerRingPegPoints[day.Get() - 1].FillColor = new Color(255, 0, 0);

            }
        }

        private static void CreateDayLabels(SKImageInfo info, SKCanvas canvas, SKPaint arcPainter, SKPaint dayLabelPainter, CircleShape[] outerRingPegPoints, CircleShape[] innerRingPegPoints)
        {
            var oneLable = "1";
            var thirtyLabel = "30";

            SKRect textbounds = new SKRect();
            dayLabelPainter.MeasureText(oneLable, ref textbounds);
            canvas.DrawText(oneLable, innerRingPegPoints[0].x - textbounds.MidX, innerRingPegPoints[0].y - GetWidthValueFromPercentage(info, 2.6f), dayLabelPainter);

            canvas.DrawText(thirtyLabel, outerRingPegPoints[29].x - GetWidthValueFromPercentage(info, 3.4f), outerRingPegPoints[29].y, dayLabelPainter);

            for (int i = 1; i <= 28; i++)
            {
                canvas.DrawLine(outerRingPegPoints[i].x, outerRingPegPoints[i].y, innerRingPegPoints[i].x, innerRingPegPoints[i].y, arcPainter);
                var xdiff = outerRingPegPoints[i].x - innerRingPegPoints[i].x;
                var ydiff = outerRingPegPoints[i].y - innerRingPegPoints[i].y;
                var xcentre = outerRingPegPoints[i].x - (xdiff / 2);
                var ycentre = outerRingPegPoints[i].y - (ydiff / 2);
                var labelText = (i + 1).ToString();

                dayLabelPainter.MeasureText(labelText, ref textbounds);
                canvas.DrawText(labelText, xcentre - textbounds.MidX, ycentre - textbounds.MidY, dayLabelPainter);
            }
        }

        private static void ConnectDayRingCrossover(SKCanvas canvas, SKPaint arcPainter, CircleShape[] outerRingPegPoints, CircleShape[] innerRingPegPoints)
        {
            canvas.DrawLine(outerRingPegPoints[1].x, outerRingPegPoints[1].y, innerRingPegPoints[28].x, innerRingPegPoints[28].y, arcPainter);
            canvas.DrawLine(outerRingPegPoints[29].x, outerRingPegPoints[29].y, innerRingPegPoints[1].x, innerRingPegPoints[1].y, arcPainter);
        }

        private static CircleShape[] GetInnerPegPoints(SKImageInfo info, SKCanvas canvas, float innerRingPercentageWidth)
        {
            var innerRingTemplate = ElementDrawer.CreateRing(info, canvas, GetWidthValueFromPercentage(info, innerRingPercentageWidth), new Color(0, 0, 0), false);
            var innerRingPegPoints = ElementDrawer.CreatePegPoints(info, canvas, innerRingTemplate, 29);
            return innerRingPegPoints;
        }

        private static CircleShape[] GetOuterPegPoints(SKImageInfo info, SKCanvas canvas, float outerRingPercentageWidth)
        {
            var outerRingTemplate = ElementDrawer.CreateRing(info, canvas, GetWidthValueFromPercentage(info, outerRingPercentageWidth), new Color(0, 0, 0), false);
            var outerRingPegPoints = ElementDrawer.CreatePegPoints(info, canvas, outerRingTemplate, 30);
            return outerRingPegPoints;
        }

        private static SKImageInfo BuildInnerDayRing(SKImageInfo info, SKCanvas canvas, float innerRingPercentageWidth, SKPaint arcPainter)
        {
            var innerRingActualWidth = GetWidthValueFromPercentage(info, innerRingPercentageWidth) * 2;
            var innerRingPadding = (info.Width - innerRingActualWidth) / 2;

            SKRect rect = new SKRect(innerRingPadding, innerRingPadding, info.Width - innerRingPadding, info.Width - innerRingPadding);

            using (SKPath path = new SKPath())
            {
                path.AddArc(rect, -79, 337);
                canvas.DrawPath(path, arcPainter);
            }

            return info;
        }

        private static SKImageInfo BuildOuterDayRing(SKImageInfo info, SKCanvas canvas, float outerRingPercentageWidth, SKPaint arcPainter)
        {
            var outerRingActualWidth = GetWidthValueFromPercentage(info, outerRingPercentageWidth) * 2;
            var outerRingPadding = (info.Width - outerRingActualWidth) / 2;

            SKRect rect = new SKRect(outerRingPadding, outerRingPadding, info.Width - outerRingPadding, info.Width - outerRingPadding);

            using (SKPath path = new SKPath())
            {
                path.AddArc(rect, -79, 337);
                canvas.DrawPath(path, arcPainter);
            }

            return info;
        }



        private static void DrawMonthRing(SKImageInfo info, SKCanvas canvas, IMonthModel month)
        {
            var monthRing = ElementDrawer.CreateRing(info, canvas, GetWidthValueFromPercentage(info, (float)21), new Color(255, 255, 255), false);
            monthRing.FillColor = new Color(255, 255, 0, 0);
            var monthPegPoints = ElementDrawer.CreatePegPoints(info, canvas, monthRing, 13, ((Math.PI * 2) / 13) / 2);

            var monthLabelRing = ElementDrawer.CreateRing(info, canvas, GetWidthValueFromPercentage(info, (float)19.5), new Color(255, 255, 255), false);

            var monthLabelPoints = ElementDrawer.CreatePegPoints(info, canvas, monthLabelRing, 13, ((Math.PI * 2) / 13) / 2);
            float rotation = 74;
            monthPegPoints[month.Get() - 1].FillColor = new Color(255, 0, 0);
            for (int i = 0; i <= monthPegPoints.Length - 1; i++)
            {
                SKPaint txtPaint1 = new SKPaint()
                {
                    Color = Color.FromRgb(255, 255, 255).ToSKColor(),
                    Style = SKPaintStyle.Fill,
                    StrokeWidth = 1,
                    TextSize = GetWidthValueFromPercentage(info, 2.2f)
                };
                SKPaint txtPaint2 = new SKPaint()
                {
                    Color = Color.FromRgb(0, 0, 0).ToSKColor(),
                    Style = SKPaintStyle.Stroke,
                    StrokeWidth = 2,
                    TextSize = GetWidthValueFromPercentage(info, 2.2f)
                };

                canvas.DrawCircle(monthPegPoints[i].x, monthPegPoints[i].y, monthPegPoints[i].Radius, GetPointPainter(monthPegPoints[i]));
                SKRect textbounds = new SKRect();
                var labelText = month.GetMonthName(i + 1);
                txtPaint1.MeasureText(labelText, ref textbounds);
                canvas.Save();
                canvas.RotateDegrees(rotation, monthLabelPoints[i].x, monthLabelPoints[i].y);
                canvas.DrawText(labelText, monthLabelPoints[i].x, monthLabelPoints[i].y + (textbounds.Height / 2), txtPaint1);
                canvas.DrawText(labelText, monthLabelPoints[i].x, monthLabelPoints[i].y + (textbounds.Height / 2), txtPaint2);
                canvas.Restore();
                rotation = rotation - 27.69f;
            }
        }

        private static void DrawMonthLine(SKImageInfo info, SKCanvas canvas, CircleShape p)
        {
            var linePainter = new SKPaint
            {
                Color = SKColors.Black,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 2
            };
            canvas.DrawLine(((info.Width - 20) / 2) + 10, (info.Width - 20) / 2, p.x, p.y, linePainter);
        }

        private static void DrawYearRing(SKImageInfo info, SKCanvas canvas, IMetonicYearModel year)
        {
            var yearRing = ElementDrawer.CreateRing(info, canvas, GetWidthValueFromPercentage(info, (float)6), new Color(0, 0, 0), true);
            var yearRing2 = ElementDrawer.CreateRing(info, canvas, GetWidthValueFromPercentage(info, (float)6), new Color(255, 255, 255), true, true);
            var yearPegPoints = ElementDrawer.CreatePegPoints(info, canvas, yearRing, 19);

            yearPegPoints[year.GetMetonicYear() - 1].FillColor = new Color(255, 0, 0);
            foreach (var p in yearPegPoints)
                canvas.DrawCircle(p.x, p.y, p.Radius, GetPointPainter(p));
        }

        private static void DrawSunCountRing(SKImageInfo info, SKCanvas canvas, ISunCountModel sunCount)
        {
            var sunCountRing = ElementDrawer.CreateRing(info, canvas, GetWidthValueFromPercentage(info, (float)22.5), new Color(0, 0, 0), true);
            var sunCountPegPoints = ElementDrawer.CreatePegPoints(info, canvas, sunCountRing, 13);
            foreach (var p in sunCountPegPoints)
                DrawMonthLine(info, canvas, p);

            sunCountPegPoints[sunCount.GetPosition() - 1].FillColor = new Color(255, 0, 0);
            foreach (var p in sunCountPegPoints)
                canvas.DrawCircle(p.x, p.y, p.Radius, GetPointPainter(p));
        }

        private static void DrawMoonRing(SKImageInfo info, SKCanvas canvas, IMoonModel moon)
        {
            var moonRing = ElementDrawer.CreateRing(info, canvas, GetWidthValueFromPercentage(info, (float)35.6), new Color(255, 255, 255,0), true);
            moonRing.FillColor = new Color(255, 255, 0, 0);
            var moonPegPoints = ElementDrawer.CreatePegPoints(info, canvas, moonRing, 56);

            moonPegPoints[moon.Get() - 1].FillColor = new Color(255, 0, 0);
            foreach (var p in moonPegPoints)
                canvas.DrawCircle(p.x, p.y, p.Radius, GetPointPainter(p));
        }

        private static void DrawSunRing(SKImageInfo info, SKCanvas canvas, ISunModel sun)
        {
            var sunRing = ElementDrawer.CreateRing(info, canvas, GetWidthValueFromPercentage(info, (float)37.5), new Color(255, 255, 255,0), true);
            sunRing.FillColor = new Color(255, 255, 0, 0);
            var sunPegPoints = ElementDrawer.CreatePegPoints(info, canvas, sunRing, 56);

            sunPegPoints[sun.Get() - 1].FillColor = new Color(255, 0, 0);
            foreach (var p in sunPegPoints)
                canvas.DrawCircle(p.x, p.y, p.Radius, GetPointPainter(p));
        }

        private static void DrawNodeRing(SKImageInfo info, SKCanvas canvas, INodesModel nodes)
        {
            var nodeRing = ElementDrawer.CreateRing(info, canvas, GetWidthValueFromPercentage(info, (float)39.4), new Color(255, 255, 255,0), true);
            nodeRing.FillColor = new Color(255, 255, 0, 0);
            var nodePegPoints = ElementDrawer.CreatePegPoints(info, canvas, nodeRing, 56);

            nodePegPoints[nodes.GetNode1Position() - 1].FillColor = new Color(255, 0, 0);
            nodePegPoints[nodes.GetNode2Position() - 1].FillColor = new Color(255, 0, 0);
            foreach (var p in nodePegPoints)
                canvas.DrawCircle(p.x, p.y, p.Radius, GetPointPainter(p));
        }

        private static SKPaint GetRingPainter(CircleShape circle)
        {
            SKPaint paint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = circle.OutlineColor.ToSKColor(),
                StrokeWidth = 2
            };
            return paint;
        }

        private static SKPaint GetPointPainter(CircleShape circle)
        {
            SKPaint paint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = circle.FillColor.ToSKColor(),
                StrokeWidth = 0
            };
            return paint;
        }
    }
}
