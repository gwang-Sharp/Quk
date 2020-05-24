using System;
using System.ComponentModel;

namespace GH.FreeBI.Component.Common
{
    public class EnumHelper
    {

        /// <summary>
        /// 获取定义在枚举上的描述元数据。
        /// </summary>
        /// <param name="value">要获取描述的枚举值。</param>
        /// <returns>成功返回获取到的描述文本，否则返回 null</returns>
        public static string GetDescription(Enum value)
        {
            var type = value.GetType();
            var name = Enum.GetName(type, value);
            var fi = type.GetField(name);
            var des = fi.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];
            if (des != null && des.Length > 0)
            {
                return des[0].Description;
            }
            return string.Empty;
        }

        public static string GetXmlEnum(Enum value)
        {
            var type = value.GetType();
            var name = Enum.GetName(type, value);
            var fi = type.GetField(name);
            var des = fi.GetCustomAttributes(typeof(System.Xml.Serialization.XmlEnumAttribute), false) as System.Xml.Serialization.XmlEnumAttribute[];
            if (des != null && des.Length > 0)
            {
                return des[0].Name;
            }
            return string.Empty;
        }


        /// <summary>
        /// 判断字符串是否是枚举的项
        /// </summary>
        /// <typeparam name="T">枚举</typeparam>
        /// <param name="s">要判断的字符串</param>
        /// <returns></returns>
        public static bool IsEnumItem<T>(string s)
        {
            bool result = false;
            Type t = typeof(T);
            foreach (string obj in Enum.GetNames(t))
            {
                if (s == obj)
                {
                    result = true;
                }
            }
            return result;
        }
        /// <summary>
        /// 根据值获取枚举
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="q"></param>
        /// <returns></returns>
        public static T GetEnumByValue<T>(string v)
        {
            T enumItem = (T)Enum.Parse(typeof(T), v);
            return enumItem;
        }


        public static T GetEnumByXMLAttributeName<T>(string value)
        {
            T enumItem = default(T);
            Type type = typeof(T);
            Array Arrays = Enum.GetValues(type);
            for (int i = 0; i < Arrays.LongLength; i++)
            {

                var name = Enum.GetName(type, Enum.Parse(typeof(T), Arrays.GetValue(i).ToString()));

                var fi = type.GetField(name);
                var des = fi.GetCustomAttributes(typeof(System.Xml.Serialization.XmlEnumAttribute), false) as System.Xml.Serialization.XmlEnumAttribute[];
                if (des != null && des.Length > 0)
                {
                    if (des[0].Name == value)
                    {
                        enumItem = (T)Enum.Parse(typeof(T), Arrays.GetValue(i).ToString());
                        break;
                    }
                }
            }
            return enumItem;
        }
    }
}
