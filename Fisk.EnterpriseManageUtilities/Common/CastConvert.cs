
/*******************************************************************
 * * 版权所有：菲斯克（上海）软件有限公司
 * * 功    能： 
 * * 作    者：jerryli
 * * 电子邮箱：jerryl@runmont.com
 * * 创建日期： 2012/9/13 5:03:25
 * *******************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;

namespace Fisk.EnterpriseManageUtilities.Common
{
    /// <summary>
    /// 类型强制转换
    /// </summary>
    public static class CastConvert
    {
        #region 共有的自定义转换方法
        /// <summary>
        /// 将类型转换成String类型,如果为空则返回空
        /// </summary>
        /// <param name="obj">转换对象</param>
        /// <returns></returns>
        public static String CastToString(this object obj)
        {
            try
            {
                if (obj.ToString().Contains(''))
                {
                    return obj.ToString().Replace("", "");
                }
                else
                {
                    return obj.ToString();
                }
            }
            catch
            {
                return String.Empty;
            }
        }

        /// <summary>
        /// 将类型转换成SQL中的String类型,处理一些sql特殊字符
        /// </summary>
        /// <param name="obj">转换对象</param>
        /// <returns></returns>
        public static String CastToSqlString(this object obj)
        {
            try
            {
                if (obj.ToString().Contains("'"))
                {
                    return obj.ToString().Replace("'", "''");
                }
                else
                {
                    return obj.ToString();
                }
            }
            catch
            {
                return String.Empty;
            }
        }
        /// <summary>
        /// 将字符串转换为GUID
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static Guid CastToGUID(this object str)
        {
            System.Guid mGuid = new Guid(str.CastToString());
            return mGuid;
        }


        public static string CastToString(this object obj, string format)
        {
            try
            {
                string s = obj.CastToString();
                if (string.IsNullOrEmpty(s))
                {
                    return string.Empty;
                }
                else
                {
                    return Convert.ToDouble(s).ToString(format);
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 将类型转换成Boolean类型,如果为空则返回false
        /// </summary>
        /// <param name="obj">转换对象</param>
        /// <returns></returns>
        public static Boolean CastToBoolean(this object obj)
        {
            try
            {
                return Convert.ToBoolean(obj);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 将类型转换成Int32类型,如果为空则返回0
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Int32 CastToInt32(this object obj)
        {
            try
            {
                return Convert.ToInt32(obj);
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// 将类型转换成float类型,如果为空则返回0
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static float CastToFloat(this object obj)
        {
            try
            {
                return float.Parse(obj.CastToString());
            }
            catch
            {
                return 0;
            }
        }

        public static float? CastToFloatOrNull(this object obj)
        {
            try
            {
                return float.Parse(obj.CastToString());
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// 将类型转换成Double类型,如果为空则返回0
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Double CastToDouble(this object obj)
        {
            try
            {
                return Convert.ToDouble(obj);
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// 将类型转换成DateTime类型,如果为空则返回DateTime最小值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static DateTime CastToDateTime(this object obj)
        {
            try
            {
                return Convert.ToDateTime(obj);
            }
            catch
            {
                return DateTime.MinValue;
            }
        }


        /// <summary>  
        /// GMT时间转成本地时间  
        /// </summary>  
        /// <param name="gmt">字符串形式的GMT时间</param>  
        /// <returns></returns>  
        public static DateTime CastUTCToLocal(this string gmt)
        {
            DateTime dt = DateTime.MinValue;
            try
            {
                string pattern = "";
                if (gmt.IndexOf("+0") != -1)
                {
                    gmt = gmt.Replace("GMT", "");
                    pattern = "ddd, dd MMM yyyy HH':'mm':'ss zzz";
                }
                if (gmt.ToUpper().IndexOf("GMT") != -1)
                {
                    pattern = "ddd, dd MMM yyyy HH':'mm':'ss 'GMT'";
                }
                if (pattern != "")
                {
                    dt = DateTime.ParseExact(gmt, pattern, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AdjustToUniversal);
                    dt = dt.ToLocalTime();
                }
                else
                {
                    dt = Convert.ToDateTime(gmt);
                }
            }
            catch
            {
            }
            return dt;
        }



        public static string CastToDateString(this object obj, string format)
        {
            if (obj == null)
            {
                return string.Empty;
            }
            else
            {
                return obj.CastToDateTime().ToString(format);
            }
        }

        public static Decimal CastToDecimalNoNull(this object obj)
        {
            try
            {
                if (obj != null)
                {
                    return Convert.ToDecimal(obj);
                }
                else
                {
                    return 0;
                }
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// 将类型转换成DateTime类型,如果为空则返回null
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static DateTime? CastToDateTimeOrNull(this object obj)
        {
            try
            {
                return Convert.ToDateTime(obj);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 将类型转换成Decimal类型
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Decimal? CastToDecimal(this object obj)
        {
            try
            {
                if (obj != null)
                {


                    if (obj.CastToString().Contains('%'))
                    {
                        return Convert.ToDecimal(obj.CastToString().Replace("%", "")) / 100;
                    }
                    else
                    {

                        return Convert.ToDecimal(obj);
                    }
                }
                else
                {
                    return null;
                }

            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 将类型转换成Decimal类型
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="i">小数点后保留小数位数</param>
        /// <returns></returns>
        public static Decimal? CastToDecimal(this object obj, int i)
        {
            if (obj.CastToDecimal() != null)
            {
                return Math.Round(decimal.Parse(obj.CastToDecimal().ToString()), i);
            }
            else
            {
                return obj.CastToDecimal();
            }

        }

        /// <summary>
        /// 将类型转换成带有%类型
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="i">保留几位小数</param>
        /// <returns></returns>
        public static string CastToPer(this object obj, int i)
        {
            try
            {
                if (obj != null)
                {


                    if (obj.CastToString().Contains('%'))
                    {
                        return obj.CastToString();
                    }
                    else
                    {
                        return (Convert.ToDecimal(obj) * 100).CastToDecimal(i).ToString() + "%";
                    }
                }
                else
                {
                    return null;
                }

            }
            catch
            {
                return null;
            }
        }


        /// <summary>
        /// 将DataTable类型转化为指定类型的实体集合
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="dt">转换的DataTable</param>
        /// <param name="dateTimeToString">是否需要将日期转换为字符串，默认为转换,值为true</param>
        /// <returns></returns>
        public static List<T> CastToList<T>(this DataTable dt, bool dateTimeToString) where T : class, new()
        {
            try
            {
                List<T> list = null;

                if (dt != null && dt.Rows.Count > 0)
                {
                    list = new List<T>();
                    List<PropertyInfo> infos = new List<PropertyInfo>();

                    Array.ForEach<PropertyInfo>(typeof(T).GetProperties(), p =>
                    {
                        if (dt.Columns.Contains(p.Name) == true)
                        {
                            infos.Add(p);
                        }
                    });

                    SetList<T>(list, infos, dt, dateTimeToString);
                }
                return list;
            }
            catch
            {
                return null;
            }

        }


        /// <summary>
        /// Ilist 转换成 DataSet
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static DataSet ConvertToDataSet<T>(this IList<T> list)
        {
            if (list == null || list.Count <= 0)
            {
                return null;
            }

            DataSet ds = new DataSet();
            DataTable dt = new DataTable(typeof(T).Name);
            DataColumn column;
            DataRow row;

            System.Reflection.PropertyInfo[] myPropertyInfo = typeof(T).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

            foreach (T t in list)
            {
                if (t == null)
                {
                    continue;
                }

                row = dt.NewRow();

                for (int i = 0, j = myPropertyInfo.Length; i < j; i++)
                {
                    System.Reflection.PropertyInfo pi = myPropertyInfo[i];

                    string name = pi.Name;

                    if (dt.Columns[name] == null)
                    {
                        column = new DataColumn(name, pi.PropertyType);
                        dt.Columns.Add(column);
                    }

                    row[name] = pi.GetValue(t, null);
                }

                dt.Rows.Add(row);
            }

            ds.Tables.Add(dt);

            return ds;
        }

        /// <summary>
        /// 将datatable转换为dictionary List
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static List<Dictionary<string, object>> CastToDictionaryList(this DataTable dt)
        {
            List<Dictionary<string, object>> lists = null;
            if (dt != null && dt.Rows.Count > 0)
            {
                lists = new List<Dictionary<string, object>>();
                foreach (DataRow dr in dt.Rows)
                {
                    Dictionary<string, object> dct = new Dictionary<string, object>();
                    foreach (DataColumn dc in dt.Columns)
                    {
                        if (dr[dc.ColumnName] != System.DBNull.Value)
                        {
                            dct.Add(dc.ColumnName, dr[dc.ColumnName]);
                        }
                        else
                        {
                            dct.Add(dc.ColumnName,"");
                        }
                    }
                    lists.Add(dct);
                }
            }
            return lists;
        }

        #endregion

        #region 扩展
        /// <summary>
        /// 对List进行Distinct
        /// 例如：
        /// var query = people.CastDistinctBy(p => new { p.Id, p.Name });
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        public static IEnumerable<TSource> CastDistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }

        /// <summary>
        /// 解码URL参数
        /// </summary>
        /// <param name="str"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string MyUrlDeCode(string str, Encoding encoding)
        {
            if (encoding == null)
            {
                Encoding utf8 = Encoding.UTF8;
                //首先用utf-8进行解码                    
                string code = System.Web.HttpUtility.UrlDecode(str.ToUpper(), utf8);
                //将已经解码的字符再次进行编码.
                string encode = System.Web.HttpUtility.UrlEncode(code, utf8).ToUpper();
                if (str == encode)
                    encoding = Encoding.UTF8;
                else
                    encoding = Encoding.GetEncoding("gb2312");
            }
            return System.Web.HttpUtility.UrlDecode(str, encoding);
        }
        #endregion

        #region 私有方法
        //DataTable转换为list时得到list
        private static void SetList<T>(List<T> list, List<PropertyInfo> infos, DataTable dt, bool dateTimeToString) where T : class, new()
        {
            foreach (DataRow dr in dt.Rows)
            {
                T model = new T();

                infos.ForEach(p =>
                {
                    if (dr[p.Name] != DBNull.Value)
                    {
                        object tempValue = dr[p.Name];
                        try
                        {
                            if (p.PropertyType != dr[p.Name].GetType())
                            {
                                if (p.PropertyType.FullName.Contains("DateTime"))
                                {
                                    DateTime? ValueType = tempValue.CastToDateTimeOrNull();
                                    p.SetValue(model, ValueType, null);
                                }
                                else if (p.PropertyType.FullName.Contains("Int"))
                                {
                                    int ValueType = tempValue.CastToInt32();
                                    p.SetValue(model, ValueType, null);
                                }
                                else if (p.PropertyType == typeof(string))
                                {
                                    string ValueType = tempValue.CastToString();
                                    p.SetValue(model, ValueType, null);

                                }
                                else if (p.PropertyType.FullName.Contains("Double"))
                                {
                                    double ValueType = tempValue.CastToDouble();
                                    p.SetValue(model, ValueType, null);
                                }
                                else if (p.PropertyType.FullName.Contains("Decimal"))
                                {
                                    decimal? ValueType = tempValue.CastToDecimal();
                                    p.SetValue(model, ValueType, null);
                                }
                                else
                                {
                                    p.SetValue(model, tempValue, null);
                                }
                            }
                            else
                            {
                                p.SetValue(model, tempValue, null);
                            }

                        }
                        catch { }
                    }
                });
                list.Add(model);
            }
        }

        #endregion

    }
}
