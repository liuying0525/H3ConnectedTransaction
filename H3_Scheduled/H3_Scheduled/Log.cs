using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace H3_Scheduled
{
    public class Log
    {
        public static void WriteLog(bool isSuccess, string Message, double time, DateTime? dt = null)
        {
            string t = dt.HasValue ? dt.Value.ToString("yyyy_MM_dd_HH_mm_ss_fff") : DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss_fff");
            string body = String.Format("【{0}】耗时{3}s，在{1}完成。\r\n----------------------- Message -----------------------\r\n{2}\r\n------------------------- End -------------------------", isSuccess ? "成功" : "失败", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff"), Message, time);
            Console.WriteLine(body);
            if (!Directory.Exists("./Logs/")) Directory.CreateDirectory("./Logs/");
            FileStream fs = File.Open("./Logs/" + t + ".txt", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            fs.Seek(0, SeekOrigin.End);
            StreamWriter sw = new StreamWriter(fs);
            sw.WriteLine(body);
            sw.Close();
            fs.Close();
        }
    }
}
