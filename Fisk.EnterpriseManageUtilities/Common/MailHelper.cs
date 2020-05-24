using log4net;
using System;
using System.Net.Mail;
using System.Text;

/*******************************************************************
 * * 版权所有：菲斯克（上海）软件有限公司
 * * 功    能： 邮件发送SMTP类
 * * 作    者：jerryli
 * * 电子邮箱：jerryl@runmont.com
 * * 创建日期： 2012/6/28 10:15:06
 * *******************************************************************/

namespace Fisk.EnterpriseManageUtilities.Common
{
    public class MailHelper
    {
        private readonly static string SmtpServer = "smtp.office365.com";
        private readonly static int Port = 587;//默认端口25
        private readonly static string EmailAccount = ConfigHelper.GetConfigStr("EmailAccount").Trim();
        private readonly static string EmailPassword = ConfigHelper.GetConfigStr("EmailPassWord").Trim();
        public static bool EnableSSL { get; set; } = true;//默认false
        private static Encoding myEncoding;
        private static ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name);
        public static Encoding MyEncoding
        {
            get
            {
                if (myEncoding == null)
                {
                    myEncoding = Encoding.UTF8;
                }
                return myEncoding;
            }
            set
            {
                myEncoding = value;
            }
        }
        /// <summary>
        /// 邮件发送
        /// </summary>
        /// <param name="EMialUrl">邮件接收者</param>
        /// <param name="CCMial">抄送者</param>
        /// <param name="File_Path">附件</param>
        /// <param name="SubjectTile">邮件标题</param>
        public static void SendMail(string EMialUrl, string CCMial, string[] File_Path, string SubjectTile)
        {
            try
            {
                MailMessage mailMessage = new MailMessage();
                mailMessage.Priority = MailPriority.Normal;
                mailMessage.From = new MailAddress(EmailAccount);
                mailMessage.To.Add(new MailAddress(EMialUrl));
                mailMessage.ReplyTo = new MailAddress(EmailAccount);
                if (!string.IsNullOrWhiteSpace(CCMial))
                {
                    mailMessage.CC.Add(new MailAddress(CCMial));
                }
                mailMessage.SubjectEncoding = MyEncoding;
                mailMessage.IsBodyHtml = false;
                mailMessage.Subject = SubjectTile;
                mailMessage.Body = "";
                foreach (var item in File_Path)
                {
                    mailMessage.Attachments.Add(new Attachment(item));
                }
                mailMessage.DeliveryNotificationOptions = DeliveryNotificationOptions.OnSuccess;
                SmtpClient client = new SmtpClient();
                client.TargetName = "STARTTLS/smtp.office365.com";
                client.Host = SmtpServer;
                client.EnableSsl = EnableSSL;
                client.UseDefaultCredentials = false;
                client.Port = Port;
                client.Credentials = new System.Net.NetworkCredential(EmailAccount, EmailPassword);
                mailMessage.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                client.Send(mailMessage);
                client.Dispose();
            }
            catch (Exception ex)
            {
                log.Error($"发送给{EMialUrl}的邮件发送失败，错误信息为{ex}");
            }
            finally
            {
                GC.Collect();
            }
        }
    }
}