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
namespace Stact.Specs.FileSystem
{
    using System;
    using System.IO;
    using System.Threading;
    using Fibers;
    using Stact.Channels;
    using Stact.Extensions;
    using Stact.FileSystem;
    using Stact.FileSystem.Events;
    using TestFramework;


    [Scenario]
    public class Renaming_a_folder_in_the_services_folder_for_poller
    {
        string _baseDirectory;
        ChannelAdapter _channel;
        Future<FileCreated> _createdListener;
        Future<FileSystemDeleted> _deletedListener;
        PollingFileSystemEventProducer _producer;
        Scheduler _scheduler;

        [When]
        public void A_file_is_created()
        {
            _baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            string dir1 = Path.Combine(_baseDirectory, "dir1");
            string dir2 = Path.Combine(_baseDirectory, "dir2");

            if (System.IO.Directory.Exists(dir1))
                System.IO.Directory.Delete(dir1, true);

            if (System.IO.Directory.Exists(dir2))
                System.IO.Directory.Delete(dir2, true);
            
            System.IO.Directory.CreateDirectory(dir1);

            _createdListener = new Future<FileCreated>();
            _deletedListener = new Future<FileSystemDeleted>();

            _channel = new ChannelAdapter();
            FiberFactory fiberFactory = () => new SynchronousFiber();
            _scheduler = new TimerScheduler(fiberFactory());
            _producer = new PollingFileSystemEventProducer(_baseDirectory, _channel, _scheduler, fiberFactory(),
                                                           20.Seconds());

            Thread.Sleep(5.Seconds());

            using (_channel.Connect(x => 
                {
                    x.AddConsumerOf<FileCreated>().UsingConsumer(m => _createdListener.Complete(m));
                    x.AddConsumerOf<FileSystemDeleted>().UsingConsumer(m => _deletedListener.Complete(m));
                }))
            {
                System.IO.Directory.Move(dir1, dir2);

                _createdListener.WaitUntilCompleted(10.Seconds());
                _deletedListener.WaitUntilCompleted(10.Seconds());
            }

            _producer.Dispose();
        }

        [Then]
        public void Should_produce_a_file_created_message()
        {
            _createdListener.IsCompleted.ShouldBeTrue();
        }

        [Then]
        public void Should_produce_a_file_deleted_message()
        {
            _deletedListener.IsCompleted.ShouldBeTrue();
        }
    }

    [Scenario]
    public class Creating_a_file_in_a_folder_for_poller
    {
        string _baseDirectory;
        ChannelAdapter _channel;
        string _filename;
        Future<FileCreated> _listener;
        string _path;
        PollingFileSystemEventProducer _producer;
        Scheduler _scheduler;

        [When]
        public void A_file_is_created()
        {
            _baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            _filename = "test2.dat";
            _path = Path.Combine(_baseDirectory, _filename);

            System.IO.File.Delete(_path);

            _listener = new Future<FileCreated>();

            _channel = new ChannelAdapter();
            FiberFactory fiberFactory = () => new SynchronousFiber();
            _scheduler = new TimerScheduler(fiberFactory());
            _producer = new PollingFileSystemEventProducer(_baseDirectory, _channel, _scheduler, fiberFactory(),
                                                           20.Seconds());

            Thread.Sleep(5.Seconds());

            using (_channel.Connect(x => x.AddConsumerOf<FileCreated>().UsingConsumer(m => _listener.Complete(m))))
            {
                System.IO.File.Create(_path);

                _listener.WaitUntilCompleted(25.Seconds());
            }

            _producer.Dispose();
        }

        [Then, Slow]
        public void Should_produce_a_file_created_message()
        {
            _listener.IsCompleted.ShouldBeTrue();
        }

        [Then, Slow]
        public void Should_match_the_full_path_of_the_file()
        {
            _listener.Value.Path.ShouldEqual(_path);
        }

        [Then, Slow]
        public void Should_match_the_name_of_the_file()
        {
            _listener.Value.Name.ShouldEqual(_filename);
        }
    }
}