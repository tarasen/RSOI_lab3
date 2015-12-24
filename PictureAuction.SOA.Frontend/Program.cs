using Nancy.Hosting.Self;
using System;

namespace PictureAuction.SOA.Frontend
{
    internal static class Program
    {
        private static void Main()
        {
            var uri = new Uri("http://localhost:1337");

            var hostConfiguration = new HostConfiguration {UrlReservations = {CreateAutomatically = true}};
            using (var host = new NancyHost(uri, new Bootstrapper(), hostConfiguration))
            {
                host.Start();

                Console.WriteLine("Your application is running on {0}", uri.AbsoluteUri);
                Console.WriteLine("Press any [Enter] to close the host.");
                Console.ReadLine();
            }
        }
    }
}