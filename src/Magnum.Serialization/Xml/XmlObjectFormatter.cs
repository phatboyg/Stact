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
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Text;
	using System.Xml;
	using Common.CollectionExtensions;

	public class XmlObjectFormatter :
		IObjectFormatter
	{
		private readonly Stream _stream;
		private readonly XmlWriter _writer;
		
		private static readonly Dictionary<Type, Func<object, string>> _converters = new Dictionary<Type, Func<object, string>>();

		static XmlObjectFormatter()
		{
			_converters.Add(typeof (object), (x) => x.ToString());
			_converters.Add(typeof (int), (x) => x.ToString());
			_converters.Add(typeof (DateTime), (x) => ((DateTime)x).ToString("u"));
		}


		public XmlObjectFormatter(Stream stream)
		{
			_stream = stream;

			XmlWriterSettings settings = new XmlWriterSettings
				{
					CloseOutput = false,
					Encoding = new UTF8Encoding(false),
					NewLineHandling = NewLineHandling.None,
					NewLineOnAttributes = false,
				};

			_writer = XmlWriter.Create(_stream, settings);
		}

		public void Start()
		{
			_writer.WriteStartDocument();
		}

		public void Stop()
		{
			_writer.WriteEndDocument();
		}

		public void StartObject(Type type)
		{
			_writer.WriteStartElement("object");
			_writer.WriteAttributeString("type", type.FullName);
		}

		public void EndObject(Type type)
		{
			_writer.WriteEndElement();
		}

		public void Write(IPropertyData data)
		{
			Func<object, string> converter;
			if (_converters.TryGetValue(data.Value.GetType(), out converter) == false)
				converter = _converters[typeof (object)];

			_writer.WriteElementString(data.Name, converter(data.Value));
		}

		public void Dispose()
		{
			_writer.Flush();
			_writer.Close();

			_stream.Flush();
		}
	}
}