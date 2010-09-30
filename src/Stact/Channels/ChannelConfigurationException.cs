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
namespace Stact.Channels
{
	using System;
	using System.Runtime.Serialization;
	using Magnum.Extensions;

	/// <summary>
	/// Thrown when an invalid configuration is supplied when configuring a channel
	/// </summary>
	[Serializable]
	public class ChannelConfigurationException :
		Exception
	{
		public ChannelConfigurationException()
		{
		}

		public ChannelConfigurationException(string message)
			: base(message)
		{
		}

		public ChannelConfigurationException(Type channelType, string message)
			: this("{0}, Channel Type: {1}".FormatWith(message, channelType.Name))
		{
		}

		public ChannelConfigurationException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		public ChannelConfigurationException(Type channelType, string message, Exception innerException)
			: this("{0}, Channel Type: {1}".FormatWith(message, channelType.Name), innerException)
		{
		}

		protected ChannelConfigurationException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}