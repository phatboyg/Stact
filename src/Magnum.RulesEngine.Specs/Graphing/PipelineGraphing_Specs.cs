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
	using NUnit.Framework;
	using Pipeline;
	using Pipeline.Segments;
	using Visualizers;

	[TestFixture]
	public class Generating_a_graph_from_a_pipeline
	{
		private class SomethingHappenedEvent
		{
		}

		[Test]
		public void Should_contain_all_nodes()

		{
			MessageConsumerSegment consumer = PipeSegment.Consumer<SomethingHappenedEvent>(x => { });
			EndSegment end = PipeSegment.End<SomethingHappenedEvent>();
			RecipientListSegment recipientList = PipeSegment.RecipientList<SomethingHappenedEvent>(new Pipe[] {consumer, end});
			FilterSegment filter = PipeSegment.Filter<object>(recipientList);
			Pipe input = PipeSegment.Input(filter);

            var generator = new PipelineGraphGenerator();

			string filename = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "graph.png");

			generator.SaveGraphToFile(input, 2560, 1920, filename);

			//PipelineDebugVisualizer.TestShowVisualizer(input);

		}
	}
}