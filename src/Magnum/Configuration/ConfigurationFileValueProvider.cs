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
    using System.IO;
    using System.Web.Script.Serialization;
    using Extensions;

    public class ConfigurationFileValueProvider :
        ConfigurationValueProvider
    {
        readonly string _path;

        public ConfigurationFileValueProvider(string path)
        {
            _path = path;
        }

        public string FileName
        {
            get { return _path; }
        }

        public ConfigurationDto[] GetEntries()
        {
            var serializer = new JavaScriptSerializer();

            using (FileStream str = new FileInfo(FileName).OpenRead())
            {
                string stuff = str.ReadToEndAsText();
                var dtos = serializer.Deserialize<ConfigurationDto[]>(stuff);
                return dtos;
            }
        }

        public string Name
        {
            get { return _path; }
        }
    }
}