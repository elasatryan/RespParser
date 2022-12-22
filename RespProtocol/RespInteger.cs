namespace RespProtocol
{
    public class RespInteger : IRespType
    {
        public RespInteger(ulong value)
        {
            Value = value;
        }

        public ulong Value { get; set; }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
