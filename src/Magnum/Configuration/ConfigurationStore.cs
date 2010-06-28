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
    using Magnum.Extensions;

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

        public IEnumerable<ConfigurationEntry> GetEntries()
        {
            var js = new JavaScriptSerializer();
            var dict = new Dictionary<string, ConfigurationEntry>();

            FilesLoaded.Each(f =>
            {
                using (FileStream str = new FileInfo(f.FileName).OpenRead())
                {
                    string stuff = str.ReadToEndAsText();
                    var col =  js.Deserialize<ConfigurationEntries>(stuff);
                    col.Entries.Each(e =>
                    {
                        if (!dict.ContainsKey(e.Key))
                            dict.Add(e.Key, null);

                        dict[e.Key] = e;
                    });
                }
            });


            return dict.Values;
        }
    }
}