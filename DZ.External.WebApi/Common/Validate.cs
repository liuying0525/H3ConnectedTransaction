using System;
using OThinker.H3.Controllers;
using System.Configuration;

namespace DZ.External.WebApi.Common
{
    public class Validate
    {
        public static string key = "h3bpmsys_key";
        public static string time = ConfigurationManager.AppSettings["expires_time"];
        public bool ValidateToken(string token)
        {
            string value = EncryptHelper.Decrypt(token, key);
            var values = value.Split('|');
            if (values.Length != 4)
            {
                return false;
            }
            var sso = AppUtility.Engine.SSOManager.GetSSOSystem(values[0]);
            if (sso == null)
                return false;
            else if (sso.Secret != MD5Encryptor.GetMD5(values[1]))
                return false;

            var d = long.Parse(values[2]);
            if (d < DateTime.Now.AddSeconds(-Convert.ToDouble(time)).Ticks)
                return false;
            return true;
        }
    }
}