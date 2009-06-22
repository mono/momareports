//
// MySqlDataAccess.cs (Moma);
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
using System.Data;
using MySql.Data.MySqlClient;

namespace Mono.Moma {
	class MySqlDataAccess : DataAccess {
		public override void BeginTransaction ()
		{
			if (transaction != null)
				throw new InvalidOperationException ("Already in a transaction");

			connection = GetConnection ();
			transaction = connection.BeginTransaction ();
		}

		public override void Rollback ()
		{
			if (transaction == null)
				throw new InvalidOperationException ("Call BeginTransaction first");

			try {
				transaction.Rollback ();
				connection.Close ();
			} finally {
				transaction = null;
				connection = null;
			}
		}

		public override void Commit ()
		{
			if (transaction == null)
				throw new InvalidOperationException ("Call BeginTransaction first");

			try {
				transaction.Commit ();
				connection.Close ();
			} finally {
				transaction = null;
				connection = null;
			}
		}

		protected override IDbConnection GetConnection ()
		{
			if (connection != null)
				return connection;

			IDbConnection cnc = new MySqlConnection ();
			cnc.ConnectionString = ConnectionString;
			cnc.Open ();
			return cnc;
		}

		public override int InsertReportMaster (DateTime submit_date, string ip, string definitions,
							string reported_by, string email, string organization,
							string homepage, string comment, string guid)
		{
			IDbConnection cnc = GetConnection ();
			try {
				IDbCommand cmd = cnc.CreateCommand ();
				cmd.Transaction = transaction;
				cmd.CommandText = "insert_report_master";
				cmd.CommandType = CommandType.StoredProcedure;
				AddParameter (cmd, "submit_date", submit_date);
				AddParameter (cmd, "ip", ip);
				AddParameter (cmd, "definitions", definitions);
				AddParameter (cmd, "reported_by", reported_by);
				AddParameter (cmd, "email", email);
				AddParameter (cmd, "organization", organization);
				AddParameter (cmd, "homepage", homepage);
				AddParameter (cmd, "comment", comment);
				AddParameter (cmd, "guid", guid);
				IDataParameter p = AddOutputParameter (cmd, "id");
				if (cmd.ExecuteNonQuery () != 1)
					throw new ApplicationException ("Error inserting new report (master)");
				return Convert.ToInt32 (p.Value);
			} finally {
				if (transaction == null)
					cnc.Close ();
			}
		}

		public override bool InsertOrUpdatePInvoke (int report_id, string library_name, string function_name)
		{
			IDbConnection cnc = GetConnection ();
			try {
				IDbCommand cmd = cnc.CreateCommand ();
				cmd.Transaction = transaction;
				cmd.CommandText = "insert_or_update_pinvoke";
				cmd.CommandType = CommandType.StoredProcedure;
				AddParameter (cmd, "report_id", report_id);
				AddParameter (cmd, "library_name", library_name);
				AddParameter (cmd, "function_name", function_name);
				int r = cmd.ExecuteNonQuery ();
				return (r == 2) ? false : true;
			} finally {
				if (transaction == null)
					cnc.Close ();
			}
		}

		public override bool InsertOrUpdateMember (int report_id, string name, bool is_missing, bool is_todo, bool is_niex, string todo_comment)
		{
			IDbConnection cnc = GetConnection ();
			try {
				IDbCommand cmd = cnc.CreateCommand ();
				cmd.Transaction = transaction;
				cmd.CommandText = "insert_or_update_member";
				cmd.CommandType = CommandType.StoredProcedure;
				AddParameter (cmd, "report_id", report_id);
				AddParameter (cmd, "name", name);
				AddParameter (cmd, "is_missing", is_missing);
				AddParameter (cmd, "is_todo", is_todo);
				AddParameter (cmd, "is_niex", is_niex);
				AddParameter (cmd, "todo_comment", todo_comment);
				int r = cmd.ExecuteNonQuery ();
				return (r == 2) ? false : true;
			} finally {
				if (transaction == null)
					cnc.Close ();
			}
		}
		
		public override void InitReportCounts (int report_id)
		{
			IDbConnection cnc = GetConnection ();
			try {
				IDbCommand cmd = cnc.CreateCommand ();
				cmd.Transaction = transaction;
				cmd.CommandText = "init_report_counts";
				cmd.CommandType = CommandType.StoredProcedure;
				AddParameter (cmd, "reportid", report_id);
				cmd.ExecuteNonQuery ();
			} finally {
				if (transaction == null)
					cnc.Close ();
			}
		}
		
		public override void SetWpf (int report_id)
		{
			IDbConnection cnc = GetConnection ();
			try {
				IDbCommand cmd = cnc.CreateCommand ();
				cmd.Transaction = transaction;
				cmd.CommandText = "UPDATE reports_master SET haswpf = 1 WHERE report_id = @reportid";
				AddParameter (cmd, "reportid", report_id);
				cmd.ExecuteNonQuery ();
			} finally {
				if (transaction == null)
					cnc.Close ();
			}
		}
	}
}

