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
namespace Stact.Workflow
{
	using System;
	using Configuration;


	public static class RespondExtensions
	{
		public static ActivityConfigurator<TWorkflow, TInstance, Request<TRequest>> Respond
			<TWorkflow, TInstance, TRequest, TResponse>(
			this ActivityConfigurator<TWorkflow, TInstance, Request<TRequest>> activityConfigurator,
			Func<TInstance, Request<TRequest>, TResponse> getResponse)
			where TWorkflow : class
			where TInstance : class
		{
			var configurator =
				new DelegateInstanceBodyConfigurator<TWorkflow, TInstance, Request<TRequest>>((instance, request) =>
					{
						TResponse response = getResponse(instance, request);

						request.Respond(response);
					});

			activityConfigurator.AddConfigurator(configurator);

			return activityConfigurator;
		}

		public static ActivityConfigurator<TWorkflow, TInstance, Request<TRequest>> Respond
			<TWorkflow, TInstance, TRequest, TResponse>(
			this ActivityConfigurator<TWorkflow, TInstance, Request<TRequest>> activityConfigurator,
			Func<Request<TRequest>, TResponse> getResponse)
			where TWorkflow : class
			where TInstance : class
		{
			var configurator =
				new DelegateInstanceBodyConfigurator<TWorkflow, TInstance, Request<TRequest>>((instance, request) =>
					{
						TResponse response = getResponse(request);

						request.Respond(response);
					});

			activityConfigurator.AddConfigurator(configurator);

			return activityConfigurator;
		}

		public static ActivityConfigurator<TWorkflow, TInstance, Request<TRequest>> Respond
			<TWorkflow, TInstance, TRequest, TResponse>(
			this ActivityConfigurator<TWorkflow, TInstance, Request<TRequest>> activityConfigurator,
			Func<TResponse> getResponse)
			where TWorkflow : class
			where TInstance : class
		{
			var configurator =
				new DelegateInstanceBodyConfigurator<TWorkflow, TInstance, Request<TRequest>>((instance, request) =>
					{
						TResponse response = getResponse();

						request.Respond(response);
					});

			activityConfigurator.AddConfigurator(configurator);

			return activityConfigurator;
		}
	}
}