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
namespace Magnum.Web.Actors
{
	using System;
	using System.Web;
	using System.Web.Script.Serialization;
	using Actions;
	using Channels;

	/// <summary>
	/// A handler is bound to the route by input type, which is the only thing used to build
	/// the route. 
	/// </summary>
	/// <typeparam name="TInput">The input model for the request</typeparam>
	/// <typeparam name="TOutput">The output model for the response</typeparam>
	public class ActorHttpAsyncHandler<TInput, TOutput> :
		IHttpAsyncHandler
		where TInput : HasOutputChannel<TOutput>
	{
		private readonly Channel<TInput> _input;
		private readonly TInput _inputModel;
		private JavaScriptSerializer _serializer;

		public ActorHttpAsyncHandler(TInput inputModel, Channel<TInput> input)
		{
			_input = input;
			_inputModel = inputModel;
			_serializer = new JavaScriptSerializer();
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
			var channel = new ConsumerChannel<TOutput>(new ThreadPoolActionQueue(), message =>
				{
					context.Response.ContentType = "application/json";
					var body = _serializer.Serialize(message);
					context.Response.Write(body);
				});

			var outputChannel = new AsyncResultChannel<TOutput>(channel, cb, extraData);
			_inputModel.OutputChannel = outputChannel;
			_input.Send(_inputModel);

			return outputChannel;
		}

		public void EndProcessRequest(IAsyncResult result)
		{
		}
	}
}