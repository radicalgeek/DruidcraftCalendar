using System;
namespace DruidCraftCalendar.Rings
{
    public class MonthModel : IMonthModel
    {
        private int _month;
        private IMetonicYearModel _metonicYear { get; set; }
        private readonly IMoonModel _moon;
        private readonly ISunModel _sun;

        public MonthModel(IMetonicYearModel metonicYear, ISunModel sun, IMoonModel moon)
        {
            _metonicYear = metonicYear;
            _sun = sun;
            _moon = moon;
        }

        public void Set(int month)
        {

            _month = month;
        }

        public int Get()
        {
            return _month;
        }

        public void Incriment()
        {

            if (_month == 13)
            {
                _month = 0;
                _metonicYear.IncrimentYear();
            }

            if (!_metonicYear.IsLeapYear() && _month == 10)
            {
                _month += 1;
            }
            _month += 1;
        }

        public void PerformMoonCorrection()
        {
            int moonPosition;
            int sunPosition = _sun.Get();
            moonPosition = sunPosition + 29;
            if (moonPosition >= 57)
            {
                moonPosition = moonPosition - 56;
            }
            _moon.Set(moonPosition);
        }

        public string GetMonthName(int month)
        {
            string monthname = "";
            switch (month)
            {
                case 1:
                    monthname = "Cold Come";
                    break;
                case 2:
                    monthname = "Harth Warm";
                    break;
                case 3:
                    monthname = "Ice Over";
                    break;
                case 4:
                    monthname = "Wind Blow";
                    break;
                case 5:
                    monthname = "Shoot Show";
                    break;
                case 6:
                    monthname = "Light Bright";
                    break;
                case 7:
                    monthname = "Shoot Grow";
                    break;
                case 8:
                    monthname = "Earth Yield";
                    break;
                case 9:
                    monthname = "Fruit Ripe";
                    break;
                case 10:
                    monthname = "Seed Drop";
                    break;
                case 11:
                    monthname = "Seed Drop";
                    break;
                case 12:
                    monthname = "Frost Fall";
                    break;
                case 13:
                    monthname = "Dark Deapths";
                    break;
            }
            return monthname;
        }

        public string GetMonthName()
        {
            string monthname = "";
            switch (_month)
            {
                case 1:
                    monthname = "Cold Come";
                    break;
                case 2:
                    monthname = "Harth Warm";
                    break;
                case 3:
                    monthname = "Ice Over";
                    break;
                case 4:
                    monthname = "Wind Blow";
                    break;
                case 5:
                    monthname = "Shoot Show";
                    break;
                case 6:
                    monthname = "Light Bright";
                    break;
                case 7:
                    monthname = "Shoot Grow";
                    break;
                case 8:
                    monthname = "Earth Yield";
                    break;
                case 9:
                    monthname = "Fruit Ripe";
                    break;
                case 10:
                    monthname = "Seed Drop";
                    break;
                case 11:
                    monthname = "Seed Drop";
                    break;
                case 12:
                    monthname = "Frost Fall";
                    break;
                case 13:
                    monthname = "Dark Deapths";
                    break;
            }
            return monthname;
        }
    }
}
