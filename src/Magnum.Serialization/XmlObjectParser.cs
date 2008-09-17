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
	using System.Collections;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.IO;
	using System.Reflection;
	using System.Runtime.Serialization;
	using System.Xml;
	using Common.Reflection;

	public class XmlObjectParser :
		IObjectParser
	{
		private Queue<object> _objects = new Queue<object>();
		private XmlReader _reader;
		private ParseState _state = ParseState.Idle;
		private Stack<object> _working = new Stack<object>();

		public XmlObjectParser(Stream stream)
		{
			XmlReaderSettings settings = new XmlReaderSettings
				{
					CloseInput = false,
					IgnoreComments = true,
					IgnoreWhitespace = true,
				};

			_reader = XmlReader.Create(stream, settings);
		}


		private bool AtEndOfObject
		{
			get { return _reader.NodeType == XmlNodeType.EndElement && _reader.Name == "object"; }
		}

		private bool InObject
		{
			get { return _state == ParseState.InObject; }
		}

		private bool AtStartOfProperty
		{
			get { return _reader.IsStartElement(); }
		}

		private bool AtStartOfObject
		{
			get { return _reader.IsStartElement() && _reader.Name == "object" && _reader.HasAttributes; }
		}

		private bool Idle
		{
			get { return _state == ParseState.Idle; }
		}

		public void Dispose()
		{
			_reader.Close();
		}

		public IEnumerator<object> GetEnumerator()
		{
			while (ReadObjectFromStream())
			{
				yield return _objects.Dequeue();
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		private bool ReadObjectFromStream()
		{
			while (_reader.Read())
			{
				if (Idle)
				{
					if (AtStartOfObject)
						StartObject();
				}
				else if (InObject)
				{
					if (AtStartOfProperty)
						SetProperty();
					else if (AtEndOfObject)
					{
						EndObject();
					}
				}
			}

			return _objects.Count > 0;
		}

		private void EndObject()
		{
			_objects.Enqueue(_working.Pop());

			_state = ParseState.Idle;
		}

		private void SetProperty()
		{
			string propertyName = _reader.Name;

			_reader.MoveToContent();
			_reader.Read();

			Type t = _working.Peek().GetType();

			PropertyInfo pi = t.GetProperty(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			if (pi != null)
			{
				object valueObj = TypeDescriptor.GetConverter(pi.PropertyType).ConvertFromInvariantString(_reader.Value);

				new FastProperty(pi).Set(_working.Peek(), valueObj);
			}
		}

		private void StartObject()
		{
			_reader.MoveToFirstAttribute();
			if (_reader.Name == "type")
			{
				string objectType = _reader.Value;

				Type t = Type.GetType(objectType);
				if (t == null)
				{
					foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
					{
						t = assembly.GetType(objectType);
						if (t != null)
							break;
					}
				}

				if (t == null)
					throw new SerializationException("Unable to find a valid type for the object " + objectType);

				object o = FormatterServices.GetSafeUninitializedObject(t);

				_working.Push(o);

				_reader.MoveToContent();

				_state = ParseState.InObject;
			}
		}

		private enum ParseState
		{
			Idle,
			InObject
		}
	}
}