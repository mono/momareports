using System;
using System.IO;
using System.Web;
using System.Text.RegularExpressions;
using Moma.Web.Models;

namespace Moma.Web.Helpers {
	public static class Util {
		static Regex valid_rx = new Regex ("[^A-Za-z0-9]");
		public static bool IsValidReportGuid (string guid)
		{
			if (String.IsNullOrEmpty (guid))
				return false;

			// Check length?
			return (!valid_rx.IsMatch (guid.ToLowerInvariant ()));
		}

		public static string GetStatus (MomaDataSet.MembersRow row)
		{
			if (row.IsFixed)
				return "DONE";
			if (row.IsTodo && row.IsNiex)
				return "TODO and Not Implemented";
			if (row.IsTodo)
				return "TODO";
			if (row.IsNiex)
				return "Not Implemented";
			if (row.IsMissing)
				return "Missing";
			throw new Exception ("This should never happen");
		}

		public static string GetUrlFromApiName (string api)
		{
			return api.Replace (':', ';').Replace ("/", "%2f");
		}

		public static string GetApiNameFromUrl (string url)
		{
			return url.Replace(';', ':');
		}

		static string [] known_dlls = {
			"advapi32", "avifil32", "aygshell", "cards", "cfgmgr32", "comctl32", "comdlg32",
			"coredll", "credui", "crypt32", "dbghlp32", "Delegates", "dhcpsapi", "difxapi",
			"dmcl40", "dnsapi", "dwmapi", "faultrep", "gdi32", "gdiplus", "glossary",
			"glu32", "glut32", "gsapi", "hhctrl", "hid", "hlink", "httpapi", "icmp", "imm32",
			"ipaqutil", "iphlpapi", "iprop", "irprops", "kernel32", "mapi32", "misc", "mpr",
			"mqrt", "mscorsn", "msdrm", "msi", "msvcrt", "netapi32", "ntdll", "ntdsapi",
			"odbc32", "odbccp32", "ole32", "oleacc", "oleaut32", "opengl32", "powrprof",
			"psapi", "pstorec", "query", "quickusb", "rapi", "rasapi32", "rpcrt4", "secur32",
			"setupapi", "shell32", "shlwapi", "Structures", "twain_32", "unicows", "urlmon",
			"user32", "userenv", "uxtheme", "winfax", "winhttp", "wininet", "winmm",
			"winscard", "winspool", "wintrust", "wlanapi", "ws2_32", "wtsapi32", "xolehlp" };
						      
		public static string GetHtmlForPInvokeFunction (string library, string function)
		{
			if (String.IsNullOrEmpty (function))
				return String.Empty;
			if (String.IsNullOrEmpty (library))
				return HttpUtility.HtmlEncode (function);

			try {
				library = library.ToLowerInvariant ();
				library = Path.GetFileNameWithoutExtension (library);
				if (Array.BinarySearch (known_dlls, library, StringComparer.InvariantCulture) >= 0) {
					int space = function.IndexOf (' ');
					int paren = function.IndexOf ('(');
					string entry_point = function.Substring (space + 1, paren - space - 1);
					entry_point = entry_point.Trim ();
					return String.Format ("<a href=\"http://www.pinvoke.net/default.aspx/{0}.{1}\">{2}</a>",
									HttpUtility.HtmlAttributeEncode (library),
									HttpUtility.HtmlAttributeEncode (entry_point),
									HttpUtility.HtmlAttributeEncode (function));
				}
			} catch {
				// Ignore and return default
			}
			return HttpUtility.HtmlEncode (function);
		}
	}
}