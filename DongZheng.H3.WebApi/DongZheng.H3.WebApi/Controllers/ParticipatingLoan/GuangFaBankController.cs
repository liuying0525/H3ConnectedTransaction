using DongZheng.H3.WebApi.Common;
using DongZheng.H3.WebApi.Common.ParticipatingLoan;
using FluentFTP;
using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OThinker.H3.Controllers;
using OThinker.H3.Data;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DongZheng.H3.WebApi.Controllers.ParticipatingLoan
{
    public class GuangFaBankController : OThinker.H3.Controllers.ControllerBase
    {
        #region FTP 配置信息

        public static string appFtpIp = System.Configuration.ConfigurationManager.AppSettings["GF_AppFtpIp"] + string.Empty;

        public static string appFtpPort = System.Configuration.ConfigurationManager.AppSettings["GF_AppFtpPort"] + string.Empty;

        public static string appFtpAccount = System.Configuration.ConfigurationManager.AppSettings["GF_AppFtpAccount"] + string.Empty;

        public static string appFtpPassword = System.Configuration.ConfigurationManager.AppSettings["GF_AppFtpPassword"] + string.Empty;


        public static string gfFtpIp = System.Configuration.ConfigurationManager.AppSettings["GF_FtpIp"] + string.Empty;

        public static string gfFtpPort = System.Configuration.ConfigurationManager.AppSettings["GF_FtpPort"] + string.Empty;

        public static string gfFtpAccount = System.Configuration.ConfigurationManager.AppSettings["GF_FtpAccount"] + string.Empty;

        public static string gfFtpPassword = System.Configuration.ConfigurationManager.AppSettings["GF_FtpPassword"] + string.Empty;

        public static string gfFtpRootPath = System.Configuration.ConfigurationManager.AppSettings["GF_FtpRootPath"] + string.Empty;

        public static string gfThreadCount = System.Configuration.ConfigurationManager.AppSettings["GF_ThreadCount"] + string.Empty;

        public static string gfThreadUploadSize = System.Configuration.ConfigurationManager.AppSettings["GF_ThreadUploadSize"] + string.Empty;

        #endregion

        #region 获取app文件接口url

        public static string getAppFileUrl = System.Configuration.ConfigurationManager.AppSettings["GF_AppFileUrl"] + string.Empty;

        #endregion


        public override string FunctionCode
        {
            get
            {
                return "";
            }
        }

        public string Index()
        {
            return "GuangFaBank service";
        }

        [HttpGet]
        public JsonResult UploadNotifyResult()
        {
            GFUploadResultCache resultCache = null;

            var result = Cache.GetCache("GFUploadFailNumber") + string.Empty;

            if (string.IsNullOrEmpty(result))
            {
                resultCache = new GFUploadResultCache { Result = "Clear" };
                return Json(resultCache, JsonRequestBehavior.AllowGet);
            }

            resultCache = JsonConvert.DeserializeObject<GFUploadResultCache>(result);

            return Json(resultCache, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 联贷平台通知上传文件至联贷平台SFTP
        /// </summary>
        [HttpPost]
        public JsonResult UploadFilesNotify(GFUploadNotifyRequest request)
        {
            AppUtility.Engine.LogWriter.Write($"联贷SFTP上传请求已接受{JsonConvert.SerializeObject(request)}");

            var rtn = new rtn_data { code = 1, message = "接收成功" };

            GFUploadResultCache uploadState = null;

            var uploadStateCache = Cache.GetCache("GFUploadFailNumber") + string.Empty;

            if (!string.IsNullOrEmpty(uploadStateCache))
            {
                uploadState = JsonConvert.DeserializeObject<GFUploadResultCache>(uploadStateCache);

                if (uploadState.Equals("Ing"))
                {
                    AppUtility.Engine.LogWriter.Write($"联贷SFTP正在上传中，请耐心等待");
                    rtn.code = -10002;
                    rtn.message = "联贷SFTP正在上传中，请耐心等待";
                    return Json(rtn, JsonRequestBehavior.AllowGet);
                }
            }

            if (!request.ContractNumbers.Any())
            {
                AppUtility.Engine.LogWriter.Write("联贷平台通知上传文件至SFTP无ContractNumbers-->" + JsonConvert.SerializeObject(request));

                rtn.code = -10001;
                rtn.message = "无ContractNumbers";

                return Json(rtn, JsonRequestBehavior.AllowGet);
            }

            var contractAppRelations = GetApplicationNumbersByContractNumbers(request.ContractNumbers);

            if (contractAppRelations == null || !contractAppRelations.Any())
            {
                AppUtility.Engine.LogWriter.Write("联贷平台通知上传文件至SFTP，未查询到相关合同信息-->" + JsonConvert.SerializeObject(request));

                rtn.code = -10001;
                rtn.message = "未查询到相关合同信息";

                return Json(rtn, JsonRequestBehavior.AllowGet);
            }

            var exceptContractNumbers = request.ContractNumbers.Except(contractAppRelations.Select(p => p.ContractNumber)).ToList();

            if (exceptContractNumbers == null || exceptContractNumbers.Any())
            {
                AppUtility.Engine.LogWriter.Write("联贷平台通知上传文件至SFTP ContractNumbers不存在-->" + JsonConvert.SerializeObject(exceptContractNumbers));

                rtn.code = -10001;
                rtn.message = "ContractNumbers不存在" + JsonConvert.SerializeObject(exceptContractNumbers);

                return Json(rtn, JsonRequestBehavior.AllowGet);
            }

            var applicationNumbers = contractAppRelations.Select(p => p.ApplicationNumber).ToList();

            var datatable = GetApplicationByAppNumber(applicationNumbers);

            if (datatable == null)
            {
                AppUtility.Engine.LogWriter.Write("联贷平台通知上传文件至SFTP 为查询到零售申请单-->" + JsonConvert.SerializeObject(applicationNumbers));

                rtn.code = -10001;
                rtn.message = "为查询到零售申请单" + JsonConvert.SerializeObject(applicationNumbers);

                return Json(rtn, JsonRequestBehavior.AllowGet);
            }

            var relations = new List<AppNumberUnionIdRelation>();

            foreach (DataRow row in datatable.Rows)
            {
                var relation = new AppNumberUnionIdRelation();

                if (!string.IsNullOrEmpty(row["AppUnionId"] + string.Empty))
                {
                    relation.UnionId = row["AppUnionId"] + string.Empty;
                }

                if (!string.IsNullOrEmpty(row["application_number"] + string.Empty))
                {
                    relation.ApplicationNumber = row["application_number"] + string.Empty;

                    var contractAppRelation = contractAppRelations.Where(p => p.ApplicationNumber == relation.ApplicationNumber).FirstOrDefault();

                    relation.ContractNumber = contractAppRelation.ContractNumber;
                }

                if (!string.IsNullOrEmpty(row["objectid"] + string.Empty))
                {
                    relation.InstanceId = row["objectid"] + string.Empty;
                }
                relations.Add(relation);
            }

            //检查对应application_number是否都有
            var appNumbers = relations.Select(p => p.ApplicationNumber).ToList();

            var exceptNumbers = applicationNumbers.Except(appNumbers);

            if (exceptNumbers.Any())
            {
                AppUtility.Engine.LogWriter.Write("联贷平台通知上传文件至SFTP AppNumber不存在-->" + JsonConvert.SerializeObject(exceptNumbers));
                rtn.code = -10001;
                rtn.message = "ApplicationNumbers" + JsonConvert.SerializeObject(exceptNumbers) + "不存在";
                return Json(rtn, JsonRequestBehavior.AllowGet);
            }

            #region 过时

            //检查每个application_number是否都有unionId
            //var checkUnionIds = relations.Where(p => string.IsNullOrEmpty(p.UnionId));

            //if (checkUnionIds.Any())
            //{
            //    AppUtility.Engine.LogWriter.Write("联贷平台通知上传文件至SFTP App无关联Id-->" + JsonConvert.SerializeObject(checkUnionIds));
            //    rtn.code = -10001;
            //    rtn.message = "无关联Id" + JsonConvert.SerializeObject(checkUnionIds.Select(p => p.ApplicationNumber)) + "不存在";
            //    return Json(rtn, JsonRequestBehavior.AllowGet);
            //}

            //var appUnionIds = relations.Select(p => p.UnionId).ToList();

            //var paras = new { orderIds = appUnionIds };

            //AppUtility.Engine.LogWriter.Write("联贷平台通知上传文件至SFTP：查询文件地址参数-->" + JsonConvert.SerializeObject(paras));

            //var result = HttpHelper.PostWebRequest(getAppFileUrl, "application/json", JsonConvert.SerializeObject(paras));

            //AppUtility.Engine.LogWriter.Write("联贷平台通知上传文件至SFTP：查询文件地址返回-->" + result);

            //if (string.IsNullOrEmpty(result))
            //{
            //    rtn.code = -10000;
            //    return Json(rtn, JsonRequestBehavior.AllowGet);
            //}

            //var jObject = JObject.Parse(result);

            //if (jObject["code"].ToString() != "10001")
            //{
            //    AppUtility.Engine.LogWriter.Write("联贷平台通知上传文件至SFTP：查询文件地址错误-->" + result);
            //    rtn.code = -10000;
            //    return Json(rtn, JsonRequestBehavior.AllowGet);
            //}

            //Task.Run(() =>
            //{
            //    UploadToGFSftp(jObject["data"].ToString(), relations);
            //});

            #endregion

            Task.Run(() =>
            {
                UploadToGFSftp(relations, 0);
            });

            //Func<List<string>> func = ()=> UploadToGFSftp(jObject["data"].ToString(), relations);

            //Task.Run(func)
            //.ContinueWith(_ =>
            //{
            //    var failIds = _.Result;
            //    if (failIds.Any())
            //    {
            //        HttpHelper.PostWebRequest(getAppFileUrl, "application/json", JsonConvert.SerializeObject(_));
            //    }
            //});

            return Json(rtn, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 上传至SFTP
        /// </summary>
        [Obsolete]
        public List<string> UploadToGFSftp(string data, List<AppNumberUnionIdRelation> relations)
        {
            var resultCache = new GFUploadResultCache { Result = "Ing" };

            Cache.SetCache("GFUploadFailNumber", JsonConvert.SerializeObject(resultCache), TimeSpan.FromHours(3));

            Stopwatch watch = new Stopwatch();
            watch.Start();

            var pictures = JsonConvert.DeserializeObject<List<GFPictureModel>>(data);

            var datetime = DateTime.Now.ToString("yyyyMMdd");

            var failContractNumbers = new List<string>();

            //自旋锁
            SpinLock _spinLock = new SpinLock();

            TaskFactory factory = new TaskFactory();

            List<Task> tasks = new List<Task>();

            var taskCount = Convert.ToInt32(gfThreadCount);
            var pageSize = Convert.ToInt32(gfThreadUploadSize);
            if (pictures.Count() > 1)
            {
                pageSize = pictures.Count() % taskCount == 0 ? pictures.Count() / taskCount : (pictures.Count() / taskCount) + 1;
            }
            for (var i = 0; i < taskCount; i++)
            {
                var pagePictures = pictures.Skip(i * pageSize).Take(pageSize).ToList();

                if (!pagePictures.Any())
                {
                    continue;
                }

                var task = factory.StartNew(() =>
                {
                    return TaskUpload(pagePictures, datetime, relations);
                }).ContinueWith(_ =>
                {
                    var failIds = _.Result;
                    if (failIds.Any())
                    {
                        bool _lock = false;
                        try
                        {
                            //放在多线程问题
                            _spinLock.Enter(ref _lock);
                            failContractNumbers.AddRange(failIds);
                        }
                        finally
                        {
                            if (_lock)
                                _spinLock.Exit();
                        }
                    }
                });

                tasks.Add(task);

            }

            factory.ContinueWhenAll(tasks.ToArray(), (continueTasks) =>
            {
                if (failContractNumbers.Any())
                {
                    resultCache.Result = "Fail";
                    resultCache.FailContractNumbers = failContractNumbers;
                    AppUtility.Engine.LogWriter.Write($"联贷SFTP上传文件失败：{JsonConvert.SerializeObject(failContractNumbers)}");
                }
                else
                {
                    resultCache.Result = "Success";
                }

                Cache.SetCache("GFUploadFailNumber", JsonConvert.SerializeObject(resultCache), TimeSpan.FromHours(3));

                watch.Stop();
                AppUtility.Engine.LogWriter.Write($"联贷SFTP上传文件用时：{watch.ElapsedMilliseconds}ms");
            });

            return failContractNumbers;
        }

        [Obsolete]
        public List<string> TaskUpload(List<GFPictureModel> pictures, string datetime, List<AppNumberUnionIdRelation> relations)
        {
            AppUtility.Engine.LogWriter.Write($"开始上传文件：{JsonConvert.SerializeObject(pictures)}");

            var failContractNumbers = new List<string>();

            foreach (var pictureModel in pictures)
            {
                var relation = relations.Where(p => p.UnionId == pictureModel.orderId).FirstOrDefault();

                var instanceContext = AppUtility.Engine.InstanceManager.GetInstanceContext(relation.InstanceId);

                var filePath = string.Format("{0}/{1}", gfFtpRootPath, datetime);

                var persontype = "mainborrower";

                var dicStream = new Dictionary<string, Stream>();

                if (!string.IsNullOrEmpty(pictureModel.idCardFace))
                {
                    AppUtility.Engine.LogWriter.Write($"联贷-->申请号：{relation.ApplicationNumber}，合同号：{relation.ContractNumber}身份证正面文件获取开始");

                    var idCardFaceFileBytes = ReadFileFromFtp(pictureModel.idCardFace);

                    if (idCardFaceFileBytes == null)
                    {
                        AppUtility.Engine.LogWriter.Write($"联贷-->申请号：{relation.ApplicationNumber}，合同号：{relation.ContractNumber}身份证正面文件获取失败");
                        continue;
                    }

                    var fileType = pictureModel.idCardFace.Substring(pictureModel.idCardFace.IndexOf(".") + 1);

                    var fileName = string.Format("{0}_{1}_cert01.{2}", relation.ContractNumber, persontype, fileType);

                    //保存附件
                    SaveAttachment(instanceContext.BizObjectSchemaCode, instanceContext.BizObjectId, "SFZ", fileName, fileType, idCardFaceFileBytes);

                    dicStream.Add(fileName, new MemoryStream(idCardFaceFileBytes));

                    AppUtility.Engine.LogWriter.Write($"联贷-->申请号：{relation.ApplicationNumber}，合同号：{relation.ContractNumber}身份证正面文件获取结束");

                }
                if (!string.IsNullOrEmpty(pictureModel.idCardBack))
                {
                    AppUtility.Engine.LogWriter.Write($"联贷-->申请号：{relation.ApplicationNumber}，合同号：{relation.ContractNumber}身份证反面文件获取开始");

                    var idCardBackFileBytes = ReadFileFromFtp(pictureModel.idCardBack);

                    if (idCardBackFileBytes == null)
                    {
                        AppUtility.Engine.LogWriter.Write($"联贷-->申请号：{relation.ApplicationNumber}，合同号：{relation.ContractNumber}身份证反面文件获取失败");
                        continue;
                    }

                    var fileType = pictureModel.idCardBack.Substring(pictureModel.idCardBack.IndexOf(".") + 1);

                    var fileName = string.Format("{0}_{1}_cert02.{2}", relation.ContractNumber, persontype, fileType);

                    //保存附件
                    SaveAttachment(instanceContext.BizObjectSchemaCode, instanceContext.BizObjectId, "SFZ", fileName, fileType, idCardBackFileBytes);

                    dicStream.Add(fileName, new MemoryStream(idCardBackFileBytes));

                    AppUtility.Engine.LogWriter.Write($"联贷-->申请号：{relation.ApplicationNumber}，合同号：{relation.ContractNumber}身份证反面文件获取结束");

                }

                //从h3附件中取身份证图片
                if (string.IsNullOrEmpty(pictureModel.idCardFace) && string.IsNullOrEmpty(pictureModel.idCardBack))
                {
                    var attachmentPropertys = GetAttachmentProperty(instanceContext.BizObjectId, "SFZ");

                    for (int i = 0; i < attachmentPropertys.Count; i++)
                    {
                        AppUtility.Engine.LogWriter.Write($"联贷-->申请号：{relation.ApplicationNumber}，合同号：{relation.ContractNumber}身份证{i}文件获取开始h3");

                        var attachment = AppUtility.Engine.BizObjectManager.GetAttachment(string.Empty, attachmentPropertys[i]["bizobjectschemacode"] + string.Empty, attachmentPropertys[i]["bizobjectid"] + string.Empty, attachmentPropertys[i]["objectid"] + string.Empty);

                        if (attachment == null || attachment.Content.Length <= 0)
                        {
                            AppUtility.Engine.LogWriter.Write($"联贷-->申请号：{relation.ApplicationNumber}，合同号：{relation.ContractNumber}无身份证{i}-->{JsonConvert.SerializeObject(attachmentPropertys[i])}");
                        }
                        else
                        {
                            var zxfilename = attachmentPropertys[0]["bizobjectid"] + string.Empty;

                            var fileType = zxfilename.Substring(zxfilename.IndexOf(".") + 1);

                            var fileName = string.Format("{0}_{1}_cert{2}.{3}", relation.ContractNumber, persontype, (i + 1).ToString("D2"), fileType);

                            dicStream.Add(fileName, new MemoryStream(attachment.Content));

                            AppUtility.Engine.LogWriter.Write($"联贷-->申请号：{relation.ApplicationNumber}，合同号：{relation.ContractNumber}身份证{i}文件获取结束");

                        }
                    }

                }


                //有从ftp取
                if (!string.IsNullOrEmpty(pictureModel.proxyBook))
                {
                    AppUtility.Engine.LogWriter.Write($"联贷-->申请号：{relation.ApplicationNumber}，合同号：{relation.ContractNumber}征信查询授权书文件获取开始");

                    var proxyBookFileBytes = ReadFileFromFtp(pictureModel.proxyBook);

                    if (proxyBookFileBytes == null)
                    {
                        AppUtility.Engine.LogWriter.Write($"联贷-->申请号：{relation.ApplicationNumber}，合同号：{relation.ContractNumber}征信查询授权书文件获取失败");
                        continue;
                    }

                    var fileType = pictureModel.proxyBook.Substring(pictureModel.proxyBook.IndexOf(".") + 1);

                    var fileName = string.Format("{0}_{1}_zxsq.{2}", relation.ContractNumber, persontype, fileType);

                    //保存附件
                    SaveAttachment(instanceContext.BizObjectSchemaCode, instanceContext.BizObjectId, "ZX", fileName, fileType, proxyBookFileBytes);

                    dicStream.Add(fileName, new MemoryStream(proxyBookFileBytes));

                    AppUtility.Engine.LogWriter.Write($"联贷-->申请号：{relation.ApplicationNumber}，合同号：{relation.ContractNumber}征信查询授权书文件获取结束");

                }
                else
                {
                    AppUtility.Engine.LogWriter.Write($"联贷-->申请号：{relation.ApplicationNumber}，合同号：{relation.ContractNumber}征信查询授权书文件获取开始h3");
                    //无从h3取
                    var attachmentPropertys = GetAttachmentProperty(instanceContext.BizObjectId, "ZX");

                    var attachment = AppUtility.Engine.BizObjectManager.GetAttachment(string.Empty, attachmentPropertys[0]["bizobjectschemacode"] + string.Empty, attachmentPropertys[0]["bizobjectid"] + string.Empty, attachmentPropertys[0]["objectid"] + string.Empty);

                    if (attachment == null || attachment.Content.Length <= 0)
                    {
                        AppUtility.Engine.LogWriter.Write($"联贷-->申请号：{relation.ApplicationNumber}，合同号：{relation.ContractNumber}无征信查询授权书");
                    }
                    else
                    {
                        var zxfilename = attachmentPropertys[0]["bizobjectid"] + string.Empty;

                        var fileType = zxfilename.Substring(zxfilename.IndexOf(".") + 1);

                        var fileName = string.Format("{0}_{1}_zxsq.{2}", relation.ContractNumber, persontype, fileType);

                        dicStream.Add(fileName, new MemoryStream(attachment.Content));

                        AppUtility.Engine.LogWriter.Write($"联贷-->申请号：{relation.ApplicationNumber}，合同号：{relation.ContractNumber}征信查询授权书文件获取结束");
                    }
                }

                if (!string.IsNullOrEmpty(pictureModel.facePicture))
                {
                    AppUtility.Engine.LogWriter.Write($"联贷-->申请号：{relation.ApplicationNumber}，合同号：{relation.ContractNumber}活体识别影像件文件获取开始");

                    var facePictureFileBytes = ReadFileFromFtp(pictureModel.facePicture);

                    if (facePictureFileBytes == null)
                    {
                        AppUtility.Engine.LogWriter.Write($"联贷-->申请号：{relation.ApplicationNumber}，合同号：{relation.ContractNumber}活体识别影像件文件获取失败");
                        continue;
                    }
                    var fileType = pictureModel.facePicture.Substring(pictureModel.facePicture.IndexOf(".") + 1);

                    var fileName = string.Format("{0}_{1}_htsb.{2}", relation.ContractNumber, persontype, fileType);

                    //保存附件
                    SaveAttachment(instanceContext.BizObjectSchemaCode, instanceContext.BizObjectId, "Living_Face", fileName, fileType, facePictureFileBytes);

                    dicStream.Add(fileName, new MemoryStream(facePictureFileBytes));

                    AppUtility.Engine.LogWriter.Write($"联贷-->申请号：{relation.ApplicationNumber}，合同号：{relation.ContractNumber}活体识别影像件文件获取结束");

                }

                if (dicStream.Count <= 0)
                {
                    AppUtility.Engine.LogWriter.Write($"联贷-->申请号：{relation.ApplicationNumber}，合同号：{relation.ContractNumber}未找到上传的文件");
                    failContractNumbers.Add(relation.ContractNumber);
                    continue;
                }

                #region 压缩文件，上传SFTP
                try
                {
                    var ms = PackageManyZip(dicStream);

                    AppUtility.Engine.LogWriter.Write($"联贷-->申请号：{relation.ApplicationNumber}，合同号：{relation.ContractNumber}ms.length={ms.Length}");

                    var uploadSuccess = SaveToGFSftp(filePath, ms, string.Format("{0}.zip", relation.ContractNumber));
                    if (!uploadSuccess)
                    {
                        AppUtility.Engine.LogWriter.Write($"联贷-->申请号：{relation.ApplicationNumber}，合同号：{relation.ContractNumber}影像件文件上传失败");
                        failContractNumbers.Add(relation.ContractNumber);
                    }
                }
                catch (Exception ex)
                {
                    AppUtility.Engine.LogWriter.Write($"申请号：{relation.ApplicationNumber}，合同号：{relation.ContractNumber}压缩上传SFTP失败{ex.ToString()}");
                    failContractNumbers.Add(relation.ContractNumber);
                }

                #endregion
            }

            if (failContractNumbers.Any())
            {
                AppUtility.Engine.LogWriter.Write($"联贷SFTP上传文件失败：{JsonConvert.SerializeObject(failContractNumbers)}");
            }
            return failContractNumbers;
        }


        /// <summary>
        /// 上传至SFTP
        /// </summary>
        public List<string> UploadToGFSftp(List<AppNumberUnionIdRelation> relations, int retryCount)
        {
            AppUtility.Engine.LogWriter.Write($"联贷SFTP上传文件第{retryCount}次：{JsonConvert.SerializeObject(relations)}");

            var resultCache = new GFUploadResultCache { Result = "Ing" };

            Cache.SetCache("GFUploadFailNumber", JsonConvert.SerializeObject(resultCache), TimeSpan.FromHours(3));

            Stopwatch watch = new Stopwatch();
            watch.Start();

            var datetime = DateTime.Now.ToString("yyyyMMdd");

            var failContractNumbers = new List<string>();

            //自旋锁
            SpinLock _spinLock = new SpinLock();

            TaskFactory factory = new TaskFactory();

            List<Task> tasks = new List<Task>();

            var taskCount = Convert.ToInt32(gfThreadCount);
            var pageSize = Convert.ToInt32(gfThreadUploadSize);
            if (relations.Count() > 1)
            {
                pageSize = relations.Count() % taskCount == 0 ? relations.Count() / taskCount : (relations.Count() / taskCount) + 1;
            }
            for (var i = 0; i < taskCount; i++)
            {
                var pageRelations = relations.Skip(i * pageSize).Take(pageSize).ToList();

                if (!pageRelations.Any())
                {
                    continue;
                }

                var task = factory.StartNew(() =>
                {
                    return TaskUpload(pageRelations, datetime);
                }).ContinueWith(_ =>
                {
                    var failIds = _.Result;
                    if (failIds.Any())
                    {
                        bool _lock = false;
                        try
                        {
                            //防止多线程问题
                            _spinLock.Enter(ref _lock);
                            failContractNumbers.AddRange(failIds);
                        }
                        finally
                        {
                            if (_lock)
                                _spinLock.Exit();
                        }
                    }
                });

                tasks.Add(task);

            }

            factory.ContinueWhenAll(tasks.ToArray(), (continueTasks) =>
            {
                if (failContractNumbers.Any())
                {
                    resultCache.Result = "Fail";
                    resultCache.FailContractNumbers = failContractNumbers;
                    AppUtility.Engine.LogWriter.Write($"联贷SFTP上传文件失败：{JsonConvert.SerializeObject(failContractNumbers)}");
                    retryCount++;
                    if (retryCount < 4)
                    {
                        var retryRelations = relations.Where(p => failContractNumbers.Contains(p.ContractNumber)).ToList();
                        
                        Task.Run(() => 
                        {
                            UploadToGFSftp(retryRelations, retryCount);
                        });
                    }
                    else
                        Cache.SetCache("GFUploadFailNumber", JsonConvert.SerializeObject(resultCache), TimeSpan.FromHours(3));
                }
                else
                {
                    resultCache.Result = "Success";
                    Cache.SetCache("GFUploadFailNumber", JsonConvert.SerializeObject(resultCache), TimeSpan.FromHours(3));
                }
                watch.Stop();
                AppUtility.Engine.LogWriter.Write($"联贷SFTP上传文件第{retryCount}次用时：{watch.ElapsedMilliseconds}ms");
            });

            return failContractNumbers;
        }

        public List<string> TaskUpload(List<AppNumberUnionIdRelation> relations, string datetime)
        {
            AppUtility.Engine.LogWriter.Write($"联贷SFTP开始上传文件：{JsonConvert.SerializeObject(relations)}");

            var failContractNumbers = new List<string>();

            foreach (var relation in relations)
            {
                var instanceContext = AppUtility.Engine.InstanceManager.GetInstanceContext(relation.InstanceId);

                var filePath = string.Format("{0}/{1}", gfFtpRootPath, datetime);

                var dicStream = new Dictionary<string, Stream>();

                //从h3附件中取身份证图片
                {
                    var attachmentPropertys = GetAttachmentProperty(instanceContext.BizObjectId, "SFZ");

                    var persontype = "mainborrower";

                    for (int i = 0; i < attachmentPropertys.Count; i++)
                    {
                        AppUtility.Engine.LogWriter.Write($"联贷-->申请号：{relation.ApplicationNumber}，合同号：{relation.ContractNumber}身份证{i}文件获取开始h3");

                        var attachment = AppUtility.Engine.BizObjectManager.GetAttachment(string.Empty, attachmentPropertys[i]["bizobjectschemacode"] + string.Empty, attachmentPropertys[i]["bizobjectid"] + string.Empty, attachmentPropertys[i]["objectid"] + string.Empty);

                        if (attachment == null || attachment.Content.Length <= 0)
                        {
                            AppUtility.Engine.LogWriter.Write($"联贷-->申请号：{relation.ApplicationNumber}，合同号：{relation.ContractNumber}无身份证{i}-->{JsonConvert.SerializeObject(attachmentPropertys[i])}");
                        }
                        else
                        {
                            var zxfilename = attachmentPropertys[i]["filename"] + string.Empty;

                            var fileType = zxfilename.Substring(zxfilename.IndexOf(".") + 1);

                            var fileName = string.Format("{0}_{1}_cert{2}.{3}", relation.ContractNumber, persontype, (i + 1).ToString("D2"), fileType);

                            dicStream.Add(fileName, new MemoryStream(attachment.Content));

                            AppUtility.Engine.LogWriter.Write($"联贷-->申请号：{relation.ApplicationNumber}，合同号：{relation.ContractNumber}身份证{i}文件获取结束");

                        }
                    }
                }

                {
                    var persontype = "mainborrower";

                    AppUtility.Engine.LogWriter.Write($"联贷-->申请号：{relation.ApplicationNumber}，合同号：{relation.ContractNumber}征信查询授权书文件获取开始h3");
                    //无从h3取
                    var attachmentPropertys = GetAttachmentProperty(instanceContext.BizObjectId, "ZX");
                    for (int i = 0; i < attachmentPropertys.Count; i++)
                    {
                        var attachment = AppUtility.Engine.BizObjectManager.GetAttachment(string.Empty, attachmentPropertys[i]["bizobjectschemacode"] + string.Empty, attachmentPropertys[i]["bizobjectid"] + string.Empty, attachmentPropertys[i]["objectid"] + string.Empty);

                        if (attachment == null || attachment.Content.Length <= 0)
                        {
                            AppUtility.Engine.LogWriter.Write($"联贷-->申请号：{relation.ApplicationNumber}，合同号：{relation.ContractNumber}无征信查询授权书");
                        }
                        else
                        {
                            var zxfilename = attachmentPropertys[i]["filename"] + string.Empty;

                            var fileType = zxfilename.Substring(zxfilename.IndexOf(".") + 1);

                            var fileName = string.Format("{0}_{1}_zxsq{2}.{3}", relation.ContractNumber, persontype, (i + 1).ToString("D2"), fileType);

                            dicStream.Add(fileName, new MemoryStream(attachment.Content));

                            AppUtility.Engine.LogWriter.Write($"联贷-->申请号：{relation.ApplicationNumber}，合同号：{relation.ContractNumber}征信查询授权书文件获取结束");
                        }
                    }
                }
                #region 暂无活体图片，后面需要从h3取

                //if (!string.IsNullOrEmpty(pictureModel.facePicture))
                //{
                //    AppUtility.Engine.LogWriter.Write($"联贷-->申请号：{relation.ApplicationNumber}，合同号：{relation.ContractNumber}活体识别影像件文件获取开始");

                //    var facePictureFileBytes = ReadFileFromFtp(pictureModel.facePicture);

                //    if (facePictureFileBytes == null)
                //    {
                //        AppUtility.Engine.LogWriter.Write($"联贷-->申请号：{relation.ApplicationNumber}，合同号：{relation.ContractNumber}活体识别影像件文件获取失败");
                //        continue;
                //    }
                //    var fileType = pictureModel.facePicture.Substring(pictureModel.facePicture.IndexOf(".") + 1);

                //    var fileName = string.Format("{0}_{1}_htsb.{2}", relation.ContractNumber, persontype, fileType);

                //    //保存附件
                //    SaveAttachment(instanceContext.BizObjectSchemaCode, instanceContext.BizObjectId, "Living_Face", fileName, fileType, facePictureFileBytes);

                //    dicStream.Add(fileName, new MemoryStream(facePictureFileBytes));

                //    AppUtility.Engine.LogWriter.Write($"联贷-->申请号：{relation.ApplicationNumber}，合同号：{relation.ContractNumber}活体识别影像件文件获取结束");

                //}

                #endregion

                if (dicStream.Count <= 0)
                {
                    AppUtility.Engine.LogWriter.Write($"联贷-->申请号：{relation.ApplicationNumber}，合同号：{relation.ContractNumber}未找到上传的文件");
                    failContractNumbers.Add(relation.ContractNumber);
                    continue;
                }

                #region 压缩文件，上传SFTP
                try
                {
                    var ms = PackageManyZip(dicStream);

                    AppUtility.Engine.LogWriter.Write($"联贷-->申请号：{relation.ApplicationNumber}，合同号：{relation.ContractNumber}ms.length={ms.Length}");

                    var uploadSuccess = SaveToGFSftp(filePath, ms, string.Format("{0}.zip", relation.ContractNumber));
                    if (!uploadSuccess)
                    {
                        AppUtility.Engine.LogWriter.Write($"联贷-->申请号：{relation.ApplicationNumber}，合同号：{relation.ContractNumber}影像件文件上传失败");
                        failContractNumbers.Add(relation.ContractNumber);
                    }
                }
                catch (Exception ex)
                {
                    AppUtility.Engine.LogWriter.Write($"申请号：{relation.ApplicationNumber}，合同号：{relation.ContractNumber}压缩上传SFTP失败{ex.ToString()}");
                    failContractNumbers.Add(relation.ContractNumber);
                }

                #endregion
            }

            if (failContractNumbers.Any())
            {
                AppUtility.Engine.LogWriter.Write($"联贷SFTP上传文件失败：{JsonConvert.SerializeObject(failContractNumbers)}");
            }
            return failContractNumbers;
        }


        #region Common Method

        /// <summary>
        /// 从App FTP下载文件
        /// </summary>
        /// <param name="ftpServer">h3,app</param>
        /// <param name="remoteFilePath">路径</param>
        /// <returns></returns>
        public byte[] ReadFileFromFtp(string remoteFilePath)
        {
            try
            {
                using (var client = new FtpClient(appFtpIp, int.Parse(appFtpPort), appFtpAccount, appFtpPassword))
                {
                    client.Connect(); //连接

                    var stream = client.OpenRead(remoteFilePath);

                    byte[] bytes = new byte[stream.Length];

                    stream.Read(bytes, 0, bytes.Length);

                    client.Disconnect();

                    client.Dispose();

                    return bytes;
                }
            }
            catch (Exception ex)
            {
                AppUtility.Engine.LogWriter.Write($"联贷-->ftp：{remoteFilePath},读取文件失败：{ex.ToString()}");
            }

            return null;
        }



        /// <summary>
        /// 保存到联贷平台SFTP
        /// </summary>
        public bool SaveToGFSftp(string remotePath, Stream fileStream, string fileName)
        {
            var isSuccess = false;

            try
            {
                using (var client = new SftpClient(gfFtpIp, int.Parse(gfFtpPort), gfFtpAccount, gfFtpPassword)) //创建连接对象
                {
                    client.Connect(); //连接

                    if (!client.Exists(remotePath))
                    {
                        var paths = remotePath.Split('/');

                        if (paths.Length > 1)
                        {
                            var path = "";
                            foreach (var p in paths)
                            {
                                path += "/" + p;
                                if (!client.Exists(path))
                                {
                                    client.CreateDirectory(path);
                                }
                            }
                        }
                    }

                    client.ChangeDirectory(remotePath); //切换目录

                    client.UploadFile(fileStream, fileName); //上传文件

                    var stpFile = client.Get(fileName);
                    if (stpFile.IsRegularFile)
                    {
                        isSuccess = true;
                    }

                    client.Disconnect();
                }
            }
            catch (Exception ex)
            {
                AppUtility.Engine.LogWriter.Write($"联贷平台SFTP上传{remotePath}/{fileName}文件错误{ex.ToString()}");
            }
            return isSuccess;
        }


        /// <summary>
        /// 保存附件 图片、pdf
        /// </summary>
        /// <param name="schemaCode">流程Id</param>
        /// <param name="bizObjectId"></param>
        /// <param name="dataField">字段名称</param>
        /// <param name="contentType">内容类型</param>
        /// <param name="content"></param>
        public void SaveAttachment(string schemaCode, string bizObjectId, string dataField, string fileName, string fileType, byte[] content)
        {

            Attachment attachment = new Attachment();
            attachment.BizObjectSchemaCode = schemaCode;
            attachment.BizObjectId = bizObjectId;
            attachment.DataField = dataField;
            attachment.Content = content;
            attachment.CreatedBy = "";
            attachment.CreatedTime = DateTime.Now;
            attachment.ContentLength = content.Length;
            attachment.LastVersion = true;
            attachment.FileName = fileName;
            attachment.FileFlag = 0;

            switch (fileType)
            {
                case "jpg":
                    attachment.ContentType = "image/jpeg";
                    break;
                case "png":
                    attachment.ContentType = "image/png";
                    break;
                case "pdf":
                    attachment.ContentType = "application/pdf";
                    break;
                case "gif":
                    attachment.ContentType = "image/gif";
                    break;
                case "bmp":
                    attachment.ContentType = "image/bmp";
                    break;
                case "tif":
                    attachment.ContentType = "image/tiff";
                    break;
                case "tiff":
                    attachment.ContentType = "image/tiff";
                    break;
                default:
                    attachment.ContentType = "image/jpeg";
                    break;
            }

            AppUtility.Engine.BizObjectManager.AddAttachment(attachment);
        }

        /// <summary>
        /// 压缩多个文件
        /// </summary>
        /// <param name="streams"></param>
        /// <returns></returns>
        public Stream PackageManyZip(Dictionary<string, Stream> streams)
        {
            MemoryStream returnStream = new MemoryStream();
            var zipMs = new MemoryStream();
            Crc32 crc = new Crc32();
            using (ZipOutputStream zipStream = new ZipOutputStream(zipMs))
            {
                zipStream.SetLevel(9);
                foreach (var kv in streams)
                {
                    string fileName = kv.Key;
                    using (var streamInput = kv.Value)
                    {

                        byte[] buffer = new byte[streamInput.Length];
                        streamInput.Read(buffer, 0, buffer.Length);

                        ZipEntry entry = new ZipEntry(fileName);

                        entry.DateTime = DateTime.Now;
                        entry.Size = streamInput.Length;
                        streamInput.Close();
                        //crc.Reset();
                        //crc.Update(buffer);

                        //entry.Crc = crc.Value;
                        zipStream.PutNextEntry(entry);
                        zipStream.Write(buffer, 0, buffer.Length);

                        zipStream.Flush();
                    }
                }
                zipStream.Finish();
                zipMs.Position = 0;
                zipMs.CopyTo(returnStream);
            }
            returnStream.Position = 0;
            return returnStream;
        }

        #endregion


        #region SQL Method

        public DataTable GetApplicationByAppNumber(List<string> appNumbers)
        {
            if (!appNumbers.Any())
            {
                return null;
            }

            var appnumberStr = string.Format("'{0}'", string.Join("','", appNumbers));

            var sql = @"select ot.objectid ,ap.AppUnionId,ap.application_number from h3.ot_instancecontext ot join h3.i_application ap on ot.bizobjectid = ap.objectid where ap.application_number in({0}) ";

            sql = string.Format(sql, appnumberStr);
            DataTable dt = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);

            if (CommonFunction.hasData(dt))
            {
                return dt;
            }

            return null;
        }

        public List<GFAppNumberContractRelation> GetApplicationNumbersByContractNumbers(List<string> contractNumbers)
        {
            if (!contractNumbers.Any())
            {
                return null;
            }

            var contractNumberStr = string.Format("'{0}'", string.Join("','", contractNumbers));

            var sql = @"select * from v_app_contract_nbr where external_contract_nbr in({0}) ";

            sql = string.Format(sql, contractNumberStr);
            DataTable dt = CommonFunction.ExecuteDataTableSql("CAPDB", sql);

            if (CommonFunction.hasData(dt))
            {
                List<GFAppNumberContractRelation> appNumbers = new List<GFAppNumberContractRelation>();

                foreach (DataRow row in dt.Rows)
                {
                    var relation = new GFAppNumberContractRelation();
                    relation.ContractNumber = row["external_contract_nbr"] + string.Empty;
                    relation.ApplicationNumber = row["APPLICATION_NUMBER"] + string.Empty;
                    appNumbers.Add(relation);
                }
                return appNumbers;
            }

            return null;
        }

        /// <summary>
        /// 获取附件属性
        /// </summary>
        /// <param name="bizobjectId"></param>
        /// <param name="datafiled"></param>
        /// <returns></returns>
        public List<Dictionary<string, string>> GetAttachmentProperty(string bizobjectId, string datafiled)
        {
            var attachmentPropertys = new List<Dictionary<string, string>>();

            string sql = " select objectid,bizobjectschemacode,bizobjectid,filename from Ot_Attachment where datafield ='" + datafiled + "' and bizobjectid ='" + bizobjectId + "'";


            DataTable dt = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);

            if (CommonFunction.hasData(dt))
            {
                foreach (DataRow row in dt.Rows)
                {
                    var attachmentProperty = new Dictionary<string, string>();
                    attachmentProperty.Add("objectid", row[0].ToString());
                    attachmentProperty.Add("bizobjectschemacode", row[1].ToString());
                    attachmentProperty.Add("bizobjectid", row[2].ToString());
                    attachmentProperty.Add("filename", row[3].ToString());
                    attachmentPropertys.Add(attachmentProperty);
                }
            }
            return attachmentPropertys;
        }

        #endregion
    }
}