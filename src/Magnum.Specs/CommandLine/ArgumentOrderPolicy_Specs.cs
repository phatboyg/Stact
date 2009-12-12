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
namespace Magnum.Specs.CommandLine
{
	using Magnum.CommandLine;
	using NUnit.Framework;

	public class ArgumentOrderPolicy_Specs
	{
		private string[] _bad_named_before_positional = new[] {"-l:local", "magnum"};
		private string[] _good_just_positional = new[] {"magnum"};
		private string[] _good_positional_then_named = new[] {"magnum", "-l:local"};

		[Test]
		public void You_can_have_just_one()
		{
			new Arguments_must_be_positional_then_named().Verify(_good_just_positional);
		}

		[Test]
		public void Positional_then_named()
		{
			new Arguments_must_be_positional_then_named().Verify(_good_positional_then_named);
		}

		[Test]
		[ExpectedException(typeof (ArgumentOrderException))]
		public void You_cant_have_named_before_positional()
		{
			new Arguments_must_be_positional_then_named().Verify(_bad_named_before_positional);
		}
	}
}