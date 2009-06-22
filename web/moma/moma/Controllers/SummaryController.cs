using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Configuration;
using Moma.DB;
using Moma.Web.Models;
using Moma.Web.Helpers;

namespace Moma.Web
{
	[HandleError]
    public class SummaryController : Controller
    {
	SummaryData db = new SummaryData (ConfigurationManager.ConnectionStrings ["Moma"].ConnectionString);

	static string GetTitleFromType (string type)
	{
		switch (type) {
		case "missing":
			return "Missing APIs";
		case "todo":
			return "TODO APIs";
		case "niex":
			return "Not Implemented APIs";
		case "pinvoke":
			return "P/Invoke APIs";
		default:
			return null;
		}
	}

	SummaryViewData GetModel (string report_name, string type, int pageno)
	{
		string title = GetTitleFromType (type);
		if (title == null)
			return null;

		int total;
		SummaryViewData model = new SummaryViewData();
		model.ApiLinkColumn = 2;
		model.Title = title;
		model.Columns = new int [] { 0, 1, 2 };
		MomaDataSet ds = null;
		string val = report_name + "-" + type;
		switch (val) {
		case "byapp-missing":
			total = db.GetCountMissingMembers ();
			ds = db.GetMissingByApplication (pageno);
			model.Pager = new Pager (ds.Report.Rows, pageno, db.PageSize, total);
			model.Data = ds;
			break;
		case "byuse-missing":
			total = db.GetCountMissingMembers ();
			ds = db.GetMissingByUse (pageno);
			model.Columns = new int [] { 1, 0, 2 };
			model.Pager = new Pager (ds.Report.Rows, pageno, db.PageSize, total);
			model.Data = ds;
			break;
		case "byapp-todo":
			total = db.GetCountTODOMembers ();
			ds = db.GetTODOByApplication (pageno);
			model.Pager = new Pager (ds.Report.Rows, pageno, db.PageSize, total);
			model.Data = ds;
			break;
		case "byuse-todo":
			total = db.GetCountTODOMembers ();
			ds = db.GetTODOByUse (pageno);
			model.Columns = new int [] { 1, 0, 2 };
			model.Pager = new Pager (ds.Report.Rows, pageno, db.PageSize, total);
			model.Data = ds;
			break;
		case "byapp-niex":
			total = db.GetCountNIEXMembers ();
			ds = db.GetNIEXByApplication (pageno);
			model.Pager = new Pager (ds.Report.Rows, pageno, db.PageSize, total);
			model.Data = ds;
			break;
		case "byuse-niex":
			total = db.GetCountNIEXMembers ();
			ds = db.GetNIEXByUse (pageno);
			model.Columns = new int [] { 1, 0, 2 };
			model.Pager = new Pager (ds.Report.Rows, pageno, db.PageSize, total);
			model.Data = ds;
			break;
		case "byapp-pinvoke":
			total = db.GetCountApplicationsWithPInvoke ();
			ds = db.GetPInvokeByApplication (pageno);
			model.Pager = new Pager (ds.Report.Rows, pageno, db.PageSize, total);
			model.Data = ds;
			model.ApiLinkColumn = -1;
			break;
		case "byuse-pinvoke":
			total = db.GetCountApplicationsWithPInvoke ();
			ds = db.GetPInvokeByUse (pageno);
			model.Columns = new int [] { 1, 0, 2 };
			model.Pager = new Pager (ds.Report.Rows, pageno, db.PageSize, total);
			model.Data = ds;
			model.ApiLinkColumn = -1;
			break;
		default:
			return null;
		}
		if (model.Pager.PageIndex > model.Pager.TotalPages)
			return null;
		return model;
	}

	public ActionResult ByUse (string type, int? page)
	{
		if (page == null || (int) page < 1)
			return View ("Error");

		SummaryViewData model = GetModel ("byuse", type, (int) page);
		if (model == null)
			return View ("NoData");
		return View ("Browse", model);
	}

        //
        // GET: /Summary/ByApp-{type}/{page}

	public ActionResult ByApp (string type, int? page)
	{
		if (page == null || (int) page < 1)
			return View ("Error");

		SummaryViewData model = GetModel ("byapp", type, (int) page);
		if (model == null)
			return View ("NoData");
		return View ("Browse", model);
	}

	public ActionResult Index ()
	{
		return View ();
	}
    }
}
