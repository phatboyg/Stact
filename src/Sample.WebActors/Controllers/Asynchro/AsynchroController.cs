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
namespace Sample.WebActors.Controllers.Asynchro
{
	using System;
	using System.Threading;
	using System.Web.Mvc;
	using Magnum;
	using Magnum.Channels;
	using Magnum.Extensions;
	using Magnum.Fibers;

	public class AsynchroController :
		AsyncController
	{
		public void IndexAsync()
		{
			AsyncManager.OutstandingOperations.Increment(2);

			Channel<QueryContent> channel = new ConsumerChannel<QueryContent>(new ThreadPoolFiber(), QueryService);

			channel.Send(new QueryContent(37, message =>
				{
					AsyncManager.Parameters["first"] = message;
					AsyncManager.OutstandingOperations.Decrement();
				}));

			channel.Send(new QueryContent(27, message =>
				{
					AsyncManager.Parameters["second"] = message;
					AsyncManager.OutstandingOperations.Decrement();
				}));
		}

		public ActionResult IndexCompleted(MyContent first, MyContent second)
		{
			return View(new AsynchroIndexViewModel
				{
					Title = first.Title,
					Detail = first.Detail,
					Body = second.Title + second.Detail,
				});
		}

		public class QueryContent
		{
			public QueryContent(int id, Consumer<MyContent> callback)
			{
				Id = id;

				var queue = new ThreadPoolFiber();
				Channel<MyContent> channel = new ConsumerChannel<MyContent>(new SynchronousFiber(), callback);

				if (SynchronizationContext.Current != null)
				{
					channel = new SynchronizedChannel<MyContent>(queue, channel, SynchronizationContext.Current);
				}
				ResponseChannel = channel;
			}

			public int Id { get; private set; }

			public Channel<MyContent> ResponseChannel { get; private set; }
		}

		public class MyContent
		{
			public string Title { get; set; }
			public string Detail { get; set; }
		}

		private void QueryService(QueryContent message)
		{
			ThreadUtil.Sleep((message.Id*100).Milliseconds());
			message.ResponseChannel.Send(new MyContent
				{
					Title = "Content Id " + message.Id,
					Detail = "Details of Content Id " + message.Id + " created at " + DateTime.Now.ToLongTimeString(),
				});
		}
	}
}