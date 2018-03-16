using System;
namespace DruidCraftCalendar.Rings
{
    public interface ISunCountModel
    {
        int GetPosition();
        void Incriment(DateTime gregorianDate);
        void Set(int value);
    }
}
