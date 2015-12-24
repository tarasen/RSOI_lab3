using Funq;
using PictureAuction.SOA.Images.ServiceInterface.Services;
using ServiceStack;
using ServiceStack.ServiceHost;
using ServiceStack.WebHost.Endpoints;
using ServiceStack.WebHost.Endpoints.Formats;
using System.IO;

namespace PictureAuction.SOA.Images
{
    public class AppHost : AppHostHttpListenerBase
    {
        public AppHost()
            : base("PictureAuction.SOA.Images", typeof (ImageService).Assembly)
        {
        }

        public override void Configure(Container container)
        {
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