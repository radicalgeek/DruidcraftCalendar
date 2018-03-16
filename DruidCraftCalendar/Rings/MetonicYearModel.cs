using System;
namespace DruidCraftCalendar.Rings
{
    public class MetonicYearModel : IMetonicYearModel
    {
        private int metonicyear { get; set; }

        public int GetMetonicYear()
        {
            return metonicyear;
        }

        public void SetMetonicYear(int yeartoSet)
        {
            metonicyear = yeartoSet;
        }

        public void IncrimentYear()
        {
            if (metonicyear == 19)
                metonicyear = 0;
            metonicyear += 1;
        }

        public bool IsLeapYear()
        {
            switch (metonicyear)
            {
                case 3:
                    return true;
                case 6:
                    return true;
                case 8:
                    return true;
                case 11:
                    return true;
                case 14:
                    return true;
                case 17:
                    return true;
                case 19:
                    return true;
            }
            return false;
        }
    }
}
