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
namespace Magnum.Specs.Data
{
    using System.Data;
    using Common.Data;
    using MbUnit.Framework;

    [TestFixture]
    public class UnitOfWork_Specs
    {
        [Test]
        public void A_default_provider_should_silently_behave_properly()
        {
            using (var unitOfWork = UnitOfWork.Start())
            using (var transaction = unitOfWork.BeginTransaction(IsolationLevel.Serializable))
            {
                // I'm not doing anything here, just testing the syntax.

                transaction.Commit();
            }
            UnitOfWork.Finish();
        }
    }
}