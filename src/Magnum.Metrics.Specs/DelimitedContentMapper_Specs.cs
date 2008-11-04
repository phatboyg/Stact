namespace Magnum.Metrics.Specs
{
    using System;
    using Common;
    using NUnit.Framework;

    [TestFixture]
    public class DelimitedContentMapper_Specs
    {
        [Test]
        public void The_content_should_be_mapped_to_the_class()
        {
            string line = "2008-10-31 19:17:31 1723 123 456";

            string[] splitLine = line.Split(' ');

            Mapper<string[], LogInfo> map = new Mapper<string[], LogInfo>();
            map.From(x => DateTime.Parse(string.Join(" ", x, 0, 2))).To(y => y.Date);
            map.From(x => int.Parse(x[2])).To(y => y.TimeTaken);
            map.From(x => int.Parse(x[3])).To(y => y.QueryTime);
            map.From(x => int.Parse(x[4])).To(y => y.FormatTime);


            LogInfo logInfo = map.Transform(splitLine);

            Assert.AreEqual(new DateTime(2008, 10, 31, 19, 17, 31, 0), logInfo.Date);
            Assert.AreEqual(1723, logInfo.TimeTaken);
            Assert.AreEqual(123, logInfo.QueryTime);
            Assert.AreEqual(456, logInfo.FormatTime);
        }
    }

    public class LogInfo
    {
        public DateTime Date { get; set; }
        public int TimeTaken { get; set; }
        public int QueryTime { get; set; }
        public int FormatTime { get; set; }
    }
}