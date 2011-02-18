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
namespace Stact.Specs.Registries
{
	using System.Diagnostics;
	using Internal;
	using Magnum;
	using Magnum.Extensions;
	using Magnum.Serialization;
	using Magnum.TestFramework;
	using Remote;


	[Scenario]
	public class When_sending_a_message_through_the_chunk_writer
	{
		HeaderChannelAdapter _channel;
		DelegateChunkWriter _chunkWriter;
		ChunkWriterChannel _rawChannel;

		[Given]
		public void A_channel()
		{
			_chunkWriter = new DelegateChunkWriter(chunk =>
				{
					string output = chunk.ToMemoryViewString();

					Trace.WriteLine(output);
				});

			_rawChannel = new ChunkWriterChannel(_chunkWriter, new FastTextSerializer());

			var chunkHeaderChannel = new ChunkHeaderChannel(_rawChannel);

			_channel = new HeaderChannelAdapter(chunkHeaderChannel);
		}

		[Then]
		public void Should_properly_serialize_a_plain_class()
		{
			_rawChannel.Send(new Test
				{
					Name = "Johnson",
				});
		}

		[Then]
		public void Should_properly_serialize_a_message()
		{
			_channel.Send<Message<Test>>(new MessageImpl<Test>(new Test
				{
					Name = "Johnson"
				}));
		}

		[Then]
		public void Should_properly_serialize_a_request()
		{
			AnonymousActor.New(inbox =>
				{
					_channel.Request(new Test
						{
							Name = "Magic"
						}, inbox);
				});

			ThreadUtil.Sleep(1.Seconds());
		}


		public class Test
		{
			public string Name { get; set; }
		}
	}
}