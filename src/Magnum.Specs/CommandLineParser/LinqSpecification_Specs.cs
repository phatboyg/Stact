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
namespace LinqSpecification_Specs
{
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using Magnum.CommandLineParser;
	using Magnum.Monads.Parser;
	using Magnum.Specs.CommandLineParser;
	using Magnum.TestFramework;
	using NUnit.Framework;

	[TestFixture]
	public class LinqSpecification_Specs
	{
		[SetUp]
		public void Setup()
		{
			_commandLineParser = new MonadicCommandLineParser();
			_commandParser = new RemoteCommandLineParser();
		}

		private MonadicCommandLineParser _commandLineParser;
		private RemoteCommandLineParser _commandParser;

		[Test]
		public void Should_be_a_remove_remote_repository_command()
		{
			string commandLine = "remote rm dru git://github.com/dru/nu.git";

			ICommand command = _commandParser.Parse(_commandLineParser.Parse(commandLine)).First();

			command.ShouldBeAnInstanceOf<RemoveRemoteRepositoryCommand>();
		}

		[Test]
		public void Should_be_an_add_remote_repository_command()
		{
			string commandLine = "remote add dru git://github.com/dru/nu.git";

			ICommand command = _commandParser.Parse(_commandLineParser.Parse(commandLine)).First();

			command.ShouldBeAnInstanceOf<AddRemoteRepositoryCommand>();
		}
	}

	public class RemoteCommandLineParser :
		CommandLineElementParser<ICommand>
	{
		private readonly Parser<IEnumerable<ICommandLineElement>, ICommand> _all;

		public RemoteCommandLineParser()
		{
			RemoteAdd = from remote in Argument("remote")
			            from add in Argument("add")
			            from alias in Argument()
			            from url in Argument()
			            select (ICommand) new AddRemoteRepositoryCommand(alias.Id, url.Id);

			RemoteRemove = from remote in Argument("remote")
			               from add in Argument("rm")
			               from alias in Argument()
			               from url in Argument()
			               select (ICommand) new RemoveRemoteRepositoryCommand(alias.Id, url.Id);

			Element = (from element in RemoteAdd select element)
				.Or(from element in RemoteRemove select element)
				;

			_all = from t in Element select t;
		}

		public Parser<IEnumerable<ICommandLineElement>, ICommand> RemoteAdd { get; set; }
		public Parser<IEnumerable<ICommandLineElement>, ICommand> RemoteRemove { get; set; }
		public Parser<IEnumerable<ICommandLineElement>, ICommand> Element { get; set; }

		public override Parser<IEnumerable<ICommandLineElement>, ICommand> All
		{
			get { return _all; }
		}
	}

	public class AddRemoteRepositoryCommand :
		ICommand
	{
		public AddRemoteRepositoryCommand(string alias, string url)
		{
			Alias = alias;
			Url = url;
		}

		public string Alias { get; private set; }
		public string Url { get; private set; }

		public int Execute()
		{
			Trace.WriteLine("Adding remote repository (" + Alias + ") for: " + Url);

			return 0;
		}
	}

	public class RemoveRemoteRepositoryCommand :
		ICommand
	{
		public RemoveRemoteRepositoryCommand(string alias, string url)
		{
			Alias = alias;
			Url = url;
		}

		public string Alias { get; private set; }
		public string Url { get; private set; }

		public int Execute()
		{
			Trace.WriteLine("Removing remote repository (" + Alias + ") for: " + Url);

			return 0;
		}
	}
}