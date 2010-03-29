using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sample.WebActors.Controllers
{
    public class TraceController : Controller
    {
        //
        // GET: /Trace/

        public ActionResult Index()
        {
        	string userAgent = this.ControllerContext.RequestContext.HttpContext.Request["HTTP_USER_AGENT"];

        	return Content("User Agent: " + userAgent);
        }

    }
}
