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
	using Binding;
	using Channels;
	using ValueProviders;

	public class BasicActorBinder<TInput, TOutput> :
		ActorBinder
		where TInput : HasOutputChannel<TOutput>

	{
		private readonly Func<Channel<TInput>> _getInputChannel;
		private readonly ModelBinder _modelBinder;

		public BasicActorBinder(ModelBinder modelBinder, Func<Channel<TInput>> getInputChannel)
		{
			_modelBinder = modelBinder;
			_getInputChannel = getInputChannel;
		}

		public IHttpAsyncHandler GetHandler(ValueProvider provider)
		{
			TInput inputModel = BindModel(provider);

			var handler = new ActorHttpAsyncHandler<TInput, TOutput>(inputModel, _getInputChannel());

			return handler;
		}

		private TInput BindModel(ValueProvider provider)
		{
			ModelBinderContext context = new ActorBinderContext(provider);

			var model = (TInput) _modelBinder.Bind(typeof (TInput), context);

			return model;
		}
	}
}