using RespProtocol;

namespace RespParser
{
    public class Parser
    {
        private const char STRING_PREFIX = '+';
        private const char BULK_STRING_PREFIX = '$';
        private const char INTEGER_PREFIX = ':';
        private const char ERROR_PREFIX = '-';
        private const char ARRAY_PREFIX = '*';

        private const string END_OF_LINE = "\r\n";

        public List<IRespType?> Parse(byte[] bytes)
        {
            StreamDecoder streamDecoder = new StreamDecoder();
            List<IRespType?> respTypes = new List<IRespType?>();

            Tuple<string, string[]> next = null;

            while (streamDecoder.GetNext(bytes, out next))
            {
                respTypes.Add(parser(next));
            }

            return respTypes;
        }

        private IRespType? parser(Tuple<string, string[]> nextItem)
        {
            IRespType? resp = null;

            if (!string.IsNullOrEmpty(nextItem.Item1))
            {
                switch (nextItem.Item1.First())
                {
                    case STRING_PREFIX:
                        resp = GetSimpleString(nextItem.Item1);
                        break;
                    case BULK_STRING_PREFIX:
                        resp = GetBulkString(nextItem.Item1);
                        break;
                    case INTEGER_PREFIX:
                        resp = GetInteger(nextItem.Item1);
                        break;
                    case ERROR_PREFIX:
                        resp = GetError(nextItem.Item1);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                resp = GetArray(nextItem.Item2);
            }

            return resp;
        }

        private RespInteger GetInteger(string line)
        {
            ulong value = ulong.Parse(line.Substring(1));

            return new RespInteger(value);
        }

        private RespSimpleString GetSimpleString(string line)
        {
            return new RespSimpleString(line.Substring(1));
        }

        private RespBulkString GetBulkString(string line)
        {
            line = line.Substring(1); // removed first delimiter
            int dataLength = GetDataLength(line);
            if (dataLength == 0)
            {
                return new RespBulkString("");
            }
            else if (dataLength == -1)
            {
                return null;
            }

            line = line.Replace(END_OF_LINE, string.Empty).Replace("" + dataLength, string.Empty); //removed size of data

            return new RespBulkString(line);
        }

        private RespError GetError(string line)
        {
            return new RespError(line.Substring(1));
        }

        private RespArray GetArray(string[] line)
        {
            IRespType?[] respTypes = new IRespType[line.Length];

            for (int i = 0; i < line.Length; i++)
            {
                if (line[i].First() == ARRAY_PREFIX)
                {
                    respTypes[i] = parser(new Tuple<string, string[]>(null, line));
                }
                else
                {
                    respTypes[i] = parser(new Tuple<string, string[]>(line[i], null));
                }
            }

            return new RespArray(respTypes);
        }

        private int GetDataLength(string line)
        {
            int value = 0;
            line = line.Substring(0, line.IndexOf('\r'));
            int.TryParse(line, out value);

            return value;
        }
    }
}
