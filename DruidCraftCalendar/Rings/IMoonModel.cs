using System;
namespace DruidCraftCalendar.Rings
{
    public interface IMoonModel
    {
        int Get();
        void Incriment();
        void Set(int moonPosition);
    }
}
