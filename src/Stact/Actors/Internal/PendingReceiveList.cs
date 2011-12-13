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
namespace Stact.Internal
{
	using System.Collections;
	using System.Collections.Generic;
	using Magnum.Extensions;


	public class PendingReceiveList :
		IEnumerable<ReceiveHandle>
	{
		readonly IList<ReceiveHandle> _receives;

		public PendingReceiveList()
		{
			_receives = new List<ReceiveHandle>();
		}

		public IEnumerator<ReceiveHandle> GetEnumerator()
		{
			return _receives.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void Add(ReceiveHandle pendingReceive)
		{
			_receives.Add(pendingReceive);
		}

		public void CancelAll()
		{
			_receives.Each(receive =>
				{
					try
					{
						receive.Cancel();
					}
					catch
					{
					}
				});

			_receives.Clear();
		}
	}
}