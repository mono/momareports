using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using MySql.Data.MySqlClient;
using Moma.Web.Models;

namespace Moma.DB
{
    public class ApiData : BaseData
    {
        public ApiData(string connection_string) : base (connection_string)
        {
        }

	static void MergeRow (MomaDataSet.MembersRow row0, MomaDataSet.MembersRow other)
	{
		row0.IsFixed |= other.IsFixed;
		row0.IsTodo |= other.IsTodo;
		row0.IsMissing |= other.IsMissing;
		row0.IsNiex |= other.IsNiex;
		if (!other.IsTODOCommentNull ())
			row0.TODOComment = other.TODOComment;
	}

	static void MergeRows (MomaDataSet.MembersDataTable tbl)
	{
		int count = tbl.Count;
		if (count == 1)
			return;
		if (count > 4 || count <= 0)
			throw new Exception ("This should not happen");

		MomaDataSet.MembersRow row0 = tbl [0];
		MergeRow (row0, tbl [1]);
		if (count > 2)
			MergeRow (row0, tbl [2]);
		if (count > 3)
			MergeRow (row0, tbl [3]);

		while (count > 1) {
			tbl.Rows.RemoveAt (1);
			count--;
		}
	}

	public MomaDataSet GetApiData (string apiname)
	{
		using (DbConnection cnc = GetConnection ()) {
			DbCommand cmd = cnc.CreateCommand ();
			cmd.CommandText =
				"SELECT m.member_id as 'MemberId' , v.version_name AS 'VersionName', m.name as 'Name', " +
				"       CASE m.is_todo WHEN b'1' THEN TRUE ELSE FALSE END as 'IsTodo', " +
				"       CASE m.is_missing WHEN b'1' THEN TRUE ELSE FALSE END as 'IsMissing', " +
				"       CASE m.is_niex WHEN b'1' THEN TRUE ELSE FALSE END as 'IsNiex', " +
				"       CASE m.is_fixed WHEN b'1' THEN TRUE ELSE FALSE END as 'IsFixed', " +
				"       m.fixed_in_version as 'FixedInVersion', m.todo_comment as 'TodoComment' " +
				"FROM members m " +
				"INNER JOIN versions v ON v.version_id = m.version_id " +
				"WHERE m.name = @name";
			AddParameter (cmd, "name", apiname);
			MomaDataSet ds = new MomaDataSet();
			DbDataAdapter adapter = GetDataAdapter(cmd);
			adapter.Fill (ds, "Members");
			if (ds.Members.Rows.Count > 0) {
				MergeRows (ds.Members);
				MomaDataSet.MembersRow row = ds.Members[0];
				cmd = cnc.CreateCommand ();
				cmd.CommandText =
					"SELECT COUNT(*) AS 'TotalApps', SUM(r.count) AS 'TotalCalls' " +
					"FROM reports_members r " +
					"WHERE r.member_id = @memberid";
				AddParameter (cmd, "memberid", row.MemberId);
				adapter = GetDataAdapter(cmd);
				adapter.Fill (ds, "ApiUse");
				if (ds.ApiUse [0].IsTotalCallsNull ())
					ds.ApiUse [0].TotalCalls = 0;
				cmd = cnc.CreateCommand ();
				cmd.CommandText =
					"SELECT r.report_id as 'ReportId', r.guid, r.submit_date as 'SubmitDate', " +
					"	rc.totaltodo, rc.totalmissing, rc.totalniex, rc.totalpinvoke " +
					"FROM reports_master r " +
					"INNER JOIN reports_counts rc ON rc.report_id = r.report_id " +
					"WHERE r.report_id IN (SELECT report_id FROM reports_members WHERE member_id = @memberid) " +
					"ORDER BY submitdate DESC";
				AddParameter (cmd, "memberid", row.MemberId);
				adapter = GetDataAdapter(cmd);
				adapter.Fill (ds, "Applications");
			}
			return ds;
		}
	}
    }
}
