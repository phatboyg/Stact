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
	public static class PgmSocketOptions
	{
		const int OptionsBase = 1000;

		public static readonly int AddReceiveIf = (OptionsBase + 8);
		public static readonly int DelReceiveIf = (OptionsBase + 9);
		public static readonly int FlushCache = (OptionsBase + 3);
		public static readonly int HighSpeedIntranetOpt = (OptionsBase + 14);
		public static readonly int LateJoin = (OptionsBase + 6);
		public static readonly int RateWindowSize = (OptionsBase + 1);
		public static readonly int ReceiverStatistics = (OptionsBase + 13);
		public static readonly int SendWindowAdvRate = (OptionsBase + 10);
		public static readonly int SenderStatistics = (OptionsBase + 5);
		public static readonly int SenderWindowAdvanceMethod = (OptionsBase + 4);
		public static readonly int SetMcastTtl = (OptionsBase + 12);
		public static readonly int SetMessageBoundary = (OptionsBase + 2);
		public static readonly int SetSendIf = (OptionsBase + 7);
		public static readonly int UseFec = (OptionsBase + 11);
	}
}