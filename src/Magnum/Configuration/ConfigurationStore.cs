// Copyright 2007-2010 The Apache Software Foundation.
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
namespace Magnum.Configuration
{
    using System.Collections.Generic;
    using Extensions;

    public class ConfigurationStore
    {
        public ConfigurationStore()
        {
            ProvidersLoaded = new List<ConfigurationValueProvider>();
        }

        public void AddFile(string jsonFile)
        {
            ProvidersLoaded.Add(new ConfigurationFileValueProvider(jsonFile));
        }
        public void AddCommandLine(string argv)
        {
            ProvidersLoaded.Add(new CommandLineValueProvider(argv));
        }

        public ICollection<ConfigurationValueProvider> ProvidersLoaded { get; private set; }

        public ConfigurationEntries GetEntries()
        {
            var result = new ConfigurationEntries();
            var dict = new Dictionary<string, ConfigurationEntry>();

            ProvidersLoaded.Each(entry =>
            {
                var dtos = entry.GetEntries();
                dtos.Each(e =>
                {
                    if (!dict.ContainsKey(e.Key))
                        dict.Add(e.Key, new ConfigurationEntry(e.Key));

                    dict[e.Key].SetValue(e.Value, entry.Name);
                });
            });

            result.Entries.AddRange(dict.Values);

            return result;
        }


    }
}