namespace Magnum.Serialization.Specs
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;

    [TestFixture]
    public class ReflectiveVisitor_Exp
    {
        [Test]
        public void We_should_go_through_the_list_quickly()
        {
            DerivedObject obj = new DerivedObject("Chris", "Patterson", "Pimp");
            DerivedObject obj2 = new DerivedObject("Joe", "Blow", "Daddy");

            //IObjectFormatter fomatter = new XmlObjectFormatter();

            MemoryStream mstream = new MemoryStream();

            using(IObjectFormatter formatter = new BEncodeObjectFormatter(mstream))
            using(IObjectSerializer serializer = new BasicObjectSerializer(formatter))
            {
                serializer.Serialize(obj);
                serializer.Serialize(obj2);
            }

            Debug.WriteLine("DATA: " + Encoding.UTF8.GetString(mstream.ToArray()));

            MemoryStream mdecode = new MemoryStream(mstream.ToArray());

            using(BDecode decoder = new BDecode(mdecode))
            {
                object newObj = decoder.Read(TimeSpan.FromSeconds(200));

                Assert.That(newObj, Is.Not.Null);
                Assert.That(newObj, Is.TypeOf(typeof(DerivedObject)), "Invalid Type Received");

                object newObj2 = decoder.Read(TimeSpan.FromSeconds(200));

                Assert.That(newObj2, Is.Not.Null);
                Assert.That(newObj2, Is.TypeOf(typeof(DerivedObject)), "Invalid Type Received");


            }
        }
    }
}