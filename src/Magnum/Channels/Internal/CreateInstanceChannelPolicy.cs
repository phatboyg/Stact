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
namespace Magnum.Channels.Internal
{
	public class CreateInstanceChannelPolicy<T, TChannel> :
		InstanceChannelPolicy<T, TChannel>
		where T : class
	{
		readonly InstanceProvider<T, TChannel> _missingInstanceProvider;

		public CreateInstanceChannelPolicy(InstanceProvider<T, TChannel> missingInstanceProvider)
		{
			_missingInstanceProvider = missingInstanceProvider;
		}

		public bool CanCreateInstance(TChannel message, out T instance)
		{
			instance = _missingInstanceProvider.GetInstance(message);

			return true;
		}

		public bool IsHandledByExistingInstance(TChannel message)
		{
			return false;
		}

		public void WasNotHandled(TChannel message)
		{
		}

		public bool CanUnloadInstance(T instance)
		{
			return false;
		}

		public bool CanRemoveInstance(T instance)
		{
			return false;
		}
	}
}