using System;
namespace DruidCraftCalendar.Rings
{
    public interface IMetonicYearModel
    {
        int GetMetonicYear();
        void IncrimentYear();
        bool IsLeapYear();
        void SetMetonicYear(int yeartoSet);
    }
}
