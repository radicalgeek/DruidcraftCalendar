using System;
using SkiaSharp;

namespace DruidCraftCalendar
{
    public static class Utility
    {
        public static float GetWidthValueFromPercentage(SKImageInfo info, float v)
        {
            var val = 0f;
            if (info.Width < info.Height)
            {
                val = (info.Width / 100) * v;
            }
            else
            {
                val = (info.Height / 100) * v;
            }
            return val;
        }
    }
}
