﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Fisk.EnterpriseManageDataAccess
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class EnterpriseManageDBEntities : DbContext
    {
        public EnterpriseManageDBEntities()
            : base("name=EnterpriseManageDBEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Administrator> Administrator { get; set; }
        public virtual DbSet<CustomerInfo> CustomerInfo { get; set; }
        public virtual DbSet<DailyRecord> DailyRecord { get; set; }
        public virtual DbSet<DepartmentList> DepartmentList { get; set; }
        public virtual DbSet<NavMenus> NavMenus { get; set; }
        public virtual DbSet<ProjectTeams> ProjectTeams { get; set; }
        public virtual DbSet<System_Dictionary> System_Dictionary { get; set; }
        public virtual DbSet<WeeklyDetail> WeeklyDetail { get; set; }
        public virtual DbSet<WeeklyRecord> WeeklyRecord { get; set; }
        public virtual DbSet<ProjectInfo> ProjectInfo { get; set; }
    }
}
