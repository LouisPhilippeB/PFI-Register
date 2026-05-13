using Models;
using System.Web.Mvc;

namespace Controllers
{
    public class AccessControl
    {
        public class UserAccess : AuthorizeAttribute
        {
            private Access RequiredAccess { get; set; }

            public UserAccess(Access access = Access.Anonymous) : base()
            {
                RequiredAccess = access;
            }

            protected override bool AuthorizeCore(System.Web.HttpContextBase httpContext)
            {
                if (RequiredAccess == Access.Anonymous)
                    return true;

                User connectedUser = User.ConnectedUser;

                if (connectedUser == null)
                    return false;

                if (connectedUser.Blocked)
                    return false;

                return connectedUser.Access >= RequiredAccess;
            }

            protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
            {
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    filterContext.HttpContext.Response.StatusCode = 401;
                    filterContext.Result = new EmptyResult();
                }
                else
                {
                    filterContext.Result = new RedirectResult("/Accounts/Login?message=Accès non autorisé!&success=false");
                }
            }
        }
    }
}