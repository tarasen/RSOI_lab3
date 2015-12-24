using ServiceStack.Logging;
using ServiceStack.Logging.Support.Logging;
using ServiceStack.Text;
using System;

namespace PictureAuction.SOA.Images
{
    internal static class Program
    {
        private static void Main()
        {
            using (var appHost = new AppHost())
            {
#if DEBUG
                LogManager.LogFactory = new ConsoleLogFactory();
#endif

                appHost.Init();
                appHost.Start("http://*:1331/");

                "ServiceStack SelfHost listening at http://localhost:1331 ".Print();

                Console.ReadLine();
            }
        }
    }
}