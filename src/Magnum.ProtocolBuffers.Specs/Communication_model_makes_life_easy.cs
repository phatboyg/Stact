namespace Magnum.ProtocolBuffers.Specs
{
    using NUnit.Framework;

    [TestFixture]
    public class Communication_model_makes_life_easy
    {
        [Test]
        public void By_auto_registering_maps()
        {
            var model = new CommunicationModel();
            model.AddMappingsFromAssembly(GetType().Assembly);
            Assert.AreEqual(3, model.NumberOfMessagesMapped);
        }
        [Test]
        public void By_auto_registering_maps_perf()
        {
            for (int i = 0; i < 10000; i++)
            {

                var model = new CommunicationModel();
                model.AddMappingsFromAssembly(GetType().Assembly);
                Assert.AreEqual(3, model.NumberOfMessagesMapped);
            }
        }
    }
}