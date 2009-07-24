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
    using System;
    using System.ComponentModel;
    using System.Windows.Forms;
    using Magnum.Pipeline;

    public partial class MainForm :
        Form
    {
        private readonly Pipe _eventPipe;
        private SubscriptionScope _subscriptionScope;

        public MainForm(Pipe eventPipe)
        {
            _eventPipe = eventPipe;

            InitializeComponent();
        }

        private void triggerEnabledCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (triggerEnabledCheckBox.Checked)
                _eventPipe.Send(new TriggerEnabled());
            else
                _eventPipe.Send(new TriggerDisabled());
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            _subscriptionScope = _eventPipe.NewSubscriptionScope();


            var controller = new TriggerController(_eventPipe, triggerButton);
            _subscriptionScope.Subscribe<TriggerEnabled>(controller.Handle);
            _subscriptionScope.Subscribe<TriggerDisabled>(controller.Handle);

            triggerEnabledCheckBox.Checked = triggerButton.Enabled;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            _subscriptionScope.Dispose();

            base.OnClosing(e);
        }
    }
}