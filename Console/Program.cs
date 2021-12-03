using System;
using System.Data;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Configuration;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using swiftpass.utils;
using System.Web.Script.Serialization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Linq;
using System.Security.Cryptography;
using System.IO;

namespace ConsoleHydee
{
    class Program
    {
        static HttpListener httpobj;
        static PublicBll _bll;

        static void Main(string[] args)
        {
            Console.WriteLine("");
            PublicBll bll = new PublicBll();
            //提供一个简单的、可通过编程方式控制的 HTTP 协议侦听器。此类不能被继承。
            httpobj = new HttpListener();
            //定义url及端口号，通常设置为配置文件
            //string url = ConfigurationManager.AppSettings["Url"];
            string url = "";
            bll.geturlParms(Environment.CurrentDirectory + "\\DBConn\\001.xml", out url);
            Console.WriteLine($"URL：{url}\r\n");
            httpobj.Prefixes.Add(url);
            //启动监听器
            httpobj.Start();
            //异步监听客户端请求，当客户端的网络请求到来时会自动执行Result委托
            //该委托没有返回值，有一个IAsyncResult接口的参数，可通过该参数获取context对象
            httpobj.BeginGetContext(Result, null);
            Console.WriteLine($"服务端初始化完毕，正在等待客户端请求,版本2.0.2时间：{DateTime.Now.ToString()}\r\n");
            Console.ReadKey();
        }
        public static string WePaySign(IDictionary<string, string> InDict, string TenPayV3_Key)
        {
            string[] arrKeys = InDict.Keys.ToArray();
            Array.Sort(arrKeys, string.CompareOrdinal);  //参数名ASCII码从小到大排序；0,A,B,a,b;

            var StrA = new StringBuilder();

            foreach (var key in arrKeys)
            {
                string value = InDict[key];
                if (!String.IsNullOrEmpty(value)) //空值不参与签名
                {
                    StrA.Append(key + "=")
                       .Append(value + "&");
                }
            }


            //foreach (var item in InDict.OrderBy(x => x.Key))//参数名字典序；0,A,a,B,b;
            //{
            //    if(!String.IsNullOrEmpty(item.Value)) //空值不参与签名
            //    {
            //        StrA.Append(item.Key + "=")
            //           .Append(item.Value + "&");
            //    }
            //}

            StrA.Append("key=" + TenPayV3_Key); //注：key为商户平台设置的密钥key
            //return StrFormat.GetMd5Hash(StrA.ToString()).ToUpper();
            return GetMD5FromString(StrA.ToString()).ToUpper();
        }
        /// <summary>
        /// 将json字符串反序列化为字典类型
        /// </summary>
        /// <typeparam name="TKey">字典key</typeparam>
        /// <typeparam name="TValue">字典value</typeparam>
        /// <param name="jsonStr">json字符串</param>
        /// <returns>字典数据</returns>
        public static Dictionary<TKey, TValue> DeserializeStringToDictionary<TKey, TValue>(string jsonStr)
        {
            if (string.IsNullOrEmpty(jsonStr))
                return new Dictionary<TKey, TValue>();

            Dictionary<TKey, TValue> jsonDict = JsonConvert.DeserializeObject<Dictionary<TKey, TValue>>(jsonStr);

            return jsonDict;

        }
        /// <summary>
        /// 计算字符串的MD5值
        /// </summary>
        /// <param name="msg">要计算的字符串</param>
        /// <returns></returns>
        private static string GetMD5FromString(string msg)
        {

            //1.创建一个用来计算MD5值的类的对象
            using (MD5 md5 = MD5.Create())
            {

                //把字符串转换为byte[]
                //注意：如果字符串中包含汉字，则这里会把汉字使用utf-8编码转换为byte[]，当其他地方
                //计算MD5值的时候，如果对汉字使用了不同的编码，则同样的汉字生成的byte[]是不一样的，所以计算出的MD5值也就不一样了。
                byte[] msgBuffer = Encoding.Default.GetBytes(msg);

                //2.计算给定字符串的MD5值
                //返回值就是就算后的MD5值,如何把一个长度为16的byte[]数组转换为一个长度为32的字符串：就是把每个byte转成16进制同时保留2位即可。
                byte[] md5Buffer = md5.ComputeHash(msgBuffer);
                md5.Clear();//释放资源

                StringBuilder sbMd5 = new StringBuilder();
                for (int i = 0; i < md5Buffer.Length; i++)
                {
                    sbMd5.Append(md5Buffer[i].ToString("x2"));
                }
                return sbMd5.ToString();

            }

        }

        private static void Result(IAsyncResult ar)
        {
            //当接收到请求后程序流会走到这里

            //继续异步监听
            httpobj.BeginGetContext(Result, null);
            var guid = Guid.NewGuid().ToString();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"接到新的请求:{guid},时间：{DateTime.Now.ToString()}\r\n");
            //获得context对象
            var context = httpobj.EndGetContext(ar);
            var request = context.Request;
            var response = context.Response;
            context.Response.ContentType = "text/plain;charset=UTF-8";//告诉客户端返回的ContentType类型为纯文本格式，编码为UTF-8
            context.Response.AddHeader("Content-type", "text/plain");//添加响应头信息
            context.Response.ContentEncoding = Encoding.UTF8;
            string returnObj = null;//定义返回客户端的信息
            if (request.HttpMethod == "POST" && request.InputStream != null)
            {
                //处理客户端发送的请求并返回处理信息
                returnObj = HandleRequest(request, response);
            }
            else
            {
                returnObj = $"不是post请求或者传过来的数据为空\r\n";
            }
            var returnByteArr = Encoding.UTF8.GetBytes(returnObj);//设置客户端返回信息的编码
            try
            {
                using (var stream = response.OutputStream)
                {
                    //把处理信息返回到客户端
                    stream.Write(returnByteArr, 0, returnByteArr.Length);
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"网络蹦了：{ex.ToString()}\r\n");
            }
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"请求处理完成：{guid},时间：{ DateTime.Now.ToString()}\r\n");
        }
        /// <summary>
        ///   替换部分字符串
        /// </summary>
        /// <param name="sPassed">需要替换的字符串</param>
        /// <returns></returns>
        public static string ReplaceString(string JsonString)
        {
            if (JsonString == null) { return JsonString; }
            if (JsonString.Contains("\\"))
            {
                JsonString = JsonString.Replace("\\", "\\\\");
            }
            if (JsonString.Contains("\'"))
            {
                JsonString = JsonString.Replace("\'", "\\\'");
            }
            //if (JsonString.Contains("\""))
            //{
            //    JsonString = JsonString.Replace("\"", "\\\"");
            //}
            //去掉字符串的回车换行符
            JsonString = Regex.Replace(JsonString, @"[\n\r]", "");
            JsonString = JsonString.Trim();
            return JsonString;
        }
        private static string HandleRequest(HttpListenerRequest request, HttpListenerResponse response)
        {
            string data = null;
            string dodata = null;
            try
            {
                var byteList = new List<byte>();
                var byteArr = new byte[2048];
                int readLen = 0;
                int len = 0;
                //接收客户端传过来的数据并转成字符串类型
                do
                {
                    readLen = request.InputStream.Read(byteArr, 0, byteArr.Length);
                    len += readLen;
                    byteList.AddRange(byteArr);
                } while (readLen != 0);
                data = Encoding.UTF8.GetString(byteList.ToArray(), 0, len);
                dodata = HydeeInterfaces(data, request.RawUrl);
                //获取得到数据data可以进行其他操作
            }
            catch (Exception ex)
            {
                response.StatusDescription = "404";
                response.StatusCode = 404;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"在接收数据时发生错误:{ex.ToString()}\r\n");
                return $"在接收数据时发生错误:{ex.ToString()}\r\n";//把服务端错误信息直接返回可能会导致信息不安全，此处仅供参考
            }
            response.StatusDescription = "200";//获取或设置返回给客户端的 HTTP 状态代码的文本说明。
            response.StatusCode = 200;// 获取或设置返回给客户端的 HTTP 状态代码。
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"接收数据完成:{data.Trim()},时间：{DateTime.Now.ToString()}\r\n");
            Console.WriteLine($"数据处理结果：{dodata}\r\n");
            //return $"接收数据完成";
            return dodata;
        }
        public static void Thread1(object obj)
        {
            string[] ls_inparam;
            string ls_retmsg;
            ls_inparam = new string[0];

            ls_retmsg = _bll.dao.Doprocedure("proc_ybgx_gy", ls_inparam, null, null, null, null, false);
            if (ls_retmsg != "TRUE")
            {
                ls_retmsg = "执行存储过程proc_ybgx_gy失败";
                #region 错误返回
                _bll.dao.RollbackTrans();
                //_retData_WAREList = new ObjectList.RetData_WAREList();
                //_retData_WAREList.code = -1;
                //_retData_WAREList.msg = ls_retmsg;
                //_retData_WAREList.rowcnt = 0;
                //_retData_WAREList.detail = null;
                //bll.dao.WriteLog("168", DateTime.Now, ReqHeadJson, ls_retmsg, null, null, null, ls_cn, "15", "2");
                //return js.Serialize(_retData_WAREList);
                #endregion
            }
            _bll.dao.CommitTrans();
        }
        public static string HydeeInterfaces(string ReqHeadJson, string ReqType)
        {
            #region 变量定义
            ObjectList.RetData_SYJList _retData_SYJList;
            ObjectList.RetData_WAREList _retData_WAREList;
            List<SYJBILLDetailList> _SYJBILLDetailList;
            List<SYJSALEDetailList> _SYJSALEDetailList;
            List<ObjectList.detail> _detaillist = null;
            ObjectList.detail _detail = null;
            PublicBll bll = new PublicBll();
            JObject jObject = null;
            JavaScriptSerializer js = new JavaScriptSerializer();
            string ls_cn = null;
            string ls_syj_dcmno = null;
            string ls_syj_busno = null;
            string ls_syj_compid = null;
            string ls_syj_billno = null;
            string ls_syj_callback_url = null;
            string ls_syj_wareid = null;
            string ls_syj_wareqty = null;
            string ls_syj_type = null;
            string ls_syj_saleprice = null;
            string ls_syj_makeno = null;
            string ls_retmsg = null;
            string ls_wareamt = null;
            string ls_batid = null;
            string ls_saler_d = null;
            string ls_stdprice = null;
            string ls_netprice_d = null;
            string ls_pruprice = null;
            string ls_saleno = null;
            string ls_salers = "";
            string ls_netprice = "";
            string ls_paytype = "";
            string ls_payamt = "";
            string ls_memcard = "";
            decimal ld_netprice = 0;
            decimal ld_wareamt = 0;
            DataTable dt = null;

            string ls_find_startdate = "";
            string ls_find_enddate = "";
            string ls_find_wareid = "";
            string ls_find_warename = "";
            string ls_find_startrow = "";
            string ls_find_endrow = "";
            _SYJBILLDetailList = new List<SYJBILLDetailList>();
            _SYJSALEDetailList = new List<SYJSALEDetailList>();
            #endregion
            try
            {
                #region 获取数据库连接
                if (!bll.getcnParms(Environment.CurrentDirectory + "\\DBConn\\" + "002.xml", out ls_retmsg))
                {
                    bll.dao.RollbackTrans();
                    _retData_SYJList = new ObjectList.RetData_SYJList();
                    _retData_SYJList.code = -1;
                    _retData_SYJList.msg = ls_retmsg;
                    bll.dao.WriteLog("168", DateTime.Now, ReqHeadJson, ls_retmsg, null, null, null, ls_cn, "5", "2");
                    return js.Serialize(_retData_SYJList);
                }
                ls_cn = ls_retmsg;
                #endregion
                #region 验证身份
                ls_retmsg = bll.checkUserValid(bll.FunctionId, bll.InterfaceUserID, bll.InterfacePassWord, bll.OperUserID, bll.OperPassWord, ls_retmsg);
                if (ls_retmsg != "TRUE")
                {
                    #region 错误返回
                    bll.dao.RollbackTrans();
                    _retData_SYJList = new ObjectList.RetData_SYJList();
                    _retData_SYJList.code = -1;
                    _retData_SYJList.msg = ls_retmsg;
                    bll.dao.WriteLog("168", DateTime.Now, ReqHeadJson, ls_retmsg, null, null, null, ls_cn, "5", "2");
                    return js.Serialize(_retData_SYJList);
                    #endregion
                }
                #endregion
                #region 售药机--新增售药机
                if (ReqType == "/Add_Madic")
                {
                    #region 反序列化请求
                    ReqHeadJson = ReplaceString(ReqHeadJson);
                    jObject = JObject.Parse(ReqHeadJson);
                    ls_syj_dcmno = jObject["dcmno"].ToString();
                    ls_syj_busno = jObject["busno"].ToString();
                    #endregion
                }
                #endregion
                #region 售药机--补退货单据上传
                if (ReqType == "/Commit_Order")
                {
                    #region 反序列化请求
                    ReqHeadJson = ReplaceString(ReqHeadJson);
                    jObject = JObject.Parse(ReqHeadJson);
                    ls_syj_dcmno = jObject["dcmno"].ToString();
                    ls_syj_busno = jObject["busno"].ToString();
                    ls_syj_billno = jObject["billno"].ToString();
                    ls_syj_callback_url = jObject["callback_url"].ToString();
                    _SYJBILLDetailList = PublicClass.JsonStringToList<SYJBILLDetailList>(jObject["detail"].ToString());
                    #endregion
                }
                #endregion
                #region 售药机--销售数据上传
                if (ReqType == "/Commit_Sale")
                {
                    #region 反序列化请求
                    ReqHeadJson = ReplaceString(ReqHeadJson);
                    jObject = JObject.Parse(ReqHeadJson);
                    ls_syj_dcmno = jObject["dcmno"].ToString();
                    ls_syj_busno = jObject["busno"].ToString();
                    ls_syj_billno = jObject["billno"].ToString();
                    _SYJSALEDetailList = PublicClass.JsonStringToList<SYJSALEDetailList>(jObject["detail"].ToString());
                    #endregion
                }
                #endregion
                #region 台州医保云查询商品目录
                if (ReqType == "/FindWare")
                {
                    #region 反序列化请求
                    ReqHeadJson = ReplaceString(ReqHeadJson);
                    jObject = JObject.Parse(ReqHeadJson);
                    ls_find_startdate = jObject["startdate"].ToString();
                    if (string.IsNullOrEmpty(ls_find_startdate))
                    { ls_find_startdate = "ALL"; }
                    ls_find_enddate = jObject["enddate"].ToString();
                    if (string.IsNullOrEmpty(ls_find_enddate))
                    { ls_find_enddate = "ALL"; }
                    ls_find_wareid = jObject["wareid"].ToString();
                    if (string.IsNullOrEmpty(ls_find_wareid))
                    { ls_find_wareid = "99999999"; }
                    ls_find_warename = jObject["warename"].ToString();
                    if (string.IsNullOrEmpty(ls_find_warename))
                    { ls_find_warename = "ALL"; }
                    ls_find_startrow = jObject["startrow"].ToString();
                    if (string.IsNullOrEmpty(ls_find_startrow))
                    { ls_find_startrow = "99999999"; }
                    ls_find_endrow = jObject["endrow"].ToString();
                    if (string.IsNullOrEmpty(ls_find_endrow))
                    { ls_find_endrow = "99999999"; }
                    if (ls_find_startrow == "99999999" || ls_find_endrow == "99999999")
                    {
                        ls_retmsg = "开始行数、结束行数不能为空";
                        #region 错误返回
                        bll.dao.RollbackTrans();
                        _retData_WAREList = new ObjectList.RetData_WAREList();
                        _retData_WAREList.code = -1;
                        _retData_WAREList.msg = ls_retmsg;
                        _retData_WAREList.rowcnt = 0;
                        _retData_WAREList.detail = null;
                        bll.dao.WriteLog("168", DateTime.Now, ReqHeadJson, ls_retmsg, null, null, null, ls_cn, "15", "2");
                        return js.Serialize(_retData_WAREList);
                        #endregion
                    }
                    if (Convert.ToInt32(ls_find_endrow) - Convert.ToInt32(ls_find_startrow) >= 1000)
                    {
                        ls_retmsg = "单次查询记录不能超过1000条";
                        #region 错误返回
                        bll.dao.RollbackTrans();
                        _retData_WAREList = new ObjectList.RetData_WAREList();
                        _retData_WAREList.code = -1;
                        _retData_WAREList.msg = ls_retmsg;
                        _retData_WAREList.rowcnt = 0;
                        _retData_WAREList.detail = null;
                        bll.dao.WriteLog("168", DateTime.Now, ReqHeadJson, ls_retmsg, null, null, null, ls_cn, "15", "2");
                        return js.Serialize(_retData_WAREList);
                        #endregion
                    }
                    #endregion
                }
                #endregion
                #region 同步医保云医保信息
                if (ReqType == "/SynInsurance")
                {
                    #region 反序列化请求

                    #endregion
                }
                #endregion
                #region 签名
                if (ReqType == "/Sign")
                {
                    #region 反序列化请求

                    #endregion
                }
                #endregion
                #region  业务处理
                switch (ReqType)
                {
                    case "/Add_Madic"://新增售药机
                        #region 处理
                        dt = bll.dao.GetDataTable("select 1 from t_busno_syj where dcmno = '" + ls_syj_dcmno + "'");
                        if (dt.Rows.Count > 0)
                        {
                            ls_retmsg = "已经存在的售药机编码";
                            #region 错误返回
                            bll.dao.RollbackTrans();
                            _retData_SYJList = new ObjectList.RetData_SYJList();
                            _retData_SYJList.code = -1;
                            _retData_SYJList.msg = ls_retmsg;
                            _retData_SYJList.data = "";
                            bll.dao.WriteLog("168", DateTime.Now, ReqHeadJson, ls_retmsg, null, null, null, ls_cn, "10", "2");
                            return js.Serialize(_retData_SYJList);
                            #endregion
                        }
                        dt = bll.dao.GetDataTable("select 1 from t_busno_syj where busno = '" + ls_syj_busno + "'");
                        if (dt.Rows.Count > 0)
                        {
                            ls_retmsg = "已经存在的门店编码";
                            #region 错误返回
                            bll.dao.RollbackTrans();
                            _retData_SYJList = new ObjectList.RetData_SYJList();
                            _retData_SYJList.code = -1;
                            _retData_SYJList.msg = ls_retmsg;
                            _retData_SYJList.data = "";
                            bll.dao.WriteLog("168", DateTime.Now, ReqHeadJson, ls_retmsg, null, null, null, ls_cn, "10", "2");
                            return js.Serialize(_retData_SYJList);
                            #endregion
                        }
                        dt = bll.dao.GetDataTable("select compid from s_busi where busno = '" + ls_syj_busno + "'");
                        ls_syj_compid = dt.Rows[0][0].ToString();
                        if (String.IsNullOrEmpty(ls_syj_compid))
                        {
                            ls_retmsg = "无法识别的门店编码";
                            #region 错误返回
                            bll.dao.RollbackTrans();
                            _retData_SYJList = new ObjectList.RetData_SYJList();
                            _retData_SYJList.code = -1;
                            _retData_SYJList.msg = ls_retmsg;
                            _retData_SYJList.data = "";
                            bll.dao.WriteLog("168", DateTime.Now, ReqHeadJson, ls_retmsg, null, null, null, ls_cn, "10", "2");
                            return js.Serialize(_retData_SYJList);
                            #endregion
                        }
                        ls_retmsg = bll.dao.SqlDataTable(@"insert into t_busno_syj
                                          (compid, busno, dcmno,lasttime,createuser)
                                        values
                                          ('" + ls_syj_compid + "', " + ls_syj_busno + ", '" + ls_syj_dcmno + "',sysdate,'168')");
                        if (ls_retmsg != "TRUE")
                        {
                            #region 错误返回
                            bll.dao.RollbackTrans();
                            _retData_SYJList = new ObjectList.RetData_SYJList();
                            _retData_SYJList.code = -1;
                            _retData_SYJList.msg = ls_retmsg;
                            _retData_SYJList.data = "";
                            bll.dao.WriteLog("168", DateTime.Now, ReqHeadJson, ls_retmsg, null, null, null, ls_cn, "10", "2");
                            return js.Serialize(_retData_SYJList);
                            #endregion
                        }
                        #endregion
                        #region 返回
                        if (ls_retmsg != "TRUE")
                        {
                            #region 错误返回
                            bll.dao.RollbackTrans();
                            _retData_SYJList = new ObjectList.RetData_SYJList();
                            _retData_SYJList.code = -1;
                            _retData_SYJList.msg = ls_retmsg;
                            _retData_SYJList.data = "";
                            bll.dao.WriteLog("168", DateTime.Now, ReqHeadJson, ls_retmsg, null, null, null, ls_cn, "10", "2");
                            return js.Serialize(_retData_SYJList);
                            #endregion
                        }
                        else
                        {
                            #region 成功返回
                            bll.dao.CommitTrans();
                            _retData_SYJList = new ObjectList.RetData_SYJList();

                            _retData_SYJList.code = 200;
                            _retData_SYJList.msg = "OK";
                            _retData_SYJList.data = "";

                            bll.dao.WriteLog("168", DateTime.Now, ReqHeadJson, ls_retmsg, null, null, null, ls_cn, "10", "1");
                            return js.Serialize(_retData_SYJList);
                            #endregion
                        }
                    #endregion
                    case "/Commit_Sale"://售药机--销售数据上传
                        #region 处理
                        dt = bll.dao.GetDataTable("select 1 from t_busno_syj where dcmno = '" + ls_syj_dcmno + "' and busno = '" + ls_syj_busno + "'");
                        if (dt.Rows.Count <= 0)
                        {
                            ls_retmsg = "无法识别的售药机编码或门店编码";
                            #region 错误返回
                            bll.dao.RollbackTrans();
                            _retData_SYJList = new ObjectList.RetData_SYJList();
                            _retData_SYJList.code = -1;
                            _retData_SYJList.msg = ls_retmsg;
                            _retData_SYJList.data = "";
                            bll.dao.WriteLog("168", DateTime.Now, ReqHeadJson, ls_retmsg, null, null, null, ls_cn, "10", "2");
                            return js.Serialize(_retData_SYJList);
                            #endregion
                        }
                        dt = bll.dao.GetDataTable("select compid from s_busi where busno = '" + ls_syj_busno + "'");
                        ls_syj_compid = dt.Rows[0][0].ToString();
                        if (String.IsNullOrEmpty(ls_syj_compid))
                        {
                            ls_retmsg = "无法识别的门店编码";
                            #region 错误返回
                            bll.dao.RollbackTrans();
                            _retData_SYJList = new ObjectList.RetData_SYJList();
                            _retData_SYJList.code = -1;
                            _retData_SYJList.msg = ls_retmsg;
                            _retData_SYJList.data = "";
                            bll.dao.WriteLog("168", DateTime.Now, ReqHeadJson, ls_retmsg, null, null, null, ls_cn, "10", "2");
                            return js.Serialize(_retData_SYJList);
                            #endregion
                        }
                        #region 插入明细表
                        ld_netprice = 0;
                        for (int i = 0; i < _SYJSALEDetailList.Count; i++)
                        {
                            ls_syj_wareid = "0";
                            ls_syj_wareqty = "0";
                            ls_syj_saleprice = "0";
                            ls_syj_makeno = "0";
                            ls_wareamt = "0";
                            ls_batid = "0";
                            ls_saler_d = "168";
                            ls_stdprice = "0";
                            ls_netprice_d = "0";
                            ls_pruprice = "0";
                            ld_wareamt = 0;
                            ls_syj_wareid = _SYJSALEDetailList[i].wareid.ToString();
                            ls_syj_wareqty = _SYJSALEDetailList[i].wareqty.ToString();
                            ls_syj_saleprice = _SYJSALEDetailList[i].saleprice.ToString();
                            ls_syj_makeno = _SYJSALEDetailList[i].makeno.ToString();
                            ld_wareamt = Convert.ToInt32(ls_syj_wareqty) * Convert.ToDecimal(ls_syj_saleprice);
                            ls_wareamt = ld_wareamt.ToString();
                            ld_netprice += ld_wareamt;
                            if (string.IsNullOrEmpty(ls_syj_makeno))
                            {
                                ls_syj_makeno = "";
                            }
                            ls_retmsg = bll.dao.SqlDataTable(@"insert into t_sale_import_gy_d
                                          (srcbillno, rowno, wareid, wareqty, wareprice, wareamt, batid, makeno,saler,stdprice,netprice,purprice)
                                        values
                                          ('" + ls_syj_billno + "', " + (i + 1).ToString() + ", " + ls_syj_wareid + ", " + ls_syj_wareqty + ", " + ls_syj_saleprice + ", " + ls_wareamt + ", " + ls_batid + ", '" + ls_syj_makeno + "', " + ls_saler_d + ", " + ls_stdprice + ", " + ls_netprice_d + ", " + ls_pruprice + ")");
                            if (ls_retmsg != "TRUE")
                            {
                                #region 错误返回
                                bll.dao.RollbackTrans();
                                _retData_SYJList = new ObjectList.RetData_SYJList();
                                _retData_SYJList.code = -1;
                                _retData_SYJList.msg = ls_retmsg;
                                _retData_SYJList.data = "";
                                bll.dao.WriteLog("168", DateTime.Now, ReqHeadJson, ls_retmsg, null, null, null, ls_cn, "10", "2");
                                return js.Serialize(_retData_SYJList);
                                #endregion
                            }
                        }
                        #endregion
                        #region 插入头表
                        dt = bll.dao.GetDataTable("select f_get_serial('SAL','" + ls_syj_busno + "') saleno from dual");
                        ls_saleno = dt.Rows[0]["saleno"].ToString();
                        ls_salers = "168";
                        ls_netprice = ld_netprice.ToString();
                        ls_paytype = "1";
                        ls_payamt = ls_netprice;
                        ls_memcard = "";
                        ls_retmsg = bll.dao.SqlDataTable(@"insert into t_sale_import_gy_h
                                              (compid, busno, srcbillno, salers, netprice, accdate, paytype, payamt, status,saleno,operdate,memcard,imptypes)
                                            values
                                              (" + ls_syj_compid + ", " + ls_syj_busno + ", '" + ls_syj_billno + "', '" + ls_salers + "', " + ls_netprice + ",sysdate, '" + ls_paytype + "', '" + ls_payamt + "', 2,'" + ls_saleno + "',sysdate,'" + ls_memcard + "',1)");
                        if (ls_retmsg != "TRUE")
                        {
                            #region 错误返回
                            bll.dao.RollbackTrans();
                            _retData_SYJList = new ObjectList.RetData_SYJList();
                            _retData_SYJList.code = -1;
                            _retData_SYJList.msg = ls_retmsg;
                            _retData_SYJList.data = "";
                            bll.dao.WriteLog("168", DateTime.Now, ReqHeadJson, ls_retmsg, null, null, null, ls_cn, "10", "2");
                            return js.Serialize(_retData_SYJList);
                            #endregion
                        }
                        #endregion
                        #endregion
                        #region 返回
                        if (ls_retmsg != "TRUE")
                        {
                            #region 错误返回
                            bll.dao.RollbackTrans();
                            _retData_SYJList = new ObjectList.RetData_SYJList();
                            _retData_SYJList.code = -1;
                            _retData_SYJList.msg = ls_retmsg;
                            _retData_SYJList.data = "";
                            bll.dao.WriteLog("168", DateTime.Now, ReqHeadJson, ls_retmsg, null, null, null, ls_cn, "10", "2");
                            return js.Serialize(_retData_SYJList);
                            #endregion
                        }
                        else
                        {
                            #region 成功返回
                            bll.dao.CommitTrans();
                            _retData_SYJList = new ObjectList.RetData_SYJList();

                            _retData_SYJList.code = 200;
                            _retData_SYJList.msg = "OK";
                            _retData_SYJList.data = "";

                            bll.dao.WriteLog("168", DateTime.Now, ReqHeadJson, ls_retmsg, null, null, null, ls_cn, "10", "1");
                            return js.Serialize(_retData_SYJList);
                            #endregion
                        }
                    #endregion
                    case "/FindWare"://台州医保查询商品信息
                        #region 处理
                        dt = bll.dao.GetDataTable(@"select rnum,
                                                           wareid,
                                                           warecode,
                                                           warename,
                                                           waregeneralname,
                                                           warespec,
                                                           wareunit,
                                                           factoryid,
                                                           fileno,
                                                           wareabc,
                                                           areacode,
                                                           barcode
                                                      from(select wareid,
                                                                   warecode,
                                                                   warename,
                                                                   waregeneralname,
                                                                   warespec,
                                                                   wareunit,
                                                                   factoryid,
                                                                   fileno,
                                                                   wareabc,
                                                                   areacode,
                                                                   barcode,
                                                                   rownum rnum,
                                                                   lasttime
                                                              from t_ware_base
                                                             where (trunc(lasttime) >= to_date('" + ls_find_startdate + @"', 'yyyy-mm-dd') or 'ALL' = '" + ls_find_startdate + @"')
                                                               and (trunc(lasttime) <= to_date('" + ls_find_enddate + @"', 'yyyy-mm-dd') or 'ALL' = '" + ls_find_enddate + @"')
                                                               and(wareid = " + ls_find_wareid + @" or '99999999' = '" + ls_find_wareid + @"')
                                                               and(warename like '%" + ls_find_warename + @"%' or 'ALL' = '" + ls_find_warename + @"')
                                                             order by lasttime)
                                                     where (rnum >= " + ls_find_startrow + @" or '99999999' = '" + ls_find_startrow + @"')
                                                       and (rnum <= " + ls_find_endrow + @" or '99999999' = '" + ls_find_endrow + @"')");
                        if (dt.Rows.Count <= 0)
                        {
                            ls_retmsg = "查询无记录";
                            #region 错误返回
                            bll.dao.RollbackTrans();
                            _retData_WAREList = new ObjectList.RetData_WAREList();
                            _retData_WAREList.code = 1;
                            _retData_WAREList.msg = ls_retmsg;
                            _retData_WAREList.rowcnt = 0;
                            _retData_WAREList.detail = null;
                            bll.dao.WriteLog("168", DateTime.Now, ReqHeadJson, ls_retmsg, null, null, null, ls_cn, "15", "2");
                            return js.Serialize(_retData_WAREList);
                            #endregion
                        }
                        else
                        {
                            if (dt.Rows.Count > 1000)
                            {
                                ls_retmsg = "查询记录超过1000条，请核查查询条件";
                                #region 错误返回
                                bll.dao.RollbackTrans();
                                _retData_WAREList = new ObjectList.RetData_WAREList();
                                _retData_WAREList.code = 1;
                                _retData_WAREList.msg = ls_retmsg;
                                _retData_WAREList.rowcnt = 0;
                                _retData_WAREList.detail = null;
                                bll.dao.WriteLog("168", DateTime.Now, ReqHeadJson, ls_retmsg, null, null, null, ls_cn, "15", "2");
                                return js.Serialize(_retData_WAREList);
                                #endregion
                            }
                            _detaillist = new List<ObjectList.detail>();
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                _detail = new ObjectList.detail();
                                _detail.areacode = dt.Rows[i]["areacode"].ToString();
                                _detail.barcode = dt.Rows[i]["barcode"].ToString();
                                _detail.factoryid = dt.Rows[i]["factoryid"].ToString();
                                _detail.fileno = dt.Rows[i]["fileno"].ToString();
                                _detail.rownum = dt.Rows[i]["rnum"].ToString();
                                _detail.wareabc = dt.Rows[i]["wareabc"].ToString();
                                _detail.warecode = dt.Rows[i]["warecode"].ToString();
                                _detail.waregeneralname = dt.Rows[i]["waregeneralname"].ToString();
                                _detail.wareid = dt.Rows[i]["wareid"].ToString();
                                _detail.warename = dt.Rows[i]["warename"].ToString();
                                _detail.warespec = dt.Rows[i]["warespec"].ToString();
                                _detail.wareunit = dt.Rows[i]["wareunit"].ToString();
                                _detaillist.Add(_detail);
                            }
                            bll.dao.CommitTrans();
                            _retData_WAREList = new ObjectList.RetData_WAREList();
                            ls_retmsg = "OK";
                            _retData_WAREList.code = 1;
                            _retData_WAREList.msg = ls_retmsg;
                            _retData_WAREList.rowcnt = dt.Rows.Count;
                            _retData_WAREList.detail = _detaillist;
                            bll.dao.WriteLog("168", DateTime.Now, ReqHeadJson, js.Serialize(_retData_WAREList), null, null, null, ls_cn, "15", "1");
                            return js.Serialize(_retData_WAREList);
                        }
                    #endregion
                    case "/SynInsurance"://同步医保信息
                        #region 处理
                        _bll = new PublicBll();
                        _bll = bll;
                        Thread thread1 = new Thread(new ParameterizedThreadStart(Thread1));
                        thread1.Start();
                        _retData_WAREList = new ObjectList.RetData_WAREList();
                        ls_retmsg = "正在后台执行数据同步，预计时间10分钟内，请不要高频调用此接口";
                        _retData_WAREList.code = 1;
                        _retData_WAREList.msg = ls_retmsg;
                        bll.dao.WriteLog("168", DateTime.Now, ReqHeadJson, js.Serialize(_retData_WAREList), null, null, null, ls_cn, "15", "1");
                        return js.Serialize(_retData_WAREList);
                    #endregion
                    case "/Commit_Order"://售药机上传补、退货单据
                        #region 处理
                        dt = bll.dao.GetDataTable("select 1 from t_busno_syj where dcmno = '" + ls_syj_dcmno + "' and busno = '" + ls_syj_busno + "'");
                        if (dt.Rows.Count <= 0)
                        {
                            ls_retmsg = "无法识别的售药机编码或门店编码";
                            #region 错误返回
                            bll.dao.RollbackTrans();
                            _retData_SYJList = new ObjectList.RetData_SYJList();
                            _retData_SYJList.code = -1;
                            _retData_SYJList.msg = ls_retmsg;
                            _retData_SYJList.data = "";
                            bll.dao.WriteLog("168", DateTime.Now, ReqHeadJson, ls_retmsg, null, null, null, ls_cn, "10", "2");
                            return js.Serialize(_retData_SYJList);
                            #endregion
                        }
                        dt = bll.dao.GetDataTable("select 1 from t_syj_adjust_h where billno = '" + ls_syj_billno + "' and status in(1,2)");
                        if (dt.Rows.Count > 0)
                        {
                            ls_retmsg = "已生成货位调整单的单据不能重复发送";
                            #region 错误返回
                            bll.dao.RollbackTrans();
                            _retData_SYJList = new ObjectList.RetData_SYJList();
                            _retData_SYJList.code = -1;
                            _retData_SYJList.msg = ls_retmsg;
                            _retData_SYJList.data = "";
                            bll.dao.WriteLog("168", DateTime.Now, ReqHeadJson, ls_retmsg, null, null, null, ls_cn, "10", "2");
                            return js.Serialize(_retData_SYJList);
                            #endregion
                        }
                        #region 插入单头
                        ls_retmsg = bll.dao.SqlDataTable("delete from t_syj_adjust_h where billno = '" + ls_syj_billno + "' and status = 0");
                        if (ls_retmsg != "TRUE")
                        {
                            #region 错误返回
                            bll.dao.RollbackTrans();
                            _retData_SYJList = new ObjectList.RetData_SYJList();
                            _retData_SYJList.code = -1;
                            _retData_SYJList.msg = ls_retmsg;
                            _retData_SYJList.data = "";
                            bll.dao.WriteLog("168", DateTime.Now, ReqHeadJson, ls_retmsg, null, null, null, ls_cn, "10", "2");
                            return js.Serialize(_retData_SYJList);
                            #endregion
                        }
                        ls_retmsg = bll.dao.SqlDataTable("delete from t_syj_adjust_d where billno = '" + ls_syj_billno + "'");
                        if (ls_retmsg != "TRUE")
                        {
                            #region 错误返回
                            bll.dao.RollbackTrans();
                            _retData_SYJList = new ObjectList.RetData_SYJList();
                            _retData_SYJList.code = -1;
                            _retData_SYJList.msg = ls_retmsg;
                            _retData_SYJList.data = "";
                            bll.dao.WriteLog("168", DateTime.Now, ReqHeadJson, ls_retmsg, null, null, null, ls_cn, "10", "2");
                            return js.Serialize(_retData_SYJList);
                            #endregion
                        }
                        ls_retmsg = bll.dao.SqlDataTable(@"insert into t_syj_adjust_h
                                                  (dcmno, busno, billno, callback_url, log_time, status, msg)
                                                values
                                                  ('" + ls_syj_dcmno + "', '" + ls_syj_busno + "', '" + ls_syj_billno + "', '" + ls_syj_callback_url + "', sysdate, 0, null)");
                        if (ls_retmsg != "TRUE")
                        {
                            #region 错误返回
                            bll.dao.RollbackTrans();
                            _retData_SYJList = new ObjectList.RetData_SYJList();
                            _retData_SYJList.code = -1;
                            _retData_SYJList.msg = ls_retmsg;
                            _retData_SYJList.data = "";
                            bll.dao.WriteLog("168", DateTime.Now, ReqHeadJson, ls_retmsg, null, null, null, ls_cn, "10", "2");
                            return js.Serialize(_retData_SYJList);
                            #endregion
                        }
                        #endregion 插入单明细
                        for (int i = 0; i < _SYJBILLDetailList.Count; i++)
                        {
                            ls_syj_wareid = null;
                            ls_syj_wareqty = null;
                            ls_syj_type = null;
                            ls_syj_makeno = null;
                            ls_syj_wareid = _SYJBILLDetailList[i].wareid.ToString();
                            ls_syj_wareqty = _SYJBILLDetailList[i].wareqty.ToString();
                            ls_syj_type = _SYJBILLDetailList[i].type.ToString();//1补货2退货
                            if (ls_syj_type == "2")
                            {
                                ls_syj_makeno = _SYJBILLDetailList[i].makeno.ToString();
                            }
                            if (string.IsNullOrEmpty(ls_syj_makeno))
                            {
                                ls_syj_makeno = "";
                            }
                            ls_retmsg = bll.dao.SqlDataTable(@"insert into t_syj_adjust_d
                                                          (billno, wareid, wareqty, type, makeno)
                                                        values
                                                          ('" + ls_syj_billno + "', '" + ls_syj_wareid + "', " + ls_syj_wareqty + ", " + ls_syj_type + ", '" + ls_syj_makeno + "')");
                            if (ls_retmsg != "TRUE")
                            {
                                #region 错误返回
                                bll.dao.RollbackTrans();
                                _retData_SYJList = new ObjectList.RetData_SYJList();
                                _retData_SYJList.code = -1;
                                _retData_SYJList.msg = ls_retmsg;
                                _retData_SYJList.data = "";
                                bll.dao.WriteLog("168", DateTime.Now, ReqHeadJson, ls_retmsg, null, null, null, ls_cn, "10", "2");
                                return js.Serialize(_retData_SYJList);
                                #endregion
                            }
                        }
                        #endregion
                        #region 返回
                        if (ls_retmsg != "TRUE")
                        {
                            #region 错误返回
                            bll.dao.RollbackTrans();
                            _retData_SYJList = new ObjectList.RetData_SYJList();
                            _retData_SYJList.code = -1;
                            _retData_SYJList.msg = ls_retmsg;
                            _retData_SYJList.data = "";
                            bll.dao.WriteLog("168", DateTime.Now, ReqHeadJson, ls_retmsg, null, null, null, ls_cn, "10", "2");
                            return js.Serialize(_retData_SYJList);
                            #endregion
                        }
                        else
                        {
                            #region 成功返回
                            bll.dao.CommitTrans();
                            _retData_SYJList = new ObjectList.RetData_SYJList();

                            _retData_SYJList.code = 200;
                            _retData_SYJList.msg = "OK";
                            _retData_SYJList.data = "";

                            bll.dao.WriteLog("168", DateTime.Now, ReqHeadJson, ls_retmsg, null, null, null, ls_cn, "10", "1");
                            return js.Serialize(_retData_SYJList);
                            #endregion
                        }
                    #endregion
                    case "/Sign"://签名
                        #region 处理
                        string ls_clientCode = null;
                        string ls_prescriptionCode = null;
                        string ls_clientKey = null;
                        ReqHeadJson = ReplaceString(ReqHeadJson);
                        jObject = JObject.Parse(ReqHeadJson);
                        ls_clientCode = jObject["clientCode"].ToString();
                        ls_prescriptionCode = jObject["prescriptionCode"].ToString();
                        ls_clientKey = jObject["clientKey"].ToString();
                        Dictionary<string, string> dc = new Dictionary<string, string>();
                        dc.Add("clientCode", ls_clientCode);
                        dc.Add("prescriptionCode", ls_prescriptionCode);
                        ls_retmsg = WePaySign(dc, ls_clientKey);
                        bll.dao.CommitTrans();
                        _retData_SYJList = new ObjectList.RetData_SYJList();

                        _retData_SYJList.code = 200;
                        _retData_SYJList.msg = "OK";
                        _retData_SYJList.data = ls_retmsg;

                        bll.dao.WriteLog("168", DateTime.Now, ReqHeadJson, ls_retmsg, null, null, null, ls_cn, "20", "1");
                        return js.Serialize(_retData_SYJList);
                    #endregion
                    default:
                        #region 异常返回
                        bll.dao.RollbackTrans();
                        _retData_SYJList = new ObjectList.RetData_SYJList();
                        _retData_SYJList.code = -1;
                        _retData_SYJList.msg = "无法识别的功能地址";
                        _retData_SYJList.data = "";
                        bll.dao.WriteLog("168", DateTime.Now, ReqHeadJson, ls_retmsg, null, null, null, ls_cn, "10", "2");
                        return js.Serialize(_retData_SYJList);
                        #endregion
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region 异常返回
                bll.dao.RollbackTrans();
                _retData_SYJList = new ObjectList.RetData_SYJList();
                _retData_SYJList.code = -1;
                ls_retmsg = ex.Message.ToString();
                _retData_SYJList.msg = ls_retmsg;
                bll.dao.WriteLog("168", DateTime.Now, ReqHeadJson, ls_retmsg, null, null, null, ls_cn, "5", "2");
                return js.Serialize(_retData_SYJList);
                #endregion
            }
        }
    }
}
