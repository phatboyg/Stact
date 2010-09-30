using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sample.WebActors.Controllers
{
	using System.Text;
	using Stact.Web.ValueProviders;

	public class TraceController : Controller
    {
        //
        // GET: /Trace/

        public ActionResult Index()
        {
			StringBuilder output = new StringBuilder();

        	output.Append("<html><body><ul>");

        	var provider = new RequestContextValueProvider(this.ControllerContext.RequestContext);

			provider.GetAll((key, value) =>
				{
					output.AppendFormat("<li>{0}<br />{1}</li>", key, value);
				});

        	output.Append("</ul></body></html>");

        	return Content(output.ToString());
        }

    }
}
