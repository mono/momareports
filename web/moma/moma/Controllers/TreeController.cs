using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using Moma.DB;
using Moma.Web.Helpers;
using Moma.Web.Models;

namespace Moma.Web.Controllers
{
    public class TreeController : Controller
    {
	TreeData db = new TreeData (ConfigurationManager.ConnectionStrings ["Moma"].ConnectionString);

	MomaNode GetRoot ()
	{
		MomaNode result = HttpRuntime.Cache ["full_nodes@@"] as MomaNode;
		if (result == null) {
			result = GetNodes (db.GetTree ().Members.Rows);
			HttpRuntime.Cache.Insert ("full_nodes@@", result, null, DateTime.Now.AddMinutes (5), Cache.NoSlidingExpiration);
		}
		return result;
	}
        //
        // GET: /Tree/

        public ActionResult Index(string root)
        {
		if (String.IsNullOrEmpty (root))
			return View (GetRoot ());

		MomaNode root_node = GetRoot ();
		List<object> nodes = GetChildren (root_node, root);
		return Json (nodes.ToArray ());
        }

	static List<object> GetChildren (MomaNode root, string node_name)
	{
		if (node_name == "source")
			return GetChildrenList (root.ChildNodes, "");

		string [] parts = node_name.Split ('-');
		MomaNode current = root;
		for (int i = 0; i < parts.Length; i++) {
			int idx = Int32.Parse (parts [i]);
			current = current.ChildNodes [idx];
		}
		return GetChildrenList (current.ChildNodes, node_name);
	}

	static List<object> GetChildrenList (List<MomaNode> nodes, string base_name)
	{
		if (String.IsNullOrEmpty (base_name))
			base_name = "";
		else
			base_name += "-";

		List<object> list = new List<object> ();
		int idx = -1;
		foreach (MomaNode node in nodes) {
			idx++;
			string t = null;
			if (!node.HasChildren) {
				string vpath = HttpRuntime.AppDomainAppVirtualPath;
				if (vpath == "/")
					vpath = "";
				t = String.Format ("<a href='{2}/apis/{0}'>{1}</a> ", Util.GetUrlFromApiName (node.FullName), node.Text, vpath);
			}
			t += String.Format (node.HtmlFormat, "<b>" + HttpUtility.HtmlEncode (node.Text) + "</b>", node.TotalMissing, node.TotalTodo, node.TotalNiex);
			list.Add (new { id = base_name + idx, text = t, hasChildren = node.HasChildren});
		}
		return list;
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
			if (type == "BrowserCapabilitiesFactory")
				continue;
			if (ns == "System.Web.UI.WebControls.WebParts")
				continue;
			if (ns == "System.Management" || ns == "System.Management.Instrumentation")
				continue;
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
    }
}
