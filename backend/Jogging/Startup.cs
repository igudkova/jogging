using Jogging.Authentication;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System.Configuration;
using System.Linq;
using System.Web.Http;

[assembly: OwinStartup(typeof(Jogging.Startup))]

namespace Jogging
{
    public class Startup
    {
        public static OAuthBearerAuthenticationOptions OAuthBearerAuthenticationOptions { get; private set; }
        public static GoogleOAuth2AuthenticationOptions GoogleOAuth2AuthenticationOptions { get; private set; }

        public void Configuration(IAppBuilder app)
        {
            ConfigureOAuth(app);

            app.UseCors(CorsOptions.AllowAll);

            ConfigureRoutes(app);
        }

        private void ConfigureOAuth(IAppBuilder app)
        {
            // use a cookie to temporarily store information about a user 
            // logging in with a third party login provider
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            OAuthBearerAuthenticationOptions = new OAuthBearerAuthenticationOptions();
            app.UseOAuthBearerAuthentication(OAuthBearerAuthenticationOptions);

            var googleAuthentication = ConfigurationManager.AppSettings["GoogleAuthentication"];
            var googleAppCredentials = googleAuthentication.Split(';')
                                        .Select(x => x.Split('='))
                                        .ToDictionary(x => x[0].Trim(), x => x[1].Trim());
            GoogleOAuth2AuthenticationOptions = new GoogleOAuth2AuthenticationOptions()
            {
                ClientId = googleAppCredentials["ClientId"],
                ClientSecret = googleAppCredentials["ClientSecret"],
                Provider = new GoogleAuthProvider()
            };
            app.UseGoogleAuthentication(GoogleOAuth2AuthenticationOptions);
        }

        private void ConfigureRoutes(IAppBuilder app)
        {
            HttpConfiguration config = new HttpConfiguration();
            config.MapHttpAttributeRoutes();

            app.UseWebApi(config);
        }
    }
}