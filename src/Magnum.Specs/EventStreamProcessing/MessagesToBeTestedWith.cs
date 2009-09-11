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
namespace Magnum.Specs.CEP
{
    public class LoginSucceeded : LoginAttempt
    {
        public LoginSucceeded(string username)
        {
            Username = username;
        }

        #region LoginAttempt Members

        public string Username { get; private set; }

        #endregion
    }

    public class LoginFailed : LoginAttempt
    {
        public LoginFailed(string username)
        {
            Username = username;
        }

        #region LoginAttempt Members

        public string Username { get; private set; }

        #endregion
    }

    public interface LoginAttempt
    {
        string Username { get; }
    }

    public class PossibleBruteForceAttack
    {
        public PossibleBruteForceAttack(params object[] messages)
        {
            Messages = messages;
        }

        private object[] Messages { get; set; }
    }
}