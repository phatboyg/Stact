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
namespace Stact.ServerFramework
{
	using System;
	using System.IO;
	using System.Security.Principal;


	/// <summary>
	/// Abstracts a connection from the underlying transport implementation
	/// </summary>
	public interface ConnectionContext
	{
		/// <summary>
		/// The request submitted for the connection
		/// </summary>
		RequestContext Request { get; }

		/// <summary>
		/// The response for the connection
		/// </summary>
		ResponseContext Response { get; }

		/// <summary>
		/// The user context under which the connection was established
		/// </summary>
		IPrincipal User { get; }

		/// <summary>
		/// The server context
		/// </summary>
		ServerContext Server { get; }

		/// <summary>
		/// Completes the response for this connection
		/// </summary>
		void Complete();

		/// <summary>
		/// Wraps the response stream (for compression, encryption, etc.)
		/// </summary>
		/// <param name="responseFilter">A method to apply a filter to the connection</param>
		void SetResponseFilter(Func<Stream, Stream> responseFilter);
	}
}