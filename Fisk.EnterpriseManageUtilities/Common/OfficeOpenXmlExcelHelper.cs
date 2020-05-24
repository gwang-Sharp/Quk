using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Web.UI;
using OfficeOpenXml;
using System.Web.UI.WebControls;
using System.IO;
using System.Data.OleDb;
using Fisk.EnterpriseManageUtilities.WebPage;
using OfficeOpenXml.Style;
using System.Data.SqlClient;

namespace Fisk.EnterpriseManageUtilities.Common
{
    public class OfficeOpenXmlExcelHelper
    {
        #region 导出

        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="filename"></param>
        /// <param name="sheetName"></param>
        /// <param name="page"></param>
        /// <param name="keepType"></param>
        public static void Export(DataSet ds, string filename, Dictionary<string, System.Drawing.Color> listsStyle, Page page, bool keepType)
        {
            GC.Collect();
            using (ExcelPackage pck = new ExcelPackage())
            {
                if (ds != null && ds.Tables.Count > 0)
                {
                    for (int p = ds.Tables.Count - 1; p >= 0; p--)
                    {
                        DataTable dt = ds.Tables[p];
                        var ws = pck.Workbook.Worksheets.Add(ds.Tables[p].TableName);
                        pck.Workbook.Worksheets.MoveToEnd(1);
                        ws = pck.Workbook.Worksheets[ds.Tables[p].TableName];
                        GenerateData(dt, ws, keepType, listsStyle);
                    }
                }               
                FileInfo tempFile = new FileInfo(Path.GetTempFileName());
                pck.SaveAs(tempFile);
                pck.Dispose();
                GC.SuppressFinalize(pck);
                page.Response.Clear();
                page.Response.AddHeader("content-disposition", "attachment;  filename=" + filename);
                page.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                page.Response.WriteFile(tempFile.FullName, true);
                // page.Response.BinaryWrite(ep.GetAsByteArray());
                page.Response.Flush();
                page.Response.End();
                page.Response.Close();
                //string script = "window.open('/ExportCommon.aspx?tempFile=" + HttpUtility.UrlEncode(tempFile.FullName) + "&fileName=" + HttpUtility.UrlEncode(filename) + "', '_blank', 'top=10000,left=10000');";
                //ScriptManager.RegisterClientScriptBlock(page.Page, page.GetType(), Guid.NewGuid().CastToString(), script, true);
            }

        }



        public static void Export(DataSet ds, string filename,  Page page, bool keepType)
        {
            GC.Collect();
            using (ExcelPackage pck = new ExcelPackage())
            {
                if (ds != null && ds.Tables.Count > 0)
                {
                    for (int p = ds.Tables.Count - 1; p >= 0; p--)
                    {
                        DataTable dt = ds.Tables[p];
                        var ws = pck.Workbook.Worksheets.Add(ds.Tables[p].TableName);
                        pck.Workbook.Worksheets.MoveToEnd(1);
                        ws = pck.Workbook.Worksheets[ds.Tables[p].TableName];
                        GenerateData(dt, ws, keepType, null);
                    }
                }
                FileInfo tempFile = new FileInfo(Path.GetTempFileName());
                pck.SaveAs(tempFile);
                pck.Dispose();
                GC.SuppressFinalize(pck);
                page.Response.Clear();

                page.Response.AddHeader("content-disposition", "attachment;  filename=" + HttpUtility.UrlEncode(filename));
                page.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                // Page.Response.BinaryWrite(ep.GetAsByteArray());
                //   Response.WriteFile(ep.File.Name);
                page.Response.WriteFile(HttpUtility.HtmlDecode(tempFile.FullName), true);
                page.Response.Flush();
                if (File.Exists(tempFile.FullName))
                {
                    File.Delete(tempFile.FullName);
                }
                page.Response.End();
                page.Response.Close();

                //string script = "window.open('/ExportCommon.aspx?tempFile=" + HttpUtility.UrlEncode(tempFile.FullName) + "&fileName=" + HttpUtility.UrlEncode(filename) + "', '_blank', 'top=10000,left=10000');";
                //ScriptManager.RegisterClientScriptBlock(page.Page, page.GetType(), Guid.NewGuid().CastToString(), script, true);
            }

        }



        public static void Export(ExcelPackage pck,string filename, Page page)
        {
            GC.Collect();
           
                FileInfo tempFile = new FileInfo(Path.GetTempFileName());
                pck.SaveAs(tempFile);
                pck.Dispose();
                GC.SuppressFinalize(pck);
                //string script = "window.open('/ExportCommon.aspx?tempFile=" + HttpUtility.UrlEncode(tempFile.FullName) + "&fileName=" + HttpUtility.UrlEncode(filename) + "', '_blank', 'top=10000,left=10000');";
                //ScriptManager.RegisterClientScriptBlock(page.Page, page.GetType(), Guid.NewGuid().CastToString(), script, true);

                page.Response.Clear();
                page.Response.AddHeader("content-disposition", "attachment;  filename=" + filename);
                page.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                page.Response.WriteFile(tempFile.FullName, true);
                // page.Response.BinaryWrite(ep.GetAsByteArray());
                page.Response.Flush();
                page.Response.End();
                page.Response.Close();
        }

        private static void GenerateData(DataTable DT, ExcelWorksheet ws, bool keepType, Dictionary<string, System.Drawing.Color> listsStyle)
        {
            //create header
            string tmpText = "";
            for (int k = 0; k < DT.Columns.Count; k++)
            {
                ExcelRange theCell = ws.Cells[1, k + 1];
                tmpText = DT.Columns[k].ColumnName;
                theCell.Value = tmpText;
                if (listsStyle != null && listsStyle.ContainsKey(DT.Columns[k].ColumnName))
                {
                    theCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    theCell.Style.Fill.BackgroundColor.SetColor(listsStyle[DT.Columns[k].ColumnName]);
                }
            }
            //create data 
            for (int i = 0; i < DT.Rows.Count; i++)
            {
                for (int j = 0; j < DT.Columns.Count; j++)
                {
                    ExcelRange theCell = ws.Cells[2 + i, j + 1];
                    if (keepType) //保留格式
                    {
                        theCell.Value = DT.Rows[i][j];
                    }
                    else //不保留以字符串输出
                    {
                        tmpText = DT.Rows[i][j].CastToString();
                        theCell.Value = tmpText;
                    }
                    if (listsStyle != null && listsStyle.ContainsKey(DT.Columns[j].ColumnName))
                    {
                        theCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        theCell.Style.Fill.BackgroundColor.SetColor(listsStyle[DT.Columns[j].ColumnName]);
                    }

                }
            }
        }

        /// <summary>
        ///导出
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="filename"></param>
        /// <param name="sheetName"></param>
        /// <param name="page"></param>
        /// <param name="keepType"></param>
        public static void ExportToCurrentPage(SqlDataReader reader, string filename, string sheetName, Page page, bool keepType)
        {
            var tempFile = new FileInfo(Path.GetTempFileName());
            try
            {
                var pck = new ExcelPackage();
                var ws = pck.Workbook.Worksheets.Add(sheetName);
                pck.SaveAs(tempFile);
                pck.Dispose();
                using (var stream = tempFile.OpenRead())
                {
                    pck.Load(stream);
                }
                pck.Workbook.Worksheets.MoveToEnd(1);
                ws = pck.Workbook.Worksheets[sheetName];
                //列名
                if (reader.FieldCount > 0)
                {
                    ExcelRange theCell;
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        theCell = ws.Cells[1, i + 1];
                        theCell.Value = reader.GetName(i);
                    }
                }
                int j = 0;
                while (reader.Read())
                {
                    if (reader.FieldCount > 0)
                    {
                        ExcelRange theCellData;
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            theCellData = ws.Cells[j + 2, i + 1];
                            if (keepType)
                            {
                                theCellData.Value = reader[i].CastToString();
                            }
                            else
                            {
                                theCellData.Value = reader[i];
                            }
                        }
                    }
                    j++;
                }
                reader.Close();
                reader.Dispose();
                pck.Save();
                pck.Dispose();
                page.Response.Clear();
                page.Response.AddHeader("content-disposition", "attachment;  filename=" + filename);
                page.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                page.Response.WriteFile(tempFile.FullName, true);
                // page.Response.BinaryWrite(ep.GetAsByteArray());
                page.Response.Flush();
                page.Response.End();
                page.Response.Close();
            }
            finally
            {
                try { tempFile.Delete(); }
                catch { }
            }
        }

        ///导出
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="filename"></param>
        /// <param name="sheetName"></param>
        /// <param name="page"></param>
        /// <param name="keepType"></param>
        public static void Export(SqlDataReader reader, string filename, string sheetName, Page page, bool keepType)
        {
            GC.Collect();
            using( ExcelPackage pck = new ExcelPackage())
            {
               // var pck = new ExcelPackage();
                var ws = pck.Workbook.Worksheets.Add(sheetName);
                //pck.SaveAs(tempFile);
                //pck.Dispose();
                //using (var stream = tempFile.OpenRead())
                //{
                //    pck.Load(stream);
                //}
                
                pck.Workbook.Worksheets.MoveToEnd(1);
                ws = pck.Workbook.Worksheets[sheetName];
                //列名
                if (reader.FieldCount > 0)
                {
                    ExcelRange theCell;
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        theCell = ws.Cells[1, i + 1];
                        theCell.Value = reader.GetName(i);
                    }
                }
                int j = 0;
                while (reader.Read())
                {
                    if (reader.FieldCount > 0)
                    {
                        ExcelRange theCellData;
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            theCellData = ws.Cells[j + 2, i + 1];
                          
                            if (!keepType)
                            {
                                theCellData.Value = reader[i].CastToString();
                            }
                            else
                            {
                                theCellData.Value = reader[i];
                            }
                           
                          
                        }
                    }
                    j++;
                }
                reader.Close();
                reader.Dispose();
                FileInfo tempFile = new FileInfo(Path.GetTempFileName());
                pck.SaveAs(tempFile);
                pck.Dispose();
                GC.SuppressFinalize(pck);
                page.Response.Clear();
                page.Response.AddHeader("content-disposition", "attachment;  filename=" + filename);
                page.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                page.Response.WriteFile(tempFile.FullName, true);
                // page.Response.BinaryWrite(ep.GetAsByteArray());
                page.Response.Flush();
                page.Response.End();
                page.Response.Close();
                //string script = "window.open('/ExportCommon.aspx?tempFile=" + HttpUtility.UrlEncode(tempFile.FullName) + "&fileName=" + HttpUtility.UrlEncode(filename) + "', '_blank', 'top=10000,left=10000');";
                //ScriptManager.RegisterClientScriptBlock(page.Page, page.GetType(), Guid.NewGuid().CastToString(), script, true);
            }
         
        }
        /// <summary>
        /// 导出多sheet的
        /// </summary>
        /// <param name="readers"></param>
        /// <param name="fileanme"></param>
        /// <param name="sheetNames"></param>
        /// <param name="page"></param>
        /// <param name="keepType"></param>
        public static void Export(SqlDataReader[] reader, string fileanme, string[] sheetName, Page page, Dictionary<string, System.Drawing.Color> listsStyle, bool keepType)
        {
            GC.Collect();
            using (ExcelPackage pck = new ExcelPackage())
            {
                // var pck = new ExcelPackage();
                if (reader != null && reader.Length > 0)
                {
                    for (int p = reader.Length-1; p >=0;p-- )
                    {
                        var ws = pck.Workbook.Worksheets.Add(sheetName[p]);
                        pck.Workbook.Worksheets.MoveToEnd(1);
                        ws = pck.Workbook.Worksheets[sheetName[p]];
                        //列名
                        if (reader[p].FieldCount > 0)
                        {
                            ExcelRange theCell;
                            for (int i = 0; i < reader[p].FieldCount; i++)
                            {
                                theCell = ws.Cells[1, i + 1];
                                theCell.Value = reader[p].GetName(i);
                                if (listsStyle != null && listsStyle.ContainsKey(reader[p].GetName(i)))
                                {
                                    theCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    theCell.Style.Fill.BackgroundColor.SetColor(listsStyle[reader[p].GetName(i)]);
                                }
                            }
                        }
                        int j = 0;
                        while (reader[p].Read())
                        {
                            if (reader[p].FieldCount > 0)
                            {
                                ExcelRange theCellData;
                                for (int i = 0; i < reader[p].FieldCount; i++)
                                {                                   
                                    theCellData = ws.Cells[j + 2, i + 1];
                                    if (!keepType)
                                    {
                                        theCellData.Value = reader[p][i].CastToString();
                                    }
                                    else
                                    {
                                        theCellData.Value = reader[p][i];
                                    }
                                    if (listsStyle != null && listsStyle.ContainsKey(reader[p].GetName(i)))
                                    {
                                        theCellData.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                        theCellData.Style.Fill.BackgroundColor.SetColor(listsStyle[reader[p].GetName(i)]);
                                    }
                                }
                            }
                            j++;
                        }
                        reader[p].Close();
                        reader[p].Dispose();
                    }
                }
               
                FileInfo tempFile = new FileInfo(Path.GetTempFileName());
                pck.SaveAs(tempFile);
                pck.Dispose();
                GC.SuppressFinalize(pck);
                page.Response.Clear();
                page.Response.AddHeader("content-disposition", "attachment;  filename=" + fileanme);
                page.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                page.Response.WriteFile(tempFile.FullName, true);
                // page.Response.BinaryWrite(ep.GetAsByteArray());
                page.Response.Flush();
                page.Response.End();
                page.Response.Close();
                //string script = "window.open('/ExportCommon.aspx?tempFile=" + HttpUtility.UrlEncode(tempFile.FullName) + "&fileName=" + HttpUtility.UrlEncode(fileanme) + "', '_blank', 'top=10000,left=10000');";
                //ScriptManager.RegisterClientScriptBlock(page.Page, page.GetType(), Guid.NewGuid().CastToString(), script, true);
            }
        }


        public static void ExportToThisPage(SqlDataReader[] reader, string fileanme, string[] sheetName,object[]filedView, Page page, Dictionary<string, System.Drawing.Color> listsStyle, bool keepType)
        {
            GC.Collect();
            using (ExcelPackage pck = new ExcelPackage())
            {
                // var pck = new ExcelPackage();
                if (reader != null && reader.Length > 0)
                {
                    for (int p = reader.Length - 1; p >= 0; p--)
                    {
                        var ws = pck.Workbook.Worksheets.Add(sheetName[p]);
                        pck.Workbook.Worksheets.MoveToEnd(1);
                        ws = pck.Workbook.Worksheets[sheetName[p]];
                        Dictionary<string, string> dic = filedView[p] as Dictionary<string, string>;
                        //列名
                        if (filedView!=null)
                        {
                            ExcelRange theCell;
                      
                            int i = 0;
                            foreach (KeyValuePair<string, string > t in dic)
                            {                   
                                theCell = ws.Cells[1, i + 1];
                                theCell.Value = t.Value;
                                i++;
                                //Console.WriteLine("{0} = {1}", thing.Key, thing.Value);
                            }
                        }
                      
                        int j = 0;
                        while (reader[p].Read())
                        {
                            if (reader[p].FieldCount > 0)
                            {
                                ExcelRange theCellData;
                                int k = 0;
                                foreach (KeyValuePair<string, string> t in dic)
                                {
                                    theCellData = ws.Cells[j + 2, k + 1];

                                    theCellData.Value = reader[p][ t.Key];
                                    k++;
                                }
                            }
                            j++;
                        }
                        reader[p].Close();
                        reader[p].Dispose();
                    }
                }

                FileInfo tempFile = new FileInfo(Path.GetTempFileName());
                pck.SaveAs(tempFile);
                pck.Dispose();
                GC.SuppressFinalize(pck);

                page.Response.Clear();
               
                page.Response.AddHeader("content-disposition", "attachment;  filename=" +HttpUtility.UrlEncode( fileanme));
                page.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                // Page.Response.BinaryWrite(ep.GetAsByteArray());
                //   Response.WriteFile(ep.File.Name);
                page.Response.WriteFile(HttpUtility.HtmlDecode(tempFile.FullName), true);
                page.Response.Flush();
                if (File.Exists(tempFile.FullName))
                {
                    File.Delete(tempFile.FullName);
                }
                page.Response.End();
                page.Response.Close();
            }
        }

        #endregion
        #region 导入
        public delegate void ImportHandler(ExcelWorksheet ws,int r,Dictionary<string,object> dic);

        //导入批量上传的拆分
        public static bool Import(Page page, FileUpload fileuploadContrl,int ColumnCount,string sheetName,string savePath, ImportHandler HandlerMethod,Dictionary<string,object> dic)
        {
            bool result = true;           
            int columnCount = 17;
            string filePath = string.Empty;  //上传的文件的路径
            if (fileuploadContrl.HasFile)//上传文件存在
            {
                if (!Directory.Exists(savePath))//文件夹不存在
                {
                    Directory.CreateDirectory(savePath);//创建文件夹
                }
                string fileName = Guid.NewGuid().CastToString() + fileuploadContrl.FileName.Substring(fileuploadContrl.FileName.LastIndexOf("."));
                filePath = savePath + @"\" + fileName;//文件路径
                if (File.Exists(filePath))//文件存在
                {
                    File.Delete(filePath);  //删除文件
                }
                try
                {
                    fileuploadContrl.SaveAs(filePath);//添加文件
                }
                catch(Exception ex)
                {
                    result = false;
                    Log.CreateLogManager().Error("SqlBulkCopyHelper.SaveToStgTable 执行失败！", ex);
                    return result;
                }
            }
            FileInfo theFile = new FileInfo(filePath);
            using (ExcelPackage pck = new ExcelPackage(theFile))
            {
                var workbook = pck.Workbook;
                if (workbook != null)
                {
                    var ws = workbook.Worksheets[sheetName];

                    if (ws != null)
                    {
                        for (int r = 2; r < ws.Cells.Rows; r++)
                        {
                            if (ws.Cells[r, 1].Value == null)
                            {
                                break;
                            }
                            for (int c = 1; c < columnCount; c++)
                            {
                                if (ws.Cells[r, c].Merge)
                                {
                                    var address = new ExcelAddress(ws.MergedCells[r, c]);
                                    int mergerStart = address.Start.Row;
                                    int mergerEnd = address.End.Row;
                                    ws.Cells[mergerStart, c, mergerEnd, c].Merge = false;
                                    while (mergerEnd >= mergerStart)
                                    {
                                        int m = mergerStart;
                                        ws.Cells[m, c].Value = ws.Cells[r, c].Value;
                                        mergerStart += 1;
                                    }

                                }
                            }
                            ImportHandler handler = new ImportHandler(HandlerMethod);
                            handler(ws, r, dic);
                        }
                    }
                    else
                    {
                        return false;
                    }

                }
              
            }
            return true;
        }


        #endregion

    }
    /// <summary>
    /// 特殊颜色的excel列样式
    /// </summary>
    public class ExcelStyleColumn
    {
        public string ColumnName { get; set; }
        public System.Drawing.Color Color { get; set; }
    }

}