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
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading;
    using DateTimeExtensions;
    using Magnum.Monads;
    using MbUnit.Framework;
    using Monads;

    [TestFixture]
    public class Continuation_Specs
    {
        [Test]
        public void I_want_a_nice_way_to_express_functions_without_all_the_noise()
        {
            var r = 5.ToIdentity().SelectMany(x => 6.ToIdentity(), (x, y) => x + y);

            Assert.AreEqual(11, r.Value);

            var t = from x in 5.ToIdentity()
                    from y in 6.ToIdentity()
                    select x + y;

            Assert.AreEqual(11, t.Value);
        }

        [Test]
        public void Should_propogate_nullable_values()
        {
            var r = from x in 5.ToMaybe() from y in Maybe<int>.Nothing select x + y;

            Assert.IsFalse(r.HasValue);
        }

        [Test]
        public void Lets_pull_some_magic_juice()
        {
            var requests = new[]
                               {
                                   WebRequest.Create("http://www.google.com/"),
                                   WebRequest.Create("http://www.yahoo.com/"),
                                   WebRequest.Create("http://channel9.msdn.com/")
                               };
            var pages = from request in requests
                        select
                            from response in AsyncExtensionsForStuff.GetResponseAsync(request)
                            let stream = response.GetResponseStream()
                            from html in stream.ReadToEndAsync()
                            select new {html, response};

            foreach (var page in pages)
            {
                page(d =>
                         {
                             Trace.WriteLine(d.response.ResponseUri.ToString());
                             Trace.WriteLine(d.html.Substring(0, 40));
                         });
            }

            Thread.Sleep(5000);

            Trace.WriteLine("Done");
        }


        [Test]
        public void Something_special_this_way_comes()
        {
            var r = from request in new[] {new MyRequestMessage()}
                    select
                        from response in request.MakeRequest()
                        let responseMessage = response.GetResponse<MyResponseMessage>()
                        let otherResponse = response.GetResponse<OtherResponseMessage>()
                        select new {Response = response, Message = responseMessage, OtherMessage = otherResponse};


            foreach (var reply in r)
            {
                reply(x =>
                          {
// so this gets called when the response is received I supppose
                          });
            }
        }
    }

    internal class OtherResponseMessage
    {}

    internal class MyResponseMessage
    {}

    public class MyRequestMessage
    {
        public void BeginGetResponse(AsyncCallback callback, object o)
        {
            Action makeCallback = () => callback(null);

            makeCallback.DeferFor(1.Seconds());
        }


        public RRScope EndGetResponse(IAsyncResult result)
        {
            return new RRScope();
        }
    }

    public static class sstuu
    {
        public static K<RRScope> MakeRequest(this MyRequestMessage request)
        {
            return respond => request.BeginGetResponse(result =>
                                                           {
                                                               var response = request.EndGetResponse(result);
                                                               respond(response);
                                                           }, null);
        }
    }

    public class RRScope
    {
        public T GetResponse<T>()
        {
            return default(T);
        }
    }

    public static class AsyncExtensionsForStuff
    {
        public static K<WebResponse> GetResponseAsync(this WebRequest request)
        {
            return respond => request.BeginGetResponse(result =>
                                                           {
                                                               var response = request.EndGetResponse(result);
                                                               respond(response);
                                                           }, null);
        }

        public static K<string> ReadToEndAsync(this Stream stream)
        {
            return respond =>
                       {
                           using (MemoryStream content = new MemoryStream())
                           {
                               byte[] buffer = new byte[1024];

                               Func<IAsyncResult> readChunk = null;

                               readChunk = () => stream.BeginRead(buffer, 0, 1024, result =>
                                                                                       {
                                                                                           int read = stream.EndRead(result);
                                                                                           if (read > 0)
                                                                                           {
                                                                                               content.Write(buffer, 0, read);

                                                                                               readChunk();
                                                                                           }
                                                                                           else
                                                                                           {
                                                                                               stream.Dispose();

                                                                                               respond(Encoding.UTF8.GetString(content.ToArray()));
                                                                                           }
                                                                                       }, null);
                               readChunk();
                           }
                       };
        }
    }
}