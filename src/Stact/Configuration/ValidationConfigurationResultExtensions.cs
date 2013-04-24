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
namespace Stact.Configuration
{
    using Configurators;


    public static class ValidationConfigurationResultExtensions
    {
        public static ValidateConfigurationResult Failure(this Configurator configurator, string message)
        {
            return new ValidateConfigurationResultImpl(ValidationConfigurationResultDisposition.Failure, message);
        }

        public static ValidateConfigurationResult Failure(this Configurator configurator, string key, string message)
        {
            return new ValidateConfigurationResultImpl(ValidationConfigurationResultDisposition.Failure, key, message);
        }

        public static ValidateConfigurationResult Failure(this Configurator configurator, string key, string value,
            string message)
        {
            return new ValidateConfigurationResultImpl(ValidationConfigurationResultDisposition.Failure, key, value,
                message);
        }

        public static ValidateConfigurationResult Warning(this Configurator configurator, string message)
        {
            return new ValidateConfigurationResultImpl(ValidationConfigurationResultDisposition.Warning, message);
        }

        public static ValidateConfigurationResult Warning(this Configurator configurator, string key, string message)
        {
            return new ValidateConfigurationResultImpl(ValidationConfigurationResultDisposition.Warning, key, message);
        }

        public static ValidateConfigurationResult Warning(this Configurator configurator, string key, string value,
            string message)
        {
            return new ValidateConfigurationResultImpl(ValidationConfigurationResultDisposition.Warning, key, value,
                message);
        }

        public static ValidateConfigurationResult Success(this Configurator configurator, string message)
        {
            return new ValidateConfigurationResultImpl(ValidationConfigurationResultDisposition.Success, message);
        }

        public static ValidateConfigurationResult Success(this Configurator configurator, string key, string message)
        {
            return new ValidateConfigurationResultImpl(ValidationConfigurationResultDisposition.Success, key, message);
        }

        public static ValidateConfigurationResult Success(this Configurator configurator, string key, string value,
            string message)
        {
            return new ValidateConfigurationResultImpl(ValidationConfigurationResultDisposition.Success, key, value,
                message);
        }

        public static ValidateConfigurationResult WithParentKey(this ValidateConfigurationResult result,
            string parentKey)
        {
            string key = parentKey + "." + result.Key;

            return new ValidateConfigurationResultImpl(result.Disposition, key, result.Value, result.Message);
        }
    }
}