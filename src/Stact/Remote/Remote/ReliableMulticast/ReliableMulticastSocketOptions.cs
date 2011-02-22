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
	public static class ReliableMulticastSocketOptions
	{
		public const int AddReceiveIf = (OptionsBase + 8);
		public const int DelReceiveIf = (OptionsBase + 9);
		public const int FlushCache = (OptionsBase + 3);
		public const int HighSpeedIntranetOpt = (OptionsBase + 14);
		public const int LateJoin = (OptionsBase + 6);
		public const int OptionsBase = 1000;
		public const int RateWindowSize = (OptionsBase + 1);
		public const int ReceiverStatistics = (OptionsBase + 13);
		public const int SendWindowAdvRate = (OptionsBase + 10);
		public const int SenderStatistics = (OptionsBase + 5);
		public const int SenderWindowAdvanceMethod = (OptionsBase + 4);
		public const int SetMcastTtl = (OptionsBase + 12);
		public const int SetMessageBoundary = (OptionsBase + 2);
		public const int SetSendIf = (OptionsBase + 7);
		public const int UseFec = (OptionsBase + 11);
	}
}