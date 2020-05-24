using System.Collections.Generic;
using System.Data;

/*******************************************************************
 * * 版权所有：菲斯克（上海）软件有限公司
 * * 功    能： 操作DataTable的扩展方法
 * * 作    者：jerryli
 * * 电子邮箱：jerryl@runmont.com
 * * 创建日期： 2012/6/19 20:49:49
 * *******************************************************************/

namespace Fisk.EnterpriseManageUtilities.WebPage
{
    /// <summary>
    /// DataTable类的帮助类
    /// </summary>
    public class DataTableHelper
    {
        /// <summary>
        /// datatable增加自增列
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static DataTable AddAutoIdColumn(DataTable dt, string columnName)
        {

            if (dt != null)
            {

                DataColumn autoColumn = new DataColumn(columnName, System.Type.GetType("System.Int32"));

                dt.Columns.Add(autoColumn);

                dt.Columns[columnName].SetOrdinal(0);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    dt.Rows[i][0] = i + 1;
                }

            }

            return dt;

        }

        /// <summary>
        /// datatable删除增加自增列
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static DataTable DeleteAutoIdColumn(DataTable dt, string columnName)
        {

            if (dt != null)
            {

                dt.Columns.Remove(columnName);
                return dt;

            }

            return dt;

        }


        public static DataTable DeleteBlankRows(DataTable dt)
        {
            List<DataRow> removelist = new List<DataRow>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                bool rowdataisnull = true;
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    if (dt.Rows[i][j].ToString().Trim() != "")
                    {

                        rowdataisnull = false;
                    }

                }
                if (rowdataisnull)
                {
                    removelist.Add(dt.Rows[i]);
                }

            }
            for (int i = 0; i < removelist.Count; i++)
            {
                dt.Rows.Remove(removelist[i]);
            }
            return dt;
        }




        /// <summary>
        /// 根据Filter 过滤Datatable
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static DataTable GetNewTable(DataTable dt, string filter)
        {
            DataTable newTable = dt.Clone();
            DataRow[] drs = dt.Select(filter);
            foreach (DataRow dr in drs)
            {
                object[] arr = dr.ItemArray;
                DataRow newrow = newTable.NewRow();
                for (int i = 0; i < arr.Length; i++)
                    newrow[i] = arr[i];
                newTable.Rows.Add(newrow);
            }
            return newTable;
        }



    }
}
