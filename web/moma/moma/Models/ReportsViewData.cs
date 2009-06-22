using System;
using System.Data;
using Moma.Web.Helpers;

namespace Moma.Web.Models {
	public class ReportsViewData {
		public string Title { get; set; }
		public Pager Pager { get; set; }
		public MomaDataSet Data { get; set; }
	}
}