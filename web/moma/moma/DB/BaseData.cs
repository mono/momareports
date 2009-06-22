using System;
using System.Data;
using System.Data.Common;
using MySql.Data.MySqlClient;

namespace Moma.DB
{
    public abstract class BaseData
    {
        string cnc_string;

        public BaseData (string connection_string)
        {
            this.cnc_string = connection_string;
        }

        protected DbConnection GetConnection()
        {
            DbConnection cnc = new MySqlConnection(cnc_string);
            cnc.Open();
            return cnc;
        }
	
	protected virtual DbDataAdapter GetDataAdapter (DbCommand cmd)
	{
		return new MySqlDataAdapter ((MySqlCommand) cmd);
	}

	protected static DbParameter AddParameter (DbCommand cmd, string name, object val)
	{
                DbParameter p = cmd.CreateParameter ();
                p.ParameterName = name;
                p.Value = val;
                cmd.Parameters.Add (p);
                return p;
        }
    }
}
