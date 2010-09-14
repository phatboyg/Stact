// Copyright 2007-2010 The Apache Software Foundation.
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
namespace Magnum.Actors.Internal
{
	using System;


	/// <summary>
	///   Maintains only one instance of an actor per thread
	/// </summary>
	/// <typeparam name = "TActor">The actor type</typeparam>
	public class ThreadStaticActorFactory<TActor> :
		ActorFactory<TActor>
		where TActor : class, Actor
	{
		[ThreadStatic]
		static ActorInstance _instance;

		public ThreadStaticActorFactory(ActorFactory<TActor> factory)
		{
			Factory = factory;
		}

		public ActorFactory<TActor> Factory { get; private set; }

		public ActorInstance GetActor()
		{
			return _instance ?? (_instance = Factory.GetActor());
		}
	}
}