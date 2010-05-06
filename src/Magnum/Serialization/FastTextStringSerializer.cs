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
namespace Magnum.Serialization
{
	using Extensions;

	[NotAutomaticallyLoaded]
	public class FastTextStringSerializer :
		TypeSerializer<string>
	{
		public TypeReader<string> GetReader()
		{
			return value =>
				{
					if (value.IsEmpty())
						return value;

					if (value[0] != FastTextSerializer.QuoteChar)
						return value;

					return value.Substring(1, value.Length - 2)
						.Replace(FastTextSerializer.DoubleQuoteString, FastTextSerializer.QuoteString);
				};
		}

		public TypeWriter<string> GetWriter()
		{
			return (value, output) =>
				{
					if (value.IsEmpty())
						output("");
					else if (value.IndexOfAny(FastTextSerializer.EscapeChars) == -1)
						output(value);
					else
					{
						output(string.Concat(FastTextSerializer.QuoteString,
							value.Replace(FastTextSerializer.QuoteString, FastTextSerializer.DoubleQuoteString),
							FastTextSerializer.QuoteString));
					}
				};
		}
	}
}