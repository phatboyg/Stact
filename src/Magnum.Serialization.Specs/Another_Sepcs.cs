namespace Magnum.Serialization.Specs
{
    using NUnit.Framework;

    [TestFixture]
    public class Another_Sepcs
    {
        [Test]
        public void How_the_fuck_is_this_hard()
        {
            FlatObject flat = new FlatObject("Some String", 47, 9.95m);

            Cerealizer cereal = new Cerealizer();

            string result = cereal.MilkIt(flat);

            Assert.AreEqual("Some XML", result);
        }


        public class FlatObject
        {
            private readonly string _text;
            private readonly int _number;
            private readonly decimal _amount;

            public FlatObject(string text, int number, decimal amount)
            {
                _text = text;
                _number = number;
                _amount = amount;
            }

            public string Text
            {
                get { return _text; }
            }

            public int Number
            {
                get { return _number; }
            }

            public decimal Amount
            {
                get { return _amount; }
            }
        }
    }
}