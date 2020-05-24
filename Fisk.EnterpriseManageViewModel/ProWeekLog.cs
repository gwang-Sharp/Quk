using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fisk.EnterpriseManageViewModel
{
    public class ProWeekLog
    {
        /// <summary>
        /// 周报类
        /// </summary>
        public string ProjectId { get; set; }
        public string ProjectName { get; set; }
        public int WorkingTime { get; set; }
        public DateTime NowWeekDay { get; set; }
        public string ProjectStatusId { get; set; }
        public string ProjectType { get; set; }
        public string ProjectStatus { get; set; }
    }
}
