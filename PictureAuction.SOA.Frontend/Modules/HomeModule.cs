using Nancy;
using Nancy.Responses;

namespace PictureAuction.SOA.Frontend.Modules
{
    public class HomeModule : NancyModule
    {
        public HomeModule()
        {
            Get["/"] = parameters => new RedirectResponse("pictures");
        }
    }
}