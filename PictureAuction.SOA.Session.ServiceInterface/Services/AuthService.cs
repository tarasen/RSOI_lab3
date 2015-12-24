using AutoMapper;
using PictureAuction.SOA.Session.ServiceModel.Routes;
using PictureAuction.SOA.Shared.DTOs;
using PictureAuction.SOA.Shared.ServiceModel.Types;
using ServiceStack.Common;
using ServiceStack.Common.Web;
using ServiceStack.OrmLite;
using ServiceStack.ServiceInterface;
using ServiceStack.Text;
using System;
using System.Linq;
using System.Net;

namespace PictureAuction.SOA.Session.ServiceInterface.Services
{
    public class AuthService : Service
    {
        static AuthService()
        {
            Mapper.CreateMap<AuthRoutes.RegistrRequest, User>();
            Mapper.CreateMap<Token, AuthDTO.TokenResponse>()
                .ForMember(x => x.ExpiresIn,
                    expression => expression.MapFrom(e => (e.ExpiresIn - DateTime.Now).Milliseconds));
        }

        [DefaultView("Login")]
        public object Get(AuthRoutes.AuthRequest request)
        {
            if (!request.Login.IsNullOrEmpty())
            {
                if (Redis.ContainsKey(request.Login))
                {
                    var responseText = Redis.Get<string>(request.Login);
                    return new HttpResult(responseText, $"{MimeTypes.Json}; charset=utf-8");
                }
                var user =
                    Db.Select<User>(
                        x =>
                            x.Limit(1)
                                .Where(
                                    $"Login = {OrmLiteConfig.DialectProvider.GetQuotedValue(request.Login, request.Login.GetType())}"))
                        .FirstOrDefault();
                if (user == null)
                    return new HttpError(HttpStatusCode.BadRequest, "Bad Request");

                var json = JsonSerializer.SerializeToString(user);
                Redis.Set(user.Id.ToString(), user.Login);
                Redis.Set(user.Login, json);

                return new HttpResult(json, $"{MimeTypes.Json}; charset=utf-8");
            }
            if (request.UserId != null)
            {
                if (Redis.ContainsKey(request.UserId.ToString()))
                {
                    var login = Redis.Get<string>(request.UserId.ToString());
                    var userjson = Redis.Get<string>(login);
                    return new HttpResult(userjson, $"{MimeTypes.Json}; charset=utf-8");
                }
                var user =
                    Db.Select<User>(
                        x =>
                            x.Limit(1)
                                .Where(
                                    $"UserId = {OrmLiteConfig.DialectProvider.GetQuotedValue(request.UserId.Value, typeof (Guid))}"))
                        .FirstOrDefault();
                if (user == null)
                    return new HttpError(HttpStatusCode.BadRequest, "Bad Request");

                var jsonl = JsonSerializer.SerializeToString(user);
                Redis.Set(user.Id.ToString(), user.Login);
                Redis.Set(user.Login, jsonl);

                return new HttpResult(jsonl, $"{MimeTypes.Json}; charset=utf-8");
            }
            return new HttpError(HttpStatusCode.BadRequest, "Bad Request");
        }

        public object Post(AuthRoutes.RegistrRequest request)
        {
            try
            {
                var entity = Mapper.Map<User>(request);
                entity.Id = Guid.NewGuid();
                Db.Save(entity);

                var json = JsonSerializer.SerializeToString(entity);
                Redis.Set(entity.Id.ToString(), entity.Login);
                Redis.Set(entity.Login, json);

                return new HttpResult(json, $"{MimeTypes.Json}; charset=utf-8");
            }
            catch
            {
                return new HttpError(HttpStatusCode.InternalServerError, "Internal Server Error");
            }
        }
    }
}