using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OThinker.H3.Controllers;
using OThinker.H3.DataModel;
using OThinker.H3.Instance;
using OThinker.H3.Data;
using OThinker.Organization;
using System.Data;


namespace DongZheng.H3.WebApi.Common.Portal
{
    public class Common
    {
        public static object LoadMvcInstanceData(string InsID, string SchemaCode)
        {
            InstanceContext context = AppUtility.Engine.InstanceManager.GetInstanceContext(InsID);
            if (context == null)
                return new { Success = false, Data = "", Msg = "InstanceID错误或不存在" };
            if (SchemaCode != context.BizObjectSchemaCode)
            {
                return new { Success = false, Data = "", Msg = "SchemaCode不一致，不能进行复制" };
            }
            var schema = AppUtility.Engine.BizObjectManager.GetPublishedSchema(context.BizObjectSchemaCode);

            var fields = schema.Fields;
            MvcDataItemTable items = new MvcDataItemTable();
            InstanceData data = new InstanceData(AppUtility.Engine, InsID, "");
            string[] sys_fields = new string[] { "ObjectID", "Name", "CreatedBy", "CreatedByParentId", "OwnerId", "OwnerParentId", "CreatedTime", "ModifiedBy", "ModifiedTime", "RunningInstanceId" };
            foreach (FieldSchema field in fields)
            {
                if (items.ContainsKey(field.Name))
                {
                    continue;
                }
                if (sys_fields.Contains(field.Name))
                {
                    continue;
                }
                MvcDataItem dataItem = new MvcDataItem(data[field.Name], false);
                switch (field.LogicType)
                {
                    case DataLogicType.BizObject: // 业务对象
                        dataItem.V = LoadBizObjectData(data, field);
                        break;
                    case DataLogicType.BizObjectArray://业务对象数组
                        dataItem.V = LoadBizObjectArrayData(data, field);
                        break;
                    //case DataLogicType.Comment://处理审批意见逻辑
                    //    dataItem.V = LoadCommentData(field.Name);
                    //    break;
                    //case DataLogicType.Attachment://处理附件逻辑
                    //    dataItem.V = LoadAttachmentData(field);
                    //    break;
                    case DataLogicType.MultiParticipant://处理多人参与者逻辑
                        dataItem.V = LoadMultiParticipantData(data, field);
                        break;
                    case DataLogicType.SingleParticipant://处理单人参与者逻辑
                        string v = data[field.Name].Value + string.Empty;
                        if (!string.IsNullOrEmpty(v))
                        {
                            OThinker.Organization.Unit unit = AppUtility.Engine.Organization.GetUnit(v);
                            if (unit != null)
                            {
                                string type = string.Empty;
                                if (unit.UnitType == OThinker.Organization.UnitType.OrganizationUnit)
                                    type = "O";
                                else if (unit.UnitType == OThinker.Organization.UnitType.Group)
                                    type = "G";
                                else if (unit.UnitType == OThinker.Organization.UnitType.User)
                                    type = "U";
                                // dataItem.V = new MvcListItem(v, this.ActionContext.Engine.Organization.GetName(v));
                                dataItem.V = new MvcListItem(v, unit.Name, null, 0, type);
                            }
                        }
                        break;
                        //case DataLogicType.Association:
                        //    dataItem.V = LoadAssociationData(field, dataItem.V + string.Empty);
                        //    break;
                }
                items.Add(field.Name, dataItem);
            }
            return new { Success = true, Data = items, Msg = "Sucess" };
        }

        private static MvcBizObject LoadBizObjectData(InstanceData Data, FieldSchema field)
        {
            BizObject bo = Data[field.Name].Value as BizObject;
            MvcBizObject mbo = new MvcBizObject();
            if (bo == null)
            {
                BizObjectSchema schema = field.Schema;
                if (schema == null)
                {
                    schema = AppUtility.Engine.BizObjectManager.GetPublishedSchema(field.ChildSchemaCode);
                }
                bo = new BizObject(AppUtility.Engine, schema, "");
            }
            foreach (PropertySchema p in bo.Schema.Properties)
            {
                mbo.DataItems.Add(field.Name + "." + p.Name, GetMvcDataFromProperty(field, p, bo));
            }
            return mbo;
        }
        private static object LoadBizObjectArrayData(InstanceData Data, FieldSchema field)
        {
            OThinker.H3.DataModel.BizObject[] boValues = GetBOsValue(Data.BizObject, field);
            MvcDataItem dataItem = new MvcDataItem(Data[field.Name], false);
            MvcBizObjectList mboList = new MvcBizObjectList();

            if (boValues != null)
            {
                foreach (BizObject bo in boValues)
                {
                    MvcBizObject mbo = new MvcBizObject();
                    foreach (PropertySchema p in bo.Schema.Properties)
                    {
                        MvcDataItem item = GetMvcDataFromProperty(field, p, bo);
                        mbo.DataItems.Add(field.Name + "." + p.Name, item);
                    }
                    mboList.Add(mbo);
                }
            }
            object V = new { R = mboList };
            return V;
        }

        //private object LoadCommentData(string datafield)
        //{
        //    //审批意见
        //    Comment[] comments;
        //    Unit[] units;
        //    //常用意见
        //    List<string> frequentlyUsedComments;
        //    //一次从Engine获取方法需要的数据
        //    AppUtility.Engine.Interactor.LoadCommentData(
        //        this.ActionContext.SchemaCode,
        //        this.ActionContext.BizObjectID,
        //        this.ActionContext.InstanceId,
        //        this.ActionContext.WorkItem == null ? null : this.ActionContext.WorkItem.WorkItemID,
        //        datafield,
        //        datafield == SheetConsultComment ? this.ActionContext.User.UserID : null,
        //        null,
        //        this.ActionContext.User.UserID,
        //        out comments,
        //        out units,
        //        out frequentlyUsedComments);
        //    AppUtility.Engine.BizObjectManager.GetCommentsByBizObject("", "", "");

        //    List<object> historyComments = null;
        //    if (comments != null)
        //    {
        //        historyComments = new List<object>();
        //        foreach (Comment comment in comments)
        //        {
        //            OThinker.Organization.User user = null;
        //            OThinker.Organization.User delegant = null;
        //            foreach (OThinker.Organization.Unit unit in units)
        //            {
        //                if (unit.ObjectID == comment.UserID)
        //                {
        //                    user = unit as OThinker.Organization.User;
        //                }
        //                if (unit.ObjectID == comment.Delegant)
        //                {
        //                    delegant = unit as OThinker.Organization.User;
        //                }
        //            }

        //            // 获取用户图片
        //            string imagePath = UserValidator.GetUserImagePath(AppUtility.Engine, delegant == null ? user : delegant, TempImagesPath);
        //            string avatar = "";
        //            if (user.Gender == Organization.UserGender.Male)
        //            {
        //                avatar = AppUtility.PortalRoot + "/img/TempImages/usermale.jpg";
        //            }
        //            else if (user.Gender == Organization.UserGender.Female)
        //            {
        //                avatar = AppUtility.PortalRoot + "/img/TempImages/userfemale.jpg";
        //            }
        //            else
        //            {
        //                avatar = AppUtility.PortalRoot + "/img/user.jpg";
        //            }

        //            if (!string.IsNullOrEmpty(imagePath))
        //            {
        //                avatar = this.GetImageUrlPath(imagePath) + "?" + DateTime.Now.ToString("yyyyMMddHHmmss");
        //            }

        //            WorkflowTemplate.Activity Activity = this.ActionContext.Workflow.GetActivityByCode(comment.Activity);
        //            bool isMyComment = false;
        //            if (this.ActionContext.WorkItem != null && this.ActionContext.WorkItem.TokenId == comment.TokenId)
        //            {// A委托给B，那么A填的意见B能改，B填的意见A也能改
        //                isMyComment = comment.UserID == this.ActionContext.WorkItem.Participant;
        //            }
        //            string itemAction = string.Empty;
        //            string itemAction2 = string.Empty;
        //            string OriGinatorName = string.Empty;
        //            InstanceDetailController instanceDetail = new InstanceDetailController();
        //            OThinker.H3.WorkItem.WorkItem workitem = AppUtility.Engine.WorkItemManager.GetWorkItem(comment.WorkItemId);
        //            if (workitem != null)
        //            {
        //                // 类型
        //                WorkItem.WorkItemType itemType = (WorkItem.WorkItemType)OThinker.Data.Convertor.Convert<int>(workitem.ItemType);
        //                itemAction = instanceDetail.GetItemAction(itemType);
        //                WorkItem.WorkItemType consulted = (WorkItem.WorkItemType)OThinker.Data.Convertor.Convert<int>(workitem.Consulted);
        //                itemAction2 = instanceDetail.GetItemAction(consulted);
        //                if (!string.IsNullOrWhiteSpace(comment.WorkItemId) && string.Equals(itemAction, Configs.Global.ResourceManager.GetString("SheetActionPane_Assist")))
        //                {
        //                    DataTable dt = AppUtility.Engine.Query.GetWorkItemCreator(comment.WorkItemId);
        //                    if (dt != null && dt.Rows.Count > 0)
        //                    {
        //                        Unit unit = AppUtility.Engine.Organization.GetUnit(dt.Rows[0]["Creator"].ToString().Trim());
        //                        if (unit != null) OriGinatorName = unit.Name + "的" + itemAction;
        //                    }
        //                }
        //                if (string.Equals(itemAction2, Configs.Global.ResourceManager.GetString("WorkItemType_Consult")))
        //                {
        //                    Unit _unit = AppUtility.Engine.Organization.GetUnit(comment.ConsultInitiator);
        //                    if (_unit != null) OriGinatorName = _unit.Name + "的" + Configs.Global.ResourceManager.GetString("WorkItemType_Consult");
        //                }
        //            }
        //            historyComments.Add(new
        //            {
        //                CommentID = comment.CommentID,
        //                Avatar = Page.ResolveUrl(avatar),                                                        // 用户图片
        //                OUName = comment.OUName,                                                                 // 意见审批人所属OU
        //                UserName = comment.UserName,                                                             // 意见审批人
        //                DelegantName = comment.Delegant == comment.UserID ? string.Empty : comment.DelegantName, // 代理人
        //                Activity = Activity == null ? comment.Activity : Activity.DisplayName,
        //                DateStr = comment.ModifiedTime.ToShortDateString() + " " + comment.ModifiedTime.ToShortTimeString(),
        //                Text = comment.Text,
        //                IsMyComment = isMyComment,
        //                SignatureId = comment.SignatureId,
        //                Approval = comment.Approval,
        //                ParentPropertyName = comment.ParentPropertyText,
        //                itemAction = itemAction,
        //                OriGinatorName = OriGinatorName.Trim()
        //            });

        //            CheckSignatureImage(comment.SignatureId);
        //        }
        //    }

        //    return new { Comments = historyComments, FrequentlyUsedComments = frequentlyUsedComments };
        //}

        //private List<MvcListItem> LoadAttachmentData(FieldSchema field)
        //{
        //    List<MvcListItem> listItems = new List<MvcListItem>();
        //    AttachmentHeader[] headers = this.ActionContext.Engine.BizObjectManager.QueryAttachment(
        //         this.ActionContext.SchemaCode,
        //         this.ActionContext.BizObjectID,
        //         field.Name,
        //         OThinker.Data.BoolMatchValue.True,
        //         null);
        //    if (headers != null)
        //    {
        //        foreach (AttachmentHeader header in headers)
        //        {
        //            string url = string.IsNullOrEmpty(header.DownloadUrl) ? AppConfig.GetReadAttachmentUrl(
        //                this.ActionContext.IsMobile,
        //                this.ActionContext.Schema.SchemaCode,
        //                this.ActionContext.BizObjectID,
        //                header.ObjectID,
        //                AttachmentOpenMethod.Download) : header.DownloadUrl;

        //            listItems.Add(new MvcListItem(header.ObjectID, header.FileName, url, header.ContentLength, header.ContentType));
        //        }
        //    }
        //    return listItems;
        //}
        private static OThinker.H3.DataModel.BizObject[] GetBOsValue(BizObject BizObject, FieldSchema field)
        {
            OThinker.H3.DataModel.BizObject[] objs = null;
            object v = BizObject[field.Name];
            if (v == null)
            {
                objs = null;
            }
            else if (v is OThinker.H3.DataModel.BizObject)
            {
                objs = new OThinker.H3.DataModel.BizObject[] { (OThinker.H3.DataModel.BizObject)v };
            }
            else if (v is OThinker.H3.DataModel.BizObject[])
            {
                objs = (OThinker.H3.DataModel.BizObject[])v;
            }
            if (!field.IsProperty)
            {// 关联对象，存储到 Session 中，因为 ObjectID 不是固定的
                // 可能需要设计将业务字段赋值给 ObjectID
                HttpContext.Current.Session[BizObject.ObjectID + field.Name] = objs;
            }
            return objs;
        }
        private static List<MvcListItem> LoadMultiParticipantData(InstanceData Data, FieldSchema field)
        {
            List<MvcListItem> listItems = null;
            string[] ids = Data[field.Name].Value as string[];
            if (ids != null)
            {
                listItems = new List<MvcListItem>();
                List<OThinker.Organization.Unit> units = AppUtility.Engine.Organization.GetUnits(ids);
                if (units != null)
                {
                    foreach (string user in ids)
                    {
                        string name = string.Empty;
                        string type = string.Empty;
                        foreach (OThinker.Organization.Unit u in units)
                        {
                            if (u.ObjectID == user)
                            {
                                name = u.Name;
                                if (u.UnitType == OThinker.Organization.UnitType.OrganizationUnit)
                                {
                                    type = "O";
                                }
                                else if (u.UnitType == OThinker.Organization.UnitType.Group)
                                {
                                    type = "G";
                                }
                                else if (u.UnitType == OThinker.Organization.UnitType.User)
                                {
                                    type = "U";
                                }
                                break;
                            }
                        }
                        //listItems.Add(new MvcListItem(user, name));
                        listItems.Add(new MvcListItem(user, name, null, 0, type));
                    }
                }
            }
            return listItems;
        }
        private static MvcDataItem GetMvcDataFromProperty(FieldSchema field, PropertySchema Property, BizObject bo)
        {
            object Value = bo[Property.Name];
            MvcDataItem dataItem = new MvcDataItem()
            {
                N = Property.DisplayName,
                L = Property.LogicType,
                O = "",
                V = Value
            };

            if (Property.LogicType == DataLogicType.SingleParticipant)
            {
                if (!string.IsNullOrEmpty(Value + string.Empty))
                {
                    dataItem.V = new MvcListItem(Value + string.Empty, AppUtility.Engine.Organization.GetName(Value + string.Empty));
                }
            }
            else if (Property.LogicType == DataLogicType.MultiParticipant)
            {
                List<MvcListItem> listItems = null;
                string[] ids = Value as string[];
                if (ids != null)
                {
                    listItems = new List<MvcListItem>();
                    List<OThinker.Organization.Unit> units = AppUtility.Engine.Organization.GetUnits(ids);
                    if (units != null)
                    {
                        foreach (string user in ids)
                        {
                            string name = string.Empty;
                            foreach (OThinker.Organization.Unit u in units)
                            {
                                if (u.ObjectID == user)
                                {
                                    name = u.Name;
                                    break;
                                }
                            }
                            listItems.Add(new MvcListItem(user, name));
                        }
                    }
                }
                dataItem.V = listItems;
            }
            else if (Property.LogicType == DataLogicType.Attachment)
            {
                List<MvcListItem> listItems = new List<MvcListItem>();
                AttachmentHeader[] headers = AppUtility.Engine.BizObjectManager.QueryAttachment(
                     Property.Name,
                     bo.ObjectID,
                     field.Name + "." + Property.Name,
                     OThinker.Data.BoolMatchValue.True,
                     null);
                if (headers != null)
                {
                    foreach (AttachmentHeader header in headers)
                    {
                        string url = AppConfig.GetReadAttachmentUrl(
                            false,
                            Property.Name,
                            bo.ObjectID,
                            header.ObjectID,
                            AttachmentOpenMethod.Download);
                        listItems.Add(new MvcListItem(header.ObjectID, header.FileName, url, header.ContentLength, header.ContentType));
                    }
                }
                dataItem.V = listItems;
            }

            return dataItem;
        }
    }
}