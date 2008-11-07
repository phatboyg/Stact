namespace Magnum.ProtocolBuffers.Specs
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using NUnit.Framework;

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
    }
}