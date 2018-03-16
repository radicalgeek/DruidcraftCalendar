using System;
namespace DruidCraftCalendar.Rings
{
    public interface IMonthModel
    {
        int Get();
        void Incriment();
        void Set(int month);
        string GetMonthName();
        string GetMonthName(int month);
        void PerformMoonCorrection();
    }
}
