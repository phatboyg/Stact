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
namespace Magnum.Channels.Internal
{
	using System.ServiceModel;

	/// <summary>
	///   A single generic channel type used for local channels via WCF/named pipes
	/// </summary>
	/// <typeparam name = "T"></typeparam>
	[ServiceContract(Namespace = "http://magnum-project.net/LocalWcfChannel")]
	public interface WcfChannel<T>
	{
		[OperationContract(IsOneWay = true, IsInitiating = true, IsTerminating = false)]
		void Send(T message);
	}
}