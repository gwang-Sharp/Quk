using Fisk.EnterpriseManageSolution.App_Start.handler;
using Fisk.EnterpriseManageUtilities.Common;
using Fisk.EnterpriseManageUtilities.WebPage;
using Fisk.EnterpriseManageViewModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Fisk.EnterpriseManageSolution.Controllers.Mobile
{

    public class MainController : Controller
    {
        private User user = null;
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var action = filterContext.ActionDescriptor.ActionName;
            if (user == null && action.ToLower() != "index")
            {
                if (SessionHelper.Get("user") == null)
                {
                    filterContext.Result = new RedirectResult("/main/index", false);
                }
                else
                {
                    user = SessionHelper.Get("user") as User;
                    ViewBag.onlyCheck = user.OnlyCheck;
                    ViewBag.Isprincipal = user.Isprincipal;
                }
            }
            base.OnActionExecuting(filterContext);
        }

        // GET: Main
        public ActionResult Index()
        {
            return View();
        }
        [QukAuthorize]
        public ActionResult Empty()
        {
            return View();
        }
        /// <summary>
        /// 项目列表页
        /// </summary>
        /// <returns></returns>
        public ActionResult ProjectList()
        {
            return View();
        }
        /// <summary>
        /// 项目详情页面
        /// </summary>
        /// <returns></returns>
        public ActionResult ProjectDetail()
        {
            Fisk.EnterpriseManageBusiness.Mobile.Main dll = new EnterpriseManageBusiness.Mobile.Main();
            var projectID = RouteData.Values["id"];
            if (projectID == null)
            {
                projectID = TempData["projectID"];
                TempData.Keep("projectID");
            }
            else
            {
                TempData["projectID"] = projectID;
                TempData.Keep();
            }
            //决定是进入项目详情页还是填写日报页
            if (dll.weekOrdaily(projectID.ToString(), user.userid))
            {
                if (!user.OnlyCheck && !user.Isprincipal)
                {
                    ViewBag.memberList = dll.GetProMemberAllTime(projectID.ToString(), user);
                }
                else
                {
                    ViewBag.memberList = JsonConvert.SerializeObject(new { msg = "查询失败", info = "管理者权限只能查看", onlyCheck = user.OnlyCheck, IsPrincipal = user.Isprincipal });
                }
                return View();
            }
            else
                return RedirectToAction("DailyReport", "main");
        }
        /// <summary>
        /// 日报页面
        /// </summary>
        /// <returns></returns>
        public ActionResult DailyReport()
        {
            Fisk.EnterpriseManageBusiness.Mobile.Main dll = new EnterpriseManageBusiness.Mobile.Main();
            ViewBag.pro = dll.GetMyProjects("日报", user);
            return View();
        }
        /// <summary>
        /// 重置日报
        /// </summary>
        /// <param name="day"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DailyRecordReset(string today)
        {
            Fisk.EnterpriseManageBusiness.Mobile.Main dll = new EnterpriseManageBusiness.Mobile.Main();
            return Json(dll.DailyRecordReset(today, user));
        }

        /// <summary>
        /// 周报页面
        /// </summary>
        /// <returns></returns>
        public ActionResult WeekReport()
        {
            Fisk.EnterpriseManageBusiness.Mobile.Main dll = new EnterpriseManageBusiness.Mobile.Main();
            ViewBag.pro = dll.GetMyProjects("周报", user);
            return View();
        }
        /// <summary>
        /// 周人天汇总
        /// </summary>
        /// <returns></returns>
        public ActionResult AllReportDetails()
        {
            var proid = TempData["projectID"].ToString();
            TempData.Keep("projectID");
            var uid = RouteData.Values["id"].ToString();
            Fisk.EnterpriseManageBusiness.Mobile.Main dll = new EnterpriseManageBusiness.Mobile.Main();
            ViewBag.details = JsonConvert.SerializeObject(dll.GetProJectDetails(proid, null, null, uid));
            return View();
        }
        /// <summary>
        /// 获取部门成员
        /// </summary>
        /// <param name="ProJectID">项目ID</param>
        /// <param name="departID">部门ID</param>
        /// <returns></returns>
        [HttpPost]
        //[OutputCache(CacheProfile = "outputCacheProfile", VaryByParam = "departID")]
        public ActionResult getDepartMember(string departID)
        {
            string ProJectID = TempData["projectID"].ToString();
            TempData.Keep("projectID");
            var token = TempData["token"].ToString();
            TempData.Keep("token");
            string url = "https://qyapi.weixin.qq.com/cgi-bin/user/list";
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("access_token", token);
            dic.Add("department_id", departID);
            dic.Add("fetch_child", "0");
            Fisk.EnterpriseManageBusiness.Mobile.Main dll = new EnterpriseManageBusiness.Mobile.Main();
            var result = HttpUtils.DoGet(url, dic);
            var allmembers = JObject.Parse(result)["userlist"];
            JArray array = JArray.FromObject(allmembers);//接口返回的成员列表

            var AllCreateUserIDs = dll.GetMemberByProjectID(ProJectID, user);
            JArray array2 = JArray.FromObject(AllCreateUserIDs);//系统返回的成员列表
            if (array2.Count > 0)
            {
                foreach (var item in array)
                {
                    foreach (var item2 in array2)
                    {
                        if (JObject.FromObject(item)["userid"].ToString() == JObject.FromObject(item2)["UserId"].ToString())
                        {
                            item["checked"] = "true";
                            break;
                        }
                    }
                }
            }
            var obj = new
            {
                members = array.ToString(),
                PmID = user.userid
            };
            Response.Cache.SetOmitVaryStar(true);
            return Json(obj);
        }
        /// <summary>
        /// 获取token
        /// </summary>
        /// <returns></returns>
        public ActionResult GetToken()
        {
            Fisk.EnterpriseManageBusiness.Mobile.Main dll = new EnterpriseManageBusiness.Mobile.Main();
            TempData["token"] = dll.GetTonken();
            TempData.Keep();
            return RedirectToAction("AddressBooks", "main");
        }
        /// <summary>
        /// 分配页面（部门页面）
        /// </summary>
        /// <returns></returns>
        [OutputCache(CacheProfile = "ViewCacheProfile", VaryByParam = "none")]
        public ActionResult AddressBooks()
        {
            var result = new JObject();
            Dictionary<string, string> dic = new Dictionary<string, string>();
            #region 获取token
            void GetDepartByToken(string TempData)
            {
                if (!dic.ContainsKey("access_token"))
                {
                    dic.Add("access_token", TempData);
                }
                var url = "https://qyapi.weixin.qq.com/cgi-bin/department/list";
                result = JObject.Parse(HttpUtils.DoGet(url, dic));
                if (TokenInvalid(result))
                    return;
            }
            #endregion
            #region token是否有效
            bool TokenInvalid(JObject GetDepartResult)
            {
                var errmsg = GetDepartResult["errmsg"].ToString();
                if (errmsg.Equals("invalid access_token"))
                {
                    SessionHelper.Del("Token");
                    CommonMethod.DelCookie("Token");
                    Fisk.EnterpriseManageBusiness.Mobile.Main dll = new EnterpriseManageBusiness.Mobile.Main();
                    TempData["token"] = dll.GetTonken();
                    dic.Remove("access_token");
                    dic.Add("access_token", TempData["token"].ToString());
                    TempData.Keep("token");
                    GetDepartByToken(null);
                }
                return true;
            }
            #endregion
            if (TempData["token"] == null)
            {
                //防止清空浏览器清除缓存再次刷新问题
                Fisk.EnterpriseManageBusiness.Mobile.Main dll = new EnterpriseManageBusiness.Mobile.Main();
                TempData["token"] = dll.GetTonken();
                TempData.Keep();
            }
            var token = TempData["token"].ToString();
            TempData.Keep("token");
            GetDepartByToken(token);
            var DepartValue = result["department"].ToString();
            var depArray = JArray.Parse(DepartValue);
            ViewBag.DepartMents = depArray;
            Response.Cache.SetOmitVaryStar(true);
            return View();
        }
        /// <summary>
        /// 添加周报
        /// </summary>
        /// <param name="proWeekLog"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult WeekRecordLog(string proWeekLog)
        {
            Fisk.EnterpriseManageBusiness.Mobile.Main dll = new EnterpriseManageBusiness.Mobile.Main();
            return Json(dll.myProWeekLogADD(proWeekLog, user));
        }
        /// <summary>
        /// 添加日报
        /// </summary>
        /// <param name="proDailyLog"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DailyRecordLog(string proDailyLog)
        {
            Fisk.EnterpriseManageBusiness.Mobile.Main dll = new EnterpriseManageBusiness.Mobile.Main();
            return Json(dll.myProDailyLogADD(proDailyLog, user));
        }
        /// <summary>
        /// 获取周报
        /// </summary>
        /// <param name="WeekDay">周</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetWeekRecord(string WeekDay)
        {
            Fisk.EnterpriseManageBusiness.Mobile.Main dll = new EnterpriseManageBusiness.Mobile.Main();
            return Json(dll.GetWeekRecords(WeekDay, user));
        }
        /// <summary>
        /// 更新周报
        /// </summary>
        /// <param name="week">周</param>
        /// <param name="proWeekLog">周报数据</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UpdateWeekLOG(string week, string proWeekLog)
        {
            Fisk.EnterpriseManageBusiness.Mobile.Main dll = new EnterpriseManageBusiness.Mobile.Main();
            return Json(dll.UpdateWeekLOGs(week, proWeekLog, user));
        }
        /// <summary>
        /// 获取日报
        /// </summary>
        /// <param name="day">日</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult LoadDailyLog(string day)
        {
            Fisk.EnterpriseManageBusiness.Mobile.Main dll = new EnterpriseManageBusiness.Mobile.Main();
            return Json(dll.LoadDailyLogs(day, user));
        }
        /// <summary>
        /// 更新日报
        /// </summary>
        /// <param name="day">日</param>
        /// <param name="proDailyLogs">日报数据</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UpdateDailyLog(string day, string proDailyLogs)
        {
            Fisk.EnterpriseManageBusiness.Mobile.Main dll = new EnterpriseManageBusiness.Mobile.Main();
            return Json(dll.UpdateDailyLogs(day, proDailyLogs, user));
        }
        /// <summary>
        /// 获取日报详情
        /// </summary>
        /// <param name="projectID">项目ID</param>
        /// <param name="week">日期</param>
        /// <param name="name">检索名</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetDetail(string week, string name)
        {
            Fisk.EnterpriseManageBusiness.Mobile.Main dll = new EnterpriseManageBusiness.Mobile.Main();
            var proID = TempData["projectID"].ToString();
            TempData.Keep("projectID");
            return Json(dll.GetProJectDetails(proID, week, name, null));
        }
        /// <summary>
        /// 项目分配成员
        /// </summary>
        /// <param name="members"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ProTeamAdd(string members)
        {
            string ProJectID = TempData["projectID"].ToString();
            TempData.Keep("projectID");
            Fisk.EnterpriseManageBusiness.Mobile.Main dll = new EnterpriseManageBusiness.Mobile.Main();
            return Json(dll.ProTeamAddMembers(user, ProJectID, members));
        }
        /// <summary>
        /// 删除项目成员
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>

        [HttpPost]
        public ActionResult proTeamDel(string userID)
        {
            var proId = TempData["projectID"].ToString();
            TempData.Keep("projectID");
            Fisk.EnterpriseManageBusiness.Mobile.Main dll = new EnterpriseManageBusiness.Mobile.Main();
            return Json(dll.ProTeamDelMember(user, proId, userID));
        }
        /// <summary>
        /// 设置项目负责人
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SetPrincipal(FormCollection form)
        {
            var proId = TempData["projectID"].ToString();
            TempData.Keep("projectID");
            Fisk.EnterpriseManageBusiness.Mobile.Main dll = new EnterpriseManageBusiness.Mobile.Main();
            return Json(dll.SetPrincipal(proId, form, user));
        }

        /// <summary>
        ///  获取项目列表
        /// </summary>
        [HttpPost]
        public ActionResult GetProjectList(FormCollection formCol)
        {
            Fisk.EnterpriseManageBusiness.Mobile.Main dll = new EnterpriseManageBusiness.Mobile.Main();
            formCol["UserId"] = user.userid;
            return Json(dll.GetProjectList(formCol, user), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        ///  获取项目详情
        /// </summary>
        [HttpPost]
        public ActionResult GetProjectDetail()
        {
            Fisk.EnterpriseManageBusiness.Mobile.Main dll = new EnterpriseManageBusiness.Mobile.Main();
            var proid = TempData["projectID"].ToString();
            TempData.Keep("projectID");
            return Json(dll.GetProjectDetail(proid, user), JsonRequestBehavior.AllowGet);

        }
        /// <summary>
        /// 企业微信消息推送
        /// </summary>
        /// <param name="msg">消息主题</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult MessageSend(string msg)
        {
            if (TempData["token"] == null)
            {
                Fisk.EnterpriseManageBusiness.Mobile.Main dll = new EnterpriseManageBusiness.Mobile.Main();
                TempData["token"] = dll.GetTonken();
                TempData.Keep("token");
            }
            var token = TempData["token"].ToString();
            var url = $"https://qyapi.weixin.qq.com/cgi-bin/message/send?access_token={token}";
            var result = HttpUtils.DoPost(url, msg);
            return Json(new { msg = result });
        }
        /// <summary>
        /// 添加项目周报详情
        /// </summary>
        /// <param name="WeeklyDetail"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult WeeklyDetailAdd(string WeeklyDetail)
        {
            Fisk.EnterpriseManageBusiness.Mobile.Main dll = new EnterpriseManageBusiness.Mobile.Main();
            var proID = TempData["projectID"].ToString();
            TempData.Keep("projectID");
            return Json(dll.WeeklyDetailAdds(user, WeeklyDetail, proID));
        }
        /// <summary>
        /// 获取周报报表
        /// </summary>
        /// <param name="CustomerName">客户名称</param>
        /// <param name="ProjectName">项目名称</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SendEmail(string CustomerName, string ProjectName)
        {
            if (string.IsNullOrWhiteSpace(user.EmailAddress))
            {
                return Json(new { sussess = false, msg = "请到企业微信个人资料中填写个人邮箱" });
            }
            Fisk.EnterpriseManageBusiness.Mobile.Main dll = new EnterpriseManageBusiness.Mobile.Main();
            var result = dll.ExportAsync(user.EmailAddress, CustomerName, ProjectName, user.userid);
            var obj = new { success = result, msg = result ? "邮件稍后发送至您的邮箱,务必确保邮箱地址正确！" : "系统出错,联系管理员!" };
            return Json(obj);
        }

    }
}