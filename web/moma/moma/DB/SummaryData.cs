using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using MySql.Data.MySqlClient;
using Moma.Web.Models;

namespace Moma.DB
{
    public class SummaryData : BaseData
    {
	int page_size = 20;

        public SummaryData (string connection_string) : base (connection_string)
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

	public int GetCountMissingMembers ()
	{
		using (DbConnection cnc = GetConnection ()) {
			DbCommand cmd = cnc.CreateCommand ();
			cmd.CommandText =
				"SELECT COUNT(DISTINCT(r.member_id))" +
				"FROM reports_view r " +
				"WHERE r.is_missing = TRUE AND r.is_fixed = FALSE";
			return Convert.ToInt32 (cmd.ExecuteScalar ());	
		}
	}

	MomaDataSet GetMissing (int page, bool sort_by_use)
	{
		string sort_clause = (sort_by_use) ? "ORDER BY 2 DESC, 1 DESC " : "ORDER BY 1 DESC, 2 DESC ";
		using (DbConnection cnc = GetConnection()) {
			DbCommand cmd = cnc.CreateCommand ();
			cmd.CommandText =
				"SELECT COUNT(DISTINCT(r.report_id)) AS 'PerAppCount', SUM(r.count) AS 'TotalCount', " +
				"r.name AS 'APIName' " +
				"FROM reports_view r " +
				"WHERE r.is_missing = TRUE AND r.is_fixed = FALSE " +
				"GROUP BY r.member_id " +
				sort_clause + 
				"LIMIT @offset,@pagesize";
			AddParameter (cmd, "offset", (page - 1) * PageSize);
			AddParameter (cmd, "pagesize", PageSize);
			MomaDataSet ds = new MomaDataSet();
			DbDataAdapter adapter = GetDataAdapter(cmd);
			adapter.Fill (ds, "Report");
			return ds;
		}
	}

        public MomaDataSet GetMissingByUse (int page)
        {
		return GetMissing (page, true);
        }

        public MomaDataSet GetMissingByApplication (int page)
        {
		return GetMissing (page, false);
        }

	public int GetCountTODOMembers ()
	{
		using (DbConnection cnc = GetConnection ()) {
			DbCommand cmd = cnc.CreateCommand ();
			cmd.CommandText =
				"SELECT COUNT(DISTINCT(r.member_id))" +
				"FROM reports_view r " +
				"WHERE r.is_todo = TRUE AND r.is_fixed = FALSE";
			return Convert.ToInt32 (cmd.ExecuteScalar ());	
		}
	}

	MomaDataSet GetTODO (int page, bool sort_by_use)
	{
		string sort_clause = (sort_by_use) ? "ORDER BY 2 DESC, 1 DESC " : "ORDER BY 1 DESC, 2 DESC ";
		using (DbConnection cnc = GetConnection()) {
			DbCommand cmd = cnc.CreateCommand ();
			cmd.CommandText =
				"SELECT COUNT(DISTINCT(r.report_id)) AS 'PerAppCount', SUM(r.count) AS 'TotalCount', " +
				"r.name AS 'APIName' " +
				"FROM reports_view r " +
				"WHERE r.is_todo = TRUE AND r.is_fixed = FALSE " +
				"GROUP BY r.member_id " +
				sort_clause + 
				"LIMIT @offset,@pagesize";
			AddParameter (cmd, "offset", (page - 1) * PageSize);
			AddParameter (cmd, "pagesize", PageSize);
			MomaDataSet ds = new MomaDataSet();
			DbDataAdapter adapter = GetDataAdapter(cmd);
			adapter.Fill (ds, "Report");
			return ds;
		}
	}

        public MomaDataSet GetTODOByUse (int page)
        {
		return GetTODO (page, true);
        }

        public MomaDataSet GetTODOByApplication (int page)
        {
		return GetTODO (page, false);
        }

	public int GetCountNIEXMembers ()
	{
		using (DbConnection cnc = GetConnection ()) {
			DbCommand cmd = cnc.CreateCommand ();
			cmd.CommandText =
				"SELECT COUNT(DISTINCT(r.member_id))" +
				"FROM reports_view r " +
				"WHERE r.is_niex = TRUE AND r.is_fixed = FALSE";
			return Convert.ToInt32 (cmd.ExecuteScalar ());	
		}
	}

	MomaDataSet GetNIEX (int page, bool sort_by_use)
	{
		string sort_clause = (sort_by_use) ? "ORDER BY 2 DESC, 1 DESC " : "ORDER BY 1 DESC, 2 DESC ";
		using (DbConnection cnc = GetConnection()) {
			DbCommand cmd = cnc.CreateCommand ();
			cmd.CommandText =
				"SELECT COUNT(DISTINCT(r.report_id)) AS 'PerAppCount', SUM(r.count) AS 'TotalCount', " +
				"r.name AS 'APIName' " +
				"FROM reports_view r " +
				"WHERE r.is_niex = TRUE AND r.is_fixed = FALSE " +
				"GROUP BY r.member_id " +
				sort_clause + 
				"LIMIT @offset,@pagesize";
			AddParameter (cmd, "offset", (page - 1) * PageSize);
			AddParameter (cmd, "pagesize", PageSize);
			MomaDataSet ds = new MomaDataSet();
			DbDataAdapter adapter = GetDataAdapter(cmd);
			adapter.Fill (ds, "Report");
			return ds;
		}
	}

        public MomaDataSet GetNIEXByUse (int page)
        {
		return GetNIEX (page, true);
        }

        public MomaDataSet GetNIEXByApplication (int page)
        {
		return GetNIEX (page, false);
        }

	public int GetCountApplicationsWithPInvoke ()
	{
		using (DbConnection cnc = GetConnection ()) {
			DbCommand cmd = cnc.CreateCommand ();
			cmd.CommandText =
				"SELECT COUNT(*) " +
				"FROM reports_counts " +
				"WHERE TotalPInvoke > 0";
			return Convert.ToInt32 (cmd.ExecuteScalar ());	
		}
	}

	MomaDataSet GetPInvoke (int page, bool sort_by_use)
	{
		string sort_clause = (sort_by_use) ? "ORDER BY 2 DESC, 1 DESC " : "ORDER BY 1 DESC, 2 DESC ";
		using (DbConnection cnc = GetConnection()) {
			DbCommand cmd = cnc.CreateCommand ();
			cmd.CommandText =
				"SELECT COUNT(DISTINCT(r.report_id)) AS 'PerAppCount', SUM(r.count) AS 'TotalCount', " +
				"r.library_name as 'Library', r.function_name AS 'Function' " +
				"FROM reports_pinvoke r " +
				"GROUP BY r.function_name,r.library_name " +
				sort_clause + 
				"LIMIT @offset,@pagesize";
			AddParameter (cmd, "offset", (page - 1) * PageSize);
			AddParameter (cmd, "pagesize", PageSize);
			MomaDataSet ds = new MomaDataSet();
			DbDataAdapter adapter = GetDataAdapter(cmd);
			adapter.Fill (ds, "Report");
			return ds;
		}
	}

        public MomaDataSet GetPInvokeByUse (int page)
        {
		return GetPInvoke (page, true);
        }

        public MomaDataSet GetPInvokeByApplication (int page)
        {
		return GetPInvoke (page, false);
        }
    }
}
