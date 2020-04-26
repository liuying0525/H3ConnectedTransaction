using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ScoreCore
{
    public delegate List<Dealer> LoadDealerInfo(List<Dealer> dealers);
    /// <summary>
    /// 评分管理
    /// </summary>
    public class ScoreManage
    {
        /// <summary>
        /// 引擎锁
        /// </summary>
        private static object MonoConnectionLockObject = new object();
        /// <summary>
        /// 引擎连接
        /// </summary>
        private static OThinker.H3.Connection MonoConnection;
        /// <summary>
        /// 引擎实例
        /// </summary>
        private static OThinker.H3.IEngine _engine = null;
        /// <summary>
        /// 设置引擎连接
        /// </summary>
        /// <param name="connection"></param>
        private static void SetEngin(string connection)
        {
            lock (MonoConnectionLockObject)
            {
                if (MonoConnection == null || MonoConnection.Engine == null)
                {
                    OThinker.H3.Connection c = new OThinker.H3.Connection();
                    string connectionString = System.Configuration.ConfigurationManager.AppSettings["BPMEngine"];
                    try
                    {
                        OThinker.Clusterware.ConnectionResult result = c.Open(connection);
                        if (result != OThinker.Clusterware.ConnectionResult.Success)
                        {
                            throw new Exception("引擎服务连接错误->" + result.ToString());
                        }
                    }
                    catch(Exception ex)
                    {
                        try
                        {
                            OThinker.Clusterware.ConnectionResult result = c.Open(connectionString);
                            if (result != OThinker.Clusterware.ConnectionResult.Success)
                            {
                                throw new Exception("引擎服务连接错误->" + result.ToString());
                            }
                        }
                        catch(Exception e)
                        {
                            throw e;
                        }

                    }
                    MonoConnection = c;
                }
            }
            _engine =  MonoConnection.Engine;
        }
        /// <summary>
        /// 获取引擎实例
        /// </summary>
        public static OThinker.H3.IEngine Engine
        {
            get
            {
                return _engine;
            }
        }
        /// <summary>
        /// 读取所有信息
        /// </summary>
        /// <param name="_scoreRuleTable">评分表</param>
        /// <param name="_gradeTable">等级表</param>
        /// <param name="structNames">字段类</param>
        /// <param name="loadDealerInfo">读取信息方法</param>
        /// <param name="engineConnection">引擎连接串</param>
        /// <returns>评分引擎实例</returns>
        public static ScoreManage LoadGradeObject(string _scoreRuleTable, string _gradeTable, StructNames structNames, LoadDealerInfo loadDealerInfo, string engineConnection = "")
        {
            if(_engine == null) SetEngin(engineConnection);
            ScoreManage scoreManage = new ScoreManage();
            DataTable groupTable = null;
            try { groupTable = Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable("select * from " + _scoreRuleTable); }
            catch(Exception e) { Console.Write(e.Message); }

            foreach (DataRow row in groupTable.Rows)
            {
                scoreManage.PushScoreIntoGroup(row, structNames);
            }

            foreach (ScoreGroup group in scoreManage.Scores)
            {
                DataTable gradeTable = Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(string.Format("SELECT {0},{1},{2},{3},{4} FROM {6} WHERE States=1 AND PID='{5}'", structNames.GradeNames.ObjectId, structNames.GradeNames.GroupId, structNames.GradeNames.Grade, structNames.GradeNames.RangeFrom, structNames.GradeNames.RangeTo, group.ObjectId, _gradeTable));
                foreach (DataRow row in gradeTable.Rows)
                {
                    group.GradeList.Add(new Grade()
                    {
                        ObjectId = row[structNames.GradeNames.ObjectId].ToString(),
                        Name = row[structNames.GradeNames.Grade].ToString(),
                        ScoreFrom = row[structNames.GradeNames.RangeFrom].ToString().IndexOf('?') >= 0 ? double.MinValue : double.Parse(row[structNames.GradeNames.RangeFrom].ToString()),
                        ScoreTo = row[structNames.GradeNames.RangeTo].ToString().IndexOf('?') >= 0 ? double.MaxValue : double.Parse(row[structNames.GradeNames.RangeTo].ToString()),
                    });
                }
            }
            scoreManage.Dealers = loadDealerInfo(scoreManage.Dealers);
            return scoreManage;
        }
        /// <summary>
        /// 所有评分
        /// </summary>
        public List<ScoreGroup> Scores { get; set; }
        /// <summary>
        /// 经销商
        /// </summary>
        public List<Dealer> Dealers { get; set; }
        /// <summary>
        /// 算评分时日志
        /// </summary>
        public string GetGradeLog { get; set; }
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="scores">评分列表</param>
        /// <param name="dealers">经销商列表</param>
        public ScoreManage(List<ScoreGroup> scores = null, List<Dealer> dealers = null)
        {
            Scores = scores ?? new List<ScoreGroup>();
            Dealers = dealers ?? new List<Dealer>();
        }
        /// <summary>
        /// 读取评分结构
        /// </summary>
        /// <param name="row">行数据</param>
        /// <param name="structNames">字段结构</param>
        public void PushScoreIntoGroup(DataRow row, StructNames structNames)
        {
            try
            {

                ScoreGroup group = Scores.FirstOrDefault(x => x.ObjectId == row[structNames.GroupNames.ObjectId].ToString());
                if (group == null)
                {
                    group = new ScoreGroup()
                    {
                        ObjectId = row[structNames.GroupNames.ObjectId].ToString(),
                        DealerIntranet = row[structNames.GroupNames.DealerIntranet].ToString(),
                        DealerType = row[structNames.GroupNames.DealerType].ToString()
                    };
                    Scores.Add(group);
                }
                Score score = group.ScoreList.FirstOrDefault(x => x.ObjectId == row[structNames.ScoreNames.ObjectId].ToString());
                if (score == null)
                {
                    float weight;
                    if (!float.TryParse(row[structNames.ScoreNames.Weight].ToString(), out weight))
                        weight = 0;
                    score = new Score()
                    {
                        ObjectId = row[structNames.ScoreNames.ObjectId].ToString(),
                        Expression = row[structNames.ScoreNames.Expression].ToString(),
                        ScoreName = row[structNames.ScoreNames.ScoreName].ToString(),
                        Weight = weight
                    };
                    group.ScoreList.Add(score);
                }
                Detail detail = score.DetailList.FirstOrDefault(x => x.ObjectId == row[structNames.DetailNames.ObjectId].ToString());
                if (detail == null)
                {
                    float rate;
                    if (!float.TryParse(row[structNames.DetailNames.Rate].ToString(), out rate))
                        rate = 0;
                    detail = new Detail()
                    {
                        ObjectId = row[structNames.DetailNames.ObjectId].ToString(),
                        Rate = rate
                    };
                    score.DetailList.Add(detail);
                }
                Option option = detail.Options.FirstOrDefault(x => x.ObjectId == row[structNames.OptionNames.ObjectId].ToString());
                if (option == null)
                {
                    double value;
                    if (!double.TryParse(row[structNames.OptionNames.Value].ToString(), out value))
                        value = 0;
                    option = new Option()
                    {
                        ObjectId = row[structNames.OptionNames.ObjectId].ToString(),
                        Memo = row[structNames.OptionNames.Memo].ToString(),
                        Value = value
                    };
                    detail.Options.Add(option);
                }
            }
            catch(Exception ex)
            {
                Scores = new List<ScoreGroup>();
            }
        }
        public bool ReCordLog { get; set; }
        /// <summary>
        /// 计算评分
        /// </summary>
        /// <param name="checkTypeKind">检查类型性质</param>
        /// <returns>经销商列表</returns>
        public List<Dealer> GetGrade(bool checkTypeKind = false)
        {
            DataTable calc = new DataTable();
            GetGradeLog = "";
            foreach (Dealer dealer in Dealers)
            {
                if(ReCordLog) GetGradeLog += string.Format(@"经销商信息:{0},{1},{2},{3}", dealer.DealerId, dealer.DealerName, dealer.DealerKind, dealer.DealerType);
                foreach (ScoreGroup group in Scores)
                {
                    if (ReCordLog) GetGradeLog += string.Format(@"评分组信息:{0},{1}", group.DealerIntranet, group.DealerType);
                    double totalScore = 0;
                    foreach (Score score in group.ScoreList)
                    {
                        if (ReCordLog) GetGradeLog += string.Format(@"评分信息:{0},{1}", score.ScoreName, score.Expression);
                        score.RealScore = 0;
                        string tmpExpression = score.Expression;
                        foreach (Detail detail in score.DetailList)
                        {
                            detail.dataValue = 0;
                        }
                        foreach (Detail detail in score.DetailList)
                        {
                            if (ReCordLog) GetGradeLog += string.Format(@"详细信息:{0}", detail.Rate);
                            foreach (Option option in detail.Options)
                            {
                                DealerData dealerData = dealer.DataList.FirstOrDefault(x => x.Variable == option.MatchingVariable);
                                switch (option.MatchingType)
                                {
                                    case MatchingType.Range:
                                        {
                                            if (dealerData == null || dealerData.Values == null) continue;
                                            double dataValue;
                                            if (double.TryParse(dealerData.Values.ToString(), out dataValue) == false) continue;
                                            if (option.RangeFrom <= dataValue && option.RangeTo > dataValue)
                                            {
                                                if (ReCordLog) GetGradeLog += string.Format(@"匹配得分:{0},{1},{2},{3},{4},{5}", option.MatchingVariable, Enum.GetName(typeof(MatchingType), option.MatchingType), option.RangeFrom, option.RangeTo, option.Value, dealerData == null ? "null" : dealerData.Values);
                                                detail.dataValue += option.Value;
                                            }
                                        }
                                        break;
                                    case MatchingType.Exists:
                                        {
                                            if (dealerData == null || dealerData.Values == null) continue;
                                            if (!string.IsNullOrWhiteSpace(dealerData.Values.ToString()))
                                            {
                                                if (ReCordLog) GetGradeLog += string.Format(@"匹配得分:{0},{1},{2},{3}", option.MatchingVariable, Enum.GetName(typeof(MatchingType), option.MatchingType), option.Value, dealerData == null ? "null" : dealerData.Values);
                                                detail.dataValue += option.Value;
                                            }
                                        }
                                        break;
                                    case MatchingType.NotExists:
                                        {
                                            if (dealerData == null || dealerData.Values == null || string.IsNullOrWhiteSpace(dealerData.Values.ToString()))
                                            {
                                                if (ReCordLog)
                                                    GetGradeLog += string.Format(@"匹配得分:{0},{1},{2}", option.MatchingVariable, Enum.GetName(typeof(MatchingType), option.MatchingType), option.Value);
                                                detail.dataValue += option.Value;
                                            }
                                        }
                                        break;
                                    case MatchingType.Equal:
                                        {
                                            if (dealerData == null || dealerData.Values == null) continue;
                                            switch (dealerData.DealerDataType)
                                            {
                                                case DealerDataType.Single:
                                                    {
                                                        if (option.EqualText.ToLower() == dealerData.Values.ToString().ToLower())
                                                        {
                                                            if (ReCordLog)
                                                                GetGradeLog += string.Format(@"匹配得分:{0},{1},{2},{3},{4}", option.MatchingVariable, Enum.GetName(typeof(MatchingType), option.MatchingType), option.EqualText, option.Value, dealerData == null ? "null" : dealerData.Values);
                                                            detail.dataValue += option.Value;
                                                        }
                                                    }
                                                    break;
                                                case DealerDataType.Array:
                                                    {
                                                        List<string> values = dealerData.Values as List<string>;
                                                        if (values != null && values.FirstOrDefault(x => x.ToLower() == option.EqualText.ToLower()) != null)
                                                        {
                                                            string vvvv = "";
                                                            foreach (var item in values)
                                                            {
                                                                vvvv += item + ",";
                                                            }
                                                            if (ReCordLog)
                                                                GetGradeLog += string.Format(@"匹配得分:{0},{1},{2},{3},{4}", option.MatchingVariable, Enum.GetName(typeof(MatchingType), option.MatchingType), option.EqualText, option.Value, vvvv);
                                                            detail.dataValue += option.Value;
                                                        }
                                                    }
                                                    break;
                                            }
                                        }
                                        break;
                                }
                            }
                            if (ReCordLog)
                                GetGradeLog += string.Format(@"单项得分:{0}", detail.dataValue);
                        }

                        double tmpScore;
                        try
                        {
                            int startIndex = tmpExpression.IndexOf('{');
                            while(startIndex >= 0)
                            {
                                int endIndex = tmpExpression.IndexOf('}', startIndex);
                                string varExp = tmpExpression.Substring(startIndex + 1, endIndex - startIndex - 1);
                                Detail detail = score.DetailList.FirstOrDefault(x => x.ObjectId == varExp);
                                tmpExpression = tmpExpression.Replace("{" + varExp + "}", detail == null ? "0" : (detail.dataValue * detail.Rate).ToString());
                                startIndex = tmpExpression.IndexOf('{', startIndex + 1);
                            }
                            
                            tmpExpression = DivisionZero(tmpExpression, score.DetailList);
                            object transcation = calc.Compute(tmpExpression, "");
                            tmpScore = double.Parse(transcation.ToString());
                        }
                        catch
                        {
                            tmpScore = 0;
                        }
                        if (ReCordLog)
                            GetGradeLog += string.Format(@"单组未加权得分:{0},{1}", score.ScoreName, tmpScore);
                        score.RealScore = tmpScore;
                        totalScore += score.Weight * tmpScore / 100;
                        if (ReCordLog)
                            GetGradeLog += string.Format(@"单组得加权分:{0},{1},{2}", score.ScoreName, score.Weight, score.Weight * tmpScore / 100);
                    }
                    if (ReCordLog)
                        GetGradeLog += string.Format(@"总体得分:{0}", totalScore);
                    Grade grade = group.GradeList.FirstOrDefault(x => x.ScoreFrom <= totalScore && totalScore <= x.ScoreTo);
                    if (ReCordLog)
                        GetGradeLog += string.Format(@"匹配等级:{0}", grade == null ? "未匹配" : grade.Name + "," + grade.ScoreFrom + "," + grade.ScoreTo);
                    List<Score> ss = new List<Score>();
                    foreach (Score item in group.ScoreList)
                    {
                        ss.Add(new Score() { Expression = item.Expression, ObjectId = item.ObjectId, RealScore = item.RealScore, ScoreName = item.ScoreName, Weight = item.Weight });
                    }
                    DealerResult dealerResult = new DealerResult() { DealerIntranet = group.DealerIntranet, DealerType = group.DealerType, ResultGrade = (grade ?? new Grade() { Name = "" }).Name, TotalScore = totalScore, Scores = ss };
                    dealer.ResultList.Add(dealerResult);
                }
                dealer.DataList = new List<DealerData>();
                if (checkTypeKind)
                {
                    for (int i = dealer.ResultList.Count - 1; i >= 0; i--)
                    {
                        if (dealer.DealerKind != null && dealer.DealerType != null && dealer.ResultList[i].DealerIntranet.ToLower() == dealer.DealerKind.ToLower() && dealer.ResultList[i].DealerType.ToLower() == dealer.DealerType.ToLower()) continue;
                        else dealer.ResultList.RemoveAt(i);
                    }
                    while (dealer.ResultList.Count > 1)
                    {
                        dealer.ResultList.RemoveAt(dealer.ResultList.Count - 1);
                    }
                }

                string scoreLog = "";
                foreach (var res in dealer.ResultList)
                {
                    scoreLog += "    " + res.DealerIntranet + "|" + res.DealerType + "[" + res.TotalScore + "|" + res.ResultGrade + "]{";
                    foreach (var score in res.Scores)
                    {
                        scoreLog += "(" + score.ScoreName + "|" + score.RealScore + ")";
                    }
                    scoreLog += @"
";
                }
                if (ReCordLog)
                    GetGradeLog += string.Format(@"经销商最终得分:{0},{1}{2}", dealer.DealerName, dealer.DealerId, scoreLog);
            }
            return Dealers;
        }
        /// <summary>
        /// 处理被除数是0的情况
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="detailList">经销商列表</param>
        /// <returns></returns>
        public string DivisionZero(string expression, List<Detail> detailList)
        {
            int divisionCharIndex = expression.IndexOf('/');
            while (divisionCharIndex >= 0)
            {
                string pre = "", next = "";
                int preLevel = 0, nextLevel = 0;
                for (int i = divisionCharIndex - 1; i >= 0; i--)
                {
                    if (expression[i] == '}')
                    {
                        for (int j = i - 1; j >= 0; j--)
                        {
                            if (expression[j] == '{')
                            {
                                pre = expression.Substring(j, i - j + 1) + pre;
                                i = j;
                                break;
                            }
                            else
                                continue;
                        }
                    }
                    else if (expression[i] == ')')
                    {
                        preLevel++;
                        pre = expression[i] + pre;
                    }
                    else if (expression[i] == '(')
                    {
                        preLevel--;
                        pre = expression[i] + pre;
                    }
                    else
                    {
                        pre = expression[i] + pre;
                    }
                    if (preLevel == 0) break;
                }
                for (int i = divisionCharIndex + 1; i < expression.Length; i++)
                {
                    if (expression[i] == '{')
                    {
                        for (int j = i + 1; j < expression.Length; j++)
                        {
                            if (expression[j] == '}')
                            {
                                next = next + expression.Substring(i, j - i + 1);
                                i = j;
                                break;
                            }
                            else
                                continue;
                        }
                    }
                    else if (expression[i] == '(')
                    {
                        nextLevel++;
                        next = next + expression[i];
                    }
                    else if (expression[i] == ')')
                    {
                        nextLevel--;
                        next = next + expression[i];
                    }
                    else
                    {
                        next = next + expression[i];
                    }
                    if (nextLevel == 0) break;
                }
                string retNext = DivisionZero(next, detailList);
                foreach (Detail detail in detailList)
                {
                    retNext = retNext.Replace("{" + detail.ObjectId + "}", (detail.Rate * detail.dataValue).ToString());
                }
                if (((object)0).Equals(new DataTable().Compute(retNext, "")))
                {
                    expression = expression.Replace(pre + "/" + next, "0");
                    divisionCharIndex = expression.IndexOf('/', divisionCharIndex - pre.Length + 1);
                }
                else
                {
                    divisionCharIndex = expression.IndexOf('/', divisionCharIndex + 1);
                }
            }
            return expression;
        }
    }
    #region 评分结构
    /// <summary>
    /// 评分组
    /// </summary>
    public class ScoreGroup
    {
        /// <summary>
        /// ObjectId
        /// </summary>
        public string ObjectId { get; set; }
        /// <summary>
        /// 内网/外网
        /// </summary>
        public string DealerType { get; set; }
        /// <summary>
        /// 4s/二手车
        /// </summary>
        public string DealerIntranet { get; set; }
        /// <summary>
        /// 评分列表
        /// </summary>
        public List<Score> ScoreList { get; set; }
        /// <summary>
        /// 等级列表
        /// </summary>
        public List<Grade> GradeList { get; set; }
        public ScoreGroup()
        {
            ScoreList = new List<Score>();
            GradeList = new List<Grade>();
        }
    }

    /// <summary>
    /// 评分
    /// </summary>
    public class Score
    {
        /// <summary>
        /// ObjectId
        /// </summary>
        public string ObjectId { get; set; }
        /// <summary>
        /// 评分名
        /// </summary>
        public string ScoreName { get; set; }
        /// <summary>
        /// 权重
        /// </summary>
        public float Weight { get; set; }
        /// <summary>
        /// 表达式
        /// </summary>
        public string Expression { get; set; }
        /// <summary>
        /// 未加权真实得分
        /// </summary>
        public double RealScore { get; set; }
        /// <summary>
        /// 详细列表
        /// </summary>
        public List<Detail> DetailList { get; set; }
        public Score()
        {
            DetailList = new List<Detail>();
        }
    }

    /// <summary>
    /// 详细
    /// </summary>
    public class Detail
    {
        /// <summary>
        /// ObjectId
        /// </summary>
        public string ObjectId { get; set; }
        /// <summary>
        /// 倍率
        /// </summary>
        public float Rate { get; set; }
        /// <summary>
        /// 分值列表
        /// </summary>
        public List<Option> Options { get; set; }
        /// <summary>
        /// 最后得分
        /// </summary>
        public double dataValue { get; set; }
        public Detail()
        {
            Options = new List<Option>();
        }
    }

    /// <summary>
    /// 分值
    /// </summary>
    public class Option
    {
        /// <summary>
        /// ObjectId
        /// </summary>
        public string ObjectId { get; set; }
        /// <summary>
        /// 匹配类型
        /// </summary>
        public MatchingType MatchingType { get; set; }
        /// <summary>
        /// 匹配变量
        /// </summary>
        public string MatchingVariable { get; set; }
        /// <summary>
        /// 开始范围（包含）
        /// </summary>
        public double RangeFrom { get; set; }
        /// <summary>
        /// 结束范围（不包含）
        /// </summary>
        public double RangeTo { get; set; }
        /// <summary>
        /// 匹配文本
        /// </summary>
        public string EqualText { get; set; }
        /// <summary>
        /// 分值
        /// </summary>
        public double Value { get; set; }
        public Option() { }
        /// <summary>
        /// 单项评分表达式解析
        /// </summary>
        public string Memo
        {
            set
            {
                string[] list = value.Split(new char[] { '[', '(', ':', '-', ']', ')' });
                switch (list[0])
                {
                    case "范围":
                        this.MatchingVariable = list[1];
                        this.EqualText = string.Empty;
                        if (list[2].IndexOf('?') >= 0)
                            this.RangeFrom = double.MinValue;
                        else if(list[2].IndexOf('%') >= 0)
                        {
                            list[2] = list[2].Replace("%", "");
                            double v = 0;
                            double.TryParse(list[2], out v);
                            this.RangeFrom = v / 100;
                        }
                        else
                        {
                            double v = 0;
                            double.TryParse(list[2], out v);
                            this.RangeFrom = v;
                        }

                        if (list[3].IndexOf('?') >= 0)
                            this.RangeTo = double.MaxValue;
                        else if (list[2].IndexOf('%') >= 0)
                        {
                            list[3] = list[3].Replace("%", "");
                            double v = 0;
                            double.TryParse(list[2], out v);
                            this.RangeTo = v / 100;
                        }
                        else
                        {
                            double v = 0;
                            double.TryParse(list[3], out v);
                            this.RangeTo = v;
                        }

                        this.MatchingType = MatchingType.Range;
                        break;
                    case "存在":
                        this.MatchingVariable = list[1];
                        this.EqualText = string.Empty;
                        this.RangeFrom = double.NaN;
                        this.RangeTo = double.NaN;
                        this.MatchingType = MatchingType.Exists;
                        break;
                    case "不存在":
                        this.MatchingVariable = list[1];
                        this.EqualText = string.Empty;
                        this.RangeFrom = double.NaN;
                        this.RangeTo = double.NaN;
                        this.MatchingType = MatchingType.NotExists;
                        break;
                    case "文本":
                        this.MatchingVariable = list[1];
                        this.EqualText = list[2];
                        this.RangeFrom = double.NaN;
                        this.RangeTo = double.NaN;
                        this.MatchingType = MatchingType.Equal;
                        break;
                }
            }
        }
    }

    /// <summary>
    /// 评分等级
    /// </summary>
    public class Grade
    {
        /// <summary>
        /// ObjectId
        /// </summary>
        public string ObjectId { get; set; }
        /// <summary>
        /// 该等级开始分数（包含）
        /// </summary>
        public double ScoreFrom { get; set; }
        /// <summary>
        /// 该等级结束分数（不包含）
        /// </summary>
        public double ScoreTo { get; set; }
        /// <summary>
        /// 等级名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 构造
        /// </summary>
        public Grade() { }
    }
    #endregion

    #region 经销商结构
    /// <summary>
    /// 经销商
    /// </summary>
    public class Dealer
    {
        /// <summary>
        /// 经销商ID
        /// </summary>
        public string DealerId { get; set; }
        /// <summary>
        /// 经销商名称
        /// </summary>
        public string DealerName { get; set; }
        /// <summary>
        /// 发起时间
        /// </summary>
        public string DealerAccount { get; set; }
        /// <summary>
        /// 经销商类型 内外网
        /// </summary>
        public string DealerType { get; set; }
        /// <summary>
        /// 经销商渠道 4S/二手车
        /// </summary>
        public string DealerKind { get; set; }
        /// <summary>
        /// 数据列表
        /// </summary>
        public List<DealerData> DataList { get; set; }
        /// <summary>
        /// 等级结果列表
        /// </summary>
        public List<DealerResult> ResultList { get; set; }
        /// <summary>
        /// 构造
        /// </summary>
        public Dealer()
        {
            DataList = new List<DealerData>();
            ResultList = new List<DealerResult>();
        }
    }
    /// <summary>
    /// 经销商数据
    /// </summary>
    public class DealerData
    {
        /// <summary>
        /// ObjectID
        /// </summary>
        public string ObjectId { get; set; }
        /// <summary>
        /// 变量名
        /// </summary>
        public string Variable { get; set; }
        /// <summary>
        /// 数据类型
        /// </summary>
        public DealerDataType DealerDataType { get; set; }
        /// <summary>
        /// 数据
        /// </summary>
        public object Values { get; set; }
    }
    /// <summary>
    /// 经销商等级结果
    /// </summary>
    public class DealerResult
    {
        /// <summary>
        /// 内外网
        /// </summary>
        public string DealerType { get; set; }
        /// <summary>
        /// 4s/2手车
        /// </summary>
        public string DealerIntranet { get; set; }
        /// <summary>
        /// 总分
        /// </summary>
        public double TotalScore { get; set; }
        /// <summary>
        /// 结果等级
        /// </summary>
        public string ResultGrade { get; set; }
        /// <summary>
        /// 评分详情
        /// </summary>
        public List<Score> Scores = new List<Score>();
    }
    #endregion
}
