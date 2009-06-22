using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using MySql.Data.MySqlClient;
using Moma.Web.Models;

namespace Moma.DB
{
    public class ReportsData : BaseData
    {
	int page_size = 20;

        public ReportsData (string connection_string) : base (connection_string)
        {
        }

        public int PageSize {
		get { return page_size; }
		set {
			if (value <= 0)
				value = 20;
			page_size = value;
		}
        }

	public int GetCountTotal ()
	{
		using (DbConnection cnc = GetConnection ()) {
			DbCommand cmd = cnc.CreateCommand ();
			cmd.CommandText =
				"SELECT COUNT(*) " +
				"FROM reports_master";
			return Convert.ToInt32 (cmd.ExecuteScalar ());	
		}
	}

        public MomaDataSet GetPagedReports (int page)
        {
		using (DbConnection cnc = GetConnection()) {
			DbCommand cmd = cnc.CreateCommand ();
			cmd.CommandText =
				"SELECT rm.report_id AS 'ReportId', rm.submit_date as 'SubmitDate', rm.definitions, rm.guid, " +
				"rc.TotalMissing, rc.TotalTodo, rc.TotalNiex, rc.TotalPInvoke " +
				"FROM reports_master rm " +
				"INNER JOIN reports_counts rc ON rc.report_id = rm.report_id " +
				"ORDER BY submit_date DESC " +
				"LIMIT @offset,@pagesize";
			AddParameter (cmd, "offset", (page - 1) * PageSize);
			AddParameter (cmd, "pagesize", PageSize);
			MomaDataSet ds = new MomaDataSet();
			DbDataAdapter adapter = GetDataAdapter(cmd);
			adapter.Fill (ds, "Applications");
			return ds;
		}
        }

	public MomaDataSet GetReport (string guid)
	{
		using (DbConnection cnc = GetConnection()) {
			DbCommand cmd = cnc.CreateCommand ();
			cmd.CommandText =
				"SELECT report_id AS 'ReportId', submit_date AS 'SubmitDate', " +
				"	ip, definitions, reported_by AS 'ReportedBy', email, " +
				"	organization, homepage, comment, guid " +
				"FROM reports_master " +
				"WHERE guid = @guid";
			AddParameter (cmd, "guid", guid);
			MomaDataSet ds = new MomaDataSet();
			DbDataAdapter adapter = GetDataAdapter(cmd);
			adapter.Fill (ds, "Applications");
			if (ds.Applications.Count == 1) {
				cmd = cnc.CreateCommand ();
				cmd.CommandText =
					"SELECT r.name, r.count, " +
					"       CASE r.is_todo WHEN b'1' THEN TRUE ELSE FALSE END as 'IsTodo', " +
					"       CASE r.is_missing WHEN b'1' THEN TRUE ELSE FALSE END as 'IsMissing', " +
					"       CASE r.is_niex WHEN b'1' THEN TRUE ELSE FALSE END as 'IsNiex', " +
					"       CASE r.is_fixed WHEN b'1' THEN TRUE ELSE FALSE END as 'IsFixed' " +
					"FROM reports_view r " +
					"WHERE r.report_id = @report_id AND r.is_fixed = FALSE " +
					"ORDER BY r.count DESC";
				AddParameter (cmd, "report_id", ds.Applications [0].ReportId);
				adapter = GetDataAdapter (cmd);
				adapter.Fill (ds, "Members");

				cmd = cnc.CreateCommand ();
				cmd.CommandText =
					"SELECT library_name as 'Library', function_name as 'Function', count " +
					"FROM reports_pinvoke " +
					"WHERE report_id = @report_id " +
					"ORDER BY count DESC,library_name";
				AddParameter (cmd, "report_id", ds.Applications [0].ReportId);
				adapter = GetDataAdapter (cmd);
				adapter.Fill (ds, "PInvokes");
			}
			return ds;
		}
	}
    }
}
