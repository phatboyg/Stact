namespace Magnum.Metrics.Specs
{
    using System;
    using NUnit.Framework;

    [TestFixture]
    public class IisLogReader_Specs
    {
        private const string data = @"#Software: Microsoft Internet Information Services 6.0
#Version: 1.0
#Date: 2008-11-04 03:37:15
#Fields: date time s-sitename s-computername s-ip cs-method cs-uri-stem cs-uri-query s-port cs-username c-ip cs-version cs(User-Agent) cs(Cookie) cs(Referer) cs-host sc-status sc-substatus sc-win32-status sc-bytes cs-bytes time-taken 
2008-11-04 03:37:15 W3SVC1 2008XW2K3 127.0.0.1 GET /iisstart.htm - 80 - 127.0.0.1 HTTP/1.1 Mozilla/4.0+(compatible;+MSIE+7.0;+Windows+NT+5.2;+.NET+CLR+1.1.4322;+.NET+CLR+2.0.50727;+.NET+CLR+3.0.04506.30;+.NET+CLR+3.0.04506.648;+.NET+CLR+3.5.21022;+MS-RTC+LM+8) - - localhost 200 0 0 1731 484 1187
2008-11-04 03:37:15 W3SVC1 2008XW2K3 127.0.0.1 GET /pagerror.gif - 80 - 127.0.0.1 HTTP/1.1 Mozilla/4.0+(compatible;+MSIE+7.0;+Windows+NT+5.2;+.NET+CLR+1.1.4322;+.NET+CLR+2.0.50727;+.NET+CLR+3.0.04506.30;+.NET+CLR+3.0.04506.648;+.NET+CLR+3.5.21022;+MS-RTC+LM+8) - http://localhost/ localhost 200 0 0 3055 364 31
2008-11-04 03:37:15 W3SVC1 2008XW2K3 127.0.0.1 GET /favicon.ico - 80 - 127.0.0.1 HTTP/1.1 Mozilla/4.0+(compatible;+MSIE+7.0;+Windows+NT+5.2;+.NET+CLR+1.1.4322;+.NET+CLR+2.0.50727;+.NET+CLR+3.0.04506.30;+.NET+CLR+3.0.04506.648;+.NET+CLR+3.5.21022;+MS-RTC+LM+8) - - localhost 404 0 2 1795 311 31
2008-11-04 03:37:15 W3SVC1 2008XW2K3 127.0.0.1 GET /iisstart.htm - 80 - 127.0.0.1 HTTP/1.1 Mozilla/4.0+(compatible;+MSIE+7.0;+Windows+NT+5.2;+.NET+CLR+1.1.4322;+.NET+CLR+2.0.50727;+.NET+CLR+3.0.04506.30;+.NET+CLR+3.0.04506.648;+.NET+CLR+3.5.21022;+MS-RTC+LM+8) - - localhost 200 0 0 1682 496 0
";

        [Test]
        public void The_number_of_lines_returned_should_be_correct()
        {
            ILineReader lineReader = new LineReader(data);

            int lineCount = 0;
            foreach(string line in lineReader)
            {
                lineCount++;
            }

            Assert.AreEqual(8, lineCount);
        }

        [Test]
        public void The_log_reader_should_be_smart()
        {
            ILineReader lineReader = new LineReader(data);
            IisLogReader logReader = new IisLogReader(lineReader);

            DateTime expected = new DateTime(2008, 11, 4, 3, 37, 15);
            int entryCount = 0;
            foreach (IisLogEntry entry in logReader)
            {
                entryCount++;

                Assert.AreEqual(expected, entry.Date);
            }

            Assert.AreEqual(4, entryCount);

        }
    }
}