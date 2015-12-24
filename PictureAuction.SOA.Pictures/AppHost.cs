using Funq;
using PictureAuction.SOA.Pictures.ServiceInterface.Services;
using ServiceStack;
using ServiceStack.OrmLite;
using ServiceStack.OrmLite.SqlServer;
using ServiceStack.ServiceHost;
using ServiceStack.Text;
using ServiceStack.WebHost.Endpoints;
using ServiceStack.WebHost.Endpoints.Formats;
using System;
using System.Configuration;
using System.IO;

namespace PictureAuction.SOA.Pictures
{
    public class AppHost : AppHostHttpListenerBase
    {
        public AppHost()
            : base("PictureAuction.Api.Pictures", typeof (PictureService).Assembly)
        {
        }

        public override void Configure(Container container)
        {
            container.Register<IDbConnectionFactory>
                (new OrmLiteConnectionFactory(
                    ConfigurationManager.ConnectionStrings["PictureAuctionConnectionString"].ConnectionString,
                    SqlServerOrmLiteDialectProvider.Instance));

            SetConfig(new EndpointHostConfig
            {
#if DEBUG
                DebugMode = true,
                WebHostPhysicalPath = Path.GetFullPath(Path.Combine("~".MapServerPath(), "..", ".."))
#endif
            });

            JsConfig<DateTime>.SerializeFn = time => time.ToString("yyyy");

            Plugins.RemoveAll(x => x is MetadataFeature);
            Plugins.RemoveAll(x => x is RequestInfoFeature);
            Plugins.RemoveAll(x => x is CsvFormat);
            Plugins.RemoveAll(x => x is MarkdownFormat);
        }
    }
}