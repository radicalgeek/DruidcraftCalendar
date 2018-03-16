using System;
namespace DruidCraftCalendar.Rings
{
    public class SunCountModel : ISunCountModel
    {
        private ISunModel _sun;
        public SunCountModel(ISunModel sun)
        {
            _sun = sun;
        }
        private int _count = 1;

        public void Incriment(DateTime gregorianDate)
        {

            if (_count == 13)
            {
                _count = 0;
            }
            _count += 1;

            if (_count == 7 || _count == 1)
            {
                _sun.Incriment(gregorianDate);
            }

        }

        public int GetPosition()
        {
            return _count;
        }

        public void Set(int value)
        {
            _count = value;
        }
    }
}
