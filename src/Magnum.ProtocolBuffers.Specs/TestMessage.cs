namespace Magnum.ProtocolBuffers.Specs
{
    using System;
    using System.Collections.Generic;

    public class TestMessage
    {
        public string Name { get; set; }
        public IList<int> Numbers { get; set; }

        public int Age { get; set; }
        public int? NumberOfPets { get; set; }
        public DateTime BirthDay { get; set; }
        public DateTime? DeadDay { get; set; }
    }
}