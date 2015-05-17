using INGA.Framework.DataAccessObjects.Membership;
using INGA.Framework.EnterpriseLibraries.Authentication.Identity;
using Microservices.ServiceBase.Models;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;


namespace Microservices.ServiceBase.Handlers
{
    public class AuthorizeMessageHandler : DelegatingHandler
    {
        public const string QueryKey = "token";
        public const string CookieName = "token";
        public const string HeaderName = "Authorization";

        public delegate IPrincipal PrincipalFromToken(string token);

        private readonly PrincipalFromToken PrincipalFromTokenCallback;

        public AuthorizeMessageHandler(PrincipalFromToken callback)
        {
            PrincipalFromTokenCallback = callback;
        }

        public string GetToken(HttpRequestMessage request)
        {
            string token = "";

            if (string.IsNullOrEmpty(token))
            {
                token = HttpUtility.ParseQueryString(request.RequestUri.Query)[QueryKey];
            }
            if (string.IsNullOrEmpty(token))
            {
                token = request.Headers.Contains(HeaderName) ? request.Headers.GetValues(HeaderName).FirstOrDefault() : null;
            }
            if (string.IsNullOrEmpty(token))
            {
                var cookie = request.Headers.GetCookies(CookieName).FirstOrDefault();
                if (cookie != null)
                {
                    token = cookie["token"].Value;
                }
            }

            return string.Format("{0}", token).Replace("Bearer ", "");

        }

        protected override System.Threading.Tasks.Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            var token = GetToken(request);

            var principal = PrincipalFromTokenCallback(token);
            if (principal == null)
            {
                return base.SendAsync(request, cancellationToken);
            }

            var context = request.Properties["MS_OwinContext"] as OwinContext;
            if (context != null)
            {
                context.Request.User = principal;
            }

            return base.SendAsync(request, cancellationToken);
        }

    }

}
