using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace PushGradeFromH3ToCrm
{
    /// <summary>
    /// 可配置视图类
    /// </summary>
    public class View
    {
        /// <summary>
        /// 视图名
        /// </summary>
        public string ViewName { get; set; }
        /// <summary>
        /// 经销商类型字段名
        /// </summary>
        public string DealerType { get; set; }
        /// <summary>
        /// 经销商性质字段名
        /// </summary>
        public string DealerKind { get; set; }
        /// <summary>
        /// 经销商唯一标识符字段名
        /// </summary>
        public string DealerColumn { get; set; }
        /// <summary>
        /// 经销商名称字段名
        /// </summary>
        public string DealerName { get; set; }
    }

    /// <summary>
    /// 配置类
    /// </summary>
    public class Config
    {
        /// <summary>
        /// 单例
        /// </summary>
        private static Config _config = new Config();
        /// <summary>
        /// 私有构造
        /// </summary>
        private Config()
        {

        }

        /// <summary>
        /// 获取对象
        /// </summary>
        public static Config Settings
        {
            get
            {
                return _config;
            }
        }

        /// <summary>
        /// 读取Xml内视图信息
        /// </summary>
        /// <param name="viewsCode">视图组节点名</param>
        /// <param name="viewCode">视图节点名</param>
        /// <returns></returns>
        public List<View> GetViews(string viewsCode, string viewCode)
        {
            List<View> ret = new List<View>();
            XmlDocument xml = new XmlDocument();
            xml.Load("Config.xml");
            XmlNodeList tmp = xml.GetElementsByTagName(viewsCode);
            foreach (XmlNode view in tmp)
            {
                View viewInfo = new View();
                foreach (XmlNode node in view.ChildNodes)
                {
                    switch (node.Name) {
                        case "SelectStr":
                            viewInfo.ViewName = node.InnerText;
                            break;
                        case "ViewDealerName":
                            viewInfo.DealerName = node.InnerText;
                            break;
                        case "DealerColumn":
                            viewInfo.DealerColumn = node.InnerText;
                            break;
                        case "ViewDealerType":
                            viewInfo.DealerType = node.InnerText;
                            break;
                        case "ViewDealerKind":
                            viewInfo.DealerKind = node.InnerText;
                            break;
                    }
                }
                ret.Add(viewInfo);
            }
            return ret;
        }

        /// <summary>
        /// 重载[]运算符
        /// </summary>
        /// <param name="key">字段</param>
        /// <returns>值</returns>
        public string this[string key]
        {
            get
            {
                XmlDocument xml = new XmlDocument();
                xml.Load("Config.xml");
                var tmp = xml.GetElementsByTagName(key);
                if(tmp.Count > 0)
                {
                    return tmp[0].InnerText;
                }
                else
                {
                    return "";
                }
            }
        }
    }
}
