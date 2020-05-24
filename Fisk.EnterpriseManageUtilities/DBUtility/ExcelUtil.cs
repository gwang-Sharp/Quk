using System.Data.OleDb;

namespace GH.FreeBI.Component.DBUtility
{
    public class ExcelUtil
    {

        /// <summary>
        /// 根据Excel返回datatable
        /// </summary>
        /// <param name="Path">excel路径</param>
        /// <returns></returns>
        public static System.Data.DataTable GetDataFromExcelFirstSheet(string Path)
        {
            try
            {
                string strConn = string.Empty;
                strConn = "Provider=Microsoft.ACE.OLEDB.12.0;Extended Properties='Excel 12.0;HDR=YES;IMEX=1;';data source=" + Path;
                OleDbConnection conn = new OleDbConnection(strConn);
                conn.Open();
                //返回Excel的架构，包括各个sheet表的名称,类型，创建时间和修改时间等 
                System.Data.DataTable dtSheetName = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "Table" });

                //包含excel中表名的字符串数组 
                string[] strTableNames = new string[dtSheetName.Rows.Count];
                for (int k = 0; k < dtSheetName.Rows.Count; k++)
                {
                    strTableNames[k] = dtSheetName.Rows[k]["TABLE_NAME"].ToString();
                }

                OleDbDataAdapter myCommand = null;
                System.Data.DataTable dt = new System.Data.DataTable();
                string strExcel = "select * from [" + strTableNames[0] + "]";
                myCommand = new OleDbDataAdapter(strExcel, strConn);
                dt = new System.Data.DataTable();
                myCommand.Fill(dt);
                conn.Close();
                conn.Dispose();
                return dt;
            }
            catch
            {
                return null;
            }
        }

    }
}
