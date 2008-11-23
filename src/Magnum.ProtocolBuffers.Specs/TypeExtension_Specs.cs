namespace Magnum.ProtocolBuffers.Specs
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using NUnit.Framework;

    [TestFixture]
    public class TypeExtension_Specs
    {
        [Test]
        public void Arrays_are_collections()
        {
            Type type = typeof (string[]);
            Assert.IsTrue(type.IsRepeatedType());

            type = typeof (IList<int>);
            Assert.IsTrue(type.IsRepeatedType(), "Closed Generic");

            type = typeof (IList<>);
            Assert.IsTrue(type.IsRepeatedType(), "Open generic");

            type = typeof (IList);
            Assert.IsTrue(type.IsRepeatedType(), "Non-Generic");
        }

        [Test]
        public void Is_required_type()
        {
            Type type = typeof (int);
            Assert.IsTrue(type.IsRequiredType());
        }

        [Test]
        public void Generic_List()
        {
            Type t = typeof (IList<int>);

            Type lt = t.GetTypeEnumerated();

            lt
                .ShouldEqual(typeof(int));
        }

        [Test]
        public void Array_List()
        {
            Type t = typeof(IList);

            Type lt = t.GetTypeEnumerated();

            lt
                .ShouldEqual(typeof(object));
        }

        [Test]
        public void Arrays()
        {
            Type t = typeof(int[]);

            Type lt = t.GetTypeEnumerated();

            lt
                .ShouldEqual(typeof(int));
        }
    }
}