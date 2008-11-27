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
namespace Magnum.MapReduce.Specs
{
	using System;
	using System.Collections.Generic;
	using MbUnit.Framework;

	[TestFixture]
	public class FileSplit_Specs
	{
		[Test]
		public void A_filesplit_should_contain_a_valid_link_to_the_file()
		{
			FileSplit fileSplit = new FileSplit
				{
					Sources = new List<Uri>
						{
							new Uri("http://localhost/LogFiles"),
						},
					Filename = "ex081101.log",
					Start = 0,
					Length = 1024*1024,
				};
		}
	}
}