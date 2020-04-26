using OThinker.H3.Acl;
using OThinker.H3.Controllers.Controllers.Admin.Handler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace OThinker.H3.Controllers.Controllers.Admin.TreeDataHandler
{
    public class WorkflowTreeController:ControllerBase
    {
        #region 常量
        private const string FunctionTypeStr = "FunctionType";
        private const string FunctionIDStr = "FunctionID";
        private const string FunctionCodeStr = "FunctionCode";
        #endregion

        public override string FunctionCode
        {
            get { return ""; }
        }

        public JsonResult CreateWorkflowTree(string FunctionID, string FunctionCode, string FunctionType, string ContainDraft = "false", string IsBizObjectMode = "false", string IsSharedPacket = "false")
        {
            return ExecuteFunctionRun(() => {

                FunctionType= FunctionType?? FunctionNodeType.ProcessModel.ToString();
                FunctionID = FunctionID ?? "";

                AbstractPortalTreeHandler handler = CreateHandler(FunctionType, ContainDraft, IsBizObjectMode, IsSharedPacket);
                object treeObj = handler.CreatePortalTree(FunctionID, FunctionCode);

                return Json(treeObj,JsonRequestBehavior.AllowGet);
            });
        }

        #region 创建对象实例
        private AbstractPortalTreeHandler CreateHandler(string FunctionType, string ContainDraft, string IsBizObjectMode, string IsSharedPacket)
        {
            FunctionNodeType nodeType = (FunctionNodeType)Enum.Parse(typeof(FunctionNodeType), FunctionType);
            switch (nodeType)
            {
                case FunctionNodeType.ProcessModel://流程模型
                case FunctionNodeType.BizWFFolder:
                case FunctionNodeType.BizFolder:
                case FunctionNodeType.BizWorkflowPackage:
                    return new WorkflowHandler(ContainDraft, IsBizObjectMode, IsSharedPacket,this);
                default:
                    return new FunctionHandler(this);
            }
        }
        #endregion
    }
}
