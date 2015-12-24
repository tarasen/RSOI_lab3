using Funq;
using PictureAuction.SOA.Session.ServiceInterface.Services;
using ServiceStack;
using ServiceStack.OrmLite;
using ServiceStack.OrmLite.SqlServer;
using ServiceStack.Redis;
using ServiceStack.ServiceHost;
using ServiceStack.Text;
using ServiceStack.WebHost.Endpoints;
using ServiceStack.WebHost.Endpoints.Formats;
using System;
using System.Configuration;
using System.IO;

namespace PictureAuction.SOA.Session
{
    public class AppHost : AppHostHttpListenerBase
    {
        public AppHost()
            : base("PictureAuction.SOA.Session", typeof (AuthService).Assembly)
        {
        }

        public override void Configure(Container container)
        {
            container.Register<IDbConnectionFactory>
                (new OrmLiteConnectionFactory(
                    ConfigurationManager.ConnectionStrings["PictureAuctionConnectionString"].ConnectionString,
                    SqlServerOrmLiteDialectProvider.Instance));
            container.Register<IRedisClientsManager>(c => new PooledRedisClientManager("localhost:6379"));
            SetConfig(new EndpointHostConfig
            {
#if DEBUG
                DebugMode = true,
                WebHostPhysicalPath = Path.GetFullPath(Path.Combine("~".MapServerPath(), "..", ".."))
#endif
            });

            JsConfig<DateTime>.SerializeFn = time => new DateTime(time.Ticks, DateTimeKind.Local).ToString("yyyy");

            Plugins.RemoveAll(x => x is MetadataFeature);
            Plugins.RemoveAll(x => x is RequestInfoFeature);
            Plugins.RemoveAll(x => x is CsvFormat);
            Plugins.RemoveAll(x => x is MarkdownFormat);
        }
    }
}