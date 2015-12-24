using Funq;
using PictureAuction.SOA.Artists.ServiceInterface.Services;
using ServiceStack;
using ServiceStack.OrmLite;
using ServiceStack.OrmLite.SqlServer;
using ServiceStack.ServiceHost;
using ServiceStack.WebHost.Endpoints;
using ServiceStack.WebHost.Endpoints.Formats;
using System.Configuration;
using System.IO;

namespace PictureAuction.SOA.Artists
{
    public class AppHost : AppHostHttpListenerBase
    {
        public AppHost()
            : base("PictureAuction.SOA.Artists", typeof (ArtistService).Assembly)
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

            Plugins.RemoveAll(x => x is MetadataFeature);
            Plugins.RemoveAll(x => x is RequestInfoFeature);
            Plugins.RemoveAll(x => x is CsvFormat);
            Plugins.RemoveAll(x => x is MarkdownFormat);
        }
    }
}