using OThinker.H3.Controllers;
using System;
using System.Data;
using System.Linq;
using System.Web;

namespace DongZheng.H3.WebApi.Common.MySetting
{
    public static class WorkflowSetting
    {
        public static int SetCustomSettingValue(string InstanceID, string SchemaCode, string SettingName, string SettingValue)
        {
            string sql_num = "select count(1) from C_FlowSetting where instanceID='{0}' and SettingName='{1}'";
            string sql = "";
            if (Convert.ToInt32(AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(string.Format(sql_num, InstanceID, SettingName))) > 0)
            {
                //Update
                sql = "update C_FlowSetting set settingvalue='{2}' where instanceid='{0}' and settingname='{1}'";
                sql = string.Format(sql, InstanceID, SettingName, HttpUtility.UrlDecode(SettingValue));
            }
            else
            {
                //Insert
                sql = "insert into C_FlowSetting(instanceid,Schemacode,Settingname,settingvalue) values('{0}','{1}','{2}','{3}')";
                sql = string.Format(sql, InstanceID, SchemaCode, SettingName, HttpUtility.UrlDecode(SettingValue));
            }
            return AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(sql);
        }

        public static string GetCustomSettingValue(string InstanceID, string SettingName)
        {
            string sql = "select SettingValue from C_FlowSetting where instanceID='{0}' and SettingName='{1}'";
            sql = string.Format(sql, InstanceID, SettingName);
            DataTable dt = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0][0] + string.Empty;
            }
            else
            {
                return "";
            }
        }
    }
}