using Fisk.EnterpriseManageBusiness.Mobile;
using Fisk.EnterpriseManageBusiness.PC;
using Newtonsoft.Json;
using System.Web.Mvc;

namespace Fisk.EnterpriseManageSolution.Controllers.PC
{
    public class AdminController : Controller
    {
        private static Admin admin = new Admin();
        /// <summary>
        /// pc端管理页面
        /// </summary>
        /// <returns></returns>
        public ActionResult AdminManage()
        {
            return View();
        }
        /// <summary>
        /// 获取导航菜单数据
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetNavMenus()
        {
            return Json(admin.getMenus());
        }

        // GET: Admin
        /// <summary>
        /// 项目列表页
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            ViewBag.list = JsonConvert.SerializeObject(admin.GetProjectTable());
            return View();
        }
        /// <summary>
        /// 获取客户列表
        /// </summary>
        /// <returns></returns>
        public ActionResult CustomerList()
        {
            ViewBag.list = JsonConvert.SerializeObject(admin.GetCustomerTable());
            return View();
        }
        /// <summary>
        /// 项目详情页面
        /// </summary>
        public ActionResult ProjectDetail()
        {
            TempData["projectID"] = RouteData.Values["id"].ToString();
            TempData.Keep();
            return View();
        }
        /// <summary>
        /// 获取项目详情
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult getDetailInfo()
        {
            var projectID = TempData.Peek("projectID").ToString();
            return Json(admin.getDetailInfos(projectID));
        }
        /// <summary>
        /// 项目列表分页查询
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="size">页码大小</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult PageNation(int page, int size)
        {
            return Json(admin.GetProjectTable(page, size));
        }
        /// <summary>
        /// 客户列表分页查询
        /// </summary>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public ActionResult CPageNation(int page, int size)
        {
            return Json(admin.GetCustomerTable(page, size));
        }
        /// <summary>
        /// 逻辑删除项目
        /// </summary>
        /// <param name="proID">项目ID</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ProDelete(int proID)
        {
            return Json(admin.ProDeleted(proID));
        }
        /// <summary>
        /// 逻辑删除客户
        /// </summary>
        /// <param name="proID"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CustomerDelete(int proID)
        {
            return Json(admin.CustomerDeleted(proID));
        }
        /// <summary>
        /// 更新项目
        /// </summary>
        /// <param name="project">项目</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UpdateProject(string project)
        {
            return Json(admin.ProUpdate(project));
        }

        /// <summary>
        /// 更新客户
        /// </summary>
        /// <param name="project">项目</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UpdateCustomer(string Customer)
        {
            return Json(admin.UpdateCustomer(Customer));
        }

        /// <summary>
        /// 创建客户
        /// </summary>
        /// <param name="Customer">客户</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CreateCustomer(string Customer)
        {
            return Json(admin.CustomerCreate(Customer));
        }
        /// <summary>
        /// 创建项目
        /// </summary>
        /// <param name="project">项目</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CreateProject(string project)
        {
            return Json(admin.ProCreate(project));
        }
        /// <summary>
        /// 获取下拉框字典集
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetDicTionary()
        {
            return Json(admin.GetDicTionaries());
        }
        /// <summary>
        /// 列表页模糊查询
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Search()
        {
            int page = int.Parse(Request["page"] ?? "1");
            int size = int.Parse(Request["size"] ?? "10");
            return Json(admin.Seach(page, size));
        }

        /// <summary>
        /// 客户列表模糊查询
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SearchCustomer()
        {
            int page = int.Parse(Request["page"] ?? "1");
            int size = int.Parse(Request["size"] ?? "10");
            return Json(admin.SeachC(page, size));
        }
        /// <summary>
        /// 获取部门信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult getDepart()
        {
            Main main = new Main();
            var token = main.GetTonken().ToString();
            return Json(admin.getDeparts(token));
        }
        /// <summary>
        /// 获取部门成员详情
        /// </summary>
        /// <param name="departID"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult getDepartMember(string departID)
        {
            Main main = new Main();
            var token = main.GetTonken().ToString();
            return Json(admin.getDepartMembers(token,departID));
        }

        //public ActionResult reportData() {

        //}
    }
}