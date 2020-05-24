using Fisk.EnterpriseManageUtilities.Common;
using Microsoft.AnalysisServices;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fisk.EnterpriseManageUtilities.DBUtility
{
    public class AmoHelper
    {
        public DataTable GetAttributes(string sourceServerConn, string sourceCategory, string sourceCube)
        {
            return GetAttributesInfo(sourceServerConn, sourceCategory, sourceCube);
        }

        private DataTable GetAttributesInfo(string SourceServerConn, string SourceCategory, string SourceCube)
        {
            DataTable dtResult = new DataTable();
            dtResult.Columns.Add("Server");
            dtResult.Columns.Add("Category");
            dtResult.Columns.Add("Cube");
            dtResult.Columns.Add("Dimension");
            dtResult.Columns.Add("Attribute_Name");
            dtResult.Columns.Add("Attribute_ID");
            dtResult.Columns.Add("Attribute_Translation_ID");
            dtResult.Columns.Add("Attribute_Translation_Caption");
            dtResult.Columns.Add("Attribute_Translation_Language");
            dtResult.Columns.Add("Attribute_Translation_CommbinedQuery");
            dtResult.Columns.Add("Attribute_Translation_CommbinedQuery_Column");
            dtResult.Columns.Add("Attribute_Column_Type");
            dtResult.Columns.Add("Attribute_Column_CommbinedQuery");
            dtResult.Columns.Add("Attribute_Column_CommbinedQuery_Column");


            Server SourceServer = new Server();
            if (SourceServerConn == null || SourceServerConn.Equals("") || SourceCategory == null || SourceCategory.Equals(""))
            {
                return dtResult;
            }

            string strConn = @"Data Source = " + SourceServerConn + ";Provider=MSOLAP.5;Integrated Security=SSPI;Persist Security Info=True;MDX Compatibility=1;Safety Options=2;";

            try
            {
                SourceServer.Connect(SourceServerConn);
                Database sSourceCategory = SourceServer.Databases.FindByName(SourceCategory);
                Cube sCube = sSourceCategory.Cubes.FindByName(SourceCube);

                int i = 0;
                foreach (CubeDimension cd in sCube.Dimensions)
                {
                    foreach (CubeAttribute a in cd.Attributes)
                    {
                        int icb = 1;

                        DataTable dtTranslation = GetAttributeTranslation(a.Attribute, 1033);

                        foreach (DataItem cb in a.Attribute.KeyColumns)
                        {
                            DataRow dr = dtResult.NewRow();
                            dtResult.Rows.Add(dr);

                            dtResult.Rows[i]["Server"] = cd.ParentServer;
                            dtResult.Rows[i]["Category"] = cd.ParentDatabase;
                            dtResult.Rows[i]["Cube"] = cd.Parent;
                            dtResult.Rows[i]["Dimension"] = a.Attribute.Parent;
                            dtResult.Rows[i]["Attribute_Name"] = a.Attribute.Name;
                            dtResult.Rows[i]["Attribute_ID"] = a.Attribute.ID;
                            dtResult.Rows[i]["Attribute_Translation_ID"] = dtTranslation.Rows.Count > 0 ? dtTranslation.Rows[0]["Attribute_Translation_ID"] : null;
                            dtResult.Rows[i]["Attribute_Translation_Caption"] = dtTranslation.Rows.Count > 0 ? dtTranslation.Rows[0]["Attribute_Translation_Caption"] : null;
                            dtResult.Rows[i]["Attribute_Translation_Language"] = dtTranslation.Rows.Count > 0 ? dtTranslation.Rows[0]["Attribute_Translation_Language"] : null;
                            dtResult.Rows[i]["Attribute_Translation_CommbinedQuery"] = dtTranslation.Rows.Count > 0 ? dtTranslation.Rows[0]["Attribute_Translation_CommbinedQuery"] : null;
                            dtResult.Rows[i]["Attribute_Translation_CommbinedQuery_Column"] = dtTranslation.Rows.Count > 0 ? dtTranslation.Rows[0]["Attribute_Translation_CommbinedQuery_Column"] : null;
                            dtResult.Rows[i]["Attribute_Column_Type"] = "KeyColumn" + (icb == 1 ? "" : "_" + icb.ToString());
                            dtResult.Rows[i]["Attribute_Column_CommbinedQuery"] = ((Microsoft.AnalysisServices.ColumnBinding)cb.Source).TableID;
                            dtResult.Rows[i]["Attribute_Column_CommbinedQuery_Column"] = ((Microsoft.AnalysisServices.ColumnBinding)cb.Source).ColumnID;

                            icb++;
                            i++;
                        }

                        if (a.Attribute.NameColumn != null)
                        {
                            DataRow dr = dtResult.NewRow();
                            dtResult.Rows.Add(dr);

                            dtResult.Rows[i]["Server"] = cd.ParentServer;
                            dtResult.Rows[i]["Category"] = cd.ParentDatabase;
                            dtResult.Rows[i]["Cube"] = cd.Parent;
                            dtResult.Rows[i]["Dimension"] = a.Attribute.Parent;
                            dtResult.Rows[i]["Attribute_Name"] = a.Attribute.Name;
                            dtResult.Rows[i]["Attribute_ID"] = a.Attribute.ID;
                            dtResult.Rows[i]["Attribute_Translation_ID"] = dtTranslation.Rows.Count > 0 ? dtTranslation.Rows[0]["Attribute_Translation_ID"] : null;
                            dtResult.Rows[i]["Attribute_Translation_Caption"] = dtTranslation.Rows.Count > 0 ? dtTranslation.Rows[0]["Attribute_Translation_Caption"] : null;
                            dtResult.Rows[i]["Attribute_Translation_Language"] = dtTranslation.Rows.Count > 0 ? dtTranslation.Rows[0]["Attribute_Translation_Language"] : null;
                            dtResult.Rows[i]["Attribute_Translation_CommbinedQuery"] = dtTranslation.Rows.Count > 0 ? dtTranslation.Rows[0]["Attribute_Translation_CommbinedQuery"] : null;
                            dtResult.Rows[i]["Attribute_Translation_CommbinedQuery_Column"] = dtTranslation.Rows.Count > 0 ? dtTranslation.Rows[0]["Attribute_Translation_CommbinedQuery_Column"] : null;
                            dtResult.Rows[i]["Attribute_Column_Type"] = "NameColumn";
                            dtResult.Rows[i]["Attribute_Column_CommbinedQuery"] = ((Microsoft.AnalysisServices.ColumnBinding)a.Attribute.NameColumn.Source).TableID;
                            dtResult.Rows[i]["Attribute_Column_CommbinedQuery_Column"] = ((Microsoft.AnalysisServices.ColumnBinding)a.Attribute.NameColumn.Source).ColumnID;

                            i++;
                        }
                        if (a.Attribute.ValueColumn != null)
                        {
                            DataRow dr = dtResult.NewRow();
                            dtResult.Rows.Add(dr);

                            dtResult.Rows[i]["Server"] = cd.ParentServer;
                            dtResult.Rows[i]["Category"] = cd.ParentDatabase;
                            dtResult.Rows[i]["Cube"] = cd.Parent;
                            dtResult.Rows[i]["Dimension"] = a.Attribute.Parent;
                            dtResult.Rows[i]["Attribute_Name"] = a.Attribute.Name;
                            dtResult.Rows[i]["Attribute_ID"] = a.Attribute.ID;
                            dtResult.Rows[i]["Attribute_Translation_ID"] = dtTranslation.Rows.Count > 0 ? dtTranslation.Rows[0]["Attribute_Translation_ID"] : null;
                            dtResult.Rows[i]["Attribute_Translation_Caption"] = dtTranslation.Rows.Count > 0 ? dtTranslation.Rows[0]["Attribute_Translation_Caption"] : null;
                            dtResult.Rows[i]["Attribute_Translation_Language"] = dtTranslation.Rows.Count > 0 ? dtTranslation.Rows[0]["Attribute_Translation_Language"] : null;
                            dtResult.Rows[i]["Attribute_Translation_CommbinedQuery"] = dtTranslation.Rows.Count > 0 ? dtTranslation.Rows[0]["Attribute_Translation_CommbinedQuery"] : null;
                            dtResult.Rows[i]["Attribute_Translation_CommbinedQuery_Column"] = dtTranslation.Rows.Count > 0 ? dtTranslation.Rows[0]["Attribute_Translation_CommbinedQuery_Column"] : null;
                            dtResult.Rows[i]["Attribute_Column_Type"] = "ValueColumn";
                            dtResult.Rows[i]["Attribute_Column_CommbinedQuery_Column"] = ((Microsoft.AnalysisServices.ColumnBinding)a.Attribute.ValueColumn.Source).TableID;
                            dtResult.Rows[i]["Attribute_Column_CommbinedQuery_Column"] = ((Microsoft.AnalysisServices.ColumnBinding)a.Attribute.ValueColumn.Source).ColumnID;

                            i++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.CreateLogManager().Error("返回值：", ex);
                throw ex;
            }
            finally
            {

            }
            return dtResult;
        }


        public DataTable getDataBasepublic(string SourceServerConn)
        {
            return getDataBase(SourceServerConn);
        }

        private DataTable getDataBase(string SourceServerConn)
        {
            DataTable dtResult = new DataTable();
            dtResult.Columns.Add("Database");
            Server SourceServer = new Server();
            try
            {
                SourceServer.Connect(SourceServerConn);

                //Database sSourceCategory = SourceServer.Databases.FindByName(SourceCategory);

                //DatabaseCollection a = ;
                for (int a = 0; a < SourceServer.Databases.Count; a++)
                {
                    DataRow dr = dtResult.NewRow();
                    dtResult.Rows.Add(dr);
                    dtResult.Rows[a]["Database"] = SourceServer.Databases[a];
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {

            }

            return dtResult;
        }


        public DataTable getCubepublic(string SourceServerConn, string SourceCategory)
        {
            return getCube(SourceServerConn, SourceCategory);
        }

        private DataTable getCube(string SourceServerConn, string SourceCategory)
        {
            DataTable dtResult = new DataTable();
            dtResult.Columns.Add("Cube");
            Server SourceServer = new Server();
            try
            {
                SourceServer.Connect(SourceServerConn);

                Database sSourceCategory = SourceServer.Databases[SourceCategory];
                //Cube a = sSourceCategory.Cubes.Count;
                //DatabaseCollection a = ;
                for (int a = 0; a < sSourceCategory.Cubes.Count; a++)
                {
                    DataRow dr = dtResult.NewRow();
                    dtResult.Rows.Add(dr);
                    dtResult.Rows[a]["Cube"] = sSourceCategory.Cubes[a];
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {

            }

            return dtResult;
        }



        private DataTable GetAttributeTranslation(DimensionAttribute da, int Language)
        {
            DataTable dtResult = new DataTable();
            dtResult.Columns.Add("Attribute_Name");
            dtResult.Columns.Add("Attribute_ID");
            dtResult.Columns.Add("Attribute_Translation_ID");
            dtResult.Columns.Add("Attribute_Translation_Caption");
            dtResult.Columns.Add("Attribute_Translation_Language");
            dtResult.Columns.Add("Attribute_Translation_CommbinedQuery");
            dtResult.Columns.Add("Attribute_Translation_CommbinedQuery_Column");

            if (da.Translations.Count == 0)
            {
                return dtResult;
            }
            AttributeTranslation at = (AttributeTranslation)da.Translations.FindByLanguage(Language);

            if (at == null)
            {
                return dtResult;
            }

            DataRow dr = dtResult.NewRow();
            dtResult.Rows.Add(dr);

            dtResult.Rows[0]["Attribute_Name"] = da.Name;
            dtResult.Rows[0]["Attribute_ID"] = da.ID;
            dtResult.Rows[0]["Attribute_Translation_ID"] = at.Caption;
            dtResult.Rows[0]["Attribute_Translation_Caption"] = at.Caption;
            dtResult.Rows[0]["Attribute_Translation_Language"] = at.Language;
            dtResult.Rows[0]["Attribute_Translation_CommbinedQuery"] = at.CaptionColumn != null ? ((Microsoft.AnalysisServices.ColumnBinding)at.CaptionColumn.Source).TableID : string.Empty;
            dtResult.Rows[0]["Attribute_Translation_CommbinedQuery_Column"] = at.CaptionColumn != null ? ((Microsoft.AnalysisServices.ColumnBinding)at.CaptionColumn.Source).ColumnID : string.Empty;

            return dtResult;
        }


    }
}
