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
	using System.Diagnostics;
	using Internal;
	using Magnum.Extensions;
	using Magnum.Reflection;


	public class TraceStateMachineVisitor :
		ReflectiveVisitorBase<TraceStateMachineVisitor>,
		StateMachineVisitor
	{
		protected virtual bool Visit<TInstance>(State<TInstance> state)
		{
			Trace.WriteLine("(" + state.Name + ")");

			return true;
		}

		protected virtual bool Visit(SimpleEvent simpleEvent)
		{
			Trace.WriteLine("[" + simpleEvent.Name + "]");

			return true;
		}

		protected virtual bool Visit<TBody>(MessageEvent<TBody> messageEvent)
		{
			Trace.WriteLine("[" + messageEvent.Name + "<" + typeof(TBody).ToShortTypeName() +  ">]");

			return true;
		}

		protected virtual bool Visit<TInstance>(Activity<TInstance> activity)
			where TInstance : class
		{
			Trace.WriteLine("({0})[{1}] Unknown Action".FormatWith(activity.State, activity.Event));

			return true;
		}

		protected virtual bool Visit<TInstance>(TransitionActivity<TInstance> activity)
			where TInstance : class
		{
			Trace.WriteLine("({0})[{1}] ==> ({2})".FormatWith(activity.State, activity.Event, activity.TargetState));

			return true;
		}

		protected virtual bool Visit<TInstance>(MethodActivity<TInstance> activity)
			where TInstance : class
		{
			Trace.WriteLine("({0})[{1}] {2}.{3}".FormatWith(activity.State, activity.Event,
			                                                typeof(TInstance).ToShortTypeName(), activity.MethodName));

			return true;
		}

		protected virtual bool Visit<TInstance, TBody>(MethodBodyActivity<TInstance, TBody> activity)
			where TInstance : class
		{
			Trace.WriteLine("({0})[{1}] {2}.{3}({4})".FormatWith(activity.State, activity.Event,
			                                                     typeof(TInstance).ToShortTypeName(), activity.MethodName,
			                                                     typeof(TBody).ToShortTypeName()));
			return true;
		}

		protected virtual bool Visit<TInstance>(DelegateActivity<TInstance> activity)
			where TInstance : class
		{
			Trace.WriteLine("({0})[{1}] Action({2})".FormatWith(activity.State, activity.Event,
			                                                     typeof(TInstance).ToShortTypeName()));

			return true;
		}

		protected virtual bool Visit<TInstance>(DelegateInstanceActivity<TInstance> activity)
			where TInstance : class
		{
			Trace.WriteLine("({0})[{1}] Action({2})".FormatWith(activity.State, activity.Event,
			                                                     typeof(TInstance).ToShortTypeName()));

			return true;
		}

		protected virtual bool Visit<TInstance, TBody>(DelegateInstanceBodyActivity<TInstance, TBody> activity)
			where TInstance : class
		{
			Trace.WriteLine("({0})[{1}] Action({2},{3})".FormatWith(activity.State, activity.Event,
			                                                        typeof(TInstance).ToShortTypeName(),
			                                                        typeof(TBody).ToShortTypeName()));
			return true;
		}

		protected virtual bool Visit<TWorkflow, TInstance>(StateMachineWorkflow<TWorkflow, TInstance> workflow)
			where TWorkflow : class
			where TInstance : class
		{
			return true;
		}
	}
}