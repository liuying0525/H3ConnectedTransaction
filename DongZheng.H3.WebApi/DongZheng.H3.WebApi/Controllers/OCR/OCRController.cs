using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OThinker.H3.Controllers;
using Newtonsoft.Json;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using System.IO;
using System.Configuration;
using System.Data;
using DongZheng.H3.WebApi.Common.Util;
using DongZheng.H3.WebApi.Models.OCR;

namespace DongZheng.H3.WebApi.Controllers.OCR
{
    [ValidateInput(false)]
    [Xss]
    public class OCRController : OThinker.H3.Controllers.ControllerBase
    {
        #region const
        //OCR相关参数
        string ocr_login_url = ConfigurationManager.AppSettings["OCR_LOGIN_URL"] + string.Empty;
        string ocr_login_name = ConfigurationManager.AppSettings["OCR_LOGIN_NAME"] + string.Empty;
        string ocr_login_password = ConfigurationManager.AppSettings["OCR_LOGIN_PASSWORD"] + string.Empty;
        string ocr_discern_url = ConfigurationManager.AppSettings["OCR_DISCERN_URL"] + string.Empty;
        double ocr_token_expire_time = Convert.ToDouble(ConfigurationManager.AppSettings["OCR_TOKEN_EXPIRE_TIME"] + string.Empty);
        string ocr_server = ConfigurationManager.AppSettings["OCR_SERVER"] + string.Empty;
        string ocr_dz_idcard = ConfigurationManager.AppSettings["OCR_DONGZHENG_IDCARD"] + string.Empty;
        string ocr_dz_bankcard = ConfigurationManager.AppSettings["OCR_DONGZHENG_BANKCARD"] + string.Empty;
        string ocr_dz_vehicleInvoice = ConfigurationManager.AppSettings["OCR_DONGZHENG_VEHICLEINVOICE"] + string.Empty;
        string ocr_dz_caridcard = ConfigurationManager.AppSettings["OCR_DONGZHENG_CARIDCARD"] + string.Empty;
        string ocr_dz_drivercard = ConfigurationManager.AppSettings["OCR_DONGZHENG_DRIVERLICENSE"] + string.Empty;
        string ocr_dz_vin = ConfigurationManager.AppSettings["OCR_DONGZHENG_VIN"] + string.Empty;

        //图片压缩相关参数
        bool pic_compress_enable = Convert.ToBoolean(ConfigurationManager.AppSettings["PIC_COMPRESS_ENABLE"] + string.Empty);
        double pic_compress_minsize_kb = Convert.ToDouble(ConfigurationManager.AppSettings["PIC_COMPRESS_MINSIZE_KB"] + string.Empty);
        string pic_compress_xy_url = ConfigurationManager.AppSettings["PIC_COMPRESS_XY_URL"] + string.Empty;
        string pic_compress_factor_url = ConfigurationManager.AppSettings["PIC_COMPRESS_FACTOR_URL"] + string.Empty;
        string pic_compress_quality_url = ConfigurationManager.AppSettings["PIC_COMPRESS_QUALITY_URL"] + string.Empty;
        string pic_compress_rotate_url = ConfigurationManager.AppSettings["PIC_COMPRESS_ROTATE_URL"] + string.Empty;

        // 图片切边url
        string pic_crop_url = ConfigurationManager.AppSettings["PIC_CROP_URL"] + string.Empty;

        public const string SettingName_OCR_Token = "OCR_Token";

        /// <summary>
        /// 图片处理的开关
        /// <para>0：不处理</para>
        /// <para>1：切边</para>
        /// <para>2：压缩</para>
        /// </summary>
        private readonly string _picProcessingSwitch = ConfigurationManager.AppSettings.Get("PictureProcessingSwitch");
        #endregion

        // GET: OCR
        public string Index()
        {
            return "OCR API";
        }

        #region Attribute
        public override string FunctionCode
        {
            get { return ""; }
        }

        private string _FileID;
        /// <summary>
        /// 页面上对应的File显示ID
        /// </summary>
        protected string FileID
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this._FileID))
                {
                    this._FileID = this.Request["fileid"] ?? "";
                }
                return this._FileID;
            }
        }

        protected bool IsMobile
        {
            get
            {
                return (Request["IsMobile"] ?? "").ToLowerInvariant() == "true";
            }
        }
        #endregion

        #region Token
        /// <summary>
        /// 获取OCR Token
        /// </summary>
        /// <returns></returns>
        public JsonResult GetToken()
        {
            var result_token = new Result_Token();
            string token = AppUtility.Engine.SettingManager.GetCustomSetting(SettingName_OCR_Token);
            string str_token = "";
            if (string.IsNullOrEmpty(token))
            {
                result_token = GenerateToken();
                if (result_token.code == "0")
                {
                    str_token = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "trdsys " + result_token.data;
                    AppUtility.Engine.SettingManager.SetCustomSetting(SettingName_OCR_Token, str_token);
                }
            }
            else
            {
                DateTime dt_token_created = Convert.ToDateTime(token.Substring(0, 19));
                if (dt_token_created.AddMinutes(ocr_token_expire_time) <= DateTime.Now)
                {
                    result_token = GenerateToken();
                    if (result_token.code == "0")
                    {
                        str_token = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "trdsys " + result_token.data;
                        AppUtility.Engine.SettingManager.SetCustomSetting(SettingName_OCR_Token, str_token);
                    }
                }
                else
                {
                    result_token.code = "0";
                    result_token.message = "success";
                    str_token = token;
                }
            }
            if (str_token != "")
            {
                result_token.data = str_token.Substring(19);
            }
            return Json(new { Token = result_token, Url = ocr_discern_url }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 重新生成Token，不校验是否过期
        /// </summary>
        /// <returns></returns>
        public JsonResult GetNewToken()
        {
            var result_token = GenerateToken();
            if (result_token.code == "0")
            {
                var str_token = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "trdsys " + result_token.data;
                result_token.data = str_token.Substring(19);
                AppUtility.Engine.SettingManager.SetCustomSetting(SettingName_OCR_Token, str_token);
            }
            return Json(new { Token = result_token, Url = ocr_discern_url }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 生成调用OCR接口的Token
        /// </summary>
        /// <returns></returns>
        public Result_Token GenerateToken()
        {
            AppUtility.Engine.LogWriter.Write("Generate Token...");
            string paras = JsonConvert.SerializeObject(new { userLoginName = ocr_login_name, password = ocr_login_password });
            string result = HttpHelper.PostWebRequest(ocr_login_url, "application/json;charset=UTF-8", paras, httpVersion: 1.0);
            AppUtility.Engine.LogWriter.Write("Generate Token Parameter:URL-->" + ocr_login_url + "，paras-->" + paras + ",Result-->" + result);
            Result_Token token = JsonConvert.DeserializeObject<Result_Token>(result);
            return token;
        }
        #endregion

        [HttpPost]
        public JsonResult RecognizeImage(Parameter_OCR ocrParameter, Parameter_Compress compressParameter, Parameter_Instance instanceParameter, bool isSheetAttachment)
        {
            //参数日志
            AppUtility.Engine.LogWriter.Write("OCR识别 DiscernImageInfo参数，Parameter_OCR-->" + JsonConvert.SerializeObject(ocrParameter) + ",Parameter_Instance-->" + JsonConvert.SerializeObject(instanceParameter) + ",Parameter_Compress-->" + JsonConvert.SerializeObject(compressParameter));

            //判断文件是否为空
            if (Request.Files == null || Request.Files.Count == 0)
            {
                return Json(new HandlerResult(this.FileID, "", "", new { code = -100, data = "", message = "文件为空，请重新上传" }));
            }

            HttpPostedFileBase file = Request.Files[0];
            string[] mediaType = new string[] { ".bmp", ".jpg", ".jpeg", ".gif", ".png" };
            if (!mediaType.Any(t => string.Equals(t, Path.GetExtension(file.FileName), StringComparison.OrdinalIgnoreCase)))
            {
                return Json(new HandlerResult(this.FileID, "", "", new { code = -100, data = "", message = "请上传图片" }));
            }

            //Header参数
            NameValueCollection nv_Header = new NameValueCollection
            {
                { "Authorization", ocrParameter.Authorization }
            };
            //Body参数
            NameValueCollection nv_body = new NameValueCollection();
            byte[] contents = new byte[file.ContentLength];
            file.InputStream.Read(contents, 0, file.ContentLength);
            byte[] buffer;
            string fileUrl;

            try
            {
                switch (_picProcessingSwitch)
                {
                    case "0":
                        return this.GetRecognizeResult(contents, ocrParameter.imageTyep, "", instanceParameter, file, isSheetAttachment);
                    case "1":
                        nv_body.Add("platformType", ocrParameter.platformType);

                        string result = HttpHelper.UploadFileEx(pic_crop_url, nv_Header, nv_body, "multipartFile", file.FileName, contents, file.ContentType, 15, httpVersion: 1.0);
                        var v_Contract = JsonConvert.DeserializeObject<Result_Crop>(result);
                        string att_id_vinNo = "";
                        if (v_Contract != null && v_Contract.code == "0")//识别成功，才保存图片
                        {
                            if (string.IsNullOrEmpty(v_Contract.data.fileUrl))//没有切边后图片，把原图保存到H3
                            {
                                att_id_vinNo = UploadFile(file.FileName, file.ContentType, contents, instanceParameter);
                            }
                            else//进行了切边，把切边后的图片保存到H3
                            {
                                att_id_vinNo = UploadOcrImg(file.FileName, v_Contract.data.fileUrl, instanceParameter);
                            }
                        }
                        buffer = v_Contract != null ? OcrHelper.GetFileContent(v_Contract.data.fileUrl) : contents;
                        fileUrl = v_Contract != null ? v_Contract.data.fileUrl : "";
                        return this.GetRecognizeResult(buffer, ocrParameter.imageTyep, fileUrl, instanceParameter, file, isSheetAttachment);
                    case "2":
                        if ((string.IsNullOrEmpty(compressParameter.x) || string.IsNullOrEmpty(compressParameter.y))
                            && string.IsNullOrEmpty(compressParameter.factor)
                            && string.IsNullOrEmpty(compressParameter.quality)
                            && string.IsNullOrEmpty(compressParameter.rotate))
                        {
                            return Json(new HandlerResult(this.FileID, "", "", new { code = -10001, data = "", message = "参数为空" }));
                        }

                        string picture_Compress_Url = "";
                        if (!string.IsNullOrEmpty(compressParameter.x) && !string.IsNullOrEmpty(compressParameter.y))
                        {
                            picture_Compress_Url = pic_compress_xy_url;
                            nv_body.Add("x", compressParameter.x);
                            nv_body.Add("y", compressParameter.y);
                        }
                        else if (!string.IsNullOrEmpty(compressParameter.factor))
                        {
                            picture_Compress_Url = pic_compress_factor_url;
                            nv_body.Add("factor", compressParameter.factor);
                        }
                        else if (!string.IsNullOrEmpty(compressParameter.quality))
                        {
                            picture_Compress_Url = pic_compress_quality_url;
                            nv_body.Add("quality", compressParameter.quality);
                        }
                        else if (!string.IsNullOrEmpty(compressParameter.rotate))
                        {
                            picture_Compress_Url = pic_compress_rotate_url;
                            nv_body.Add("rotate", compressParameter.rotate);
                        }

                        //图片压缩
                        result = HttpHelper.UploadFileEx(picture_Compress_Url, nv_Header, nv_body, "multipartFile", file.FileName, contents, file.ContentType, 15, httpVersion: 1.0);
                        var v_result = JsonConvert.DeserializeObject<Result_PictureCompress>(result);
                        string id = "";
                        if (v_result != null && v_result.code == "0")//压缩成功，才保存图片
                        {
                            id = UploadOcrImg(file.FileName, v_result.data.fileUrl, instanceParameter);
                            SavePictureCompressResult(instanceParameter.InstanceId, v_result);
                        }

                        buffer = v_result != null ? OcrHelper.GetFileContent(v_result.data.fileUrl) : contents;
                        fileUrl = v_result != null ? v_result.data.fileUrl : "";
                        return this.GetRecognizeResult(buffer, ocrParameter.imageTyep, fileUrl, instanceParameter, file, isSheetAttachment);
                }
            }
            catch (Exception ex)
            {
                AppUtility.Engine.LogWriter.Write("OCR识别异常New-->" + ex.ToString());
                return Json(new
                {
                    code = "-10001",
                    message = "OCR识别失败，请联系管理员"
                });
            }

            return null;
        }

        /// <summary>
        /// 获取图片识别结果
        /// </summary>
        /// <param name="buffer">需要识别的图片二进制内容</param>
        /// <param name="imageTyep">
        /// 图片类型
        /// <para>7：扣款授权书图片</para>
        /// <para>8：抵押贷款图片</para>
        /// <para>9：保单图片</para>
        /// </param>
        /// <returns></returns>
        private JsonResult GetRecognizeResult(byte[] buffer, string imageTyep, string fileUrl, Parameter_Instance instanceParameter, HttpPostedFileBase file, bool isSheetAttachment)
        {
            string attachmentId = UploadFile(file.FileName, file.ContentType, buffer, instanceParameter);
            string url = string.Empty;

            if (isSheetAttachment)
            {
                url = AppConfig.GetReadAttachmentUrl(this.IsMobile, instanceParameter.SchemaCode, "", attachmentId);
            }

            switch (imageTyep)
            {
                case "7":
                    AuthorizationResult authorizationResult = OcrHelper.AuthorizationRecognizeAsync(buffer).Result;
                    Result_DeductionsAuthorized result_DeductionsAuthorized = new Result_DeductionsAuthorized
                    {
                        code = authorizationResult.ErrorCode == 200 ? "0" : authorizationResult.ErrorCode.ToString(),
                        message = authorizationResult.ErrorMsg,
                        data = new Result_DeductionsAuthorized.data_deductionsAuthorized
                        {
                            id = authorizationResult.Id,
                            customer = authorizationResult.Customer,
                            account = authorizationResult.Account,
                            identity = authorizationResult.Identity,
                            identityType = authorizationResult.IdentityType,
                            fileUrl = url
                        }
                    };
                    SaveDeductionsAuthorizedResult(instanceParameter.InstanceId, result_DeductionsAuthorized);

                    return Json(new HandlerResult(this.FileID, attachmentId, url, result_DeductionsAuthorized));
                case "8":
                    ContractResult contractResult = OcrHelper.ContractRecognizeAsync(buffer).Result;
                    Result_Contract result_Contract = new Result_Contract
                    {
                        code = contractResult.ErrorCode == 200 ? "0" : contractResult.ErrorCode.ToString(),
                        message = contractResult.ErrorMsg,
                        data = new Result_Contract.data_contract
                        {
                            hetongbianhao = contractResult.Data.SingleDataList.FirstOrDefault(it => it.Key.ToLower() == "hetongbianhao").Value,
                            daikuanbenjin_daxie = contractResult.Data.SingleDataList.FirstOrDefault(it => it.Key.ToLower() == "daikuanbenjin_daxie").Value,
                            danbaofangshi = contractResult.Data.SingleDataList.FirstOrDefault(it => it.Key.ToLower() == "danbaofangshi").Value,
                            chejiahao = contractResult.Data.SingleDataList.FirstOrDefault(it => it.Key.ToLower() == "chejiahao").Value,
                            gongjieren_xingming = contractResult.Data.SingleDataList.FirstOrDefault(it => it.Key.ToLower() == "gongjieren_xingming").Value,
                            gongjieren_zhengjianhaoma = contractResult.Data.SingleDataList.FirstOrDefault(it => it.Key.ToLower() == "gongjieren_zhengjianhaoma").Value,
                            jiekuanren_xingming = contractResult.Data.SingleDataList.FirstOrDefault(it => it.Key.ToLower() == "jiekuanren_xingming").Value,
                            jiekuanren_zhengjianhaoma = contractResult.Data.SingleDataList.FirstOrDefault(it => it.Key.ToLower() == "jiekuanren_zhengjianhaoma").Value,
                            daikuanqixian = contractResult.Data.SingleDataList.FirstOrDefault(it => it.Key.ToLower() == "daikuanqixian").Value,
                            fujiafeijine = contractResult.Data.SingleDataList.FirstOrDefault(it => it.Key.ToLower() == "fujiafeijine").Value,
                            yuehuankuanjine = contractResult.Data.SingleDataList.FirstOrDefault(it => it.Key.ToLower() == "yuehuankuanjine").Value,
                            jiekuanrenshoufukuanjine = contractResult.Data.SingleDataList.FirstOrDefault(it => it.Key.ToLower() == "jiekuanrenshoufukuanjine").Value,
                            daikuanlilv = contractResult.Data.SingleDataList.FirstOrDefault(it => it.Key.ToLower() == "daikuanlilv").Value,
                            chejiajine = contractResult.Data.SingleDataList.FirstOrDefault(it => it.Key.ToLower() == "chejiajine").Value,
                            fileUrl = url
                        }
                    };
                    SaveContractResult(instanceParameter.InstanceId, result_Contract);

                    return Json(new HandlerResult(this.FileID, attachmentId, url, result_Contract));
                case "9":
                    WarrantyResult warrantyResult = OcrHelper.WarrantyRecognizeAsync(buffer).Result;
                    Result_Policy result_Policy = new Result_Policy()
                    {
                        code = warrantyResult.ErrorCode == 200 ? "0" : warrantyResult.ErrorCode.ToString(),
                        message = warrantyResult.ErrorMsg,
                        data = new Result_Policy.data_policy
                        {
                            beibaoxianren = warrantyResult.Data.SingleDataList.FirstOrDefault(it => it.Key.ToLower() == "beibaoxianren").Value,
                            zhengjianhaoma = warrantyResult.Data.SingleDataList.FirstOrDefault(it => it.Key.ToLower() == "zhengjianhaoma").Value,
                            chejiahao = warrantyResult.Data.SingleDataList.FirstOrDefault(it => it.Key.ToLower() == "chejiahao").Value,
                            baofeiheji = warrantyResult.Data.SingleDataList.FirstOrDefault(it => it.Key.ToLower() == "baofeiheji").Value,
                            shifouyoubujimianpei = warrantyResult.Data.SingleDataList.FirstOrDefault(it => it.Key.ToLower() == "shifouyoubujimianpei").Value,
                            shifouyousanzexian = warrantyResult.Data.SingleDataList.FirstOrDefault(it => it.Key.ToLower() == "shifouyousanzexian").Value,
                            diyishouyiren = warrantyResult.Data.SingleDataList.FirstOrDefault(it => it.Key.ToLower() == "diyishouyiren").Value,
                            fileUrl = url
                        }
                    };
                    SavePolicyResult(instanceParameter.InstanceId, result_Policy);

                    return Json(new HandlerResult(this.FileID, attachmentId, url, result_Policy));
            }

            return null;
        }

        [HttpPost]
        public JsonResult DiscernImageInfo(Parameter_OCR para_OCR, Parameter_Instance para_Instance, bool isSheetAttachment)
        {
            var result = string.Empty;
            try
            {
                //参数日志
                AppUtility.Engine.LogWriter.Write("OCR识别 DiscernImageInfo参数，Parameter_OCR-->" + JsonConvert.SerializeObject(para_OCR) + ",Parameter_Instance-->" + JsonConvert.SerializeObject(para_Instance) + ",isSheetAttachment-->" + isSheetAttachment);
                //增加判断文件是否为空
                if (Request.Files == null || Request.Files.Count == 0)
                {
                    var obj = new
                    {
                        code = "-100",
                        message = "文件为空，请重新上传",
                        data = ""
                    };
                    if (isSheetAttachment)
                    {
                        string url = AppConfig.GetReadAttachmentUrl(this.IsMobile, para_Instance.SchemaCode, "", "");
                        return Json(new HandlerResult(this.FileID, "", url, obj));
                    }
                    return Json(obj, JsonRequestBehavior.AllowGet);
                }

                var file = Request.Files[0];

                if (ocr_server == "dongzheng")
                {
                    return DiscernImageInfoDongzheng(para_OCR.imageTyep, para_Instance, isSheetAttachment, file);
                }
                else
                {
                    return DiscernImageInfoZhengtong(para_OCR, para_Instance, isSheetAttachment, file);
                }

            }
            catch (Exception ex)
            {
                AppUtility.Engine.LogWriter.Write("OCR识别异常-->" + ex.ToString());
                if (isSheetAttachment)
                {
                    string url = AppConfig.GetReadAttachmentUrl(this.IsMobile, para_Instance.SchemaCode, "", "");
                    return Json(new HandlerResult(this.FileID, "", url, new { code = "-10001", message = "OCR识别失败，请联系管理员" }));
                }
                return Json(new { code = "-10001", message = "OCR识别失败，请联系管理员" }, JsonRequestBehavior.AllowGet);
            }
            
        }

        /// <summary>
        /// 调用正通ocr识别
        /// </summary>
        /// <param name="para_OCR"></param>
        /// <param name="para_Instance"></param>
        /// <param name="isSheetAttachment"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        private JsonResult DiscernImageInfoZhengtong(Parameter_OCR para_OCR, Parameter_Instance para_Instance, bool isSheetAttachment,HttpPostedFileBase file)
        {
            string result = string.Empty;
            byte[] contents = new byte[file.ContentLength];
            try
            {
                //Header参数
                NameValueCollection nv_Header = new NameValueCollection();
                nv_Header.Add("Authorization", para_OCR.Authorization);
                //Body参数
                NameValueCollection nv_body = new NameValueCollection();
                nv_body.Add("platformType", para_OCR.platformType);
                nv_body.Add("imageTyep", para_OCR.imageTyep);
                nv_body.Add("isCutImage", para_OCR.isCutImage);
                //图片内容
                file.InputStream.Read(contents, 0, file.ContentLength);
                //OCR识别
                result = HttpHelper.UploadFileEx(ocr_discern_url, nv_Header, nv_body, "multipartFile", file.FileName, contents, file.ContentType, 15, httpVersion: 1.0);
                
            }
            catch (Exception ex)
            {
                AppUtility.Engine.LogWriter.Write("OCR识别异常New-->" + ex.ToString());
                if (isSheetAttachment)
                {
                    string url = AppConfig.GetReadAttachmentUrl(this.IsMobile, para_Instance.SchemaCode, "", "");
                    return Json(new HandlerResult(this.FileID, "", url, new { code = "-10001", message = "OCR识别失败，请联系管理员" }));
                }
                return Json(new { code = "-10001", message = "OCR识别失败，请联系管理员" }, JsonRequestBehavior.AllowGet);
            }

            switch (para_OCR.imageTyep)
            {
                case "1"://银行卡
                    #region 银行卡
                    var v_bankcard = JsonConvert.DeserializeObject<Result_bankcard>(result);
                    string att_id_bankcard = "";
                    if (v_bankcard.code == "0")
                    {
                        if (para_OCR.isCutImage == "1" && !string.IsNullOrEmpty(v_bankcard.data.fileUrl))//进行了切边，把切边后的图片保存到H3
                        {
                            att_id_bankcard = UploadOcrImg(file.FileName, v_bankcard.data.fileUrl, para_Instance);
                        }
                        else//没有切边后图片，把原图保存到H3
                        {
                            att_id_bankcard = UploadFile(file.FileName, file.ContentType, contents, para_Instance);
                            v_bankcard.data.fileUrl = "/Portal/ReadAttachment/Read?BizObjectSchemaCode=" + para_Instance.SchemaCode + "&BizObjectID=" + para_Instance.BizObjectId + "&AttachmentID=" + att_id_bankcard + "&OpenMethod=0";
                        }
                        SaveBankCardResult(para_Instance.InstanceId, v_bankcard);
                    }
                    if (isSheetAttachment)
                    {
                        //之前识别失败是:10003,现在修改成了99999
                        //if (v_bankcard.code == "10003")
                        if (v_bankcard.code == "99999")
                        {
                            att_id_bankcard = UploadFile(file.FileName, file.ContentType, contents, para_Instance);
                            v_bankcard.data = new Result_bankcard.data_bankcard
                            {
                                fileUrl = "/Portal/ReadAttachment/Read?BizObjectSchemaCode=" + para_Instance.SchemaCode + "&BizObjectID=" + para_Instance.BizObjectId + "&AttachmentID=" + att_id_bankcard + "&OpenMethod=0"
                            };
                            v_bankcard.code = "0";
                        }
                        string url = AppConfig.GetReadAttachmentUrl(this.IsMobile, para_Instance.SchemaCode, "", att_id_bankcard);
                        return Json(new HandlerResult(this.FileID, att_id_bankcard, url, v_bankcard));
                    }
                    return Json(v_bankcard, JsonRequestBehavior.AllowGet);
                #endregion
                case "2"://购车发票
                    #region 购车发票
                    var v_vehicleInvoice = JsonConvert.DeserializeObject<Result_vehicleInvoice>(result);
                    string att_id_vehicleInvoice = "";
                    if (v_vehicleInvoice.code == "0")//识别成功，才保存图片
                    {
                        if (string.IsNullOrEmpty(v_vehicleInvoice.data.fileUrl))//没有切边后图片，把原图保存到H3
                        {
                            att_id_vehicleInvoice = UploadFile(file.FileName, file.ContentType, contents, para_Instance);
                            v_vehicleInvoice.data.fileUrl = "/Portal/ReadAttachment/Read?BizObjectSchemaCode=" + para_Instance.SchemaCode + "&BizObjectID=" + para_Instance.BizObjectId + "&AttachmentID=" + att_id_vehicleInvoice + "&OpenMethod=0";
                        }
                        else//进行了切边，把切边后的图片保存到H3
                        {
                            att_id_vehicleInvoice = UploadOcrImg(file.FileName, v_vehicleInvoice.data.fileUrl, para_Instance);
                        }
                        SaveVehicleInvoiceResult(para_Instance.InstanceId, v_vehicleInvoice);
                    }
                    if (isSheetAttachment)
                    {
                        string url = AppConfig.GetReadAttachmentUrl(this.IsMobile, para_Instance.SchemaCode, "", att_id_vehicleInvoice);
                        return Json(new HandlerResult(this.FileID, att_id_vehicleInvoice, url, v_vehicleInvoice));
                    }
                    return Json(v_vehicleInvoice, JsonRequestBehavior.AllowGet);
                #endregion
                case "3"://身份证
                    #region 身份证
                    var v_idcard = JsonConvert.DeserializeObject<Result_idcard>(result);
                    string att_id_idcard = "";
                    if (v_idcard.code == "0")//识别成功，才保存图片
                    {
                        if (para_OCR.isCutImage == "1" && !string.IsNullOrEmpty(v_idcard.data.fileUrl))//进行了切边，把切边后的图片保存到H3
                        {
                            att_id_idcard = UploadOcrImg(file.FileName, v_idcard.data.fileUrl, para_Instance);
                        }
                        else//没有切边后图片，把原图保存到H3
                        {
                            att_id_idcard = UploadFile(file.FileName, file.ContentType, contents, para_Instance);
                            v_idcard.data.fileUrl = "/Portal/ReadAttachment/Read?BizObjectSchemaCode=" + para_Instance.SchemaCode + "&BizObjectID=" + para_Instance.BizObjectId + "&AttachmentID=" + att_id_idcard + "&OpenMethod=0";
                        }
                        SaveIdCardResult(para_Instance.InstanceId, v_idcard);
                    }
                    if (isSheetAttachment)
                    {
                        //之前识别失败是:10003,现在修改成了99999
                        //if (v_idcard.code == "10003")
                        if (v_idcard.code == "99999")
                        {
                            att_id_idcard = UploadFile(file.FileName, file.ContentType, contents, para_Instance);
                            v_idcard.data = new Result_idcard.data_idcard
                            {
                                fileUrl = "/Portal/ReadAttachment/Read?BizObjectSchemaCode=" + para_Instance.SchemaCode + "&BizObjectID=" + para_Instance.BizObjectId + "&AttachmentID=" + att_id_idcard + "&OpenMethod=0"
                            };
                            v_idcard.code = "0";
                        }
                        string url = AppConfig.GetReadAttachmentUrl(this.IsMobile, para_Instance.SchemaCode, "", att_id_idcard);
                        return Json(new HandlerResult(this.FileID, att_id_idcard, url, v_idcard));
                    }
                    return Json(v_idcard, JsonRequestBehavior.AllowGet);
                #endregion
                case "4"://驾驶证
                    #region 驾驶证
                    var v_drivingLicense = JsonConvert.DeserializeObject<Result_drivingLicense>(result);
                    string att_id_drivingLicense = "";
                    if (v_drivingLicense.code == "0")//识别成功，才保存图片
                    {
                        if (string.IsNullOrEmpty(v_drivingLicense.data.fileUrl))//没有切边后图片，把原图保存到H3
                        {
                            att_id_drivingLicense = UploadFile(file.FileName, file.ContentType, contents, para_Instance);
                            v_drivingLicense.data.fileUrl = "/Portal/ReadAttachment/Read?BizObjectSchemaCode=" + para_Instance.SchemaCode + "&BizObjectID=" + para_Instance.BizObjectId + "&AttachmentID=" + att_id_drivingLicense + "&OpenMethod=0";
                        }
                        else//进行了切边，把切边后的图片保存到H3
                        {
                            att_id_drivingLicense = UploadOcrImg(file.FileName, v_drivingLicense.data.fileUrl, para_Instance);
                        }
                        SaveDrivingCardResult(para_Instance.InstanceId, v_drivingLicense);
                    }
                    if (isSheetAttachment)
                    {
                        string url = AppConfig.GetReadAttachmentUrl(this.IsMobile, para_Instance.SchemaCode, "", att_id_drivingLicense);
                        return Json(new HandlerResult(this.FileID, att_id_drivingLicense, url, v_drivingLicense));
                    }
                    return Json(v_drivingLicense, JsonRequestBehavior.AllowGet);
                #endregion
                case "5"://车架号
                    #region 车架号
                    var v_vinNo = JsonConvert.DeserializeObject<Result_vinNo>(result);
                    string att_id_vinNo = "";
                    if (v_vinNo.code == "0")//识别成功，才保存图片
                    {
                        if (string.IsNullOrEmpty(v_vinNo.data.fileUrl))//没有切边后图片，把原图保存到H3
                        {
                            att_id_vinNo = UploadFile(file.FileName, file.ContentType, contents, para_Instance);
                            v_vinNo.data.fileUrl = "/Portal/ReadAttachment/Read?BizObjectSchemaCode=" + para_Instance.SchemaCode + "&BizObjectID=" + para_Instance.BizObjectId + "&AttachmentID=" + att_id_vinNo + "&OpenMethod=0";
                        }
                        else//进行了切边，把切边后的图片保存到H3
                        {
                            att_id_vinNo = UploadOcrImg(file.FileName, v_vinNo.data.fileUrl, para_Instance);
                        }
                        SaveVinNoResult(para_Instance.InstanceId, v_vinNo);
                    }
                    if (isSheetAttachment)
                    {
                        string url = AppConfig.GetReadAttachmentUrl(this.IsMobile, para_Instance.SchemaCode, "", att_id_vinNo);
                        return Json(new HandlerResult(this.FileID, att_id_vinNo, url, v_vinNo));
                    }
                    return Json(v_vinNo, JsonRequestBehavior.AllowGet);
                #endregion
                case "8"://抵押贷款合同
                    #region 抵押贷款合同
                    var v_Contract = JsonConvert.DeserializeObject<Result_Contract>(result);
                    string att_id_Contract = "";
                    if (v_Contract.code == "0")//识别成功，才保存图片
                    {
                        if (string.IsNullOrEmpty(v_Contract.data.fileUrl))//没有切边后图片，把原图保存到H3
                        {
                            att_id_vinNo = UploadFile(file.FileName, file.ContentType, contents, para_Instance);
                            v_Contract.data.fileUrl = "/Portal/ReadAttachment/Read?BizObjectSchemaCode=" + para_Instance.SchemaCode + "&BizObjectID=" + para_Instance.BizObjectId + "&AttachmentID=" + att_id_vinNo + "&OpenMethod=0";
                        }
                        else//进行了切边，把切边后的图片保存到H3
                        {
                            att_id_vinNo = UploadOcrImg(file.FileName, v_Contract.data.fileUrl, para_Instance);
                        }
                        SaveContractResult(para_Instance.InstanceId, v_Contract);
                    }
                    if (isSheetAttachment)
                    {
                        string url = AppConfig.GetReadAttachmentUrl(this.IsMobile, para_Instance.SchemaCode, "", att_id_Contract);
                        return Json(new HandlerResult(this.FileID, att_id_Contract, url, v_Contract));
                    }
                    return Json(v_Contract, JsonRequestBehavior.AllowGet);

                #endregion
                case "7"://扣款授权书
                    #region 扣款授权书
                    var v_Withholdauthorizingletter = JsonConvert.DeserializeObject<Result_DeductionsAuthorized>(result);
                    string att_id_Withholdauthorizingletter = "";
                    if (v_Withholdauthorizingletter.code == "0")//识别成功，才保存图片
                    {
                        if (string.IsNullOrEmpty(v_Withholdauthorizingletter.data.fileUrl))//没有切边后图片，把原图保存到H3
                        {
                            att_id_vinNo = UploadFile(file.FileName, file.ContentType, contents, para_Instance);
                            v_Withholdauthorizingletter.data.fileUrl = "/Portal/ReadAttachment/Read?BizObjectSchemaCode=" + para_Instance.SchemaCode + "&BizObjectID=" + para_Instance.BizObjectId + "&AttachmentID=" + att_id_vinNo + "&OpenMethod=0";
                        }
                        else//进行了切边，把切边后的图片保存到H3
                        {
                            att_id_vinNo = UploadOcrImg(file.FileName, v_Withholdauthorizingletter.data.fileUrl, para_Instance);
                        }
                        SaveDeductionsAuthorizedResult(para_Instance.InstanceId, v_Withholdauthorizingletter);
                    }
                    if (isSheetAttachment)
                    {
                        string url = AppConfig.GetReadAttachmentUrl(this.IsMobile, para_Instance.SchemaCode, "", att_id_Withholdauthorizingletter);
                        return Json(new HandlerResult(this.FileID, att_id_Withholdauthorizingletter, url, v_Withholdauthorizingletter));
                    }
                    return Json(v_Withholdauthorizingletter, JsonRequestBehavior.AllowGet);

                #endregion

                case "9"://保单识别
                    #region 保单识别
                    var v_Policy = JsonConvert.DeserializeObject<Result_Policy>(result);
                    string att_id_Policy = "";
                    if (v_Policy.code == "0")//识别成功，才保存图片
                    {
                        if (string.IsNullOrEmpty(v_Policy.data.fileUrl))//没有切边后图片，把原图保存到H3
                        {
                            att_id_vinNo = UploadFile(file.FileName, file.ContentType, contents, para_Instance);
                            v_Policy.data.fileUrl = "/Portal/ReadAttachment/Read?BizObjectSchemaCode=" + para_Instance.SchemaCode + "&BizObjectID=" + para_Instance.BizObjectId + "&AttachmentID=" + att_id_vinNo + "&OpenMethod=0";
                        }
                        else//进行了切边，把切边后的图片保存到H3
                        {
                            att_id_vinNo = UploadOcrImg(file.FileName, v_Policy.data.fileUrl, para_Instance);
                        }
                        SavePolicyResult(para_Instance.InstanceId, v_Policy);
                    }
                    if (isSheetAttachment)
                    {
                        string url = AppConfig.GetReadAttachmentUrl(this.IsMobile, para_Instance.SchemaCode, "", att_id_Policy);
                        return Json(new HandlerResult(this.FileID, att_id_Policy, url, v_Policy));
                    }
                    return Json(v_Policy, JsonRequestBehavior.AllowGet);

                #endregion
                default: break;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        private JsonResult DiscernImageInfoDongzheng(string imageType, Parameter_Instance para_Instance, bool isSheetAttachment, HttpPostedFileBase file)
        {
            var result = string.Empty;

            byte[] contents = new byte[file.ContentLength];
            file.InputStream.Read(contents, 0, file.ContentLength);
            try
            {
                switch (imageType)
                {
                    case "1"://银行卡
                        #region 银行卡

                        var att_id_bankcard = string.Empty;
                        var bankCardResult = OcrHelper.BankCardRecognizeAsync(contents).Result;
                        var dzbankresult = new DZBankCardResult { code = bankCardResult.ErrorCode.ToString() };
                        if (bankCardResult.ErrorCode == 0)
                        {
                            att_id_bankcard = UploadFile(file.FileName, file.ContentType, contents, para_Instance);
                            bankCardResult.fileUrl = "/Portal/ReadAttachment/Read?BizObjectSchemaCode=" + para_Instance.SchemaCode + "&BizObjectID=" + para_Instance.BizObjectId + "&AttachmentID=" + att_id_bankcard + "&OpenMethod=0";
                            SaveBankCardResult(para_Instance.InstanceId, bankCardResult);
                        }

                        if (isSheetAttachment)
                        {
                            if (bankCardResult.ErrorCode != 0)
                            {
                                att_id_bankcard = UploadFile(file.FileName, file.ContentType, contents, para_Instance);
                            }
                            string url = AppConfig.GetReadAttachmentUrl(this.IsMobile, para_Instance.SchemaCode, "", att_id_bankcard);
                            bankCardResult.fileUrl = url;
                            dzbankresult.data = bankCardResult;
                            return Json(new HandlerResult(this.FileID, att_id_bankcard, url, dzbankresult));
                        }
                        dzbankresult.data = bankCardResult;
                        return Json(dzbankresult, JsonRequestBehavior.AllowGet);

                        #endregion
                    case "2"://购车发票
                        #region 购车发票

                        var att_id_vehicle = string.Empty;
                        var vehicleResult = OcrHelper.VehicleInvoiceRecognizeAsync(contents).Result;
                        var dzvehicleresult = new DZVehicleInvoiceResult { code = vehicleResult.ErrorCode.ToString() };
                        if (vehicleResult.ErrorCode == 0)
                        {
                            att_id_bankcard = UploadFile(file.FileName, file.ContentType, contents, para_Instance);
                            vehicleResult.fileUrl = "/Portal/ReadAttachment/Read?BizObjectSchemaCode=" + para_Instance.SchemaCode + "&BizObjectID=" + para_Instance.BizObjectId + "&AttachmentID=" + att_id_vehicle + "&OpenMethod=0";
                            SaveVehicleInvoiceResult(para_Instance.InstanceId, vehicleResult);
                        }

                        if (isSheetAttachment)
                        {
                            string url = AppConfig.GetReadAttachmentUrl(this.IsMobile, para_Instance.SchemaCode, "", att_id_vehicle);
                            return Json(new HandlerResult(this.FileID, att_id_vehicle, url, dzvehicleresult));
                        }
                        dzvehicleresult.data = vehicleResult;
                        return Json(dzvehicleresult, JsonRequestBehavior.AllowGet);

                        #endregion

                    case "3"://身份证
                        #region 身份证

                        var att_id_idcard = string.Empty;
                        var idCardResult = OcrHelper.IdCardRecognizeAsync(contents).Result;
                        var dzresult = new DZIdCardResult { code = idCardResult.ErrorCode.ToString() };
                        if (idCardResult.ErrorCode == 0)
                        {
                            att_id_idcard = UploadFile(file.FileName, file.ContentType, contents, para_Instance);
                            idCardResult.fileUrl = "/Portal/ReadAttachment/Read?BizObjectSchemaCode=" + para_Instance.SchemaCode + "&BizObjectID=" + para_Instance.BizObjectId + "&AttachmentID=" + att_id_idcard + "&OpenMethod=0";
                            SaveIdCardResult(para_Instance.InstanceId, idCardResult);
                        }

                        if (isSheetAttachment)
                        {
                            if (idCardResult.ErrorCode != 0)
                            {
                                att_id_idcard = UploadFile(file.FileName, file.ContentType, contents, para_Instance);
                            }
                            string url = AppConfig.GetReadAttachmentUrl(this.IsMobile, para_Instance.SchemaCode, "", att_id_idcard);
                            idCardResult.fileUrl = url;
                            dzresult.data = idCardResult;
                            return Json(new HandlerResult(this.FileID, att_id_idcard, url, dzresult));
                        }
                        dzresult.data = idCardResult;
                        return Json(dzresult, JsonRequestBehavior.AllowGet);

                        #endregion
                    case "4"://驾驶证
                        #region 驾驶证

                        var att_id_drivingLicense = string.Empty;
                        var driverLicenseResult = OcrHelper.DriverLicenseRecognizeAsync(contents).Result;
                        var dzdriverLicenseResult = new DZDriverLicenseResult { code = driverLicenseResult.ErrorCode.ToString() };
                        if (driverLicenseResult.ErrorCode == 0)
                        {
                            att_id_bankcard = UploadFile(file.FileName, file.ContentType, contents, para_Instance);
                            driverLicenseResult.fileUrl = "/Portal/ReadAttachment/Read?BizObjectSchemaCode=" + para_Instance.SchemaCode + "&BizObjectID=" + para_Instance.BizObjectId + "&AttachmentID=" + att_id_drivingLicense + "&OpenMethod=0";
                            SaveDrivingCardResult(para_Instance.InstanceId, driverLicenseResult);
                        }

                        if (isSheetAttachment)
                        {
                            string url = AppConfig.GetReadAttachmentUrl(this.IsMobile, para_Instance.SchemaCode, "", att_id_drivingLicense);
                            return Json(new HandlerResult(this.FileID, att_id_drivingLicense, url, dzdriverLicenseResult));
                        }
                        dzdriverLicenseResult.data = driverLicenseResult;
                        return Json(dzdriverLicenseResult, JsonRequestBehavior.AllowGet);

                        #endregion
                        
                    case "5"://车架号
                        #region 车架号

                        var att_id_vin = string.Empty;
                        var vinResult = OcrHelper.VinRecognizeAsync(contents).Result;
                        var dzvinResult = new DZVinResult { code = vinResult.ErrorCode.ToString() };
                        if (vinResult.ErrorCode == 0)
                        {
                            att_id_bankcard = UploadFile(file.FileName, file.ContentType, contents, para_Instance);
                            vinResult.fileUrl = "/Portal/ReadAttachment/Read?BizObjectSchemaCode=" + para_Instance.SchemaCode + "&BizObjectID=" + para_Instance.BizObjectId + "&AttachmentID=" + att_id_vin + "&OpenMethod=0";
                            SaveVinNoResult(para_Instance.InstanceId, vinResult);
                        }

                        if (isSheetAttachment)
                        {
                            string url = AppConfig.GetReadAttachmentUrl(this.IsMobile, para_Instance.SchemaCode, "", att_id_vin);
                            return Json(new HandlerResult(this.FileID, att_id_vin, url, dzvinResult));
                        }
                        dzvinResult.data = vinResult;
                        return Json(dzvinResult, JsonRequestBehavior.AllowGet);

                        #endregion
                        
                    default: break;
                }
            }
            catch (Exception ex)
            {
                AppUtility.Engine.LogWriter.Write("OCR识别异常New-->" + ex.ToString());
                if (isSheetAttachment)
                {
                    string url = AppConfig.GetReadAttachmentUrl(this.IsMobile, para_Instance.SchemaCode, "", "");
                    return Json(new HandlerResult(this.FileID, "", url, new { code = "-10001", message = "OCR识别失败，请联系管理员" }));
                }
                return Json(new { code = "-10001", message = "OCR识别失败，请联系管理员" }, JsonRequestBehavior.AllowGet);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult PictureCompress(string Authorization, string x, string y, string factor, string quality, string rotate, Parameter_Instance para_Instance)
        {
            try
            {
                AppUtility.Engine.LogWriter.Write(string.Format("图片压缩 PictureCompress参数，Authorization-->{0},x-->{1},y-->{2},factor-->{3},quality-->{4},rotate-->{5},Parameter_Instance-->{6}",
                    Authorization, x, y, factor, quality, rotate, JsonConvert.SerializeObject(para_Instance)
                    ));
                if ((string.IsNullOrEmpty(x) || string.IsNullOrEmpty(y))
                    && string.IsNullOrEmpty(factor)
                    && string.IsNullOrEmpty(quality)
                    && string.IsNullOrEmpty(rotate))
                {
                    return Json(new HandlerResult(this.FileID, "", "", new { code = -10001, data = "", message = "参数为空" }));
                }
                //增加判断文件是否为空
                if (Request.Files == null || Request.Files.Count == 0)
                {
                    var obj = new
                    {
                        code = "-100",
                        message = "文件为空，请重新上传",
                        data = ""
                    };

                    string url_emp = AppConfig.GetReadAttachmentUrl(this.IsMobile, para_Instance.SchemaCode, "", "");
                    return Json(new HandlerResult(this.FileID, "", url_emp, obj));
                }

                var file = Request.Files[0];
                string[] supper_type = new string[] { ".bmp", ".jpg", ".jpeg", ".gif", ".png" };

                //1.没有设置全局图片压缩，不进行图片压缩.
                //2.文件大小小于设置的大小,不进行图片压缩.
                //3.非图片文件，直接保存，不做压缩
                if (!pic_compress_enable || file.ContentLength < pic_compress_minsize_kb * 1024 ||
                    !supper_type.Contains(Path.GetExtension(file.FileName).ToLower()))
                {
                    //文件或图片
                    byte[] att_contents = new byte[file.ContentLength];
                    file.InputStream.Read(att_contents, 0, file.ContentLength);
                    string att_id = UploadFile(file.FileName, file.ContentType, att_contents, para_Instance);
                    return Json(new HandlerResult(this.FileID, att_id, AppConfig.GetReadAttachmentUrl(this.IsMobile, para_Instance.SchemaCode, "", att_id), null));
                }

                string result = "";
                try
                {
                    var picture_Compress_Url = "";
                    //Header参数
                    NameValueCollection nv_Header = new NameValueCollection();
                    nv_Header.Add("Authorization", Authorization);
                    //Body参数
                    NameValueCollection nv_body = new NameValueCollection();
                    #region 给Body参数赋值
                    if (!string.IsNullOrEmpty(x) && !string.IsNullOrEmpty(y))
                    {
                        picture_Compress_Url = pic_compress_xy_url;
                        nv_body.Add("x", x);
                        nv_body.Add("y", y);
                    }
                    else if (!string.IsNullOrEmpty(factor))
                    {
                        picture_Compress_Url = pic_compress_factor_url;
                        nv_body.Add("factor", factor);
                    }
                    else if (!string.IsNullOrEmpty(quality))
                    {
                        picture_Compress_Url = pic_compress_quality_url;
                        nv_body.Add("quality", quality);
                    }
                    else if (!string.IsNullOrEmpty(rotate))
                    {
                        picture_Compress_Url = pic_compress_rotate_url;
                        nv_body.Add("rotate", rotate);
                    }
                    #endregion

                    //图片内容
                    byte[] contents = new byte[file.ContentLength];
                    file.InputStream.Read(contents, 0, file.ContentLength);
                    //图片压缩
                    result = HttpHelper.UploadFileEx(picture_Compress_Url, nv_Header, nv_body, "multipartFile", file.FileName, contents, file.ContentType, 15, httpVersion: 1.0);
                }
                catch (Exception ex)
                {
                    AppUtility.Engine.LogWriter.Write("图片压缩异常New-->" + ex.ToString());
                    return Json(new HandlerResult(this.FileID, "", "", new { code = -10002, data = "", message = "图片压缩异常" }));
                }

                var v_result = JsonConvert.DeserializeObject<Result_PictureCompress>(result);
                string id = "";
                if (v_result.code == "0")//压缩成功，才保存图片
                {
                    id = UploadOcrImg(file.FileName, v_result.data.fileUrl, para_Instance);
                    SavePictureCompressResult(para_Instance.InstanceId, v_result);
                }
                string url = AppConfig.GetReadAttachmentUrl(this.IsMobile, para_Instance.SchemaCode, "", id);
                return Json(new HandlerResult(this.FileID, id, url, v_result));
            }
            catch (Exception ex)
            {
                AppUtility.Engine.LogWriter.Write("图片压缩异常-->" + ex.ToString());
                return Json(new HandlerResult(this.FileID, "", "", new { code = -10002, data = "", message = "图片压缩异常" }));
            }
        }

        #region Basic Method

        /// <summary>
        /// 保存文件到H3
        /// </summary>
        /// <param name="FileName"></param>
        /// <param name="FileType"></param>
        /// <param name="Contents"></param>
        /// <param name="para_Instance"></param>
        /// <returns></returns>
        private string UploadFile(string FileName, string FileType, byte[] Contents, Parameter_Instance para_Instance)
        {
            // 添加这个附件
            OThinker.H3.Data.Attachment attachment = new OThinker.H3.Data.Attachment();
            attachment.ObjectID = Guid.NewGuid().ToString();
            attachment.Content = Contents;
            attachment.ContentType = FileType;
            attachment.CreatedBy = this.UserValidator.UserID;
            attachment.CreatedTime = DateTime.Now;
            attachment.FileName = Path.GetFileName(FileName);
            attachment.LastVersion = true;
            attachment.ModifiedBy = null;
            attachment.ModifiedTime = System.DateTime.Now;
            attachment.BizObjectSchemaCode = para_Instance.SchemaCode;
            attachment.ContentLength = Contents.Length;
            attachment.BizObjectId = para_Instance.BizObjectId;
            attachment.DataField = para_Instance.DataField;
            return this.Engine.BizObjectManager.AddAttachment(attachment);
        }
        /// <summary>
        /// 保存识别后的图片
        /// </summary>
        /// <param name="FileName"></param>
        /// <param name="Url"></param>
        /// <param name="imageTyep"></param>
        /// <param name="para_Instance"></param>
        /// <returns></returns>
        private string UploadOcrImg(string FileName, string Url, Parameter_Instance para_Instance)
        {
            try
            {
                //1.把图片链接保存成一张图片保存到服务器；
                string file_name = Url.Substring(Url.LastIndexOf("/"));
                string file_ext = ".jpg";
                if (file_name.LastIndexOf(".") > 0)
                    file_ext = file_name.Substring(file_name.LastIndexOf("."));
                string temp_filename = Server.MapPath("/Portal/TempImages/" + Guid.NewGuid() + file_ext);
                WebRequest imgRequst = WebRequest.Create(Url);
                var img_stream = imgRequst.GetResponse().GetResponseStream();
                System.Drawing.Image downImage = System.Drawing.Image.FromStream(img_stream);
                downImage.Save(temp_filename);
                downImage.Dispose();

                //2.读取保存的图片 
                FileStream fileStream = new FileStream(temp_filename, FileMode.Open, FileAccess.Read, FileShare.Read);
                // 读取文件的 byte[] 
                byte[] bytes = new byte[fileStream.Length];
                fileStream.Read(bytes, 0, bytes.Length);
                fileStream.Close();

                //3.把图片保存到附件中；
                string attr_id = UploadFile(FileName, MimeMapping.GetMimeMapping(temp_filename), bytes, para_Instance);

                //4.把图片删除；
                System.IO.File.Delete(temp_filename);

                return attr_id;
            }
            catch (Exception ex)
            {
                AppUtility.Engine.LogWriter.Write(string.Format("保存OCR识别后的文件异常，FileName-->{0},Url-->{1},para_Instance-->{2},Exception-->{3}", FileName, Url, JsonConvert.SerializeObject(para_Instance), ex.ToString()));
                return "";
            }
        }

        #region 保存OCR识别后的结果
        /// <summary>
        /// 保存身份证信息
        /// </summary>
        /// <param name="instanceID"></param>
        /// <param name="idcard"></param>
        private void SaveIdCardResult(string instanceID, Result_idcard idcard)
        {
            var d = idcard.data;
            string sql_Insert = "insert into C_OCR_Idcard(objectid,InstanceId,createdtime,name,idNumber,birthday,sex,people,address,Issueauthority,validity,fileUrl) values('" + Guid.NewGuid() + "','{0}',sysdate,'{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}')";
            sql_Insert = string.Format(sql_Insert, instanceID, d.name, d.idNumber, d.birthday, d.sex, d.people, d.address, d.issueAuthority, d.validity, d.fileUrl);
            int n = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(sql_Insert);
            if (n > 0)
            {
                AppUtility.Engine.LogWriter.Write("保存OCR身份证信息成功,受影响行数-->" + n + ",InstanceID-->" + instanceID);
            }
            else
            {
                AppUtility.Engine.LogWriter.Write("保存OCR身份证信息失败,受影响行数-->" + n + ",SQL-->" + sql_Insert);
            }
        }

        private void SaveIdCardResult(string instanceID, IdCardResult idcard)
        {
            string sql_Insert = "insert into C_OCR_Idcard(objectid,InstanceId,createdtime,name,idNumber,birthday,sex,people,address,Issueauthority,validity,fileUrl) values('" + Guid.NewGuid() + "','{0}',sysdate,'{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}')";
            sql_Insert = string.Format(sql_Insert, instanceID, idcard.name, idcard.idNumber, idcard.birthday, idcard.sex, idcard.people, idcard.address, idcard.issueAuthority, idcard.validity, idcard.fileUrl);
            int n = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(sql_Insert);
            if (n > 0)
            {
                AppUtility.Engine.LogWriter.Write("保存OCR身份证信息成功,受影响行数-->" + n + ",InstanceID-->" + instanceID);
            }
            else
            {
                AppUtility.Engine.LogWriter.Write("保存OCR身份证信息失败,受影响行数-->" + n + ",SQL-->" + sql_Insert);
            }
        }

        /// <summary>
        /// 保存银行卡信息
        /// </summary>
        /// <param name="instanceID"></param>
        /// <param name="bankcard"></param>
        private void SaveBankCardResult(string instanceID, Result_bankcard bankcard)
        {
            var d = bankcard.data;
            string sql_Insert = "insert into C_OCR_Bankcard(objectid,InstanceId,createdtime,cardtype,cardNumber,Cardvalidate,Holdername,Issuer,fileUrl) values('" + Guid.NewGuid() + "','{0}',sysdate,'{1}','{2}','{3}','{4}','{5}','{6}')";
            sql_Insert = string.Format(sql_Insert, instanceID, d.type, d.cardNumber, d.validate, d.holderName, d.issuer, d.fileUrl);
            int n = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(sql_Insert);
            if (n > 0)
            {
                AppUtility.Engine.LogWriter.Write("保存OCR银行卡信息成功,受影响行数-->" + n + ",InstanceID-->" + instanceID);
            }
            else
            {
                AppUtility.Engine.LogWriter.Write("保存OCR银行卡信息失败,受影响行数-->" + n + ",SQL-->" + sql_Insert);
            }
        }

        /// <summary>
        /// 保存银行卡信息
        /// </summary>
        /// <param name="instanceID"></param>
        /// <param name="bankcard"></param>
        private void SaveBankCardResult(string instanceID, BankCardResult bankcard)
        {
            var d = bankcard;
            string sql_Insert = "insert into C_OCR_Bankcard(objectid,InstanceId,createdtime,cardtype,cardNumber,Cardvalidate,Holdername,Issuer,fileUrl) values('" + Guid.NewGuid() + "','{0}',sysdate,'{1}','{2}','{3}','{4}','{5}','{6}')";
            sql_Insert = string.Format(sql_Insert, instanceID, d.type, d.cardNumber, d.validate, d.holderName, d.issuer, d.fileUrl);
            int n = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(sql_Insert);
            if (n > 0)
            {
                AppUtility.Engine.LogWriter.Write("保存OCR银行卡信息成功,受影响行数-->" + n + ",InstanceID-->" + instanceID);
            }
            else
            {
                AppUtility.Engine.LogWriter.Write("保存OCR银行卡信息失败,受影响行数-->" + n + ",SQL-->" + sql_Insert);
            }
        }

        /// <summary>
        /// 保存驾驶证信息
        /// </summary>
        /// <param name="instanceID"></param>
        /// <param name="drivingcard"></param>
        private void SaveDrivingCardResult(string instanceID, Result_drivingLicense drivingcard)
        {
            var d = drivingcard.data;
            string sql_Insert = @"insert into C_OCR_DrivingLicense(objectid,InstanceId,createdtime,address,birthday,driveType,drivingLicenseMainNationality,
                drivingLicenseMainNumber,drivingLicenseMainValidFor,drivingLicenseMainValidFrom,issueDate,name,sex,drivingLicenseDocumentNumber,type,fileUrl) 
values('" + Guid.NewGuid() + "','{0}',sysdate,'{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}')";
            sql_Insert = string.Format(sql_Insert, instanceID, d.address, d.birthday, d.driveType, d.drivingLicenseMainNationality,
                d.drivingLicenseMainNumber, d.drivingLicenseMainValidFor, d.drivingLicenseMainValidFrom, d.issueDate, d.name, d.sex, d.drivingLicenseDocumentNumber, d.type, d.fileUrl);
            int n = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(sql_Insert);
            if (n > 0)
            {
                AppUtility.Engine.LogWriter.Write("保存OCR驾驶证信息成功,受影响行数-->" + n + ",InstanceID-->" + instanceID);
            }
            else
            {
                AppUtility.Engine.LogWriter.Write("保存OCR驾驶证信息失败,受影响行数-->" + n + ",SQL-->" + sql_Insert);
            }
        }

        /// <summary>
        /// 保存驾驶证信息
        /// </summary>
        /// <param name="instanceID"></param>
        /// <param name="drivingcard"></param>
        private void SaveDrivingCardResult(string instanceID, DriverLicenseResult drivingcard)
        {
            var d = drivingcard;
            string sql_Insert = @"insert into C_OCR_DrivingLicense(objectid,InstanceId,createdtime,address,birthday,driveType,drivingLicenseMainNationality,
                drivingLicenseMainNumber,drivingLicenseMainValidFor,drivingLicenseMainValidFrom,issueDate,name,sex,drivingLicenseDocumentNumber,type,fileUrl) 
values('" + Guid.NewGuid() + "','{0}',sysdate,'{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}')";
            sql_Insert = string.Format(sql_Insert, instanceID, d.address, d.birthday, d.driveType, d.drivingLicenseMainNationality,
                d.drivingLicenseMainNumber, d.drivingLicenseMainValidFor, d.drivingLicenseMainValidFrom, d.issueDate, d.name, d.sex, d.drivingLicenseDocumentNumber, d.type, d.fileUrl);
            int n = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(sql_Insert);
            if (n > 0)
            {
                AppUtility.Engine.LogWriter.Write("保存OCR驾驶证信息成功,受影响行数-->" + n + ",InstanceID-->" + instanceID);
            }
            else
            {
                AppUtility.Engine.LogWriter.Write("保存OCR驾驶证信息失败,受影响行数-->" + n + ",SQL-->" + sql_Insert);
            }
        }

        /// <summary>
        /// 保存车架号信息
        /// </summary>
        /// <param name="instanceID"></param>
        /// <param name="vinno"></param>
        private void SaveVinNoResult(string instanceID, Result_vinNo vinno)
        {
            var d = vinno.data;
            string sql_Insert = "insert into C_OCR_VinNo(objectid,InstanceId,createdtime,vehicleLicenseMainVin,fileUrl) values('" + Guid.NewGuid() + "','{0}',sysdate,'{1}','{2}')";
            sql_Insert = string.Format(sql_Insert, instanceID, d.vehicleLicenseMainVin, d.fileUrl);
            int n = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(sql_Insert);
            if (n > 0)
            {
                AppUtility.Engine.LogWriter.Write("保存OCR车架号信息成功,受影响行数-->" + n + ",InstanceID-->" + instanceID);
            }
            else
            {
                AppUtility.Engine.LogWriter.Write("保存OCR车架号信息失败,受影响行数-->" + n + ",SQL-->" + sql_Insert);
            }
        }

        /// <summary>
        /// 保存车架号信息
        /// </summary>
        /// <param name="instanceID"></param>
        /// <param name="vinno"></param>
        private void SaveVinNoResult(string instanceID, VinResult vinno)
        {
            var d = vinno;
            string sql_Insert = "insert into C_OCR_VinNo(objectid,InstanceId,createdtime,vehicleLicenseMainVin,fileUrl) values('" + Guid.NewGuid() + "','{0}',sysdate,'{1}','{2}')";
            sql_Insert = string.Format(sql_Insert, instanceID, d.vehicleLicenseMainVin, d.fileUrl);
            int n = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(sql_Insert);
            if (n > 0)
            {
                AppUtility.Engine.LogWriter.Write("保存OCR车架号信息成功,受影响行数-->" + n + ",InstanceID-->" + instanceID);
            }
            else
            {
                AppUtility.Engine.LogWriter.Write("保存OCR车架号信息失败,受影响行数-->" + n + ",SQL-->" + sql_Insert);
            }
        }

        /// <summary>
        /// 保存购车发票信息
        /// </summary>
        /// <param name="instanceID"></param>
        /// <param name="vehicleInvoice"></param>
        private void SaveVehicleInvoiceResult(string instanceID, Result_vehicleInvoice vehicleInvoice)
        {
            var d = vehicleInvoice.data;
            string sql_Insert = @"insert into C_OCR_VehicleInvoice(objectid,InstanceId,createdtime,type,vehicleInvoiceBuyer,
vehicleInvoiceBuyerId,vehicleInvoiceCarModel,vehicleInvoiceCarMadePlace,vehicleInvoiceCertId,vehicleInvoiceEngineId,
vehicleInvoiceCarVin,vehicleInvoiceTotalPrice,vehicleInvoiceTotalPriceDigits,vehicleInvoiceTelephone,vehicleInvoiceIssueDate,
vehicleInvoiceDaima,vehicleInvoiceHaoma,vehicleInvoiceDealer,vehicleInvoiceJidaDaima,vehicleInvoiceJidaHaoma,vehicleInvoiceTaxRate,
vehicleInvoiceTaxAmount,vehicleInvoicePriceWithoutTax,fileUrl) 
values('" + Guid.NewGuid() + "','{0}', sysdate,'{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}')";
            sql_Insert = string.Format(sql_Insert, instanceID, d.type, d.vehicleInvoiceBuyer, d.vehicleInvoiceBuyerId, d.vehicleInvoiceCarModel,
                d.vehicleInvoiceCarMadePlace, d.vehicleInvoiceCertId, d.vehicleInvoiceEngineId, d.vehicleInvoiceCarVin, d.vehicleInvoiceTotalPrice,
                d.vehicleInvoiceTotalPriceDigits, d.vehicleInvoiceTelephone, d.vehicleInvoiceIssueDate, d.vehicleInvoiceDaima, d.vehicleInvoiceHaoma,
                d.vehicleInvoiceDealer, d.vehicleInvoiceJidaDaima, d.vehicleInvoiceJidaHaoma, d.vehicleInvoiceTaxRate, d.vehicleInvoiceTaxAmount,
                d.vehicleInvoicePriceWithoutTax, d.fileUrl);
            int n = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(sql_Insert);
            if (n > 0)
            {
                AppUtility.Engine.LogWriter.Write("保存OCR购车发票信息成功,受影响行数-->" + n + ",InstanceID-->" + instanceID);
            }
            else
            {
                AppUtility.Engine.LogWriter.Write("保存OCR购车发票信息失败,受影响行数-->" + n + ",SQL-->" + sql_Insert);
            }
        }

        /// <summary>
        /// 保存购车发票信息
        /// </summary>
        /// <param name="instanceID"></param>
        /// <param name="vehicleInvoice"></param>
        private void SaveVehicleInvoiceResult(string instanceID, VehicleInvoiceResult vehicleInvoice)
        {
            var d = vehicleInvoice;
            string sql_Insert = @"insert into C_OCR_VehicleInvoice(objectid,InstanceId,createdtime,type,vehicleInvoiceBuyer,
vehicleInvoiceBuyerId,vehicleInvoiceCarModel,vehicleInvoiceCarMadePlace,vehicleInvoiceCertId,vehicleInvoiceEngineId,
vehicleInvoiceCarVin,vehicleInvoiceTotalPrice,vehicleInvoiceTotalPriceDigits,vehicleInvoiceTelephone,vehicleInvoiceIssueDate,
vehicleInvoiceDaima,vehicleInvoiceHaoma,vehicleInvoiceDealer,vehicleInvoiceJidaDaima,vehicleInvoiceJidaHaoma,vehicleInvoiceTaxRate,
vehicleInvoiceTaxAmount,vehicleInvoicePriceWithoutTax,fileUrl) 
values('" + Guid.NewGuid() + "','{0}', sysdate,'{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}')";
            sql_Insert = string.Format(sql_Insert, instanceID, d.type, d.vehicleInvoiceBuyer, d.vehicleInvoiceBuyerId, d.vehicleInvoiceCarModel,
                d.vehicleInvoiceCarMadePlace, d.vehicleInvoiceCertId, d.vehicleInvoiceEngineId, d.vehicleInvoiceCarVin, d.vehicleInvoiceTotalPrice,
                d.vehicleInvoiceTotalPriceDigits, d.vehicleInvoiceTelephone, d.vehicleInvoiceIssueDate, d.vehicleInvoiceDaima, d.vehicleInvoiceHaoma,
                d.vehicleInvoiceDealer, d.vehicleInvoiceJidaDaima, d.vehicleInvoiceJidaHaoma, d.vehicleInvoiceTaxRate, d.vehicleInvoiceTaxAmount,
                d.vehicleInvoicePriceWithoutTax, d.fileUrl);
            int n = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(sql_Insert);
            if (n > 0)
            {
                AppUtility.Engine.LogWriter.Write("保存OCR购车发票信息成功,受影响行数-->" + n + ",InstanceID-->" + instanceID);
            }
            else
            {
                AppUtility.Engine.LogWriter.Write("保存OCR购车发票信息失败,受影响行数-->" + n + ",SQL-->" + sql_Insert);
            }
        }

        /// <summary>
        /// 保存压缩图片后的结果
        /// </summary>
        /// <param name="instanceID"></param>
        /// <param name="idcard"></param>
        private void SavePictureCompressResult(string instanceID, Result_PictureCompress pic)
        {
            var d = pic.data;
            string sql_Insert = "insert into C_OCR_PictureCompress(InstanceId,createdtime,x,y,factor,rotate,quality,fileUrl,objectid) values('{0}',sysdate,'{1}','{2}','{3}','{4}','{5}','{6}','{7}')";
            sql_Insert = string.Format(sql_Insert, instanceID, d.x, d.y, d.factor, d.rotate, d.quality, d.fileUrl, Guid.NewGuid().ToString());
            int n = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(sql_Insert);
            if (n > 0)
            {
                AppUtility.Engine.LogWriter.Write("保存压缩图片成功,受影响行数-->" + n + ",InstanceID-->" + instanceID);
            }
            else
            {
                AppUtility.Engine.LogWriter.Write("保存压缩图片失败,受影响行数-->" + n + ",SQL-->" + sql_Insert);
            }
        }

        /// <summary>
        /// 保存抵押贷款合同
        /// </summary>
        /// <param name="instanceID"></param>
        /// <param name="vinno"></param>
        private void SaveContractResult(string instanceID, Result_Contract vinno)
        {
            var d = vinno.data;
            string sql_Insert = "insert into C_OCR_Contract(INSTANCEID, CREATEDTIME, HETONGBIANHAO, DAIKUANBENJIN_DAXIE, DANBAOFANGSHI, CHEJIAHAO, GONGJIEREN_XINGMING, GONGJIEREN_ZHENGJIANHAOMA, JIEKUANREN_XINGMING, JIEKUANREN_ZHENGJIANHAOMA, DAIKUANQIXIAN, FUJIAFEIJINE, YUEHUANKUANJINE, JIEKUANRENSHOUFUKUANJINE,DAIKUANLILV, CHEJIAJINE, FILEURL,objectid) values('{0}',sysdate,'{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}')";
            sql_Insert = string.Format(sql_Insert, instanceID, d.hetongbianhao, d.daikuanbenjin_daxie, d.danbaofangshi, d.chejiahao, d.gongjieren_xingming, d.gongjieren_zhengjianhaoma, d.jiekuanren_xingming, d.jiekuanren_zhengjianhaoma, d.daikuanqixian, d.fujiafeijine, d.yuehuankuanjine, d.jiekuanrenshoufukuanjine, d.daikuanlilv, d.chejiajine, d.fileUrl, Guid.NewGuid().ToString());
            int n = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(sql_Insert);
            if (n > 0)
            {
                AppUtility.Engine.LogWriter.Write("保存OCR抵押贷款合同信息成功,受影响行数-->" + n + ",InstanceID-->" + instanceID);
            }
            else
            {
                AppUtility.Engine.LogWriter.Write("保存OCR抵押贷款合同信息失败,受影响行数-->" + n + ",SQL-->" + sql_Insert);
            }
        }

        /// <summary>
        /// 保存扣款授权书
        /// </summary>
        /// <param name="instanceID"></param>
        /// <param name="vinno"></param>
        private void SaveDeductionsAuthorizedResult(string instanceID, Result_DeductionsAuthorized withhold)
        {
            var d = withhold.data;
            string sql_Insert = "INSERT INTO C_OCR_DeductionsAuthorized(INSTANCEID, CREATEDTIME, ID, CUSTOMER, ACCOUNT, IDENTITY, IDENTITYTYPE, FILEURL,objectid) VALUES('{0}',sysdate,'{1}','{2}','{3}','{4}','{5}','{6}','{7}')";
            sql_Insert = string.Format(sql_Insert, instanceID, d.id, d.customer, d.account, d.identity, d.identityType, d.fileUrl, Guid.NewGuid().ToString());
            int n = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(sql_Insert);
            if (n > 0)
            {
                AppUtility.Engine.LogWriter.Write("保存OCR划扣授权书信息成功,受影响行数-->" + n + ",InstanceID-->" + instanceID);
            }
            else
            {
                AppUtility.Engine.LogWriter.Write("保存OCR划扣授权书信息失败,受影响行数-->" + n + ",SQL-->" + sql_Insert);
            }
        }

        /// <summary>
        /// 保存保单
        /// </summary>
        /// <param name="instanceID"></param>
        /// <param name="vinno"></param>
        private void SavePolicyResult(string instanceID, Result_Policy vinno)
        {
            var d = vinno.data;
            string sql_Insert = "INSERT INTO C_OCR_POLICY(INSTANCEID, CREATEDTIME, BEIBAOXIANREN, ZHENGJIANHAOMA, CHEJIAHAO, BAOFEIHEJI, SHIFOUYOUBUJIMIANPEI,SHIFOUYOUSANZEXIAN,DIYISHOUYIREN, FILEURL,objectid) values('{0}',sysdate,'{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}')";
            sql_Insert = string.Format(sql_Insert, instanceID, d.beibaoxianren, d.zhengjianhaoma, d.chejiahao, d.baofeiheji, d.shifouyoubujimianpei, d.shifouyousanzexian, d.diyishouyiren, d.fileUrl, Guid.NewGuid().ToString());
            int n = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(sql_Insert);
            if (n > 0)
            {
                AppUtility.Engine.LogWriter.Write("保存OCR保单信息成功,受影响行数-->" + n + ",InstanceID-->" + instanceID);
            }
            else
            {
                AppUtility.Engine.LogWriter.Write("保存OCR保单信息失败,受影响行数-->" + n + ",SQL-->" + sql_Insert);
            }
        }
        #endregion

        #region 从本地库获取OCR识别后的结果

        /// <summary>
        /// 获取身份证信息
        /// </summary>
        public JsonResult GetIdCardResult(string instanceID)
        {
            List<object> objs = new List<object>();
            string sql = "select * from C_OCR_Idcard where VALIDITY is null and INSTANCEID='" + instanceID + "' order by createdtime";
            DataTable dt = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                objs.Add(new
                {
                    name = row["NAME"].ToString().Trim(),
                    idNumber = row["IDNUMBER"].ToString().Trim(),
                });
            }
            return Json(new { Success = true, Data = objs }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取银行卡信息
        /// </summary>
        public JsonResult GetBankCardResult(string instanceID)
        {
            object obj = new object();
            string sql = "select * from C_OCR_Bankcard where INSTANCEID='" + instanceID + "' order by createdtime desc";
            DataTable dt = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
            if (dt.Rows.Count > 0)
            {
                obj = new
                {
                    cardNumber = dt.Rows[0]["CARDNUMBER"].ToString().Trim()
                };
            }
            return Json(new { Success = true, Data = obj }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取购车发票信息
        /// </summary>
        /// <param name="instanceID"></param>
        /// <returns></returns>
        public JsonResult GetVehicleInvoiceResult(string instanceID)
        {
            object obj = new object();
            string sql = "select * from C_OCR_VehicleInvoice where INSTANCEID='" + instanceID + "' order by createdtime desc";
            DataTable dt = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
            if (dt.Rows.Count > 0)
            {
                obj = new
                {
                    vehicleInvoiceBuyer = dt.Rows[0]["VEHICLEINVOICEBUYER"].ToString().Trim(),
                    vehicleInvoiceBuyerId = dt.Rows[0]["VEHICLEINVOICEBUYERID"].ToString().Trim(),
                    vehicleInvoiceTotalPrice = dt.Rows[0]["VEHICLEINVOICETOTALPRICE"].ToString().Trim(),
                    vehicleInvoiceTotalPriceDigits = dt.Rows[0]["VEHICLEINVOICETOTALPRICEDIGITS"].ToString().Trim(),
                    vehicleInvoiceCarVin = dt.Rows[0]["VEHICLEINVOICECARVIN"].ToString().Trim(),
                    vehicleInvoiceDealer = dt.Rows[0]["VEHICLEINVOICEDEALER"].ToString().Trim(),
                    vehicleInvoiceIssueDate = dt.Rows[0]["VEHICLEINVOICEISSUEDATE"].ToString().Trim(),
                };
            }
            return Json(new { Success = true, Data = obj }, JsonRequestBehavior.AllowGet);

        }


        /// <summary>
        /// 获取抵押贷款合同
        /// </summary>
        /// <param name="instanceID"></param>
        /// <returns></returns>
        public JsonResult GetContractResult(string instanceID)
        {
            object obj = new object();
            string sql = "select * from C_OCR_Contract where INSTANCEID='" + instanceID + "' order by HETONGBIANHAO,createdtime desc";
            DataTable dt = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
            if (dt.Rows.Count > 0)
            {
                obj = new
                {
                    hetongbianhao = dt.Rows[0]["hetongbianhao"].ToString().Trim(),
                    jiekuanren_xingming = dt.Rows[0]["jiekuanren_xingming"].ToString().Trim(),
                    jiekuanren_zhengjianhaoma = dt.Rows[0]["jiekuanren_zhengjianhaoma"].ToString().Trim(),
                    gongjieren_xingming = dt.Rows[0]["gongjieren_xingming"].ToString().Trim(),
                    gongjieren_zhengjianhaoma = dt.Rows[0]["gongjieren_zhengjianhaoma"].ToString().Trim(),
                    chejiajine = dt.Rows[0]["chejiajine"].ToString().Trim(),
                    fujiafeijine = dt.Rows[0]["fujiafeijine"].ToString().Trim(),
                    daikuanbenjin_daxie = dt.Rows[0]["daikuanbenjin_daxie"].ToString().Trim(),
                    jiekuanrenshoufukuanjine = dt.Rows[0]["jiekuanrenshoufukuanjine"].ToString().Trim(),
                    daikuanqixian = dt.Rows[0]["daikuanqixian"].ToString().Trim(),
                    daikuanlilv = dt.Rows[0]["daikuanlilv"].ToString().Trim(),
                    danbaofangshi = dt.Rows[0]["danbaofangshi"].ToString().Trim(),
                    chejiahao = dt.Rows[0]["chejiahao"].ToString().Trim(),
                };
            }
            return Json(new { Success = true, Data = obj }, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// 获取扣款授权书
        /// </summary>
        /// <param name="instanceID"></param>
        /// <returns></returns>
        public JsonResult GetDeductionsAuthorizedResult(string instanceID)
        {
            object obj = new object();
            string sql = "select * from C_OCR_DeductionsAuthorized where INSTANCEID='" + instanceID + "' order by createdtime desc";
            DataTable dt = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
            if (dt.Rows.Count > 0)
            {
                obj = new
                {
                    id = dt.Rows[0]["id"].ToString().Trim(),
                    customer = dt.Rows[0]["customer"].ToString().Trim(),
                    account = dt.Rows[0]["account"].ToString().Trim(),
                    identity = dt.Rows[0]["identity"].ToString().Trim(),
                    identityType = dt.Rows[0]["identityType"].ToString().Trim(),
                };
            }
            return Json(new { Success = true, Data = obj }, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// 获取保单信息
        /// </summary>
        /// <param name="instanceID"></param>
        /// <returns></returns>
        public JsonResult GetPolicyResult(string instanceID)
        {
            object obj = new object();
            string sql = "select * from C_OCR_Policy where INSTANCEID='" + instanceID + "' order by createdtime desc";
            DataTable dt = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
            if (dt.Rows.Count > 0)
            {
                obj = new
                {
                    beibaoxianren = dt.Rows[0]["beibaoxianren"].ToString().Trim(),
                    zhengjianhaoma = dt.Rows[0]["zhengjianhaoma"].ToString().Trim(),
                    chejiahao = dt.Rows[0]["chejiahao"].ToString().Trim(),
                    baofeiheji = dt.Rows[0]["baofeiheji"].ToString().Trim(),
                    shifouyoubujimianpei = dt.Rows[0]["shifouyoubujimianpei"].ToString().Trim(),
                    shifouyousanzexian = dt.Rows[0]["shifouyousanzexian"].ToString().Trim(),
                    diyishouyiren = dt.Rows[0]["diyishouyiren"].ToString().Trim(),
                };
            }
            return Json(new { Success = true, Data = obj }, JsonRequestBehavior.AllowGet);

        }

        #endregion
        #endregion

        #region Class

        /// <summary>
        /// OCR接口相关参数
        /// </summary>
        public class Parameter_OCR
        {
            public string Authorization { get; set; }
            public string platformType { get; set; }
            public string imageTyep { get; set; }
            public string isCutImage { get; set; }
        }
        /// <summary>
        /// 图片压缩相关参数
        /// </summary>
        public class Parameter_Compress
        {
            public string x { get; set; }
            public string y { get; set; }
            public string factor { get; set; }
            public string quality { get; set; }
            public string rotate { get; set; }
        }
        /// <summary>
        /// 流程相关参数
        /// </summary>
        public class Parameter_Instance
        {
            public string InstanceId { get; set; }
            public string SchemaCode { get; set; }
            public string BizObjectId { get; set; }
            public string DataField { get; set; }
        }
        /// <summary>
        /// OCR Login返回结果
        /// </summary>
        public class Result_Token
        {
            public string code { get; set; }
            public string message { get; set; }
            public string data { get; set; }
        }
        /// <summary>
        /// 银行卡识别结果
        /// </summary>
        public class Result_bankcard
        {
            public string code { get; set; }
            public string message { get; set; }
            public data_bankcard data { get; set; }

            public class data_bankcard
            {
                public string type { get; set; }
                public string cardNumber { get; set; }
                public string validate { get; set; }
                public string holderName { get; set; }
                public string issuer { get; set; }
                public string fileUrl { get; set; }
            }
        }
        /// <summary>
        /// 身份证识别结果
        /// </summary>
        public class Result_idcard
        {
            public string code { get; set; }
            public string message { get; set; }
            public data_idcard data { get; set; }

            public class data_idcard
            {
                public string name { get; set; }
                public string idNumber { get; set; }
                public string birthday { get; set; }
                public string sex { get; set; }
                public string people { get; set; }
                public string address { get; set; }
                public string issueAuthority { get; set; }
                public string validity { get; set; }
                public string fileUrl { get; set; }
            }
        }
        /// <summary>
        /// 驾驶证识别结果
        /// </summary>
        public class Result_drivingLicense
        {
            public string code { get; set; }
            public string message { get; set; }
            public data_drivingLicense data { get; set; }

            public class data_drivingLicense
            {
                public string address { get; set; }
                public string birthday { get; set; }
                public string driveType { get; set; }
                public string drivingLicenseMainNationality { get; set; }
                public string drivingLicenseMainNumber { get; set; }
                public string drivingLicenseMainValidFor { get; set; }
                public string drivingLicenseMainValidFrom { get; set; }
                public string issueDate { get; set; }
                public string name { get; set; }
                public string sex { get; set; }
                public string drivingLicenseDocumentNumber { get; set; }
                public string type { get; set; }
                public string fileUrl { get; set; }
            }
        }
        /// <summary>
        /// 车架号识别结果
        /// </summary>
        public class Result_vinNo
        {
            public string code { get; set; }
            public string message { get; set; }
            public data_vinNo data { get; set; }

            public class data_vinNo
            {
                public string vehicleLicenseMainVin { get; set; }
                public string fileUrl { get; set; }
            }
        }
        /// <summary>
        /// 购车发票识别结果
        /// </summary>
        public class Result_vehicleInvoice
        {
            public string code { get; set; }
            public string message { get; set; }
            public data_vehicleInvoice data { get; set; }

            public class data_vehicleInvoice
            {
                public string type { get; set; }
                public string vehicleInvoiceBuyer { get; set; }
                public string vehicleInvoiceBuyerId { get; set; }
                public string vehicleInvoiceCarModel { get; set; }
                public string vehicleInvoiceCarMadePlace { get; set; }
                public string vehicleInvoiceCertId { get; set; }
                public string vehicleInvoiceEngineId { get; set; }
                public string vehicleInvoiceCarVin { get; set; }
                public string vehicleInvoiceTotalPrice { get; set; }
                public string vehicleInvoiceTotalPriceDigits { get; set; }
                public string vehicleInvoiceTelephone { get; set; }
                public string vehicleInvoiceIssueDate { get; set; }
                public string vehicleInvoiceDaima { get; set; }
                public string vehicleInvoiceHaoma { get; set; }
                public string vehicleInvoiceDealer { get; set; }
                public string vehicleInvoiceJidaDaima { get; set; }
                public string vehicleInvoiceJidaHaoma { get; set; }
                public string vehicleInvoiceTaxRate { get; set; }
                public string vehicleInvoiceTaxAmount { get; set; }
                public string vehicleInvoicePriceWithoutTax { get; set; }
                public string fileUrl { get; set; }
            }
        }
        /// <summary>
        /// SheetAttachment控件需要返回的数据格式
        /// </summary>
        protected class HandlerResult
        {
            public string FileId;
            public string AttachmentId;
            /// <summary>
            /// 状态：
            /// 0:默认
            /// 1:成功
            /// 2:超过文件限制大小
            /// 3:不是限制的文件类型
            /// </summary>
            public int State;
            public string ResultStr = string.Empty;
            public string Url = "";
            public object OCR_Result;
            public HandlerResult(string fileId, string attachmentId, string url, object ocr)
            {
                this.FileId = fileId;
                this.AttachmentId = attachmentId;
                this.State = 1;
                this.Url = url;
                this.OCR_Result = ocr;
            }

            public HandlerResult(string fileId, string attachmentId, int state)
            {
                this.FileId = fileId;
                this.AttachmentId = attachmentId;
                this.State = state;
                this.ResultStr = "";
                switch (this.State)
                {
                    case 0:
                    case 1:
                        break;
                    case 2:
                        this.ResultStr = "超过文件限制大小";
                        break;
                    case 3:
                        this.ResultStr = "限制的文件类型";
                        break;
                }
            }
        }

        /// <summary>
        /// 图片切边结果
        /// </summary>
        public class Result_Crop
        {
            public string code { get; set; }

            public string message { get; set; }

            public data_pic_crop data { get; set; }

            public class data_pic_crop
            {
                public string cropEnhanceImageName { get; set; }

                [JsonProperty("cropEnhanceImageUrl")]
                public string fileUrl { get; set; }
            }
        }

        /// <summary>
        /// 图片压缩结果
        /// </summary>
        public class Result_PictureCompress
        {
            public string code { get; set; }
            public string message { get; set; }
            public data_pic_Compress data { get; set; }

            public class data_pic_Compress
            {
                public string x { get; set; }
                public string y { get; set; }
                public string factor { get; set; }
                public string rotate { get; set; }
                public string quality { get; set; }
                public string fileUrl { get; set; }
            }
        }

        /// <summary>
        /// 抵押贷款合同结果
        /// </summary>
        public class Result_Contract
        {
            public string code { get; set; }
            public string message { get; set; }
            public data_contract data { get; set; }

            public class data_contract
            {
                public string hetongbianhao { get; set; }
                public string daikuanbenjin_daxie { get; set; }
                public string danbaofangshi { get; set; }
                public string chejiahao { get; set; }
                public string gongjieren_xingming { get; set; }
                public string gongjieren_zhengjianhaoma { get; set; }
                public string jiekuanren_xingming { get; set; }
                public string jiekuanren_zhengjianhaoma { get; set; }
                public string daikuanqixian { get; set; }
                public string fujiafeijine { get; set; }
                public string yuehuankuanjine { get; set; }
                public string jiekuanrenshoufukuanjine { get; set; }
                public string daikuanlilv { get; set; }
                public string chejiajine { get; set; }
                public string fileUrl { get; set; }

            }
        }


        /// <summary>
        /// 划扣授权书结果
        /// </summary>
        public class Result_DeductionsAuthorized
        {
            public string code { get; set; }
            public string message { get; set; }
            public data_deductionsAuthorized data { get; set; }

            public class data_deductionsAuthorized
            {
                public string id { get; set; }
                public string customer { get; set; }
                public string account { get; set; }
                public string identity { get; set; }
                public string identityType { get; set; }
                public string fileUrl { get; set; }

            }
        }

        /// <summary>
        /// 保单识别结果
        /// </summary>
        public class Result_Policy
        {
            public string code { get; set; }
            public string message { get; set; }
            public data_policy data { get; set; }

            public class data_policy
            {
                public string beibaoxianren { get; set; }
                public string zhengjianhaoma { get; set; }
                public string chejiahao { get; set; }
                public string baofeiheji { get; set; }
                public string shifouyoubujimianpei { get; set; }
                public string shifouyousanzexian { get; set; }
                public string diyishouyiren { get; set; }
                public string fileUrl { get; set; }

            }
        }
        #endregion
    }
}