using System;
using DruidCraftCalendar.Rings;

namespace DruidCraftCalendar
{
    public class CalendarControl
    {
        public readonly IDayModel _dayModel;
        public readonly IMetonicYearModel _metonicYearModel;
        public readonly IMonthModel _monthModel;
        public readonly IMoonModel _moonModel;
        public readonly INodesModel _nodesModel;
        public readonly ISunCountModel _sunCountModel;
        public readonly ISunModel _sunModel;
        public DateTime _gregorianDate;

        public CalendarControl()
        {
            _metonicYearModel = new MetonicYearModel();
            _moonModel = new MoonModel();
            _nodesModel = new NodesModel();
            _sunModel = new SunModel(_nodesModel);
            _sunCountModel = new SunCountModel(_sunModel);
            _monthModel = new MonthModel(_metonicYearModel, _sunModel, _moonModel);
            _dayModel = new DayModel(_monthModel, _metonicYearModel);
        }

        public string GetMetonicDate()
        {
            string date = _dayModel.Get() + " " + _monthModel.GetMonthName() + ", Year " + _metonicYearModel.GetMetonicYear();
            return date;
        }

        public DateTime GetGregorianDate()
        {
            return _gregorianDate;
        }

        public void SetGregorianDate(DateTime date)
        {
            _gregorianDate = date;
        }

        public string GetMetonicDate(DateTime date)
        {
            SetYearZero();
            TimeSpan diff = date - _gregorianDate;
            AddDays(diff.Days);
            return GetMetonicDate();
        }

        public bool CheckLunarEclipse()
        {
            bool isEclipse = false;

            bool node1SunAligned = false;
            if (_nodesModel.GetNode1Position() == _sunModel.Get() || _nodesModel.GetNode1Position() == _sunModel.Get() - 1 || _nodesModel.GetNode1Position() == _sunModel.Get() + 1)
                node1SunAligned = true;

            bool node1MoonAligned = false;
            if (_nodesModel.GetNode1Position() == _moonModel.Get() || _nodesModel.GetNode1Position() == _moonModel.Get() - 1 || _nodesModel.GetNode1Position() == _moonModel.Get() + 1)
                node1MoonAligned = true;

            bool node2SunAligned = false;
            if (_nodesModel.GetNode2Position() == _sunModel.Get() || _nodesModel.GetNode2Position() == _sunModel.Get() - 1 || _nodesModel.GetNode2Position() == _sunModel.Get() + 1)
                node2SunAligned = true;

            bool node2MoonAligned = false;
            if (_nodesModel.GetNode2Position() == _moonModel.Get() || _nodesModel.GetNode2Position() == _moonModel.Get() - 1 || _nodesModel.GetNode2Position() == _moonModel.Get() + 1)
                node2MoonAligned = true;

            if ((node1SunAligned == true && node2MoonAligned == true) || (node2SunAligned == true && node1MoonAligned == true))
                isEclipse = true;

            return isEclipse;

        }

        public bool CheckSolarEclipse()
        {

            bool isEclipse = false;

            bool node1SunAligned = false;
            if (_nodesModel.GetNode1Position() == _sunModel.Get() || _nodesModel.GetNode1Position() == _sunModel.Get() - 1 || _nodesModel.GetNode1Position() == _sunModel.Get() + 1)
                node1SunAligned = true;

            bool node1MoonAligned = false;
            if (_nodesModel.GetNode1Position() == _moonModel.Get() || _nodesModel.GetNode1Position() == _moonModel.Get() - 1 || _nodesModel.GetNode1Position() == _moonModel.Get() + 1)
                node1MoonAligned = true;

            bool node2SunAligned = false;
            if (_nodesModel.GetNode2Position() == _sunModel.Get() || _nodesModel.GetNode2Position() == _sunModel.Get() - 1 || _nodesModel.GetNode2Position() == _sunModel.Get() + 1)
                node2SunAligned = true;

            bool node2MoonAligned = false;
            if (_nodesModel.GetNode2Position() == _moonModel.Get() || _nodesModel.GetNode2Position() == _moonModel.Get() - 1 || _nodesModel.GetNode2Position() == _moonModel.Get() + 1)
                node2MoonAligned = true;

            if ((node1SunAligned == true && node1MoonAligned == true) || (node2SunAligned == true && node2MoonAligned == true))
                isEclipse = true;

            return isEclipse;
        }

        public bool CheckNextFestivalDay()
        {
            if (CheckSamhain() || CheckWinterSolstice() || CheckImbolc() || CheckSpringEquinox() || CheckBeltain() || CheckSummerSolstice() || CheckLamas() || CheckAutumnEquinox())
                return true;

            return false;
        }

        public bool CheckAutumnEquinox()
        {
            if (_gregorianDate.Month == 9 && _gregorianDate.Day == 21)
                return true;

            return false;
        }

        public bool CheckLamas()
        {
            if (_gregorianDate.Month == 8 && _gregorianDate.Day == 1)
                return true;

            return false;
        }

        public bool CheckSummerSolstice()
        {
            if (_gregorianDate.Month == 6 && _gregorianDate.Day == 21)
                return true;

            return false;
        }

        public bool CheckBeltain()
        {
            if (_gregorianDate.Month == 5 && _gregorianDate.Day == 1)
                return true;

            return false;
        }

        public bool CheckSpringEquinox()
        {
            if (_gregorianDate.Month == 2 && _gregorianDate.Day == 21)
                return true;

            return false;
        }

        public bool CheckImbolc()
        {
            if (_gregorianDate.Month == 2 && _gregorianDate.Day == 2)
                return true;

            return false;
        }

        public bool CheckWinterSolstice()
        {
            if (_gregorianDate.Month == 12 && _gregorianDate.Day == 21)
                return true;

            return false;
        }

        public bool CheckSamhain()
        {
            if (_gregorianDate.Month == 10 && _gregorianDate.Day == 31)
                return true;

            return false;
        }

        public void AddDays(int daysToAdd)
        {
            for (int i = 1; i <= daysToAdd; i++)
            {
                AddDay();
            }
        }

        public void AddDay()
        {
            _dayModel.Incriment();
            _moonModel.Incriment();
            _gregorianDate = _gregorianDate.AddDays(1);
            _sunCountModel.Incriment(_gregorianDate);

        }

        public void SetYearZero()
        {
            _dayModel.Set(1);
            _dayModel.IsShortMonth();
            _monthModel.Set(13);
            _metonicYearModel.SetMetonicYear(4);
            _moonModel.Set(28);
            _sunCountModel.Set(1);
            _sunModel.Set(1);
            _nodesModel.SetNode1Position(8);
            _nodesModel.SetNode2Position(36);
            _gregorianDate = new DateTime(1998, 12, 18);
        }
    }
}
