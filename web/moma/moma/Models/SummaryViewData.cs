using System;
using System.Data;
using Moma.Web.Helpers;

namespace Moma.Web.Models {
	public class SummaryViewData {
		public string Title { get; set; }
		public Pager Pager { get; set; }
		public MomaDataSet Data { get; set; }
		public int [] Columns { get; set; }
		public int ApiLinkColumn { get; set; }
	}
}