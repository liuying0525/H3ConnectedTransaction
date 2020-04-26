using DongZheng.H3.WebApi.Models.OCR;
using Newtonsoft.Json;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DongZheng.H3.WebApi.Common.Util
{
    public class OcrHelper
    {
        /// <summary>
        /// 扣款授权书识别方法
        /// </summary>
        /// <param name="buffer">需要识别的扣款授权书图片二进制内容</param>
        /// <returns>扣款授权书识别结果</returns>
        public static async Task<AuthorizationResult> AuthorizationRecognizeAsync(byte[] buffer)
        {
            string url = ConfigurationManager.AppSettings["AuthorizationRecognizeUrl"];

            return await GetResultAsync<AuthorizationResult>(buffer, url);
        }

        /// <summary>
        /// 抵押贷款合同识别方法
        /// </summary>
        /// <param name="buffer">需要识别的抵押贷款合同图片二进制内容</param>
        /// <returns>抵押贷款合同识别结果</returns>
        public static async Task<ContractResult> ContractRecognizeAsync(byte[] buffer)
        {
            string url = ConfigurationManager.AppSettings["ContractRecognizeUrl"];

            return await GetResultAsync<ContractResult>(buffer, url);
        }

        /// <summary>
        /// 保单识别方法
        /// </summary>
        /// <param name="buffer">需要识别的保单图片二进制内容</param>
        /// <returns>保单识别结果</returns>
        public static async Task<WarrantyResult> WarrantyRecognizeAsync(byte[] buffer)
        {
            string url = ConfigurationManager.AppSettings["WarrantyRecognizeUrl"];

            return await GetResultAsync<WarrantyResult>(buffer, url);
        }

        /// <summary>
        /// 身份证识别
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static async Task<IdCardResult> IdCardRecognizeAsync(byte[] buffer)
        { 
            string url = ConfigurationManager.AppSettings["OCR_DONGZHENG_IDCARD"];

            return await GetResultAsync<IdCardResult>(buffer, url);
        }

        /// <summary>
        /// 银行卡识别
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static async Task<BankCardResult> BankCardRecognizeAsync(byte[] buffer)
        {
            string url = ConfigurationManager.AppSettings["OCR_DONGZHENG_BANKCARD"];

            return await GetResultAsync<BankCardResult>(buffer, url);
        }

        /// <summary>
        /// 购车发票识别
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static async Task<VehicleInvoiceResult> VehicleInvoiceRecognizeAsync(byte[] buffer)
        {
            string url = ConfigurationManager.AppSettings["OCR_DONGZHENG_VEHICLEINVOICE"];

            return await GetResultAsync<VehicleInvoiceResult>(buffer, url);
        }

        /// <summary>
        /// 驾驶证识别
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static async Task<DriverLicenseResult> DriverLicenseRecognizeAsync(byte[] buffer)
        {
            string url = ConfigurationManager.AppSettings["OCR_DONGZHENG_DRIVERLICENSE"];

            return await GetResultAsync<DriverLicenseResult>(buffer, url);
        }

        /// <summary>
        /// 车架号识别
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static async Task<VinResult> VinRecognizeAsync(byte[] buffer)
        {
            string url = ConfigurationManager.AppSettings["OCR_DONGZHENG_VIN"];

            return await GetResultAsync<VinResult>(buffer, url);
        }

        public static byte[] GetFileContent(string url)
        {
            byte[] fileBytes;

            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.KeepAlive = false;
            request.AllowAutoRedirect = false;
            request.Timeout = 10000;
            request.ContentType = "application/x-www-form-urlencoded;charset=utf-8";

            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                Stream stream = response.GetResponseStream();
                byte[] buffer = new byte[32 * 1024];
                int i;

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    while ((i = stream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        memoryStream.Write(buffer, 0, i);
                    }
                    fileBytes = memoryStream.ToArray();
                }
            }

            return fileBytes;
        }

        private static async Task<TResult> GetResultAsync<TResult>(byte[] buffer, string url, string method = "POST")
        {
            TResult result;

            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.ContentType = "application/x-www-form-urlencoded;charset=utf-8";
            request.Method = method;
            request.ContentLength = buffer.Length;
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(buffer, 0, buffer.Length);
            requestStream.Close();

            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    string content = await reader.ReadToEndAsync();
                    result = JsonConvert.DeserializeObject<TResult>(content);
                }
            }

            return result;
        }
    }
}