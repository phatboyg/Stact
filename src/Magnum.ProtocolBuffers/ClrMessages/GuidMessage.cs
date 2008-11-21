namespace Magnum.ProtocolBuffers.ClrMessages
{
    using System;
    using Common;
    using Mapping;

    public class GuidMessageMap :
        MessageMap<GuidMessage>
    {
        public GuidMessageMap()
        {
            Field(m => m.Low).MakeRequired();
            Field(m => m.High).MakeRequired();
        }

        
    }

    public class GuidMessage
    {
        private byte[] _low = new byte[8];
        private byte[] _high = new byte[8];
        private Guid _value;

        //fixed int64
        public byte[] Low
        {
            get
            {

                return _low;
            }
        }
        public byte[] High { get
        {
            return _high;
        } }

        public Guid Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                byte[] x = _value.ToByteArray();
                Range<int> lower = 0.Through(7);
                Range<int> upper = 8.Through(15);
                
                foreach(var i in lower.Forward(step=>step+1))
                    _low[i] = x[i];
                
                foreach (var i in upper.Forward(step => step + 1))
                    _low[i-7] = x[i-7];
            }
        }
    }
}