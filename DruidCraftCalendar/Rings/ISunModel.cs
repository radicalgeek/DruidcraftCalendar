using System;
namespace DruidCraftCalendar.Rings
{
    public interface ISunModel
    {
        int Get();
        void Incriment(DateTime gregorianDate);
        void Set(int value);
    }
}
