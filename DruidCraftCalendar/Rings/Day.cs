using System;
namespace DruidCraftCalendar.Rings
{
    public class DayModel : IDayModel
    {
        private int _day;
        public bool _shortMonth;
        private IMonthModel _month;
        private IMetonicYearModel _year;


        public DayModel(IMonthModel month, IMetonicYearModel year)
        {
            _month = month;
            _year = year;

        }

        public bool IsShortMonth()
        {
            return _shortMonth;
        }

        public void Set(int day)
        {
            _day = day;
        }

        public int Get()
        {
            return _day;
        }

        public void Incriment()
        {
            if ((_shortMonth == true && _day == 15) || (_shortMonth == false && _day == 16))
                _month.PerformMoonCorrection();
            if ((_month.Get() == 10 || _month.Get() == 11) && _year.IsLeapYear() == true)
            {
                if (_day == 30)
                {
                    _day = 0;
                    _month.Incriment();
                }
            }
            else
            {
                if (_shortMonth == true)
                {
                    if (_day == 29)
                    {
                        _day = 0;
                        SetLongMonth();
                        _month.Incriment();
                    }
                }
                else
                {
                    if (_day == 30)
                    {
                        _day = 0;
                        SetShortMonth();
                        _month.Incriment();
                    }
                }
            }

            _day += 1;
        }

        public void SetShortMonth()
        {
            _shortMonth = true;
        }

        public void SetLongMonth()
        {
            _shortMonth = false;
        }
    }
}
