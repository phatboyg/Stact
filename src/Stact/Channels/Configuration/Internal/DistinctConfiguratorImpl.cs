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


    public class DistinctConfiguratorImpl<TChannel, TKey> :
        DistinctConfigurator<TChannel, TKey>,
        ChannelBuilderConfigurator<ICollection<TChannel>>
    {
        readonly IList<ChannelBuilderConfigurator<IDictionary<TKey, TChannel>>> _configurators;
        readonly KeyAccessor<TChannel, TKey> _keyAccessor;

        public DistinctConfiguratorImpl(KeyAccessor<TChannel, TKey> keyAccessor)
        {
            _keyAccessor = keyAccessor;
            _configurators = new List<ChannelBuilderConfigurator<IDictionary<TKey, TChannel>>>();
        }

        public void Configure(ChannelBuilder<ICollection<TChannel>> builder)
        {
            var channelBuilder = new DistinctChannelBuilder<TChannel, TKey>(builder, _keyAccessor);

            foreach (var configurator in _configurators)
                configurator.Configure(channelBuilder);
        }

        public void ValidateConfiguration()
        {
            if (_configurators.Count == 0)
                throw new ChannelConfigurationException(typeof(TChannel), "No channels were configured");

            foreach (var configurator in _configurators)
                configurator.ValidateConfiguration();
        }

        public void AddConfigurator(ChannelBuilderConfigurator<IDictionary<TKey, TChannel>> configurator)
        {
            _configurators.Add(configurator);
        }
    }
}