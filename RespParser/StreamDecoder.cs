using System.Text;

namespace RespParser
{
    public class StreamDecoder
    {

        private const char STRING_PREFIX = '+';
        private const char BULK_STRING_PREFIX = '$';
        private const char INTEGER_PREFIX = ':';
        private const char ERROR_PREFIX = '-';
        private const char ARRAY_PREFIX = '*';

        private const char END_OF_LINE_R = '\r';
        private const char END_OF_LINE_N = '\n';

        private const int END_OF_LINE_SYMBOLS_COUNT = 2;

        private int _position = 0;

        private UTF8Encoding _utf8 = new UTF8Encoding();

        public StreamDecoder()
        {


        }

        public bool GetNext(byte[] bytes, out Tuple<string, string[]> nextItem)
        {
            nextItem = null;
            if (bytes != null && bytes.Length > 0 && _position < bytes.Length)
            {
                nextItem = GetNextLine(bytes, _position);

                if (nextItem.Item1 == null)
                {
                    _position += nextItem.Item2.Sum(h => h.Length + END_OF_LINE_SYMBOLS_COUNT);
                    _position += 1 + nextItem.Item2.Length.ToString().Length + END_OF_LINE_SYMBOLS_COUNT;
                }
                else
                {
                    _position += nextItem.Item1.Length + END_OF_LINE_SYMBOLS_COUNT; // +2 for \r\n
                }
                return true;
            }

            return false;
        }

        private string GeneralCase(byte[] bytes, int startIndex)
        {
            return _utf8.GetString(bytes[startIndex..].TakeWhile((value, index) =>
                        (value != BULK_STRING_PREFIX &&
                        value != ARRAY_PREFIX &&
                        value != END_OF_LINE_R)).ToArray());
        }

        private string GetBulkString(byte[] bytes, int startIndex)
        {
            string result = String.Empty;

            string dataLength = _utf8.GetString(bytes[(startIndex + 1)..]
                .TakeWhile((value, index) =>
                value != END_OF_LINE_R).ToArray());

            string start = _utf8.GetString(bytes[startIndex..]
                .Take(1 + dataLength.Length + END_OF_LINE_SYMBOLS_COUNT).ToArray());

            result += start + _utf8.GetString(bytes[(start.Length + startIndex)..].Take(int.Parse(dataLength)).ToArray());

            return result;
        }

        private string[] GetArrayByteData(byte[] bytes, int startIndex)
        {

            string arrLength = _utf8.GetString(bytes[(startIndex + 1)..]
                    .TakeWhile((value, index) =>
                    value != END_OF_LINE_R).ToArray());

            string[] result = new string[int.Parse(arrLength)];

            string start = _utf8.GetString(bytes[startIndex..].Take(1 + arrLength.Length + END_OF_LINE_SYMBOLS_COUNT).ToArray());

            startIndex += start.Length;

            for (int i = 0; i < int.Parse(arrLength); i++)
            {
                result[i] = GetNextLine(bytes, startIndex).Item1;
                startIndex += result[i].Length + END_OF_LINE_SYMBOLS_COUNT;
            }

            return result;
        }

        private Tuple<string, string[]> GetNextLine(byte[] bytes, int startIndex)
        {
            string nextLine = null;
            string[] nextLineArray = null;

            switch (bytes[startIndex])
            {

                case (byte)BULK_STRING_PREFIX:
                    nextLine = GetBulkString(bytes, startIndex);
                    break;
                case (byte)ARRAY_PREFIX:
                    nextLineArray = GetArrayByteData(bytes, startIndex);
                    break;
                default:
                    nextLine = GeneralCase(bytes, startIndex);
                    break;
            }

            return new Tuple<string, string[]>(nextLine, nextLineArray);
        }
    }
}