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
namespace Magnum.Specs.Threading
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Threading;
    using Magnum.DateTimeExtensions;
    using Magnum.Threading;
    using MbUnit.Framework;

    [TestFixture]
    public class ErlangInfluencedStyle_Specs
    {
        [Test]
        public void The_task_should_be_able_to_run_asynchronously()
        {
            ContentLoader loader = new ContentLoader();

            ManualResetEvent _completed = new ManualResetEvent(false);

            IAsyncResult asyncResult = AsyncExecutor.RunAsync(loader.Load, () => _completed.Set());

            Assert.IsTrue(asyncResult.AsyncWaitHandle.WaitOne(10.Seconds(), true), "No response from the executor");

            Assert.IsTrue(_completed.WaitOne(0, true), "Should have completed by now");

            Trace.WriteLine("Content Length: " + loader.TotalBytesRead);
        }

        [Test]
        public void The_task_should_run_synchronously()
        {
            ContentLoader loader = new ContentLoader();

            AsyncExecutor.Run(loader.Load);

            Assert.IsTrue(loader.Completed);

            Trace.WriteLine("Content Length: " + loader.TotalBytesRead);
        }
    }

    public class ContentLoader
    {
        public long TotalBytesRead { get; private set; }

        public bool Completed { get; private set; }

        public IEnumerator<int> Load(IAsyncExecutor async)
        {
            WebRequest request = WebRequest.Create("http://blog.phatboyg.com/");

            request.BeginGetResponse(async.End(), null);

            yield return 1;

            using (WebResponse response = request.EndGetResponse(async.Result()))
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    var blockSize = 16384;
                    byte[] block = new byte[blockSize];

                    int bytesRead;
                    do
                    {
                        responseStream.BeginRead(block, 0, blockSize, async.End(), null);

                        yield return 1;

                        bytesRead = responseStream.EndRead(async.Result());

                        TotalBytesRead += bytesRead;
                    } while (bytesRead != 0);
                }
            }

            Completed = true;
        }
    }
}