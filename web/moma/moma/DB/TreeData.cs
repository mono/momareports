using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using MySql.Data.MySqlClient;
using Moma.Web.Models;

namespace Moma.DB {
	public class TreeData : BaseData {
		public TreeData (string connection_string)
			: base (connection_string)
		{
		}

		public MomaDataSet GetTree ()
		{
			using (DbConnection cnc = GetConnection ()) {
				DbCommand cmd = cnc.CreateCommand ();
				cmd.CommandText =
					"SELECT r.name, " +
					"   CASE r.is_todo WHEN b'1' THEN TRUE ELSE FALSE END AS 'IsTodo', " +
					"   CASE r.is_missing WHEN b'1' THEN TRUE ELSE FALSE END AS 'IsMissing', " +
					"   CASE r.is_niex WHEN b'1' THEN TRUE ELSE FALSE END AS 'IsNiex', " +
					"   0 AS 'Count' " +
					"FROM reports_view r " +
					"WHERE r.is_fixed = 0 " +
					"GROUP BY r.member_id " +
					"ORDER BY r.name ";
				MomaDataSet ds = new MomaDataSet ();
				DbDataAdapter adapter = GetDataAdapter (cmd);
				adapter.Fill (ds, "Members");
				return ds;
			}
		}
	}
}
