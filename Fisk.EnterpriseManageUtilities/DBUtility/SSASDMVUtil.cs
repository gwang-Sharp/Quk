using Fisk.EnterpriseManageUtilities.Common;
using Microsoft.AnalysisServices.AdomdClient;
using System;
using System.Data;
using System.IO;
using System.Xml;

/************************************************************************************
 *Copyright (c) 2014 All Rights Reserved.
 *CLR版本： 4.0.30319.18444
 *公司名称：容盟软件（上海）有限公司
 *机器名称：DEV
 *命名空间：Fisk.EnterpriseManageUtilities.DBUtility
 *文件名：  SSASDMVUtil
 *版本号：  V1.0.0.0
 *唯一标识：550a192b-b069-4137-bd8f-22d1b483f2df
 *当前的用户域：LXY
 *创建人：  jerryli
 *创建时间：2014/3/11 14:14:20
 *描述：操作分析服务DMV方法
 *
 *=====================================================================
 *修改时间：2014/3/11 14:14:20
 *修改人： jerryli
 *版本号： V1.0.0.0
 *描述：
 *
/************************************************************************************/



namespace Fisk.EnterpriseManageUtilities.DBUtility
{
    /// <summary>
    /// 操作分析服务DMV方法
    /// </summary>
    public class SSASDMVUtil
    {
        /// <summary>
        /// 得到连接字符串
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static AdomdConnection GetConn(string connstr)
        {
            AdomdConnection conn = new AdomdConnection();
            conn.ConnectionString = connstr;
            return conn;
        }
        /// <summary>
        /// 分析服务是否连接成功
        /// </summary>
        /// <param name="connStr"></param>
        /// <returns></returns>
        public static bool IsSecessConnect(string connStr)
        {
            bool result = false;
            try
            {
                AdomdConnection conn = new AdomdConnection(connStr);
                conn.Open();

                conn.Close();
                result = true;
            }
            catch
            {
                result = false;
            }
            return result;
        }

        /// <summary>
        /// 得到ADOMD Reader 
        /// </summary>
        /// <param name="conn">ADOMDConnection 切记用完关闭</param>
        /// <param name="cmdText"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        public static AdomdDataReader ExecuteReader(AdomdConnection conn, string cmdText, params AdomdParameter[] cmdParms)
        {
            AdomdCommand cmd = new AdomdCommand();
            AdomdDataReader reader;
            try
            {
                cmd.Connection = conn;
                cmd.CommandText = cmdText;
                cmd.CommandTimeout = 60000;
                cmd.CommandType = CommandType.Text;
                if (cmdParms != null)
                {
                    foreach (AdomdParameter parm in cmdParms)
                        cmd.Parameters.Add(parm);
                }
                reader = cmd.ExecuteReader();
                return reader;
            }
            catch (Exception ex)
            {
                conn.Close();
                Log.CreateLogManager().Error("SSASDMV 方法ExecuteReader执行错误", ex);
                return null;
            }
            finally
            {

            }

        }

        /// <summary>
        /// 打开连接
        /// </summary>
        /// <param name="conn"></param>
        public static void OpenConn(AdomdConnection conn)
        {
            if (conn != null && conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
        }
        /// <summary>
        /// 关闭连接
        /// </summary>
        /// <param name="conn"></param>
        public static void CloseConn(AdomdConnection conn)
        {
            if (conn != null && conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
        }

        /// <summary>
        /// 得到笛卡尔积数据 根据MDX
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="mdx"></param>
        /// <returns></returns>
        public static CellSet GetCellSet(AdomdConnection conn, string mdx)
        {
            CellSet result = null;
            if (conn != null && conn.State == ConnectionState.Open)
            {
                AdomdCommand command;
                command = new AdomdCommand(mdx, conn);
                result = command.ExecuteCellSet();
                //   command.ExecuteXmlReader();
            }
            return result;
        }


        public static bool SaveCellToXML(AdomdConnection conn, string mdx, string xmlName)
        {
#pragma warning disable CS0219 // 变量“result”已被赋值，但从未使用过它的值
            CellSet result = null;
#pragma warning restore CS0219 // 变量“result”已被赋值，但从未使用过它的值
            if (conn != null && conn.State == ConnectionState.Open)
            {
                AdomdCommand command;
                command = new AdomdCommand(mdx, conn);
                XmlReader xmlReader = command.ExecuteXmlReader();
                string xml = xmlReader.ReadOuterXml();
                StreamWriter streamWriter = File.CreateText(xmlName);
                streamWriter.WriteLine(xml);
                streamWriter.Close();
                //  result = command.ExecuteCellSet();
            }
            return true;
        }

        /// <summary>  
        /// CellSet转成DataTable（只能转换一维二维数据）  
        /// </summary>  
        /// <param name="cs"></param>  
        /// <returns></returns>  
        public static DataTable ToDataTable(CellSet cs)
        {
            DataTable dt = new DataTable("ResultTable");
            DataColumn dc = null;
            DataRow dr = null;

            //列名或行名  
            string name;
            //添加行数据  
            int pos = 0;

            if (cs.Axes.Count > 0)
            {
                //当为二维数据时，第一列：必有为维度描述（行头）  
                if (cs.Axes.Count == 2) dt.Columns.Add(new DataColumn("Description"));

                //生成数据列对象  
                foreach (Position px in cs.Axes[0].Positions)
                {
                    dc = new DataColumn();
                    name = "";

                    foreach (Member m in px.Members) name = name + m.Caption + " ";
                    dc.ColumnName = name;

                    dt.Columns.Add(dc);
                }
            }

            if (cs.Axes.Count == 1)
            {
                //一维数据转换为DataTable  
                int rowCount = cs.Cells.Count / dt.Columns.Count;
                for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
                {
                    dr = dt.NewRow();

                    for (int columnIndex = 0; columnIndex < dt.Columns.Count; columnIndex++)
                    {
                        dr[columnIndex] = cs[pos++].FormattedValue;
                    }

                    dt.Rows.Add(dr);
                }
            }
            else if (cs.Axes.Count == 2)
            {
                //二维数据转换为DataTable  
                foreach (Position py in cs.Axes[1].Positions)
                {
                    dr = dt.NewRow();

                    //维度描述列数据（行头）  
                    name = "";
                    foreach (Member m in py.Members)
                    {
                        name = name + m.Caption;// +"\r\n";  
                    }
                    dr[0] = name;

                    //数据列  
                    for (int x = 1; x <= cs.Axes[0].Positions.Count; x++)
                    {
                        dr[x] = cs[pos++].FormattedValue;
                    }

                    dt.Rows.Add(dr);
                }
            }

            return dt;
        }
    }
}
