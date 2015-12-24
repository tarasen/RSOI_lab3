using Nancy;
using Nancy.Security;
using PictureAuction.SOA.Frontend.Authentication;
using PictureAuction.SOA.Frontend.Extentions;
using PictureAuction.SOA.Frontend.Models;
using ServiceStack.Service;
using ServiceStack.ServiceClient.Web;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PictureAuction.SOA.Frontend.Networks
{
    public class SessionClient : ISessionClient, IDisposable
    {
        private readonly IRestClientAsync _sessionClient = new JsonServiceClient(Configuration.SessionBackendUri);

        public void Dispose()
        {
            _sessionClient.Dispose();
        }

        public async Task<Guid> CreateUserAsync(RegisterModel model)
        {
            var user = new PasswordIdentity
            {
                Salt = Path.GetRandomFileName(),
                Login = model.Login
            };

            user.PasswordHash = GetPasswordHash(model, user);

            var restResponse = await _sessionClient.PostItemAsync(user);
            return restResponse.Id;
        }

        public IUserIdentity GetUserFromIdentifier(Guid identifier, NancyContext context)
        {
            return
                Task.Run(async () => await _sessionClient.GetItemAsync<UserIdentity>($"?user_id={identifier}")).Result;
        }

        public async Task<Guid> ValidateUserAsync(LoginModel model)
        {
            var restResponse = await _sessionClient.GetItemAsync<PasswordIdentity>($"?login={model.Login}");

            var hashString = GetPasswordHash(model, restResponse);
            return hashString == restResponse.PasswordHash ? restResponse.Id : Guid.Empty;
        }

        private static string GetPasswordHash(LoginModel user, PasswordIdentity restResponse)
        {
            var enc = Encoding.UTF8;
            using (var sha1 = SHA1.Create())
            {
                var hash = sha1.ComputeHash(enc.GetBytes(user.Password + restResponse.Salt));
                return string.Join("", hash.Select(x => x.ToString("x")));
            }
        }
    }
}