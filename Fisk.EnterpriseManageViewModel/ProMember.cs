using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fisk.EnterpriseManageViewModel
{
    /// <summary>
    /// 项目列表成员
    /// </summary>
    public class ProMember
    {
        public string name { get; set; }
        public string userid { get; set; }
        public int DepartmentID { get; set; }
        public decimal WorkingTime { get; set; }
        public string Position { get; set; }

        public bool Isprincipal = false;
        public string IsPrincipal
        {
            get => Isprincipal.ToString();
            set => Isprincipal = value.ToLower() == "false" ? false : true;
        }
    }
}
