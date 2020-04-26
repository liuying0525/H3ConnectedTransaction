using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace OThinker.H3.Controllers
{
    /// <summary>
    /// SQL 语句事件
    /// </summary>
    public class EditorColumnTrackEventArgs : EventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        public EditorColumnTrackEventArgs()
        {
        }

        private string instanceId = null;
        /// <summary>
        /// 获取或设置流程实例ID
        /// </summary>
        public string InstanceID
        {
            get
            {
                return this.instanceId;
            }
            set
            {
                this.instanceId = value;
            }
        }

        private string parentObjectID = null;
        /// <summary>
        /// 获取或设置子表所在行的 ObjectID
        /// </summary>
        public string ParentObjectID
        {
            get
            {
                return this.parentObjectID;
            }
            set
            {
                this.parentObjectID = value;
            }
        }

        private string activityName = null;
        /// <summary>
        /// 获取或设置流程节点名称
        /// </summary>
        public string ActivityName
        {
            get
            {
                return this.activityName;
            }
            set
            {
                this.activityName = value;
            }
        }

        private string _ModifiedBy = null;
        /// <summary>
        /// 获取或设置编辑用户ID
        /// </summary>
        public string ModifiedBy
        {
            get
            {
                return this._ModifiedBy;
            }
            set
            {
                this._ModifiedBy = value;
            }
        }

        private DateTime _ModifiedTime = DateTime.Now;
        /// <summary>
        /// 获取或设置修改时间
        /// </summary>
        public DateTime ModifiedTime
        {
            get
            {
                return this._ModifiedTime;
            }
            set
            {
                this._ModifiedTime = value;
            }
        }

        private string tableName = null;
        /// <summary>
        /// 获取或设置子表名称
        /// </summary>
        public string TableName
        {
            get
            {
                return this.tableName;
            }
            set
            {
                this.tableName = value;
            }
        }

        private string columnName = null;
        /// <summary>
        /// 获取或设置子表字段名称
        /// </summary>
        public string ColumnName
        {
            get
            {
                return this.columnName;
            }
            set
            {
                this.columnName = value;
            }
        }

        private string columnValue = null;
        /// <summary>
        /// 获取或设置子表值
        /// </summary>
        public string ColumnValue
        {
            get
            {
                return this.columnValue;
            }
            set
            {
                this.columnValue = value;
            }
        }

    }
}
