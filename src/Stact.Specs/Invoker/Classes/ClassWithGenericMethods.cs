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
namespace Stact.Specs.Invoker.Classes
{
	using System;
	using Activator.Classes;

	public class ClassWithGenericMethods
	{
		public bool OneGenericArgumentCalled { get; private set; }
		public bool TwoGenericArgumentsCalled { get; private set; }
		public bool OneGenericArgumentNoParametersCalled { get; private set; }
		public bool TwoGenericArgumentsOneParameterCalled { get; private set; }
		public bool OneGenericArgumentOnUnrelatedParameterCalled { get; private set; }

		public Type FirstArgumentType { get; private set; }
		public object FirstArgumentValue { get; private set; }
		public Type SecondArgumentType { get; private set; }
		public object SecondArgumentValue { get; private set; }

		public void OneGenericArgument<T>(T argument)
		{
			OneGenericArgumentCalled = true;
			FirstArgumentType = typeof (T);
			FirstArgumentValue = argument;
		}

		public void TwoGenericArguments<T, V>(T argumentT, V argumentV)
		{
			TwoGenericArgumentsCalled = true;
			FirstArgumentType = typeof (T);
			FirstArgumentValue = argumentT;

			SecondArgumentType = typeof (V);
			SecondArgumentValue = argumentV;
		}

		public void TwoGenericArgumentsOneParameter<T, V>(T argument)
			where T : ConstrainedBy<V>
		{
			TwoGenericArgumentsOneParameterCalled = true;

			FirstArgumentType = typeof (T);
			SecondArgumentType = typeof (V);
			FirstArgumentValue = argument;
		}

		public void OneGenericArgumentNoParameters<T>()
		{
			OneGenericArgumentNoParametersCalled = true;

			FirstArgumentType = typeof (T);
		}

		public void OneGenericArgumentOnUnrelatedParameter<T>(string name)
		{
			OneGenericArgumentOnUnrelatedParameterCalled = true;

			FirstArgumentType = typeof (T);
			SecondArgumentType = typeof (string);
			SecondArgumentValue = name;
		}
	}
}