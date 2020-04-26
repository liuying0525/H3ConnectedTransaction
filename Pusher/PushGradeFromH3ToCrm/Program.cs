using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScoreCore;
using System.Data;
using System.Runtime.Serialization.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.IO;

namespace PushGradeFromH3ToCrm
{
    /// <summary>
    /// 程序入口
    /// </summary>
    class Program
    {
        /// <summary>
        /// 插入评分列表语句
        /// </summary>
        public static string DealerGrade = "INSERT INTO C_JXSMC(DEALERID,JXSMC,NWWW,ZRSCORE,DHSCORE,ZHSCORE,JXS_LEVEL,KIND,YEAR,MONTH,DAY,CRMDEALERID,HASSCORE)VALUES([VALUES])";
        private static bool recordLog = false;
        /// <summary>
        /// 程序入口
        /// </summary>
        /// <param name="args">程序集参数</param>
        static void Main(string[] args)
        {
            //string load = LoadDataAndGetGrade();
            RunGrade.GetInstance().StartRunner((PushInterval)int.Parse(Config.Settings["PushInterval"]), DateTime.Parse(Config.Settings["PushPointTime"]), LoadDataAndGetGrade);
            Pusher.GetInstance().StartPusher();
        }
        /// <summary>
        /// 读取数据和计算评分
        /// </summary>
        /// <returns>评分json</returns>
        static string LoadDataAndGetGrade()
        {
            Log.WriteLog(true, "开始读取数据结构", 0.0, null);
            ScoreManage sm = ScoreManage.LoadGradeObject("loadfullscorestruct", "C_Grade", new StructNames(), LoadDealerInfo, Config.Settings["Engine"]);
            //ScoreManage.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable("TRUNCATE TABLE C_JXSMC ");
            //Log.WriteLog(true, sm.GetGradeLog, 0);
            sm.ReCordLog = Program.recordLog;
            Log.WriteLog(true, "开始计算评分", 0.0, null);
            List<Dealer> dealers = sm.GetGrade(true);
            if (Program.recordLog)
            {
                Log.WriteLog(true, sm.GetGradeLog, 0.0, null);
            }
            JArray jarray = new JArray();
            JArray ret = new JArray();
            List<string> sqlList = new List<string>();
            foreach (Dealer item in dealers)
            {
                JObject token = new JObject();
                token.Add("menu_name", "customer_success");
                token.Add("third_system_no", item.DealerId);
                token.Add("account", item.DealerAccount);
                token.Add("dealerType", item.DealerType);
                token.Add("dealerKind", item.DealerKind);
                token.Add("dealerName", item.DealerName);
                token.Add("hasScore", item.ResultList.Count > 0 ? "1" : "0");
                token.Add("SystemScore", item.ResultList.Count > 0 ? item.ResultList[0].TotalScore.ToString() : "");
                token.Add("grade", item.ResultList.Count > 0 ? item.ResultList[0].ResultGrade : "");
                ret.Add(token);
                Score rw = item.ResultList.Count > 0 ? item.ResultList[0].Scores.FirstOrDefault(x => x.ScoreName == "入网评分") : null;
                Score yw = item.ResultList.Count > 0 ? item.ResultList[0].Scores.FirstOrDefault(x => x.ScoreName == "业务评分") : null;
                sqlList.Add(DealerGrade.Replace("[VALUES]",
                    "'" + item.DealerId + "','" + item.DealerName + "','" + item.DealerType + "','" +
                    (rw != null ? rw.RealScore.ToString() : "") + "','" + 
                    (yw != null ? yw.RealScore.ToString() : "") + "','" + 
                    (item.ResultList.Count > 0 ? item.ResultList[0].TotalScore.ToString() : "") + "','" + 
                    (item.ResultList.Count > 0 ? item.ResultList[0].ResultGrade : "") + "','" +
                    item.DealerKind + "','" + DateTime.Now.Year + "','" + DateTime.Now.Month + "','" +
                    DateTime.Now.Day + "','" + item.DealerId + "','" + (item.ResultList.Count > 0 ? "1" : "0") + "'"));
            }
            try
            {
                foreach (string sql in sqlList)
                {
                    int i = ScoreManage.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(sql);
                }
            }
            catch (Exception e)
            {
            }
            string sendData = ret.ToString();

            if (!Directory.Exists(".\\WaitPush")) Directory.CreateDirectory(".\\WaitPush");
            StreamWriter sw = new StreamWriter(".\\WaitPush\\" + DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss_fff") + ".txt");
            sw.Write(sendData);
            sw.Close();

            return "源数据：\r\n" + JsonConvert.SerializeObject(dealers) + "\r\n处理后数据：\r\n" + sendData + "\r\n";
        }
        /// <summary>
        /// 读取经销商信息
        /// </summary>
        /// <param name="dealers">经销商列表</param>
        /// <returns>经销商列表</returns>
        static List<Dealer> LoadDealerInfo(List<Dealer> dealers)
        {
            string logs = "";
            DataTable table = ScoreManage.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable("SELECT * FROM DEALERACCEDE WHERE DISABLED = 0");
            foreach (DataRow row in table.Rows)
            {
                if (dealers.FirstOrDefault(x => x.DealerId == row["DealerId"].ToString()) == null)
                {
                    dealers.Add(new Dealer()
                    {
                        DealerId = row["DealerId"].ToString(),
                        DealerAccount = row["DealerAccount"].ToString(),
                        DealerName = row["DealerName"].ToString()
                    });
                }
                else
                {
                    logs += "[" + row["DealerId"] + "|" + row["DealerName"] + "],";
                }
            }
            logs += string.Format(@"从数据库中读取到已入网经销商{0}家,读取到结构中{1}家,其中重复项为{2}
", table.Rows.Count, dealers.Count, logs);
            table = ScoreManage.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable("SELECT * FROM DealerData");
            int count = 0;
            foreach (DataRow row in table.Rows)
            {
                var dealer = dealers.FirstOrDefault(x => x.DealerId == row["DealerId"].ToString());
                if (dealer != null)
                {
                    count++;
                    dealer.DealerType = row["DealerType"].ToString();
                    dealer.DealerKind = row["DealerKind"].ToString();
                    DealerData dealerData = new DealerData()
                    {
                        ObjectId = row["ObjectID"].ToString(),
                        DealerDataType = row["Type"].ToString().ToLower() == "single" ? DealerDataType.Single : DealerDataType.Array,
                        Variable = row["Name"].ToString(),
                        Values = row["Value"] == null || row["Value"] == DBNull.Value ? "" : row["Value"],
                    };
                    if (dealerData.DealerDataType == DealerDataType.Array)
                    {
                        List<string> values = new List<string>();
                        DataTable valueTable = ScoreManage.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable("SELECT * FROM DealerDataArray WHERE \"DealerDataObjectID\"='" + dealerData.ObjectId + "'");
                        foreach (DataRow valueItem in valueTable.Rows)
                        {
                            values.Add(valueItem["Value"].ToString());
                        }
                        dealerData.Values = values;
                    }
                    dealer.DataList.Add(dealerData);
                }
            }
            var noDataDealer = dealers.Where(x => x.DataList.Count <= 0);
            string noDataLog = "";
            foreach (Dealer item in noDataDealer)
            {
                noDataLog += "[" + item.DealerId + "|" + item.DealerName + "],";
            }
            logs += string.Format(@"共有{0}家经销商读取到入网数据数据.有{1}家未读取到数据{{{2}}}
", count, noDataDealer.Count(), noDataLog);

            string viewLog = "";
            List<View> views = Config.Settings.GetViews("DataViews", "SelectStr");
            foreach (View view in views)
            {
                viewLog += string.Format(@"读取视图:{0},{1},{2},{3},{4}
", view.ViewName, view.DealerColumn, view.DealerName, view.DealerType, view.DealerKind);
                try
                {
                    Dictionary<string, bool> noDataDic = new Dictionary<string, bool>();
                    foreach (var item in dealers)
                    {
                        noDataDic.Add(item.DealerId, false);
                    }
                    table = ScoreManage.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable("SELECT * FROM " + view.ViewName);
                    if (table.Columns.Contains(view.DealerColumn) && table.Columns.Contains(view.DealerName) && table.Columns.Contains(view.DealerKind) && table.Columns.Contains(view.DealerType))
                    {
                        count = 0;
                        string noFindLog = "";
                        foreach (DataRow row in table.Rows)
                        {
                            try
                            {
                                Dealer dealer = dealers.FirstOrDefault(x => x.DealerId == row[view.DealerColumn].ToString());
                                if (dealer == null)
                                {
                                    count++;
                                    noFindLog += string.Format(@"        未找到经销商{0},新建
", row[view.DealerColumn]);
                                    dealer = new Dealer() { DealerName = row[view.DealerName] + "", DealerAccount = "", DealerKind = row[view.DealerKind] + "", DealerId = row[view.DealerColumn] + "", DealerType = row[view.DealerType] + "" };
                                    dealers.Add(dealer);
                                }
                                else
                                {
                                    noDataDic[dealer.DealerId] = true;
                                }
                                foreach (DataColumn column in table.Columns)
                                {
                                    if (column.ColumnName == view.DealerColumn || column.ColumnName == view.DealerKind || column.ColumnName == view.DealerName || column.ColumnName == view.DealerType) continue;
                                    dealer.DataList.Add(new DealerData() { ObjectId = Guid.NewGuid().ToString(), DealerDataType = DealerDataType.Single, Variable = column.ColumnName, Values = row[column] + "" });
                                }
                            }
                            catch
                            {

                            }
                        }
                        noDataLog = "";
                        var noDataDealerKP = noDataDic.Where(x => x.Value == false);
                        foreach (var item in noDataDealerKP)
                        {
                            var d = dealers.FirstOrDefault(x => x.DealerId == item.Key);
                            noDataLog += "[" + d.DealerId + "|" + d.DealerName + "],";
                        }

                        viewLog += string.Format(@"    视图读取成功能,视图内共有{2}条数据,其中有{1}家匹配到业务数据,有{0}家未匹配到业务数据.
{6}
    有{3}条业务数据未匹配到入网经销商.有{4}条业务数据匹配到入网经销商.
{5}", noDataDealerKP.Count(), dealers.Count - noDataDealerKP.Count(), table.Rows.Count, count, table.Rows.Count - count, noDataLog, noFindLog);
                    }
                    else
                    {
                        viewLog += string.Format(@"    视图字段匹配失败:{0}|{1},{2}|{3},{4}|{5},{6}|{7}
", view.DealerColumn, table.Columns.Contains(view.DealerColumn), view.DealerName, table.Columns.Contains(view.DealerName), view.DealerKind, table.Columns.Contains(view.DealerKind), view.DealerType, table.Columns.Contains(view.DealerType));
                    }
                }
                catch(Exception ex)
                {
                    viewLog += string.Format(@"    视图{0}读取失败:{1}
", view.ViewName, ex.ToString());
                }
            }
            //Log.WriteLog(true, logs + viewLog, 0);
            return dealers;
        }
    }
}
