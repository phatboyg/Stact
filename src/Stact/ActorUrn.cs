// Copyright 2010 Chris Patterson
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
namespace Stact
{
	using System;
	using System.Runtime.Serialization;


	[Serializable]
	public class ActorUrn :
		Uri
	{
		public ActorUrn(string uriString)
			: base(uriString)
		{
		}

		public ActorUrn(Uri baseUri, string relativeUri = "")
			: base(baseUri, relativeUri)
		{
		}

		protected ActorUrn(SerializationInfo serializationInfo, StreamingContext streamingContext)
			: base(serializationInfo, streamingContext)
		{
		}

		public Guid GetId()
		{
			if (Segments.Length == 0)
				return Guid.Empty;

			string[] names = Segments[0].Split(':');
			if (names[0] != "actor")
				return Guid.Empty;

			if (names.Length == 3)
			{
				if(names[1] == "uuid")
				{
					return new Guid(names[2]);
				}
			}

			return Guid.Empty;
		}
	}
}