using OThinker.H3.Acl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.ViewModels
{
    public class ProcessFolderViewModel:ViewModelBase
    {

        public string Name { get; set; }

        public string Code { get; set; }

        public string ParentID { get; set; }

        public string ParentCode { get; set; }

        /// <summary>
        /// 父节点名称 
        /// </summary>
        public string ParentName { get; set; }

        public int SortKey { get; set; }

        /// <summary>
        /// 文件夹节点类型
        /// </summary>
        public FunctionNodeType ObjectType { get; set; }
    }
}
