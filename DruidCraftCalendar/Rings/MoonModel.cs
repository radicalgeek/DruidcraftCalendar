using System;
namespace DruidCraftCalendar.Rings
{
    public class MoonModel : IMoonModel
    {
        private int _count = 1;

        public void Incriment()
        {
            if (_count == 55)
            {
                _count = -1;
            }
            else if (_count == 56)
            {
                _count = 0;
            }
            _count += 2;
        }

        public void Set(int moonPosition)
        {
            _count = moonPosition;
        }

        public int Get()
        {
            return _count;
        }
    }
}
