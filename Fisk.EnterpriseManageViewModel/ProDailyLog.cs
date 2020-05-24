using System;

namespace Fisk.EnterpriseManageViewModel
{
    public class ProDailyLog
    {
        /// <summary>
        /// 日报类
        /// </summary>
        public string ProjectId { get; set; }
        public string ProjectName { get; set; }
        public double WorkingTime { get; set; }
        public DateTime ThatDay { get; set; }
        public string ProjectStatusId { get; set; }
        public string ProjectType { get; set; }
        public string ProjectStatus { get; set; }

        public string Remark { get; set; }
    }
}
