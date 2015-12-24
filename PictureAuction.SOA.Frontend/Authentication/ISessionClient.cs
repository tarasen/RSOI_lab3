using Nancy.Authentication.Forms;
using PictureAuction.SOA.Frontend.Models;
using System;
using System.Threading.Tasks;

namespace PictureAuction.SOA.Frontend.Authentication
{
    public interface ISessionClient : IUserMapper
    {
        Task<Guid> CreateUserAsync(RegisterModel user);

        Task<Guid> ValidateUserAsync(LoginModel model);
    }
}