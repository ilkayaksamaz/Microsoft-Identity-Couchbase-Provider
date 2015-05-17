using System.Web;
using ServiceStack;
using INGA.Framework.DataAccessObjects.Membership;
using INGA.Framework.EnterpriseLibraries.Authentication.Identity;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System.Web.Http;


namespace Microservices.ServiceBase
{
    public class IngaBaseService : ApiController
    {
        private UserManager<UserModel> _manager = new UserManager<UserModel>(new UserStore<UserModel>());

        public virtual UserManager<UserModel> UserManager
        {
            get { return _manager ?? (_manager = new UserManager<UserModel>(new UserStore<UserModel>())); }
        }


        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                var request = HttpContext.Current.Request as System.Web.HttpRequest;
                return request.GetOwinContext().Authentication;
            }
        }

        public void SignInUser(UserModel user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalBearer);
            var identity = UserManager.CreateIdentity(user, DefaultAuthenticationTypes.ExternalBearer);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
        }

    }
}
