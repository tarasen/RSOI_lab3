using ServiceStack.Common.Web;
using ServiceStack.ServiceHost;
using System.IO;

namespace PictureAuction.SOA.Images.ServiceModel.Routes
{
    public static class ImageRoutes
    {
        [Route("/img/{Name}.jpg", HttpMethods.Get)]
        public class GetImage : IReturn<Stream>
        {
            public string Name { get; set; }
        }
    }
}