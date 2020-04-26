using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OThinker.H3.Controllers
{
    /// <summary>
    /// MVC返回前端数据
    /// </summary>
    public class PermittedActions
    {
        /// <summary>
        /// 获取或设置是否允许调整活动参与者
        /// </summary>
        public bool AdjustParticipant
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置是否允许协办
        /// </summary>
        public bool Assist
        {
            get;
            set;
        }

        private bool _CancelInstance = false;
        /// <summary>
        /// 获取或设置是否提交前允许取消流程
        /// </summary>
        public bool CancelInstance
        {
            get
            {
                return this._CancelInstance;
            }
            set
            {
                this._CancelInstance = value;
            }
        }

        private bool _RetrieveInstance;
        /// <summary>
        /// 取回流程
        /// </summary>
        public bool RetrieveInstance
        {
            get
            {
                return this._RetrieveInstance;
            }
            set
            {
                this._RetrieveInstance = value;
            }
        }

        /// <summary>
        /// 获取或设置是否允许传阅
        /// </summary>
        public bool Circulate
        {
            get;
            set;
        }

        /// <summary>
        /// 已阅
        /// </summary>
        public bool Viewed
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置是否允许再传阅
        /// </summary>
        public bool Recirculate
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置是否允许征询
        /// </summary>
        public bool Consult
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置是否允许结束流程
        /// </summary>
        public bool FinishInstance
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置是否允许转发
        /// </summary>
        public bool Forward
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置是否允许预览
        /// </summary>
        public bool PreviewParticipant
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置是否允许驳回
        /// </summary>
        public bool Reject
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置是否显示关闭按钮
        /// </summary>
        public bool Close
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置是否显示打印按钮
        /// </summary>
        public bool Print
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置是否显示保存按钮
        /// </summary>
        public bool Save
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置是否允许提交
        /// </summary>
        public bool Submit
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置是否可以查看流程状态
        /// </summary>
        public bool ViewInstance
        {
            get;
            set;
        }

        private bool _LockInstance = false;
        /// <summary>
        /// 获取或设置是否允许锁定
        /// </summary>
        public bool LockInstance
        {
            get
            {
                return this._LockInstance;
            }
            set
            {
                this._LockInstance = value;
            }
        }

        private bool _UnLockInstance = false;
        /// <summary>
        /// 获取或设置是否允许解除锁定
        /// </summary>
        public bool UnLockInstance
        {
            get
            {
                return this._UnLockInstance;
            }
            set
            {
                this._UnLockInstance = value;
            }
        }

        private bool _Choose = false;
        /// <summary>
        /// 获取或设置是否允许手工选择
        /// </summary>
        public bool Choose
        {
            get
            {
                return this._Choose;
            }
            set
            {
                this._Choose = value;
            }
        }


    }
}
