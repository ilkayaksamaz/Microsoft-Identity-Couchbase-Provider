using INGA.Framework.DataLayer.Entities.Interfaces;
using INGA.Framework.DataLayer.Entities.Membership;
using INGA.Framework.EnterpriseLibraries.Authentication.Helpers;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.DataProtection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INGA.Framework.EnterpriseLibraries.Authentication.Identity
{
    public class ApplicationUserManager<TUser> : UserManager<TUser> where TUser : class,IIdentityUser<string>
    {

        public ApplicationUserManager(IUserStore<TUser> userStore)
            : base(userStore)
        {

        }

        public ApplicationUserManager<TUser> Create()
        {
            var provider = new DpapiDataProtectionProvider("Inga");

            var _manager = new ApplicationUserManager<TUser>(new UserStore<TUser>());

            _manager.UserValidator = new CustomUserValidator<TUser>(_manager);

            // Configure validation logic for passwords
            _manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = false,
                RequireDigit = false,
                RequireLowercase = false,
                RequireUppercase = false,
            };

            _manager.RegisterTwoFactorProvider("EmailCode", new EmailTokenProvider<TUser>()
            {
                Subject = "SecurityCode",
                BodyFormat = "Your security code is {0}"
            });

            _manager.EmailService = new EmailService();

            _manager.UserTokenProvider = new DataProtectorTokenProvider<TUser, string>(
provider.Create("ASP.NET Identity"));

            return _manager;
        }
    }
}
