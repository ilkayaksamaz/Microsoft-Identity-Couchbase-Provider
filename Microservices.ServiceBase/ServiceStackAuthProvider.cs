using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using INGA.Framework.DataAccessObjects.Membership;
using INGA.Framework.EnterpriseLibraries.Authentication.Identity;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using ServiceStack;
using ServiceStack.Auth;
using ServiceStack.Web;
using ServiceStack.Configuration;
using Microsoft.Owin.Security.Infrastructure;

namespace Microservices.ServiceBase
{
    public class CustomCredentialsAuthProvider : IAuthProvider
    {


        private UserManager<UserModel> _manager = new UserManager<UserModel>(new UserStore<UserModel>());

        public UserManager<UserModel> UserManager
        {
            get { return _manager ?? (_manager = new UserManager<UserModel>(new UserStore<UserModel>())); }
        }



        //public CustomCredentialsAuthProvider(IAppSettings appSettings, string authRealm, string oAuthProvider)
        //{
        //    // Enhancement per https://github.com/ServiceStack/ServiceStack/issues/741
        //    this.AuthRealm = appSettings != null ? appSettings.Get("OAuthRealm", authRealm) : authRealm;

        //    this.Provider = oAuthProvider;
        //    if (appSettings != null)
        //    {
        //        this.CallbackUrl = appSettings.GetString("oauth.{0}.CallbackUrl".Fmt(oAuthProvider))
        //            ?? "";

        //    }

        //}
        public string Provider { get; set; }
        public string AuthRealm { get; set; }
        public string CallbackUrl { get; set; }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                var request = HttpContext.Current.Request as System.Web.HttpRequest;
                return request.GetOwinContext().Authentication;
            }
        }

        public object Authenticate(IServiceBase authService, IAuthSession session, Authenticate request)
        {


            var userModel = UserManager.FindByName(request.UserName);

            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalBearer);
            var identity = UserManager.CreateIdentity(userModel, DefaultAuthenticationTypes.ExternalBearer);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = true }, identity);

            return new AuthenticateResponse() { DisplayName = userModel.UserName, UserId = userModel.Id };
        }


        public bool IsAuthorized(IAuthSession session, IAuthTokens tokens, Authenticate request = null)
        {
            var owinContext = HttpContext.Current.GetOwinContext();
            var authenticated = owinContext.Authentication.User.Identity.IsAuthenticated;
            return authenticated;
        }

        public object Logout(IServiceBase service, Authenticate request)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalBearer);

            return new AuthenticateResponse();
        }


    }

}
