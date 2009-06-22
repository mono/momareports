using System;
using System.Configuration;
using System.Collections.Specialized;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Text;

using MySql.Data.MySqlClient;

namespace Moma {
	class MainClass {
		static string cnc_string;
		
		public static void Main (string[] args)
		{
			NameValueCollection col = ConfigurationManager.AppSettings;
			cnc_string = col ["MomaDB"];
			if (String.IsNullOrEmpty (cnc_string))
				throw new ApplicationException ("Missing connection string from configuration file.");
			
			foreach (string arg in args) {
				ProcessFile (arg);
			}
			Console.WriteLine ("Tables/Views/Sprocs created");
		}

		static DbConnection GetConnection ()
		{
			MySqlConnection conn = new MySqlConnection();
			conn.ConnectionString = cnc_string;
			conn.Open ();
			return conn;
		}

		static void ProcessFile (string filename)
		{
			Console.WriteLine ("Processing file {0}", filename);
			using (StreamReader reader = new StreamReader (filename)) {
				using (DbConnection conn = GetConnection ()) {
						StringBuilder sb = new StringBuilder();
						string line = null;
						while ((line = reader.ReadLine ()) != null) {
							if (line.StartsWith ("@STATEMENT")) {
								ExecuteStatement (conn, sb.ToString ());
								sb.Length = 0;
								continue;
							}
							sb.AppendFormat ("{0}\n", line);
						}
					}
			}
		}
				
		static void ExecuteStatement (DbConnection cnc, string command)
		{
			if (String.IsNullOrEmpty (command))
				return;
			Console.WriteLine ("***************************");
			Console.WriteLine (command);

			DbCommand cmd = cnc.CreateCommand ();
			cmd.CommandText = command;
			int res = cmd.ExecuteNonQuery ();
			Console.WriteLine ("Result: {0}", res);
			Console.WriteLine ("##########################");
		}
	}
}