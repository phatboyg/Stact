namespace Magnum.Specs.Calendar
{
    using System;
    using Magnum.Calendar;
    using NUnit.Framework;

    [TestFixture]
    public class IsHoliday_Specs
    {
        [SetUp]
        public void TestSetup()
        {
            DefaultCalendar.Define(c =>
            {
                c.AddHoliday("New Years", Month.IsJanuary(d => d.DayIs(1)));
                c.AddHoliday("New Years (Observed)", Month.IsJanuary(d =>
                {
                    d.IsMonday();
                    d.DayIs(2);
                }));
                c.AddHoliday("Martin Luther King Jr. Day (Observed)", Month.IsJanuary(d => d.NthDayOfMonth(3, DayOfWeek.Monday)));
                c.AddHoliday("Christmas Day", Month.IsDecember(d =>
                {
                    d.IsWeekday();
                    d.DayIs(25);
                }));
                c.AddHoliday("Labor Day", Month.IsSeptember(d => d.NthDayOfMonth(1, DayOfWeek.Monday)));
                c.AddHoliday("Columbus Day (Observed)", Month.IsOctober(d => d.NthDayOfMonth(2, DayOfWeek.Monday)));
                c.AddHoliday("Flag Day", Month.IsJune(d =>
                {
                    d.DayIs(14);
                    d.IsWeekday();
                }));
                c.AddHoliday("Flag Day (Observed)", Month.IsJune(d=>
                {
                    d.DayIs(15);
                    d.IsMonday();
                }));
                c.AddHoliday("Christmas Day", Month.IsDecember(d=>
                {
                    d.DayIs(25);
                }));
                c.AddHoliday("Christmas Day (Observed)", Month.IsDecember(d =>
                {
                    d.IsMonday();
                    d.DayIs(26);
                }));
                c.AddHoliday("Independence Day", Month.IsJuly(d=>d.DayIs(4)));
                c.AddHoliday("Independence Day", Month.IsJuly(d=>
                {
                    d.DayIs(5);
                    d.IsMonday();
                }));
                c.AddHoliday("US Memorial Day", Month.IsMay(d =>
                {
                    d.LastDayOfMonth(DayOfWeek.Monday);
                    //d.IsMonday();
                    //d.InRange(24, 31);
                }));
                c.AddHoliday("Presidents Day", Month.IsFebruary(d =>
                {
                    d.IsMonday();
                    d.NthDayOfMonth(3, DayOfWeek.Monday);
                }));
                c.AddHoliday("Thanksgiving Day", Month.IsNovember(d =>
                {
                    d.IsThursday();
                    d.NthDayOfMonth(3, DayOfWeek.Monday);
                }));
                c.AddHoliday("Veterans Day", Month.IsNovember(d =>
                {
                    d.DayIs(11);
                    d.IsWeekday();
                }));
                c.AddHoliday("Veterans Day (Observed)", Month.IsNovember(d =>
                {
                    d.DayIs(12);
                    d.IsMonday();
                }));

            });
        }

        [Test]
        public void Test_Christmas()
        {
            //var b = DefaultCalendar.Check(new DateTime(2009, 12, 25));
            //b.IsMatch.ShouldBeTrue();
        }
    }
}