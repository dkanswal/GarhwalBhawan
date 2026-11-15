using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace GarhwalBhawan.Controllers
{
    public class LogInFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (HttpContext.Current == null || HttpContext.Current.Session == null || HttpContext.Current.Session["UserName"] == null)
            {
                FormsAuthentication.SignOut();
                //send the user to login page with return URL
                filterContext.Result =
                    new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary { { "controller", "Admin" }, { "action", "LogIn" }, { "returnUrl", filterContext.HttpContext.Request.RawUrl } });
                return;
            }
        }
    }
}