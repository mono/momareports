using System;
using System.Data;
using System.Drawing;
using Moma.Web.Helpers;

namespace Moma.Web.Models {
	public class ChartViewData {
		public string Title { get; set; }
		public string Colors { get; set; }
		public string NameColumn { get; set; }
		public string DataColumn { get; set; }
		public Size Size { get; set; }
		public DataRowCollection Rows { get; set; }
	}
}