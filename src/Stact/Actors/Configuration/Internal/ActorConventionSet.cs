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
namespace Stact.Configuration.Internal
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using Magnum;
	using Magnum.Extensions;


	public class ActorConventionSet<TActor> :
		IEnumerable<ActorConvention<TActor>>
		where TActor : Actor
	{
		readonly IList<ActorConvention<TActor>> _conventions;

		public ActorConventionSet(params ActorConvention<TActor>[] initialConventions)
		{
			_conventions = new List<ActorConvention<TActor>>();

			initialConventions
				.Each(Add);
		}

		public IEnumerator<ActorConvention<TActor>> GetEnumerator()
		{
			return _conventions.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void Add(ActorConvention<TActor> convention)
		{
			Guard.AgainstNull(convention, "convention");

			if (_conventions.Any(existingConvention => existingConvention.GetType() == convention.GetType()))
				return;

			_conventions.Add(convention);
		}
	}
}