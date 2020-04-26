using System;
using System.Collections.Generic;
using System.Text;
using OThinker.H3;
using System.Data;
using OThinker.H3.Instance;
using EventHandler;

namespace OThinker.H3.EventHandlers
{
    public class WorkItemSyncHandler : IWorkItemEventHandler
    {
        HttpHelper hh = new HttpHelper();
        public WorkItemSyncHandler()
        {

        }

        public void OnCreated(IEngine Engine, WorkItem.WorkItem WorkItem)
        {
            string url = Engine.SettingManager.GetCustomSetting("HttpPostCreatedURL");
            if (!string.IsNullOrEmpty(url))
            {
                Engine.LogWriter.Write("EventHandler OnCreated " + WorkItem.ObjectID + ",State:" + WorkItem.State.ToString());
                hh.GetWebRequest(url + "/" + WorkItem.WorkItemID);
            }
        }

        public void OnUpdated(IEngine Engine, WorkItem.WorkItem WorkItem)
        {
            string url = Engine.SettingManager.GetCustomSetting("HttpPostUpdatedURL");
            if (!string.IsNullOrEmpty(url))
            {
                Engine.LogWriter.Write("EventHandler OnUpdated " + WorkItem.ObjectID);
                hh.GetWebRequest(url + "/" + WorkItem.ObjectID);
            }
        }

        /// <summary>
        /// 任务删除，或者待办任务完成后，都会执行Removed方法
        /// </summary>
        /// <param name="Engine"></param>
        /// <param name="WorkItemId"></param>
        public void OnRemoved(IEngine Engine, string WorkItemId)
        {
            string url = Engine.SettingManager.GetCustomSetting("HttpPostRemovedURL");
            if (!string.IsNullOrEmpty(url))
            {
                Engine.LogWriter.Write("EventHandler OnRemoved " + WorkItemId);
                hh.GetWebRequest(url + "/" + WorkItemId);
            }
        }
    }
}
