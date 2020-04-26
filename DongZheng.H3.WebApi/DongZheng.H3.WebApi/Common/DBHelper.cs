using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using OThinker.H3.Controllers;


public static class DBHelper
{
    public static DataTable ExecuteDataTableSql(string connectionCode, string sql)
    {
        try
        {
            var dbObject = AppUtility.Engine.SettingManager.GetBizDbConnectionConfig(connectionCode);
            if (dbObject != null)
            {
                OThinker.Data.Database.CommandFactory factory = new OThinker.Data.Database.CommandFactory(dbObject.DbType, dbObject.DbConnectionString);
                var command = factory.CreateCommand();
                return command.ExecuteDataTable(sql);
            }
            return null;
        }
        catch (Exception ex)
        {
            AppUtility.Engine.LogWriter.Write("ExecuteDataTableSql Exception:connectionCode-->" + connectionCode + ",sql-->" + sql + "," + ex);
            return null;
        }
    }
}
