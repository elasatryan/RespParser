namespace RespProtocol
{
    public class RespError : IRespType
    {
        public RespError(string value)
        {
            Value = value;
        }

        public string Value { get; set; }

        public override string ToString()
        {
            return $"Error: {Value}";
        }
    }
}
