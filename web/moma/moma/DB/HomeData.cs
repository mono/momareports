using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using MySql.Data.MySqlClient;
using Moma.Web.Models;

namespace Moma.DB
{
    public class HomeData : BaseData
    {
        public HomeData (string connection_string) : base (connection_string)
        {
        }

	public MomaDataSet GetHomeData ()
	{
		using (DbConnection cnc = GetConnection ()) {
			DbCommand cmd = cnc.CreateCommand ();
			cmd.CommandText =
				"SELECT 'No issues' AS 'Title', COUNT(*) as 'Apps' " +
				"FROM reports_counts " +
				"WHERE TotalProblems = 0 " +
				"UNION " +
				"SELECT 'Between 1 and 3', COUNT(*) " +
				"FROM reports_counts " +
				"WHERE TotalProblems >= 1 AND TotalProblems <= 3 " +
				"UNION " +
				"SELECT 'Between 4 and 10', COUNT(*) " +
				"FROM reports_counts " +
				"WHERE TotalProblems >= 4 AND TotalProblems <= 10 " +
				"UNION " +
				"SELECT 'Between 11 and 30', COUNT(*) " +
				"FROM reports_counts " +
				"WHERE TotalProblems >= 11 AND TotalProblems <= 30 " +
				"UNION " +
				"SELECT 'Between 31 and 50', COUNT(*) " +
				"FROM reports_counts " +
				"WHERE TotalProblems >= 31 AND TotalProblems <= 50 " +
				"UNION " +
				"SELECT 'More than 50', COUNT(*) " +
				"FROM reports_counts " +
				"WHERE TotalProblems > 50";
			MomaDataSet ds = new MomaDataSet();
			DbDataAdapter adapter = GetDataAdapter(cmd);
			adapter.Fill (ds, "ByIssue");
			foreach (MomaDataSet.ByIssueRow row in ds.ByIssue.Rows)
				row.Title = String.Format ("{0} ({1})", row.Title, row.Apps);

			cmd = cnc.CreateCommand ();
			cmd.CommandText =
				"SELECT 'No issues' AS 'Title', COUNT(*) as 'Apps' " +
				"FROM reports_counts " +
				"WHERE TotalProblems - TotalPInvoke = 0 " +
				"UNION " +
				"SELECT 'Between 1 and 3', COUNT(*) " +
				"FROM reports_counts " +
				"WHERE TotalProblems - TotalPInvoke >= 1 AND TotalProblems - TotalPInvoke <= 3 " +
				"UNION " +
				"SELECT 'Between 4 and 10', COUNT(*) " +
				"FROM reports_counts " +
				"WHERE TotalProblems - TotalPInvoke >= 4 AND TotalProblems - TotalPInvoke <= 10 " +
				"UNION " +
				"SELECT 'Between 11 and 30', COUNT(*) " +
				"FROM reports_counts " +
				"WHERE TotalProblems - TotalPInvoke >= 11 AND TotalProblems - TotalPInvoke <= 30 " +
				"UNION " +
				"SELECT 'Between 31 and 50', COUNT(*) " +
				"FROM reports_counts " +
				"WHERE TotalProblems - TotalPInvoke >= 31 AND TotalProblems - TotalPInvoke <= 50 " +
				"UNION " +
				"SELECT 'More than 50', COUNT(*) " +
				"FROM reports_counts " +
				"WHERE TotalProblems - TotalPInvoke > 50";
			adapter = GetDataAdapter(cmd);
			adapter.Fill (ds, "ByIssueNoPInvoke");
			foreach (MomaDataSet.ByIssueNoPInvokeRow row in ds.ByIssueNoPInvoke.Rows)
				row.Title = String.Format ("{0} ({1})", row.Title, row.Apps);

			cmd = cnc.CreateCommand ();
			cmd.CommandText =
				"SELECT 'No issues' AS 'Title', COUNT(*) as 'Apps' " +
				"FROM reports_counts " +
				"WHERE TotalProblems - TotalTodo = 0 " +
				"UNION " +
				"SELECT 'Between 1 and 3', COUNT(*) " +
				"FROM reports_counts " +
				"WHERE TotalProblems - TotalTodo >= 1 AND TotalProblems - TotalTodo <= 3 " +
				"UNION " +
				"SELECT 'Between 4 and 10', COUNT(*) " +
				"FROM reports_counts " +
				"WHERE TotalProblems - TotalTodo >= 4 AND TotalProblems - TotalTodo <= 10 " +
				"UNION " +
				"SELECT 'Between 11 and 30', COUNT(*) " +
				"FROM reports_counts " +
				"WHERE TotalProblems - TotalTodo >= 11 AND TotalProblems - TotalTodo <= 30 " +
				"UNION " +
				"SELECT 'Between 31 and 50', COUNT(*) " +
				"FROM reports_counts " +
				"WHERE TotalProblems - TotalTodo >= 31 AND TotalProblems - TotalTodo <= 50 " +
				"UNION " +
				"SELECT 'More than 50', COUNT(*) " +
				"FROM reports_counts " +
				"WHERE TotalProblems - TotalTodo > 50";
			adapter = GetDataAdapter(cmd);
			adapter.Fill (ds, "ByIssueNoTodo");
			foreach (MomaDataSet.ByIssueNoTodoRow row in ds.ByIssueNoTodo.Rows)
				row.Title = String.Format ("{0} ({1})", row.Title, row.Apps);
			return ds;
		}
	}
    }
}
