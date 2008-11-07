namespace Magnum.ProtocolBuffers.ScalarValueTypes
{
    using System;

    public class Double : ScalarBase
    {
        public override Type DotNetType
        {
            get { return typeof(double); }
        }

        public override string Name
        {
            get { return "double"; }
        }
    }
}