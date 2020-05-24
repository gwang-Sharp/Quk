using Fisk.EnterpriseManageBusiness.Mobile;
using Fisk.EnterpriseManageDataAccess;
using Fisk.EnterpriseManageSolution.Models;
using Fisk.EnterpriseManageUtilities.Common;
using Fisk.EnterpriseManageUtilities.WebPage;
using Fisk.EnterpriseManageViewModel;
using log4net;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Fisk.EnterpriseManageSolution.App_Start.handler
{
    public class QukAuthorizeAttribute : AuthorizeAttribute
    {
        #region 字段
        EnterpriseManageDBEntities db = new EnterpriseManageDBEntities();
        private User user = null;
        private string jumpurl = string.Empty;
        private bool isIgnore;//是否忽略验证
        private string resultMsg = string.Empty;
        public bool IsIgnore
        {
            get => isIgnore = true;

            set => isIgnore = value;
        }
        ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name);
        #endregion
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            try
            {
                Main main = new Main();
                var code = filterContext.RouteData.Values["id"].ToString();
                string access_token = main.GetTonken().CastToString();
                if (access_token == "False")
                {
                    this.isIgnore = false;
                    this.resultMsg = "系统内部错误,请联系管理员";
                    base.OnAuthorization(filterContext);
                    return;
                }
                JToken jsondata = JToken.Parse(GetUserInfo(access_token, code));
                if (jsondata != null)
                {
                    UserInfoVM Userinfo = new UserInfoVM();
                    Userinfo.UserId = jsondata["userid"].ToString();
                    Userinfo.UserName = jsondata["name"].ToString();
                    Userinfo.Position = jsondata["position"].ToString();
                    Userinfo.Email = jsondata["email"].ToString();
                    Userinfo.Position = "经理";
                    Userinfo.DepartmentID = jsondata["department"].ToString();
                    var Administrator = db.Administrator.AsNoTracking().Where(it => it.UserID == Userinfo.UserId).Any();
                    //var Administrator = db.Administrator.AsNoTracking().Where(it => it.UserID == "markc").Any();
                    if (Userinfo.Position.Contains(PublicClass.PmPosition) || Administrator)
                    {
                        this.jumpurl = PublicClass.PM_URL;
                        this.user = new User()
                        {
                            userid = "LiXinYi",
                            username ="李新毅",
                            departID = CastConvert.CastToInt32(Userinfo.DepartmentID.ToString().Replace("[", "").Replace("]", "").Replace(" ", "")),
                            Position = Userinfo.Position,
                            EmailAddress = Userinfo.Email
                        };
                        if (Administrator)
                        {
                            this.user.OnlyCheck = true;
                        }
                        this.resultMsg = "查询成功";
                        SessionHelper.Add("user", user);
                    }
                    else if (Userinfo.Position.Contains(PublicClass.EngineerPosition))
                    {
                        this.user = new User()
                        {
                            userid = Userinfo.UserId,
                            username = Userinfo.UserName,
                            departID = CastConvert.CastToInt32(Userinfo.DepartmentID.ToString().Replace("[", "").Replace("]", "").Replace(" ", "")),
                            Position = Userinfo.Position,
                            EmailAddress = Userinfo.Email
                        };
                        var Isprincipal = db.ProjectTeams.AsNoTracking().Where(it => it.IsPrincipal.ToLower() == "true" && it.UserId == Userinfo.UserId).Any();
                        if (Isprincipal)
                        {
                            this.jumpurl = PublicClass.PM_URL;
                            this.user.Isprincipal = true;
                        }
                        else
                        {
                            this.jumpurl = PublicClass.Engineer_URL;
                        }

                        this.resultMsg = "查询成功";
                        SessionHelper.Add("user", user);
                    }
                    else
                    {
                        this.resultMsg = "你没有权限查看";
                    }
                }
                else
                {
                    this.resultMsg = "获取用户信息失败！";
                }
                this.isIgnore = false;
                base.OnAuthorization(filterContext);
            }
            catch (System.Exception ex)
            {
                log.Error("获取用户信息失败,错误为：" + ex);
                this.resultMsg = "获取用户信息失败！";
                this.isIgnore = false;
                base.OnAuthorization(filterContext);
            }

        }
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            return isIgnore;
        }
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (user != null)
            {
                filterContext.HttpContext.Response.Write("<script>loading.close();</script>");
                filterContext.Result = new RedirectResult(this.jumpurl, false);//页面重定向
            }
            else
            {
                ContentResult contentResult = new ContentResult();
                contentResult.Content = $"<script>alert('{this.resultMsg}');</script>";
                filterContext.Result = contentResult;
            }
        }

        private string GetUserInfo(string access_token, string code)
        {
            try
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("access_token", access_token);
                dic.Add("code", code);
                string resultData = HttpUtils.DoGet(PublicClass.useridurl, dic);
                JToken JsondataUser = JToken.Parse(resultData);
                string UserId = JsondataUser["UserId"].ToString();
                Dictionary<string, string> dic1 = new Dictionary<string, string>();
                dic1.Add("access_token", access_token);
                dic1.Add("userid", UserId);
                string res = HttpUtils.DoGet(PublicClass.userinfourl, dic1);
                return res;
            }
            catch (System.Exception ex)
            {

                log.Error("执行GetUserInfo方法获取用户信息失败：" + ex);
                return null;
            }

        }
    }
}