using System;

namespace OThinker.H3.Controllers
{
    /// <summary>
    /// 堆数据集合
    /// </summary>
    public class HeapDataItemCollection
    {
        private string InstanceId;
        private H3.Instance.IHeapDataManager HeapDataManager;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="Engine"></param>
        /// <param name="InstanceId"></param>
        public HeapDataItemCollection(IEngine Engine, string InstanceId)
        {
            this.HeapDataManager = Engine.HeapDataManager;
            this.InstanceId = InstanceId;
        }

        /// <summary>
        /// 根据名称获取
        /// </summary>
        /// <param name="ItemName"></param>
        /// <returns></returns>
        public object this[string ItemName]
        {
            get
            {
                return this.HeapDataManager.GetItemValue(this.InstanceId, ItemName);
            }
            set
            {
                this.HeapDataManager.SetItemValue(this.InstanceId, ItemName, value);
            }
        }

    }
}
