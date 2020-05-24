using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Fisk.EnterpriseManageUtilities.Common
{
    public class HttpUtils
    {
        /// <summary>
        /// 执行HTTP GET请求。
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="parameters">请求参数</param>
        /// <returns>HTTP响应</returns>
        public static string DoGet(string url, IDictionary<string, string> parameters)
        {
            if (parameters != null && parameters.Count > 0)
            {
                if (url.Contains("?"))
                {
                    url = url + "&" + BuildPostData(parameters);
                }
                else
                {
                    url = url + "?" + BuildPostData(parameters);
                }
            }
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.ServicePoint.Expect100Continue = false;
            req.Method = "GET";
            req.KeepAlive = true;
            req.UserAgent = "Test";
            req.ContentType = "application/x-www-form-urlencoded;charset=utf-8";
            HttpWebResponse rsp = null;
            try
            {
                rsp = (HttpWebResponse)req.GetResponse();
            }
            catch (WebException webEx)
            {
                if (webEx.Status == WebExceptionStatus.Timeout)
                {
                    rsp = null;
                }
            }

            if (rsp != null)
            {
                if (rsp.CharacterSet != null)
                {
                    Encoding encoding = Encoding.GetEncoding(rsp.CharacterSet);
                    return GetResponseAsString(rsp, encoding);
                }
                else
                {
                    return string.Empty;
                }
            }
            else
            {
                return string.Empty;
            }
        }
        /// <summary>
        /// 执行HTTP GET 请求
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="parameters">请求参数</param>
        /// <param name="proxyIP">代理IP</param>
        /// <param name="portNum">端口</param>
        /// <returns></returns>
        public static string DoGet(string url, IDictionary<string, string> parameters, string proxyIP, int portNum)
        {
            if (parameters != null && parameters.Count > 0)
            {
                if (url.Contains("?"))
                {
                    url = url + "&" + BuildPostData(parameters);
                }
                else
                {
                    url = url + "?" + BuildPostData(parameters);
                }
            }
            string beginProxy = proxyIP;
            int port = portNum;
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            System.Net.WebProxy proxy = new WebProxy(beginProxy, port);
            req.Proxy = proxy;
            req.ServicePoint.Expect100Continue = false;
            req.Method = "GET";
            req.KeepAlive = true;
            req.UserAgent = "Test";
            req.ContentType = "application/x-www-form-urlencoded;charset=utf-8";

            HttpWebResponse rsp = null;
            try
            {
                rsp = (HttpWebResponse)req.GetResponse();
            }
            catch (WebException webEx)
            {
                if (webEx.Status == WebExceptionStatus.Timeout)
                {
                    rsp = null;
                }
            }

            if (rsp != null)
            {
                if (rsp.CharacterSet != null)
                {
                    Encoding encoding = Encoding.GetEncoding(rsp.CharacterSet);
                    return GetResponseAsString(rsp, encoding);
                }
                else
                {
                    return string.Empty;
                }
            }
            else
            {
                return string.Empty;
            }
        }



        /// <summary>
        /// 把响应流转换为文本。
        /// </summary>
        /// <param name="rsp">响应流对象</param>
        /// <param name="encoding">编码方式</param>
        /// <returns>响应文本</returns>
        private static string GetResponseAsString(HttpWebResponse rsp, Encoding encoding)
        {
            StringBuilder result = new StringBuilder();
            Stream stream = null;
            StreamReader reader = null;
            try
            {
                // 以字符流的方式读取HTTP响应
                stream = rsp.GetResponseStream();
                reader = new StreamReader(stream, encoding);

                // 每次读取不大于256个字符，并写入字符串
                char[] buffer = new char[256];
                int readBytes = 0;
                while ((readBytes = reader.Read(buffer, 0, buffer.Length)) > 0)
                {
                    result.Append(buffer, 0, readBytes);
                }
            }
            catch (WebException webEx)
            {
                if (webEx.Status == WebExceptionStatus.Timeout)
                {
                    result = new StringBuilder();
                }
            }
            finally
            {
                // 释放资源
                if (reader != null) reader.Close();
                if (stream != null) stream.Close();
                if (rsp != null) rsp.Close();
            }

            return result.ToString();
        }

        /// <summary>
        /// 组装普通文本请求参数。
        /// </summary>
        /// <param name="parameters">Key-Value形式请求参数字典。</param>
        /// <returns>URL编码后的请求数据。</returns>
        private static string BuildPostData(IDictionary<string, string> parameters)
        {
            StringBuilder postData = new StringBuilder();
            bool hasParam = false;

            IEnumerator<KeyValuePair<string, string>> dem = parameters.GetEnumerator();
            while (dem.MoveNext())
            {
                string name = dem.Current.Key;
                string value = dem.Current.Value;
                // 忽略参数名或参数值为空的参数
                if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(value))
                {
                    if (hasParam)
                    {
                        postData.Append("&");
                    }

                    postData.Append(name);
                    postData.Append("=");
                    postData.Append(Uri.EscapeDataString(value));
                    hasParam = true;
                }
            }

            return postData.ToString();
        }


        /// <summary>
        /// 执行HTTP Post请求。
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="paramjsonStr">请求的参数的json字符串</param>
        /// <param name="proxyIP">代理IP</param>
        /// <param name="portNum">代理端口</param>
        /// <returns>HTTP响应</returns>
        public static string DoPost(string url, string paramjsonStr, string proxyIP, int portNum)
        {
            string beginProxy = proxyIP;
            int port = portNum;
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            System.Net.WebProxy proxy = new WebProxy(beginProxy, port);
            req.Proxy = proxy;
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            byte[] bs = Encoding.UTF8.GetBytes(paramjsonStr);
            req.ContentLength = bs.Length;
            using (Stream reqStream = req.GetRequestStream())
            {
                reqStream.Write(bs, 0, bs.Length);
                reqStream.Close();
            }

            Encoding encoding = Encoding.UTF8;
            string responseData = String.Empty;
            using (HttpWebResponse response = (HttpWebResponse)req.GetResponse())
            {

                using (StreamReader reader = new StreamReader(response.GetResponseStream(), encoding))
                {

                    responseData = reader.ReadToEnd().ToString();

                }

            }
            return responseData;
        }

        /// <summary>
        /// 执行HTTP Post请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="paramjsonStr">请求的参数的json字符串</param>
        /// <returns></returns>
        public static string DoPost(string url, string paramjsonStr)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            byte[] bs = Encoding.UTF8.GetBytes(paramjsonStr);
            req.ContentLength = bs.Length;
            using (Stream reqStream = req.GetRequestStream())
            {
                reqStream.Write(bs, 0, bs.Length);
                reqStream.Close();
            }
            Encoding encoding = Encoding.UTF8;
            string responseData = String.Empty;
            using (HttpWebResponse response = (HttpWebResponse)req.GetResponse())
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream(), encoding))
                {
                    responseData = reader.ReadToEnd().ToString();
                }

            }
            return responseData;
        }

        /// <summary>
        /// 指定Post地址使用Get 方式获取全部字符串
        /// </summary>
        /// <param name="url">请求后台地址</param>
        /// <returns></returns>
        public static string Post(string url, Dictionary<string, string> dic)
        {
            string result = "";
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            #region 添加Post 参数
            StringBuilder builder = new StringBuilder();
            int i = 0;
            foreach (var item in dic)
            {
                if (i > 0)
                    builder.Append("&");
                builder.AppendFormat("{0}={1}", item.Key, item.Value);
                i++;
            }
            byte[] data = Encoding.UTF8.GetBytes(builder.ToString());
            req.ContentLength = data.Length;
            using (Stream reqStream = req.GetRequestStream())
            {
                reqStream.Write(data, 0, data.Length);
                reqStream.Close();
            }
            #endregion
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            Stream stream = resp.GetResponseStream();
            //获取响应内容
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                result = reader.ReadToEnd();
            }
            return result;
        }

        /// <summary>
        /// 执行HTTP Post请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="paramjsonStr">请求的参数的json字符串</param>
        /// <returns></returns>
        //public static string DoPostIDictionary(string url, IDictionary<string, string> parameters)
        //{



        //    HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
        //    req.Method = "POST";
        //    req.ContentType = "application/x-www-form-urlencoded";
        //    byte[] bs = Encoding.UTF8.GetBytes(paramjsonStr);
        //    req.ContentLength = bs.Length;
        //    using (Stream reqStream = req.GetRequestStream())
        //    {
        //        reqStream.Write(bs, 0, bs.Length);
        //        reqStream.Close();
        //    }
        //    Encoding encoding = Encoding.UTF8;
        //    string responseData = String.Empty;
        //    using (HttpWebResponse response = (HttpWebResponse)req.GetResponse())
        //    {
        //        using (StreamReader reader = new StreamReader(response.GetResponseStream(), encoding))
        //        {
        //            responseData = reader.ReadToEnd().ToString();
        //        }

        //    }
        //    return responseData;
        //}

        #region  soap
        public static void DoSoap(string url,string userName,string password)
        {
            System.Uri wsUri = new System.Uri(url);
            HttpWebRequest myHttpWebRequest = (HttpWebRequest)HttpWebRequest.Create(wsUri);
            //要发送soap请求的内容，必须使用post方法传送数据
            myHttpWebRequest.Method = "POST";
            //缺省当前登录用户的身份凭据
           // myHttpWebRequest.Credentials = CredentialCache.DefaultCredentials;
            myHttpWebRequest.Credentials = new NetworkCredential(userName, password);
            //soap请求的内容
            string cont = @"<?xml version='1.0' encoding='UTF-8'?>
<soapenv:Envelope xmlns:soapenv='http://schemas.xmlsoap.org/soap/envelope/' xmlns:glob='http://sap.com/xi/SAPGlobal20/Global' xmlns:y62='http://0001092235-one-off.sap.com/Y628OLNKY' xmlns:a1o0='http://sap.com/xi/AP/CustomerExtension/BYD/A1O0O'>
   <soapenv:Header/>
   <soapenv:Body>
      <glob:SalesOrderBundleMaintainRequest_sync>
         <!--1 or more repetitions:-->
         <SalesOrder actionCode='01'>
            <!--Optional:-->
            <BuyerID>JD#11756319640</BuyerID>
            <!--Optional:-->
            <PostingDate>2015-12-27T19:57:15Z</PostingDate>
            <!--Optional:-->
            <Name languageCode='ZH'/>
            <!--Optional:-->
            <DataOriginTypeCode>4</DataOriginTypeCode>
            <!--Optional:-->
            <SalesAndServiceBusinessArea actionCode='01'>
               <!--Optional:-->
               <DistributionChannelCode>01</DistributionChannelCode>
            </SalesAndServiceBusinessArea>
            <!--Optional:-->
            <BillToParty actionCode='01'>
               <!--Optional:-->
               <PartyID>C00001536</PartyID>
            </BillToParty>
            <!--Optional:-->
            <AccountParty actionCode='01'>
               <!--Optional:-->
               <PartyID>C00001536</PartyID>
               <!--Zero or more repetitions:-->
               <ContactParty actionCode='01'>
                  <!--Optional:-->
                  <PartyID>C00001536</PartyID>
               </ContactParty>
            </AccountParty>
            <!--Optional:-->
            <PayerParty actionCode='01'>
               <!--Optional:-->
               <!--<PartyID>SA-BJ90900</PartyID>-->
            </PayerParty>
            <!--Optional:-->
            <ProductRecipientParty actionCode='01'>
               <!--Optional:-->
               <PartyID>C00001536</PartyID>
               <Address>
				<!--<Name>
                     <Name>
                        <FirstLineName>XXX 先生</FirstLineName>
                        <SecondLineName>ZZ</SecondLineName>
                        <ThirdLineName>XX</ThirdLineName>
                        <FourthLineName>YY</FourthLineName>
                     </Name>
                  </Name>-->
                  <PostalAddress>
                     <CountryCode>CN</CountryCode>
                     <RegionCode listID='CN'>070</RegionCode>
                     <CityName>大连市</CityName>
                     <StreetPostalCode>000000</StreetPostalCode>
                     <StreetPrefixName>辽宁大连市西岗区高尔基路212号-18-11-1（胜利花园A区）</StreetPrefixName>
                     <!--<StreetName>123</StreetName>-->
                     <StreetSuffixName></StreetSuffixName>
                     <!--<HouseID>23245</HouseID>-->
                  </PostalAddress>

               </Address>
            </ProductRecipientParty>
            <!--Optional:-->
            <EmployeeResponsibleParty actionCode='01'>
               <!--Optional:-->
               <PartyID>EKSHHHT001.05</PartyID>
            </EmployeeResponsibleParty>
            <!--Optional:-->
            <SellerParty actionCode='01'>
               <!--Optional:-->
               <PartyID>A-KS-R3A1-01</PartyID>

            </SellerParty>
            <!--Optional:-->
            <SalesUnitParty actionCode='01'>
               <!--Optional:-->
               <PartyID>A-KS-R3A1-010102</PartyID>

            </SalesUnitParty>
            <!--Optional:-->
            <DeliveryTerms actionCode='01'>
               <!--Optional:-->
               <PartialDeliveryControlCode>3</PartialDeliveryControlCode>
            </DeliveryTerms>
            <!--Optional:-->
            <PricingTerms actionCode='01'>
               <!--Optional:-->
               <CurrencyCode>CNY</CurrencyCode>
               <GrossAmountIndicator>true</GrossAmountIndicator>
            </PricingTerms>
            <!--Zero or more repetitions:-->
            <Item actionCode='01'>
            	<!--Optional:-->
            	<ID>1</ID>
               <!--Optional:-->
               <!--<PostingDate>2015-12-27T19:57:15Z</PostingDate>-->
               <!--Optional:-->
               <Description languageCode='ZH'>VK150 家用真空吸尘器(White)</Description>
               <!--Optional:-->
               <ItemProduct actionCode='01'>
                  <!--Optional:-->
                   <ProductID>61197</ProductID>
                  <!--Optional:-->
                  <ProductInternalID>61197</ProductInternalID>
                 
               </ItemProduct>
               <!--Optional:-->
               <ItemDeliveryTerms actionCode='01'>
                  <!--Optional:-->
                  <DeliveryPriorityCode>3</DeliveryPriorityCode>
                  <!--Optional:-->
                  <PartialDeliveryControlCode>3</PartialDeliveryControlCode>
               </ItemDeliveryTerms>
               <ItemScheduleLine actionCode='01'>
				<Quantity unitCode='EA'>1</Quantity>
                </ItemScheduleLine>
                <!--<VendorItemParty actionCode='01'>
				<PartyID>E67890</PartyID>
                </VendorItemParty>-->
               <!--Optional:-->
               <PriceAndTaxCalculationItem actionCode='01'>
                  <!--Optional:-->
                  <CountryCode>CN</CountryCode>
                   <!--Optional:-->
                <TaxationCharacteristicsCode listID='CN'>501</TaxationCharacteristicsCode>
                 
               </PriceAndTaxCalculationItem>
            </Item>
            <!--Optional:-->
            <CashDiscountTerms actionCode='01'>
               <!--Optional:-->
               <Code>1001</Code>
            </CashDiscountTerms>
            <!--Optional:-->
           <!--     <TextCollection actionCode='01'>
               Zero or more repetitions:
               <Text actionCode='01'>
              <TypeCode>10024</TypeCode>
                  <ContentText></ContentText> 
               </Text>
            </TextCollection>-->
            <!--Optional:-->
            <y62:InvoiceCustomerName>大连泰和医疗器械有限公司</y62:InvoiceCustomerName>
            <!--Optional:-->
            <y62:InvoiceCustomerTaxNumber></y62:InvoiceCustomerTaxNumber>
            <!--Optional:-->
            <y62:InvoiceCustomerAddressTel></y62:InvoiceCustomerAddressTel>
            <!--Optional:-->
            <y62:CustomerBankAccount></y62:CustomerBankAccount>
            <!--Optional:-->
            <y62:Drawer>施春燕</y62:Drawer>
            <!--Optional:-->
            <!--<y62:InvoiceType/>
            --><!--Optional:--><!--
            <y62:TypeofShipping/>
            --><!--Optional:--><!--
            <y62:SalesPlatform/>
		  --><!--Optional:--><!--
            <y62:BillingConten/>
            --><!--Optional:--><!--
             <a1o0:DDLX>101</a1o0:DDLX>
		   --><!--Optional:--><!--
            <y62:BillingAddress/>-->
         </SalesOrder>
      </glob:SalesOrderBundleMaintainRequest_sync>
   </soapenv:Body>
</soapenv:Envelope>";
            byte[] byteRequest = Encoding.UTF8.GetBytes(cont);
            myHttpWebRequest.ContentType = "application/soap+xml; charset=utf-8";
            myHttpWebRequest.ContentLength = byteRequest.Length;
            myHttpWebRequest.Timeout = 100000;
            //将soap请求的内容放入HttpWebRequest对象post方法的请求数据部分

            Stream newStream = myHttpWebRequest.GetRequestStream();

            newStream.Write(byteRequest, 0, byteRequest.Length);

            //发送请求
            HttpWebResponse myWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();

            //将收到的回应从Stream转换成string

            newStream = myWebResponse.GetResponseStream();

            byte[] byteResponse = new byte[myWebResponse.ContentLength];

            newStream.Read(byteResponse, 0, (int)myWebResponse.ContentLength);

            //str里面就是返回soap回应的字符串了

            string str = Encoding.UTF8.GetString(byteResponse);
            myWebResponse.Close();

        }




        #endregion









    }
}
