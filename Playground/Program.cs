using RespProtocol;

namespace Playground
{
    internal class Program
    {
        // Simple string example
        //      "+OK\r\n"
        // Error example
        //      "-Error message\r\n"
        // Integer example
        //      "-Error message\r\n"
        // Bulk string example
        //      "$5\r\nhello\r\n"
        //      "$0\r\n\r\n"
        //      "$-1\r\n"
        // Array example
        //      "*2\r\n$5\r\nhello\r\n$5\r\nworld\r\n"
        //      "*2\r\n*2\r\n$5\r\nhello\r\n$5\r\nworld\r\n\r\n$5\r\nworld \r\n"


        static void Main(string[] args)
        {
            var integer = new RespInteger(1);
            var simpleString = new RespSimpleString("simple");
            var bulkString = new RespBulkString("bulk");
            var error = new RespError("error");
            var array = new RespArray(new IRespType?[]{ integer, simpleString, bulkString, error, null });

            Console.WriteLine(integer);
            Console.WriteLine(simpleString);
            Console.WriteLine(bulkString);
            Console.WriteLine(error);
            Console.WriteLine(array);
        }
    }
}