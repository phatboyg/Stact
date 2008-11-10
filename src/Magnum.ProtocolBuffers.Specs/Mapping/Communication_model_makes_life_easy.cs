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
            model.AddMappingsFromAssembly(this.GetType().Assembly);
            Assert.AreEqual(1, model.NumberOfMessagesMapped);
        }
        
    }
}