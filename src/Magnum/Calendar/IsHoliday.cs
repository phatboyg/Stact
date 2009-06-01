using System;
using System.Collections.Generic;

namespace Magnum.Calendar
{
    public class IsHoliday
    {
        private static readonly IList<IHolidayCheck> _holidays;

        static IsHoliday()
        {
            _holidays = new List<IHolidayCheck>();
        }

        public static void Define(Action<IHolidayConfigurator> configuration)
        {
            HolidayConfigurator cfg = new HolidayConfigurator();
            configuration(cfg);
            cfg.AddHolidays(_holidays);
        }

        public static CheckResult Check(DateTime dateToCheck)
        {
            var result = new CheckResult();

            foreach (var check in _holidays)
            {
                if(check.Check(dateToCheck))
                {
                    result = new CheckResult(true, check.HolidayName);
                    break;
                }
                    
            }

            return result;
        }
    }

    public class HolidayConfigurator :
        IHolidayConfigurator
    {
        public void AddHolidays(IList<IHolidayCheck> holidays)
        {
            

        }

        public void AddHoliday(string name, DateCheck check)
        {
            
        }
    }

    public static class Month
    {
        public static DateCheck IsJanuary()
        {
            return new DateCheck(d=>d.Month == (int)Months.January);
        }
        public static DateCheck IsJanuary(Action<DaySpecs> daySpecs)
        {
            return IsMonth(IsJanuary(), daySpecs);
        }

        public static DateCheck IsFebruary()
        {
            return new DateCheck(d => d.Month == (int)Months.Feburary);
        }
        public static DateCheck IsFebruary(Action<DaySpecs> daySpecs)
        {
            return IsMonth(IsFebruary(), daySpecs);
        }

        public static DateCheck IsMay()
        {
            return new DateCheck(d => d.Month == (int)Months.May);
        }
        public static DateCheck IsMay(Action<DaySpecs> daySpecs)
        {
            return IsMonth(IsMay(), daySpecs);
        }

        public static DateCheck IsSeptember()
        {
            return new DateCheck(d => d.Month == (int)Months.May);
        }
        public static DateCheck IsSeptember(Action<DaySpecs> daySpecs)
        {
            return IsMonth(IsSeptember(), daySpecs);
        }

        public static DateCheck IsNovember()
        {
            return new DateCheck(d => d.Month == (int)Months.November);
        }
        public static DateCheck IsNovember(Action<DaySpecs> daySpecs)
        {
            return IsMonth(IsNovember(), daySpecs);
        }
        public static DateCheck IsDecember()
        {
            return new DateCheck(d=>d.Month == (int)Months.December);
        }
        public static DateCheck IsDecember(Action<DaySpecs> daySpecs)
        {
            return IsMonth(IsDecember(), daySpecs);
        }
        private static DateCheck IsMonth(DateCheck month, Action<DaySpecs> daySpecs)
        {
            DateCheck dc = month;
            var spec = new DaySpecs();
            daySpecs(spec);

            spec.Configure(dc);

            return dc;
        }

        
    }
    public class DaySpecs
    {
        private IList<Func<DateTime, bool>>  _checks = new List<Func<DateTime, bool>>();

        public void IsMonday()
        {
            _checks.Add(d => d.DayOfWeek == DayOfWeek.Monday);
        }
        public void IsTuesday()
        {
            _checks.Add(d => d.DayOfWeek == DayOfWeek.Tuesday);
        }
        public void IsWednesday()
        {
            _checks.Add(d => d.DayOfWeek == DayOfWeek.Wednesday);
        }
        public void IsThursday()
        {
            _checks.Add(d => d.DayOfWeek == DayOfWeek.Thursday);
        }

        public void DayIs(int dayNumber)
        {
            _checks.Add(d=>d.Day == dayNumber);
        }

        public void Configure(DateCheck check)
        {
            check.AddChecks(_checks);
        }

        //http://michaelthompson.org/technikos/holidays.php
        public void NthDayOfMonth(int nTh, DayOfWeek day)
        {
            _checks.Add(d=>d.DayOfWeek == day);
            _checks.Add(d=> d.Day >= (1+7*(nTh-1)));
        }
        public void IsWeekday()
        {
            _checks.Add(d => d.DayOfWeek != DayOfWeek.Saturday && d.DayOfWeek != DayOfWeek.Sunday);
        }
        public void IsWeekend()
        {
            _checks.Add(d => d.DayOfWeek == DayOfWeek.Saturday && d.DayOfWeek == DayOfWeek.Sunday);
        }

        public void InRange(int start, int end)
        {
            _checks.Add(d=> start < d.Day && d.Day < end);
        }

        public void Custom(Func<DateTime, bool> func)
        {
            _checks.Add(func);
        }
    }
    public class DateCheck
    {
        private IList<Func<DateTime, bool>> _checks = new List<Func<DateTime, bool>>();

        public DateCheck(Func<DateTime, bool> monthCheck)
        {
            _checks.Add(monthCheck);
        }

        public void AddChecks(IList<Func<DateTime, bool>> checks)
        {
            foreach (var check in checks)
            {
                _checks.Add(check);
            }
        }
    }

    public interface IHolidayConfigurator
    {
        void AddHoliday(string name, DateCheck check);
    }

    public interface IHolidayCheck
    {
        bool Check(DateTime dateToCheck);
        string HolidayName { get; }

    }

    public class CheckResult
    {
        private readonly bool _isHoliday;
        private readonly string _description;

        public CheckResult() : this(false, "No Holiday Detected")
        {

        }

        public CheckResult(bool isHoliday, string description)
        {
            _isHoliday = isHoliday;
            _description = description;
        }


        public bool IsHoliday
        {
            get { return _isHoliday; }
        }

        public string Description
        {
            get { return _description; }
        }
    }
}