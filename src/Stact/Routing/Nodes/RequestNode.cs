// Copyright 2010 Chris Patterson
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
namespace Stact.Routing.Nodes
{
    using Contexts;


    public class RequestNode<T> :
        Activation<T>
    {
        readonly Activation<Request<T>> _output;

        public RequestNode(Activation<Request<T>> output)
        {
            _output = output;
        }

        public Activation<Request<T>> Output
        {
            get { return _output; }
        }

        public bool Enabled
        {
            get { return _output.Enabled; }
        }

        public void Activate(RoutingContext<T> context)
        {
            context.Match(message => UpgradeMessageToRequest(context, message),
                          request => _output.Activate(request),
                          response => { });
        }

        void UpgradeMessageToRequest(RoutingContext<T> context, RoutingContext<Message<T>> message)
        {
            var proxy = new RequestMessageProxy<T>(message.Body);
            var requestProxy = new RequestRoutingContextProxy<T, T>(context, proxy);
            _output.Activate(requestProxy);
        }
    }
}