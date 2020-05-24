using Fisk.EnterpriseManageBusiness.ReprotReference;
using Fisk.EnterpriseManageUtilities.Common;
using log4net;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using static Fisk.EnterpriseManageUtilities.Common.MailHelper;

namespace Fisk.EnterpriseManageBusiness.Mobile
{
    public class ReportService
    {
        private static readonly string ParamOne = ConfigHelper.GetConfigStr("rpt.CustomerName").Trim();
        private static readonly string ParamTwo = ConfigHelper.GetConfigStr("rpt.ProjectName").Trim();
        private static readonly string User = ConfigHelper.GetConfigStr("rpt.Admin").Trim();
        private static readonly string Pwd = ConfigHelper.GetConfigStr("rpt.AdminPassWord").Trim();
        private static readonly string _ServerUrl = ConfigHelper.GetConfigStr("rpt.ServerUrl");
        private static readonly string WeekReportUrl = "/Quk_WeeklyReport/WorkReport";
        private static readonly string WeeklyReportUrl = "/Quk_WeeklyReport/WeeklyReport";
        ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name);
        #region 属性
        private string _User;
        private string _PWD;
        private NetworkCredential _NetworkCredentials;
        public string ToEmail { get; set; }
        public string Domain { get; set; }
        public string UserName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_User))
                {
                    _User = User;
                }
                return _User;
            }
            set
            {
                _User = value;
            }
        }
        public string Password
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_PWD))
                {
                    _PWD = Pwd;
                }
                return _PWD;
            }
            set
            {
                _PWD = value;
            }
        }
        public NetworkCredential NetworkCredentials
        {
            get
            {
                if (_NetworkCredentials == null)
                {
                    _NetworkCredentials = new NetworkCredential(UserName, Password, Domain);
                }
                return _NetworkCredentials;
            }
            set
            {
                _NetworkCredentials = value;
            }
        }
        public ParameterValue[] Parameters { get; set; }
        public string ServerUrl { get; set; }
        public string FolderPath { get; set; }
        #endregion
        #region 导出报表
        /// <summary>
        /// 导出报表
        /// </summary>
        /// <param name="CustomerName">客户名称</param>
        /// <param name="ProjectName">项目名称</param>
        public bool Export(string CustomerName, string ProjectName, string LoginUser)
        {
            this.Parameters = new ParameterValue[2];
            this.Parameters[0] = new ParameterValue();
            this.Parameters[0].Label = ParamOne;
            this.Parameters[0].Name = ParamOne;
            this.Parameters[0].Value = CustomerName;
            this.Parameters[1] = new ParameterValue();
            this.Parameters[1].Label = ParamTwo;
            this.Parameters[1].Name = ParamTwo;
            this.Parameters[1].Value = ProjectName;
            this.ServerUrl = _ServerUrl;
            string path = HttpContext.Current.Server.MapPath("~/ReportServices/" + LoginUser + "/");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            if (Directory.GetFiles(path).Length > 0)
            {
                Directory.Delete(path, true);
            }
            bool success = false;
            try
            {
                GetReport(path);
                success = true;
                return success;
            }
            catch (Exception ex)
            {
                log.Error("执行ExportSevice导出报表出错：" + ex);
                success = false;
                return success;
            }
        }
        /// <summary>
        /// 获取周报
        /// </summary>
        /// <param name="fileName">导出报表路径</param>
        /// <returns></returns>
        public void GetReport(string fileName)
        {
            try
            {
                var task = Task.Run(() =>
                {
                    GetWeeklyReport(fileName);
                });
                var task2 = task.ContinueWith((t) =>
                {
                    if (task.IsCompleted)
                    {
                        task.Dispose();
                    }
                    GetWeekReport(fileName);
                    if (t.IsCompleted)
                    {
                        t.Dispose();
                    }
                });
                var task3 = task2.ContinueWith((t) =>
                {
                    if (task2.IsCompleted)
                    {
                        task2.Dispose();
                    }
                    _SendEmail(fileName);
                    if (t.IsCompleted)
                    {
                        task.Dispose();
                        t.Dispose();
                        GC.Collect();
                    }
                });
            }
            catch (Exception ex)
            {
                log.Error("执行GetReport方法获取报表并发送邮件出错：" + ex);
            }

        }
        public void _SendEmail(string fileName)
        {
            try
            {
                string[] files = {
                fileName + this.Parameters[1].Value + "_" + DateTime.Now.ToString("yyyy-MM-dd")+"_周报" + ".doc",
                fileName + this.Parameters[1].Value + "_" + DateTime.Now.ToString("yyyy-MM-dd")+"_组员日报" + ".doc"
            };
                SendMail(ToEmail, "", files, this.Parameters[1].Value + "_" + DateTime.Now.ToString("yyyy-MM-dd"));
            }
            catch (Exception ex)
            {

                log.Error("执行_SendEmail方法发送邮件出错：" + ex);
            }

        }

        /// <summary>
        /// 获取周报
        /// </summary>
        /// <param name="fileName">导出报表路径</param>
        public void GetWeeklyReport(string fileName)
        {
            try
            {
                ReportExecutionService rs = new ReportExecutionService();
                Warning[] warnings = null;
                ExecutionHeader execHeader = new ExecutionHeader();
                byte[] result = null;
                string historyId = null;
                string devinfo = "<DeviceInfo><Toolbar>True</Toolbar></DeviceInfo>";
                rs.Url = this.ServerUrl.Trim() + "/ReportExecution2005.asmx";
                this.FolderPath = WeeklyReportUrl;
                rs.Credentials = this.NetworkCredentials;
                rs.Timeout = 2000000;
                rs.ExecutionHeaderValue = execHeader;
                rs.LoadReport(this.FolderPath, historyId);
                rs.SetExecutionParameters(this.Parameters, "en_us");
                result = rs.Render("Word", devinfo, out string extension, out string mimetype, out string encoding, out warnings, out string[] streamid);
                if (!Directory.Exists(fileName))
                {
                    Directory.CreateDirectory(fileName);
                }
                using (FileStream fs = new FileStream(fileName + this.Parameters[1].Value + "_" + DateTime.Now.ToString("yyyy-MM-dd") + "_周报" + ".doc", FileMode.OpenOrCreate))
                {
                    fs.Write(result, 0, result.Length);
                    fs.Flush();
                    fs.Close();
                }
            }
            catch (Exception ex)
            {
                log.Error("执行GetWeeklyReport方法导出周报报表出错：" + ex);
            }

        }
        /// <summary>
        /// 获取周报详情
        /// </summary>
        /// <param name="fileName">导出报表路径</param>
        /// <returns></returns>
        public void GetWeekReport(string fileName)
        {
            try
            {
                ReportExecutionService rs = new ReportExecutionService();
                Warning[] warnings = null;
                ExecutionHeader execHeader = new ExecutionHeader();
                byte[] result = null;
                string historyId = null;
                string devinfo = "<DeviceInfo><Toolbar>True</Toolbar></DeviceInfo>";
                rs.Url = this.ServerUrl.Trim() + "/ReportExecution2005.asmx";
                this.FolderPath = WeekReportUrl;
                rs.Credentials = this.NetworkCredentials;
                rs.Timeout = 2000000;
                rs.ExecutionHeaderValue = execHeader;
                rs.LoadReport(this.FolderPath, historyId);
                rs.SetExecutionParameters(this.Parameters, "en_us");
                result = rs.Render("Word", devinfo, out string extension, out string mimetype, out string encoding, out warnings, out string[] streamid);
                if (!Directory.Exists(fileName))
                {
                    Directory.CreateDirectory(fileName);
                }
                using (FileStream fs = new FileStream(fileName + this.Parameters[1].Value + "_" + DateTime.Now.ToString("yyyy-MM-dd") + "_组员日报" + ".doc", FileMode.OpenOrCreate))
                {
                    fs.Write(result, 0, result.Length);
                    fs.Flush();
                    fs.Close();
                }
            }
            catch (Exception ex)
            {
                log.Error("执行GetWeekReport方法导出组员周报报表出错：" + ex);
            }
        }
        #endregion
    }
}
