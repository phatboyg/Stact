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
    using System.IO;
    using System.Web.Script.Serialization;
    using Extensions;

    public class ConfigurationStore
    {
        public ConfigurationStore()
        {
            FilesLoaded = new List<ConfigurationFile>();
        }

        public void AddFile(string jsonFile)
        {
            FilesLoaded.Add(new ConfigurationFile(jsonFile));
        }

        public ICollection<ConfigurationFile> FilesLoaded { get; private set; }

        public ConfigurationEntries GetEntries()
        {
            var result = new ConfigurationEntries();
            var serializer = new JavaScriptSerializer();
            var dict = new Dictionary<string, ConfigurationEntry>();

            FilesLoaded.Each(file =>
            {
                using (FileStream str = new FileInfo(file.FileName).OpenRead())
                {
                    string stuff = str.ReadToEndAsText();
                    var dtos = serializer.Deserialize<ConfigurationDto[]>(stuff);
                    dtos.Each(e =>
                    {
                        if (!dict.ContainsKey(e.Key))
                            dict.Add(e.Key, new ConfigurationEntry(e.Key));

                        dict[e.Key].SetValue(e.Value, file.FileName);
                    });
                }
            });

            result.Entries.AddRange(dict.Values);

            return result;
        }
    }
}