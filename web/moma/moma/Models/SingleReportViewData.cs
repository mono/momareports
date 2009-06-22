using System;
using System.Data;
using Moma.Web.Helpers;

namespace Moma.Web.Models {
	public class SingleReportViewData {
		public MomaDataSet.ApplicationsRow App { get; set; }
		//public MomaDataSet.MembersDataTable Members { get; set; }
		public MomaDataSet.PInvokesDataTable PInvokes { get; set; }
		public MomaNode RootNode { get; set; }
		public int TotalAPIs { get; set; }
		public int TotalUses { get; set; }
		public int TotalPInvokes { get; set; }
		public int TotalPInvokeUses { get; set; }
	}
}