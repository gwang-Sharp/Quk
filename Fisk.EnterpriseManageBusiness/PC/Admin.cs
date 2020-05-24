using Fisk.EnterpriseManageDataAccess;
using Fisk.EnterpriseManageSolution.Models;
using Fisk.EnterpriseManageUtilities.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;

namespace Fisk.EnterpriseManageBusiness.PC
{

    public class Admin
    {
        EnterpriseManageDBEntities db = new EnterpriseManageDBEntities();
        /// <summary>
        /// 获取项目列表
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="size">页码大小</param>
        /// <returns></returns>
        public object GetProjectTable(int page = 1, int size = 10)
        {
            var obj = new object();
            try
            {
                int pageNum = page - 1;
                var query = from p in db.ProjectInfo.AsNoTracking()
                            where p.Validty == 1
                            orderby p.ProjectStatusId descending
                            select new
                            {
                                p.Id,
                                p.CustomerName,
                                p.CustomerId,
                                p.ProjectName,
                                p.ProjectId,
                                p.ContractNumber,
                                p.ContractDays,
                                p.PMdepartmentID,
                                p.SigningTime,
                                p.StartTime,
                                p.EstimateEndTime,
                                p.ActualEndTime,
                                p.PMname,
                                p.PMid,
                                p.ProjectStatusId,
                                p.ProjectStatus,
                                p.ProjectType,
                                p.ProjectTypeId,
                                WeekTimeSum = (from a1 in db.WeeklyRecord
                                               where a1.ProjectId == p.ProjectId
                                               select a1.WorkingTime
                                               ).Sum() ?? 0,
                                DailyTimeSum = (from d1 in db.DailyRecord
                                                where d1.ProjectId == p.ProjectId
                                                select d1.WorkingTime
                                               ).Sum() ?? 0,
                            };
                var count = query.Count();
                var objList = query.Skip(pageNum * size).Take(size).ToList().Select(
                    pro => new
                    {
                        pro.Id,
                        pro.CustomerName,
                        pro.CustomerId,
                        pro.ProjectName,
                        pro.ProjectId,
                        pro.ContractNumber,
                        pro.ContractDays,
                        pro.PMdepartmentID,
                        SigningTime = pro.SigningTime.Value.ToString("yyyy-MM-dd"),
                        StartTime = pro.StartTime.Value.ToString("yyyy-MM-dd"),
                        EstimateEndTime = pro.EstimateEndTime.Value.ToString("yyyy-MM-dd"),
                        ActualEndTime = string.IsNullOrEmpty(pro.ActualEndTime.ToString()) ? "" : pro.ActualEndTime.Value.ToString("yyyy-MM-dd"),
                        pro.PMname,
                        pro.PMid,
                        pro.ProjectStatusId,
                        pro.ProjectStatus,
                        pro.ProjectType,
                        pro.ProjectTypeId,
                        EstimateManDay = Math.Round((pro.WeekTimeSum + pro.DailyTimeSum), 2),
                        ActualManDay = Math.Round((pro.WeekTimeSum + pro.DailyTimeSum), 2)
                    }
                    )
                 ;
                obj = new
                {
                    msg = "查询成功",
                    list = objList,
                    Count = count,
                    success = true
                };
                return obj;
            }
            catch (Exception ex)
            {
                PublicClass.log.Error("执行GetProjectTable获取项目列表错处:" + ex);
                obj = new
                {
                    msg = "查看日志，联系管理员！！！",
                    Count = 0,
                    success = false
                };
                return obj;
            }

        }

        /// <summary>
        /// 获取客户列表
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="size">页码大小</param>
        /// <returns></returns>

        public object GetCustomerTable(int page = 1, int size = 10)
        {
            var obj = new object();
            try
            {
                int pageNum = page - 1;
                var query = from c in db.CustomerInfo.AsNoTracking()
                            where c.Validty == 1
                            orderby c.Id
                            select c;
                var count = query.Count();
                var objList = query.Skip(pageNum * size).Take(size).ToList().Select(c => new
                {
                    c.CustomerName,
                    c.Telephone,
                    c.TaxNumber,
                    c.CompanyAddress,
                    CreateTime = c.CreateTime.Value.ToString("yyyy-MM-dd"),
                    c.Contacts,
                    c.ContactTitle,
                    c.ContactPhone,
                    c.Id,
                    c.CustomerAbbreviation
                });
                obj = new
                {
                    msg = "查询成功",
                    list = objList,
                    Count = count,
                    success = true
                };
                return obj;
            }
            catch (Exception ex)
            {
                PublicClass.log.Error("执行GetCustomerTable获取客户列表错处:" + ex);
                obj = new
                {
                    msg = "查看日志，联系管理员！！！",
                    Count = 0,
                    success = false
                };
                return obj;
            }

        }
        /// <summary>
        /// 逻辑删除项目
        /// </summary>
        /// <param name="id">主键ID</param>
        /// <returns></returns>
        public object ProDeleted(int id)
        {
            var obj = new object();
            try
            {
                var proObj = db.ProjectInfo.Where(it => it.Id == id).FirstOrDefault();
                db.Entry(proObj).State = EntityState.Modified;
                proObj.Validty = 0;
                proObj.ModifyTime = DateTime.Now;
                db.Entry(proObj).Property(it => it.ProjectId).IsModified = false;
                db.Entry(proObj).Property(it => it.CustomerId).IsModified = false;
                db.Entry(proObj).Property(it => it.CustomerName).IsModified = false;
                db.Entry(proObj).Property(it => it.PMid).IsModified = false;
                db.Entry(proObj).Property(it => it.PMname).IsModified = false;
                db.Entry(proObj).Property(it => it.ProjectName).IsModified = false;
                db.Entry(proObj).Property(it => it.ProjectTypeId).IsModified = false;
                db.Entry(proObj).Property(it => it.ProjectType).IsModified = false;
                db.Entry(proObj).Property(it => it.ContractDays).IsModified = false;
                db.Entry(proObj).Property(it => it.ContractNumber).IsModified = false;
                db.Entry(proObj).Property(it => it.SigningTime).IsModified = false;
                db.Entry(proObj).Property(it => it.StartTime).IsModified = false;
                db.Entry(proObj).Property(it => it.EstimateEndTime).IsModified = false;
                db.Entry(proObj).Property(it => it.ProjectStatusId).IsModified = false;
                db.Entry(proObj).Property(it => it.ProjectStatus).IsModified = false;
                db.Entry(proObj).Property(it => it.EstimateManDay).IsModified = false;
                db.Entry(proObj).Property(it => it.ProjectProgress).IsModified = false;
                db.Entry(proObj).Property(it => it.ProjectHealth).IsModified = false;
                db.Entry(proObj).Property(it => it.CreateName).IsModified = false;
                db.Entry(proObj).Property(it => it.CreateTime).IsModified = false;
                db.Entry(proObj).Property(it => it.ModifyUserId).IsModified = false;
                db.Entry(proObj).Property(it => it.ModifyName).IsModified = false;
                db.SaveChanges();
                return obj = new
                {
                    msg = "删除成功",
                    success = true
                };
            }
            catch (Exception ex)
            {

                PublicClass.log.Error("执行ProDeleted删除项目出错：" + ex);
                obj = new
                {
                    msg = "删除异常，联系管理员！！！"
                };
                return obj;
            }

        }
        /// <summary>
        /// 逻辑删除客户
        /// </summary>
        /// <param name="id">主键ID</param>
        /// <returns></returns>
        public object CustomerDeleted(int id)
        {
            var obj = new object();
            try
            {
                var proObj = db.CustomerInfo.Where(it => it.Id == id).FirstOrDefault();
                db.Entry(proObj).State = EntityState.Modified;
                proObj.Validty = 0;
                proObj.ModifyTime = DateTime.Now;
                db.Entry(proObj).Property(it => it.CustomerId).IsModified = false;
                db.Entry(proObj).Property(it => it.CustomerName).IsModified = false;
                db.Entry(proObj).Property(it => it.CustomerAbbreviation).IsModified = false;
                db.Entry(proObj).Property(it => it.TaxNumber).IsModified = false;
                db.Entry(proObj).Property(it => it.CompanyAddress).IsModified = false;
                db.Entry(proObj).Property(it => it.Telephone).IsModified = false;
                db.Entry(proObj).Property(it => it.Contacts).IsModified = false;
                db.Entry(proObj).Property(it => it.ContactTitle).IsModified = false;
                db.Entry(proObj).Property(it => it.ContactPhone).IsModified = false;
                db.Entry(proObj).Property(it => it.CreateTime).IsModified = false;
                db.Entry(proObj).Property(it => it.CreateUserId).IsModified = false;
                db.Entry(proObj).Property(it => it.ModifyUserId).IsModified = false;
                db.Entry(proObj).Property(it => it.ModifyName).IsModified = false;
                db.SaveChanges();
                return obj = new
                {
                    msg = "删除成功",
                    success = true
                };
            }
            catch (Exception ex)
            {

                PublicClass.log.Error("执行ProDeleted删除项目出错：" + ex);
                obj = new
                {
                    msg = "删除异常，联系管理员！！！"
                };
                return obj;
            }

        }
        /// <summary>
        /// 创建项目
        /// </summary>
        /// <param name="project">项目</param>
        /// <returns></returns>
        public object ProCreate(string project)
        {
            var obj = new object();
            var pro = JsonConvert.DeserializeObject<ProjectInfo>(project);
            var time = DateTime.Now;
            try
            {
                pro.ProjectId = Guid.NewGuid().ToString();
                pro.EstimateManDay = pro.ContractDays;
                pro.Validty = 1;
                pro.CreateTime = time;
                pro.CreateUserId = "system";
                pro.CreateName = "system";
                pro.ModifyName = "system";
                pro.ModifyTime = time;
                pro.ModifyUserId = "system";
                pro.PMid = pro.PMid;
                pro.PMname = pro.PMname;
                pro.CustomerId = pro.CustomerId;
                pro.ProjectProgress = "0";
                pro.ProjectHealth = "健康";
                pro.ActualManDay = 0;
                db.ProjectInfo.Add(pro);
                db.SaveChanges();
                obj = new
                {
                    msg = "创建成功",
                    success = true
                };
                return obj;
            }
            catch (Exception ex)
            {

                PublicClass.log.Error("执行ProCreate创建项目出错：" + ex);
                obj = new
                {
                    msg = "操作异常，联系管理员!",
                    success = false
                };
                return obj;
            }
            finally
            {
                ProjectTeams teams = new ProjectTeams();
                teams.ProjectId = pro.ProjectId;
                teams.DepartmentID = pro.PMdepartmentID.Value;
                teams.UserId = pro.PMid;
                teams.Name = pro.PMname;
                teams.Position = "项目经理";
                teams.Validty = 1;
                teams.CreateTime = time;
                teams.IsPrincipal = "False";
                teams.CreateUserId = "system";
                teams.CreateName = "system";
                teams.ModifyTime = time;
                teams.ModifyName = "system";
                teams.ModifyUserId = "system";
                db.ProjectTeams.Add(teams);
                db.SaveChanges();
            }

        }
        /// <summary>
        /// 创建客户
        /// </summary>
        /// <param name="Customer">客户</param>
        /// <returns></returns>
        public object CustomerCreate(string Customer)
        {
            var obj = new object();
            try
            {
                var c = JsonConvert.DeserializeObject<CustomerInfo>(Customer);
                var time = DateTime.Now;
                c.Validty = 1;
                c.CreateTime = time;
                c.CreateUserId = "system";
                c.CreateName = "system";
                c.ModifyName = "system";
                c.ModifyTime = time;
                c.ModifyUserId = "system";
                c.CustomerId = Guid.NewGuid().ToString();
                db.CustomerInfo.Add(c);
                db.SaveChanges();
                obj = new
                {
                    msg = "创建成功",
                    success = true
                };
                return obj;
            }
            catch (Exception ex)
            {

                PublicClass.log.Error("执行CustomerCreate创建客户出错：" + ex);
                obj = new
                {
                    msg = "操作异常，联系管理员!",
                    success = false
                };
                return obj;
            }

        }
        /// <summary>
        /// 编辑客户
        /// </summary>
        /// <param name="Customer">客户</param>
        /// <returns></returns>
        public object UpdateCustomer(string Customer)
        {
            var obj = new object();
            try
            {
                var c = JsonConvert.DeserializeObject<CustomerInfo>(Customer);
                var existCustomer = db.CustomerInfo.Where(it => it.Id == c.Id).FirstOrDefault();
                db.Entry(existCustomer).State = EntityState.Modified;
                existCustomer.CustomerName = c.CustomerName;
                existCustomer.CustomerAbbreviation = c.CustomerAbbreviation;
                existCustomer.TaxNumber = c.TaxNumber;
                existCustomer.CompanyAddress = c.CompanyAddress;
                existCustomer.Telephone = c.Telephone;
                existCustomer.Contacts = c.Contacts;
                existCustomer.ContactTitle = c.ContactTitle;
                existCustomer.ContactPhone = c.ContactPhone;
                existCustomer.ModifyTime = DateTime.Now;
                db.Entry(existCustomer).Property(it => it.CreateName).IsModified = false;
                db.Entry(existCustomer).Property(it => it.CreateTime).IsModified = false;
                db.Entry(existCustomer).Property(it => it.CreateUserId).IsModified = false;
                db.Entry(existCustomer).Property(it => it.ModifyName).IsModified = false;
                db.Entry(existCustomer).Property(it => it.ModifyUserId).IsModified = false;
                db.Entry(existCustomer).Property(it => it.Validty).IsModified = false;
                db.Entry(existCustomer).Property(it => it.CustomerId).IsModified = false;
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
                PublicClass.log.Error("执行ProUpdate方法出错：" + ex);
                obj = new
                {
                    msg = "操作异常,联系管理员！"
                };
                return obj;
            }
        }
        /// <summary>
        /// 编辑项目
        /// </summary>
        /// <param name="project">项目</param>
        /// <returns></returns>
        public object ProUpdate(string project)
        {
            var obj = new object();
            try
            {
                var pro = JsonConvert.DeserializeObject<ProjectInfo>(project);
                var existPro = db.ProjectInfo.Where(it => it.Id == pro.Id).FirstOrDefault();
                var time = DateTime.Now;
                if (pro.PMid != existPro.PMid)
                {
                    var team = db.ProjectTeams.Where(it => it.ProjectId == existPro.ProjectId && it.UserId == existPro.PMid && it.Name == existPro.PMname).FirstOrDefault();
                    db.Entry(team).State = EntityState.Modified;
                    team.UserId = pro.PMid;
                    team.Name = pro.PMname;
                    team.ModifyTime = time;
                    team.DepartmentID = pro.PMdepartmentID.Value;
                    db.Entry(team).Property(it => it.ProjectId).IsModified = false;
                    db.Entry(team).Property(it => it.CreateName).IsModified = false;
                    db.Entry(team).Property(it => it.CreateTime).IsModified = false;
                    db.Entry(team).Property(it => it.CreateUserId).IsModified = false;
                    db.Entry(team).Property(it => it.ModifyName).IsModified = false;
                    db.Entry(team).Property(it => it.ModifyUserId).IsModified = false;
                    db.Entry(team).Property(it => it.Position).IsModified = false;
                    db.Entry(team).Property(it => it.Validty).IsModified = false;
                    db.SaveChanges();
                }
                db.Entry(existPro).State = EntityState.Modified;
                existPro.CustomerName = pro.CustomerName;
                existPro.ProjectName = pro.ProjectName;
                existPro.ProjectType = pro.ProjectType;
                existPro.ContractNumber = pro.ContractNumber;
                existPro.ContractDays = pro.ContractDays;
                existPro.SigningTime = pro.SigningTime;
                existPro.PMname = pro.PMname;
                existPro.PMid = pro.PMid;
                existPro.PMdepartmentID = pro.PMdepartmentID;
                existPro.StartTime = pro.StartTime;
                existPro.EstimateEndTime = pro.EstimateEndTime;
                existPro.ActualEndTime = pro.ActualEndTime;
                existPro.ProjectStatus = pro.ProjectStatus;
                existPro.ProjectStatusId = pro.ProjectStatusId;
                existPro.ProjectTypeId = pro.ProjectTypeId;
                existPro.ActualManDay = pro.ActualManDay;
                existPro.ModifyTime = time;
                db.Entry(existPro).Property(it => it.CreateName).IsModified = false;
                db.Entry(existPro).Property(it => it.EstimateManDay).IsModified = false;
                db.Entry(existPro).Property(it => it.ProjectId).IsModified = false;
                db.Entry(existPro).Property(it => it.CreateTime).IsModified = false;
                db.Entry(existPro).Property(it => it.CreateUserId).IsModified = false;
                db.Entry(existPro).Property(it => it.ModifyName).IsModified = false;
                db.Entry(existPro).Property(it => it.ModifyUserId).IsModified = false;
                db.Entry(existPro).Property(it => it.Validty).IsModified = false;
                db.Entry(existPro).Property(it => it.CustomerId).IsModified = false;
                db.Entry(existPro).Property(it => it.ProjectHealth).IsModified = false;
                db.Entry(existPro).Property(it => it.ProjectProgress).IsModified = false;
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
                PublicClass.log.Error("执行ProUpdate方法出错：" + ex);
                obj = new
                {
                    msg = "操作异常,联系管理员！"
                };
                return obj;
            }
            finally
            {

            }
        }
        /// <summary>
        /// 获取下拉框字典集
        /// </summary>
        /// <returns></returns>
        public object GetDicTionaries()
        {
            var obj = new object();
            try
            {
                var CustomerList = (from c in db.CustomerInfo.AsNoTracking()
                                    where c.Validty == 1
                                    select new
                                    {
                                        c.CustomerName,
                                        c.CustomerId
                                    }).ToList();
                var ProStatus = (from status in db.System_Dictionary.AsNoTracking()
                                 where status.Dic_Key == "ProjectStatus"
                                 select new
                                 {
                                     status.Dic_Type,
                                     status.Dic_TypeId
                                 }).ToList();
                var ProTypes = (from type in db.System_Dictionary.AsNoTracking()
                                where type.Dic_Key == "ProjectType"
                                select new
                                {
                                    type.Dic_Type,
                                    type.Dic_TypeId
                                }).ToList();
                obj = new
                {
                    msg = "查询成功",
                    CustomersList = CustomerList,
                    StatusDics = ProStatus,
                    TypeDics = ProTypes,
                    //PMlist = resultData,
                    success = true
                };
                return obj;
            }
            catch (Exception ex)
            {
                PublicClass.log.Error("执行GetDicTionaries获取下拉框字典出错：" + ex);
                obj = new
                {
                    success = false
                };
                return obj;
            }

        }
        /// <summary>
        /// 模糊查询
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="size">页码大小</param>
        /// <returns></returns>
        public object SeachC(int page = 1, int size = 10)
        {
            var obj = new object();
            try
            {
                int pageNum = page - 1;
                var query = db.CustomerInfo.AsNoTracking().AsQueryable();
                var request = HttpContext.Current.Request;
                if (!string.IsNullOrEmpty(request["CustomerName"]))
                {
                    var proName = request["CustomerName"].ToString();
                    query = query.Where(it => it.CustomerName.Contains(proName));
                }
                if (!string.IsNullOrEmpty(request["Telephone"]))
                {
                    var contractNum = int.Parse(request["Telephone"].ToString());
                    query = query.Where(it => it.Telephone == contractNum);
                }
                var count = query.Count();
                var objlist = query.OrderBy(it => it.Id).Skip(pageNum * size).Take(size).ToList().Select(c => new
                {
                    c.CustomerName,
                    c.Telephone,
                    c.TaxNumber,
                    c.CompanyAddress,
                    CreateTime = c.CreateTime.Value.ToString("yyyy-MM-dd"),
                    c.Contacts,
                    c.ContactTitle,
                    c.ContactPhone,
                    c.Id,
                    c.CustomerAbbreviation
                }); ;
                obj = new
                {
                    msg = "查询成功",
                    list = objlist,
                    Count = count,
                    success = true
                };
                return obj;
            }
            catch (Exception ex)
            {
                PublicClass.log.Error("执行Seach模糊查询出错：" + ex);
                obj = new
                {
                    msg = "查询异常，联系管理员！",
                    success = false
                };
                return obj;
            }
        }

        /// <summary>
        /// 模糊查询
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="size">页码大小</param>
        /// <returns></returns>
        public object Seach(int page = 1, int size = 10)
        {
            var obj = new object();
            try
            {
                int pageNum = page - 1;
                var query = db.ProjectInfo.AsNoTracking().AsQueryable();
                var request = HttpContext.Current.Request;
                if (!string.IsNullOrEmpty(request["ProjectName"]))
                {
                    var proName = request["ProjectName"].ToString();
                    query = query.Where(it => it.ProjectName.Contains(proName));
                }
                if (!string.IsNullOrEmpty(request["ContractNumber"]))
                {
                    var contractNum = request["ContractNumber"].ToString();
                    query = query.Where(it => it.ContractNumber == contractNum);
                }
                if (!string.IsNullOrEmpty(request["CustomerName"]))
                {
                    var name = request["CustomerName"].ToString();
                    query = query.Where(it => it.CustomerName.Contains(name));
                }
                if (!string.IsNullOrEmpty(request["startTime"]) && !string.IsNullOrEmpty(request["startTime"]))
                {
                    var startTime = DateTime.Parse(request["startTime"].ToString());
                    var endTime = DateTime.Parse(request["startTime"].ToString());
                    query = query.Where(it => it.StartTime >= startTime && it.EstimateEndTime <= endTime);
                }
                if (!string.IsNullOrEmpty(request["PM"]))
                {
                    var pmName = request["PM"].ToString();
                    query = query.Where(it => it.PMname.Contains(pmName));
                }
                var count = query.Count();
                var objlist = query.OrderByDescending(it => it.ProjectStatusId).Skip(pageNum * size).Take(size).ToList().Select(
                    pro => new
                    {
                        pro.Id,
                        pro.CustomerName,
                        pro.CustomerId,
                        pro.ProjectName,
                        pro.ProjectId,
                        pro.ContractNumber,
                        pro.ContractDays,
                        pro.PMdepartmentID,
                        SigningTime = pro.SigningTime.Value.ToString("yyyy-MM-dd HH:ss:mm"),
                        StartTime = pro.StartTime.Value.ToString("yyyy-MM-dd HH:ss:mm"),
                        EstimateEndTime = pro.EstimateEndTime.Value.ToString("yyyy-MM-dd HH:ss:mm"),
                        pro.PMname,
                        pro.PMid,
                        pro.ProjectStatusId,
                        pro.ProjectStatus,
                        pro.ProjectType,
                        pro.ProjectTypeId,
                        pro.ActualManDay
                    }
                    )
                 ;
                obj = new
                {
                    msg = "查询成功",
                    list = objlist,
                    Count = count,
                    success = true
                };
                return obj;
            }
            catch (Exception ex)
            {
                PublicClass.log.Error("执行Seach模糊查询出错：" + ex);
                obj = new
                {
                    msg = "查询异常，联系管理员！",
                    success = false
                };
                return obj;
            }
        }
        /// <summary>
        /// 递归查询子菜单
        /// </summary>
        /// <param name="parentID"></param>
        /// <returns></returns>
        public object getChildMenus(int parentID)
        {
            var obj = new object();
            try
            {
                var query = from m in db.NavMenus.AsNoTracking()
                            where m.Validty == 1 && m.ParentID == parentID
                            select m;
                if (query.Any())
                {
                    obj = query.ToList().Select(m => new
                    {
                        Id = m.Id.ToString(),
                        m.MenuTitle,
                        m.MenuUrl,
                        m.MenuIcon,
                        m.ParentID,
                        ChildrenMenu = getChildMenus(m.Id)
                    });
                }
                else
                {
                    obj = null;
                }
                return obj;
            }
            catch (Exception ex)
            {
                PublicClass.log.Error("执行getChildMenus递归查询子菜单出错：" + ex);
                obj = null;
                return obj;
            }
        }

        /// <summary>
        /// 获取导航跟菜单
        /// </summary>
        /// <returns></returns>
        public object getMenus()
        {
            var obj = new object();
            try
            {
                var menus = (from n in db.NavMenus.AsNoTracking()
                             where n.Validty == 1 && n.ParentID == 0
                             select n).ToList().Select(n => new
                             {
                                 Id = n.Id.ToString(),
                                 n.MenuTitle,
                                 n.MenuUrl,
                                 n.MenuIcon,
                                 n.ParentID,
                                 ChildrenMenu = getChildMenus(n.Id)
                             });
                obj = new
                {
                    msg = "查询成功",
                    nav = menus
                };
                return obj;
            }
            catch (Exception ex)
            {
                PublicClass.log.Error("执行getMenus获取导航菜单出错：" + ex);
                obj = new
                {
                    msg = "查询菜单导航异常，联系管理员！",
                    success = false
                };
                return obj;
            }
        }

        /// <summary>
        /// 获取项目详情
        /// </summary>
        /// <param name="proID"></param>
        /// <returns></returns>
        public object getDetailInfos(string proID)
        {
            var obj = new object();
            try
            {
                var BasicData = (from p in db.ProjectInfo.AsNoTracking()
                                 join w in db.WeeklyDetail.AsNoTracking()
                                 on p.ProjectId equals w.ProjectId
                                 where p.Validty == 1 && p.ProjectId == proID
                                 select new
                                 {
                                     p.EstimateManDay,
                                     p.ProjectProgress,
                                     p.ProjectHealth,
                                     w.TimeProgress,
                                     w.ImplementationProgress,
                                     w.PersonnelUse,
                                     w.Id,
                                     WeekTimeSum = (from a1 in db.WeeklyRecord
                                                    where a1.ProjectId == p.ProjectId
                                                    select a1.WorkingTime
                                               ).Sum() ?? 0,
                                     DailyTimeSum = (from d1 in db.DailyRecord
                                                     where d1.ProjectId == p.ProjectId
                                                     select d1.WorkingTime
                                               ).Sum() ?? 0,
                                 } into pro
                                 select pro).ToList().Select(D => new
                                 {
                                     D.EstimateManDay,
                                     ActualDays = Math.Round((D.WeekTimeSum + D.DailyTimeSum), 2),
                                     ProjectProgress = decimal.Parse(D.ProjectProgress ?? "0"),
                                     D.ProjectHealth,
                                     TimeProgress = decimal.Parse(D.TimeProgress ?? "0"),
                                     ImplementationProgress = decimal.Parse(D.ImplementationProgress ?? "0"),
                                     PersonnelUse = decimal.Parse(D.PersonnelUse ?? "0"),
                                     PersonnelUseData = decimal.Parse(D.PersonnelUse ?? "0") > 100 ? 100 : decimal.Parse(D.PersonnelUse ?? "0"),
                                     D.Id
                                 });
                var basicOne = new object();
                if (BasicData.LastOrDefault() == null)
                {
                    basicOne = BasicData.OrderByDescending(it => it.Id).FirstOrDefault();
                }
                var weeklyData = (from p in db.ProjectInfo.AsNoTracking()
                                  join w in db.WeeklyDetail.AsNoTracking()
                                  on p.ProjectId equals w.ProjectId
                                  where p.Validty == 1 && p.ProjectId == proID
                                  select new
                                  {
                                      p.PMname,
                                      w.WeeklyContent,
                                      WorkTime = (from weekrepot in db.WeeklyRecord
                                                  where weekrepot.Validty == 1 && weekrepot.ProjectId == proID
                                                  select weekrepot.WorkingTime).FirstOrDefault(),
                                      w.CreateTime
                                  } into week
                                  select week).ToList().Select(a => new
                                  {
                                      a.PMname,
                                      a.WeeklyContent,
                                      a.WorkTime,
                                      CreateTime = a.CreateTime.Value.ToString("yyyy-MM-dd")
                                  });
                var membersQuery = $@"select p.Name,cast(sum(d.WorkingTime) as decimal(18,2)) WorkingTime ,max(d.CreateTime) CreateTime from [dbo].[ProjectTeams] p left join [dbo].[DailyRecord] d
                                     on p.ProjectId = d.ProjectId where p.Validty = 1 and p.ProjectId = '{proID}'
                                     group by p.Name";
                var members = db.Database.SqlQuery<proMember>(membersQuery).ToList();
                obj = new
                {
                    msg = "查询成功",
                    detail = BasicData.LastOrDefault() ?? basicOne,
                    weekReport = weeklyData,
                    proMembers = members
                };
                return obj;
            }
            catch (Exception ex)
            {

                PublicClass.log.Error("执行getDetailInfos获取项目详情出错：" + ex);
                obj = new
                {
                    msg = "获取项目详情异常，联系管理员！",
                    success = false
                };
                return obj;
            }
        }
        private class proMember
        {
            public string Name { get; set; }
            public decimal? WorkingTime { get; set; } = 0;

            public string createTime;
            public DateTime? CreateTime
            {
                get => DateTime.Parse(createTime);
                set => createTime = value == null ? DateTime.Now.ToString("yyyy-MM-dd") : value.Value.ToString("yyyy-MM-dd");
            }
        }

        /// <summary>
        /// 获取部门信息
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public object getDeparts(string token)
        {
            var url = "https://qyapi.weixin.qq.com/cgi-bin/department/list";
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("access_token", token);
            var result = JObject.Parse(HttpUtils.DoGet(url, dic));
            var DepartValue = result["department"].ToString();
            var depArray = JArray.Parse(DepartValue).ToString();
            return depArray;
        }


        /// <summary>
        /// 获取部门成员详情信息
        /// </summary>
        /// <param name="token"></param>
        /// <param name="departID"></param>
        /// <returns></returns>
        public object getDepartMembers(string token, string departID)
        {
            string url = "https://qyapi.weixin.qq.com/cgi-bin/user/list";
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("access_token", token);
            dic.Add("department_id", departID);
            dic.Add("fetch_child", "1");
            var result = HttpUtils.DoGet(url, dic);
            var allmembers = JObject.Parse(result)["userlist"];
            JArray array = JArray.FromObject(allmembers);//接口返回的成员列表
            return array.ToString();
        }
        /// <summary>
        /// byte[]转换文件
        /// </summary>
        /// <param name="path">文件path</param>
        /// <returns></returns>
        public static byte[] GetFileBytes(string path)
        {
            using (FileStream file = new FileStream(path, FileMode.Open))
            {
                byte[] ms = new byte[file.Length];
                file.Read(ms, 0, Convert.ToInt32(file.Length));
                file.Close();
                return ms;
            }
        }

    }
}
