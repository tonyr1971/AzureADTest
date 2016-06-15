using LocationSvcClient.Trusted;
using Microsoft.Owin;

using Microsoft.Owin.Security.ActiveDirectory;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;

using System.Configuration;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Owin.Security;
using Owin;

[assembly: OwinStartup(typeof(Startup))]
namespace LocationSvcClient.Trusted
{
    public class Startup
    {
        private static string clientId = ConfigurationManager.AppSettings["ida:clientId"];
        private static string appKey = ConfigurationManager.AppSettings["ida:appKey"];
        private static string aadInstance = ConfigurationManager.AppSettings["ida:aadInstance"];
        private static string tenant = ConfigurationManager.AppSettings["ida:tenant"];
        private static string postLogoutRedirectUri = ConfigurationManager.AppSettings["ida:postLogoutRedirectUri"];
        private static string authority = string.Format(CultureInfo.InvariantCulture, aadInstance, tenant);

        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }

        public void ConfigureAuth(IAppBuilder app)
        {
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);
            app.UseCookieAuthentication(new CookieAuthenticationOptions());

            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
            {
                ClientId = clientId,
                Authority = authority,
                PostLogoutRedirectUri = postLogoutRedirectUri,
                Notifications = new OpenIdConnectAuthenticationNotifications
                {
                    AuthenticationFailed = context =>
                                           {
                                               context.HandleResponse();
                                               context.Response.Redirect("/Home/Error");
                                               return Task.FromResult(0);
                                           }
                }
            });
        }
    }
}