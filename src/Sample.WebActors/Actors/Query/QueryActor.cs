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
namespace Sample.WebActors.Actors.Query
{
	using System.Threading;
	using Echo;
	using Stact.Channels;
	using Stact.Fibers;
	using Stact.Logging;

	public class QueryActor
	{
		private static readonly ILogger _log = Logger.GetLogger<EchoActor>();

		private readonly Fiber _fiber;

		public QueryActor(Fiber fiber)
		{
			_fiber = fiber;

			GetCityChannel = new ConsumerChannel<QueryInputModel>(_fiber, ProcessRequest);
		}

		public Channel<QueryInputModel> GetCityChannel { get; private set; }
		public Channel<QueryInputModel> GetAreaCodeChannel { get; private set; }

		private void ProcessRequest(QueryInputModel inputModel)
		{
			_log.Debug(x => x.Write("Query[{0}]: {1}", Thread.CurrentThread.ManagedThreadId, inputModel.ZipCode));

			inputModel.OutputChannel.Send(new QueryOutputModel
				{
					City = "Tulsa",
					State = "OK",
					YourIp = inputModel.UserHostAddress,
				});
		}
	}
}