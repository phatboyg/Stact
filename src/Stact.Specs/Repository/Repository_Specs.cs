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
namespace Stact.Specs.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Stact.Data;
    using NUnit.Framework;
    using Rhino.Mocks;

    [TestFixture]
    public class When_loading_a_value_from_a_repository
    {
        #region Setup/Teardown

        [SetUp]
        public void Before_each()
        {
            _memberRepository = new MemberRepository();
            _memberRepository.Save(new Member(_memberId));

            _mocks = new MockRepository();
            _container = _mocks.DynamicMock<IObjectBuilder>();
            SetupResult.For(_container.Resolve<MemberRepository>()).Return(_memberRepository);

            _mocks.ReplayAll();
        }

        #endregion

        private MockRepository _mocks;
        private IObjectBuilder _container;
        private MemberRepository _memberRepository;
        private readonly Guid _memberId = new Guid("3DAEE114-10A8-4D96-9A51-5E7BFEABC764");

        [Test]
        public void The_object_should_be_loaded_without_issues()
        {
            using (MemberRepository repository = _container.Resolve<MemberRepository>())
            {
                var query = from m in repository where m.Id == _memberId select m;

                Member member = query.FirstOrDefault();

                Assert.IsNotNull(member);
                //Assert.That(member, Is.Not.Null);

                Assert.AreEqual(_memberId, member.Id);
                //Assert.That(member.Id, Is.EqualTo(_memberId));
            }
        }
    }

    public class MemberRepository :
        RepositoryBase<Member>
    {
        private List<Member> _members = new List<Member>();

        protected override IQueryable<Member> RepositoryQuery
        {
            get { return _members.AsQueryable(); }
        }

        public override void Dispose()
        {
            _members.Clear();
            _members = null;
        }

        public override void Save(Member member)
        {
            _members.Add(member);
        }

        public override void Update(Member item)
        {
        }

        public override void Delete(Member item)
        {
        	_members.Remove(item);
        }
    }

    public class Member : IAggregateRoot<Guid>
    {
        private readonly Guid _id;

        public Member(Guid id)
        {
            _id = id;
        }

        public Guid Id
        {
            get { return _id; }
        }
    }

    public interface IObjectBuilder
    {
        T Resolve<T>();
    }
}