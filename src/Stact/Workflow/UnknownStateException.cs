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
	using System.Runtime.Serialization;
	using Magnum.Extensions;


	[Serializable]
	public class UnknownStateException :
		StateMachineWorkflowException
	{
		public UnknownStateException(Type workflowType, string name)
			: base("The {0} state is not valid for the {1} workflow".FormatWith(name, workflowType.Name))
		{
		}

		protected UnknownStateException()
		{
		}

		protected UnknownStateException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}