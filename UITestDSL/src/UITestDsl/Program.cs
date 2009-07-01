using System.IO;

namespace UITestDsl
{
    internal class Program
    {
        private static void Main( string[] args )
        {
            string path = "../../../example.uit";
            Stream file = File.OpenRead( path );
            Scanner scanner = new Scanner(file);
            Parser parser = new Parser( scanner );
            bool res = parser.Parse();
        }
    }
}