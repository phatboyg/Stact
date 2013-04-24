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
namespace Stact.Configuration.Configurators
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;


    [Serializable]
    [DebuggerDisplay("{Message}")]
    public class StactConfigurationResult :
        ConfigurationResult
    {
        readonly IList<ValidateConfigurationResult> _results;

        public StactConfigurationResult(IEnumerable<ValidateConfigurationResult> results)
        {
            _results = results.ToList();
        }

        public bool IsFailed
        {
            get { return _results.Any(x => x.Disposition == ValidationConfigurationResultDisposition.Failure); }
        }

        public IEnumerable<ValidateConfigurationResult> Results
        {
            get { return _results; }
        }

        public string Message
        {
            get
            {
                string debuggerString = string.Join(", ", _results);

                return string.IsNullOrWhiteSpace(debuggerString)
                           ? ""
                           : debuggerString;
            }
        }
    }
}