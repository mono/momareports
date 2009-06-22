using System;
using System.Data;
using Moma.Web.Helpers;

namespace Moma.Web.Models {
	public class ApiViewData {
		public string Title { get; set; }
		public string Name { get; set; }
		public string Status { get; set; }
		public string Comment { get; set; }
		public MomaDataSet Data;
	}
}