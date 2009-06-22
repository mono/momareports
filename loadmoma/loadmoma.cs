//
// loadmoma.cs
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
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace Mono.Moma {
	class Populate {
		static DataAccess GetDataAccess ()
		{
			return new MySqlDataAccess ();
			//return new PostgresDataAccess ();
		}

		static int Main (string [] args)
		{
			if (args.Length != 1)
				return 1;

			string base_path = args [0];
			if (!Directory.Exists (base_path)) {
				Console.Error.WriteLine ("Directory {0} not found.", base_path);
				return 2;
			}

			DataAccess da = GetDataAccess ();
			da.BeginTransaction ();
			try {
				int version_id = LoadVersion (da, base_path);
				LoadTodo (da, version_id, base_path);
				LoadMissing (da, version_id, base_path);
				LoadException (da, version_id, base_path);
				if (da.InTransaction) // Should always be true
					da.Commit ();
				Console.WriteLine ("Done. Version ID: {0}", version_id);
			} catch (Exception exc) {
				if (da.InTransaction)
					da.Rollback ();
				Console.WriteLine (exc);
			}
			return 0;
		}

		static int LoadVersion (DataAccess da, string base_path)
		{
			string filename = Path.Combine (base_path, "version.txt");
			string name = null;
			DateTime date;
			using (StreamReader reader = new StreamReader (filename)) {
				name = reader.ReadLine ().Trim ();
				date = DateTime.Parse (reader.ReadLine ());
			}
			if (String.IsNullOrEmpty (name))
				throw new ApplicationException ("Invalid name");
			Console.WriteLine ("Version: {0} Date: {1}", name, date);
			return da.InsertVersion (name, date);
		}

		static void LoadTodo (DataAccess da, int version_id, string base_path)
		{
			LoadFile (da, version_id, Path.Combine (base_path, "monotodo.txt"), true, false, false);
		}

		static void LoadMissing (DataAccess da, int version_id, string base_path)
		{
			LoadFile (da, version_id, Path.Combine (base_path, "missing.txt"), false, true, false);
		}

		static void LoadException (DataAccess da, int version_id, string base_path)
		{
			LoadFile (da, version_id, Path.Combine (base_path, "exception.txt"), false, false, true);
		}

		static void LoadFile (DataAccess da, int version_id, string filename, bool is_todo, bool is_missing, bool is_niex)
		{
			Console.WriteLine ("Loading {0}...", filename);
			using (StreamReader reader = new StreamReader (filename)) {
				string comment;
				string line;
				int n = 0;
				while ((line = reader.ReadLine ()) != null) {
					string name = line.Trim ();
					comment = null;
					if (is_todo) {
						string [] parts = name.Split ('-');
						name = parts [0].Trim ();
						comment = parts [1].Trim ();
						if (comment == "")
							comment = null;
					}
					da.InsertMember (version_id, name, is_todo, is_missing, is_niex, false, null, comment);
					n++;
					if ((n % 1000) == 0)
						Console.WriteLine (n);
				}
			}
		}
	}
}

