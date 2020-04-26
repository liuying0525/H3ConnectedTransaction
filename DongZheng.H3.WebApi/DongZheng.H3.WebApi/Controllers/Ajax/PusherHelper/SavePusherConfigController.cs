using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;

namespace DongZheng.H3.WebApi.Controllers.Ajax.PusherHelper
{
    [ValidateInput(false)]
    [Xss]
    public class SavePusherConfigController : OThinker.H3.Controllers.ControllerBase
    {
        public override string FunctionCode => "";

        public void Index()
        {
            var context = HttpContext;
            JObject jobject = new JObject();
            using (StreamReader sr = new StreamReader(context.Request.InputStream))
            {
                string data = sr.ReadToEnd();
                jobject = JObject.Parse(data);
            }
            XmlDocument doc = new XmlDocument();
            XmlElement root = doc.CreateElement("configuration");
            XmlElement PushInterval = doc.CreateElement("PushInterval");
            PushInterval.InnerText = jobject["PushInterval"].ToString();
            root.AppendChild(PushInterval);
            XmlElement Engine = doc.CreateElement("Engine");
            Engine.InnerText = jobject["Engine"].ToString();
            root.AppendChild(Engine);
            XmlElement PushPointTime = doc.CreateElement("PushPointTime");
            string dateStr = "";
            switch (jobject["PushInterval"].ToString())
            {
                case "0":
                    dateStr = DateTime.Now.ToString("yyyy/MM/dd ") + jobject["PushPointTime"].ToString() + ":00:00";
                    break;
                case "1":
                    dateStr = DateTime.Now.ToString("yyyy/MM/") + jobject["PushPointTime"].ToString() + ":00:00";
                    break;
                case "2":
                    dateStr = DateTime.Now.ToString("yyyy/") + jobject["PushPointTime"].ToString() + ":00:00";
                    break;
            }
            PushPointTime.InnerText = dateStr;
            root.AppendChild(PushPointTime);
            XmlElement PushUrl = doc.CreateElement("PushUrl");
            PushUrl.InnerText = jobject["PushUrl"].ToString();
            root.AppendChild(PushUrl);
            XmlElement StructNames = doc.CreateElement("StructNames");
            XmlElement GroupNames = doc.CreateElement("GroupNames");
            XmlElement GroupId = doc.CreateElement("ObjectId");
            GroupId.InnerText = jobject["StructNames"]["GroupNames"]["ObjectId"].ToString();
            GroupNames.AppendChild(GroupId);
            XmlElement DealerType = doc.CreateElement("DealerType");
            DealerType.InnerText = jobject["StructNames"]["GroupNames"]["DealerType"].ToString();
            GroupNames.AppendChild(DealerType);
            XmlElement DealerIntranet = doc.CreateElement("DealerIntranet");
            DealerIntranet.InnerText = jobject["StructNames"]["GroupNames"]["DealerIntranet"].ToString();
            GroupNames.AppendChild(DealerIntranet);
            StructNames.AppendChild(GroupNames);
            XmlElement ScoreNames = doc.CreateElement("ScoreNames");
            XmlElement ScoreId = doc.CreateElement("ObjectId");
            ScoreId.InnerText = jobject["StructNames"]["ScoreNames"]["ObjectId"].ToString();
            ScoreNames.AppendChild(ScoreId);
            XmlElement ScoreName = doc.CreateElement("ScoreName");
            ScoreName.InnerText = jobject["StructNames"]["ScoreNames"]["ScoreName"].ToString();
            ScoreNames.AppendChild(ScoreName);
            XmlElement Weight = doc.CreateElement("Weight");
            Weight.InnerText = jobject["StructNames"]["ScoreNames"]["Weight"].ToString();
            ScoreNames.AppendChild(Weight);
            XmlElement Expression = doc.CreateElement("Expression");
            Expression.InnerText = jobject["StructNames"]["ScoreNames"]["Expression"].ToString();
            ScoreNames.AppendChild(Expression);
            StructNames.AppendChild(ScoreNames);
            XmlElement DetailNames = doc.CreateElement("DetailNames");
            XmlElement DetailId = doc.CreateElement("ObjectId");
            DetailId.InnerText = jobject["StructNames"]["DetailNames"]["ObjectId"].ToString();
            DetailNames.AppendChild(DetailId);
            XmlElement Rate = doc.CreateElement("Rate");
            Rate.InnerText = jobject["StructNames"]["DetailNames"]["Rate"].ToString();
            DetailNames.AppendChild(Rate);
            StructNames.AppendChild(DetailNames);
            XmlElement OptionNames = doc.CreateElement("OptionNames");
            XmlElement OptionId = doc.CreateElement("ObjectId");
            OptionId.InnerText = jobject["StructNames"]["OptionNames"]["ObjectId"].ToString();
            OptionNames.AppendChild(OptionId);
            XmlElement Memo = doc.CreateElement("Memo");
            Memo.InnerText = jobject["StructNames"]["OptionNames"]["Memo"].ToString();
            OptionNames.AppendChild(Memo);
            XmlElement Value = doc.CreateElement("Value");
            Value.InnerText = jobject["StructNames"]["OptionNames"]["Value"].ToString();
            OptionNames.AppendChild(Value);
            StructNames.AppendChild(OptionNames);
            XmlElement GradeNames = doc.CreateElement("GradeNames");
            XmlElement ObjectId = doc.CreateElement("ObjectId");
            ObjectId.InnerText = jobject["StructNames"]["GradeNames"]["ObjectId"].ToString();
            GradeNames.AppendChild(ObjectId);
            XmlElement PID = doc.CreateElement("GroupId");
            PID.InnerText = jobject["StructNames"]["GradeNames"]["GroupId"].ToString();
            GradeNames.AppendChild(PID);
            XmlElement Grade = doc.CreateElement("Grade");
            Grade.InnerText = jobject["StructNames"]["GradeNames"]["Grade"].ToString();
            GradeNames.AppendChild(Grade);
            XmlElement RangeFrom = doc.CreateElement("RangeFrom");
            RangeFrom.InnerText = jobject["StructNames"]["GradeNames"]["RangeFrom"].ToString();
            GradeNames.AppendChild(RangeFrom);
            XmlElement RangeTo = doc.CreateElement("RangeTo");
            RangeTo.InnerText = jobject["StructNames"]["GradeNames"]["RangeTo"].ToString();
            GradeNames.AppendChild(RangeTo);
            StructNames.AppendChild(GradeNames);
            root.AppendChild(StructNames);
            foreach (JObject token in jobject["Views"])
            {
                XmlElement Views = doc.CreateElement("DataViews");
                XmlElement SelectStr = doc.CreateElement("SelectStr");
                SelectStr.InnerText = token["SelectStr"].ToString();
                Views.AppendChild(SelectStr);
                XmlElement DealerColumn = doc.CreateElement("DealerColumn");
                DealerColumn.InnerText = token["DealerColumn"].ToString();
                Views.AppendChild(DealerColumn);
                XmlElement DealerName = doc.CreateElement("ViewDealerName");
                DealerName.InnerText = token["DealerName"].ToString();
                Views.AppendChild(DealerName);
                XmlElement ViewDealerType = doc.CreateElement("ViewDealerType");
                ViewDealerType.InnerText = token["ViewDealerType"].ToString();
                Views.AppendChild(ViewDealerType);
                XmlElement ViewDealerKind = doc.CreateElement("ViewDealerKind");
                ViewDealerKind.InnerText = token["ViewDealerKind"].ToString();
                Views.AppendChild(ViewDealerKind);
                root.AppendChild(Views);
            }
            doc.AppendChild(root);
            doc.Save(System.Configuration.ConfigurationManager.AppSettings["PusherConfigFile"]);
            context.Response.Write("配置文件保存成功");
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}