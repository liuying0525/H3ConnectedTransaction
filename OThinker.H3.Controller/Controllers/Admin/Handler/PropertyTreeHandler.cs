using OThinker.H3.DataModel;
using OThinker.H3.Instance.Keywords;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;

namespace OThinker.H3.Controllers.Controllers.Admin.Handler
{
    public class PropertyTreeHandler
    {
        private ControllerBase _controller;

        public PropertyTreeHandler(ControllerBase controller)
        {
            this._controller = controller;
        }

        public List<ComboxTree> GetPropertyTree(string schemaCode)
        {
            ComboxTree root = new ComboxTree()
            {
                text = "PropertyTreeHandler.DataItem",
                children = new List<ComboxTree>()
            };
            // 系统数据项
            root.children.Add(new ComboxTree()
            {
                text = "PropertyTreeHandler.SystemDataItem",
                children = new List<ComboxTree>()
            });

            TreeNode systemNode = this.GetInstanceDataTreeNode();
            AddSystemTreeNode(root.children[0], systemNode);

            // 流程数据项
            root.children.Add(new ComboxTree()
            {
                text = "PropertyTreeHandler.WorkflowDataItem",
                children = new List<ComboxTree>()
            });
            BizObjectSchema schema = _controller.Engine.BizObjectManager.GetPublishedSchema(schemaCode);
            if (schema != null)
            {
                PropertySchema[] schemas = schema.Properties;
                if (schemas != null)
                {
                    foreach (PropertySchema field in schemas)
                    {
                        root.children[1].children.Add(new ComboxTree()
                        {
                            id = field.Name,
                            text = field.FullName
                        });
                    }
                }
            }
            List<ComboxTree> results = new List<ComboxTree>();
            results.Add(root);
            return results;
        }

        #region 从KeyWordNode拷贝到TreeNode ------------
        /// <summary>
        /// 获取流程系统数据项树节点
        /// </summary>
        /// <returns></returns>
        protected TreeNode GetInstanceDataTreeNode()
        {
            KeyWordNode KeyWordNode = OThinker.H3.Instance.Keywords.ParserFactory.GetDataTreeNode();
            TreeNode systemNode = new TreeNode(KeyWordNode.Text, KeyWordNode.Text);
            CopyTreeNode(KeyWordNode, systemNode);
            return systemNode;
        }

        /// <summary>
        /// 从KeyWordNode拷贝到TreeNode
        /// </summary>
        /// <param name="fromParentNode"></param>
        /// <param name="toParentNode"></param>
        private void CopyTreeNode(KeyWordNode fromParentNode, TreeNode toParentNode)
        {
            if (toParentNode != null && fromParentNode != null)
            {
                if (fromParentNode.ChildNodes.Count == 0)
                {
                    //toParentNode.ChildNodes.Add(new TreeNode(fromParentNode.Text, fromParentNode.Text));
                }
                else
                {
                    foreach (KeyWordNode fromChild in fromParentNode.ChildNodes)
                    {
                        TreeNode toChild = new TreeNode(fromChild.Text, fromChild.Text);
                        toParentNode.ChildNodes.Add(toChild);
                        CopyTreeNode(fromChild, toChild);
                    }
                }
            }
        }

        #endregion
        private void AddSystemTreeNode(ComboxTree treeNode, System.Web.UI.WebControls.TreeNode systemNode)
        {
            foreach (System.Web.UI.WebControls.TreeNode node in systemNode.ChildNodes)
            {
                ComboxTree t = new ComboxTree()
                {
                    id = node.Value,
                    text = node.Text
                };
                if (node.ChildNodes.Count > 0)
                {
                    t.children = new List<ComboxTree>();
                    AddSystemTreeNode(t, node);
                }
                treeNode.children.Add(t);
            }
        }

        public class ComboxTree
        {
            private string _id = string.Empty;
            public string id
            {
                get { return this._id; }
                set { this._id = value; }
            }

            private string _text = string.Empty;
            public string text
            {
                get { return this._text; }
                set { this._text = value; }
            }

            private List<ComboxTree> _children = null;
            public List<ComboxTree> children
            {
                get { return this._children; }
                set { this._children = value; }
            }
        }
    }
}
