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
	using System.IO;
	using System.Text;
	using System.Web;
	using System.Web.Script.Serialization;
	using Abstractions;
	using Binding;
	using Channels;
	using StreamExtensions;
	using ValueProviders;

	public class BasicActorBinder<TInput> :
		ActorBinder
	{
		private readonly Func<Channel<TInput>> _getInputChannel;
		private readonly ModelBinder _modelBinder;

		public BasicActorBinder(ModelBinder modelBinder, Func<Channel<TInput>> getInputChannel)
		{
			_modelBinder = modelBinder;
			_getInputChannel = getInputChannel;
		}

		public IHttpAsyncHandler GetHandler(ActorRequestContext context)
		{
			TInput inputModel;
			if (!BindJsonRequest(context, out inputModel))
			{
				inputModel = (TInput) _modelBinder.Bind(typeof (TInput), context);
			}

			var handler = new ActorHttpAsyncHandler<TInput>(context, inputModel, _getInputChannel());

			return handler;
		}

		private bool BindJsonRequest(ActorRequestContext context, out TInput model)
		{
			model = default(TInput);

			if (!context.IsAjaxRequest())
				return false;

			bool hasJsonContent = context.GetValue("ContentType", x => x.ToString().Contains(MediaType.Json.ToString()));
			if (!hasJsonContent)
				return false;

			string json = "";

			bool read = context.GetValue("InputStream", x =>
				{
					json = Encoding.UTF8.GetString(((Stream) x).ReadToEnd());
					return json.Length > 0;
				});

			if (!read)
				return false;

			model = new JavaScriptSerializer().Deserialize<TInput>(json);
			return true;
		}
	}
}