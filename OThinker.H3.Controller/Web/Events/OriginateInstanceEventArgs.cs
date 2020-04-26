using System;
using System.Collections.Generic;
using System.Text;

namespace OThinker.H3.Controllers
{
    [System.Serializable]
    public class OriginateInstanceEventArgs : EventArgs
    {
        public OriginateInstanceEventArgs(string InstanceId, Dictionary<string, object> ParameterTable)
        {
            this._InstanceId = InstanceId;
            this._InstanceParamterTable = ParameterTable;
            if (this._InstanceParamterTable == null)
            {
                this._InstanceParamterTable = new Dictionary<string, object>();
            }
        }

        private string _InstanceId;
        /// <summary>
        /// 流程Id
        /// </summary>
        public string InstanceId
        {
            get
            {
                return this._InstanceId;
            }
        }

        private Dictionary<string, object> _InstanceParamterTable = null;
        /// <summary>
        /// 流程发起的时候将带这个参数
        /// </summary>
        public Dictionary<string, object> InstanceParameterTable
        {
            get
            {
                return this._InstanceParamterTable;
            }
        }
    }
}
