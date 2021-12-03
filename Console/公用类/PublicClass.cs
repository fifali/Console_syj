using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using System.Data;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Net;
using System.Web.Script.Serialization;
using System.Runtime.Serialization.Json;
using Newtonsoft.Json;
namespace ConsoleHydee
{
    public static class PublicClass
    {

        #region B2B请求消息格式化
        public class B2BReqJSConverter : JavaScriptConverter
        {
            public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
            {
                ProductList node = new ProductList();
                object value = null;
                if (dictionary.TryGetValue("carname", out value))
                    node.orderItemId = (string)value;
                if (dictionary.TryGetValue("productCode", out value))
                    node.productCode = (string)value;
                if (dictionary.TryGetValue("amount", out value))
                    node.amount = (string)value;
                if (dictionary.TryGetValue("price", out value))
                    node.price = (string)value;
                if (dictionary.TryGetValue("money", out value))
                    node.money = (string)value;
                if (dictionary.TryGetValue("batchNumber", out value))
                    node.batchNumber = (string)value;
                return node;
            }
            public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
            {
                Dictionary<string, object> dic = new Dictionary<string, object>();
                var node = obj as ProductList;
                if (node == null)
                    return null;
                if (!string.IsNullOrEmpty(node.orderItemId))
                    dic.Add("orderItemId", node.orderItemId);
                if (!string.IsNullOrEmpty(node.productCode))
                    dic.Add("productCode", node.productCode);
                if (!string.IsNullOrEmpty(node.amount))
                    dic.Add("amount", node.amount);
                if (!string.IsNullOrEmpty(node.price))
                    dic.Add("price", node.price);
                if (!string.IsNullOrEmpty(node.money))
                    dic.Add("money", node.money);
                if (!string.IsNullOrEmpty(node.batchNumber))
                    dic.Add("batchNumber", node.batchNumber);
                return dic;
            }
            public override IEnumerable<Type> SupportedTypes
            {
                get
                {
                    return new Type[] { typeof(ProductList) };
                }
            }
        }
        #endregion

        //#region B2B返回消息格式化
        //public class RetData_B2BList : JavaScriptConverter
        //{
        //    public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        //    {
        //        RetData_B2BList node = new RetData_B2BList();
        //        object value = null;
        //        if (dictionary.TryGetValue("returnCode", out value))
        //            node.Deserialize = (string)value;
        //        if (dictionary.TryGetValue("productCode", out value))
        //            node.productCode = (string)value;
        //        if (dictionary.TryGetValue("amount", out value))
        //            node.amount = (string)value;
        //        if (dictionary.TryGetValue("price", out value))
        //            node.price = (string)value;
        //        if (dictionary.TryGetValue("money", out value))
        //            node.money = (string)value;
        //        if (dictionary.TryGetValue("batchNumber", out value))
        //            node.batchNumber = (string)value;
        //        return node;
        //    }
        //    public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        //    {
        //        Dictionary<string, object> dic = new Dictionary<string, object>();
        //        var node = obj as RetData_B2BList;
        //        if (node == null)
        //            return null;
        //        if (!string.IsNullOrEmpty(node.returnCode))
        //            dic.Add("returnCode", node.returnCode);
        //        if (!string.IsNullOrEmpty(node.productCode))
        //            dic.Add("productCode", node.productCode);
        //        if (!string.IsNullOrEmpty(node.amount))
        //            dic.Add("amount", node.amount);
        //        if (!string.IsNullOrEmpty(node.price))
        //            dic.Add("price", node.price);
        //        if (!string.IsNullOrEmpty(node.money))
        //            dic.Add("money", node.money);
        //        if (!string.IsNullOrEmpty(node.batchNumber))
        //            dic.Add("batchNumber", node.batchNumber);
        //        return dic;
        //    }
        //    public override IEnumerable<Type> SupportedTypes
        //    {
        //        get
        //        {
        //            return new Type[] { typeof(RetData_B2BList) };
        //        }
        //    }
        //}
        //#endregion

        #region 字符处理
        public static T parse<T>(string JsonString)
        {
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(JsonString)))
            {
                return (T)new DataContractJsonSerializer(typeof(T)).ReadObject(ms);
            }
        }

        public static string stringify(object JsonObject)
        {
            using (var ms = new MemoryStream())
            {
                new DataContractJsonSerializer(JsonObject.GetType()).WriteObject(ms, JsonObject);
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }

        public static List<T> JsonStringToList<T>(string JsonStr)
        {
            JavaScriptSerializer Serializer = new JavaScriptSerializer();
            List<T> objs = Serializer.Deserialize<List<T>>(JsonStr);
            return objs;
        }

        public static T Deserialize<T>(string Json)
        {
            T obj = Activator.CreateInstance<T>();
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(Json)))
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
                return (T)serializer.ReadObject(ms);
            }
        }

        public static string String2Base64(string str)
        {
            byte[] byteBody = Encoding.UTF8.GetBytes(str);
            return (Convert.ToBase64String(byteBody));
        }
        #endregion

        #region 将datatable转换为json
        public static string Dtb2Json(DataTable dtb, string appid, string secret, string timestamp, string publickey, ref string apisign, ref string ls_biz)
        {
            string ls_sign_before1 = "";
            string ls_sign_before2 = "";
            string sign = "";
            string ls_content = "";
            string ls_content_old = "";
            string ls_memid = "";
            JavaScriptSerializer jss = new JavaScriptSerializer();
            System.Collections.ArrayList dic = new System.Collections.ArrayList();
            foreach (DataRow dr in dtb.Rows)
            {
                System.Collections.Generic.Dictionary<string, object> drow = new System.Collections.Generic.Dictionary<string, object>();
                foreach (DataColumn dc in dtb.Columns)
                {
                    if (dc.ColumnName == "MERID")
                    { dc.ColumnName = "merId";
                        ls_memid = dr[dc.ColumnName].ToString(); }
                    if (dc.ColumnName == "SUBAPPID")
                    { dc.ColumnName = "subAppId";
                        //if (string.IsNullOrEmpty(dr[dc.ColumnName].ToString()))
                        //{
                        //    continue;
                        //}
                    }
                    if (dc.ColumnName == "ORDERID")
                    { dc.ColumnName = "orderId"; }
                    if (dc.ColumnName == "AUTHCODE")
                    { dc.ColumnName = "authCode"; }
                    if (dc.ColumnName == "USERID")
                    { dc.ColumnName = "userId";
                        //if (string.IsNullOrEmpty(dr[dc.ColumnName].ToString()))
                        //{
                        //    continue;
                        //}
                    }
                    if (dc.ColumnName == "TERMID")
                    { dc.ColumnName = "termId"; }
                    if (dc.ColumnName == "NOTIFYURL")
                    { dc.ColumnName = "notifyUrl"; }
                    if (dc.ColumnName == "TXNAMT")
                    { dc.ColumnName = "txnAmt"; }
                    if (dc.ColumnName == "CURRENCYCODE")
                    { dc.ColumnName = "currencyCode";
                        //if (string.IsNullOrEmpty(dr[dc.ColumnName].ToString()))
                        //{
                        //    continue;
                        //}
                    }
                    if (dc.ColumnName == "BODY")
                    { dc.ColumnName = "body";
                        //if (string.IsNullOrEmpty(dr[dc.ColumnName].ToString()))
                        //{
                        //    continue;
                        //}
                    }
                    if (dc.ColumnName == "MCHRESERVED")
                    { dc.ColumnName = "mchReserved";
                        //if (string.IsNullOrEmpty(dr[dc.ColumnName].ToString()))
                        //{
                        //    continue;
                        //}
                    }
                    if (dc.ColumnName == "TRADESCENE")
                    { dc.ColumnName = "tradeScene";
                        if (string.IsNullOrEmpty(dr[dc.ColumnName].ToString()))
                        {
                            continue;
                        }
                    }
                    if (dc.ColumnName == "IDENTITY")
                    { dc.ColumnName = "identity";
                        if (string.IsNullOrEmpty(dr[dc.ColumnName].ToString()))
                        {
                            continue;
                        }
                    }
                    if (dc.ColumnName == "POLICYNO")
                    { dc.ColumnName = "policyNo";
                        if (string.IsNullOrEmpty(dr[dc.ColumnName].ToString()))
                                {
                            continue;
                        }}
                    if (dr[dc.ColumnName].ToString() == "" || dr[dc.ColumnName].ToString() == null)
                    {
                        drow.Add(dc.ColumnName, "");
                    }
                    else
                    {
                        drow.Add(dc.ColumnName, dr[dc.ColumnName]);
                    }
                }
                dic.Add(drow);
            }
            ls_content = (jss.Serialize(dic).Replace("[", "")).Replace("]", "");
            ls_content_old = ls_content;
            ls_content = ls_content.Replace("\"", "\\\"");
            ls_content = ls_content.Replace("@@@@@@@@", "\\\\\\\"");
            //序列化  
            ls_sign_before1 = "{\"" + dtb.TableName.ToString() + "\":\"" + ls_content + "\",\"encoding\":\"UTF-8\",";
            ls_sign_before2 = "\"signMethod\":\"01\"," + "\"version\":\"0.0.1\"" + "}";
            ls_biz = "biz_content=" + ls_content_old + "&encoding=UTF-8&signMethod=01&version=0.0.1";
            ls_biz = ls_biz.Replace("@@@@@@@@", "\\\"");
            if (!File.Exists(Environment.CurrentDirectory + "\\DBConn\\"+ ls_memid + ".pem"))
            {
                return "@@";
            }
            sign = RSASign(ls_biz, ls_memid);

            apisign = "appid=" + appid + "&secret=" + secret + "&sign=" + sign + "&timestamp=" + timestamp;
            apisign = UserMd5(apisign);
            return ls_sign_before1 + "\"sign\":\"" + sign + "\"," + ls_sign_before2;
        }
        #endregion

        #region 将json转换为DataTable
        public static DataTable JsonToDataTable(string strJson, string tablename)
        {
            //转换json格式
            strJson = strJson.Replace(",\"", "*\"").Replace("\":", "\"#").ToString();
            //取出表名   
            var rg = new Regex(@"(?<={)[^:]+(?=:\[)", RegexOptions.IgnoreCase);
            string strName = rg.Match(strJson).Value;
            DataTable tb = null;
            //去除表名   
            strJson = strJson.Substring(strJson.IndexOf("[") + 1);
            strJson = strJson.Substring(0, strJson.IndexOf("]"));

            //获取数据   
            rg = new Regex(@"(?<={)[^}]+(?=})");
            MatchCollection mc = rg.Matches(strJson);
            for (int i = 0; i < mc.Count; i++)
            {
                string strRow = mc[i].Value;
                string[] strRows = strRow.Split('*');

                //创建表   
                if (tb == null)
                {
                    tb = new DataTable();
                    tb.TableName = tablename;
                    foreach (string str in strRows)
                    {
                        var dc = new DataColumn();
                        string[] strCell = str.Split('#');

                        if (strCell[0].Substring(0, 1) == "\"")
                        {
                            int a = strCell[0].Length;
                            dc.ColumnName = strCell[0].Substring(1, a - 2);
                        }
                        else
                        {
                            dc.ColumnName = strCell[0];
                        }
                        tb.Columns.Add(dc);
                    }
                    tb.AcceptChanges();
                }

                //增加内容   
                DataRow dr = tb.NewRow();
                for (int r = 0; r < strRows.Length; r++)
                {
                    dr[r] = strRows[r].Split('#')[1].Trim().Replace("，", ",").Replace("：", ":").Replace("\"", "");
                }
                tb.Rows.Add(dr);
                tb.AcceptChanges();
            }
            return tb;
        }
        #endregion

        /// <summary>
        /// C#反射遍历对象属性
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="model">对象</param>
        public static DataTable ForeachClassProperties<T>(T model, DataTable dtinput, string ls_guid, string ls_func)
        {
            string ls_priname = "";
            DataTable dt = null;
            DataTable dtt = null;
            Type t = model.GetType();
            PropertyInfo[] PropertyList = t.GetProperties();
            if (dtinput == null || dtinput.Rows.Count < 1)
            {
                dt = new DataTable(t.Name);
                foreach (PropertyInfo item in PropertyList)
                {
                    dt.Columns.Add(item.Name, System.Type.GetType(HConvertByType(item.PropertyType)));
                    object[] objArray = item.GetCustomAttributes(false);
                    if (objArray.Length > 0)
                    {
                        if (((System.Runtime.Serialization.DataMemberAttribute)objArray[0]).IsRequired == true)
                            ls_priname = item.Name;
                        dt.PrimaryKey = new DataColumn[] { dt.Columns[ls_priname] };
                    }
                }
                dtt = dt;
            }
            else
            {
                dtt = dtinput;
                ls_priname = dtinput.PrimaryKey[0].ToString();
            }
            DataRow dr = dtt.NewRow();
            foreach (PropertyInfo item in PropertyList)
            {
                string name = item.Name;
                object value = item.GetValue(model, null);
                if (value == null)
                {
                    dr[name] = DBNull.Value;
                }
                else
                {
                    dr[name] = value;
                }
            }
            if (string.IsNullOrEmpty(dr[ls_priname].ToString()))
                dr[ls_priname] = ls_guid;
            dtt.Rows.Add(dr);
            return dtt;
        }
        #region 根据TYPE进行转换
        public static string HConvertByType(Type type)
        {
            Type valtype = type;
            string pptypeName = valtype.Name;
            string pname = valtype.Name;
            pptypeName = valtype.FullName;
            if (pname.LastIndexOf("Int") != -1)
            {
                return pptypeName;
            }
            else if (pname == "DateTime")
            {
                return pptypeName;
            }
            else if (pname == "String")
            {
                return pptypeName;
            }
            else if (pname == "Decimal")
            {
                return pptypeName;
            }
            else if (pname == "Nullable`1")
            {
                if (pptypeName.LastIndexOf("Int") != -1)
                {
                    return "System.Int32";
                }
                else if (pptypeName.LastIndexOf("DateTime") != -1)
                {
                    return "System.DateTime";
                }
                else if (pptypeName.LastIndexOf("String") != -1)
                {
                    return "System.String";
                }
                else if (pptypeName.LastIndexOf("Decimal") != -1)
                {
                    return "System.Decimal";
                }
            }
            return "NoDefintType";
        }
        #endregion

        #region 发送短信
        public static string GetHtmlFromUrl(string url)
        {
            string strRet = null;
            if (url == null || url.Trim().ToString() == "")
            {
                return "短信通讯地址为空";
            }
            string targeturl = url.Trim().ToString();
            try
            {
                HttpWebRequest hr = (HttpWebRequest)WebRequest.Create(targeturl);
                hr.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1)";
                hr.Method = "GET";
                hr.Timeout = 30 * 60 * 1000;
                WebResponse hs = hr.GetResponse();
                Stream sr = hs.GetResponseStream();
                StreamReader ser = new StreamReader(sr, Encoding.Default);
                strRet = ser.ReadToEnd();
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
            return "TRUE";
        }
        #endregion

        #region 文件处理
        public static string UpFile(byte[] bt, string FilePath, string FileName)//上传
        {
            try
            {
                if (Directory.Exists(FilePath) == false)//如果不存在就创建file文件夹
                {
                    Directory.CreateDirectory(FilePath);
                }
                //else
                //{
                //    if (File.Exists(FileName + "\\" + FileName))
                //    {
                //        //存在文件
                //        File.Delete(FileName + "\\" + FileName);
                //    }
                //}
                MemoryStream m = new MemoryStream(bt);
                using (FileStream fs = File.Open(FilePath + "\\" + FileName, FileMode.Create))
                {
                    m.WriteTo(fs);
                    m.Close();
                    fs.Close();
                    return "TRUE";
                }
            }
            catch (Exception xx) { return xx.Message; }
        }

        public static byte[] DownFile(string FilePath)//下载
        {
            try
            {
                using (FileStream fs = File.Open(FilePath, FileMode.Open))
                {
                    byte[] bt = new byte[fs.Length];
                    fs.Read(bt, 0, Convert.ToInt32(fs.Length));
                    fs.Close();
                    return bt;
                }
            }
            catch { return null; }
        }
        #endregion

        #region 随机生成码
        private static char[] constant =
                      {
                        '0','1','2','3','4','5','6','7','8','9',
                        'A','B','C','D','E','F','G','H','J','K','L','M','N','P','Q','R','S','T','U','V','W','X','Y'
                      };

        public static string GenerateRandomNumber(long Length)
        {
            System.Text.StringBuilder newRandom = new System.Text.StringBuilder(33);
            Random rd = new Random();
            for (long i = 0; i < Length; i++)
            {
                newRandom.Append(constant[rd.Next(33)]);
            }
            return newRandom.ToString();
        }
        #endregion

        #region 字符串拼接
        public static string JsonChange(ArrayList ls_str)
        {
            string ls_json;
            ls_json = ls_str[0].ToString();
            for (int i = 1; i < ls_str.Count; i++)
            {
                ls_json = ls_json.Trim().Substring(0, ls_json.Trim().Length - 1) + "," + ls_str[i].ToString().Trim().Substring(1);
            }
            return ls_json;
        }
        #endregion

        #region 解析通讯密匙
        /*
		 * / <summary>
		 * / MD5 32位加密
		 * / </summary>
		 * / <param name="str"></param>
		 * / <returns></returns>
		 */
        public static string UserMd5(string str)
        {
            string cl = str;
            string pwd = "";
            MD5 md5 = MD5.Create(); /* 实例化一个md5对像 */
            /* 加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择　 */
            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(cl));
            /* 通过使用循环，将字节类型的数组转换为字符串，此字符串是常规字符格式化所得 */
            for (int i = 0; i < s.Length; i++)
            {
                /* 将得到的字符串使用十六进制类型格式。格式后的字符是小写的字母，如果使用大写（X）则格式后的字符是大写字符 */
                pwd = pwd + s[i].ToString("X2");
            }
            return (pwd.ToLower());
        }

        #endregion

        #region sha256加密
        public static string sha256(string data)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            byte[] hash = SHA256Managed.Create().ComputeHash(bytes);

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                builder.Append(hash[i].ToString("X2"));
            }

            return builder.ToString();
        }
        #endregion

        #region base64
        ///编码
        public static string EncodeBase64(string code_type, string code)
        {
            string encode = "";
            byte[] bytes = Encoding.GetEncoding(code_type).GetBytes(code);
            try
            {
                encode = Convert.ToBase64String(bytes);
            }
            catch
            {
                encode = code;
            }
            return encode;
        }
        ///解码
        public static string DecodeBase64(string code_type, string code)
        {
            string decode = "";
            byte[] bytes = Convert.FromBase64String(code);
            try
            {
                decode = Encoding.GetEncoding(code_type).GetString(bytes);
            }
            catch
            {
                decode = code;
            }
            return decode;
        }
        #endregion

        #region RSA2加密
        public static string RSA2(string publickey, string plaintext)
        {
            RSACryptoServiceProvider rsaProvider = DecodeRSAPrivateKey(plaintext);
            String PrivateKey = rsaProvider.ToXmlString(true);

            RSACryptoServiceProvider rsa2 = new RSACryptoServiceProvider();
            rsa2.FromXmlString(PrivateKey);

            byte[] data = Encoding.UTF8.GetBytes(plaintext);
            byte[] endata = rsa2.SignData(data, "SHA256");
            return Convert.ToBase64String(endata);

            //rsa2开始加密   
            //byte[] cipherbytes;
            //cipherbytes = rsa2.Encrypt(Encoding.UTF8.GetBytes(plaintext),false);
            //return System.Text.Encoding.Default.GetString(cipherbytes);
        }

        public static RSACryptoServiceProvider DecodeRSAPrivateKey(string priKey)
        {
            //var privkey = Convert.FromBase64String(priKey);
            byte[] MODULUS, E, D, P, Q, DP, DQ, IQ;


            // ---------  Set up stream to decode the asn.1 encoded RSA private key  ------
            //MemoryStream mem = new MemoryStream(privkey);
            //BinaryReader binr = new BinaryReader(mem);
            string path = @"D:\\project\\ConsoleApplication1\\li_pri.der";
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);

            BinaryReader binr = new BinaryReader(fs);    //wrap Memory Stream with BinaryReader for easy reading
            byte bt = 0;
            ushort twobytes = 0;
            int elems = 0;
            try
            {
                twobytes = binr.ReadUInt16();
                if (twobytes == 0x8130) //data read as little endian order (actual data order for Sequence is 30 81)
                    binr.ReadByte();        //advance 1 byte
                else if (twobytes == 0x8230)
                    binr.ReadInt16();       //advance 2 bytes
                else
                    return null;


                twobytes = binr.ReadUInt16();
                if (twobytes != 0x0102) //version number
                    return null;
                bt = binr.ReadByte();
                if (bt != 0x00)
                    return null;




                //------  all private key components are Integer sequences ----
                elems = GetIntegerSize(binr);
                MODULUS = binr.ReadBytes(elems);


                elems = GetIntegerSize(binr);
                E = binr.ReadBytes(elems);


                elems = GetIntegerSize(binr);
                D = binr.ReadBytes(elems);


                elems = GetIntegerSize(binr);
                P = binr.ReadBytes(elems);


                elems = GetIntegerSize(binr);
                Q = binr.ReadBytes(elems);


                elems = GetIntegerSize(binr);
                DP = binr.ReadBytes(elems);


                elems = GetIntegerSize(binr);
                DQ = binr.ReadBytes(elems);


                elems = GetIntegerSize(binr);
                IQ = binr.ReadBytes(elems);


                // ------- create RSACryptoServiceProvider instance and initialize with public key -----
                RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
                RSAParameters RSAparams = new RSAParameters();
                RSAparams.Modulus = MODULUS;
                RSAparams.Exponent = E;
                RSAparams.D = D;
                RSAparams.P = P;
                RSAparams.Q = Q;
                RSAparams.DP = DP;
                RSAparams.DQ = DQ;
                RSAparams.InverseQ = IQ;
                RSA.ImportParameters(RSAparams);




                return RSA;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + e.StackTrace);
                return null;
            }
            finally
            {
                binr.Close();
            }
        }

        public static string RSASign(string data,string ls_memid)
        {
            string signType = "RSA2";
            RSACryptoServiceProvider rsaCsp = LoadCertificateFile(PrivateKey+ ls_memid+ ".pem", signType);
            byte[] dataBytes = null;
            dataBytes = Encoding.UTF8.GetBytes(data);

            if ("RSA2".Equals(signType))
            {
                byte[] signatureBytes = rsaCsp.SignData(dataBytes, "SHA256");
                return Convert.ToBase64String(signatureBytes);
            }
            else
            {
                byte[] signatureBytes = rsaCsp.SignData(dataBytes, "SHA1");
                return Convert.ToBase64String(signatureBytes);
            }
        }
        public static string PrivateKey
        {
            get
            {
                //return AppDomain.CurrentDomain.BaseDirectory + "/Keys/rsa_private_key.pem";
                return Environment.CurrentDirectory + "\\DBConn\\";
            }
        }
        private static RSACryptoServiceProvider LoadCertificateFile(string filename, string signType)
        {
            using (System.IO.FileStream fs = System.IO.File.OpenRead(filename))
            {
                byte[] data = new byte[fs.Length];
                byte[] res = null;
                fs.Read(data, 0, data.Length);
                if (data[0] != 0x30)
                {
                    res = GetPem("RSA PRIVATE KEY", data);
                }
                try
                {
                    RSACryptoServiceProvider rsa = DecodeRSAPrivateKey(res, signType);
                    return rsa;
                }
                catch
                {
                    throw;
                }

            }
        }
        private static byte[] GetPem(string type, byte[] data)
        {
            string pem = Encoding.UTF8.GetString(data);
            string header = String.Format("-----BEGIN {0}-----\\n", type);
            string footer = String.Format("-----END {0}-----", type);
            int start = pem.IndexOf(header) + header.Length;
            int end = pem.IndexOf(footer, start);
            string base64 = pem.Substring(start, (end - start));

            return Convert.FromBase64String(base64);
        }

        private static RSACryptoServiceProvider DecodeRSAPrivateKey(byte[] privkey, string signType)
        {
            byte[] MODULUS, E, D, P, Q, DP, DQ, IQ;

            // --------- Set up stream to decode the asn.1 encoded RSA private key ------
            MemoryStream mem = new MemoryStream(privkey);
            BinaryReader binr = new BinaryReader(mem);  //wrap Memory Stream with BinaryReader for easy reading
            byte bt = 0;
            ushort twobytes = 0;
            int elems = 0;
            try
            {
                twobytes = binr.ReadUInt16();
                if (twobytes == 0x8130) //data read as little endian order (actual data order for Sequence is 30 81)
                    binr.ReadByte();    //advance 1 byte
                else if (twobytes == 0x8230)
                    binr.ReadInt16();    //advance 2 bytes
                else
                    return null;

                twobytes = binr.ReadUInt16();
                if (twobytes != 0x0102) //version number
                    return null;
                bt = binr.ReadByte();
                if (bt != 0x00)
                    return null;


                //------ all private key components are Integer sequences ----
                elems = GetIntegerSize(binr);
                MODULUS = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                E = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                D = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                P = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                Q = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                DP = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                DQ = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                IQ = binr.ReadBytes(elems);


                // ------- create RSACryptoServiceProvider instance and initialize with public key -----
                CspParameters CspParameters = new CspParameters();
                CspParameters.Flags = CspProviderFlags.UseMachineKeyStore;

                int bitLen = 1024;
                if ("RSA2".Equals(signType))
                {
                    bitLen = 2048;
                }

                RSACryptoServiceProvider RSA = new RSACryptoServiceProvider(bitLen, CspParameters);
                RSAParameters RSAparams = new RSAParameters();
                RSAparams.Modulus = MODULUS;
                RSAparams.Exponent = E;
                RSAparams.D = D;
                RSAparams.P = P;
                RSAparams.Q = Q;
                RSAparams.DP = DP;
                RSAparams.DQ = DQ;
                RSAparams.InverseQ = IQ;
                RSA.ImportParameters(RSAparams);
                return RSA;
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
                return null;
            }
            finally
            {
                binr.Close();
            }
        }
        private static int GetIntegerSize(BinaryReader binr)
        {
            byte bt = 0;
            byte lowbyte = 0x00;
            byte highbyte = 0x00;
            int count = 0;
            bt = binr.ReadByte();
            if (bt != 0x02)     //expect integer
                return 0;
            bt = binr.ReadByte();

            if (bt == 0x81)
                count = binr.ReadByte();    // data size in next byte
            else
                if (bt == 0x82)
            {
                highbyte = binr.ReadByte(); // data size in next 2 bytes
                lowbyte = binr.ReadByte();
                byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };
                count = BitConverter.ToInt32(modint, 0);
            }
            else
            {
                count = bt;     // we already have the data size
            }

            while (binr.ReadByte() == 0x00)
            {   //remove high order zeros in data
                count -= 1;
            }
            binr.BaseStream.Seek(-1, SeekOrigin.Current);       //last ReadByte wasn't a removed zero, so back up a byte
            return count;
        }

        public static string GetSignContent(IDictionary<string, string> parameters)
        {
            IDictionary<string, string> sortedParams = new SortedDictionary<string, string>(parameters);
            IEnumerator<KeyValuePair<string, string>> dem = sortedParams.GetEnumerator();

            StringBuilder query = new StringBuilder("");
            while (dem.MoveNext())
            {
                string key = dem.Current.Key;
                string value = dem.Current.Value;
                if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
                {
                    query.Append(key).Append("=").Append(value).Append("&");
                }
            }
            string content = query.ToString().Substring(0, query.Length - 1);
            //content = RSASign(content);
            return content;
        }

        #endregion

        # region 获取时间戳
        public static string GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }
        #endregion
    }
    /// <summary>
    /// Json帮助类
    /// </summary>
    public static class JsonHelper
    {
        /// <summary>
        /// 将对象序列化为JSON格式
        /// </summary>
        /// <param name="o">对象</param>
        /// <returns>json字符串</returns>
        public static string SerializeObject(object o)
        {
            string json = JsonConvert.SerializeObject(o);
            return json;
        }

        /// <summary>
        /// 解析JSON字符串生成对象实体
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="json">json字符串(eg.{"ID":"112","Name":"石子儿"})</param>
        /// <returns>对象实体</returns>
        public static T DeserializeJsonToObject<T>(string json) where T : class
        {
            JsonSerializer serializer = new JsonSerializer();
            StringReader sr = new StringReader(json);
            object o = serializer.Deserialize(new JsonTextReader(sr), typeof(T));
            T t = o as T;
            return t;
        }

        /// <summary>
        /// 解析JSON数组生成对象实体集合
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="json">json数组字符串(eg.[{"ID":"112","Name":"石子儿"}])</param>
        /// <returns>对象实体集合</returns>
        public static List<T> DeserializeJsonToList<T>(string json) where T : class
        {
            JsonSerializer serializer = new JsonSerializer();
            StringReader sr = new StringReader(json);
            object o = serializer.Deserialize(new JsonTextReader(sr), typeof(List<T>));
            List<T> list = o as List<T>;
            return list;
        }

        /// <summary>
        /// 反序列化JSON到给定的匿名对象.
        /// </summary>
        /// <typeparam name="T">匿名对象类型</typeparam>
        /// <param name="json">json字符串</param>
        /// <param name="anonymousTypeObject">匿名对象</param>
        /// <returns>匿名对象</returns>
        public static T DeserializeAnonymousType<T>(string json, T anonymousTypeObject)
        {
            T t = JsonConvert.DeserializeAnonymousType(json, anonymousTypeObject);
            return t;
        }
    }
}