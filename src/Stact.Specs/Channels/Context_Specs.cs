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
namespace Stact.Specs.Channels
{
	using System;
	using TestFramework;


	[Scenario]
	public class Given_a_message_type
	{
		const string TestMessageType = "urn:message:Stact:Specs:Channels:TestMessageType";
		Uri _messageType;

		[Given]
		public void A_message_type()
		{
			_messageType = new Uri(TestMessageType);
		}

		[Then]
		public void Should_be_storable_in_a_uri()
		{
			_messageType.ToString().ShouldEqual(TestMessageType);
		}

		[Then]
		public void Should_have_an_absolute_path_only()
		{
			_messageType.AbsolutePath.ShouldEqual(TestMessageType.Substring(4));
		}
	}
}