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
namespace Stact.ServerFramework.Internal
{
	using System;
	using System.Net.Sockets;


	public class PgmSocket :
		Socket
	{
		const SocketOptionLevel PgmLevel = (SocketOptionLevel)PgmProtocol;
		const int PgmProtocol = 113;
		const ProtocolType PgmProtocolType = (ProtocolType)PgmProtocol;


		public PgmSocket()
			: base(AddressFamily.InterNetwork, SocketType.Rdm, PgmProtocolType)
		{
		}

		public bool SetPgmOption(int option, byte[] value)
		{
			try
			{
				SetSocketOption(PgmLevel, (SocketOptionName)option, value);
				return true;
			}
			catch (Exception ex)
			{
				return false;
			}
		}

		public bool SetPgmOption(int option, uint value)
		{
			return SetPgmOption(option, BitConverter.GetBytes(value));
		}

		public bool EnableHighSpeed()
		{
			return SetPgmOption(PgmSocketOptions.HighSpeedIntranetOpt, 1);
		}
	}
}