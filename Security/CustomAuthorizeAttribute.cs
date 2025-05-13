using System.Security.Claims;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SMARTV3.Security
{
    public class CustomAuthorizeAttribute : ActionFilterAttribute
    {
        public string? Roles { get; set; }
        private const string AccessDeniedPagePath = "~/Home/AccessDenied/";

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var user = filterContext.HttpContext.User;
            if (!IsUserLoggedIn(user))
            {
                filterContext.Result = new RedirectResult(AccessDeniedPagePath);
            }

            List<string> roles = Roles!.Split(",").ToList();
            // Check user rights here
            bool userIsInRoles = roles.Any(role => user.IsInRole(role.Trim()));

            if (!userIsInRoles)
            {
                filterContext.HttpContext.Response.StatusCode = 403;
                filterContext.Result = new RedirectResult(AccessDeniedPagePath);
            }
        }

        private static bool IsUserLoggedIn(ClaimsPrincipal user)
        {
            if (user.Identity != null && user.Identity.IsAuthenticated)
            {
                return true;
            }
            return false;
        }
    }
}