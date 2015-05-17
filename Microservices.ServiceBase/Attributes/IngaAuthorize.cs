using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using Microsoft.Owin.Security;
using System.Net.Http;
using Microservices.ServiceBase.Models;
using System.Net;
using System.Net.Http.Formatting;


namespace Microservices.ServiceBase.Attributes
{
    public class IngaAuthorize : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {

            actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized)
            {
                Content = new ObjectContent<AuthorizeMessageOutput>(new AuthorizeMessageOutput
                            {
                                Status = false,
                                Message = "Sorry, you are not authorized"
                            }, new JsonMediaTypeFormatter(), "application/json")
            };

        }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            base.OnAuthorization(actionContext);
        }

        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            var identity = Thread.CurrentPrincipal.Identity;
            if (identity == null && HttpContext.Current != null)
                identity = HttpContext.Current.User.Identity;

            if (identity != null && identity.IsAuthenticated)
            {
                var request = HttpContext.Current.Request as System.Web.HttpRequest;
                if (request.IsAuthenticated)
                {
                    return true;
                }
            }

            return false;
        }
    }

}
