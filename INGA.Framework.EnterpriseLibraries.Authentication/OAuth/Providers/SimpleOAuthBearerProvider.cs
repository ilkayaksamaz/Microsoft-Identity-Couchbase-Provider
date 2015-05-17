using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INGA.Framework.EnterpriseLibraries.Authentication.OAuth
{
    public class SimpleOAuthBearerProvider : OAuthBearerAuthenticationProvider
    {
        private string HeaderName = "Authorization";
        public override Task RequestToken(OAuthRequestTokenContext context)
        {
            string token = context.Request.Headers.ContainsKey(HeaderName) ? context.Request.Headers.GetValues(HeaderName).FirstOrDefault() : null;
            if (!string.IsNullOrEmpty(token))
            {
                context.Token = token;
            }
            return Task.FromResult<object>(null);
        }
    }
}
