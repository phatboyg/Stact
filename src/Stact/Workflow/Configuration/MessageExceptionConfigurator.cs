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
namespace Stact.Workflow.Configuration
{
	using System;


	public interface MessageExceptionConfigurator<TWorkflow, TInstance, TBody>
		where TWorkflow : class
		where TInstance : class
	{
		MessageExceptionConfigurator<TWorkflow, TInstance, TException, TBody> Exception<TException>()
			where TException : Exception;
	}


	public interface MessageExceptionConfigurator<TWorkflow, TInstance, TException, TBody> :
		MessageExceptionConfigurator<TWorkflow,TInstance,TBody>,
		ExceptionConfigurator<TWorkflow, TInstance, TException>
		where TWorkflow : class
		where TInstance : class
		where TException : Exception
	{
		void AddConfigurator(ActivityBuilderConfigurator<TWorkflow, TInstance, TBody> configurator);
	}
}