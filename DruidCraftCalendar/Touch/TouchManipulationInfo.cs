using SkiaSharp;

namespace DruidCraftCalendar
{
    internal class TouchManipulationInfo
    {
        public SKPoint PreviousPoint { get; set; }

        public SKPoint NewPoint { get; set; }
    }
}