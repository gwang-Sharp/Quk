using Fisk.EnterpriseManageDataAccess;
using Fisk.EnterpriseManageSolution.Models;
using Fisk.EnterpriseManageUtilities.Common;
using Fisk.EnterpriseManageUtilities.WebPage;
using Fisk.EnterpriseManageViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Web.Mvc;

namespace Fisk.EnterpriseManageBusiness.Mobile
{
    public class Main
    {
        private string MyProJectQuerySql = @"select p.ProjectId,p.ProjectName,p.ProjectType,p.ProjectStatus,p.ProjectStatusId
                                                    from [dbo].[ProjectInfo] p
                                                    left join[dbo].[ProjectTeams] pt on p.ProjectId=pt.ProjectId
                                                    where pt.UserId='{0}' and p.Validty=1 and CONVERT(varchar(100), p.StartTime, 23) <= '{1}' and p.ProjectStatusId<>6
                                                    union all
										            select p.ProjectId,p.ProjectName,p.ProjectType,p.ProjectStatus,p.ProjectStatusId
										            from [dbo].[ProjectInfo] p
										            where p.ProjectType='默认'
                                                    order by ProjectStatusId";
        EnterpriseManageDBEntities db = new EnterpriseManageDBEntities();
        private static object lockobj = new object();
        #region
        /// <summary>
        ///  获取访问令牌
        /// </summary>
        public object GetTonken()
        {
            try
            {
                //string cookieToken = Encryptor.AESDecrypt(CommonMethod.getCookie("Token").CastToString());
                string SessionToken = Encryptor.AESDecrypt(SessionHelper.Get("Token").CastToString());
                if (SessionToken == "" || SessionToken == null)
                {

                    Dictionary<string, string> dic = new Dictionary<string, string>();
                    dic.Add("corpid", PublicClass.corpid);
                    dic.Add("corpsecret", PublicClass.secret);
                    string resultData = HttpUtils.DoGet(PublicClass.tokenurl, dic);
                    dynamic jsonDataToken = Newtonsoft.Json.Linq.JToken.Parse(resultData) as dynamic;
                    string tokenValue = jsonDataToken["access_token"].Value;
                    SessionHelper.Add("Token", Encryptor.AESEncrypt(tokenValue));
                    return tokenValue;
                }
                else
                {
                    return SessionToken;
                }

            }
            catch (Exception ex)
            {

                PublicClass.log.Debug("获取Access_Token错误,错误信息：" + ex + "");
                return "False";

            }
        }
        /// <summary>
        /// 组装普通文本请求参数。
        /// </summary>
        /// <param name="parameters">Key-Value形式请求参数字典。</param>
        /// <returns>URL编码后的请求数据。</returns>
        private static string BuildPostData(IDictionary<string, string> parameters)
        {
            StringBuilder postData = new StringBuilder();
            bool hasParam = false;

            IEnumerator<KeyValuePair<string, string>> dem = parameters.GetEnumerator();
            while (dem.MoveNext())
            {
                string name = dem.Current.Key;
                string value = dem.Current.Value;
                if (hasParam)
                {
                    postData.Append("&");
                }

                postData.Append(name);
                postData.Append("=");
                postData.Append(Uri.EscapeDataString(value));
                hasParam = true;

            }

            return postData.ToString();
        }


        /// <summary>
        ///  获取项目列表
        /// </summary>
        public object GetProjectList(FormCollection formCol, User user)
        {
            string UserId = formCol["UserId"];
            int page = formCol["page"].CastToInt32();
            int limit = formCol["limit"].CastToInt32();
            string field = formCol["field"].CastToString();
            string order = formCol["order"].CastToString();
            string SearchContent = formCol["SearchContent"].CastToString();
            var obj = new object() { };
            int count = 0;
            try
            {
                var data = from a in db.ProjectInfo.AsNoTracking()
                           join b in db.ProjectTeams.AsNoTracking()
                           on a.ProjectId equals b.ProjectId
                           where a.Validty == 1 && b.CreateName == "system"
                           select new
                           {
                               a.PMid,
                               a.ProjectId,
                               a.ProjectName,
                               a.ProjectTypeId,
                               a.ProjectType,
                               a.ContractNumber,
                               a.SigningTime,
                               a.StartTime,
                               a.EstimateEndTime,
                               a.ProjectStatusId,
                               a.ProjectStatus,
                               a.EstimateManDay,
                               a.ProjectProgress,
                               a.ProjectHealth,
                               a.CreateTime,
                               a.CreateUserId,
                               a.CreateName,
                               a.ModifyTime,
                               a.ModifyUserId,
                               a.ModifyName,
                               WeekTimeSum = (from a1 in db.WeeklyRecord
                                              where a1.ProjectId == a.ProjectId
                                              select a1.WorkingTime
                                               ).Sum(),
                               DailyTimeSum =
                                               (from d1 in db.DailyRecord
                                                where d1.ProjectId == a.ProjectId
                                                select d1.WorkingTime
                                               ).Sum(),
                               DeveloperSum =
                                               (from pt in db.ProjectTeams
                                                where pt.ProjectId == a.ProjectId
                                                select 1
                                                  ).Sum(),
                           };

                //拼接where条件
                if (!string.IsNullOrEmpty(SearchContent))
                {
                    data = data.Where(u => u.ProjectName.Contains(SearchContent) || u.ProjectStatus.Contains(SearchContent));
                }
                //如果只是项目经理
                if (!user.OnlyCheck && !user.Isprincipal)
                {
                    data = data.Where(u => u.PMid == UserId && u.ProjectStatusId != 6);
                }
                //项目负责人
                if (user.Isprincipal)
                {
                    var myProjectId = db.ProjectTeams.AsNoTracking().Where(it => it.IsPrincipal.ToLower() == "true" && it.UserId == user.userid).Select(it => it.ProjectId).ToList();
                    data = data.Where(it => myProjectId.Contains(it.ProjectId) && it.ProjectStatusId != 6);
                }
                count = data.Count();
                //算总数
                if (data.Any())
                {

                    var resualtList = data.OrderBy(a => a.ProjectStatusId).OrderBy(field + " " + order).Skip((page - 1) * limit).Take(limit).ToList().Select(
                                              a => new
                                              {
                                                  a.ProjectId,
                                                  ProjectName = a.ContractNumber + "   " + a.ProjectName,
                                                  a.ProjectTypeId,
                                                  a.ProjectType,
                                                  a.ContractNumber,
                                                  SigningTime = a.SigningTime.Value.ToString("yyyy-MM-dd HH:ss:mm"),
                                                  StartTime = a.StartTime.Value.ToString("yyyy-MM-dd HH:ss:mm"),
                                                  EstimateEndTime = a.EstimateEndTime.Value.ToString("yyyy-MM-dd HH:ss:mm"),
                                                  a.ProjectStatusId,
                                                  a.ProjectStatus,
                                                  a.EstimateManDay,
                                                  ProjectProgress = CastConvert.CastToInt32(a.ProjectProgress),
                                                  a.ProjectHealth,
                                                  CreatTime = a.CreateTime.Value.ToString("yyyy-MM-dd HH:ss:mm"),
                                                  a.CreateUserId,
                                                  a.CreateName,
                                                  ModifyTime = a.ModifyTime.Value.ToString("yyyy-MM-dd HH:ss:mm"),
                                                  a.ModifyUserId,
                                                  a.ModifyName,
                                                  TimeProgress = !string.IsNullOrEmpty(GetTimeProgress(CastConvert.CastToDateTime(a.StartTime.Value.ToString("yyyy-MM-dd HH:ss:mm")), CastConvert.CastToDateTime(a.EstimateEndTime.Value.ToString("yyyy-MM-dd HH:ss:mm")))["TimeProgress"]) ? (GetTimeProgress(CastConvert.CastToDateTime(a.StartTime.Value.ToString("yyyy-MM-dd HH:ss:mm")), CastConvert.CastToDateTime(a.EstimateEndTime.Value.ToString("yyyy-MM-dd HH:ss:mm")))["TimeProgress"]) : "0",
                                                  ProgressExceed = GetTimeProgress(CastConvert.CastToDateTime(a.StartTime.Value.ToString("yyyy-MM-dd HH:ss:mm")), CastConvert.CastToDateTime(a.EstimateEndTime.Value.ToString("yyyy-MM-dd HH:ss:mm")))["ProgressExceed"],
                                                  ProgressCss = GetTimeProgress(CastConvert.CastToDateTime(a.StartTime.Value.ToString("yyyy-MM-dd HH:ss:mm")), CastConvert.CastToDateTime(a.EstimateEndTime.Value.ToString("yyyy-MM-dd HH:ss:mm")))["ProgressCss"],
                                                  PersonnelUse = GetPersonnelUse(a.EstimateManDay, a.WeekTimeSum, a.DailyTimeSum),
                                                  a.DeveloperSum
                                              }
                                         );

                    obj = new
                    {
                        count = count,
                        data = resualtList,
                        message = "查询成功！",
                        success = true,
                        onlyCheck = user.OnlyCheck
                    };
                }
                else
                {
                    obj = new
                    {
                        count = count,
                        data = "",
                        message = "暂无数据！",
                        success = true,
                        onlyCheck = user.OnlyCheck
                    };

                }

            }
            catch (Exception ex)
            {
                PublicClass.log.Error("GetProjectList方法获取项目列表异常,错误信息" + ex);

                obj = new
                {
                    count = count,
                    data = "",
                    message = "系统内部错误，请联系管理员！",
                    success = false,
                    onlyCheck = user.OnlyCheck
                };
            }
            return obj;
        }

        /// <summary>
        /// 计算两个日期之间相差的天数
        /// </summary>
        /// <param name="Startdate"></param>
        /// <param name="Enddate"></param>
        /// <returns></returns>
        private int DateDiff(DateTime Startdate, DateTime Enddate)
        {
            if (Enddate == null)
            {
                return 0;
            }
            else
            {
                DateTime start = Convert.ToDateTime(Startdate.ToShortDateString());
                DateTime end = Convert.ToDateTime(Enddate.ToShortDateString());
                TimeSpan sp = end.Subtract(start);
                return sp.Days;
            }
        }

        /// <summary>
        /// 计算当前时间进度
        /// </summary>
        /// <param name="Startdate">开始时间</param>
        /// <param name="EstimateEndTime">预计结束时间</param>
        /// <returns></returns>
        private Dictionary<string, string> GetTimeProgress(DateTime? StartTime, DateTime? EstimateEndTime)
        {
            //获取计划时长
            double? Date1 = DateDiff(CastConvert.CastToDateTime(StartTime), CastConvert.CastToDateTime(EstimateEndTime));
            //获取进行时长
            double? Date2 = DateDiff(CastConvert.CastToDateTime(StartTime), CastConvert.CastToDateTime(System.DateTime.Now.ToString("yyyy-MM-dd HH:ss:mm")));
            double? TimeProgress = null;
            if (Date1 == 0.0)
            {
                TimeProgress = 0;
            }
            else
            {
                TimeProgress = Math.Round(CastConvert.CastToDouble(Date2 / Date1 * 100), 2, MidpointRounding.AwayFromZero);
            }
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("TimeProgress", CastConvert.CastToString(Math.Round(TimeProgress.Value)));
            if (TimeProgress <= 80)
            {
                dic.Add("ProgressCss", "tag_status3");

            }
            else if (TimeProgress >= 80)
            {
                dic.Add("ProgressCss", "tag_status2");
            }
            else
            {
                dic.Add("ProgressCss", "tag_status1");
            }
            //逾期
            if (Date2 > Date1)
            {

                dic.Add("ProgressExceed", "(逾期" + (Date2 - Date1) + "天)");

                return dic;
            }
            else
            {
                dic.Add("ProgressExceed", "");
            }
            return dic;

        }

        /// <summary>
        /// 计算人天匹配度
        /// </summary>
        /// <param name="EstimateManDay">预计人天</param>
        /// <param name="WeekTimeSum">PM人天</param>
        /// <param name="DailyTimeSum">组员人天</param>
        /// <returns></returns>
        private string GetPersonnelUse(double? EstimateManDay, int? WeekTimeSum, Double? DailyTimeSum)
        {
            if (EstimateManDay == null || EstimateManDay <= 0)
            {
                return "0";
            }
            else
            {
                //获取当前使用人天
                var ThisDay = CastConvert.CastToDouble((WeekTimeSum == null ? 0 : WeekTimeSum) + (DailyTimeSum == null ? 0 : DailyTimeSum));
                //获取人天匹配度
                return CastConvert.CastToString(Math.Round(CastConvert.CastToDouble(ThisDay / EstimateManDay * 100), 2, MidpointRounding.AwayFromZero));
            }
        }

        /// <summary>
        /// 获取项目详情
        /// </summary>
        public object GetProjectDetail(string proID, User user)
        {
            var obj = new object();
            int count = 0;
            GregorianCalendar gc = new GregorianCalendar();
            int thisWeek = gc.GetWeekOfYear(System.DateTime.Now, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
            DateTime NowTime = System.DateTime.Now;   //当前时间
            //查询项目详情
            try
            {
                var benzhou = DateTime.Parse(GetBenZhouDate());
                var benzhouWeekDetail = (from a in db.WeeklyDetail.AsNoTracking()
                                         where a.Validty == 1 && a.CreateTime == benzhou && a.ProjectId == proID
                                         select new
                                         {
                                             a.ProjectStatus,
                                             a.WeeklyContent,
                                             a.Differences,
                                             a.NextWeekPlans
                                         }).FirstOrDefault();
                var status = benzhouWeekDetail?.ProjectStatus ?? "请选择状态";
                var Differences = benzhouWeekDetail?.Differences ?? "";
                var NextWeekPlans = benzhouWeekDetail?.NextWeekPlans ?? "";
                var weeklyContent = benzhouWeekDetail?.WeeklyContent;
                var data = from a in db.ProjectInfo.AsNoTracking()
                           where a.Validty == 1 && a.ProjectId == proID
                           select new
                           {
                               a.ProjectId,
                               a.ProjectName,
                               a.CustomerName,
                               a.ProjectTypeId,
                               a.ProjectType,
                               a.ContractNumber,
                               a.ContractDays,
                               a.SigningTime,
                               a.StartTime,
                               a.EstimateEndTime,
                               a.ProjectStatusId,
                               ProjectStatus = status,
                               a.EstimateManDay,
                               a.ProjectProgress,
                               a.ProjectHealth,
                               a.CreateTime,
                               a.CreateUserId,
                               a.CreateName,
                               a.ModifyTime,
                               a.ModifyUserId,
                               a.ModifyName,
                               WeekTimeSum = (from a1 in db.WeeklyRecord
                                              where a1.ProjectId == a.ProjectId
                                              select a1.WorkingTime
                                               ).Sum() ?? 0,
                               DailyTimeSum =
                                               (from d1 in db.DailyRecord
                                                where d1.ProjectId == a.ProjectId
                                                select d1.WorkingTime
                                               ).Sum() ?? 0,
                               DeveloperSum =
                                               (from pt in db.ProjectTeams
                                                where pt.ProjectId == a.ProjectId
                                                select 1
                                                  ).Sum(),
                               WeeklyContent = weeklyContent,
                               Differences,
                               NextWeekPlans
                           };
                if (data.Any())
                {
                    var resultData = data.ToList().Select(
                                              a => new
                                              {
                                                  a.CustomerName,
                                                  a.WeekTimeSum,
                                                  a.DailyTimeSum,
                                                  a.ProjectId,
                                                  a.ProjectName,
                                                  a.ProjectTypeId,
                                                  a.ProjectType,
                                                  a.ContractNumber,
                                                  a.EstimateManDay,
                                                  ContractDays = a.ContractDays ?? 0,
                                                  SigningTime = a.SigningTime.Value.ToString("yyyy-MM-dd HH:ss:mm"),
                                                  StartTime = a.StartTime.Value.ToString("yyyy-MM-dd HH:ss:mm"),
                                                  EstimateEndTime = a.EstimateEndTime.Value.ToString("yyyy-MM-dd HH:ss:mm"),
                                                  a.ProjectStatusId,
                                                  a.ProjectStatus,
                                                  ProjectProgress = CastConvert.CastToInt32(a.ProjectProgress),
                                                  ProjectProgressCss = CastConvert.CastToInt32(a.ProjectProgress) > 100 ? 100 : CastConvert.CastToInt32(a.ProjectProgress),
                                                  a.ProjectHealth,
                                                  CreatTime = a.CreateTime.Value.ToString("yyyy-MM-dd HH:ss:mm"),
                                                  a.CreateUserId,
                                                  a.CreateName,
                                                  ModifyTime = a.ModifyTime.Value.ToString("yyyy-MM-dd HH:ss:mm"),
                                                  a.ModifyUserId,
                                                  a.ModifyName,
                                                  TimeProgress = CastConvert.CastToDouble(GetTimeProgress(CastConvert.CastToDateTime(a.StartTime.Value.ToString("yyyy-MM-dd HH:ss:mm")), CastConvert.CastToDateTime(a.EstimateEndTime.Value.ToString("yyyy-MM-dd HH:ss:mm")))["TimeProgress"]) >= 0.0 ? CastConvert.CastToDouble(GetTimeProgress(CastConvert.CastToDateTime(a.StartTime.Value.ToString("yyyy-MM-dd HH:ss:mm")), CastConvert.CastToDateTime(a.EstimateEndTime.Value.ToString("yyyy-MM-dd HH:ss:mm")))["TimeProgress"]) : 0,
                                                  TimeProgressCss = CastConvert.CastToDouble(GetTimeProgress(CastConvert.CastToDateTime(a.StartTime.Value.ToString("yyyy-MM-dd HH:ss:mm")), CastConvert.CastToDateTime(a.EstimateEndTime.Value.ToString("yyyy-MM-dd HH:ss:mm")))["TimeProgress"]) > 100 ? 100 : CastConvert.CastToDouble(GetTimeProgress(CastConvert.CastToDateTime(a.StartTime.Value.ToString("yyyy-MM-dd HH:ss:mm")), CastConvert.CastToDateTime(a.EstimateEndTime.Value.ToString("yyyy-MM-dd HH:ss:mm")))["TimeProgress"]) >= 0.0 ? CastConvert.CastToDouble(GetTimeProgress(CastConvert.CastToDateTime(a.StartTime.Value.ToString("yyyy-MM-dd HH:ss:mm")), CastConvert.CastToDateTime(a.EstimateEndTime.Value.ToString("yyyy-MM-dd HH:ss:mm")))["TimeProgress"]) > 100 ? 100 : CastConvert.CastToDouble(GetTimeProgress(CastConvert.CastToDateTime(a.StartTime.Value.ToString("yyyy-MM-dd HH:ss:mm")), CastConvert.CastToDateTime(a.EstimateEndTime.Value.ToString("yyyy-MM-dd HH:ss:mm")))["TimeProgress"]) : 0,
                                                  ProgressCss = GetTimeProgress(CastConvert.CastToDateTime(a.StartTime.Value.ToString("yyyy-MM-dd HH:ss:mm")), CastConvert.CastToDateTime(a.EstimateEndTime.Value.ToString("yyyy-MM-dd HH:ss:mm")))["ProgressCss"],
                                                  ActualDays = Math.Round((a.WeekTimeSum + a.DailyTimeSum), 2),
                                                  PersonnelUse = CastConvert.CastToDouble(GetPersonnelUse(a.EstimateManDay, a.WeekTimeSum, a.DailyTimeSum)) >= 0.0 ? CastConvert.CastToDouble(GetPersonnelUse(a.EstimateManDay, a.WeekTimeSum, a.DailyTimeSum)) : 0,
                                                  PersonnelUseCss = CastConvert.CastToDouble(GetPersonnelUse(a.EstimateManDay, a.WeekTimeSum, a.DailyTimeSum)) > 100 ? 100 : CastConvert.CastToDouble(GetPersonnelUse(a.EstimateManDay, a.WeekTimeSum, a.DailyTimeSum)),
                                                  NaturalWeek = NowTime.ToString("yyyy-MM-dd") + " (第" + thisWeek + "周)",
                                                  ProjectWeek = "第" + Math.Ceiling((double)DateDiff(a.StartTime.CastToDateTime(), NowTime) / 7) + "周",
                                                  a.WeeklyContent,
                                                  a.Differences,
                                                  a.NextWeekPlans
                                              }
                                         );
                    obj = new
                    {
                        data = resultData,
                        message = "查询成功！",
                        success = true
                    };
                }
                else
                {
                    obj = new
                    {
                        data = "",
                        message = "暂无数据！",
                        success = true

                    };

                }

            }
            catch (Exception ex)
            {
                PublicClass.log.Error("GetProjectDetail方法获取项目详情异常,错误信息" + ex);

                obj = new
                {
                    count = count,
                    data = "",
                    message = "系统内部错误，请联系管理员！",
                    success = false
                };
            }
            return obj;
        }

        #endregion
        /// <summary>
        /// 获取我的项目
        /// </summary>
        /// <returns></returns>
        public object GetMyProjects(string reportType, User user)
        {
            var obj = new object();
            try
            {
                if (reportType == "周报")
                {
                    #region 是否有本周的日报
                    var BenZhouTime = GetBenZhouDate();//获取本周周一的时间
                    var weekQuerySql = $@"select p.ProjectId,p.ProjectName,p.ProjectType,p.ProjectStatus,p.ProjectStatusId, w.WorkingTime
                                        from [dbo].[ProjectInfo] p
                                        left join [dbo].[WeeklyRecord] w on p.ProjectId=w.ProjectId
							            where w.CreateUserId='{user.userid}' and w.CreateTime='{BenZhouTime}' and p.Validty=1 and p.ProjectStatusId<>6
                                        order by ProjectStatusId";
                    var weekReportData = db.Database.SqlQuery<MyProjects2>(weekQuerySql);
                    if (weekReportData.Any())//是否含有本周的周报数据
                    {
                        MyProJectQuerySql = string.Format(MyProJectQuerySql, user.userid, DateTime.Parse(BenZhouTime).AddDays(6).ToString("yyyy-MM-dd"));
                        var myProjects = db.Database.SqlQuery<MyProjects>(MyProJectQuerySql).ToList();
                        var weekReports = weekReportData.ToList();
                        myProjects.ForEach(p =>
                        {
                            var equalP = weekReports.Find(it => it.ProjectId == p.ProjectId);
                            if (equalP != null)
                            {
                                p.WorkingTime = equalP.WorkingTime;
                            }
                        });
                        obj = new
                        {
                            msg = "查询成功",
                            Pros = myProjects.GroupBy(it => it.ProjectType),
                            success = true,
                            Edit = true
                        };
                        return SerializeHelper.JsonSerialize(obj);
                    }
                    #endregion
                }
                else
                {
                    #region 是否有本日的日报
                    var dayTime = DateTime.Now.ToString("yyyy-MM-dd");//对应数据库的时间格式
                    var t = DateTime.Parse(dayTime);
                    var BenRIQuery = $@"select p.ProjectId,p.ProjectName,p.ProjectType,p.ProjectStatus,p.ProjectStatusId, d.WorkingTime,d.Remark
                                        from [dbo].[ProjectInfo] p
                                        left join [dbo].[DailyRecord] d on p.ProjectId=d.ProjectId
							            where d.CreateUserId='{user.userid}' and d.CreateTime='{t}' and p.Validty=1 and p.ProjectStatusId<>6
                                        order by ProjectStatusId";
                    var DailyReportData = db.Database.SqlQuery<MyProjects>(BenRIQuery);
                    if (DailyReportData.Any())//是否含有本日的日报
                    {
                        //都没有就只获取本人的项目信息
                        MyProJectQuerySql = string.Format(MyProJectQuerySql, user.userid, DateTime.Now.ToString("yyyy-MM-dd"));
                        var mProjects = db.Database.SqlQuery<MyProjects>(MyProJectQuerySql).ToList();
                        var dailyReports = DailyReportData.ToList();
                        mProjects.ForEach(p =>
                        {
                            var equalprject = dailyReports.Find(it => it.ProjectId == p.ProjectId);
                            if (equalprject != null)
                            {
                                p.WorkingTime = equalprject.WorkingTime;
                            }
                            p.Remark = dailyReports.Find(it => it.ProjectId == p.ProjectId).Remark;
                        });
                        obj = new
                        {
                            msg = "查询成功",
                            Pros = mProjects.GroupBy(it => it.ProjectType),
                            success = true,
                            Edit = true
                        };
                        return SerializeHelper.JsonSerialize(obj);
                    }
                    #endregion
                }
                if (reportType == "周报")
                {
                    //都没有就只获取本人的项目信息
                    MyProJectQuerySql = string.Format(MyProJectQuerySql, user.userid, DateTime.Parse(GetBenZhouDate()).AddDays(6).ToString("yyyy-MM-dd"));
                }
                else
                {
                    //都没有就只获取本人的项目信息
                    MyProJectQuerySql = string.Format(MyProJectQuerySql, user.userid, DateTime.Now.ToString("yyyy-MM-dd"));
                }

                var Projects = db.Database.SqlQuery<MyProjects>(MyProJectQuerySql).ToList().GroupBy(it => it.ProjectType);
                obj = new
                {
                    msg = "查询成功",
                    Pros = Projects,
                    success = true,
                    Edit = false
                };
                return SerializeHelper.JsonSerialize(obj);
            }
            catch (Exception ex)
            {
                PublicClass.log.Error("GetMyProjects方法获取我的项目列表出错:" + ex);
                obj = new
                {
                    msg = "操作异常,请查看日志文件,联系管理员！",
                    Pros = 0,
                    success = false,
                    Edit = false
                };
                return SerializeHelper.JsonSerialize(obj);
            }
        }

        /// <summary>
        /// 获取本周周一的日期
        /// </summary>
        /// <returns></returns>
        public static string GetBenZhouDate()
        {
            var dayofweek = DateTime.Now.DayOfWeek;
            var BenZhou = string.Empty;
            switch (dayofweek)
            {
                case DayOfWeek.Monday:
                    BenZhou = DateTime.Now.ToString("yyyy-MM-dd");
                    break;
                case DayOfWeek.Tuesday:
                    BenZhou = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
                    break;
                case DayOfWeek.Wednesday:
                    BenZhou = DateTime.Now.AddDays(-2).ToString("yyyy-MM-dd");
                    break;
                case DayOfWeek.Thursday:
                    BenZhou = DateTime.Now.AddDays(-3).ToString("yyyy-MM-dd");
                    break;
                case DayOfWeek.Friday:
                    BenZhou = DateTime.Now.AddDays(-4).ToString("yyyy-MM-dd");
                    break;
                case DayOfWeek.Saturday:
                    BenZhou = DateTime.Now.AddDays(-5).ToString("yyyy-MM-dd");
                    break;
                case DayOfWeek.Sunday:
                    BenZhou = DateTime.Now.AddDays(-6).ToString("yyyy-MM-dd");
                    break;
            }
            return BenZhou;
        }
        /// <summary>
        /// 添加周报
        /// </summary>
        /// <param name="proWeekLog">周报数据</param>
        /// <returns></returns>
        public object myProWeekLogADD(string proWeekLog, User user)
        {
            var obj = new object();
            try
            {
                var time = DateTime.Now;
                var WeekLog = JsonConvert.DeserializeObject<List<List<ProWeekLog>>>(proWeekLog);
                List<WeeklyRecord> dailyRecords = new List<WeeklyRecord>();
                foreach (var item in WeekLog)
                {
                    item.ForEach(w =>
                    {
                        WeeklyRecord weekRecord = new WeeklyRecord();
                        weekRecord.ProjectId = w.ProjectId;
                        weekRecord.WorkingTime = w.WorkingTime;
                        weekRecord.Validty = 1;
                        weekRecord.Remark = "无";
                        weekRecord.Position = user.Position;
                        weekRecord.CreateTime = w.NowWeekDay;
                        weekRecord.CreateName = user.username;
                        weekRecord.CreateUserId = user.userid;
                        weekRecord.ModifyTime = w.NowWeekDay;
                        weekRecord.ModifyUserId = user.userid;
                        weekRecord.DepartmentID = user.departID;
                        weekRecord.ModifyName = user.username;
                        dailyRecords.Add(weekRecord);
                    });
                }
                db.WeeklyRecord.AddRange(dailyRecords);
                db.SaveChanges();
                obj = new { msg = "提交成功", success = true };
                return obj;
            }
            catch (Exception ex)
            {
                PublicClass.log.Error("myProWeekLogADD方法添加我的周报日志出错:" + ex);
                obj = new { msg = "操作异常,请查看日志文件,联系管理员！", success = false };
                return obj;
            }

        }
        /// <summary>
        /// 添加日报
        /// </summary>
        /// <param name="proDailyLog">日报数据</param>
        /// <returns></returns>
        public object myProDailyLogADD(string proDailyLog, User user)
        {
            var obj = new object();
            try
            {
                var time = DateTime.Now;
                var dailyLog = JsonConvert.DeserializeObject<List<List<ProDailyLog>>>(proDailyLog);
                List<DailyRecord> dailyRecords = new List<DailyRecord>();
                foreach (var item in dailyLog)
                {
                    item.ForEach(d =>
                    {
                        DailyRecord dailyRecord = new DailyRecord();
                        dailyRecord.DepartmentID = user.departID;
                        dailyRecord.ProjectId = d.ProjectId;
                        dailyRecord.WorkingTime = d.WorkingTime;
                        dailyRecord.Validty = 1;
                        dailyRecord.Remark = d.Remark;
                        dailyRecord.Position = user.Position ?? "工程师";
                        dailyRecord.CreateTime = d.ThatDay;
                        dailyRecord.CreateName = user.username;
                        dailyRecord.CreateUserId = user.userid;
                        dailyRecord.ModifyTime = d.ThatDay;
                        dailyRecord.ModifyUserId = user.userid;
                        dailyRecord.ModifyName = user.username;
                        dailyRecords.Add(dailyRecord);
                    });
                }
                db.DailyRecord.AddRange(dailyRecords);
                db.SaveChanges();
                obj = new { msg = "提交成功", success = true };
                return obj;
            }
            catch (Exception ex)
            {
                //DbEntityValidationException
                PublicClass.log.Error("myProDailyLogADD方法添加我的日报日志出错:" + ex);
                obj = new { msg = "操作异常,请查看日志文件,联系管理员！", success = false };
                return obj;
            }

        }
        /// <summary>
        /// 获取周报
        /// </summary>
        /// <param name="week">周</param>
        /// <returns></returns>
        public object GetWeekRecords(string week, User user)
        {
            var obj = new object();
            try
            {
                var weekQuerySql = $@"select p.ProjectId,p.ProjectName,p.ProjectType,p.ProjectStatus,p.ProjectStatusId, w.WorkingTime
                                        from [dbo].[ProjectInfo] p
                                        left join [dbo].[WeeklyRecord] w on p.ProjectId=w.ProjectId
							            where w.CreateUserId='{user.userid}' and w.CreateTime='{week}' and p.Validty=1 and p.ProjectStatusId<>6
                                        order by ProjectStatusId";
                var weekReportData = db.Database.SqlQuery<MyProjects2>(weekQuerySql);
                if (weekReportData.Any())
                {
                    obj = weekReportData.ToList().GroupBy(it => it.ProjectType);
                }
                else
                {
                    var thisweek = DateTime.Parse(week).AddDays(6).ToString("yyyy-MM-dd");
                    MyProJectQuerySql = string.Format(MyProJectQuerySql, user.userid, thisweek);
                    obj = db.Database.SqlQuery<MyProjects>(MyProJectQuerySql).ToList().GroupBy(it => it.ProjectType);
                }
                return obj;
            }
            catch (Exception ex)
            {
                PublicClass.log.Error("GetWeekRecords方法获取周报日志出错:" + ex);
                obj = null;
                return obj;
            }
        }
        /// <summary>
        /// 更新周报
        /// </summary>
        /// <param name="week">周</param>
        /// <param name="proWeekLog">周报数据</param>
        /// <returns></returns>
        public object UpdateWeekLOGs(string week, string proWeekLog, User user)
        {
            var obj = new object();
            try
            {
                var Date = DateTime.Parse(week);
                var weekData = from w in db.WeeklyRecord
                               where w.CreateTime == Date && w.CreateUserId == user.userid
                               select w;
                var WeekLog = JsonConvert.DeserializeObject<List<List<MyProjects2>>>(proWeekLog);
                var time = DateTime.Now;
                db.WeeklyRecord.RemoveRange(weekData.ToList());
                List<WeeklyRecord> dailyRecords = new List<WeeklyRecord>();
                foreach (var item in WeekLog)
                {
                    item.ForEach(w =>
                    {
                        WeeklyRecord weekRecord = new WeeklyRecord();
                        weekRecord.ProjectId = w.ProjectId;
                        weekRecord.WorkingTime = w.WorkingTime;
                        weekRecord.Validty = 1;
                        weekRecord.Remark = "无";
                        weekRecord.Position = user.Position;
                        weekRecord.CreateTime = Date;
                        weekRecord.DepartmentID = user.departID;
                        weekRecord.CreateName = user.username;
                        weekRecord.CreateUserId = user.userid;
                        weekRecord.ModifyTime = Date;
                        weekRecord.ModifyUserId = user.userid;
                        weekRecord.ModifyName = user.username;
                        dailyRecords.Add(weekRecord);
                    });
                }
                db.WeeklyRecord.AddRange(dailyRecords);
                db.SaveChanges();
                obj = new
                {
                    msg = "更新成功",
                    success = true
                };
                return obj;
            }
            catch (Exception ex)
            {
                PublicClass.log.Error("UpdateWeekLOGs方法更新周报日志出错:" + ex);
                obj = new { msg = "操作异常,请查看日志文件,联系管理员！", success = false };
                return obj;
            }

        }
        /// <summary>
        ///重置日报
        /// </summary>
        /// <param name="day">当天日期</param>
        /// <param name="user">当前操作人</param>
        /// <returns></returns>
        public object DailyRecordReset(string day, User user)
        {
            var obj = new object();
            try
            {
                var DATE = DateTime.Parse(day).ToString("yyyy-MM-dd");
                var sql = $"delete from [dbo].[DailyRecord] where CreateTime='{DATE}' and CreateUserId='{user.userid}'";
                var result = db.Database.ExecuteSqlCommand(sql);
                if (result >= 0)
                {
                    obj = new { success = true, msg = "请重新填写" };
                }
                return obj;
            }
            catch (Exception ex)
            {
                PublicClass.log.Error("DailyRecordReset方法重置日报出错:" + ex);
                obj = new { success = false, msg = "操作异常，联系管理员" };
                return obj;
            }

        }

        /// <summary>
        /// 获取日报数据
        /// </summary>
        /// <param name="day">日</param>
        /// <returns></returns>
        public object LoadDailyLogs(string day, User user)
        {
            var obj = new object();
            try
            {
                var DATE = DateTime.Parse(day).ToString("yyyy-MM-dd");

                var dailyQuery = $@"select p.ProjectId,p.ProjectName,p.ProjectType,p.ProjectStatus,p.ProjectStatusId, d.WorkingTime,d.Remark
                                    from [dbo].[ProjectInfo] p
                                    left join [dbo].[DailyRecord] d on p.ProjectId=d.ProjectId
							        where d.CreateUserId='{user.userid}' and d.CreateTime='{DATE}' and p.Validty=1 and p.ProjectStatusId<>6
                                    order by ProjectStatusId";
                var DailyReportData = db.Database.SqlQuery<MyProjects>(dailyQuery);
                if (DailyReportData.Any())
                    obj = DailyReportData.ToList().GroupBy(it => it.ProjectType);
                else
                {
                    MyProJectQuerySql = string.Format(MyProJectQuerySql, user.userid, DATE);
                    obj = db.Database.SqlQuery<MyProjects>(MyProJectQuerySql).ToList().GroupBy(it => it.ProjectType);
                }
                return obj;
            }
            catch (Exception ex)
            {
                PublicClass.log.Error("LoadDailyLogs方法获取日报日志出错:" + ex);
                obj = null;
                return obj;
            }

        }
        /// <summary>
        /// 更新日报
        /// </summary>
        /// <param name="day">日</param>
        /// <param name="proDailyLogs">日报数据</param>
        /// <returns></returns>
        public object UpdateDailyLogs(string day, string proDailyLogs, User user)
        {
            var obj = new object();
            try
            {
                var Date = DateTime.Parse(day);
                var dailyLog = JsonConvert.DeserializeObject<List<List<MyProjects>>>(proDailyLogs);
                var daily = from d in db.DailyRecord
                            where d.CreateTime == Date && d.CreateUserId == user.userid
                            select d;
                var time = DateTime.Now;
                db.DailyRecord.RemoveRange(daily.ToList());
                List<DailyRecord> dailyRecords = new List<DailyRecord>();
                foreach (var item in dailyLog)
                {
                    item.ForEach(d =>
                    {
                        DailyRecord dailyRecord = new DailyRecord();
                        dailyRecord.ProjectId = d.ProjectId;
                        dailyRecord.WorkingTime = d.WorkingTime;
                        dailyRecord.DepartmentID = user.departID;
                        dailyRecord.Validty = 1;
                        dailyRecord.Remark = d.Remark;
                        dailyRecord.Position = user.Position;
                        dailyRecord.CreateTime = Date;
                        dailyRecord.CreateName = user.username;
                        dailyRecord.CreateUserId = user.userid;
                        dailyRecord.ModifyTime = Date;
                        dailyRecord.ModifyUserId = user.userid;
                        dailyRecord.ModifyName = user.username;
                        dailyRecords.Add(dailyRecord);
                    });
                }
                db.DailyRecord.AddRange(dailyRecords);
                db.SaveChanges();
                obj = new
                {
                    msg = "更新成功",
                    success = true
                };
                return obj;
            }
            catch (Exception ex)
            {
                PublicClass.log.Error("UpdateDailyLogs方法更新日报日志出错:" + ex);
                obj = new { msg = "操作异常,请查看日志文件,联系管理员！", success = false };
                return obj;
            }

        }

        /// <summary>
        /// 获取项目成员日报详情
        /// </summary>
        /// <param name="projectID"></param>
        /// <param name="day"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public object GetProJectDetails(string projectID, string day, string name, string uid)
        {
            var obj = new object();
            try
            {
                if (string.IsNullOrEmpty(day))
                {
                    day = GetBenZhouDate();
                }
                var startTime = DateTime.Parse(day);
                var endTime = DateTime.Parse(day).AddDays(6);
                StringBuilder sqlQuery = new StringBuilder();
                sqlQuery.Append($@"select p.ProjectName,d.CreateName, d.WorkingTime,d.CreateTime from [dbo].[ProjectInfo] p
                              left join [dbo].[DailyRecord] d 
                              on p.ProjectId=d.ProjectId
                              where p.Validty=1 and p.ProjectId='{projectID}' and d.Position like '%工程师%' and d.CreateTime>='{startTime}' and d.CreateTime<='{endTime}'");
                if (!string.IsNullOrEmpty(uid))
                {
                    sqlQuery.Append($" and d.CreateUserId='{uid}'");
                }
                if (!string.IsNullOrEmpty(name))
                {
                    sqlQuery.Append($" and d.CreateName like '%{name}%'");
                }
                sqlQuery.Append("order by d.CreateTime");
                var query = db.Database.SqlQuery<dailyDetail>(sqlQuery.ToString());
                if (!query.Any())
                {
                    obj = new
                    {
                        msg = "无相关数据",
                        success = false
                    };
                    return obj;
                }
                var ProName = query.FirstOrDefault().ProjectName;
                var details = query.ToList().GroupBy(it => it.CreateTime);
                obj = new
                {
                    ProjectName = ProName,
                    Array = details,
                    success = true
                };
                return obj;
            }
            catch (Exception ex)
            {
                PublicClass.log.Error("GetProJectDetails方法获取项目日报详情出错:" + ex);
                obj = new { msg = "操作异常,请查看日志文件,联系管理员！", success = false };
                return obj;
            }

        }
        /// <summary>
        /// 获取项目小组成员
        /// </summary>
        /// <param name="ProID"></param>
        /// <returns></returns>
        public object GetMemberByProjectID(string ProID, User user)
        {
            var obj = db.ProjectTeams.AsNoTracking().Where(it => it.ProjectId == ProID && it.Name != user.username && it.CreateUserId == user.userid).Select(it => new { it.UserId }).ToList();
            return obj;
        }
        /// <summary>
        /// 项目团队分配成员
        /// </summary>
        /// <param name="uid">操作人员ID</param>
        /// <param name="uname">操作人姓名</param>
        /// <param name="ProJectID">项目ID</param>
        /// <param name="members">项目成员</param>
        /// <returns></returns>
        public object ProTeamAddMembers(User user, string ProJectID, string members)
        {
            var obj = new object();
            try
            {
                var hasMemberQuery = db.ProjectTeams.Where(it => it.ProjectId == ProJectID && it.Name != user.username);
                if (hasMemberQuery.Any())
                {
                    db.ProjectTeams.RemoveRange(hasMemberQuery.ToList());
                }
                List<members> membersAdd = JsonConvert.DeserializeObject<List<members>>(members);
                var time = DateTime.Now;
                List<ProjectTeams> projectTeams = new List<ProjectTeams>();
                membersAdd.ForEach(m =>
                {
                    ProjectTeams team = new ProjectTeams();
                    team.ProjectId = ProJectID;
                    team.DepartmentID = m.DepartmentID;
                    team.UserId = m.userid;
                    team.Name = m.name;
                    team.Position = m.Position ?? "工程师";
                    team.IsPrincipal = m.isleader ?? "false";
                    team.Validty = 1;
                    team.CreateTime = time;
                    team.CreateName = user.username;
                    team.CreateUserId = user.userid;
                    team.ModifyName = user.username;
                    team.ModifyUserId = user.userid;
                    team.ModifyTime = time;
                    projectTeams.Add(team);
                });
                db.ProjectTeams.AddRange(projectTeams);
                db.SaveChanges();
                obj = new { msg = "分配成功", success = true };
                return obj;
            }
            catch (Exception ex)
            {
                PublicClass.log.Error("ProTeamAddMembers方法分配人员出错:" + ex);
                obj = new { msg = "操作异常,请查看日志文件,联系管理员！", success = false };
                return obj;
            }
        }

        /// <summary>
        /// 删除项目成员
        /// </summary>
        /// <param name="user"></param>
        /// <param name="ProJectID"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public object ProTeamDelMember(User user, string ProJectID, string userid)
        {
            try
            {
                var hasMember = db.ProjectTeams.Where(it => it.ProjectId == ProJectID && it.Name != user.username).ToList();
                var nowMember = hasMember.FindAll(IT => IT.UserId != userid);
                db.ProjectTeams.RemoveRange(hasMember);
                db.SaveChanges();
                db.ProjectTeams.AddRange(nowMember);
                db.SaveChanges();
                return GetProMemberAllTime(ProJectID, user);
            }
            catch (Exception ex)
            {
                PublicClass.log.Error("ProTeamDelMember删除项目成员出错:" + ex);
                var obj = new { msg = "操作异常,请查看日志文件,联系管理员！", success = false };
                return SerializeHelper.JsonSerialize(obj);
            }
        }
        /// <summary>
        /// 设置项目负责人
        /// </summary>
        /// <param name="proID"></param>
        /// <param name="form"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public object SetPrincipal(string proID, FormCollection form, User user)
        {
            var obj = new object();
            try
            {
                var userid = form["userID"].ToString();
                var IsPrincipal = form["IsPrincipal"].ToString();
                var chooseOne = db.ProjectTeams.Where(it => it.ProjectId == proID && it.UserId == userid && it.CreateUserId == user.userid).First();
                db.Entry(chooseOne).State = EntityState.Modified;
                chooseOne.IsPrincipal = IsPrincipal;
                chooseOne.ModifyTime = DateTime.Now;
                db.Entry(chooseOne).Property(it => it.ProjectId).IsModified = false;
                db.Entry(chooseOne).Property(it => it.UserId).IsModified = false;
                db.Entry(chooseOne).Property(it => it.Name).IsModified = false;
                db.Entry(chooseOne).Property(it => it.Position).IsModified = false;
                db.Entry(chooseOne).Property(it => it.Validty).IsModified = false;
                db.Entry(chooseOne).Property(it => it.CreateTime).IsModified = false;
                db.Entry(chooseOne).Property(it => it.CreateName).IsModified = false;
                db.Entry(chooseOne).Property(it => it.CreateUserId).IsModified = false;
                db.Entry(chooseOne).Property(it => it.ModifyName).IsModified = false;
                db.Entry(chooseOne).Property(it => it.ModifyUserId).IsModified = false;
                db.Entry(chooseOne).Property(it => it.DepartmentID).IsModified = false;
                db.SaveChanges();
                obj = new { msg = "操作成功", success = true };
                return obj;
            }
            catch (Exception ex)
            {
                PublicClass.log.Error("SetPrincipal设置为项目负责人出错:" + ex);
                obj = new { msg = "操作异常,请查看日志文件,联系管理员！", success = false };
                return obj;
            }

        }

        /// <summary>
        /// 获取项目成员一周日报详情
        /// </summary>
        /// <param name="projectID">项目ID</param>
        /// <returns></returns>
        public object GetProMemberAllTime(string projectID, User user)
        {
            var obj = new object();
            try
            {
                var IsPrincipalList = (from p in db.ProjectInfo.AsNoTracking()
                                       join t in db.ProjectTeams.AsNoTracking()
                                       on p.ProjectId equals t.ProjectId
                                       where p.ProjectId == projectID && t.UserId != user.userid && p.Validty == 1
                                       orderby t.Id
                                       select new
                                       {
                                           t.IsPrincipal,
                                           t.UserId
                                       }).ToList();
                var benzhou = GetBenZhouDate();
                var startTime = DateTime.Parse(benzhou);
                var endTime = startTime.AddDays(6);
                var query = $@"select d.CreateName AS name,d.CreateUserId AS userid,d.DepartmentID,CAST(sum(d.WorkingTime) AS decimal(18,2)) WorkingTime,d.Position
                               from [dbo].[DailyRecord] d
                               where d.ProjectId = '{projectID}' and d.CreateTime>='{startTime}' and d.CreateTime<='{endTime}'
                               and d.CreateUserId in(select p.UserId as CreateUserId from ProjectTeams p where p.ProjectId='{projectID}')
                               group by d.CreateName,d.CreateUserId,d.DepartmentID,d.Position
						       union all
						       select t.Name AS name,t.UserId AS userid,t.DepartmentID, 0 as WorkingTime,t.Position
						       from [dbo].[ProjectTeams]  t 
						       where t.ProjectId = '{projectID}'
						       and t.UserId  Not in(
						       select d.CreateUserId as UserId from [dbo].[DailyRecord] d
                               where d.ProjectId = '{projectID}' and d.CreateTime>='{startTime}' and d.CreateTime<='{endTime}'
                               group by d.CreateUserId) and t.UserId<>'{user.userid}'";
                var data = db.Database.SqlQuery<ProMember>(query).ToList();
                foreach (var item in IsPrincipalList)
                {
                    data.Find(d => d.userid == item.UserId).IsPrincipal = item.IsPrincipal;
                }
                obj = new
                {
                    msg = "查询成功",
                    list = data,
                    onlyCheck = user.OnlyCheck
                };
                return SerializeHelper.JsonSerialize(obj);
            }
            catch (Exception ex)
            {
                PublicClass.log.Error("GetProMemberAllTime获取项目成员列表出错:" + ex);
                obj = new { msg = "操作异常,请查看日志文件,联系管理员！", success = false };
                return SerializeHelper.JsonSerialize(obj);
            }

        }
        /// <summary>
        /// 周报详情添加
        /// </summary>
        /// <param name="uid">操作人ID</param>
        /// <param name="uname">操作人姓名</param>
        /// <param name="WeeklyDetailAdd">周报详情</param>
        /// <returns></returns>

        public object WeeklyDetailAdds(User user, string WeeklyDetailAdd, string proID)
        {
            lock (lockobj)
            {
                var WeeklyDetailObj = JsonConvert.DeserializeObject<WeeklyDetailS>(WeeklyDetailAdd);
                WeeklyDetailObj.ProjectId = proID;
                var benzhou = GetBenZhouDate();
                var time = DateTime.Parse(benzhou);
                var obj = new object();
                try
                {
                    var query = db.WeeklyDetail.AsNoTracking().Where(it => it.ProjectId == WeeklyDetailObj.ProjectId && it.CreateTime == time);
                    if (query.Any())
                    {
                        var existObj = query.FirstOrDefault();
                        db.Entry(existObj).State = EntityState.Modified;
                        db.Entry(existObj).Property(it => it.ProjectId).IsModified = false;
                        db.Entry(existObj).Property(it => it.NaturalWeek).IsModified = false;
                        db.Entry(existObj).Property(it => it.ProjectWeek).IsModified = false;
                        db.Entry(existObj).Property(it => it.Validty).IsModified = false;
                        db.Entry(existObj).Property(it => it.CreateTime).IsModified = false;
                        db.Entry(existObj).Property(it => it.CreateName).IsModified = false;
                        db.Entry(existObj).Property(it => it.CreateUserId).IsModified = false;
                        db.Entry(existObj).Property(it => it.Position).IsModified = false;
                        db.Entry(existObj).Property(it => it.ContractDays).IsModified = false;
                        db.Entry(existObj).Property(it => it.PersonnelUse).IsModified = false;
                        db.Entry(existObj).Property(it => it.TimeProgress).IsModified = false;
                        db.Entry(existObj).Property(it => it.ActualDays).IsModified = false;
                        existObj.ProjectStatus = WeeklyDetailObj.ProjectStatus;
                        existObj.ImplementationProgress = WeeklyDetailObj.ProjectProgress;
                        existObj.WeeklyContent = WeeklyDetailObj.WeeklyContent;
                        existObj.ModifyTime = DateTime.Now;
                        existObj.ProjectProgress = WeeklyDetailObj.ProjectProgress;
                        existObj.ModifyUserId = user.userid;
                        existObj.ModifyName = user.username;
                        existObj.NextWeekPlans = WeeklyDetailObj.NextWeekPlans;
                        existObj.Differences = WeeklyDetailObj.Differences;
                        obj = new
                        {
                            msg = "更新成功",
                            success = true
                        };
                    }
                    else
                    {
                        WeeklyDetail detail = new WeeklyDetail();
                        detail.ProjectId = WeeklyDetailObj.ProjectId;
                        detail.ProjectStatus = WeeklyDetailObj.ProjectStatus;
                        detail.ProjectProgress = WeeklyDetailObj.ProjectProgress;
                        detail.ContractDays = WeeklyDetailObj.ContractDays;
                        detail.ActualDays = WeeklyDetailObj.ActualDays;
                        detail.TimeProgress = WeeklyDetailObj.TimeProgress;
                        detail.ImplementationProgress = WeeklyDetailObj.ProjectProgress;
                        detail.PersonnelUse = WeeklyDetailObj.PersonnelUse;
                        detail.WeeklyContent = WeeklyDetailObj.WeeklyContent;
                        detail.Position = WeeklyDetailObj.Position;
                        detail.NaturalWeek = WeeklyDetailObj.NaturalWeek;
                        detail.ProjectWeek = WeeklyDetailObj.ProjectWeek;
                        detail.Validty = 1;
                        detail.CreateTime = time;
                        detail.CreateUserId = user.userid;
                        detail.ModifyName = user.username;
                        detail.CreateName = user.username;
                        detail.ModifyTime = time;
                        detail.ModifyUserId = user.userid;
                        detail.NextWeekPlans = WeeklyDetailObj.NextWeekPlans;
                        detail.Differences = WeeklyDetailObj.Differences;
                        db.WeeklyDetail.Add(detail);
                        obj = new
                        {
                            msg = "添加成功",
                            success = true
                        };
                    }
                    db.SaveChanges();
                    return obj;
                }
                catch (Exception ex)
                {
                    PublicClass.log.Error("WeeklyDetailAdds添加周报详情出错:" + ex);
                    obj = new
                    {
                        msg = "操作异常,请查看日志文件,联系管理员！",
                        success = false
                    };
                    return obj;
                }
                finally
                {
                    var pro = db.ProjectInfo.Where(it => it.ProjectId == WeeklyDetailObj.ProjectId).FirstOrDefault();
                    db.Entry(pro).State = EntityState.Modified;
                    db.Entry(pro).Property(it => it.PMid).IsModified = false;
                    db.Entry(pro).Property(it => it.CustomerId).IsModified = false;
                    db.Entry(pro).Property(it => it.CustomerName).IsModified = false;
                    db.Entry(pro).Property(it => it.PMname).IsModified = false;
                    db.Entry(pro).Property(it => it.ContractNumber).IsModified = false;
                    db.Entry(pro).Property(it => it.CreateName).IsModified = false;
                    db.Entry(pro).Property(it => it.CreateTime).IsModified = false;
                    db.Entry(pro).Property(it => it.CreateUserId).IsModified = false;
                    db.Entry(pro).Property(it => it.EstimateEndTime).IsModified = false;
                    db.Entry(pro).Property(it => it.EstimateManDay).IsModified = false;
                    db.Entry(pro).Property(it => it.ProjectHealth).IsModified = false;
                    db.Entry(pro).Property(it => it.ProjectId).IsModified = false;
                    db.Entry(pro).Property(it => it.ProjectName).IsModified = false;
                    db.Entry(pro).Property(it => it.ProjectStatus).IsModified = false;
                    db.Entry(pro).Property(it => it.ProjectType).IsModified = false;
                    db.Entry(pro).Property(it => it.ProjectTypeId).IsModified = false;
                    db.Entry(pro).Property(it => it.SigningTime).IsModified = false;
                    db.Entry(pro).Property(it => it.StartTime).IsModified = false;
                    db.Entry(pro).Property(it => it.Validty).IsModified = false;
                    db.Entry(pro).Property(it => it.ModifyName).IsModified = false;
                    db.Entry(pro).Property(it => it.ModifyUserId).IsModified = false;
                    pro.ProjectProgress = WeeklyDetailObj.ProjectProgress;
                    pro.ModifyTime = time;
                    db.SaveChanges();
                }
            }
        }
        /// <summary>
        /// 是进入项目详情页还是日报页
        /// </summary>
        /// <param name="proID"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        public bool weekOrdaily(string proID, string uid)
        {

            var admin = db.Administrator.AsNoTracking().Where(it => it.UserID == uid).Any();
            //var admin = db.Administrator.AsNoTracking().Where(it => it.UserID == "markc").Any();
            var result = db.ProjectInfo.AsNoTracking().Where(it => it.ProjectId == proID && it.PMid == uid).Any();
            var Isprincipal = db.ProjectTeams.AsNoTracking().Where(it => it.UserId == uid && it.IsPrincipal.ToLower() == "true" && it.ProjectId == proID).Any();
            return result || admin || Isprincipal;
        }
        /// <summary>
        /// 导出报表并发送邮件
        /// </summary>
        /// <param name="EmailUrl">接收人邮箱</param>
        /// <param name="CustomerName">客户名称</param>
        /// <param name="ProjectName">项目名称</param>
        /// <param name="LoginUser">当前登录人</param>
        /// <returns></returns>
        public bool ExportAsync(string EmailUrl, string CustomerName, string ProjectName, string LoginUser)
        {
            ReportService service = new ReportService();
            service.ToEmail = EmailUrl;
            return service.Export(CustomerName, ProjectName, LoginUser);
        }
    }
}
