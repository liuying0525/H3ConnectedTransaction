using OThinker.H3.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace DongZheng.H3.WebApi.Controllers.PCM
{
    [ValidateInput(false)]
    [Xss]
    public class PCMController : OThinker.H3.Controllers.ControllerBase
    {
        public override string FunctionCode => "";

        // GET: PCM
        public string Index()
        {
            return "PCM Controller";
        }

        public JsonResult Get_PCM_Result()
        {
            AppUtility.Engine.LogWriter.Write("中科软回调开始");
            var xml_result = "";
            try
            {
                //1.直接拿到Body中的内容
                var sr = new StreamReader(Request.InputStream);
                xml_result = HttpUtility.HtmlDecode(sr.ReadToEnd());
                //2.截取出PBOCDATA内的内容
                int first_index = xml_result.IndexOf("<PBOCDATA>");
                int last_index = xml_result.IndexOf("</PBOCDATA>");
                var result = "<?xml version=\"1.0\" encoding=\"UTF - 8\" ?>" + xml_result.Substring(first_index, last_index - first_index + 11);
                AppUtility.Engine.LogWriter.Write("Get_PCM_Result:" + result);
                //3.采用xml反序列化进行，但是Applicant在有多个人的情况下是并列的，这不符合规范，所以增加了一个Applicants节点，把所有的Applicant包括在内；
                int f_index = result.IndexOf("<Applicant>");
                int l_index = result.LastIndexOf("</Applicant>");
                result = result.Substring(0, f_index) + "<Applicants>" + result.Substring(f_index, l_index - f_index + 12) +
                    "</Applicants>" + result.Substring(l_index + 12);
                TextReader tr = new StringReader(result);
                XmlSerializer reader = new XmlSerializer(typeof(PBOCDATA));
                PBOCDATA pboc_data = reader.Deserialize(tr) as PBOCDATA;
                AppUtility.Engine.LogWriter.Write("中科软回调申请号：" + pboc_data.Data.application_no);
                //4.保存pboc_data的值；
                Save_PBOC_Data(pboc_data);
                //5.发送给融数；
                string application_no = pboc_data.Data.application_no;
                if (!string.IsNullOrEmpty(application_no))
                {
                    string SchemaCode = "RetailApp";
                    string[] arryV = application_no.Split('_');
                    string manual = application_no.Split('_')[1].Substring(0, 1);
                    string schemaid = application_no.Split('_')[1].Substring(1,1);
                    if (schemaid == "2")
                    {
                        SchemaCode = "CompanyApp";
                    }
                    else if (schemaid == "3")
                    {
                        SchemaCode = "APPLICATION";
                    }
                    string sql = "select objectid from OT_InstanceContext where  bizobjectschemacode='" + SchemaCode + "'and sequenceno='" + arryV[2] + "'";
                    string instanceid = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(sql).ToString();
                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    dic.Add("SchemaCode", SchemaCode + string.Empty);
                    dic.Add("manual", manual + string.Empty);
                    dic.Add("instanceid", instanceid + string.Empty);
                    BizService.ExecuteBizNonQuery("DZWebService", "postHttp", dic);
                }
                else
                {
                    AppUtility.Engine.LogWriter.Write("调用风控失败：申请号，" + pboc_data.Data.application_no);
                }
            }
            catch (Exception ex)
            {
                AppUtility.Engine.LogWriter.Write("中科软回调开始失败--> xml_result:" + xml_result + "，错误信息：" + ex.Message);
            }

            return Json("Received!", JsonRequestBehavior.AllowGet);
        }


        [Serializable]
        public class PBOCDATA
        {
            public PCM_Data Data { get; set; }
            public List<Applicant> Applicants { get; set; }
            public class PCM_Data
            {
                public string application_no { get; set; }
                public string res_date { get; set; }
            }
            public class Applicant
            {
                public string Name { get; set; }
                public string Id_number { get; set; }
                public string pboc_report { get; set; }
                public string pboc_no { get; set; }
                /// <summary>
                /// 角色，type是关键字，直接使用会有@，故转成app_type
                /// </summary>
                [XmlElement("type")]
                public string app_type { get; set; }
            }
        }

        public bool Save_PBOC_Data(PBOCDATA data)
        {
            try
            {
                string sql_num = "select count(1) from C_PBOC_RESULT where application_no='" + data.Data.application_no + "'";
                int num = Convert.ToInt32(AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(sql_num));
                if (num > 0)//原表中有数据，先移动到历史记录表；
                {
                    string sql_to_his = "insert into C_PBOC_RESULT_HIS select* from C_PBOC_RESULT where application_no = '" + data.Data.application_no + "'";
                    int n_to_his = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(sql_to_his);
                    string sql_del = "delete from C_PBOC_RESULT where application_no = '" + data.Data.application_no + "'";
                    int n_del = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(sql_del);
                    AppUtility.Engine.LogWriter.Write("插入到历史表C_PBOC_RESULT_HIS成功，受影响行数-->" + n_to_his + "，C_PBOC_RESULT删除数据成功，受影响行数-->" + n_del);
                }
                string sql_insert = "begin\r\n";
                foreach (var v in data.Applicants)
                {
                    sql_insert += string.Format("insert into C_PBOC_RESULT(application_no,res_date,Name,Id_number,pboc_report,pboc_no,app_type) values ('{0}',to_date('{1}','yyyy-mm-dd hh24:mi:ss'),'{2}','{3}','{4}','{5}','{6}');\r\n", data.Data.application_no,
                        data.Data.res_date, v.Name, v.Id_number, v.pboc_report, v.pboc_no, v.app_type);
                }
                sql_insert += "end;";
                int n_insert = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(sql_insert);
                AppUtility.Engine.LogWriter.Write("插入到C_PBOC_RESULT成功，受影响行数-->" + n_insert);
            }
            catch (Exception ex)
            {
                AppUtility.Engine.LogWriter.Write("Save PBOC Data Exception-->申请号" + data.Data.application_no + "，" + ex.ToString());
            }
            return true;
        }
    }
}