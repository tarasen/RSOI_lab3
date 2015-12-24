using Nancy;
using Nancy.Authentication.Forms;
using Nancy.Bootstrapper;
using Nancy.Cryptography;
using Nancy.TinyIoc;
using PictureAuction.SOA.Frontend.Authentication;

namespace PictureAuction.SOA.Frontend
{
    public class Bootstrapper : DefaultNancyBootstrapper
    {
        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);

            pipelines.AfterRequest += ctx =>
            {
                if (ctx.Response.ContentType == "text/html")
                    ctx.Response.ContentType = "text/html; charset=utf-8";
            };

            var cryptographyConfiguration = new CryptographyConfiguration(
                new RijndaelEncryptionProvider(new PassphraseKeyGenerator(Configuration.EncryptionKey,
                    new byte[] {8, 2, 10, 4, 68, 120, 7, 14})),
                new DefaultHmacProvider(new PassphraseKeyGenerator(Configuration.HmacKey,
                    new byte[] {1, 20, 73, 49, 25, 106, 78, 86})));

            var authenticationConfiguration =
                new FormsAuthenticationConfiguration
                {
                    CryptographyConfiguration = cryptographyConfiguration,
                    RedirectUrl = "/login",
                    UserMapper = container.Resolve<ISessionClient>()
                };

            FormsAuthentication.Enable(pipelines, authenticationConfiguration);
        }
    }
}