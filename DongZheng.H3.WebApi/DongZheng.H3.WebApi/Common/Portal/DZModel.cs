using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// model 的摘要说明
/// </summary>


namespace DongZheng.H3.WebApi.Common.Portal
{
    public class DZModel
    {
        public DZModel()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }

    }
    public partial class CAPHistory
    {
        public string applicationType { get; set; }
        public string applicationNo { get; set; }
        public string ThaiFirstName { get; set; }
        public string financedamount { get; set; }
    }
    public partial class RSResult
    {
        public string reqID { get; set; }
        public data data { get; set; }
        public string code { get; set; }
        public string msg { get; set; }
    }
    public partial class data
    {
        public string action { get; set; }
        public string limit { get; set; }
    }
    public class AttachmentParamNew
    {
        /// <summary>
        /// 获取或设置文件名称
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// 获取或设置文件内容
        /// </summary>
        public byte[] Content { get; set; }
        /// <summary>
        /// 获取或设置文件类型
        /// </summary>
        public string ContentType { get; set; }
        /// <summary>
        /// 获取或设置文件大小
        /// </summary>
        public int ContentLength { get; set; }
        /// <summary>
        /// 获取或设置文件下载地址
        /// </summary>
        public string DownloadUrl { get; set; }
    }
    /// <summary>
    /// 内外网经销商
    /// </summary>
    public class jxs_model
    {
        /// <summary>
        /// 内外网
        /// </summary>
        public string nww { get; set; }
        /// <summary>
        /// 经销商名字
        /// </summary>
        public string jxsname { get; set; }
        /// <summary>
        /// 经销商名字
        /// </summary>
        public string jxs { get; set; }
        /// <summary>
        /// 初始放款额度
        /// </summary>
        public System.Decimal csfked { get; set; }
        /// <summary>
        /// 已使用额度
        /// </summary>
        public System.Decimal ysyed { get; set; }
        /// <summary>
        /// 可用额度
        /// </summary>
        public System.Decimal kyed { get; set; }
        /// <summary>
        /// objectid
        /// </summary>
        public string objectid { get; set; }
    }
 /// <summary>
    /// 客户还款能力计算
    /// </summary>
    public class hknl_model
    {
        public hknl_model()
        {
            APPNO = "";
            ZJR = "";
            GJR = "";
            DKJE = "0";
            QS = "0";
            ZJR_YSRGZ = "0";
            ZJR_YYHZW = "0";
            ZJR_KHZCGZ = "0";
            ZJR_KHYHKNL = "0";
            GJR_YSRGZ = "0";
            GJR_YYHZW = "0";
            GJR_KHZCGZ = "0";
            GJR_KHYHKNL = "0";
            SRFZC = "0";
            CJSJ = DateTime.Now.ToString();
        }
        public string APPNO { get; set; }
        public string ZJR { get; set; }
        public string GJR { get; set; }
        public string DKJE { get; set; }
        public string QS { get; set; }
        public string ZJR_YSRGZ { get; set; }
        public string ZJR_YYHZW { get; set; }
        public string ZJR_KHZCGZ { get; set; }
        public string ZJR_KHYHKNL { get; set; }
        public string GJR_YSRGZ { get; set; }
        public string GJR_YYHZW { get; set; }
        public string GJR_KHZCGZ { get; set; }
        public string GJR_KHYHKNL { get; set; }
        public string SRFZC { get; set; }
        public string CJSJ { get; set; }
    }
    /// <summary>
    /// 回传参数
    /// </summary>
    public partial class hknl_result
    {
        public string Result { get; set; }
        public string C_IncomOfMonth { get; set; }
        public string C_DabtsOfMonth { get; set; }
        public string C_AssetValuation { get; set; }
        public string C_RepayLoan { get; set; }

    }
}