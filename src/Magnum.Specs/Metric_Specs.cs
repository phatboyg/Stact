// Copyright 2007-2008 The Apache Software Foundation.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace Magnum.Specs
{
    using log4net;
    using NUnit.Framework;

    [TestFixture]
    public class When_tracking_metrics_on_a_function
    {
        private static readonly ILog _log = LogManager.GetLogger("Default");

        private static void VerifyCalled(string text)
        {
            Assert.AreEqual(5, text.Split(' ').Length, "Expected two values and the description past the date/time");
        }

        [Test]
        public void The_tracker_should_do_its_job()
        {
            string header;
            using (var tracker = new FunctionTimer("TEST", VerifyCalled))
            {
                using (var mark = tracker.Mark())
                {
                }

                header = tracker.Header;
            }

            Assert.AreEqual("Date Time TimeTaken Mark1 Description", header);
        }
    }

    [TestFixture]
    public class When_using_a_class_derived_from_the_tracker
    {
        private static void VerifyCalled(string text)
        {
            string[] columns = text.Split(' ');

            Assert.AreEqual(6, columns.Length, "Expected two values and the description past the date/time");
        }

        [Test]
        public void The_output_should_include_property_values()
        {
            using (var tracker = new FunctionTimer<MyData>("TEST", VerifyCalled))
            {
                tracker.Values.QueryCount = 5;
                tracker.Values.QueryAmount = 123.45m;
            }
        }
    }

    internal class MyData
    {
        public int QueryCount { get; set; }
        public decimal QueryAmount { get; set; }
    }
}