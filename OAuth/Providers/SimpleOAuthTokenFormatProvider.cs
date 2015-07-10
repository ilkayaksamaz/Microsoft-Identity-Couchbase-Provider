using System;
using Microsoft.Owin.Security;

namespace INGA.Framework.EnterpriseLibraries.Authentication.OAuth.Providers
{
    public class SimpleOAuthTokenFormatProvider : ISecureDataFormat<AuthenticationTicket> 
    {
        public string Protect(AuthenticationTicket data)
        {
            throw new NotImplementedException();
        }

        public AuthenticationTicket Unprotect(string protectedText)
        {
            throw new NotImplementedException();
        }
    }
}
