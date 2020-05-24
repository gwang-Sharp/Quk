using System;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI.WebControls;

namespace Fisk.EnterpriseManageUtilities.Common
{
    public class SqlBulkCopyHelper
    {
        //保存Excel数据到临时表
        public static bool SaveToStgTable(string excelServerPath, FileUpload fileuploadContrl, string sql, string sqlconnnectionString, string stgTable)
        {
            bool result = true;
            string savePath = excelServerPath;
            string filePath = string.Empty;  //上传的文件的路径
            if (fileuploadContrl.HasFile)//上传文件存在
            {
                if (!Directory.Exists(savePath))//文件夹不存在
                {
                    Directory.CreateDirectory(savePath);//创建文件夹
                }
                //  string fileName = fileExcel.FileName;//文件名字
                string fileName = Guid.NewGuid().CastToString() + fileuploadContrl.FileName.Substring(fileuploadContrl.FileName.LastIndexOf("."));
                filePath = savePath + @"\" + fileName;//文件路径
                if (File.Exists(filePath))//文件存在
                {
                    File.Delete(filePath);  //删除文件
                }
                try
                {
                    fileuploadContrl.SaveAs(filePath);//添加文件
                    string connectionString = "Provider=Microsoft.Ace.OLEDB.12.0;Data Source=" + filePath + ";Extended Properties='Excel 12.0;HDR=Yes;IMEX=1;'";
                    using (OleDbConnection connection = new OleDbConnection(connectionString))
                    {
                        connection.Open();
                        OleDbCommand command = new OleDbCommand();
                        command.CommandText = sql;
                        command.Connection = connection;
                        command.CommandText = sql;
                        command.CommandTimeout = 0;
                        OleDbDataReader reader = command.ExecuteReader();
                        using (SqlBulkCopy bulkCopy = new SqlBulkCopy(sqlconnnectionString))
                        {
                            bulkCopy.BulkCopyTimeout = 0;
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                bulkCopy.ColumnMappings.Add(reader.GetName(i), reader.GetName(i));
                            }
                            bulkCopy.DestinationTableName = stgTable;
                            bulkCopy.BulkCopyTimeout = 0;
                            bulkCopy.WriteToServer(reader);
                        }
                    }
                }
                catch (Exception ex)
                {
                    result = false;
                    Log.CreateLogManager().Error("SqlBulkCopyHelper.SaveToStgTable 执行失败！", ex);
                }
            }
            else
            {
                result = false;
            }
            return result;
        }
    }
}
