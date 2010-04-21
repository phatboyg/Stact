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

	/// <summary>
	/// Maintains only one instance of an actor per thread
	/// </summary>
	/// <typeparam name="TActor">The actor type</typeparam>
	public class ThreadStaticActorInstanceProvider<TActor> :
		ActorInstanceProvider<TActor>
		where TActor : class
	{
		[ThreadStatic]
		private static TActor _instance;

		public ThreadStaticActorInstanceProvider(ActorInstanceProvider<TActor> instanceProvider)
		{
			InstanceProvider = instanceProvider;
		}

		public ActorInstanceProvider<TActor> InstanceProvider { get; private set; }

		public TActor GetActor()
		{
			if (_instance == null)
			{
				_instance = InstanceProvider.GetActor();
			}

			return _instance;
		}
	}
}