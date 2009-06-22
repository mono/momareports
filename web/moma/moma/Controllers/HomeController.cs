using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.Mvc;
using Moma.Web.Models;
using Moma.DB;

using System.Web.Routing;

namespace Moma.Web.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
	HomeData db = new HomeData (ConfigurationManager.ConnectionStrings ["Moma"].ConnectionString);

        public ActionResult Index()
        {
		MomaDataSet ds = db.GetHomeData ();
		return View (ds);
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
