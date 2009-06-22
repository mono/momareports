using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Moma.Web.Helpers {
	[Flags]
	public enum NodeStatus {
		OK = 0,
		Missing = 1,
		Todo = 2,
		Niex = 4
	}

	public class MomaNode {
		public int TotalIssues;
		public int TotalMissing;
		public int TotalTodo;
		public int TotalNiex;
		public int NumberOfUses;

		public NodeStatus Status;
		public string HtmlFormat { get; private set; }
		public string Text { get; private set; }
		public string FullName { get; set; }

		List<MomaNode> children;
		public bool HasChildren { get { return children != null && children.Count > 0; } }
		public List<MomaNode> ChildNodes {
			get {
				if (children == null)
					children = new List<MomaNode> ();
				return children;
			}
		}

		public MomaNode (string text)
		{
			this.Text = text;
		}

		public void SortAndCount ()
		{
			SortAndCount (CompareNode);
		}

		static int CompareNode (MomaNode a, MomaNode b)
		{
			return String.CompareOrdinal (a.Text, b.Text);
		}

		void SortAndCount (Comparison<MomaNode> comparison)
		{
			if (HasChildren) {
				children.Sort (comparison);
				foreach (MomaNode child in children) {
					child.SortAndCount (comparison);
					TotalIssues += child.TotalIssues;
					TotalMissing += child.TotalMissing;
					TotalTodo += child.TotalTodo;
					TotalNiex += child.TotalNiex;
				}
			}

			StringBuilder sb = new StringBuilder();
			if ((Status & NodeStatus.Missing) == NodeStatus.Missing) {
				TotalMissing++;
				TotalIssues++;
			}
			if ((Status & NodeStatus.Niex) == NodeStatus.Niex) {
				TotalNiex++;
				TotalIssues++;
			}
			if ((Status & NodeStatus.Todo) == NodeStatus.Todo) {
				TotalTodo++;
				TotalIssues++;
			}
			StringBuilder fmt = new StringBuilder ();
			if (String.IsNullOrEmpty (Text))
				Text = "Total: ";

			fmt.AppendFormat  ("<span>{0}&nbsp;&nbsp;", HasChildren ? "{0}" : "");
			if (TotalMissing > 0)
				//fmt.Append ("<span class='report'><span class='icons suffix miss'/>{1}</span>");
				fmt.Append ("<img src='../../Content/images/sm.gif'/> {1} ");
			if (TotalTodo > 0)
				//fmt.Append ("<span class='report'><span class='icons suffix todo'/>{2}</span>");
				fmt.Append ("<img src='../../Content/images/st.gif'/> {2} ");
			if (TotalNiex > 0)
				//fmt.Append ("<span class='report'><span class='icons suffix warn'/>{3}</span>");
				fmt.Append ("<img src='../../Content/images/se.gif'/> {3} ");

			fmt.Append ("</span>");

			HtmlFormat = fmt.ToString ();
			//String.Format (fmt.ToString (), HttpUtility.HtmlEncode (Text), TotalMissing, TotalTodo, TotalNiex);
		}
	}
}
