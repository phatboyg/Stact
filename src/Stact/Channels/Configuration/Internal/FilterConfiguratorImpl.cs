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
    using System.Linq;
    using Builders;
    using Configurators;


    public class FilterConfiguratorImpl<TChannel> :
        FilterConfigurator<TChannel>,
        ChannelBuilderConfigurator<TChannel>
    {
        readonly IList<ChannelBuilderConfigurator<TChannel>> _configurators;
        readonly Filter<TChannel> _filter;

        public FilterConfiguratorImpl(Filter<TChannel> filter)
        {
            _filter = filter;
            _configurators = new List<ChannelBuilderConfigurator<TChannel>>();
        }

        public IEnumerable<ValidateConfigurationResult> ValidateConfiguration()
        {
            if (_filter == null)
                yield return this.Failure("Filter", "must be specified");

            if (_configurators.Count == 0)
                yield return this.Failure("Channels", "must be configured");

            foreach (var result in _configurators.SelectMany(x => x.ValidateConfiguration()))
                yield return result;
        }

        public void Configure(ChannelBuilder<TChannel> builder)
        {
            var filterBuilder = new FilterChannelBuilder<TChannel>(builder, _filter);

            foreach (var configurator in _configurators)
                configurator.Configure(filterBuilder);
        }


        public void AddConfigurator(ChannelBuilderConfigurator<TChannel> configurator)
        {
            _configurators.Add(configurator);
        }
    }
}