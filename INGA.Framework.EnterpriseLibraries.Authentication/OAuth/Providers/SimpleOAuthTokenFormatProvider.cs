using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INGA.Framework.EnterpriseLibraries.Authentication.OAuth
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
