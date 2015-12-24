using PictureAuction.SOA.Shared.ServiceModel.Types;
using ServiceStack.Common.Web;
using ServiceStack.ServiceHost;
using System;
using System.Runtime.Serialization;

namespace PictureAuction.SOA.Session.ServiceModel.Routes
{
    public static class AuthRoutes
    {
        [Route("/user", HttpMethods.Get)]
        [DataContract]
        public class AuthRequest : IReturn<User>
        {
            [DataMember(Name = "login")]
            public string Login { get; set; }

            [DataMember(Name = "user_id")]
            public Guid? UserId { get; set; }
        }

        [Route("/user", HttpMethods.Post)]
        public class RegistrRequest : User, IReturn<User>
        {
        }
    }
}