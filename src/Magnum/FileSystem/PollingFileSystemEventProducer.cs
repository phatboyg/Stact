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
namespace Magnum.FileSystem
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;
    using Channels;
    using Extensions;
    using Fibers;
    using Internal;


    public class PollingFileSystemEventProducer :
        IDisposable
    {
        readonly UntypedChannel _channel;
        readonly string _directory;
        ScheduledAction _scheduledAction;
        Fiber _fiber;
        Scheduler _scheduler;
        bool _disposed;
        Dictionary<string, Guid> _hashes;
        TimeSpan _checkInterval;

        /// <summary>
        /// Creates a PollingFileSystemEventProducer
        /// </summary>		
        /// <param name="directory">The directory to watch</param>
        /// <param name="channel">The channel where events should be sent</param>
        /// <param name="scheduler">Event scheduler</param>
        /// <param name="fiber">Fiber to schedule on</param>
        public PollingFileSystemEventProducer(string directory, UntypedChannel channel, Scheduler scheduler, Fiber fiber) :
            this(directory, channel, scheduler, fiber, 1.Minutes())
        {
            
        }

        public PollingFileSystemEventProducer(string directory, UntypedChannel channel, Scheduler scheduler, Fiber fiber, TimeSpan checkInterval)
        {
            _directory = directory;
            _channel = channel;
            _fiber = fiber;
            _hashes = new Dictionary<string, Guid>();
            _scheduler = scheduler;
            _checkInterval = checkInterval;

            _scheduledAction = scheduler.Schedule(3.Seconds(), _fiber, () => HashFileSystem());
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        void HashFileSystem()
        {
            try
            {
                Dictionary<string, Guid> newHashes = new Dictionary<string, Guid>();

                ProcessDirectory(newHashes, _directory);

                _hashes.ToList().ForEach(hashKvp =>
                    {
                        if (!newHashes.ContainsKey(hashKvp.Key))
                        {
                            _hashes.Remove(hashKvp.Key);
                            _channel.Send(new FileSystemDeletedImpl(Path.GetFileName(hashKvp.Key),
                                                                    Path.GetFullPath(hashKvp.Key)));
                        }
                        else
                        {
                            if (hashKvp.Value == newHashes[hashKvp.Key])
                            {
                                newHashes.Remove(hashKvp.Key);
                            }
                            else
                            {
                                _hashes[hashKvp.Key] = newHashes[hashKvp.Key];
                                _channel.Send(new FileChangedImpl(Path.GetFileName(hashKvp.Key),
                                                                  Path.GetFullPath(hashKvp.Key)));
                            }

                            newHashes.Remove(hashKvp.Key);
                        }
                    });

                foreach (var newHashKvp in newHashes)
                {
                    _hashes.Add(newHashKvp.Key, newHashKvp.Value);
                    _channel.Send(new FileCreatedImpl(Path.GetFileName(newHashKvp.Key),
                                                      Path.GetFullPath(newHashKvp.Key)));
                }
            }
            finally
            {
                _scheduledAction = _scheduler.Schedule(_checkInterval, _fiber, () => HashFileSystem());
            }
        }

        void ProcessDirectory(Dictionary<string, Guid> hashes, string baseDirectory)
        {
            string[] files = Directory.GetFiles(baseDirectory);

            foreach (string file in files)
            {
                try
                {
                    string hashValue;
                    using (FileStream f = File.OpenRead(file))
                    using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
                    {
                        byte[] fileHash = md5.ComputeHash(f);

                        hashValue = BitConverter.ToString(fileHash).Replace("-", "");
                    }

                    hashes.Add(Path.Combine(baseDirectory, file), new Guid(hashValue));
                }
                catch (Exception)
                {
                    // chew up exception and say empty hash
                    // can we do something more interesting than this?
                    hashes.Add(Path.Combine(baseDirectory, file), Guid.Empty);
                }

            }

            Directory.GetDirectories(baseDirectory).ToList().Each(dir => ProcessDirectory(hashes, dir));
        }

        void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            if (disposing)
                _scheduledAction.Cancel();

            _disposed = true;
        }
    }
}