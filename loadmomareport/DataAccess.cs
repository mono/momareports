//
// DataAccess.cs (MoMa)
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
using System.Text;

namespace Mono.Moma {
	abstract class DataAccess {
		static string cnc_string;

		static DataAccess ()
		{
			NameValueCollection col = ConfigurationManager.AppSettings;
			cnc_string = col ["MomaDB"];
			if (String.IsNullOrEmpty (cnc_string))
				throw new ApplicationException ("Missing connection string from configuration file.");
		}

		protected IDbTransaction transaction;
		protected IDbConnection connection;

		public DataAccess ()
		{
		}

		protected string ConnectionString {
			get { return cnc_string; }
		}

		public bool InTransaction {
			get { return transaction != null; }
		}

		public virtual void BeginTransaction ()
		{
			
		}

		public virtual void Rollback ()
		{
		}

		public virtual void Commit ()
		{
		}

		public abstract int InsertReportMaster (DateTime submit_date, string ip, string definitions,
							string reported_by, string email, string organization,
							string homepage, string comment, string guid);
		public abstract bool InsertOrUpdatePInvoke (int report_id, string library_name, string function_name);
		public abstract bool InsertOrUpdateMember (int report_id, string name, bool is_missing, bool is_todo, bool is_niex, string todo_comment);
		public abstract void InitReportCounts (int report_id);
		public abstract void SetWpf (int report_id);

		protected abstract IDbConnection GetConnection ();

		protected static IDataParameter AddParameter (IDbCommand cmd, string name, object val)
		{
			IDataParameter p = cmd.CreateParameter ();
			p.ParameterName = name;
			p.Value = val;
			cmd.Parameters.Add (p);
			return p;
		}

		protected static IDataParameter AddOutputParameter (IDbCommand cmd, string name)
		{
			IDataParameter p = cmd.CreateParameter ();
			p.ParameterName = name;
			p.Direction = ParameterDirection.Output;
			cmd.Parameters.Add (p);
			return p;
		}
	}
}

