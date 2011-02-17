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
namespace Stact.Remote.ReliableMulticast
{
	using System;
	using System.Net.Sockets;


	public class ReliableMulticastSocket :
		Socket
	{
		const SocketOptionLevel PgmLevel = (SocketOptionLevel)PgmProtocol;
		const int PgmProtocol = 113;
		const ProtocolType PgmProtocolType = (ProtocolType)PgmProtocol;


		public ReliableMulticastSocket()
			: base(AddressFamily.InterNetwork, SocketType.Rdm, PgmProtocolType)
		{
		}

		public bool EnableHighSpeed()
		{
			return EnableHighSpeed(this);
		}

		public bool SetPgmOption(int option, byte[] value)
		{
			return SetPgmOption(this, option, value);
		}

		public bool SetPgmOption(int option, uint value)
		{
			return SetPgmOption(this, option, value);
		}

		public static bool SetPgmOption(Socket socket, int option, byte[] value)
		{
			try
			{
				socket.SetSocketOption(PgmLevel, (SocketOptionName)option, value);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public static bool SetPgmOption(Socket socket, int option, uint value)
		{
			return SetPgmOption(socket, option, BitConverter.GetBytes(value));
		}

		public static bool EnableHighSpeed(Socket socket)
		{
			return SetPgmOption(socket, ReliableMulticastSocketOptions.HighSpeedIntranetOpt, 1);
		}
	}
}