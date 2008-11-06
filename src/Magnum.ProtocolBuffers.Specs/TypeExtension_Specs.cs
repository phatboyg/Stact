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
            Assert.IsTrue(type.IsCollection());

            type = typeof (IList<int>);
            Assert.IsTrue(type.IsCollection(), "Closed Generic");

            type = typeof (IList<>);
            Assert.IsTrue(type.IsCollection(), "Open generic");

            type = typeof (IList);
            Assert.IsTrue(type.IsCollection(), "Non-Generic");
        }
    }
}