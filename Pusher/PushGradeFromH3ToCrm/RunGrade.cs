using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace PushGradeFromH3ToCrm
{
    /// <summary>
    /// 推送间隔类型
    /// </summary>
    public enum PushInterval
    {
        /// <summary>
        /// 日
        /// </summary>
        Day = 0,
        /// <summary>
        /// 月
        /// </summary>
        Month,
        /// <summary>
        /// 年
        /// </summary>
        Year
    }
    /// <summary>
    /// 引擎委托
    /// </summary>
    /// <returns></returns>
    public delegate string DoSomething();
    /// <summary>
    /// 推送信息
    /// </summary>
    public class PusherInfo
    {
        /// <summary>
        /// 推送间隔
        /// </summary>
        public PushInterval PushInterval;
        /// <summary>
        /// 推送时间点
        /// </summary>
        public DateTime TimePoint;
        /// <summary>
        /// 推送委托实体
        /// </summary>
        public DoSomething TimerOut;
    }
    /// <summary>
    /// 计算类
    /// </summary>
    public class RunGrade
    {
        /// <summary>
        /// 单例
        /// </summary>
        private static RunGrade _self = new RunGrade();
        /// <summary>
        /// 计算线程
        /// </summary>
        private Thread _runner;
        /// <summary>
        /// 私有构造
        /// </summary>
        private RunGrade() {
        }
        /// <summary>
        /// 获取实例
        /// </summary>
        /// <returns></returns>
        public static RunGrade GetInstance()
        {
            return _self;
        }
        /// <summary>
        /// 运算实体
        /// </summary>
        /// <param name="info">推送信息</param>
        private void Runner(object info)
        {
            DateTime start = DateTime.Now;
            PusherInfo pusherInfo = info as PusherInfo;
            while (true)
            {
                try
                {
                    string ret = "";
                    switch (pusherInfo.PushInterval)
                    {
                        case PushInterval.Day:
                            if (DateTime.Now.Hour == ((DateTime)pusherInfo.TimePoint).Hour)
                            {
                                ret = pusherInfo.TimerOut();
                                Log.WriteLog(true, ret, (DateTime.Now - start).TotalSeconds, start);
                            }
                            break;
                        case PushInterval.Month:
                            if (DateTime.Now.Hour == ((DateTime)pusherInfo.TimePoint).Hour && DateTime.Now.Day == ((DateTime)pusherInfo.TimePoint).Day)
                            {
                                ret = pusherInfo.TimerOut();
                                Log.WriteLog(true, ret, (DateTime.Now - start).TotalSeconds, start);
                            }
                            break;
                        case PushInterval.Year:
                            if (DateTime.Now.Hour == ((DateTime)pusherInfo.TimePoint).Hour && DateTime.Now.Day == ((DateTime)pusherInfo.TimePoint).Day && DateTime.Now.Month == ((DateTime)pusherInfo.TimePoint).Month)
                            {
                                ret = pusherInfo.TimerOut();
                                Log.WriteLog(true, ret, (DateTime.Now - start).TotalSeconds, start);
                            }
                            break;
                    }
                }
                catch(Exception ex)
                {
                    Log.WriteLog(false, ex.ToString(), (DateTime.Now - start).TotalSeconds, start);
                }

                Thread.Sleep(60 * 60 * 1000);//计算分值的时间间隔
            }
        }
        /// <summary>
        /// 开始计算
        /// </summary>
        /// <param name="interval">计算间隔</param>
        /// <param name="timePoint">计算时间点</param>
        /// <param name="timerOut">计算实体</param>
        /// <returns></returns>
        public bool StartRunner(PushInterval interval, DateTime timePoint, DoSomething timerOut)
        {
            try
            {
                _runner = new Thread(new ParameterizedThreadStart(Runner));
                _runner.Start(new PusherInfo() { TimerOut = timerOut, PushInterval = interval, TimePoint = timePoint });
                Log.WriteLog(true, "服务启动成功", 0);
                return true;
            }
            catch(Exception ex)
            {
                Log.WriteLog(false, "服务启动错误：" + ex.ToString(), 0);
                return false;
            }
        }
        /// <summary>
        /// 计算状态
        /// </summary>
        /// <returns></returns>
        public bool RunnerState()
        {
            return _runner.IsAlive;
        }
        /// <summary>
        /// 停止计算
        /// </summary>
        /// <returns></returns>
        public bool StopRunner()
        {
            try
            {
                _runner.Abort();
                Log.WriteLog(true, "服务停止成功", 0);
                return true;
            }
            catch (Exception ex)
            {
                Log.WriteLog(false, "服务停止错误：" + ex.ToString(), 0);
                return false;
            }
        }
    }
}
