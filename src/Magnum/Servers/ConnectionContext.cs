// Copyright 2007-2010 The Apache Software Foundation.
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
namespace Magnum.Servers
{
	using System;
	using System.IO;
	using System.Security.Principal;


	public interface ConnectionContext
	{
		RequestContext Request { get; }
		ResponseContext Response { get; }
		IPrincipal User { get; }
		void FinalizeResonse();
		void SetResponseFilter(Func<Stream, Stream> responseFilter);
	}
}