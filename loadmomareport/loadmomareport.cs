//
// loadmomareport.cs
//
// Authors:
//      Gonzalo Paniagua Javier (gonzalo@novell.com)
//
// Copyright (c) Copyright 2009 Novell, Inc
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.IO;
using System.Net;
using System.Xml;

namespace Mono.Moma {
	class Populate {
		static Dictionary<string, string> wpftypes = new Dictionary<string, string> ();
		
		static DataAccess GetDataAccess ()
		{
			return new MySqlDataAccess ();
			//return new PostgresDataAccess ();
		}
		
		static bool LoadWpfExclusions ()
		{
			bool loaded = false;
			try {
				using (StreamReader reader = new StreamReader ("wpf-exclusions.txt")) {
					string line;
					while ((line = reader.ReadLine ()) != null)
						wpftypes.Add (line, line);
				}
				loaded = true;
			} catch (Exception exc) {
				Console.Error.WriteLine ("Error loading WPF exclusions file: {0}", exc.Message);
			}
			return loaded;
		}

		static int Main (string [] args)
		{
			if (args.Length == 0)
				return 1;
			
			if (!LoadWpfExclusions ())
				return 1;

			bool xml = true;
			foreach (string arg in args) {
				if (arg == "--text") {
					xml = false;
					continue;
				}
				Console.WriteLine ("Loading {0}", arg);
				try {
					LoadReport (xml, arg);
				} catch (Exception e) {
					Console.Error.WriteLine ("{0} failed: {1}", arg, e.ToString ());
				}
			}
			return 0;
		}

		static void LoadReport (bool xml, string filename)
		{
			DateTime webparts_date = new DateTime (2008, 1, 1);
			Loader loader;
			if (xml)
				loader =  new XmlFileLoader (filename);
			else
				loader =  new TextFileLoader (filename);

			DataAccess da = GetDataAccess ();
			da.BeginTransaction ();
			try {
				bool have_wpf;
				using (loader) {
					have_wpf = false;
					int report_id = da.InsertReportMaster ( loader.ReportDate, loader.IPAddress.ToString (),
										loader.Definitions, loader.UserName,
										loader.Email, loader.Organization,
										loader.HomePage, loader.Comments, loader.Guid);
					foreach (Issue issue in loader.GetIssues ()) {
						if (issue.IssueType == IssueType.PInvoke) {
							PInvokeIssue p = (PInvokeIssue) issue;
							da.InsertOrUpdatePInvoke (report_id, p.Library, p.Function);
						} else {
							MemberIssue m = (MemberIssue) issue;
							bool is_missing, is_todo, is_niex;
							GetAsBooleans (m.IssueType, out is_missing, out is_todo, out is_niex);
							// Ignore most of the WPF types
							if (!IsWpf (m.Name)) {
								// Ignore reports submitted prior to 2008-01-01 that are using WebParts.
								if (loader.ReportDate < webparts_date && m.Name.IndexOf (" System.Web.UI.WebControls.WebParts") != -1) {
									Console.WriteLine ("Ignoring report {0} (uses WebParts and older than 2008-01-01)", loader.Guid);
									da.Rollback ();
									return;
								}
									
								da.InsertOrUpdateMember (report_id, m.Name, is_missing,is_todo, is_niex, m.Comment);
							} else {
								have_wpf = true;
							}
						}
					}
					da.InitReportCounts (report_id);
					if (have_wpf)
						da.SetWpf (report_id);
				}
				
				if (da.InTransaction) // Should be true...
					da.Commit ();
			} catch (Exception) {
				if (da.InTransaction)
					da.Rollback ();
				throw;
				//Console.WriteLine (exc);
				//Environment.Exit (1);
			}
		}
		
		static bool IsWpf (string member)
		{
			int colon = member.IndexOf ("::");
		    if (colon == -1)
			    return false;

			int space = member.IndexOf (' ');
			if (space == -1)
				return false;
			string fullname = member.Substring (space + 1, colon - space - 1);
			return wpftypes.ContainsKey (fullname);
		}

		static void GetAsBooleans (IssueType it, out bool is_missing, out bool is_todo, out bool is_niex)
		{
			is_missing = (it == IssueType.Missing);
			is_todo = (it == IssueType.Todo);
			is_niex = (it == IssueType.Niex);
		}
	}

	enum IssueType {
		Todo,
		Missing,
		Niex,
		PInvoke
	}

	class Issue {
		IssueType type;

		protected Issue (IssueType type)
		{
			this.type = type;
		}

		public IssueType IssueType {
			get { return type; }
		}
	}

	class MemberIssue : Issue {
		string name;
		string comment;

		public MemberIssue (IssueType type, string method_name) : this (type, method_name, null)
		{
		}

		public MemberIssue (IssueType type, string method_name, string todo_comment) : base (type)
		{
			this.name = method_name;
			this.comment = todo_comment;
		}

		public string Name {
			get { return name; }
		}

		public string Comment {
			get { return comment; }
		}
	}

	class PInvokeIssue : Issue {
		string library;
		string function;

		public PInvokeIssue (string library, string function) : base (IssueType.PInvoke)
		{
			this.library = library;
			this.function = function;
		}

		public string Library {
			get { return library; }
		}

		public string Function {
			get { return function; }
		}
	}

	class Loader : IDisposable {
		protected string filename;
		protected DateTime date;
		protected IPAddress address;
		protected string definitions;
		protected string moma_version;
		protected string user_name;
		protected string email;
		protected string organization;
		protected string homepage;
		protected string comments;
		protected string app_type;
		protected string guid;

		protected Loader (string filename)
		{
			this.filename = filename;	
			this.guid = Path.GetFileNameWithoutExtension (filename);
			LoadMetadata ();
		}

		public DateTime ReportDate {
			get { return date; }
		}

		public IPAddress IPAddress {
			get { return address; }
		}

		public string Definitions {
			get { return definitions; }
		}

		public string MomaVersion {
			get { return moma_version; }
		}

		public string UserName {
			get { return user_name; }
		}

		public string Organization {
			get { return organization; }
		}

		public string HomePage {
			get { return homepage; }
		}

		public string Comments {
			get { return comments; }
		}

		public string ApplicationType {
			get { return app_type; }
		}

		public string Email {
			get { return email; }
		}

		public string Guid {
			get { return guid; }
		}

		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		protected virtual void LoadMetadata ()
		{
		}
		
		public virtual IEnumerable<Issue> GetIssues ()
		{
			yield break;
		}

		protected virtual void Dispose (bool disposing)
		{
		}
	}

	class TextFileLoader : Loader {
		static char [] spl = new char [] { '-' };
		StreamReader reader;
		int lineno;

		public TextFileLoader (string filename) : base (filename)
		{
		}

		public override IEnumerable<Issue> GetIssues ()
		{
			string line;
			while ((line = reader.ReadLine ()) != null) {
				lineno++;
				if (line == "" || line.Length <= 7)
					continue;

				IssueType issue_type;
				string type = line.Substring (0, 6);
				switch (type) {
				case "[MISS]":
					issue_type = IssueType.Missing;
					break;
				case "[NIEX]":
					issue_type = IssueType.Niex;
					break;
				case "[PINV]":
					issue_type = IssueType.PInvoke;
					break;
				case "[TODO]":
					issue_type = IssueType.Todo;
					break;
				default:
					Console.Error.WriteLine ("Ignoring line {0} in {1}: {2}", lineno, filename, line);
					continue;
				}
				line = line.Substring (7).Trim ();
				string str1 = line;
				string str2 = null;
				if (issue_type == IssueType.Todo || issue_type == IssueType.PInvoke) {
					string [] parts = line.Split (spl, 2);
					str1 = parts [0].Trim ();
					str2 = null;
					if (parts.Length == 2)
						str2 = parts [1].Trim ();
				}

				if (issue_type == IssueType.PInvoke) {
					if (str1 != null)
						str1 = str1.ToLowerInvariant ();
					else
						str1 = "";
					if (str2 != null)
						str2 = str2.ToLowerInvariant ();
					else
						str2 = "";
					yield return new PInvokeIssue (str2, str1);
				} else {
					yield return new MemberIssue (issue_type, str1, str2);
				}
			}
		}

		protected override void Dispose (bool disposing)
		{
			if (disposing) {
				if (reader != null)
					reader.Close ();
			}
			reader = null;
			base.Dispose (disposing);
		}

		protected override void LoadMetadata ()
		{
			reader = new StreamReader (filename);
			lineno = 1;
			date = DateTime.Parse (reader.ReadLine ());
			lineno++;
			address = IPAddress.Parse (reader.ReadLine ());
			lineno++;
			definitions = GetValue (reader.ReadLine ());
			lineno++;
			user_name = GetValue (reader.ReadLine ());
			lineno++;
			email = GetValue (reader.ReadLine ());
			lineno++;
			organization = GetValue (reader.ReadLine ());
			lineno++;
			homepage = GetValue (reader.ReadLine ());
			lineno++;
			comments = GetValue (reader.ReadLine ());
			lineno++;
			app_type = null;
			moma_version = null;
		}

		static string GetValue (string line)
		{
			if (String.IsNullOrEmpty (line))
				return null;

			if (line [0] != '@')
				return null;

			int idx = line.IndexOf (':');
			if (idx == -1 || line.Length - 2 > idx)
				return null;

			return line.Substring (idx + 2);
		}
	}

	class XmlFileLoader : Loader {
		XmlDocument doc;

		public XmlFileLoader (string filename) : base (filename)
		{
		}

		public override IEnumerable<Issue> GetIssues ()
		{
			XmlNodeList issues = doc.SelectNodes ("/report/assemblies/assembly/issue");
			foreach (XmlNode node in issues) {
				string type_str = node.Attributes ["type"].Value;
				IssueType issue_type = GetIssueTypeFromString (type_str);
				Issue issue;
				if (issue_type == IssueType.PInvoke) {
					XmlNode function = node.SelectSingleNode ("method");
					XmlNode library = node.SelectSingleNode ("data");
					string lib = library.InnerText;
					if (!String.IsNullOrEmpty (lib))
						lib = lib.ToLowerInvariant ();
					string func = function.InnerText;
					if (!String.IsNullOrEmpty (func))
						func = func.ToLowerInvariant ();

					issue = new PInvokeIssue (lib, func);
				} else  {
					XmlNode name = node.SelectSingleNode ("raw");
					issue = new MemberIssue (issue_type, name.InnerText, null);
				}

				yield return issue;
			}
		}

		static IssueType GetIssueTypeFromString (string str)
		{
			switch (str) {
			case "pinv": return IssueType.PInvoke;
			case "todo": return IssueType.Todo;
			case "niex": return IssueType.Niex;
			case "miss": return IssueType.Missing;
			default:
				Console.WriteLine ("Unknown type: {0}", str);
				return IssueType.Missing;
			}
		}

		protected override void LoadMetadata ()
		{
			doc = new XmlDocument ();
			using (StreamReader reader = new StreamReader (filename)) {
				date = DateTime.Parse (reader.ReadLine ());
				address = IPAddress.Parse (reader.ReadLine ());
				doc.Load (reader);
			}
			definitions = GetNodeValue (doc, "/report/metadata/definitions");
			moma_version = GetNodeValue (doc, "/report/metadata/momaversion");
			user_name = GetNodeValue (doc, "/report/metadata/name");
			email = GetNodeValue (doc, "/report/metadata/email");
			organization = GetNodeValue (doc, "/report/metadata/organization");
			homepage = GetNodeValue (doc, "/report/metadata/homepage");
			comments = GetNodeValue (doc, "/report/metadata/comments");
			app_type = GetNodeValue (doc, "/report/metadata/apptype");
		}

		string GetNodeValue (XmlNode n, string path)
		{
			XmlNode node = n.SelectSingleNode (path);
			if (node == null) {
				Console.WriteLine ("{0} not found", path);
				return null;
			}

			return node.InnerText;
		}
	}
}

