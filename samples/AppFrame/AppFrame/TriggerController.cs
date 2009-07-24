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
namespace AppFrame
{
    using System.Windows.Forms;
    using Magnum.Pipeline;

    public class TriggerController
    {
        private readonly Pipe _eventPipe;
        private readonly Button _button;

        public TriggerController(Pipe eventPipe, Button button)
        {
            _eventPipe = eventPipe;
            _button = button;


        }

        public void Handle(TriggerEnabled message)
        {
            _button.Enabled = true;
        }

        public void Handle(TriggerDisabled message)
        {
            _button.Enabled = false;
        }
    }
}