namespace RespProtocol
{
    public class RespSimpleString : IRespType
    {
        public RespSimpleString(string value)
        {
            Value = value;
        }

        public string Value { get; set; }

        public override string ToString()
        {
            return Value;
        }
    }
}
