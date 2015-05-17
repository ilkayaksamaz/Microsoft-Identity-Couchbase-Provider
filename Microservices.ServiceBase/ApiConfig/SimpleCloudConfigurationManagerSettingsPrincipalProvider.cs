using Microsoft.Owin.Security.DataHandler;
using Microsoft.Owin.Security.DataProtection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Microservices.ServiceBase.ApiConfig
{
    public static class SimpleCloudConfigurationManagerSettingsPrincipalProvider
    {
        public static IPrincipal GetPrincipal(string token)
        {
            IPrincipal principal = null;
            if (principal == null)
            {

                if (!string.IsNullOrEmpty(token))
                {
                    var ticket = GlobalStartup.OAuthServerOptions.AccessTokenFormat.Unprotect(token);
                    if (ticket != null)
                        principal = new GenericPrincipal(ticket.Identity, new[] { "Client" });
                }

            }

            return principal;
        }
    }

}
