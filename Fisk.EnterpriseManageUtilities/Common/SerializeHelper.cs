using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web.Script.Serialization;
using System.Runtime.Serialization.Json;
using Newtonsoft.Json.Linq;

namespace Fisk.EnterpriseManageUtilities.Common
{
    public class SerializeHelper
    {
        /// <summary>          
        /// 序列化对象
        /// </summary> 
        /// <param name="obj">待序列化的对象</param>     
        /// <returns>序列化后的2进制数据</returns>          
        public static byte[] Serialize(object obj)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        /// <summary>          
        /// 序列化对象
        /// </summary> 
        /// <param name="obj">待序列化的对象</param>     
        /// <returns>序列化后的2进制数据</returns>          
        public static string SerializeToString(object obj)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                return Encoding.Default.GetString(Serialize(obj));
            }
        }


        /// <summary>          
        /// 反序列化          
        /// </summary>          
        /// <param name="data">2进制数据</param>          
        /// <returns>反序列化生成的对象</returns>          
        public static object DeSerialize(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream(data))
            {
                BinaryFormatter bf = new BinaryFormatter();
                return bf.Deserialize(ms);
            }
        }

        /// <summary>          
        /// 反序列化          
        /// </summary>          
        /// <param name="data">2进制数据</param>          
        /// <returns>反序列化生成的对象</returns>          
        public static T DeSerialize<T>(string data)
        {
            return (T)DeSerialize(Encoding.Default.GetBytes(data));
        }



        /// <summary>          
        /// Json序列化          
        /// </summary>          
        /// <param name="obj">待序列化的对象</param>
        /// <returns>json数据</returns>
        public static string JsonSerialize(object obj)
        {

            JavaScriptSerializer jss = new JavaScriptSerializer();
            StringBuilder result = new StringBuilder();
            jss.Serialize(obj, result);
            return result.ToString();
        }

        /// <summary>
        /// Json反序列化
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="json">json数据</param>
        /// <returns>反序列化生成的对象</returns>
        public static T JsonDeSerialize<T>(string json)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            StringBuilder result = new StringBuilder();
            return jss.Deserialize<T>(json);
        }
        /// <summary>
        /// json数组对象反序列化成list
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="json">json数组</param>
        /// <returns>list数据集</returns>
        public static List<T> JsonToSerializeList<T>(string json)
        {
            List<T> ImportDatas = new List<T>();
            DataContractJsonSerializer _Json = new DataContractJsonSerializer(ImportDatas.GetType());
            byte[] _Using = System.Text.Encoding.UTF8.GetBytes(json);
            System.IO.MemoryStream _MemoryStream = new System.IO.MemoryStream(_Using);
            _MemoryStream.Position = 0;
            ImportDatas = (List<T>)_Json.ReadObject(_MemoryStream);
            return ImportDatas;
        }

        /// <summary> 
        /// DataTable转为json 
        /// </summary> 
        /// <param name="dt">DataTable</param> 
        /// <returns>json数据</returns> 
        public static string DataTableToJson(DataTable dt)
        {
            if (dt != null && dt.Rows.Count > 0)
            {
                List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
                foreach (DataRow dr in dt.Rows)
                {
                    Dictionary<string, object> result = new Dictionary<string, object>();
                    foreach (DataColumn dc in dt.Columns)
                    {
                        result.Add(dc.ColumnName, dr[dc]);
                    }
                    list.Add(result);
                }

                return JsonSerialize(list);
            }
            else
            {
                return string.Empty;
            }
        }


        /// <summary>
        /// 把dataset数据转换成json的格式
        /// </summary>
        /// <param name="ds">dataset数据集</param>
        /// <returns>json格式的字符串</returns>
        public static string DataSetToJson(DataSet ds)
        {
            if (ds == null || ds.Tables.Count <= 0 || ds.Tables[0].Rows.Count <= 0)
            {
                //如果查询到的数据为空则返回标记ok:false
                return "{\"ok\":false}";
            }
            StringBuilder sb = new StringBuilder();
            sb.Append("{\"ok\":true,");
            int k = 0;
            foreach (DataTable dt in ds.Tables)
            {
                sb.Append(string.Format("\"{0}\":", !string.IsNullOrEmpty(dt.TableName) ? dt.TableName : "data(" + k + ")"));

                string json = DataTableToJson(dt);
                sb.Append(json);
                sb.Append(",");
                k++;
            }
            sb.Remove(sb.ToString().LastIndexOf(','), 1);
            sb.Append("}");
            return sb.ToString();
        }


        //字符串转数据字典
        public static Dictionary<string, object> JsonToDic(string jsonstr)
        {
            //Dictionary<object, object> dic = null;
            //JObject mJObj = JObject.Parse(jsonstr);
            //if (mJObj.Properties() != null && mJObj.Count > 0)
            //{
            //    dic = new Dictionary<object, object>();
            //    foreach (JProperty jp in mJObj.Properties())
            //    {
            //        dic.Add(jp.Name, jp.Value);
            //    }
            //}
            JavaScriptSerializer jss = new JavaScriptSerializer();
            Dictionary<string, object> dic = (Dictionary<string, object>)jss.DeserializeObject(jsonstr);


            return dic;
        }



    }
}
