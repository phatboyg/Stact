// Copyright 2010-2013 Chris Patterson
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
namespace Stact.Configuration.Internal
{
    using System.Collections.Generic;
    using Builders;


    public class TypedConnectionConfigurator<T> :
        ConnectionConfigurator<T>
    {
        readonly Channel<T> _channel;
        readonly List<ConnectionBuilderConfigurator<T>> _configurators;

        public TypedConnectionConfigurator(Channel<T> channel)
        {
            _channel = channel;
            _configurators = new List<ConnectionBuilderConfigurator<T>>();
        }


        public void AddConfigurator(ConnectionBuilderConfigurator<T> configurator)
        {
            _configurators.Add(configurator);
        }

        public ChannelConnection Complete()
        {
            foreach (var configurator in _configurators)
                configurator.ValidateConfiguration();

            var connection = new TypedChannelConnectionBuilder<T>(_channel);

            foreach (var configurator in _configurators)
                configurator.Configure(connection);

            return connection.ChannelConnection;
        }
    }
}