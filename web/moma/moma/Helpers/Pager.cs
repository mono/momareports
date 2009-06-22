using System;
using System.Collections;

namespace Moma.Web.Helpers {
	public class Pager {
		public int PageIndex { get; private set; }
		public int PageSize { get; private set; }
		public int TotalCount { get; private set; }
		public int TotalPages { get; private set; }
		public int CurrentFirst { get; private set; }
		public int CurrentLast { get; private set; }

		public Pager (ICollection source, int page, int page_size, int total_count)
		{
			PageIndex = page;
			PageSize = page_size;
			TotalCount = total_count;
			CurrentFirst = (page - 1) * page_size + 1;
			CurrentLast = Math.Min (TotalCount, CurrentFirst + page_size - 1);
			TotalPages = (int) Math.Ceiling (TotalCount / (double) page_size);
		}

		public bool HasPreviousPage {
			get { return (PageIndex > 1); }
		}

		public bool HasNextPage {
			get { return (PageIndex < TotalPages); }
		}
	}
}