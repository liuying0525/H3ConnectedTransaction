using OThinker.H3.Acl;
using OThinker.H3.BizBus.BizRule;
using OThinker.H3.Controllers.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Xml;

namespace OThinker.H3.Controllers.Controllers.BizRule
{
    public class BizRuleImportController:ControllerBase
    {
        public override string FunctionCode
        {
            get { return Acl.FunctionNode.Category_BizRule_Code; }
        }


        public JsonResult Import(BizRuleHanderViewModel model)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();
                //业务规则必须以字母开始，不让创建到数据库表字段时报错
                System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex("^[a-zA-Z\\u4e00-\\u9fa5][0-9a-zA-Z\\u4e00-\\u9fa5_]*$");
                if (!regex.Match(model.Code).Success)
                {
                    result.Success = false;
                    result.Message = "EditBizObjectSchemaProperty.Msg4";
                    return Json(result);
                }
                BizRuleTable BizRuleTable = ReadXmlFile(model);
                
                if (BizRuleTable == null)
                {
                    result.Success = false;
                    result.Message = "BizRule.CreateFailed";
                    return Json(result);
                }

                if (string.IsNullOrEmpty(model.Code))
                {
                    result.Success = false;
                    result.Message = "BizRule.EmptyCode";
                    return Json(result);
                }

                bool isSuccess = true;
                string errorMsg = "msgGlobalString.ImportFail";

                BizRuleTable record = this.Engine.BizBus.GetBizRule(model.Code);

                if (record != null)
                {
                    if (model.IsCover)
                    {
                        isSuccess = this.Engine.BizBus.RemoveBizRule(model.Code);
                    }
                    else
                    {
                        result.Success = false;
                        result.Message = "msgGlobalString.CodeDuplicate";
                        return Json(result);
                    } 
                }
                if (isSuccess)
                {
                    BizRuleTable.Code = model.Code;
                    BizRuleTable.DisplayName = model.DisplayName;
                    isSuccess = this.Engine.BizBus.AddBizRule(BizRuleTable, model.ParentCode);
                }
                if (isSuccess)
                {
                    result.Success = true;
                    result.Message = "msgGlobalString.ImportSucceed";
                 
                }
                else
                {
                    result.Success = false;
                    result.Message = errorMsg;
                }

                return Json(result);

            });
        }

        public JsonResult Upload()
        {
            return ExecuteFunctionRun(() =>
            {
                System.Web.HttpFileCollectionBase files = HttpContext.Request.Files;//传输的文件
                ActionResult result = new ActionResult();
                BizRuleHanderViewModel model = new BizRuleHanderViewModel();

                if (files == null || files.Count == 0 || string.IsNullOrEmpty(files[0].FileName))
                {

                    result.Success = false;
                    result.Message = "msgGlobalString.SelectFile";
                    return Json(result, "text/html", JsonRequestBehavior.AllowGet);
                }
                string fileType = Path.GetExtension(TrimHtml(Path.GetFileName(files[0].FileName))).ToLowerInvariant();
                if (!fileType.Replace(".", "").Equals("xml"))
                {
                    result.Success = false;
                    result.Message = "msgGlobalString.FileMustIsXML";
                    return Json(result, "text/html", JsonRequestBehavior.AllowGet);
                }

                //将文件内容存放在缓存中
                string xmlStr = string.Empty;
                using (StreamReader sr = new StreamReader(files[0].InputStream))
                {
                    xmlStr = sr.ReadToEnd();
                }
                string newName = Guid.NewGuid().ToString() + fileType;

                model.FileName = newName;
                model.XMLString = xmlStr;

                //验证是否为伪造的xml文件
                try
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(model.XMLString);
                }
                catch (Exception ex)
                {
                    result.Success = false;
                    result.Message = "msgGlobalString.InvilidateFile";
                    return Json(result, "text/html", JsonRequestBehavior.AllowGet);
                }

                BizRuleTable BizRuleTable = ReadXmlFile( model);//从XML中读取服务名称和编码

                model.Code = BizRuleTable.Code;
                model.DisplayName = BizRuleTable.DisplayName;

                //界面控制，前台控制
                result.Success = true;
                result.Message = "";
                result.Extend = model;//返回上传的BizRule
                return Json(result, "text/html", JsonRequestBehavior.AllowGet);
            });
        }

        [HttpGet]
        public Object DownLoad(string ruleCode) {
            return ExecuteFileResultFunctionRun(() => {
                BizRuleTable RuleTable = this.Engine.BizBus.GetBizRule(ruleCode);
                if (RuleTable == null) return null;
                XmlDocument XmlDoc = new XmlDocument();
                XmlElement bizRule = XmlDoc.CreateElement(FunctionNodeType.BizRule.ToString());
                XmlDoc.AppendChild(bizRule);
                RuleTable.SaveAsXml(XmlDoc, bizRule);

                //导出文件
                string path = Server.MapPath("~/TempImages/");
                string fileName = ruleCode + ".xml";

                XmlDoc.Save(path + fileName);

                //"text/xml"
                return File(path + fileName, "application/octect-stream", fileName);
            });
        }


        private BizRuleTable ReadXmlFile( BizRuleHanderViewModel model)
        {
            XmlDocument xmlDoc = new XmlDocument();

            var xmlString = Server.UrlDecode(model.XMLString).Replace("&nbsp;", " ");
            xmlDoc.LoadXml(xmlString);//根节点

            //数据模型
            XmlNode Node = xmlDoc.DocumentElement.GetElementsByTagName(FunctionNodeType.BizRule.ToString())[0];

            return new BizRuleTable(xmlDoc.DocumentElement);
        }

    }
}
