using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ScoreCore
{
    /// <summary>
    /// 字段结构体
    /// </summary>
    public class StructNames
    {
        /// <summary>
        /// 组信息字段名
        /// </summary>
        public GroupNames GroupNames = new GroupNames();
        /// <summary>
        /// 评分信息字段名
        /// </summary>
        public ScoreNames ScoreNames = new ScoreNames();
        /// <summary>
        /// 详细信息字段名
        /// </summary>
        public DetailNames DetailNames = new DetailNames();
        /// <summary>
        /// 单项信息字段名
        /// </summary>
        public OptionNames OptionNames = new OptionNames();
        /// <summary>
        /// 等级信息字段名
        /// </summary>
        public GradeNames GradeNames = new GradeNames();
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="loadXml">是否读取xml</param>
        public StructNames(bool loadXml = true)
        {
            if (!loadXml) return;
            XmlDocument xml = new XmlDocument();
            xml.Load("Config.xml");
            XmlNodeList structNames = xml.GetElementsByTagName("StructNames");
            foreach (XmlElement structName in structNames)
            {
                foreach (XmlElement childs in structName.ChildNodes)
                {
                    foreach (XmlElement child in childs.ChildNodes)
                    {
                        switch (childs.Name)
                        {
                            case "GroupNames":
                                switch (child.Name)
                                {
                                    case "GroupId":
                                        GroupNames.ObjectId = child.InnerText;
                                        break;
                                    case "DealerType":
                                        GroupNames.DealerType = child.InnerText;
                                        break;
                                    case "DealerIntranet":
                                        GroupNames.DealerIntranet = child.InnerText;
                                        break;
                                }
                                break;
                            case "ScoreNames":
                                switch (child.Name)
                                {
                                    case "ScoreId":
                                        ScoreNames.ObjectId = child.InnerText;
                                        break;
                                    case "Weight":
                                        ScoreNames.Weight = child.InnerText;
                                        break;
                                    case "Expression":
                                        ScoreNames.Expression = child.InnerText;
                                        break;
                                }
                                break;
                            case "DetailNames":
                                switch (child.Name)
                                {
                                    case "DetailId":
                                        DetailNames.ObjectId = child.InnerText;
                                        break;
                                    case "Rate":
                                        DetailNames.Rate = child.InnerText;
                                        break;
                                }
                                break;
                            case "OptionNames":
                                switch (child.Name)
                                {
                                    case "OptionId":
                                        OptionNames.ObjectId = child.InnerText;
                                        break;
                                    case "Memo":
                                        OptionNames.Memo = child.InnerText;
                                        break;
                                    case "Score":
                                        OptionNames.Value = child.InnerText;
                                        break;
                                }
                                break;
                            case "GradeNames":
                                switch (child.Name)
                                {
                                    case "ObjectId":
                                        GradeNames.ObjectId = child.InnerText;
                                        break;
                                    case "PID":
                                        GradeNames.GroupId = child.InnerText;
                                        break;
                                    case "Grade":
                                        GradeNames.Grade = child.InnerText;
                                        break;
                                    case "RangeFrom":
                                        GradeNames.RangeFrom = child.InnerText;
                                        break;
                                    case "RangeTo":
                                        GradeNames.RangeTo = child.InnerText;
                                        break;
                                }
                                break;
                        }
                    }
                }
            }
        }
    }
    /// <summary>
    /// 组字段类
    /// </summary>
    public class GroupNames
    {
        /// <summary>
        /// ObjectId字段名
        /// </summary>
        public string ObjectId = "GroupId";
        /// <summary>
        /// 经销商类型字段名
        /// </summary>
        public string DealerType = "DealerType";
        /// <summary>
        /// 经销商性质字段名
        /// </summary>
        public string DealerIntranet = "DealerIntranet";
    }
    /// <summary>
    /// 评分字段类
    /// </summary>
    public class ScoreNames
    {
        /// <summary>
        /// ObjectId字段名
        /// </summary>
        public string ObjectId = "ScoreId";
        /// <summary>
        /// 评分名称字段名
        /// </summary>
        public string ScoreName = "ScoreName";
        /// <summary>
        /// 权重字段名
        /// </summary>
        public string Weight = "Weight";
        /// <summary>
        /// 公式字段名
        /// </summary>
        public string Expression = "Expression";
    }
    /// <summary>
    /// 详细字段类
    /// </summary>
    public class DetailNames
    {
        /// <summary>
        /// ObjectId字段名
        /// </summary>
        public string ObjectId = "DetailId";
        /// <summary>
        /// 倍率字段名
        /// </summary>
        public string Rate = "Rate";
    }
    /// <summary>
    /// 单项字段类
    /// </summary>
    public class OptionNames
    {
        /// <summary>
        /// ObjectId字段名
        /// </summary>
        public string ObjectId = "OptionId";
        /// <summary>
        /// 表达式字段名
        /// </summary>
        public string Memo = "Memo";
        /// <summary>
        /// 分值字段名
        /// </summary>
        public string Value = "Score";
    }
    /// <summary>
    /// 等级字段类
    /// </summary>
    public class GradeNames
    {
        /// <summary>
        /// ObjectId字段名
        /// </summary>
        public string ObjectId = "ObjectId";
        /// <summary>
        /// 组Id字段名
        /// </summary>
        public string GroupId = "PID";
        /// <summary>
        /// 等级字段名
        /// </summary>
        public string Grade = "Grade";
        /// <summary>
        /// 评分范围下限字段名
        /// </summary>
        public string RangeFrom = "RangeFrom";
        /// <summary>
        /// 评分范围上限字段名
        /// </summary>
        public string RangeTo = "RangeTo";
    }
}
