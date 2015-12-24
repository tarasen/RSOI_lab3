using Nancy.Security;
using System;
using System.Collections.Generic;

namespace PictureAuction.SOA.Frontend.Authentication
{
    public class UserIdentity : IUserIdentity
    {
        public Guid Id { get; set; }
        public string Login { get; set; }
        public IEnumerable<string> Claims { get; set; }
        public string UserName => Login;
    }
}