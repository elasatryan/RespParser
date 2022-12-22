namespace RespProtocol
{
    public class RespBulkString : IRespType
    {
        public RespBulkString(string value)
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
