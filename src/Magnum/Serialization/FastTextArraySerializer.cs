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
	using System.Text;
	using TypeSerializers;

	public class FastTextArraySerializer<T, TElement> :
		ArraySerializer<T, TElement>
	{
		public FastTextArraySerializer(TypeSerializer<TElement> elementTypeSerializer)
			: base(elementTypeSerializer)
		{
		}

		public override TypeReader<T> GetReader()
		{
			return value => { return default(T); };
		}

		public override TypeWriter<T> GetWriter()
		{
			var baseWriter = base.GetWriter();

			return (value, output) =>
				{
					var sb = new StringBuilder(2048);

					sb.Append(FastTextSerializer.ListStartChar);

					bool addSeparator = false;

					baseWriter(value, text =>
						{
							if (addSeparator)
								sb.Append(FastTextSerializer.ItemSeparatorString);
							else
								addSeparator = true;

							sb.Append(text);
						});

					sb.Append(FastTextSerializer.ListEndChar);

					output(sb.ToString());
				};
		}
	}
}