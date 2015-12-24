using Nancy;
using Nancy.Authentication.Forms;
using Nancy.ModelBinding;
using PictureAuction.SOA.Frontend.Authentication;
using PictureAuction.SOA.Frontend.Models;
using System;

namespace PictureAuction.SOA.Frontend.Modules
{
    public class AuthModule : NancyModule
    {
        public AuthModule(ISessionClient sessionClient)
        {
            Get["/login"] = arg => View["Views/login.sshtml"];
            Get["/register"] = arg => View["Views/register.sshtml"];

            Post["/login", true] = async (p, c) =>
            {
                var user = this.Bind<LoginModel>();
                var userGuid = await sessionClient.ValidateUserAsync(user);

                if (userGuid == Guid.Empty)
                {
                    user.Password = "";
                    return View["Views/login.sshtml", user];
                }

                DateTime? expiry = DateTime.Now + TimeSpan.FromDays(7);
                return this.LoginAndRedirect(userGuid, expiry);
            };

            Post["/register", true] = async (p, c) =>
            {
                var user = this.Bind<RegisterModel>();
                if (user.Password != user.PasswordRepeat)
                {
                    user.Password = "";
                    user.PasswordRepeat = "";
                    return View["Views/register.sshtml", user];
                }

                var userGuid = await sessionClient.CreateUserAsync(user);

                DateTime? expiry = DateTime.Now + TimeSpan.FromDays(7);
                return this.LoginAndRedirect(userGuid, expiry);
            };

            Get["/logout"] = x => this.LogoutAndRedirect("~/");
        }
    }
}