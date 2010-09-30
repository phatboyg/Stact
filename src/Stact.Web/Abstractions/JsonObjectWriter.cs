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
namespace Stact.Web.Abstractions
{
	using System.Web;
	using System.Web.Script.Serialization;
	using Stact.ValueProviders;


	/// <summary>
	/// Handles the serialization of an object to Json using the built-in .NET serializer
	/// </summary>
	public class JsonObjectWriter :
		ObjectWriter
	{
		private readonly ValueProvider _valueProvider;
		private readonly ContentWriter _writer;
		private JavaScriptSerializer _serializer;

		public JsonObjectWriter(ContentWriter writer, ValueProvider valueProvider)
		{
			_writer = writer;
			_valueProvider = valueProvider;
		}

		private JavaScriptSerializer Serializer
		{
			get
			{
				if (_serializer == null)
					_serializer = new JavaScriptSerializer();
				return _serializer;
			}
		}

		/// <summary>
		/// Writes the object as a JSON response if the request is an Ajax request, otherwise the
		/// response is wrapped in an HTML text area for displaying to the user.
		/// </summary>
		/// <param name="obj"></param>
		public void Write(object obj)
		{
			string json = Serializer.Serialize(obj);

			if (_valueProvider.IsAjaxRequest())
			{
				_writer.Write(MediaType.Json.ToString(), json);
			}
			else
			{
				// For proper jquery.form plugin support of file uploads
				// See the discussion on the File Uploads sample at http://malsup.com/jquery/form/#code-samples

				string html = "<html><body><textarea rows=\"10\" cols=\"80\">" + HttpUtility.HtmlEncode(json) + "</textarea></body></html>";
				_writer.Write(MediaType.Html.ToString(), html);
			}
		}
	}
}