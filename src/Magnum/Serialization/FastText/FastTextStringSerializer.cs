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
namespace Magnum.Serialization.FastText
{
	using Extensions;

	public class FastTextStringSerializer :
		FastTextParser,
		TypeSerializer<string>
	{
		public TypeReader<string> GetReader()
		{
			return value =>
				{
					if (value.IsEmpty())
						return value;

					if (value[0] != Quote)
						return value;

					return value.Substring(1, value.Length - 2)
						.Replace(DoubleQuoteString, QuoteString);
				};
		}

		public TypeWriter<string> GetWriter()
		{
			return (value, output) =>
				{
					if(value == null)
						return;

					if (value.IsEmpty())
						return;
						
					if (value.IndexOfAny(EscapeChars) == -1)
						output(value);
					else
					{
						output(string.Concat(QuoteString,
						                     value.Replace(QuoteString, DoubleQuoteString),
						                     QuoteString));
					}
				};
		}
	}
}