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
namespace Magnum.Specs.CommandLineParser.PlugInCommands
{
	using System.Collections.Generic;
	using System.Linq;
	using Magnum.CommandLineParser;
	using Magnum.Monads.Parser;
	using NUnit.Framework;
	using TestFramework;

	[TestFixture]
	public class When_parsing_a_command_line_into_commands
	{
		private static void InitializeCommandLinePatterns(ICommandLineElementParser<ICommand> x)
		{
			Parser<IEnumerable<ICommandLineElement>, ISwitchElement> switches =
				(from replace in x.Switch("replace") select replace);

			x.Add(from remote in x.Argument("remote")
			      from add in x.Argument("add")
				  from replace in switches.Optional("replace", false)
			      from alias in x.Argument()
			      from url in x.Argument()
			      select (ICommand) new AddRemoteRepositoryCommand(alias.Id, url.Id, replace.Value));

			x.Add(from remote in x.Argument("remote")
			      from add in x.Argument("rm")
			      from alias in x.Argument()
			      from url in x.Argument()
			      select (ICommand) new RemoveRemoteRepositoryCommand(alias.Id, url.Id));
		}

		private void InitializeCommandLineParser(ICommandLineElementParser<ICommand> x)
		{
			Parser<IEnumerable<ICommandLineElement>, IDefinitionElement> definitions =
				(from output in x.Definition("out") select output);

			Parser<IEnumerable<ICommandLineElement>, ISwitchElement> switches =
				(from verbose in x.Switch("verbose") select verbose)
					.Or(from quiet in x.Switch("quiet") select quiet);

			x.Add(from arg in x.Argument("version")
			      from verbose in switches.Optional("verbose", false)
			      from quiet in switches.Optional("quiet", false)
				  from output in definitions.Optional("out", "output.txt")
			      select (ICommand) new VersionCommand(verbose.Value, quiet.Value, output.Value));
		}

		[Test]
		public void Should_be_able_to_support_optional_parameters()
		{
			string commandLine = "version --verbose";

			var commands = CommandLine.Parse<ICommand>(commandLine, InitializeCommandLineParser).ToArray();

			ICommand command = commands.First();

			command.ShouldBeAnInstanceOf<VersionCommand>();

			var versionCommand = (VersionCommand) command;

			versionCommand.Verbose.ShouldBeTrue();
			versionCommand.Quiet.ShouldBeFalse();
			versionCommand.Output.ShouldEqual("output.txt");
		}

		[Test]
		public void Should_be_able_to_support_optional_parameters_when_missing()
		{
			string commandLine = "version";

			ICommand command = CommandLine.Parse<ICommand>(commandLine, InitializeCommandLineParser).First();

			command.ShouldBeAnInstanceOf<VersionCommand>();

			var versionCommand = (VersionCommand) command;

			versionCommand.Verbose.ShouldBeFalse();
			versionCommand.Quiet.ShouldBeFalse();
		}

		[Test]
		public void Should_support_an_output_file_spec()
		{
			string commandLine = "version -out \"filename.txt\"";

			ICommand command = CommandLine.Parse<ICommand>(commandLine, InitializeCommandLineParser).First();

			command.ShouldBeAnInstanceOf<VersionCommand>();

			var versionCommand = (VersionCommand) command;

			versionCommand.Output.ShouldEqual("filename.txt");
		}

		[Test]
		public void Should_be_able_to_support_in_order_arguments()
		{
			string commandLine = "version --verbose --quiet";

			ICommand command = CommandLine.Parse<ICommand>(commandLine, InitializeCommandLineParser).First();

			command.ShouldBeAnInstanceOf<VersionCommand>();

			var versionCommand = (VersionCommand) command;

			versionCommand.Verbose.ShouldBeTrue();
			versionCommand.Quiet.ShouldBeTrue();
		}

		[Test]
		public void Should_be_able_to_support_out_of_order_arguments()
		{
			string commandLine = "version --quiet --verbose";

			ICommand command = CommandLine.Parse<ICommand>(commandLine, InitializeCommandLineParser).First();

			command.ShouldBeAnInstanceOf<VersionCommand>();

			var versionCommand = (VersionCommand) command;

			versionCommand.Verbose.ShouldBeTrue();
			versionCommand.Quiet.ShouldBeTrue();
		}

		[Test]
		public void Should_be_a_remove_remote_repository_command()
		{
			string commandLine = "remote rm dru git://github.com/dru/nu.git";

			ICommand command = CommandLine.Parse<ICommand>(commandLine, InitializeCommandLinePatterns).First();

			command.ShouldBeAnInstanceOf<RemoveRemoteRepositoryCommand>();
		}

		[Test]
		public void Should_handle_paths_now()
		{
			string commandLine = "add .";

			ICommand command = CommandLine.Parse<ICommand>(commandLine, x =>
				{
					x.Add(from remote in x.Argument("add")
						  from add in x.ValidPath()
						  select (ICommand)new AddFileCommand(add.Id));

				}).First();

			command.ShouldBeAnInstanceOf<AddFileCommand>();
			command.Execute();
		}

		[Test]
		public void Should_handle_super_paths_now()
		{
			string commandLine = @"add c:\";

			ICommand command = CommandLine.Parse<ICommand>(commandLine, x =>
				{
					x.Add(from remote in x.Argument("add")
						  from add in x.ValidPath()
						  select (ICommand)new AddFileCommand(add.Id));

				}).First();

			command.ShouldBeAnInstanceOf<AddFileCommand>();
			command.Execute();
		}

		[Test]
		public void Should_be_an_add_remote_repository_command()
		{
			string commandLine = "remote add dru git://github.com/dru/nu.git";

			ICommand command = CommandLine.Parse<ICommand>(commandLine, InitializeCommandLinePatterns).First();

			command.ShouldBeAnInstanceOf<AddRemoteRepositoryCommand>();

			var typed = (AddRemoteRepositoryCommand) command;

			typed.Alias.ShouldEqual("dru");
			typed.Url.ShouldEqual("git://github.com/dru/nu.git");
			typed.ReplaceExisting.ShouldBeFalse();
		}

		[Test]
		public void Adding_the_optional_argument_should_not_break_the_parsing_logic()
		{
			string commandLine = "remote add --replace dru git://github.com/dru/nu.git";

			ICommand command = CommandLine.Parse<ICommand>(commandLine, InitializeCommandLinePatterns).First();

			command.ShouldBeAnInstanceOf<AddRemoteRepositoryCommand>();

			var typed = (AddRemoteRepositoryCommand) command;

			typed.Alias.ShouldEqual("dru");
			typed.Url.ShouldEqual("git://github.com/dru/nu.git");
			typed.ReplaceExisting.ShouldBeTrue();
		}
	}
}