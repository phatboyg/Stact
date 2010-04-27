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
namespace Magnum.Web.Actors.Internal
{
	using System;
	using System.Threading;
	using System.Web.Routing;
	using Abstractions;
	using Channels;
	using Fibers;
	using Logging;
	using Magnum.ValueProviders;
	using Reflection;
	using ValueProviders;

	public class HttpActorRequestContext :
		ActorRequestContext
	{
		private static readonly ILogger _log = Logger.GetLogger<HttpActorRequestContext>();

		private readonly ContentWriter _contentWriter;
		private readonly ObjectWriter _objectWriter;
		private readonly Fiber _fiber;
		private readonly ValueProvider _valueProvider;
		private AsyncCallback _callback;
		private volatile bool _completed;
		private Type _responseType;
		private object _state;

		public HttpActorRequestContext(Fiber fiber, RequestContext requestContext)
		{
			_fiber = fiber;

			_valueProvider = new RequestContextValueProvider(requestContext);
			_contentWriter = new HttpResponseContentWriter(requestContext.HttpContext.Response);
			_objectWriter = new JsonObjectWriter(_contentWriter, _valueProvider);
		}

		public bool GetValue(string key, Func<object, bool> matchingValueAction)
		{
			return _valueProvider.GetValue(key, matchingValueAction);
		}

		public bool GetValue(string key, Func<object, bool> matchingValueAction, Action missingValueAction)
		{
			return _valueProvider.GetValue(key, matchingValueAction, missingValueAction);
		}

		public void GetAll(Action<string, object> valueAction)
		{
			_valueProvider.GetAll(valueAction);
		}

		public Channel<T> GetChannel<T>()
		{
			return new ObjectWriterChannel<T>(_fiber, _objectWriter, Complete);
		}

		public void SetCallback(AsyncCallback callback, object state)
		{
			_callback = callback;
			_state = state;
		}

		public bool IsCompleted
		{
			get { return _completed; }
		}

		public WaitHandle AsyncWaitHandle
		{
			get { throw new NotSupportedException("Wait handles are not supported by the channel framework"); }
		}

		public object AsyncState
		{
			get { return _state; }
		}

		public bool CompletedSynchronously
		{
			get { return false; }
		}

		public Channel GetChannel(Type channelType)
		{
			return this.FastInvoke<HttpActorRequestContext, Channel>(new[] {channelType}, "GetChannel");
		}

		public override string ToString()
		{
			string actor = "Unknown";

			GetValue("Actor", x => (actor = x.ToString()) != null);

			return string.Format("{0}", actor);
		}

		private void Complete<T>(Channel<T> channel)
		{
			_log.Debug(x => x.Write("Response[{0}]: {1}", Thread.CurrentThread.ManagedThreadId, typeof (T).FullName));

			_responseType = typeof (T);
			_completed = true;

			if (_callback != null)
				_callback(this);
		}
	}
}