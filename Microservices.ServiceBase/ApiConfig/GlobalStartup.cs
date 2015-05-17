using INGA.Framework.EnterpriseLibraries.Authentication.OAuth;
using Microservices.ServiceBase.Handlers;
using Microsoft.Owin;
using Microsoft.Owin.Security.DataHandler;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microservices.ServiceBase.ApiConfig
{
    public static class GlobalStartup
    {

        private static OAuthBearerAuthenticationOptions _OAuthBearerOptions = new OAuthBearerAuthenticationOptions()
        {
            Provider = new SimpleOAuthBearerProvider(),
        };

        private static OAuthAuthorizationServerOptions _OAuthServerOptions = new OAuthAuthorizationServerOptions()
        {
            AuthenticationType = "Bearer",
            AuthenticationMode = Microsoft.Owin.Security.AuthenticationMode.Passive,
            AllowInsecureHttp = true,
            TokenEndpointPath = new PathString("/api/token"),
            AccessTokenExpireTimeSpan = TimeSpan.FromDays(1),
            Provider = new SimpleAuthorizationServerProvider(),
            RefreshTokenProvider = new SimpleRefreshTokenProvider(),
            AuthorizeEndpointPath = new PathString("/api/Account/Auth")
        };

        public static OAuthBearerAuthenticationOptions OAuthBearerOptions
        {
            get
            {
                if (_OAuthBearerOptions == null)
                {
                    _OAuthBearerOptions = new OAuthBearerAuthenticationOptions();
                }
                return _OAuthBearerOptions;
            }
            private set { }
        }

        public static OAuthAuthorizationServerOptions OAuthServerOptions
        {
            get
            {
                if (_OAuthServerOptions == null)
                {
                    _OAuthServerOptions = new OAuthAuthorizationServerOptions();
                }
                return _OAuthServerOptions;
            }
            private set { }
        }

        public static HttpConfiguration Configuration(IAppBuilder app)
        {
            HttpConfiguration config = new HttpConfiguration();
            ConfigureOAuth(app);
            app.SetDataProtectionProvider(new SimpleDataProtectionProvider());
            config.MessageHandlers.Add(new AuthorizeMessageHandler(SimpleCloudConfigurationManagerSettingsPrincipalProvider.GetPrincipal));
            ;
            return config;
        }

        public static void ConfigureOAuth(IAppBuilder app)
        {
            app.UseOAuthAuthorizationServer(OAuthServerOptions);
            app.UseOAuthBearerAuthentication(OAuthBearerOptions);
        }

    }
}
