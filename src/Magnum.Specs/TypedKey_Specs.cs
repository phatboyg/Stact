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
namespace Magnum.Specs
{
    using System.Collections;
    using NUnit.Framework;

    [TestFixture]
    public class TypedKey_Specs
    {
        [Test]
        public void FIRST_TEST_NAME()
        {
            Hashtable items = new Hashtable();

            Range<int> through = 1.Through(5);
            items.Store(through);

            var value = items.Retrieve<Range<int>>();

            value.ShouldEqual(through);
        }
    }
}