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
namespace Stact
{
	using System;


	public class Identity<T>
	{
		public Identity(T value)
		{
			Value = value;
		}

		public T Value { get; private set; }

		public static Func<T, T> Function
		{
			get { return x => x; }
		}

		public static Identity<T> Unit(T value)
		{
			return new Identity<T>(value);
		}

		public static Identity<U> Bind<U>(
			Identity<T> argument,
			Func<T, Identity<U>> operation)
		{
			return operation(argument.Value);
		}
	}
}