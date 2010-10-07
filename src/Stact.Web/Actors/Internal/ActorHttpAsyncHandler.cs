// Copyright 2010 Chris Patterson
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
namespace Stact.Web.Actors.Internal
{
	using System;
	using System.Threading;
	using System.Web;
	
	using Magnum.Logging;

	/// <summary>
	/// A handler is bound to the route by input type, which is the only thing used to build
	/// the route. 
	/// </summary>
	/// <typeparam name="TInput">The input model for the request</typeparam>
	public class ActorHttpAsyncHandler<TInput> :
		IHttpAsyncHandler
	{
		private static readonly ILogger _log = Logger.GetLogger<ActorHttpAsyncHandler<TInput>>();

		private readonly ActorRequestContext _context;
		private readonly Channel<TInput> _input;
		private readonly TInput _inputModel;

		public ActorHttpAsyncHandler(ActorRequestContext context, TInput inputModel, Channel<TInput> input)
		{
			_context = context;
			_input = input;
			_inputModel = inputModel;
		}

		public void ProcessRequest(HttpContext context)
		{
			throw new InvalidOperationException("This should not be called since we are an asynchronous handler");
		}

		public bool IsReusable
		{
			get { return false; }
		}

		public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData)
		{
			_log.Debug(x => x.Write("Request[{0}]: {1}", Thread.CurrentThread.ManagedThreadId, typeof (TInput).FullName));

			_context.SetCallback(cb, extraData);

			_input.Send(_inputModel);

			return _context;
		}

		public void EndProcessRequest(IAsyncResult result)
		{
		}
	}
}