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
namespace Stact.ServerFramework
{
	using System;
	using System.Reflection;
	using System.Web;
	using Magnum.Extensions;
	using Stact.Internal;


	public class VersionConnectionHandler :
		PatternMatchConnectionHandler
	{
		readonly VersionChannel _versionChannel;

		public VersionConnectionHandler()
			: base("^/version", "GET")
		{
			_versionChannel = new VersionChannel();
		}

		protected override Channel<ConnectionContext> CreateChannel(ConnectionContext context)
		{
			return _versionChannel;
		}


		class VersionChannel :
			Channel<ConnectionContext>
		{
			static readonly Assembly _assembly;
			static readonly string _copyright;
			static readonly string _message;
			static readonly AssemblyName _name;
			static readonly Version _version;
			static readonly string _versionTag;
			readonly Fiber _fiber;

			static VersionChannel()
			{
				_assembly = Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly() ?? Assembly.GetExecutingAssembly();
				_name = _assembly.GetName();
				_version = _name.Version;

				_versionTag = "{0} {1}".FormatWith(_name.Name, _version.ToString());

				_copyright = _assembly.GetAttribute<AssemblyCopyrightAttribute>().ValueOrDefault(x => x.Copyright, "");

				_message = @"<body><h1>{0}</h1><p>{1}</p></body>".FormatWith(HttpUtility.HtmlEncode(_versionTag),
				                                                             HttpUtility.HtmlEncode(_copyright));
			}

			public VersionChannel()
			{
				_fiber = new TaskFiber();
			}

			public void Send(ConnectionContext context)
			{
				_fiber.Execute(() =>
					{
						context.Response.WriteHtml(_message);
						context.Complete();
					});
			}
		}
	}
}