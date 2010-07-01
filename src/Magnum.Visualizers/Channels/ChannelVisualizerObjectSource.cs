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
namespace Magnum.Visualizers.Channels
{
	using System.IO;
	using Magnum.Channels;
	using Magnum.Channels.Visitors;
	using Microsoft.VisualStudio.DebuggerVisualizers;
	using Reflection;


	/// <summary>
	/// provides the channel data necessary to visualize the channel network outside
	/// of the process
	/// </summary>
	public class ChannelVisualizerObjectSource :
		VisualizerObjectSource
	{
		public override void GetData(object target, Stream outgoingData)
		{
			if (target == null)
				return;

			if (!typeof(Channel).IsAssignableFrom(target.GetType()))
				return;

			var visitor = new GraphChannelVisitor();

			visitor.FastInvoke("Visit", target);

			ChannelGraphData data = visitor.GetGraphData();

			base.GetData(data, outgoingData);
		}
	}
}