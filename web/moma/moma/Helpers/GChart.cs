using System;
using System.Data;
using System.Drawing;
using System.Text;
using System.Web;

namespace Moma.Web.Helpers {
	public static class GChart {
		public static string GetURLFromData (Size size, DataRowCollection rows, string name_row, string data_row)
		{
			if (rows == null || String.IsNullOrEmpty (name_row) || String.IsNullOrEmpty (data_row))
				return null;

			// http://chart.apis.google.com/chart?cht=p3&chd=t:60,40&chs=250x100&chl=Hello|World

			StringBuilder labels = new StringBuilder ();
			labels.Append ("chl=");
			StringBuilder data = new StringBuilder ();
			data.Append ("chd=t:");
			int min = Int32.MaxValue;
			int max = Int32.MinValue;
			foreach (DataRow row in rows) {
				labels.AppendFormat ("{0}|", HttpUtility.UrlEncode (row [name_row].ToString ()));
				int val = Convert.ToInt32 (row [data_row]);
				data.AppendFormat ("{0},", val);
				if (val < min)
					min = val;
				if (val > max)
					max = val;
			}

			if (rows.Count > 0) {
				labels.Length--;
				data.Length--;
			}
			return String.Format ("http://chart.apis.google.com/chart?cht=p3&chs={2}x{3}&{0}&{1}&chds=0,{5}",
							data.ToString (),
							labels.ToString (),
							size.Width, size.Height,
							min, max);
		}
	}
}