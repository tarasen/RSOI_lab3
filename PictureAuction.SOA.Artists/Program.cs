using ServiceStack.Logging;
using ServiceStack.Logging.Support.Logging;
using ServiceStack.Text;
using System;

namespace PictureAuction.SOA.Artists
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
                appHost.Start("http://*:1330/");

                "ServiceStack SelfHost listening at http://localhost:1330 ".Print();

                Console.ReadLine();
            }
        }
    }
}