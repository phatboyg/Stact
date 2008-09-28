namespace Magnum.Common.Specs.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Common.Repository;
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;
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

                Assert.That(member, Is.Not.Null);

                Assert.That(member.Id, Is.EqualTo(_memberId));
            }
        }
    }

    public class MemberRepository :
        RepositoryBase<Member, Guid>
    {
        //private readonly IRepository<Member> _repository;

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

        public override Member Get(Guid id)
        {
            throw new NotImplementedException();
        }

        public override IList<Member> List()
        {
            throw new NotImplementedException();
        }

        public override void Save(Member member)
        {
            _members.Add(member);
        }

        public override void Update(Member item)
        {
            throw new System.NotImplementedException();
        }

        public override void Delete(Member item)
        {
            throw new NotImplementedException();
        }
    }

    public class Member :
        IAggregateRoot
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