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
namespace Stact.Specs.Usage
{
	using System;
	using System.Collections.Generic;
	using Magnum.TestFramework;


//	[Scenario]
//	public class Using_distribution_keys_to_create_aggregates
//	{
//		ChannelConnection _connection;
//		UntypedChannel _input;
//		IList<Demographics> _demographicses;
//
//		[When]
//		public void Distribution_keys_by_type()
//		{
//			_demographicses = new List<Demographics>();
//
//			var instanceProvider = new DelegateInstanceProvider<Demographics, Admission>(x =>
//				{
//					var demographics = new Demographics(x.Gender, GetBracket(x.Age));
//
//					_demographicses.Add(demographics);
//
//					return demographics;
//				});
//
//			_input = new ChannelAdapter();
//			_connection = _input.Connect(x =>
//				{
//					x.AddConsumerOf<Admission>()
//						.UsingInstance().Of<Demographics>()
//						.DistributedBy(y => y.Gender)
//						.HandleOnCallingThread()
//						.DistributedBy(y => GetBracket(y.Age))
//						.HandleOnCallingThread()
//						.ObtainedBy(instanceProvider)
//						.OnChannel(y => y.AdmissionChannel);
//				});
//
//			PumpData(_input);
//		}
//
//		[Then]
//		public void Should_have_three_entries()
//		{
//			_demographicses.Count.ShouldEqual(3);
//		}
//
//		static void PumpData(UntypedChannel channel)
//		{
//			channel.Send(new Admission("M", 18));
//			channel.Send(new Admission("F", 25));
//			channel.Send(new Admission("M", 40));
//		}
//
//		static int GetBracket(int age)
//		{
//			if (age >= 65)
//				return 65;
//			if (age >= 50)
//				return 50;
//			if (age >= 35)
//				return 35;
//			if (age >= 25)
//				return 25;
//			if (age >= 18)
//				return 18;
//
//			return 17;
//		}
//
//		[Finally]
//		public void Finally()
//		{
//			_connection.Dispose();
//		}
//
//
//		class Admission
//		{
//			public Admission(string gender, int age)
//			{
//				Gender = gender;
//				Age = age;
//			}
//
//			public string Gender { get; private set; }
//			public int Age { get; private set; }
//		}
//
//
//		class Demographics
//		{
//			int _count;
//
//			public Demographics(string gender, int ageBracket)
//			{
//				AdmissionChannel = new ConsumerChannel<Admission>(new SynchronousFiber(), HandleAdmission);
//			}
//
//
//			public Channel<Admission> AdmissionChannel { get; private set; }
//
//			void HandleAdmission(Admission message)
//			{
//				_count++;
//			}
//		}
//	}
}