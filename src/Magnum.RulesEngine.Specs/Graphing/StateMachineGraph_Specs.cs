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
namespace Magnum.RulesEngine.Specs.Graphing
{
	using System.IO;
	using System.Reflection;
	using Magnum.Specs.StateMachine;
	using NUnit.Framework;
	using StateMachine;
	using Visualizers.StateMachine;


	[TestFixture]
	public class StateMachineGraph_Specs
	{
		[Test]
		public void I_want_to_see_you_pretty()
		{
			var machine = new OrderStateMachine();

			var generator = new StateMachineGraphGenerator();

			string filename = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "graph.png");

			generator.SaveGraphToFile(machine.GetGraphData(), 2560, 1920, filename);
		}
	}
}