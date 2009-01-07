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
namespace Magnum.Common.Specs.StateMachine
{
	using System;
	using System.IO;
	using System.Runtime.Serialization;
	using Common.StateMachine;

	[Serializable]
	public class FileImportWorkflow :
		StateMachine<FileImportWorkflow>
	{
		static FileImportWorkflow()
		{
			Define(() =>
				{
					Initially(
						When(FileAvailable)
							.And(details => Path.GetExtension(details.Path).ToLowerInvariant() == ".txt")
							.Then((workflow, details) =>
								{
								})
							.TransitionTo(ProcessingTextFile),
						When(FileAvailable)
							.And(details => Path.GetExtension(details.Path).ToLowerInvariant() == ".dat")
							.Then((workflow, details) =>
								{
								}));


					During(ProcessingTextFile,
						When(FileProcessed)
							.Then((workflow) =>
								{
								}));
				});
		}

		protected FileImportWorkflow()
		{
		}

		public FileImportWorkflow(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public static State Initial { get; set; }
		public static State WaitingForExclusiveLock { get; set; }
		public static State ProcessingTextFile { get; set; }
		public static State ProcessingDataFile { get; set; }
		public static State WritingFileAcknowledgement { get; set; }
		public static State Completed { get; set; }

		public static Event<FileAvailableDetails> FileAvailable { get; set; }
		public static Event TimeoutExpired { get; set; }
		public static Event FileProcessed { get; set; }
		public static Event OutOfDiskSpace { get; set; }
	}


	public class FileAvailableDetails
	{
		public string Path { get; set; }
		public long Length { get; set; }
	}
}