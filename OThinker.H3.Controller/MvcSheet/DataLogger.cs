using Newtonsoft.Json;
using OThinker.H3.Data;
using OThinker.H3.DataModel;
using OThinker.H3.WorkflowTemplate;
using OThinker.H3.WorkItem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.MvcSheet
{
    public class DataLogger
    {
        public string DataTrack(MvcPostValue MvcPost, FieldSchema[] fields, SheetDataType sheetDataType,ClientActivity clientActivity)
        {
            var dics = new Dictionary<string, object>();

            // 数据项存在2个情况,1.数据项名称  2.数据项.子数据项
            foreach (FieldSchema field in fields)
            {
                // 如果数据项设置不允许保存，那么不会被保存进去
                if (sheetDataType == SheetDataType.Workflow && !clientActivity.GetItemEditable(field.Name)) continue;
                if (!MvcPost.BizObject.DataItems.ContainsKey(field.Name)) continue;
                if (field.LogicType == DataLogicType.BizObject || field.LogicType == DataLogicType.BizObjectArray) continue;

                if (field.LogicType == DataLogicType.Comment && int.Equals(MvcPost.WorkItemType, (int)WorkItemType.WorkItemAssist))
                {
                    // 协办处理审核意见和审批结果逻辑
                    //SaveCommentData(MvcPost.BizObject, field.Name, MvcResult, MvcPost);
                }
                else if (field.LogicType == DataLogicType.Comment)
                {
                    // 处理审核意见和审批结果逻辑
                    //SaveCommentData(MvcPost.BizObject, field.Name, MvcResult, null);
                }
                else if (field.LogicType == DataLogicType.Attachment)
                {
                    // 处理附件逻辑
                }
                else if (field.LogicType == DataLogicType.MultiParticipant)
                {
                    // 处理多人参与者逻辑
                }
                else
                {   // 处理普通数据项
                    //this.ActionContext.InstanceData[field.Name].Value = MvcPost.BizObject.DataItems[field.Name].V;
                    dics.Add(field.Name, MvcPost.BizObject.DataItems[field.Name].V);
                }
            }

            // 最后保存子表，因为关联关系的子表主键有可能在主表中，所以先保存主表
            foreach (FieldSchema field in fields)
            {
                // 如果数据项设置不允许保存，那么不会被保存进去
                if (sheetDataType == SheetDataType.Workflow && !clientActivity.GetItemEditable(field.Name)) continue;
                if (!MvcPost.BizObject.DataItems.ContainsKey(field.Name)) continue;
                if (field.LogicType != DataLogicType.BizObject && field.LogicType != DataLogicType.BizObjectArray) continue;

                if (field.LogicType == DataLogicType.BizObject)
                {
                    // 业务对象可以使用普通控件显示，例如：业务对象A.Code,可以使用文本框绑定数据项显示 A.Code
                    
                }
                else if (field.LogicType == DataLogicType.BizObjectArray)
                {
                    if (field.Name == "APPLICANT_TYPE") continue;
                    // 记录字表变动数据
                    List<Dictionary<string, object>> result = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(MvcPost.BizObject.DataItems[field.Name].V.ToString());
                    if (result != null && result.Any())
                    {
                        var dicItems = new List<Dictionary<string, object>>();
                        foreach (var item in result)
                        {
                            var dicItem = new Dictionary<string, object>();
                            foreach (var dicKey in item.Keys)
                            {
                                if (dicKey.ToLower() == "objectid") {
                                    continue;
                                }
                                var itemField = field.Schema.Fields.Where(p => p.Name == dicKey).FirstOrDefault();
                                if (itemField != null)
                                {
                                    dicItem.Add(dicKey, item[dicKey]);
                                }
                            }
                            dicItems.Add(dicItem);
                        }
                        dics.Add(field.Name, dicItems);
                    }
                }
            }

            var dataTrack = JsonConvert.SerializeObject(dics, Formatting.Indented);

            return dataTrack;
        }
    }
}
