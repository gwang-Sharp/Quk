using System;

namespace Fisk.EnterpriseManageViewModel
{
    /// <summary>
    /// 项目成员日报详情类
    /// </summary>
    public class dailyDetail
    {
        public string ProjectName { get; set; }
        public string CreateName { get; set; }
        public double WorkingTime { get; set; }
        private string _thatDay;
        public DateTime CreateTime
        {
            get => DateTime.Parse(_thatDay);
            set => _thatDay = value.ToString("yyyy-MM-dd");
        }
    }
}
