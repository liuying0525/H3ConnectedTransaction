using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OThinker.H3.Controllers.ViewModels;
using OThinker.H3.Data;

namespace OThinker.H3.Controllers.Controllers.Admin.Handler
{
    public class MasterDataTreeHandler : AbstractPortalTreeHandler
    {
        /// <summary>
        /// 控制器
        /// </summary>
        private ControllerBase _controller;

        protected override ControllerBase controller
        {
            get { return _controller; }
        }

        /// <summary>
        ///构造函数
        /// </summary>
        /// <param name="controller"></param>
        public MasterDataTreeHandler(ControllerBase controller)
        {
            if (_controller == null)
            {
                this._controller = controller;
            }
        }

        public override object CreatePortalTree(string Category)
        {
            List<PortalTreeNode> Tree = new List<PortalTreeNode>();
            Dictionary<string, string> Categorys = _controller.Engine.MetadataRepository.GetCategoryTable();
            foreach (string key in Categorys.Keys)
            {
                if (string.IsNullOrWhiteSpace(Category))
                {
                    Tree.Add(new PortalTreeNode
                    {
                        ObjectID = key,
                        Code = key,
                        IsLeaf = false,
                        Text = Categorys[key],
                        children = GetMetadata(key)
                    });
                }
                else if (key.Equals(Category, StringComparison.InvariantCultureIgnoreCase))
                {
                    Tree.Add(new PortalTreeNode()
                    {
                        ObjectID = Guid.NewGuid().ToString(),
                        Code = key,
                        IsLeaf = false,
                        Text = Categorys[key],
                        children = GetMetadata(key)
                    });
                    break;
                }
            }
            return Tree;
        }

        private List<PortalTreeNode> GetMetadata(string Category)
        {
            List<PortalTreeNode> Tree = new List<PortalTreeNode>();
            Data.EnumerableMetadata[] Metadatas = _controller.Engine.MetadataRepository.GetByCategory(Category);
            foreach (Data.EnumerableMetadata Metadata in Metadatas)
            {
                Tree.Add(new PortalTreeNode()
                {
                    ObjectID = Metadata.ObjectID,
                    Code = Metadata.Code,
                    Text = Metadata.EnumValue,
                    IsLeaf = true
                });
            }
            return Tree;
        }
    }
}
