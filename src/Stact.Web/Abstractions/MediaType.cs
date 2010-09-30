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
	using System.Net.Mime;

	public class MediaType
	{
		public static readonly MediaType Html = new MediaType(MediaTypeNames.Text.Html);
		public static readonly MediaType Json = new MediaType("application/json");
		public static readonly MediaType Text = new MediaType(MediaTypeNames.Text.Plain);
		private readonly string _mimeType;

		private MediaType(string typeName)
		{
			_mimeType = typeName;
		}

		public override string ToString()
		{
			return _mimeType;
		}
	}
}