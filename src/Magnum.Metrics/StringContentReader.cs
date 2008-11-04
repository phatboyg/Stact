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
namespace Magnum.Metrics
{
	using System.Collections;
	using System.Collections.Generic;
	using System.IO;

	public class StringContentReader :
		IContentReader
	{
		private readonly string _data;

		public StringContentReader(string data)
		{
			_data = data;
		}

		public IEnumerator<string> GetEnumerator()
		{
			using (TextReader reader = new StringReader(_data))
			{
				string line;
				while (string.IsNullOrEmpty(line = reader.ReadLine()) == false)
				{
					yield return line;
				}
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}