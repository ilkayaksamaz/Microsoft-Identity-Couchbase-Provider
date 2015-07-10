using System.Linq;
using System.Threading.Tasks;
using Microsoft.Owin.Security.OAuth;

namespace INGA.Framework.EnterpriseLibraries.Authentication.OAuth.Providers
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
