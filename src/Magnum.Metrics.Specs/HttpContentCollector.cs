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
namespace Magnum.Metrics.Specs
{
	using System;
	using System.IO;
	using System.Net;

	public class HttpContentCollector : 
		IContentCollector
	{
		private readonly Uri _uri;

		public HttpContentCollector(string url)
			: this(new Uri(url))
		{
		}

		public HttpContentCollector(Uri uri)
		{
			_uri = uri;
		}

		private V GetRequestHeader<V>(Func<HttpWebResponse, V> func)
		{
			HttpWebRequest request = (HttpWebRequest) WebRequest.Create(_uri);
			request.Method = "HEAD";

			using (HttpWebResponse response = (HttpWebResponse) request.GetResponse())
			{
				return func(response);
			}
		}

		public DateTime GetLastModified()
		{
			return GetRequestHeader(r => r.LastModified);
		}

		public long GetContentLength()
		{
			return GetRequestHeader(r => r.ContentLength);
		}

		public ArraySegment<byte> GetContentSegment(int offset, int length)
		{
			HttpWebRequest request = (HttpWebRequest) WebRequest.Create(_uri);
			request.Method = "GET";
			request.AddRange(offset, offset + length - 1);

			using (HttpWebResponse response = (HttpWebResponse) request.GetResponse())
			{
				int contentLength = (int) response.ContentLength;

				return GetResponse(response, contentLength);
			}
		}

		private static ArraySegment<byte> GetResponse(WebResponse response, int length)
		{
			using (Stream s = response.GetResponseStream())
			{
				s.ReadTimeout = 1000;

				byte[] buffer = new byte[length];

				int readOffset = 0;
				int remaining = length;

				while( readOffset < length)
				{
					int read = s.Read(buffer, readOffset, remaining);
					if (read == 0)
						break;

					readOffset += read;
					remaining = length - readOffset;
				}

				return new ArraySegment<byte>(buffer, 0, readOffset);
			}
		}
	}
}