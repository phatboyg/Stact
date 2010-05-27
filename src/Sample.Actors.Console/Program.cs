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
namespace Sample.Actors.Console
{
	using System;
	using Magnum.Actors;
	using Magnum.Extensions;
	using Magnum.Fibers;
	using Messages;
	using StructureMap;

	internal class Program
	{
		private static void Main(string[] args)
		{
			Console.WriteLine("Starting up...");

			new Bootstrapper().Bootstrap();

			var scheduler = ObjectFactory.GetInstance<Scheduler>();

			Actor auctioneer = ObjectFactory.GetInstance<Auctioneer>();

			RegisterSellers(auctioneer, 1000);
		}

		private static void RegisterSellers(Actor auctioneer, int count)
		{
			for (int i = 0; i < count; i++)
			{
				string name = "Seller {0}".FormatWith(i);
				var message = new RegisterSeller
					{
						Name = name
					};

				auctioneer.Send(message);
			}
		}
	}
}