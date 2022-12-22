namespace RespProtocol
{
    public class RespArray : IRespType
    {
        public RespArray(IRespType?[] value)
        {
            Value = value;
        }

        public IRespType?[] Value { get; set; }

        public override string ToString()
        {
            return $"[{string.Join(", ", Value.Select(x => x.ToString()))}]";
        }
    }
}
