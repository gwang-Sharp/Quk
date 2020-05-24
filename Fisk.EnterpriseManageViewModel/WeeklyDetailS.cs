using System.ComponentModel.DataAnnotations;

namespace Fisk.EnterpriseManageViewModel
{
    public class WeeklyDetailS
    {
        [Display(Name = "项目ID")]
        public string ProjectId { get; set; }
        [Display(Name = "经理ID")]
        public string PMid { get; set; }
        [Display(Name = "项目经理")]
        public string PMname { get; set; }
        [Display(Name = "客户名称")]
        public string CustomerName { get; set; }
        [Display(Name = "客户编号")]
        public string CustomerId { get; set; }
        [Display(Name = "项目进度")]
        public string ProjectProgress { get; set; }
        [Display(Name = "项目状态")]
        public string ProjectStatus { get; set; }
        [Display(Name = "合同人天")]
        public string ContractDays { get; set; }
        [Display(Name = "实际人天")]
        public string ActualDays { get; set; }
        [Display(Name = "时间进度")]
        public string TimeProgress { get; set; }
        [Display(Name = "项目进度")]
        public string ImplementationProgress { get; set; }
        [Display(Name = "人天使用")]
        public string PersonnelUse { get; set; }
        [Display(Name = "周报内容")]
        public string WeeklyContent { get; set; }

        [Display(Name = "差异及分析")]
        public string Differences { get; set; }

        [Display(Name = "下周计划")]
        public string NextWeekPlans { get; set; }
        [Display(Name = "职位")]
        public string Position { get; set; } = "项目经理";
        [Display(Name = "自然周")]
        public string NaturalWeek { get; set; }
        [Display(Name = "项目周")]
        public string ProjectWeek { get; set; }
    }
}
