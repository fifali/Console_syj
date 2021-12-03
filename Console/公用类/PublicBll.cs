using System;
using System.Text;
using System.Net;
using System.Xml;
using System.IO;
using System.Net.Cache;
using Oracle.ManagedDataAccess.Client;
using System.Web.Script.Serialization;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Data;
using swiftpass.utils;
using System.Collections;

namespace ConsoleHydee
{
    public class PublicBll
    {
        #region 变量
        private ClientResponseHandler resHandler = new ClientResponseHandler();
        //private Dictionary<string, string> cfg = new Dictionary<string, string>(1);
        private PayHttpClient pay = new PayHttpClient();
        private RequestHandler reqHandler = null;
        public string UserName = "";
        public string DBConnStr = "";
        public string DBServer = "";
        public string Port = "";
        public string Host = "";
        public string Server_Name = "";
        public string OrgCode = "";
        public string OrgId = "";
        public string OrgName = "";
        public string UserID = "";
        public string PassWord = "";
        public string UserPower = "";
        public string Opertime = "";
        public string Interface = "";
        public string OperUserID = "";
        public string OperPassWord = "";
        public string InterfaceUserID = "";
        public string InterfacePassWord = "";
        public bool IsValidUser = false;
        public string FunctionId = "";
        public PbulicDao dao = null;
        #endregion

        #region 结构化
        public PublicBll()
        {
            dao = new PbulicDao();
        }
        #endregion

        #region 数据库连接相关
        /*
		 * / <summary>
		 * / 获取数据库联接参数串
		 * / </summary>
		 * / <param name="OrgCode"></param>
		 * / <returns></returns>
		 */
        public bool getcnParms(string databaseini, out string mess)
        {
            XmlTextReader txtReader = new XmlTextReader(databaseini);
            try
            {
                /* 找到符合的节点获取需要的属性值 */
                while (txtReader.Read())
                {
                    txtReader.MoveToElement();
                    if (txtReader.Name == "org")
                    {
                        DBServer = txtReader.GetAttribute("DBServer");
                        Port = txtReader.GetAttribute("PORT");
                        Host = txtReader.GetAttribute("HOST");
                        Server_Name = txtReader.GetAttribute("SERVICE_NAME");
                        UserID = txtReader.GetAttribute("UserID");
                        PassWord = txtReader.GetAttribute("PassWord");
                        break;
                    }
                }
                if (DBServer == "")
                {
                    mess = "获取机构" + OrgCode + "的数据库连接参数错误，请检查配置文件！";
                    dao.RollbackTrans();
                    return (false);
                }
                else
                {
                    mess = "Data Source=(DESCRIPTION =    (ADDRESS_LIST =      (ADDRESS = (PROTOCOL = TCP)(HOST = " + Host + ")(PORT = " + Port + "))    )    (CONNECT_DATA =      (SERVER = DEDICATED)      (SERVICE_NAME = " + Server_Name + ")    )  );Persist Security Info=True;User ID=" + UserID + ";Password=" + PassWord + ";";
                    return (true);
                }
            }
            catch (Exception e)
            {
                if (e.Message.ToString().Contains("未能找到文件"))
                {
                    mess = "服务器没有找到机构" + OrgCode + "的数据库连接配置参数！";
                }
                else
                {
                    mess = "获取机构" + OrgCode + "的数据库连接参数错误：" + e.Message.ToString();
                }
                dao.RollbackTrans();
                return (false);
            }
            finally
            {
                txtReader.Close();
            }
        }

        public bool geturlParms(string databaseini, out string url)
        {
            XmlTextReader txtReader = new XmlTextReader(databaseini);
            try
            {
                /* 找到符合的节点获取需要的属性值 */
                while (txtReader.Read())
                {
                    txtReader.MoveToElement();
                    if (txtReader.Name == "org")
                    {
                        url = txtReader.GetAttribute("url");
                        return (true);
                    }
                }
                url = "";
                return (false);
            }
            catch (Exception e)
            {
                url = e.Message.ToString();
                return (false);
            }
            finally
            {
                txtReader.Close();
            }
        }
        /*
         * / <summary>
         * / 根据给定的参数类型获得数据库联接串
         * / </summary>
         * / <param name="DataType"></param>
         * / <param name="Data"></param>
         * / <param name="mess"></param>
         * / <returns></returns>
         */
        public bool getDBConnStr(string DataType, string Data, out string mess)
        {
            try
            {
                if (DBConnStr != "")
                {
                    mess = DBConnStr;
                    return (true);
                }
                else
                {
                    switch (DataType)
                    {
                        case "OrgCode":
                            OrgCode = Data;
                            break;
                        case "hospitalcode":
                            OrgCode = Data.Substring(0, 6);
                            break;
                        case "bookcard":
                            OrgCode = Data.Substring(0, 6);
                            break;
                        default:
                            mess = "指定的区域型数据类型错误，无法去联接数据库！";
                            return (false);
                    }
                    if (getcnParms(OrgCode, out mess))
                    {
                        DBConnStr = "Provider=SQLOLEDB.1;Persist Security Info=False;" + mess;
                        return (true);
                    }
                    else
                    {
                        return (false);
                    }
                }
            }
            catch (Exception e)
            {
                DBConnStr = "";
                mess = "寻址数据库异常：" + e.Message.ToString();
                return (false);
            }
        }
        #endregion

        #region 用户状态
        /*
		 * / <summary>
		 * / 得到当前用户是否有效
		 * / </summary>
		 * / <returns></returns>
		 */
        public bool getUserState()
        {
            if (IsValidUser)
            {
                return (true);
            }
            return (false);
        }

        /*
         * / <summary>
         * / 设置当前用户是否有效
         * / </summary>
         * / <param name="IfValid"></param>
         * / <returns></returns>
         */
        public bool setUserState(bool IfValid)
        {
            IsValidUser = IfValid;
            return (IfValid);
        }
        #endregion

        #region 用户验证
        /*
		 * / <summary>
		 * / 验证用户身份，返回“TRUE”表示验证通过，InterFacePower表示允许使用的接口业务类型
		 * / </summary>
		 * / <param name="areacode"></param>
		 * / <param name="hospitalcode"></param>
		 * / <param name="userid"></param>
		 * / <param name="pwd"></param>
		 * / <returns></returns>
		 * [WebMethod(Description="Service用户身份验证，返回“TRUE”表示验证通过")]
		 */
        public string checkUserValid(string FunctionId, string userid, string pwd, string operuserid, string operuserpass, string ls_mess)
        {
            string ls_sql;
            dao.cn = new Oracle.ManagedDataAccess.Client.OracleConnection(ls_mess);
            return "TRUE"; 
            dao.cmd = null;
            OracleDataReader myReader = null;
            try
            {
                dao.Open();
                dao.BeginTrans();
                ls_sql = "SELECT Userguid,UserName,departguid,(select Departname from Tb_Dms_Depart where DepartCode = '" + OrgCode + "') as departname FROM Tb_Dms_User WHERE loginname ='" + operuserid + "' AND UserPass='" + operuserpass + "' AND DepartGUID in (select DepartGUID from Tb_Dms_Depart where DepartCode = '" + OrgCode + "')";
                dao.cmd = new Oracle.ManagedDataAccess.Client.OracleCommand(ls_sql, dao.cn);

                if (dao.inTransaction)
                {
                    dao.cmd.Transaction = dao.trans;
                }
                myReader = dao.cmd.ExecuteReader();
                if (!myReader.HasRows)
                {
                    return ("用户代码或密码错误，身份验证失败！");
                }
                else
                {
                    myReader.Read();
                    UserID = myReader.GetString(0);
                    UserName = myReader.GetString(1);
                    OrgId = myReader.GetString(2);
                    OrgName = myReader.GetString(3);
                    setUserState(true);
                    return ("TRUE");
                }
            }
            catch (Exception e)
            {
                dao.RollbackTrans();
                return ("验证异常！" + e.Message.ToString());
            }
            finally
            {

                if (myReader != null)
                {
                    if (!myReader.IsClosed)
                        myReader.Close();
                    myReader.Dispose();
                }
                dao.cmd = null;
            }
        }

        public string checkUserValid_hydee(string FunctionId, string userid, string pwd, string operuserid, string operuserpass, string mess)
        {
            string ls_sql;
            dao.cn = new Oracle.ManagedDataAccess.Client.OracleConnection(mess);
            if (FunctionId == "2001" || FunctionId == "2002" || FunctionId == "2003" || FunctionId == "2004" || FunctionId == "2005" || FunctionId == "2006" || FunctionId == "3001" || FunctionId == "3002" || FunctionId == "3003" || FunctionId == "3004")
            { return "TRUE"; }
            dao.cmd = null;
            OracleDataReader myReader = null;
            try
            {
                dao.Open();
                dao.BeginTrans();
                ls_sql = "SELECT Userguid,UserName FROM Tb_hydee_User WHERE loginname ='" + operuserid + "' AND UserPass='" + operuserpass + "' and status = '1'";
                dao.cmd = new Oracle.ManagedDataAccess.Client.OracleCommand(ls_sql, dao.cn);

                if (dao.inTransaction)
                {
                    dao.cmd.Transaction = dao.trans;
                }
                myReader = dao.cmd.ExecuteReader();
                if (!myReader.HasRows)
                {
                    return ("用户代码或密码错误，身份验证失败！");
                }
                else
                {
                    myReader.Read();
                    UserID = myReader.GetString(0);
                    UserName = myReader.GetString(1);
                    //OrgId = myReader.GetString(2);
                    //OrgName = myReader.GetString(3);
                    setUserState(true);
                    return ("TRUE");
                }
            }
            catch (Exception e)
            {
                dao.RollbackTrans();
                return ("验证异常！" + e.Message.ToString());
            }
            finally
            {

                if (myReader != null)
                {
                    if (!myReader.IsClosed)
                        myReader.Close();
                    myReader.Dispose();
                }
                dao.cmd = null;
            }
        }
        #endregion

        #region 获取GUID
        public string getguid()
        {
            string ls_return = "";
            Oracle.ManagedDataAccess.Client.OracleCommand cmdnew;
            try
            {
                dao.Open();
                cmdnew = new Oracle.ManagedDataAccess.Client.OracleCommand("SELECT createguid() from dual", dao.cn);
                if (dao.inTransaction)
                {
                    cmdnew.Transaction = dao.trans;
                }
                OracleDataReader myReader = cmdnew.ExecuteReader();
                try
                {
                    if (!myReader.HasRows)
                    {
                        return ("获取GUID失败！");
                    }
                    else
                    {
                        myReader.Read();
                        ls_return = myReader.GetString(0);
                        return (ls_return);
                    }
                }
                finally
                {
                    if (myReader != null)
                    {
                        if (!myReader.IsClosed)
                            myReader.Close();
                        myReader.Dispose();
                    }
                }
            }
            catch (Exception e)
            {
                return ("获取GUID异常！" + e.Message.ToString());
            }
            finally
            {
                cmdnew = null;
            }
        }
        #endregion
        
        #region 调用http
        public string HttpWeb(string sUrl, string appid, string apisign, string timestamp, string sPostData)
        {
            string sMode = "POST";
            Encoding myEncoding = Encoding.UTF8;
            //string sContentType = "application/x-www-form-urlencoded";
            string sContentType = "application/json";
            HttpWebRequest req;

            try
            {
                // init
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                req = HttpWebRequest.Create(sUrl) as HttpWebRequest;
                req.Method = sMode;
                req.Accept = "*/*";
                req.KeepAlive = false;
                req.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
                req.Headers.Add("appid", appid);
                req.Headers.Add("timestamp", timestamp);
                req.Headers.Add("apisign", apisign);
                if (0 == string.Compare("POST", sMode))
                {
                    byte[] bufPost = myEncoding.GetBytes(sPostData);
                    req.ContentType = sContentType;
                    req.ContentLength = bufPost.Length;
                    Stream newStream = req.GetRequestStream();
                    newStream.Write(bufPost, 0, bufPost.Length);
                    newStream.Close();
                }

                // Response
                HttpWebResponse res = req.GetResponse() as HttpWebResponse;
                try
                {
                    // 找到合适的编码
                    Encoding encoding = null;
                    //encoding = Encoding_FromBodyName(res.CharacterSet);	// 后来发现主体部分的字符集与Response.CharacterSet不同.
                    //if (null == encoding) encoding = myEncoding;
                    encoding = myEncoding;
                    //System.Diagnostics.Debug.WriteLine(encoding);

                    // body
                    using (Stream resStream = res.GetResponseStream())
                    {
                        using (StreamReader resStreamReader = new StreamReader(resStream, encoding))
                        {
                            return resStreamReader.ReadToEnd();
                        }
                    }
                }
                finally
                {
                    res.Close();
                }
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }
        #endregion

        #region XML字符串转DS
        public DataSet ConvertXMLToDataSet(string xmlData)
        {
            StringReader stream = null;
            XmlTextReader reader = null;
            try
            {
                DataSet xmlDS = new DataSet();
                stream = new StringReader(xmlData);
                reader = new XmlTextReader(stream);
                xmlDS.ReadXml(reader);
                return xmlDS;
            }
            catch (Exception ex)
            {
                string strTest = ex.Message;
                return null;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }
        #endregion

        #region 兴E付支付
        public string interface_xyf(string ls_sendtext, string ls_param1, string ls_param2, string ls_param3, string ls_param4, string ls_param5)
        {
            string ls_auth_code = "";
            string ls_out_trade_no = "";
            string ls_body = "";
            string ls_attach = "";
            string ls_retmsg = "";
            string ls_tbtotal_fee = "";
            string ls_time_expire = "";
            string ls_mch_create_ip = "";
            string ls_tbtime_start = "";
            string ls_sql = "";
            DataTable dt = null;
            DataTable dtnew = null;

            dt = dao.GetDataTable("select nvl(time_start,'') as time_start,nvl(time_expire,'') as time_expire,nvl(attach,'') as attach,nvl(body,'') as body,nvl(total_fee,'') as total_fee,nvl(mch_create_ip,'') as mch_create_ip,nvl(auth_code,'') as auth_code from interface_gjzh_fkmsk where out_trade_no = '" + ls_sendtext + "'");
            if (dt.Rows.Count == 0)
            {
                ls_retmsg = "无法识别的订单号！";
                return ls_retmsg;
            }
            ls_auth_code = dt.Rows[0]["auth_code"].ToString();
            ls_out_trade_no = ls_sendtext;
            ls_body = dt.Rows[0]["body"].ToString();
            ls_attach = dt.Rows[0]["attach"].ToString();
            ls_tbtotal_fee = dt.Rows[0]["total_fee"].ToString();
            ls_mch_create_ip = dt.Rows[0]["mch_create_ip"].ToString();
            if(string.IsNullOrEmpty(ls_mch_create_ip))
            { ls_mch_create_ip = "192.168.1.1"; }
            ls_tbtime_start = dt.Rows[0]["time_start"].ToString();
            ls_time_expire = dt.Rows[0]["time_expire"].ToString();




            this.reqHandler = new RequestHandler(null);
            //加载配置数据
            //this.cfg = Utils.loadCfg();
            //this.reqHandler.setGateUrl(this.cfg["req_url"].ToString());
            this.reqHandler.setGateUrl(ls_param2);
            //this.reqHandler.setKey(this.cfg["key"].ToString());
            this.reqHandler.setParameter("auth_code", ls_auth_code);//付款授权码
            this.reqHandler.setParameter("out_trade_no", ls_out_trade_no);//商户订单号
            this.reqHandler.setParameter("body", ls_body);//商品描述
            this.reqHandler.setParameter("sign_type", "RSA_1_1");//签名方式
            this.reqHandler.setParameter("attach", ls_attach);//附加信息
            this.reqHandler.setParameter("total_fee", ls_tbtotal_fee);//总金额
            this.reqHandler.setParameter("mch_create_ip", ls_mch_create_ip);//终端IP
            this.reqHandler.setParameter("time_start", ls_tbtime_start); //订单生成时间
            this.reqHandler.setParameter("time_expire", ls_time_expire);//订单超时时间
            this.reqHandler.setParameter("service", "unified.trade.micropay");//接口类型： 
            //this.reqHandler.setParameter("mch_id", this.cfg["mch_id"].ToString());//必填项，商户号，由平台分配
            this.reqHandler.setParameter("mch_id", ls_param1);//必填项，商户号，由平台分配
            //this.reqHandler.setParameter("version", this.cfg["version"].ToString());//接口版本号
            this.reqHandler.setParameter("version", ls_param5);//接口版本号
            this.reqHandler.setParameter("nonce_str", Utils.random());//随机字符串，必填项，不长于 32 位
            this.reqHandler.createSign(ls_param3);//创建签名
                                         //以上参数进行签名
            string data = Utils.toXml(this.reqHandler.getAllParameters());//生成XML报文
            Dictionary<string, string> reqContent = new Dictionary<string, string>();
            reqContent.Add("url", this.reqHandler.getGateUrl());
            reqContent.Add("data", data);
            this.pay.setReqContent(reqContent);
            Console.WriteLine($"------接口通讯发送------：\r\n{data}\r\n");
            if (this.pay.call())
            {
                this.resHandler.setContent(this.pay.getResContent());
                Console.WriteLine($"------接口通讯接收------：\r\n{resHandler.getContent()}\r\n");

                DataSet ds = ConvertXMLToDataSet(resHandler.getContent());
                if (ds.Tables.Count > 0)
                {
                    dtnew = ds.Tables[0];
                    dtnew.TableName = "interface_gjzh_fkmsk_ret";
                }
                Hashtable param = this.resHandler.getAllParameters();
                if (int.Parse(param["status"].ToString()) == 0)
                {
                    if (this.resHandler.isTenpaySign())
                    {
                        /*当调用支付接口后各个情况的处理可参考如下方案：
                            1、 支付请求后：status和result code字段返回都为0时，判定订单支付成功；
                            2、 支付请求后：status返回为0，result code返回不为0时，根据返回字段“need query ”来判定：
                            ①“need query” 返回为 Y  则调用订单查询接口（建议 ：查询6次每隔5秒查询一次 ，具体的查询次数和时间也可自定义，建议查询时间不低于30秒）6次查询走完， 接口仍未返回成功标识(即查询接口返回的trade_state不是等于SUCCESS)则调用冲正接口进行 关单；
                            3、在status字段返回都为不为0时，建议也调用订单查询接口，调用建议方式如第2点
                         *  4、如果没有返回need query参数，先输出错误信息，再通过人工判断是否手动继续执行查询；建议开发一个手动查询功能，避免特殊情况的返回结果。
                         * 
                         */
                        #region 逻辑处理
                        Utils.writeFile("请求响应结果日志", param);
                        if (int.Parse(param["status"].ToString()) != 0)
                        {
                            if (param["message"] != null)
                            {
                                //txt_err.Text = param["message"].ToString();
                                ls_retmsg = param["message"].ToString();
                            }
                            else if (param["err_msg"] != null)
                            {
                                //txt_err.Text = param["err_msg"].ToString();
                                ls_retmsg = param["err_msg"].ToString();
                            }
                            else { }
                            if (param["need_query"] != null)
                            {
                                if (param["need_query"].ToString() != "N")
                                {
                                    //txt_err.Text = "确认顾客情况后，需要手动查询结果！";
                                    ls_retmsg = "确认顾客情况后，需要手动查询结果！";
                                    ls_sql = "update interface_gjzh_fkmsk set MESSAGE = '" + ls_retmsg + "',STATUS = '2' where out_trade_no = '" + ls_sendtext + "'";
                                    dao.SqlDataTableCommit(ls_sql);
                                }
                                else
                                {
                                    ls_sql = "delete from interface_gjzh_fkmsk where out_trade_no = '" + ls_sendtext + "'";
                                    dao.SqlDataTableCommit(ls_sql);
                                }
                            }
                            return ls_retmsg;
                        }
                        if (int.Parse(param["status"].ToString()) == 0 && int.Parse(param["result_code"].ToString()) != 0)
                        {
                            if (param["message"] != null)
                            {
                                //txt_err.Text = param["message"].ToString();
                                ls_retmsg = param["message"].ToString();
                            }
                            else if (param["err_msg"] != null)
                            {
                                //txt_err.Text = param["err_msg"].ToString();
                                ls_retmsg = param["err_msg"].ToString();
                            }
                            else { }
                            if (param["need_query"] != null)
                            {
                                if (param["need_query"].ToString() != "N")
                                {
                                    //txt_err.Text = "确认顾客情况后，需要手动查询结果！";
                                    ls_retmsg = "确认顾客情况后，需要手动查询结果！";
                                    ls_sql = "update interface_gjzh_fkmsk set MESSAGE = '" + ls_retmsg + "',STATUS = '2' where out_trade_no = '" + ls_sendtext + "'";
                                    dao.SqlDataTableCommit(ls_sql);
                                }
                                else
                                {
                                    ls_sql = "delete from interface_gjzh_fkmsk where out_trade_no = '" + ls_sendtext + "'";
                                    dao.SqlDataTableCommit(ls_sql);
                                }
                            }
                            return ls_retmsg;
                        }
                        #endregion
                        if (int.Parse(param["status"].ToString()) == 0 && int.Parse(param["result_code"].ToString()) == 0)
                        {
                            //更新商户订单系统DB,提示成功或跳转完成页面。
                            //Response.Redirect("payCode.aspx"); 
                            //txt_err.Text = "支付成功！";
                            dao.InsertDataTable(dtnew);
                            ls_sql = "update interface_gjzh_fkmsk set MESSAGE = '支付成功',STATUS = '1' where out_trade_no = '" + ls_sendtext + "'";
                            dao.SqlDataTableCommit(ls_sql);
                            return "TRUE";
                        }
                        else
                        {
                            ls_retmsg = "未知错误！";
                            ls_sql = "delete from interface_gjzh_fkmsk where out_trade_no = '" + ls_sendtext + "'";
                            dao.SqlDataTableCommit(ls_sql);
                            return ls_retmsg;
                        }
                    }
                    else
                    {
                        ls_retmsg = "未知错误！";
                        ls_sql = "delete from interface_gjzh_fkmsk where out_trade_no = '" + ls_sendtext + "'";
                        dao.SqlDataTableCommit(ls_sql);
                        return ls_retmsg;
                    }
                }
                else
                {
                    Utils.writeFile("请求响应结果日志", param);
                    ls_retmsg = "返回结果校验签名错误，请检查签名算法及核查结果内容是否正确！";
                    ls_sql = "delete from interface_gjzh_fkmsk where out_trade_no = '" + ls_sendtext + "'";
                    dao.SqlDataTableCommit(ls_sql);
                    return ls_retmsg;
                    //Response.Write("<script>alert('返回结果校验签名错误，请检查签名算法及核查结果内容是否正确！')</script>");
                }
            }
            else
            {
                //txt_err.Text = "错误代码：" + this.pay.getResponseCode() + "错误信息：" + this.pay.getErrInfo();
                ls_retmsg = "错误代码：" + this.pay.getResponseCode() + "错误信息：" + this.pay.getErrInfo();
                ls_sql = "delete from interface_gjzh_fkmsk where out_trade_no = '" + ls_sendtext + "'";
                dao.SqlDataTableCommit(ls_sql);
                return ls_retmsg;
            }
        }
        #endregion

        #region 兴E付支付查询
        public string interface_xyf_find(string ls_sendtext, string ls_param1, string ls_param2, string ls_param3, string ls_param4, string ls_param5)
        {
            string ls_retmsg = "";
            string ls_sql = "";
            DataTable dtnew;
            string result = SelectPayCMD(ls_sendtext, out dtnew,ls_param1, ls_param2, ls_param3, ls_param4, ls_param5);//查询结果赋值
            //判断结果值输出对应提示。
            if (result == "SUCCESS")
            {
                ls_retmsg = "支付成功！";
                dao.InsertDataTable(dtnew);
                ls_sql = "update interface_gjzh_fkmsk set MESSAGE = '" + ls_retmsg + "',STATUS = '1' where out_trade_no = '" + ls_sendtext + "'";
                dao.SqlDataTableCommit(ls_sql);
                return "TRUE";
            }
            else if (result == "REVOKED")
            {
                ls_retmsg = "已冲正！";
                ls_sql = "update interface_gjzh_fkmsk set MESSAGE = '" + ls_retmsg + "',STATUS = '3' where out_trade_no = '" + ls_sendtext + "'";
                dao.SqlDataTableCommit(ls_sql);
            }
            else if (result == "NOTPAY")
            {
                ls_retmsg = "未支付！";
                ls_sql = "update interface_gjzh_fkmsk set MESSAGE = '" + ls_retmsg + "',STATUS = '0' where out_trade_no = '" + ls_sendtext + "'";
                dao.SqlDataTableCommit(ls_sql);
            }
            else if (result == "PAYERROR")
            {
                ls_retmsg = "支付失败！";
                ls_sql = "update interface_gjzh_fkmsk set MESSAGE = '" + ls_retmsg + "',STATUS = '4' where out_trade_no = '" + ls_sendtext + "'";
                dao.SqlDataTableCommit(ls_sql);
            }
            else if (result == "订单不存在")
            {
                ls_retmsg = "订单不存在！";
                ls_sql = "update interface_gjzh_fkmsk set MESSAGE = '" + ls_retmsg + "',STATUS = '9' where out_trade_no = '" + ls_sendtext + "'";
                dao.SqlDataTableCommit(ls_sql);
            }
            else
            {
                ls_retmsg = result;
            }
            return ls_retmsg;
        }

        private string SelectPayCMD(string ls_orderid, out DataTable dtnew, string ls_param1, string ls_param2, string ls_param3, string ls_param4, string ls_param5)
        {
            dtnew = null;
            this.reqHandler = new RequestHandler(null);
            //加载配置数据
            //this.cfg = Utils.loadCfg(); ;
            //初始化数据 
            this.reqHandler.setGateUrl(ls_param2);
            //this.reqHandler.setKey(this.cfg["key"].ToString());
            this.reqHandler.setParameter("out_trade_no", ls_orderid);//商户订单号             
            this.reqHandler.setParameter("service", "unified.trade.query");//接口 unified.trade.query 
            this.reqHandler.setParameter("mch_id", ls_param1);//必填项，商户号，由平台分配
            this.reqHandler.setParameter("version", ls_param5);//接口版本号
            this.reqHandler.setParameter("sign_type", "RSA_1_1");//签名方式
            this.reqHandler.setParameter("nonce_str", Utils.random());//随机字符串，必填项，不长于 32 位
            this.reqHandler.createSign(ls_param3);//创建签名
            //以上参数进行签名
            string sdata = Utils.toXml(this.reqHandler.getAllParameters());//生成XML报文
            Dictionary<string, string> sreqContent = new Dictionary<string, string>();
            sreqContent.Add("url", this.reqHandler.getGateUrl());
            sreqContent.Add("data", sdata);
            this.pay.setReqContent(sreqContent);
            this.pay.call();
            this.resHandler.setContent(this.pay.getResContent());
            //this.resHandler.setKey(this.cfg["key"].ToString());
            Hashtable sparam = this.resHandler.getAllParameters();

            DataSet ds = ConvertXMLToDataSet(resHandler.getContent());
            if (ds.Tables.Count > 0)
            {
                dtnew = ds.Tables[0];
                dtnew.TableName = "interface_gjzh_fkmcx_ret";
            }
            //当返回状态与业务结果都为0时才返回结果，其它结果请查看接口文档
            if (int.Parse(sparam["status"].ToString()) == 0 && int.Parse(sparam["result_code"].ToString()) == 0)
            {
                //查询订单成功更新商户订单系统DB,结束轮询机制
                return sparam["trade_state"].ToString();
            }
            else
            {
                if (sparam["message"] != null)
                {
                    return sparam["message"].ToString();
                }
                else if (sparam["err_msg"] != null)
                {
                    return sparam["err_msg"].ToString();
                }
                else
                {
                    return "";
                }
            }

        }
        #endregion

        #region 兴E付退款查询
        public string interface_xyf_ret_find(string ls_sendtext, string ls_param1, string ls_param2, string ls_param3, string ls_param4, string ls_param5)
        {
            string ls_retmsg = "";
            string ls_sql = "";
            string ls_out_refund_no;
            string ls_refund_id;
            DataTable dt;
            dt = dao.GetDataTable("select nvl(out_refund_no,'') as out_refund_no,nvl(refund_id,'') as refund_id,nvl(time_start,'') as time_start,nvl(time_expire,'') as time_expire,nvl(attach,'') as attach,nvl(body,'') as body,nvl(total_fee,'') as total_fee,nvl(mch_create_ip,'') as mch_create_ip,nvl(auth_code,'') as auth_code from interface_gjzh_fkmsk where out_trade_no = '" + ls_sendtext + "'");
            if (dt.Rows.Count == 0)
            {
                ls_retmsg = "无法识别的订单号！";
                return ls_retmsg;
            }
            ls_out_refund_no = dt.Rows[0]["out_refund_no"].ToString();
            ls_refund_id = dt.Rows[0]["refund_id"].ToString();
            this.reqHandler = new RequestHandler(null);
            //加载配置数据
            //this.cfg = Utils.loadCfg(); ;
            //初始化数据 
            this.reqHandler.setGateUrl(ls_param2);
            //this.reqHandler.setKey(this.cfg["key"].ToString());
            this.reqHandler.setParameter("out_trade_no", ls_sendtext);//商户订单号
            this.reqHandler.setParameter("transaction_id", "");//平台订单号     
            this.reqHandler.setParameter("out_refund_no", ls_out_refund_no);//商户退款单号
            this.reqHandler.setParameter("refund_id", ls_refund_id);//平台退款单号 
            this.reqHandler.setParameter("sign_type", "RSA_1_1");//签名方式
            this.reqHandler.setParameter("service", "unified.trade.refundquery");//接口 unified.trade.refundquery 
            this.reqHandler.setParameter("mch_id", ls_param1);//必填项，商户号，由平台分配
            this.reqHandler.setParameter("version", ls_param5);//接口版本号 
            this.reqHandler.setParameter("nonce_str", Utils.random());//随机字符串，必填项，不长于 32 位
            this.reqHandler.createSign(ls_param3);//创建签名
                                         //以上参数进行签名
            string data = Utils.toXml(this.reqHandler.getAllParameters());//生成XML报文
            Dictionary<string, string> reqContent = new Dictionary<string, string>();
            reqContent.Add("url", this.reqHandler.getGateUrl());
            reqContent.Add("data", data);

            this.pay.setReqContent(reqContent);

            if (this.pay.call())
            {
                this.resHandler.setContent(this.pay.getResContent());
                //this.resHandler.setKey(this.cfg["key"].ToString());
                Hashtable param = this.resHandler.getAllParameters();
                if (this.resHandler.isTenpaySign())
                {
                    //当返回状态与业务结果都为0时才返回结果，其它结果请查看接口文档
                    if (int.Parse(param["status"].ToString()) == 0 && int.Parse(param["result_code"].ToString()) == 0)
                    {
                        Utils.writeFile("查询退款", param);
                        //Response.Write("<script>alert('查询退款成功，请查看result.txt文件！')</script>");
                        //ls_retmsg = "退款处理中";
                        if(param["refund_status_0"].ToString() == "SUCCESS")
                        {
                            ls_retmsg = "退款成功";
                            ls_sql = "update interface_gjzh_fkmsk set MESSAGE = '退款成功',STATUS = '6' where out_trade_no = '" + ls_sendtext + "'";
                            dao.SqlDataTableCommit(ls_sql);
                            return "TRUE";
                        }
                        else if (param["refund_status_0"].ToString() == "FAIL")
                        {
                            ls_retmsg = "退款失败";
                            ls_sql = "update interface_gjzh_fkmsk set MESSAGE = '退款失败',STATUS = '7' where out_trade_no = '" + ls_sendtext + "'";
                            dao.SqlDataTableCommit(ls_sql);
                        }
                        else if (param["refund_status_0"].ToString() == "CHANGE")
                        {
                            ls_retmsg = "转入代发";
                            ls_sql = "update interface_gjzh_fkmsk set MESSAGE = '转入代发',STATUS = '8' where out_trade_no = '" + ls_sendtext + "'";
                            dao.SqlDataTableCommit(ls_sql);
                        }
                        else
                        {
                            ls_retmsg = "退款处理中";
                            ls_sql = "update interface_gjzh_fkmsk set MESSAGE = '退款处理中',STATUS = '5' where out_trade_no = '" + ls_sendtext + "'";
                            dao.SqlDataTableCommit(ls_sql);
                        }
                    }
                    else
                    {
                        //Response.Write("<script>alert('错误代码：" + param["err_code"] + ",错误信息：" + param["err_msg"] + "')</script>");
                        ls_retmsg = "错误代码：" + param["err_code"] + ",错误信息：" + param["err_msg"] + "";
                    }

                }
                else
                {
                    //Response.Write("<script>alert('错误代码：" + param["status"] + ",错误信息：" + param["message"] + "')</script>");
                    ls_retmsg = "错误代码：" + param["status"] + ",错误信息：" + param["message"] + "";
                }
            }
            else
            {
                //Response.Write("<script>alert('错误代码：" + this.pay.getResponseCode() + ",错误信息：" + this.pay.getErrInfo() + "')</script>");
                ls_retmsg = "错误代码：" + this.pay.getResponseCode() + ",错误信息：" + this.pay.getErrInfo() + "";
            }
            return ls_retmsg;
        }
        #endregion

        #region 兴E付退款
        public string interface_xyf_ret(string ls_sendtext, string ls_param1, string ls_param2, string ls_param3, string ls_param4, string ls_param5)
        {
            string ls_retmsg;
            string ls_tbtotal_fee;
            string ls_sql;
            string ls_out_refund_no;
            string ls_refund_id;
            DataTable dt;
            dt = dao.GetDataTable("select nvl(out_refund_no,'') as out_refund_no,nvl(refund_id,'') as refund_id,nvl(time_start,'') as time_start,nvl(time_expire,'') as time_expire,nvl(attach,'') as attach,nvl(body,'') as body,nvl(total_fee,'') as total_fee,nvl(mch_create_ip,'') as mch_create_ip,nvl(auth_code,'') as auth_code from interface_gjzh_fkmsk where out_trade_no = '" + ls_sendtext + "'");
            if (dt.Rows.Count == 0)
            {
                ls_retmsg = "无法识别的订单号！";
                return ls_retmsg;
            }
            ls_tbtotal_fee = dt.Rows[0]["total_fee"].ToString();

            this.reqHandler = new RequestHandler(null);
            //加载配置数据
            //this.cfg = Utils.loadCfg(); ;
            //初始化数据
            this.reqHandler.setGateUrl(ls_param2);
            //this.reqHandler.setKey(this.cfg["key"].ToString());
            this.reqHandler.setParameter("out_trade_no", ls_sendtext);//商户订单号
            this.reqHandler.setParameter("transaction_id", "");//平台订单号     
            this.reqHandler.setParameter("out_refund_no", Utils.Nmrandom());//商户退款单号
            this.reqHandler.setParameter("total_fee", ls_tbtotal_fee);//总金额
            this.reqHandler.setParameter("refund_fee", ls_tbtotal_fee);//退款金额
            this.reqHandler.setParameter("refund_channel", "ORIGINAL");//退款渠道
            this.reqHandler.setParameter("sign_type", "RSA_1_1");//签名方式
            this.reqHandler.setParameter("service", "unified.trade.refund");//接口 unified.trade.refund 
            this.reqHandler.setParameter("mch_id", ls_param1);//必填项，商户号，由平台分配
            this.reqHandler.setParameter("version", ls_param5);//接口版本号
            this.reqHandler.setParameter("op_user_id", ls_param1);//必填项，操作员帐号,默认为商户号
            this.reqHandler.setParameter("nonce_str", Utils.random());//随机字符串，必填项，不长于 32 位
            this.reqHandler.createSign(ls_param3);//创建签名
                                         //以上参数进行签名
            string data = Utils.toXml(this.reqHandler.getAllParameters());//生成XML报文
            Dictionary<string, string> reqContent = new Dictionary<string, string>();
            reqContent.Add("url", this.reqHandler.getGateUrl());
            reqContent.Add("data", data);

            this.pay.setReqContent(reqContent);

            if (this.pay.call())
            {
                this.resHandler.setContent(this.pay.getResContent());
                //this.resHandler.setKey(this.cfg["key"].ToString());
                Hashtable param = this.resHandler.getAllParameters();
                if (this.resHandler.isTenpaySign())
                {
                    //当返回状态与业务结果都为0时才返回结果，其它结果请查看接口文档
                    if (int.Parse(param["status"].ToString()) == 0 && int.Parse(param["result_code"].ToString()) == 0)
                    {
                        Utils.writeFile("提交退款", param);
                        //Response.Write("<script>alert('提交退款成功，请查看result.txt文件！')</script>");
                        ls_retmsg = "退款处理中";
                        ls_out_refund_no = param["out_refund_no"].ToString();
                        ls_refund_id = param["refund_id"].ToString();
                        ls_sql = "update interface_gjzh_fkmsk set MESSAGE = '提交退款成功',STATUS = '5',out_refund_no = '"+ls_out_refund_no+"',refund_id = '"+ls_refund_id+"' where out_trade_no = '" + ls_sendtext + "'";
                        dao.SqlDataTableCommit(ls_sql);
                        return ls_retmsg;
                    }
                    else
                    {
                        //Response.Write("<script>alert('错误代码：" + param["err_code"] + ",错误信息：" + param["err_msg"] + "')</script>");
                        ls_retmsg = "错误代码：" + param["err_code"] + ",错误信息：" + param["err_msg"] + "";
                    }
                }
                else
                {
                    //Response.Write("<script>alert('错误代码：" + param["status"] + ",错误信息：" + param["message"] + "')</script>");
                    ls_retmsg = "错误代码：" + param["status"] + ",错误信息：" + param["message"] + "";
                }
            }
            else
            {
                // Response.Write("<script>alert('错误代码：" + this.pay.getResponseCode() + ",错误信息：" + this.pay.getErrInfo() + "')</script>");
                ls_retmsg = "错误代码：" + this.pay.getResponseCode() + ",错误信息：" + this.pay.getErrInfo() + "";
            }
            return ls_retmsg;
        }
        #endregion

        #region 兴E付冲正
        public string interface_xyf_cancel(string ls_sendtext, string ls_param1, string ls_param2, string ls_param3, string ls_param4, string ls_param5)
        {
            string ls_retmsg;
            string ls_sql;
            if (ReserveOrder(ls_sendtext, ls_param1, ls_param2,ls_param3, ls_param4, ls_param5))
            {
                ls_retmsg = "冲正成功！";
                ls_sql = "update interface_gjzh_fkmsk set MESSAGE = '冲正成功',STATUS = '3' where out_trade_no = '" + ls_sendtext + "'";
                dao.SqlDataTableCommit(ls_sql);
                return "TRUE";
            }
            else
            {
                ls_retmsg = "冲正失败，请继续！";
            }
            return ls_retmsg;
        }
        private bool ReserveOrder(string ls_orderid, string ls_param1, string ls_param2, string ls_param3, string ls_param4, string ls_param5)
        {
            this.reqHandler = new RequestHandler(null);
            //加载配置数据
            //this.cfg = Utils.loadCfg(); ;
            //初始化数据 
            this.reqHandler.setGateUrl(ls_param2);
            //this.reqHandler.setKey(this.cfg["key"].ToString());
            this.reqHandler.setParameter("out_trade_no", ls_orderid);//商户订单号             
            this.reqHandler.setParameter("service", "unified.micropay.reverse");//接口 unified.micropay.reverse
            this.reqHandler.setParameter("mch_id", ls_param1);//必填项，商户号，由平台分配
            this.reqHandler.setParameter("version", ls_param5);//接口版本号
            this.reqHandler.setParameter("sign_type", "RSA_1_1");//签名方式
            this.reqHandler.setParameter("nonce_str", Utils.random());//随机字符串，必填项，不长于 32 位
            this.reqHandler.createSign(ls_param3);//创建签名
            //以上参数进行签名
            string sdata = Utils.toXml(this.reqHandler.getAllParameters());//生成XML报文
            Dictionary<string, string> sreqContent = new Dictionary<string, string>();
            sreqContent.Add("url", this.reqHandler.getGateUrl());
            sreqContent.Add("data", sdata);
            this.pay.setReqContent(sreqContent);
            this.pay.call();
            this.resHandler.setContent(this.pay.getResContent());
            //this.resHandler.setKey(this.cfg["key"].ToString());
            Hashtable sparam = this.resHandler.getAllParameters();
            //当返回状态与业务结果都为0时才返回结果，其它结果请查看接口文档
            if (int.Parse(sparam["status"].ToString()) == 0 && int.Parse(sparam["result_code"].ToString()) == 0)
            {
                //冲正成功
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region 亿保收银
        public string interface_yb_sy(string reqdata, string ls_param1, string ls_param2, string ls_param3, string ls_param4, string ls_param5)
        {
            string ls_retmsg = "";
            this.reqHandler = new RequestHandler(null);
            Dictionary<string, string> reqContent = new Dictionary<string, string>();
            //this.cfg = Utils.loadCfg(); ;
            this.reqHandler.setGateUrl_ybsy(ls_param1);
            reqContent.Add("url", this.reqHandler.getGateUrl_ybsy());
            reqContent.Add("data", reqdata);

            this.pay.setReqContent(reqContent);

            if (this.pay.call())
            {
                ls_retmsg = this.pay.getResContent();
                return ls_retmsg;
            }
            else
            {
                ls_retmsg = "错误代码：" + this.pay.getResponseCode() + ",错误信息：" + this.pay.getErrInfo() + "";
            }
            return ls_retmsg;
        }
        #endregion

        #region 亿保查询余额
        public string interface_yb_cxye(string reqdata, string ls_param1, string ls_param2, string ls_param3, string ls_param4, string ls_param5)
        {
            string ls_retmsg = "";
            this.reqHandler = new RequestHandler(null);
            //this.cfg = Utils.loadCfg(); ;
            Dictionary<string, string> reqContent = new Dictionary<string, string>();
            this.reqHandler.setGateUrl_ybcxye(ls_param2);
            reqContent.Add("url", this.reqHandler.getGateUrl_ybcxye());
            reqContent.Add("data", reqdata);

            this.pay.setReqContent(reqContent);

            if (this.pay.call())
            {
                ls_retmsg = this.pay.getResContent();
                return ls_retmsg;
            }
            else
            {
                ls_retmsg = "错误代码：" + this.pay.getResponseCode() + ",错误信息：" + this.pay.getErrInfo() + "";
            }
            return ls_retmsg;
        }
        #endregion

        #region 亿保交易询问
        public string interface_yb_xw(string reqdata, string ls_param1, string ls_param2, string ls_param3, string ls_param4, string ls_param5)
        {
            string ls_retmsg = "";
            this.reqHandler = new RequestHandler(null);
            //this.cfg = Utils.loadCfg(); ;
            Dictionary<string, string> reqContent = new Dictionary<string, string>();
            this.reqHandler.setGateUrl_ybxw(ls_param3);
            reqContent.Add("url", this.reqHandler.getGateUrl_ybxw());
            reqContent.Add("data", reqdata);

            this.pay.setReqContent(reqContent);

            if (this.pay.call())
            {
                ls_retmsg = this.pay.getResContent();
                return ls_retmsg;
            }
            else
            {
                ls_retmsg = "错误代码：" + this.pay.getResponseCode() + ",错误信息：" + this.pay.getErrInfo() + "";
            }
            return ls_retmsg;
        }
        #endregion

        #region 亿保退费
        public string interface_yb_ret(string reqdata, string ls_param1, string ls_param2, string ls_param3, string ls_param4, string ls_param5)
        {
            string ls_retmsg = "";
            this.reqHandler = new RequestHandler(null);
            //this.cfg = Utils.loadCfg(); ;
            Dictionary<string, string> reqContent = new Dictionary<string, string>();
            this.reqHandler.setGateUrl_ybret(ls_param4);
            reqContent.Add("url", this.reqHandler.getGateUrl_ybret());
            reqContent.Add("data", reqdata);

            this.pay.setReqContent(reqContent);

            if (this.pay.call())
            {
                ls_retmsg = this.pay.getResContent();
                return ls_retmsg;
            }
            else
            {
                ls_retmsg = "错误代码：" + this.pay.getResponseCode() + ",错误信息：" + this.pay.getErrInfo() + "";
            }
            return ls_retmsg;
        }
        #endregion
    }
}