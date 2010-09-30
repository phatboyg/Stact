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
namespace Stact.Servers
{
	using System;
	using System.Collections.Generic;
	using System.Text.RegularExpressions;
	using Channels;


	/// <summary>
	/// A connection handler returns an appropriate channel to handle the connection
	/// based on a URI mapping and a set of accepted verbs
	/// </summary>
	public abstract class PatternMatchConnectionHandler :
		ConnectionHandler
	{
		readonly Regex _uriPatternMatcher;
		readonly HashSet<string> _verbsSupported;

		protected PatternMatchConnectionHandler(string uriPattern, params string[] verbsSupported)
		{
			_verbsSupported = new HashSet<string>(verbsSupported, StringComparer.InvariantCultureIgnoreCase);
			_uriPatternMatcher = new Regex(uriPattern, RegexOptions.Singleline | RegexOptions.Compiled);
		}

		public Channel<ConnectionContext> GetChannel(ConnectionContext context)
		{
			string requestUri = context.GetRequestUri();

			Match match = _uriPatternMatcher.Match(requestUri);

			if (false == match.Success ||
			    false == _verbsSupported.Contains(context.Request.HttpMethod))
				return null;

			return CreateChannel(context);
		}

		protected abstract Channel<ConnectionContext> CreateChannel(ConnectionContext context);
	}
}