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
namespace Magnum.Serialization.TypeSerializers
{
	using Extensions;

	public class StringSerializer :
		TypeSerializer<string>
	{
		public TypeReader<string> GetReader()
		{
			return value => value;
		}

		public TypeWriter<string> GetWriter()
		{
			return (value, output) =>
				{
					if (value.IsEmpty())
						return;

					output(value);
				};
		}
	}
}

//	public static string ToCsvField(this string text)
//		{
//			return string.IsNullOrEmpty(text) || text.IndexOfAny(TypeSerializer.EscapeChars) == -1
//		       	? text
//		       	: string.Concat
//		       	  	(
//		       	  		TypeSerializer.QuoteString,
//		       	  		text.Replace(TypeSerializer.QuoteString, TypeSerializer.DoubleQuoteString),
//		       	  		TypeSerializer.QuoteString
//		       	  	);
//		}
//
//		public static string FromCsvField(this string text)
//		{
//			const int startingQuotePos = 1;
//			const int endingQuotePos = 2;
//			return string.IsNullOrEmpty(text) || text[0] != TypeSerializer.QuoteChar
//			       	? text
//					: text.Substring(startingQuotePos, text.Length - endingQuotePos)
//			       	  	.Replace(TypeSerializer.DoubleQuoteString, TypeSerializer.QuoteString);
//		}