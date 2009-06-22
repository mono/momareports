using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Web.UI.WebControls;
using Moma.DB;
using Moma.Web.Helpers;
using Moma.Web.Models;

namespace Moma.Web.Controllers
{
    public class ReportsController : Controller
    {
	ReportsData db = new ReportsData (ConfigurationManager.ConnectionStrings ["Moma"].ConnectionString);
        //
        // GET: /Applications/

        public ActionResult Index(int? page)
        {
		if (page == null)
			return View ("NoData");
		MomaDataSet ds = db.GetPagedReports ((int) page);
		if (ds.Applications.Count == 0)
			return View ("NoData");
		ReportsViewData model = new ReportsViewData ();
		int total = db.GetCountTotal ();
		model.Title = "Report List";
		model.Data = ds;
		model.Pager = new Pager (ds.Applications.Rows, (int) page, db.PageSize, total);
		return View (model);
        }

	public ActionResult SingleReport (string guid)
	{
		if (!Util.IsValidReportGuid (guid))
			return View ("NoData");

		MomaDataSet ds = db.GetReport (guid);
		if (ds.Applications.Count == 0)
			return View ("NoData");

		SingleReportViewData model = new SingleReportViewData ();
		model.App = ds.Applications [0];
		model.PInvokes = ds.PInvokes;
		model.TotalAPIs = ds.Members.Count;
		model.RootNode = GetNodes (ds.Members.Rows);
		foreach (MomaDataSet.MembersRow row in ds.Members)
			model.TotalUses += row.Count;
		model.TotalPInvokes = model.PInvokes.Count;
		foreach (MomaDataSet.PInvokesRow row in ds.PInvokes)
			model.TotalPInvokeUses += row.Count;
		return View (model);
	}

	    static MomaNode GetNodes (DataRowCollection rows)
	    {
		    MomaNode root = new MomaNode ("");
		    Dictionary<string, MomaNode> namespaces = new Dictionary<string, MomaNode> ();
		    Dictionary<string, MomaNode> types = new Dictionary<string, MomaNode> ();
		    foreach (MomaDataSet.MembersRow row in rows) {
			    string ns;
			    string type;
			    string name;
			    SplitMember (row.Name, out ns, out type, out name);
			    MomaNode nsnode;
			    if (!namespaces.TryGetValue (ns, out nsnode)) {
				    nsnode = new MomaNode (ns);
				    namespaces [ns] = nsnode;
				    root.ChildNodes.Add (nsnode);
			    }
			    MomaNode typenode;
			    if (!types.TryGetValue (ns + type, out typenode)) {
				    typenode = new MomaNode (type);
				    types [ns + type] = typenode;
				    nsnode.ChildNodes.Add (typenode);
			    }
			    MomaNode new_node = new MomaNode (name);
			    new_node.FullName = row.Name;
			    new_node.NumberOfUses = row.Count;
			    if (row.IsTodo)
				    new_node.Status |= NodeStatus.Todo;
			    if (row.IsNiex)
				    new_node.Status |= NodeStatus.Niex;
			    if (row.IsMissing)
				    new_node.Status |= NodeStatus.Missing;
			    typenode.ChildNodes.Add (new_node);
		    }
		    root.SortAndCount ();
		    return root;
	    }

	    static void SplitMember (string member, out string ns, out string type, out string name)
	    {
		    int colon = member.IndexOf ("::");
		    if (colon == -1) {
			    ns = "<Unknown namespace>";
			    type = "<Unknown type>";
			    name = "<Unknown name>";
			    return;
		    }

		    name = member.Substring (colon + 2);
		    string fullname = member.Substring (0, colon);
		    int tidx = fullname.LastIndexOf ('.');
		    if (tidx != -1) {
			    ns = fullname.Substring (0, tidx);
			    int space = ns.IndexOf (' ');
			    if (space != -1)
				    ns = ns.Substring (space + 1);
			    type = fullname.Substring (tidx + 1);
		    } else {
			    ns = fullname;
			    type = "<Unknown type>";
		    }
	    }
    }
}
