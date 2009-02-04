using System;
using System.Collections.Generic;

namespace Magnum.Common.Calendar
{
    public class IsHoliday
    {
        private static readonly IList<IHolidayCheck> _holidays;

        static IsHoliday()
        {
            _holidays = new List<IHolidayCheck>();
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