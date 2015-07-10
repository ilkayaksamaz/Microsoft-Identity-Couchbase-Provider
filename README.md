# Microsoft-Identity-Couchbase-Provider

ASP.NET Identity provider that users Couchbase for storage


## USAGE

```
[Authorize]
public void SomeMethod()
```


## CONFIGURATION FILE
```
 private static OAuthBearerAuthenticationOptions _OAuthBearerOptions = new OAuthBearerAuthenticationOptions()
        {
            Provider = new SimpleOAuthBearerProvider(),
        };

        private static OAuthAuthorizationServerOptions _OAuthServerOptions = new OAuthAuthorizationServerOptions()
        {
            AuthenticationType = "Bearer",
            AuthenticationMode = Microsoft.Owin.Security.AuthenticationMode.Passive,
            AllowInsecureHttp = true,
            TokenEndpointPath = new PathString("/api/v1/auth/token"),
            AccessTokenExpireTimeSpan = TimeSpan.FromDays(1),
            Provider = new SimpleAuthorizationServerProvider(),
            RefreshTokenProvider = new SimpleRefreshTokenProvider()
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
            config.Formatters.Clear();
            config.Formatters.Add(new JsonMediaTypeFormatter());

            ConfigureOAuth(app);
            app.SetDataProtectionProvider(new SimpleDataProtectionProvider());
            config.MessageHandlers.Add((DelegatingHandler)new AuthorizeMessageHandler(SimpleCloudConfigurationManagerSettingsPrincipalProvider.GetPrincipal));
            return config;
        }

        public static void ConfigureOAuth(IAppBuilder app)
        {
            app.CreatePerOwinContext<ApplicationUserManager<UserModel>>(new ApplicationUserManager<UserModel>(new UserStore<UserModel>()).Create);

            app.UseOAuthAuthorizationServer(OAuthServerOptions);
            app.UseOAuthBearerAuthentication(OAuthBearerOptions);
        }
```

