using ServiceStack.Logging;
using ServiceStack.Logging.Support.Logging;
using ServiceStack.Text;
using System;

namespace PictureAuction.SOA.Pictures
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
                appHost.Start("http://*:1333/");

                "ServiceStack SelfHost listening at http://localhost:1333 ".Print();

                Console.ReadLine();
            }
        }
    }
}