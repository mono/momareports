using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using Moma.DB;
using Moma.Web.Models;
using Moma.Web.Helpers;

namespace Moma.Web.Controllers {
    public class ApisController : Controller
    {
	ApiData db = new ApiData (ConfigurationManager.ConnectionStrings ["Moma"].ConnectionString);

	ApiViewData GetModel (string apiname)
	{
		ApiViewData model = new ApiViewData ();
		model.Title = apiname;
		model.Data = db.GetApiData (apiname);
		MomaDataSet.MembersRow row = model.Data.Members[0];
		model.Name = row.Name;
		model.Status = Util.GetStatus (row);
		if (!row.IsTODOCommentNull ())
			model.Comment = row.TODOComment;
		return model;
	}
        //
        // GET: /API/

	public ActionResult Index(string apiname)
	{
		if (apiname == null)
			return View();

		apiname = Util.GetApiNameFromUrl (apiname);
		ApiViewData model = GetModel (apiname);
		if (model == null || model.Data.Members.Count != 1)
			return View ("NoData");
		return View ("Api", model);
        }
    }
}
