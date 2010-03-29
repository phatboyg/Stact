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
namespace Magnum.Web.Abstractions
{
	using System.Web;

	public class HttpResponseContentWriter :
		ContentWriter
	{
		private readonly HttpResponseBase _response;

		public HttpResponseContentWriter(HttpResponseBase response)
		{
			_response = response;
		}

		public void WriteFile(string contentType, string localFilePath, string displayName)
		{
			_response.ContentType = contentType;

			if (displayName != null)
			{
				_response.AppendHeader("content-disposition", "attachment; filename=" + displayName);
			}

			_response.WriteFile(localFilePath);
		}

		public void Write(string contentType, string renderedOutput)
		{
			_response.ContentType = contentType;
			_response.Write(renderedOutput);
		}

		public void RedirectToUrl(string url)
		{
			_response.Redirect(url, false);
		}
	}
}