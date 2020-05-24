/*
 * Author: Jerry Bai
 * Create Date: 2011/8/17
 * Description: Log Utility
 * Licence: BSD
 */
using System;
using System.IO;
using System.Configuration;


namespace Fisk.EnterpriseManageUtilities.Common
{
    /// <summary>
    /// 写日志，引用例子：Log.CreateLogManager()
    /// </summary>
    public class Log
    {
        /// <summary>
        ///创建log实体，根据配置文件中配置的节点：LogURL ，Demo： <add key="LogURL" value="C:\\DumexWeb\\DuoMexlog"/>
        /// </summary>
        /// <returns></returns>
        public static ILogManager CreateLogManager()
        {
            return CreateLogManager(ConfigurationManager.AppSettings["LogURL"]);
        }
        /// <summary>
        /// 创建Log实体，根据logFolder
        /// </summary>
        /// <param name="logFolder">参数实体</param>
        /// <returns></returns>
        public static ILogManager CreateLogManager(string logFolder)
        {
            return new FileLogManager(logFolder);
        }
    }

    public interface ILogManager
    {
        void Debug(string messageInfo);
        void Debug(System.Exception ex);
        void Debug(string messageInfo, System.Exception ex);

        void Warn(string messageInfo);
        void Warn(System.Exception ex);
        void Warn(string messageInfo, System.Exception ex);

        void Error(string messageInfo);
        void Error(System.Exception ex);
        void Error(string messageInfo, System.Exception ex);

    }

    internal class FileLogManager : ILogManager
    {
        private string _LogFolder = "";
        private static object _Lock = new object();

        internal FileLogManager(string logFolder)
        {
            _LogFolder = logFolder;
        }

        public void Debug(string messageInfo)
        {
            Log(0, messageInfo, null);
        }

        public void Debug(System.Exception ex)
        {
            Log(0, String.Empty, ex);
        }

        public void Debug(string messageInfo, System.Exception ex)
        {
            Log(0, messageInfo, ex);
        }

        public void Warn(string messageInfo)
        {
            Log(2, messageInfo, null);
        }

        public void Warn(System.Exception ex)
        {
            Log(2, String.Empty, ex);
        }

        public void Warn(string messageInfo, System.Exception ex)
        {
            Log(2, messageInfo, ex);
        }

        public void Error(string messageInfo)
        {
            Log(3, messageInfo, null);
        }

        public void Error(System.Exception ex)
        {
            Log(3, String.Empty, ex);
        }

        public void Error(string messageInfo, System.Exception ex)
        {
            Log(3, messageInfo, ex);
        }

        private void Log(int logLevel, string messageInfo, System.Exception ex)
        {
            lock (_Lock)
            {
                string logInfo = "";
                string logLevelText = "Error";
                switch (logLevel)
                {
                    case 0:
                        logLevelText = "Debug";
                        break;
                    case 2:
                        logLevelText = "Warn";
                        break;
                    case 3:
                        logLevelText = "Error";
                        break;
                    default:
                        logLevelText = "Error";
                        break;
                }
                if (ex == null)
                {
                    logInfo = String.Format("{0}:{1}\r\n", new object[] { DateTime.Now.ToString("HH:mm:ss"), messageInfo });
                }
                else
                {
                    if (String.IsNullOrEmpty(messageInfo))
                    {
                        logInfo = String.Format("{0}:{1}\r\n{2}\r\n", new object[] { DateTime.Now.ToString("HH:mm:ss"), ex.Message, ex.StackTrace });
                    }
                    else
                    {
                        logInfo = String.Format("{0}:{1}\r\n{2}\r\n{3}\r\n", new object[] { DateTime.Now.ToString("HH:mm:ss"), messageInfo, ex.Message, ex.StackTrace });
                    }
                }
                logInfo = logLevelText + "-" + logInfo;
                StreamWriter sw = null;
                try
                {
                    DirectoryInfo di = new DirectoryInfo(_LogFolder);
                    if (!di.Exists)
                    {
                        di.Create();
                    }
                    string logFile = di.FullName + Path.DirectorySeparatorChar + DateTime.Today.ToString("yyyy-MM-dd") + ".log";
                    if (File.Exists(logFile))
                    {
                        sw = File.AppendText(logFile);
                    }
                    else
                    {
                        sw = File.CreateText(logFile);
                    }
                    sw.WriteLine(logInfo);
                    sw.Flush();
                }
                catch(Exception innerException)
                {
                    throw new Exception("请检查Web.config中的appSettings节点，请确认是否已经配置“Wicresoft.ITSG.Components.LogFolder”或指定路径错误或无权限访问！内部异常：" + innerException.Message);
                }
                finally
                {
                    if (sw != null)
                    {
                        sw.Close();
                        sw.Dispose();
                    }
                }
            }
        }
    }
}
