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
namespace Magnum.Specs.FileSystem
{
    using System;
    using System.IO;
    using Magnum.Channels;
    using Magnum.Extensions;
    using Magnum.FileSystem;
    using Magnum.FileSystem.Events;
    using TestFramework;


    [Scenario]
    public class Creating_a_file_in_a_folder
    {
        string _baseDirectory;
        ChannelAdapter _channel;
        string _filename;
        Future<FileCreated> _listener;
        string _path;
        FileSystemEventProducer _producer;

        [When]
        public void A_file_is_created()
        {
            _baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            _filename = "test.dat";
            _path = Path.Combine(_baseDirectory, _filename);

            File.Delete(_path);

            _listener = new Future<FileCreated>();

            _channel = new ChannelAdapter();
            _producer = new FileSystemEventProducer(_baseDirectory, _channel);

			using (var subscription = _channel.Connect(x => x.AddConsumerOf<FileCreated>().UsingConsumer(m => _listener.Complete(m))))
            {
                File.Create(_path);

                _listener.WaitUntilCompleted(10.Seconds());
            }

            _producer.Dispose();
        }

        [Then]
        public void Should_produce_a_file_created_message()
        {
            _listener.IsCompleted.ShouldBeTrue();
        }

        [Then]
        public void Should_match_the_full_path_of_the_file()
        {
            _listener.Value.Path.ShouldEqual(_path);
        }

        [Then]
        public void Should_match_the_name_of_the_file()
        {
            _listener.Value.Name.ShouldEqual(_filename);
        }
    }
}