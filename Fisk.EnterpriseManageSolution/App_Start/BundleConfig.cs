using System;
using System.Web.Optimization;

namespace Fisk.EnterpriseManageSolution
{
    public class BundleConfig
    {
        // 有关捆绑的详细信息，请访问 https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            BundleTable.EnableOptimizations = false;
            //bundles.Add(new ScriptBundle("~/bundles/stuManage").Include("~/Content/Scripts/stuManage.js"));
            MoudleJS(bundles);
            MoudleCSS(bundles);
        }

        public static void MoudleJS(BundleCollection bundles)
        {
            //开发必须JS包                                                                            
            bundles.Add(new ScriptBundle("~/bundles/JSPackage").Include(
                     "~/Content/Plugins/jquery-3.3.1.js",
                     "~/Content/Plugins/jquery-3.3.1.min.js",
                     "~/Content/Plugins/vue/vue.js",
                     "~/Content/Plugins/element/index.js",
                     "~/Content/Plugins/mint-ui/index.js"
                ));

            //项目列表JS
            bundles.Add(new ScriptBundle("~/bundles/ProjectList").Include(
                             "~/Content/Script/ProjectWeekly/ProjectList.js"
                  ));
            //项目详情JS
            bundles.Add(new ScriptBundle("~/bundles/ProjectDetail").Include(
                             "~/Content/Script/ProjectWeekly/ProjectDetail.js"
                  ));
            //人天填写JS
            bundles.Add(new ScriptBundle("~/bundles/WeekReport").Include(
                             "~/Content/Script/WeekReport/WeekReport.js"
                  ));
            //工时录入JS
            bundles.Add(new ScriptBundle("~/bundles/DailyReport").Include(
                             "~/Content/Script/DailyReport/DailyReport.js"
                  ));
            //通讯录JS
            bundles.Add(new ScriptBundle("~/bundles/AddressBooks").Include(
                             "~/Content/Script/AddressBooks/AddressBooks.js"
                  ));
            //周天人汇总JS
            bundles.Add(new ScriptBundle("~/bundles/AllReportDetails").Include(
                 "~/Content/Script/AllReportDetails/AllReportDetails.js"
                  ));
            //PC端项目列表管理
            bundles.Add(new ScriptBundle("~/bundles/ManageIndex").Include(
                 "~/Content/Script/PC/ProjectList/Index.js"
                  ));
            //PC端项目列表管理
            bundles.Add(new ScriptBundle("~/bundles/CustomerList").Include(
                 "~/Content/Script/PC/CustomerList/CustomerList.js"
                  ));
            //PC端管理页面
            bundles.Add(new ScriptBundle("~/bundles/AdminManage").Include(
                 "~/Content/Script/PC/AdminManage/AdminManage.js"
                  ));
            bundles.Add(new ScriptBundle("~/bundles/ProjectDetailPC").Include(
                 "~/Content/Script/PC/ProjectList/ProjectDetail.js"
                  ));
        }

        public static void MoudleCSS(BundleCollection bundles)
        {
            //开发必须CSS包
            bundles.Add(new StyleBundle("~/bundles/CssPackage").Include(
                      "~/Content/Plugins/element/index.css",
                      "~/Content/Plugins/mint-ui/style.css",
                     "~/Content/Plugins/weui/weui.css",
                     "~/Content/Plugins/weui/ weui.min.css"
                      ));
        }


    }
}
