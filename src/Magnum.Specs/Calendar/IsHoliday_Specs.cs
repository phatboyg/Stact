namespace Magnum.Specs.Calendar
{
    using System;
    using Magnum.Calendar;
    using MbUnit.Framework;

    [TestFixture]
    public class IsHoliday_Specs
    {
        [Test]
        public void TestSetup()
        {
            IsHoliday.Define(c=>
                                 {
                                     c.AddHoliday("New Years", Month.IsJanuary(d=>d.DayIs(1)));
                                     c.AddHoliday("New Years (Observed)", Month.IsJanuary(d=>
                                                                                              {
                                                                                                  d.IsMonday();
                                                                                                  d.DayIs(2);
                                                                                              }));
                                     c.AddHoliday("Martin Luther King Jr. Day (Observed)", Month.IsJanuary(d=>
                                                                                                    {
                                                                                                        d.IsMonday();
                                                                                                        d.NthDayOfMonth(3, DayOfWeek.Monday);
                                                                                                    }));
                                     c.AddHoliday("Christmas Day", Month.IsDecember(d=>
                                                                                        {
                                                                                            d.IsWeekday();
                                                                                            d.DayIs(25);
                                                                                        }));
                                     c.AddHoliday("Labor Day", Month.IsSeptember(d=> { }));
                                     c.AddHoliday("Christmas Day (Observed)", Month.IsDecember(d=>
                                                                                                   {
                                                                                                       d.IsMonday();
                                                                                                       d.DayIs(26);
                                                                                                   }));
                                     c.AddHoliday("US Memorial Day", Month.IsMay(d=>
                                                                                     {
                                                                                         d.IsMonday();
                                                                                         d.InRange(24, 31);
                                                                                     }));
                                     c.AddHoliday("Presidents Day", Month.IsFebruary(d=>
                                                                                         {
                                                                                             d.IsMonday();
                                                                                             d.NthDayOfMonth(3, DayOfWeek.Monday);
                                                                                         }));
                                     c.AddHoliday("Thanksgiving Day", Month.IsNovember(d=>
                                                                                           {
                                                                                               d.IsThursday();
                                                                                               d.NthDayOfMonth(3, DayOfWeek.Monday);
                                                                                           }));
                                     c.AddHoliday("Veterans Day", Month.IsNovember(d=>
                                                                                       {
                                                                                           d.DayIs(11);
                                                                                           d.IsWeekday();
                                                                                       }));
                                     c.AddHoliday("Veterans Day (Observed)", Month.IsNovember(d=>
                                                                                                  {
                                                                                                      d.DayIs(12);
                                                                                                      d.IsMonday();
                                                                                                  }));

                                 });
        }
    }
}