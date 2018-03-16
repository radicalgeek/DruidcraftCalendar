using System;
namespace DruidCraftCalendar.Rings
{
    public class SunModel : ISunModel
    {
        private INodesModel _nodes;
        private int _count = 1;

        public SunModel(INodesModel nodes)
        {
            _nodes = nodes;
        }

        public void Incriment(DateTime gregorianDate)
        {
            if (_count == 56)
            {
                _count = 0;
            }
            _count += 1;
            if (_count == 19 || _count == 39 || _count == 1)
            {
                _nodes.Incriment();
            }
            if (gregorianDate.Month == 12 && gregorianDate.Day == 21)
            {
                Set(1);
            }
            if (gregorianDate.Month == 6 && gregorianDate.Day == 21)
            {
                Set(29);
            }

        }

        public int Get()
        {
            return _count;
        }

        public void Set(int value)
        {
            _count = value;
        }
    }
}
