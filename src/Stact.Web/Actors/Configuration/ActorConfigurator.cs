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
namespace Stact.Web.Actors.Configuration
{
	using System;
	using System.Linq.Expressions;

	/// <summary>
	/// Configures the attributes of the actor being added
	/// </summary>
	public interface ActorConfigurator
	{
		/// <summary>
		/// Configures all channels available on the actor
		/// </summary>
		/// <returns>The configurator</returns>
		ActorConfigurator All();

		/// <summary>
		/// Specifies that only one instance of the actor should be created per thread,
		/// keeping the object count low but possibly limiting concurrency
		/// </summary>
		/// <returns>The configurator</returns>
		ActorConfigurator PerThread();
	}

	public interface ActorConfigurator<TActor> :
		ActorConfigurator
	{
		ActorConfigurator<TActor> Channel<TChannel>(Expression<Func<TActor, TChannel>> expression);
	}
}