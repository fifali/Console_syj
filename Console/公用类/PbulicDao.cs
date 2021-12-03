using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;
namespace ConsoleHydee
{
    public class PbulicDao
    {
        #region 变量定义
        public Oracle.ManagedDataAccess.Client.OracleTransaction trans;/*事务处理类*/
        public Oracle.ManagedDataAccess.Client.OracleCommand cmd = null;
        public bool inTransaction = false;/*指示当前是否正处于事务中*/
        public Oracle.ManagedDataAccess.Client.OracleConnection cn; /*数据库连接*/
        #endregion

        #region 结构化
        public PbulicDao()
        {

        }
        #endregion

        #region 方法
        public void WriteLog(string ls_operuser, DateTime ls_opertime, string ls_text, string ls_text2, string ls_text3, string ls_text4, string ls_readtext, string ls_sendtext, string funcid, string ls_status)
        {
            try
            {
                string ls_sql;
                //Open();
                //BeginTrans();
                if (ls_text == String.Empty || ls_text == null)
                {
                    ls_text = " ";
                }
                if (ls_text2 == String.Empty || ls_text2 == null)
                {
                    ls_text2 = " ";
                }
                if (ls_text3 == String.Empty || ls_text3 == null)
                {
                    ls_text3 = " ";
                }
                if (ls_text4 == String.Empty || ls_text4 == null)
                {
                    ls_text4 = " ";
                }
                if (ls_readtext == String.Empty || ls_readtext == null)
                {
                    ls_readtext = " ";
                }
                if (ls_sendtext == String.Empty || ls_sendtext == null)
                {
                    return;
                }
                ls_text = ls_text.Replace("'", "''");
                ls_text2 = ls_text2.Replace("'", "''");
                ls_text3 = ls_text3.Replace("'", "''");
                ls_text4 = ls_text4.Replace("'", "''");
                ls_readtext = ls_readtext.Replace("'", "''");
                ls_sendtext = ls_sendtext.Replace("'", "''");
                cn = new Oracle.ManagedDataAccess.Client.OracleConnection(ls_sendtext);
                cn.Open();
                ls_sql = @"insert into t_gy_webserviceInterfaceLog(LOGGUID,
                                           LOGID,
                                           LOGTEXT,
                                           LOGTEXTD,
                                           LOGTEXT2,
                                           LOGTEXT3,
                                           OPERDATE,
                                           OPERUSER,
                                           REMARK,
                                           SENDTO,
                                           READTO,
                                            FuncID,
                                            status)
                                           values
                                           (
                                           createguid(),
                                            " + funcid + @",
                                           :exper,               
                                           :exper3,                
                                           :exper2,              
                                           :exper4,              
                                           sysdate,               
                                           '" + ls_operuser + @"',               
                                           null,               
                                           '" + ls_sendtext + @"',               
                                           '" + ls_readtext + @"',              
                                           '" + funcid + @"',
                                           " + ls_status + @"                                         
                                           )";
                cmd = new Oracle.ManagedDataAccess.Client.OracleCommand(ls_sql, cn);
                OracleParameter op = new OracleParameter("exper", OracleDbType.Clob);
                op.Value = ls_text;
                OracleParameter op3 = new OracleParameter("exper3", OracleDbType.Clob);
                op3.Value = ls_text3;
                OracleParameter op2 = new OracleParameter("exper2", OracleDbType.Clob);
                op2.Value = ls_text2;
                OracleParameter op4 = new OracleParameter("exper4", OracleDbType.Clob);
                op4.Value = ls_text4;
                cmd.Parameters.Add(op);
                cmd.Parameters.Add(op3);
                cmd.Parameters.Add(op2);
                cmd.Parameters.Add(op4);
                cmd.ExecuteNonQuery();
                //CommitTrans();
                cmd = null;
                Close();
            }
            catch (Exception ex)
            {
                cmd = null;
                Close();
            }
        }

        public void WriteLog_hydee(string ls_operuser, DateTime ls_opertime, string ls_text, string ls_text2, string ls_text3, string ls_text4, string ls_readtext, string ls_sendtext, string funcid, string ls_status)
        {
            string ls_sql;
            if (cn == null)
            {
                return;
            }
            Open();
            BeginTrans();
            if (ls_text == String.Empty || ls_text == null)
            {
                ls_text = " ";
            }
            if (ls_text2 == String.Empty || ls_text2 == null)
            {
                ls_text2 = " ";
            }
            if (ls_text3 == String.Empty || ls_text3 == null)
            {
                ls_text3 = " ";
            }
            if (ls_text4 == String.Empty || ls_text4 == null)
            {
                ls_text4 = " ";
            }
            if (ls_readtext == String.Empty || ls_readtext == null)
            {
                ls_readtext = " ";
            }
            if (ls_sendtext == String.Empty || ls_sendtext == null)
            {
                ls_sendtext = " ";
            }
            ls_text = ls_text.Replace("'", "''");
            ls_text2 = ls_text2.Replace("'", "''");
            ls_text3 = ls_text3.Replace("'", "''");
            ls_text4 = ls_text4.Replace("'", "''");
            ls_readtext = ls_readtext.Replace("'", "''");
            ls_sendtext = ls_sendtext.Replace("'", "''");
            ls_sql = @"insert into Tb_hydee_webserviceLog (
                                           LOGID,
                                           LOGTEXT,
                                           LOGTEXT1,
                                           LOGTEXT2,
                                           LOGTEXT3,
                                           OPERDATE,
                                           OPERUSER,
                                           REMARK,
                                           SENDTO,
                                           READTO,
                                            FuncID,
                                            funccode)
                                           values
                                           (
                                           (select nvl(max(logid),0) + 1 from Tb_hydee_webserviceLog),
                                           '" + ls_text + @"', 
                                           '" + ls_text2 + @"',   
                                           '" + ls_text3 + @"',  
                                           '" + ls_text4 + @"',          
                                           sysdate,               
                                           '" + ls_operuser + @"',               
                                           null,               
                                           '" + ls_sendtext + @"',               
                                           '" + ls_readtext + @"',              
                                           '" + funcid + @"',
                                           '" + ls_status + @"'                                           
                                           )";
            //ls_returnmsg = SqlDataTable(ls_sql);
            cmd = new Oracle.ManagedDataAccess.Client.OracleCommand(ls_sql, cn);
            //OracleParameter op = new OracleParameter("exper", OracleDbType.Clob);
            //op.Value = ls_text;
            //OracleParameter op3 = new OracleParameter("exper3", OracleDbType.Clob);
            //op3.Value = ls_text3;
            //OracleParameter op2 = new OracleParameter("exper2", OracleDbType.Clob);
            //op2.Value = ls_text2;
            //OracleParameter op4 = new OracleParameter("exper4", OracleDbType.Clob);
            //op4.Value = ls_text4;
            //cmd.Parameters.Add(op);
            //cmd.Parameters.Add(op3);
            //cmd.Parameters.Add(op2);
            //cmd.Parameters.Add(op4);
            cmd.ExecuteNonQuery();
            CommitTrans();
            Close();
        }

        public string SqlDataTableCommit(string strSql)
        {
            string ls_return = "";
            Oracle.ManagedDataAccess.Client.OracleTransaction ston = null;
            Oracle.ManagedDataAccess.Client.OracleCommand cmdd = null;
            try
            {
                Open();
                BeginTrans();
                if (!inTransaction)
                {
                    ston = cn.BeginTransaction();
                    inTransaction = true;
                }
                cmdd = new Oracle.ManagedDataAccess.Client.OracleCommand();
                cmdd.Connection = this.cn;

                if (inTransaction)
                {
                    cmdd.Transaction = trans;
                }
                else
                {
                    cmdd.Transaction = ston;
                }

                cmdd.CommandText = strSql;
                cmdd.ExecuteNonQuery();
                cmdd.Transaction.Commit();
            }
            catch (Exception ex)
            {
                ls_return = ex.Message.ToString();
                if (!inTransaction && cn.State.ToString().ToUpper() == "OPEN")
                {
                    ston.Rollback();
                }

                cmdd = null;
                return (ls_return);
            }
            finally
            {
                cmdd = null;
            }
            return ("TRUE");
        }

        public string Doprocedure(string proname, string[] inparam, string[] inparamvalue, string[] inparamtype, string[] outparam, string[] outparamtype, bool ib_commit)
        {
            Int32 returncode = -1;//存储过程返回编码
            string returnmsg = "";//存储过程返回信息
            cmd = cn.CreateCommand();
            OracleParameter param = null;
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            OracleDataAdapter da = null;
            ib_commit = false;
            string ls_cursorname = "";
            string ls_text = "";
            try
            {
                Open();
                BeginTrans();
                if (inTransaction)
                {
                    cmd.Transaction = trans;
                }
                cmd.CommandText = proname;
                cmd.CommandType = CommandType.StoredProcedure;
                for (int i = 0; i < inparam.Length; i++)
                {
                    if (inparamtype[i] == "int")
                    {
                        param = cmd.Parameters.Add(new OracleParameter(inparam[i], OracleDbType.Int32, 8));
                    }
                    else if (inparamtype[i] == "varchar")
                    {
                        param = cmd.Parameters.Add(new OracleParameter(inparam[i], OracleDbType.Varchar2, 400));
                    }
                    param.Direction = ParameterDirection.Input;
                    param.Value = inparamvalue[i];
                }

                if (outparam != null)
                {
                    for (int i = 0; i < outparam.Length; i++)
                    {
                        if (outparamtype[i] == "int")
                        {
                            param = cmd.Parameters.Add(new OracleParameter(outparam[i], OracleDbType.Int32, 4));
                        }
                        else if (outparamtype[i] == "varchar")
                        {
                            param = cmd.Parameters.Add(new OracleParameter(outparam[i], OracleDbType.Varchar2, 400));
                        }
                        else if (outparamtype[i] == "cursor")
                        {
                            param = cmd.Parameters.Add(new OracleParameter(outparam[i], OracleDbType.RefCursor, 400));
                            ls_cursorname = outparam[i];
                        }
                        param.Direction = ParameterDirection.Output;
                        param.Value = ls_text.PadRight(400, ' ');
                    }
                }
                cmd.ExecuteNonQuery();
                //returncode = Convert.ToInt32(cmd.Parameters["as_returncode"].Value.ToString());
                //returnmsg = Convert.ToString(cmd.Parameters["as_returnmsg"].Value.ToString());
                //if (returncode != 0)
                //{
                    //RollbackTrans();
                    //return (returnmsg);
                //}
                //if (ls_cursorname != "")
                //{
                    //if (func == 1001 || func == 1002 || func == 1060 || func == 1061 || func == 1009 || func == 1010 || func == 1013 || func == 1014 || func == 1015 || func == 1019 || func == 1022 || func == 1023 || func == 1033 || func == 1024 || func == 1026 || func == 1027 || func == 1036 || func == 1037 || func == 1046 || func == 1048 || func == 1049 || func == 1050 || func == 1052 || func == 1053 || func == 1054 || func == 1056 || func == 1057 || func == 1101 || func == 1103 || func == 1188 || func == 1039 || func == 1040 || func == 1062 || func == 1063)
                    //{
                    //    da = new OracleDataAdapter(cmd);
                    //    da.TableMappings.Add("Table", ls_cursorname);
                    //    da.Fill(ds);
                    //    dt = ds.Tables[0];
                    //    if (dt.Rows.Count == 0)
                    //    {
                    //        if (func == 1002)
                    //        {
                    //            return ("服务器找不到相关任务");
                    //        }
                    //        else
                    //        {
                    //            //WriteXml(dt);
                    //            //ls_returnxml = doc.InnerXml;
                    //        }
                    //    }
                    //    else
                    //    {
                    //        //WriteXml(dt);
                    //        //ls_returnxml = doc.InnerXml;
                    //    }
                    //}
                //}
                //else
                //{
                    //if (func == 1003)
                    //{
                    //    ls_returnxml += "<?xml version='1.0' encoding='gb2312'?>";
                    //    ls_returnxml += "<function>";
                    //    ls_returnxml += "<data rowcount='1' columns='3'>";
                    //    ls_returnxml += "<row rownum='0'>";
                    //    ls_returnxml += "<column colnum='0' colname='checkguid'>" + Convert.ToString(cmd.Parameters["as_checkguid"].Value.ToString()) + "</column>";
                    //    ls_returnxml += "<column colnum='1' colname='inguid'>" + Convert.ToString(cmd.Parameters["as_inguid"].Value.ToString()) + "</column>";
                    //    ls_returnxml += "<column colnum='2' colname='flag'>" + Convert.ToString(cmd.Parameters["as_flag"].Value.ToString()) + "</column>";
                    //    ls_returnxml += "</row>";
                    //    ls_returnxml += "</data>";
                    //    ls_returnxml += "</function>";
                    //}
                    //else
                    //{
                    //    ls_returnxml = "";
                    //}
                //}
                if (ib_commit)
                {
                    CommitTrans();
                }
                return ("TRUE");
            }
            catch (Exception ex)
            {
                RollbackTrans();
                returnmsg = ex.Message.ToString();
                return (returnmsg);
            }
            finally
            {
                cmd = null;
                if (da != null)
                {
                    da.Dispose();
                }
                if (ds != null)
                {
                    ds.Dispose();
                }
                if (dt != null)
                {
                    dt.Dispose();
                }
            }
        }

        public DataSet GetDataSet(string QueryString)
        {
            DataSet ds = null;
            Oracle.ManagedDataAccess.Client.OracleCommand cmdcur = null;
            OracleDataAdapter ad = null;
            try
            {
                Open();
                BeginTrans();
                cmdcur = new Oracle.ManagedDataAccess.Client.OracleCommand();
                cmdcur.Connection = this.cn;
                if (inTransaction)
                    cmdcur.Transaction = trans;
                ds = new DataSet();
                ad = new OracleDataAdapter();
                cmdcur.CommandText = QueryString;
                ad.SelectCommand = cmdcur;
                ad.Fill(ds);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cmdcur = null;
                ad = null;
            }
            return (ds);
        }

        public DataTable GetDataTable(string QueryString)
        {
            DataSet ds = GetDataSet(QueryString);
            if (ds != null)
            {
                if (ds.Tables.Count > 0)
                {
                    return (ds.Tables[0]);
                }
                else
                {
                    return (new DataTable());
                }
            }
            else
            {
                return (new DataTable());
            }
        }

        public string convertstring(string str/*, DataColumn dtcol*/)
        {
            str = "'" + str + "'";
            return str;
        }

        /*public string InsertAdapter(DataTable dt, string tablename, int row, string columnname, string guid)
        {
            string sql = "insert into " + tablename + "  (" + columnname + ",";
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                if (i < dt.Columns.Count - 1)
                    sql += dt.Columns[i].ColumnName + ",";
                else
                    sql += dt.Columns[i].ColumnName;
            }
            sql += " ) values ('" + guid + "',";

            for (int i = 0; i < dt.Columns.Count; i++)
            {
                if (i < dt.Columns.Count - 1)
                    sql += convertstring(dt.Rows[row][i].ToString().Trim()) + ",";
                else
                    sql += convertstring(dt.Rows[row][i].ToString().Trim());
            }
            sql += " )";

            return (sql);
        }*/

        public string InsertAdapter(DataTable dt, string tablename, int row)
        {
            DataTable dtcur = new DataTable();
            dtcur = GetDataTable("select * from " + tablename + " where 1 = 2");
            string sql = "insert into " + tablename + "  (";
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                if (i < dt.Columns.Count - 1)
                    sql += dt.Columns[i].ColumnName + ",";
                else
                    sql += dt.Columns[i].ColumnName;
            }
            sql += " ) values (";

            for (int i = 0; i < dt.Columns.Count; i++)
            {
                if (dtcur.Columns[dt.Columns[i].ColumnName].DataType == typeof(string))
                {
                    if (i < dt.Columns.Count - 1)
                        sql += convertstring(dt.Rows[row][i].ToString().Trim()) + ",";
                    else
                        sql += convertstring(dt.Rows[row][i].ToString().Trim());
                }
                else if (dtcur.Columns[dt.Columns[i].ColumnName].DataType == typeof(DateTime))
                {
                    if (i < dt.Columns.Count - 1)
                        if (string.IsNullOrEmpty(dt.Rows[row][i].ToString().Trim()))
                        {
                            sql += "null,";
                        }
                        else
                        {
                            sql += "to_date(" + convertstring(dt.Rows[row][i].ToString().Trim()) + ",'yyyymmddhh24miss')" + ",";
                        }
                    else
                        if (string.IsNullOrEmpty(dt.Rows[row][i].ToString().Trim()))
                    {
                        sql += "null";
                    }
                    else
                    {
                        sql += "to_date(" + convertstring(dt.Rows[row][i].ToString().Trim()) + ",'yyyymmddhh24miss')";
                    }
                }
                else
                {
                    if (i < dt.Columns.Count - 1)
                        if (string.IsNullOrEmpty(dt.Rows[row][i].ToString().Trim()))
                        {
                            sql += "null,";
                        }
                        else
                        {
                            sql += dt.Rows[row][i].ToString().Trim() + ",";
                        }
                    else
                        if (string.IsNullOrEmpty(dt.Rows[row][i].ToString().Trim()))
                    {
                        sql += "null";
                    }
                    else
                    {
                        sql += dt.Rows[row][i].ToString().Trim();
                    }
                }
            }
            sql += " )";

            return (sql);
        }

        public string DeleteAdapter(DataTable dt, string tablename, int row)
        {
            string sql = "delete " + tablename + " where ";
            sql += dt.Columns[0].ColumnName + " = ";
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                if (i < dt.Columns.Count - 1)
                    sql += convertstring(dt.Rows[row][i].ToString().Trim()) + ",";
                else
                    sql += convertstring(dt.Rows[row][i].ToString().Trim());
            }
            sql += " ";

            return (sql);
        }

        public string FindAdapter(DataTable dt, int row, string sql)
        {
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                sql += convertstring(dt.Rows[row][i].ToString().Trim()) + ",";
            }

            return (sql);
        }

        public string UpdateAdapter(DataTable dt, string tablename, int row, string ls_upcol)
        {
            DataTable dtcur = new DataTable();
            dtcur = GetDataTable("select * from " + tablename + " where 1 = 2");
            string sql = "update " + tablename + " set ";
            DataColumn[] dtcols = dt.PrimaryKey;
            if (dtcols.Length == 0)
            {
                return "该表没有主键,无法生成修改语句";
            }
            else
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    bool iskey = false;
                    for (int k = 0; k < dtcols.Length; k++)
                    {
                        if (dt.Columns[i].ColumnName.Trim() == dtcols[k].ColumnName.Trim())
                            iskey = true;
                    }

                    if (!iskey)
                    {
                        if (i < dt.Columns.Count - 1)
                        {
                            if (dt.Rows[row][i].ToString().Trim() != "noupdate")
                            {
                                if (dtcur.Columns[dt.Columns[i].ColumnName].DataType == typeof(string))
                                {
                                    sql += dt.Columns[i].ColumnName + "=" + convertstring(dt.Rows[row][i].ToString().Trim()) + ", ";
                                }
                                else if (dtcur.Columns[dt.Columns[i].ColumnName].DataType == typeof(DateTime))
                                {
                                    if (string.IsNullOrEmpty(dt.Rows[row][i].ToString().Trim()))
                                    {
                                        sql += dt.Columns[i].ColumnName + "=" + "null,";
                                    }
                                    else
                                    {
                                        sql += dt.Columns[i].ColumnName + "= to_date(" + convertstring(dt.Rows[row][i].ToString().Trim()) + ",'yyyymmddhh24miss'),";
                                    }
                                }
                                else
                                {
                                    if (string.IsNullOrEmpty(dt.Rows[row][i].ToString().Trim()))
                                    {
                                        sql += dt.Columns[i].ColumnName + "=" + "null,";
                                    }
                                    else
                                    {
                                        sql += dt.Columns[i].ColumnName + "=" + dt.Rows[row][i].ToString().Trim() + ", ";
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (dt.Rows[row][i].ToString().Trim() != "noupdate")
                            {
                                if (dtcur.Columns[dt.Columns[i].ColumnName].DataType == typeof(string))
                                {
                                    sql += dt.Columns[i].ColumnName + "=" + convertstring(dt.Rows[row][i].ToString().Trim());
                                }
                                else if (dtcur.Columns[dt.Columns[i].ColumnName].DataType == typeof(DateTime))
                                {
                                    if (string.IsNullOrEmpty(dt.Rows[row][i].ToString().Trim()))
                                    {
                                        sql += dt.Columns[i].ColumnName + "=" + "null";
                                    }
                                    else
                                    {
                                        sql += dt.Columns[i].ColumnName + "=to_date(" + convertstring(dt.Rows[row][i].ToString().Trim()) + ",'yyyymmddhh24miss')";
                                    }
                                }
                                else
                                {
                                    if (string.IsNullOrEmpty(dt.Rows[row][i].ToString().Trim()))
                                    {
                                        sql += dt.Columns[i].ColumnName + "=" + "null";
                                    }
                                    else
                                    {
                                        sql += dt.Columns[i].ColumnName + "=" + dt.Rows[row][i].ToString().Trim();
                                    }
                                }
                            }
                            else
                            {
                                sql = sql.Substring(0, sql.Trim().Length - 1);
                            }
                        }
                    }
                }
                sql += " where ";
                for (int j = 0; j < dtcols.Length; j++)
                {
                    if (string.IsNullOrEmpty(ls_upcol))
                    {
                        if (j == 0)
                            sql += dtcols[j].ColumnName.Trim() + "=" + convertstring(dt.Rows[row][dtcols[j].ColumnName.Trim()].ToString().Trim()) + "  ";
                        else
                            sql += " and " + dtcols[j].ColumnName.Trim() + "=" + convertstring(dt.Rows[row][dtcols[j].ColumnName.Trim()].ToString().Trim()) + " ";
                    }
                    else
                    {
                        if (j == 0)
                            sql += ls_upcol.Trim() + "=" + convertstring(dt.Rows[row][ls_upcol].ToString().Trim()) + "  ";
                        else
                            sql += " and " + ls_upcol.Trim() + "=" + convertstring(dt.Rows[row][ls_upcol].ToString().Trim()) + " ";
                    }
                }
            }
            return (sql);
        }

        /// <summary> 
        /// 中文转化为GUID（先插入基础字典后转化）
        /// </summary> 
        /// <param name="dt">需转化的数据</param>
        /// <param name="functionid">函数功能ID</param> 
        /// <param name="dbtype">数据操作类型</param> 
        /// <param name="dtnew">转化后的数据</param> 
        /// <returns>返回值</returns>
        public string data_base_do(DataTable dt, string functionid, string dbtype, out DataTable dtnew)
        {
            string ls_name;
            string ls_msg;
            string ls_productareaguid;
            string ls_unitguid;
            string ls_guid;
            long ll_maxproductareacode;
            long ll_maxunitcode;
            string ls_maxproductareacode;
            string ls_maxunitcode;
            DataTable dtcur = new DataTable();
            dtnew = dt;
            if (functionid == "2002")//机构商品目录
            {
                dtcur = GetDataTable("select max(productareacode) from tb_Dms_productarea");
                ls_maxproductareacode = dtcur.Rows[0][0].ToString();
                if (string.IsNullOrEmpty(ls_maxproductareacode))
                {
                    ll_maxproductareacode = 0;
                }
                else
                {
                    ll_maxproductareacode = Convert.ToInt32(ls_maxproductareacode);
                }
                dtcur = GetDataTable("select max(unitcode) from Tb_Dms_WareUnit");
                ls_maxunitcode = dtcur.Rows[0][0].ToString();
                if (string.IsNullOrEmpty(ls_maxunitcode))
                {
                    ll_maxunitcode = 0;
                }
                else
                {
                    ll_maxunitcode = Convert.ToInt32(ls_maxunitcode);
                }
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        if (dt.Columns[j].ColumnName.ToLower() == "productareaguid")
                        {
                            ls_name = dt.Rows[i]["productareaguid"].ToString();
                            if (ls_name == string.Empty && ls_name.Trim() == "")
                            {
                                dtnew.Rows[i][j] = null;
                                continue;
                            }
                            dtcur = GetDataTable("select productareaguid from tb_Dms_productarea where ProductAreaName = '" + ls_name + "'");
                            if (dtcur.Rows.Count == 0)
                            {
                                dtcur = GetDataTable("select createguid() from dual");
                                ls_productareaguid = dtcur.Rows[0][0].ToString();
                                ll_maxproductareacode++;
                                ls_maxproductareacode = ll_maxproductareacode.ToString().PadLeft(5, '0');
                                ls_msg = SqlDataTable("insert into tb_Dms_productarea(productareaguid,ProductAreaCode,ProductAreaName) values('" + ls_productareaguid + "','" + ls_maxproductareacode + "','" + ls_name + "')");
                                if (ls_msg != "TRUE")
                                {
                                    return ls_msg;
                                }
                                ls_guid = ls_productareaguid;
                            }
                            else
                            {
                                ls_productareaguid = dtcur.Rows[0][0].ToString();
                                ls_guid = ls_productareaguid;
                            }
                            dtnew.Rows[i][j] = ls_guid;
                        }
                        else if (dt.Columns[j].ColumnName.ToLower() == "unitguid")
                        {
                            ls_name = dt.Rows[i]["unitguid"].ToString();
                            if (ls_name == string.Empty && ls_name.Trim() == "")
                            {
                                dtnew.Rows[i][j] = null;
                                continue;
                            }
                            dtcur = GetDataTable("select UnitGUID from Tb_Dms_WareUnit where UnitName = '" + ls_name + "'");
                            if (dtcur.Rows.Count == 0)
                            {
                                dtcur = GetDataTable("select createguid() from dual");
                                ls_unitguid = dtcur.Rows[0][0].ToString();
                                ll_maxunitcode++;
                                ls_maxunitcode = ll_maxunitcode.ToString().PadLeft(5, '0');
                                ls_msg = SqlDataTable("insert into Tb_Dms_WareUnit(UnitGUID,UnitCode ,UnitName) values('" + ls_unitguid + "','" + ls_maxunitcode + "','" + ls_name + "')");
                                if (ls_msg != "TRUE")
                                {
                                    return ls_msg;
                                }
                                ls_guid = ls_unitguid;
                            }
                            else
                            {
                                ls_unitguid = dtcur.Rows[0][0].ToString();
                                ls_guid = ls_unitguid;
                            }
                            dtnew.Rows[i][j] = ls_guid;
                        }
                    }
                }
            }
            else if (functionid == "2011")//销售出库订单明细
            {
                string ls_orgguid;
                string ls_wareguid;
                string ls_batchs;
                long l_singnum;
                long l_packnum;
                long l_realsingle;
                long l_realpack;
                long l_PackNumnew;
                long l_singnum2;
                long l_packnum2;
                long l_KeepsingleNum;
                long l_keeppacknum;
                long l_orderKeepsingleNum;
                long l_orderkeeppacknum;
                long l_realsinglesum;
                long l_data;
                long l_ceilnum;
                long l_setpack;
                long l_setsingle;
                dtcur = GetDataTable("select orgguid from Tb_Dms_SaleOutstoreOrder where OrderGUID = '" + dt.Rows[0]["OrderGUID"] + "'");
                ls_orgguid = dtcur.Rows[0][0].ToString();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    ls_wareguid = dt.Rows[i]["wareguid"].ToString();
                    ls_batchs = dt.Rows[i]["batchs"].ToString();
                    dtcur = GetDataTable("SELECT COALESCE(sum(nvl(WareStoreSingleNum,0)),0),COALESCE(sum(nvl(WareStorePackNum,0)),0) FROM Tb_Dms_OrgStore WHERE orgguid = '" + ls_orgguid + "' AND wareguid = '" + ls_wareguid + "' And BATCHS = '" + ls_batchs + "'");
                    l_singnum = dtcur.Rows.Count > 0 ? Convert.ToInt32(dtcur.Rows[0][0]) : 0;
                    l_packnum = dtcur.Rows.Count > 0 ? Convert.ToInt32(dtcur.Rows[0][1]) : 0;
                    dtcur = GetDataTable("SELECT COALESCE(sum(nvl(KeepsingleNum,0)),0),COALESCE(sum(nvl(KeepNum,0)),0) FROM Tb_Dms_PickKeepStore WHERE OrgGUID = '" + ls_orgguid + "' AND wareguid = '" + ls_wareguid + "'And BATCHS = '" + ls_batchs + "'");
                    l_KeepsingleNum = dtcur.Rows.Count > 0 ? Convert.ToInt32(dtcur.Rows[0][0]) : 0;
                    l_keeppacknum = dtcur.Rows.Count > 0 ? Convert.ToInt32(dtcur.Rows[0][1]) : 0;
                    dtcur = GetDataTable("SELECT COALESCE(sum(nvl(KeepsingleNum,0)),0),COALESCE(sum(nvl(KeepNum,0)),0) FROM Tb_Dms_orderKeepStore WHERE OrgGUID = '" + ls_orgguid + "' AND wareguid = '" + ls_wareguid + "' And BATCHS = '" + ls_batchs + "'");
                    l_orderKeepsingleNum = dtcur.Rows.Count > 0 ? Convert.ToInt32(dtcur.Rows[0][0]) : 0;
                    l_orderkeeppacknum = dtcur.Rows.Count > 0 ? Convert.ToInt32(dtcur.Rows[0][1]) : 0;
                    l_singnum2 = l_singnum - l_KeepsingleNum - l_orderKeepsingleNum;
                    l_packnum2 = l_packnum - l_keeppacknum - l_orderkeeppacknum;

                    l_realsingle = l_singnum2;
                    l_realpack = l_packnum2;
                    dtcur = GetDataTable("SELECT PackNum FROM tb_Dms_Dmsware Where WAREGUID = '" + ls_wareguid + "'");
                    l_PackNumnew = dtcur.Rows.Count > 0 ? Convert.ToInt32(dtcur.Rows[0][0]) : 0;
                    l_realsinglesum = l_realpack * l_PackNumnew + l_realsingle;
                    l_data = Convert.ToInt32(dt.Rows[i]["SingleNum"]);
                    if (l_data > l_realsinglesum)
                    {
                        return "商品【" + dt.Rows[i]["warename"].ToString() + "】库存不足";
                    }

                    if (l_packnum == 0)
                    {
                        dtnew.Rows[i]["singlenum"] = l_data;
                        dtnew.Rows[i]["packnum"] = 0;
                    }
                    else
                    {
                        l_ceilnum = Convert.ToInt32(l_data / l_packnum);
                        if (l_ceilnum > l_realpack)
                        {
                            l_setpack = l_realpack;
                            l_setsingle = l_data - l_setpack * l_packnum;
                        }
                        else
                        {
                            l_setpack = l_ceilnum;
                            l_setsingle = l_data - l_setpack * l_packnum;
                        }
                        dtnew.Rows[i]["singlenum"] = l_setsingle;
                        dtnew.Rows[i]["packnum"] = l_setpack;
                    }
                }
            }
            else if (functionid == "2013")//购进退货出库订单明细
            {
                string ls_orgguid;
                string ls_wareguid;
                string ls_batchs;
                long l_singnum;
                long l_packnum;
                long l_realsingle;
                long l_realpack;
                long l_PackNumnew;
                long l_singnum2;
                long l_packnum2;
                long l_KeepsingleNum;
                long l_keeppacknum;
                long l_orderKeepsingleNum;
                long l_orderkeeppacknum;
                long l_realsinglesum;
                long l_data;
                long l_ceilnum;
                long l_setpack;
                long l_setsingle;
                dtcur = GetDataTable("select orgguid from Tb_Dms_SaleOutstoreOrder where OrderGUID = '" + dt.Rows[0]["OrderGUID"] + "'");
                ls_orgguid = dtcur.Rows[0][0].ToString();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    ls_wareguid = dt.Rows[i]["wareguid"].ToString();
                    ls_batchs = dt.Rows[i]["batchs"].ToString();
                    dtcur = GetDataTable("SELECT COALESCE(sum(nvl(WareStoreSingleNum,0)),0),COALESCE(sum(nvl(WareStorePackNum,0)),0) FROM Tb_Dms_OrgStore WHERE orgguid = '" + ls_orgguid + "' AND wareguid = '" + ls_wareguid + "' And BATCHS = '" + ls_batchs + "'");
                    l_singnum = dtcur.Rows.Count > 0 ? Convert.ToInt32(dtcur.Rows[0][0]) : 0;
                    l_packnum = dtcur.Rows.Count > 0 ? Convert.ToInt32(dtcur.Rows[0][1]) : 0;
                    dtcur = GetDataTable("SELECT COALESCE(sum(nvl(KeepsingleNum,0)),0),COALESCE(sum(nvl(KeepNum,0)),0) FROM Tb_Dms_PickKeepStore WHERE OrgGUID = '" + ls_orgguid + "' AND wareguid = '" + ls_wareguid + "'And BATCHS = '" + ls_batchs + "'");
                    l_KeepsingleNum = dtcur.Rows.Count > 0 ? Convert.ToInt32(dtcur.Rows[0][0]) : 0;
                    l_keeppacknum = dtcur.Rows.Count > 0 ? Convert.ToInt32(dtcur.Rows[0][1]) : 0;
                    dtcur = GetDataTable("SELECT COALESCE(sum(nvl(KeepsingleNum,0)),0),COALESCE(sum(nvl(KeepNum,0)),0) FROM Tb_Dms_orderKeepStore WHERE OrgGUID = '" + ls_orgguid + "' AND wareguid = '" + ls_wareguid + "' And BATCHS = '" + ls_batchs + "'");
                    l_orderKeepsingleNum = dtcur.Rows.Count > 0 ? Convert.ToInt32(dtcur.Rows[0][0]) : 0;
                    l_orderkeeppacknum = dtcur.Rows.Count > 0 ? Convert.ToInt32(dtcur.Rows[0][1]) : 0;
                    l_singnum2 = l_singnum - l_KeepsingleNum - l_orderKeepsingleNum;
                    l_packnum2 = l_packnum - l_keeppacknum - l_orderkeeppacknum;

                    l_realsingle = l_singnum2;
                    l_realpack = l_packnum2;
                    dtcur = GetDataTable("SELECT PackNum FROM tb_Dms_Dmsware Where WAREGUID = '" + ls_wareguid + "'");
                    l_PackNumnew = dtcur.Rows.Count > 0 ? Convert.ToInt32(dtcur.Rows[0][0]) : 0;
                    l_realsinglesum = l_realpack * l_PackNumnew + l_realsingle;
                    l_data = Convert.ToInt32(dt.Rows[i]["SingleNum"]);

                    if (l_packnum == 0)
                    {
                        dtnew.Rows[i]["singlenum"] = l_data;
                        dtnew.Rows[i]["packnum"] = 0;
                    }
                    else
                    {
                        l_ceilnum = Convert.ToInt32(l_data / l_packnum);
                        if (l_ceilnum > l_realpack)
                        {
                            l_setpack = l_realpack;
                            l_setsingle = l_data - l_setpack * l_packnum;
                        }
                        else
                        {
                            l_setpack = l_ceilnum;
                            l_setsingle = l_data - l_setpack * l_packnum;
                        }
                        dtnew.Rows[i]["singlenum"] = l_setsingle;
                        dtnew.Rows[i]["packnum"] = l_setpack;
                    }
                }
            }
            return "TRUE";
        }

        public string data_validation(DataTable dt, string functionid, string dbtype)
        {
            if (dt.Rows.Count == 0)
            {
                return "没有传入明细数据，请检查！";
            }
            if (functionid == "2002")//商品目录
            {
                if (dbtype == "1" || dbtype == "2")//新增、更新
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (string.IsNullOrEmpty(dt.Rows[i]["warename"].ToString()))
                        {
                            return "第【" + (i + 1).ToString() + "】行商品名称不能为空！";
                        }
                        else if (string.IsNullOrEmpty(dt.Rows[i]["bar"].ToString()))
                        {
                            return "第【" + (i + 1).ToString() + "】行商品条码不能为空！";
                        }
                        else if (string.IsNullOrEmpty(dt.Rows[i]["WareCode"].ToString()))
                        {
                            return "第【" + (i + 1).ToString() + "】行商品编码不能为空！";
                        }
                        else if (string.IsNullOrEmpty(dt.Rows[i]["PackNum"].ToString()))
                        {
                            return "第【" + (i + 1).ToString() + "】行件装数量不能为空！";
                        }
                        else if (string.IsNullOrEmpty(dt.Rows[i]["PackVolumn"].ToString()))
                        {
                            return "第【" + (i + 1).ToString() + "】行件装体积不能为空！";
                        }
                        else if (string.IsNullOrEmpty(dt.Rows[i]["SingleVolumn"].ToString()))
                        {
                            return "第【" + (i + 1).ToString() + "】行单品体积不能为空！";
                        }
                        else if (string.IsNullOrEmpty(dt.Rows[i]["UnitGUID"].ToString()))
                        {
                            return "第【" + (i + 1).ToString() + "】行单位不能为空！";
                        }
                        else if (string.IsNullOrEmpty(dt.Rows[i]["KindGUID"].ToString()))
                        {
                            return "第【" + (i + 1).ToString() + "】行商品类别不能为空！";
                        }
                        else if (string.IsNullOrEmpty(dt.Rows[i]["SaveKindGUID"].ToString()))
                        {
                            return "第【" + (i + 1).ToString() + "】行存储类别不能为空！";
                        }
                        else if (string.IsNullOrEmpty(dt.Rows[i]["ProductAreaGUID"].ToString()))
                        {
                            return "第【" + (i + 1).ToString() + "】行产地不能为空！";
                        }
                    }
                }
            }
            else if (functionid == "2006")//采购入库订单
            {
                if (dbtype == "1" || dbtype == "2")//新增、更新
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (string.IsNullOrEmpty(dt.Rows[i]["OrgGUID"].ToString()))
                        {
                            return "第【" + (i + 1).ToString() + "】行机构不能为空！";
                        }
                        else if (string.IsNullOrEmpty(dt.Rows[i]["DmsCompanyGUID"].ToString()))
                        {
                            return "第【" + (i + 1).ToString() + "】行往来单位不能为空！";
                        }
                        else if (string.IsNullOrEmpty(dt.Rows[i]["OrderType"].ToString()))
                        {
                            return "第【" + (i + 1).ToString() + "】行订单类型不能为空！";
                        }
                        else if (string.IsNullOrEmpty(dt.Rows[i]["OrderNo"].ToString()))
                        {
                            return "第【" + (i + 1).ToString() + "】行订单编号不能为空！";
                        }
                    }
                }
            }
            else if (functionid == "2007")//采购入库订单明细
            {
                if (dbtype == "1" || dbtype == "2")//新增、更新
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (string.IsNullOrEmpty(dt.Rows[i]["warebar"].ToString()))
                        {
                            return "第【" + (i + 1).ToString() + "】行商品条码不能为空！";
                        }
                        else if (string.IsNullOrEmpty(dt.Rows[i]["OrderGUID"].ToString()))
                        {
                            return "第【" + (i + 1).ToString() + "】行订单号不能为空！";
                        }
                        else if (string.IsNullOrEmpty(dt.Rows[i]["WareGUID"].ToString()))
                        {
                            return "第【" + (i + 1).ToString() + "】行商品ID不能为空！";
                        }
                        else if (string.IsNullOrEmpty(dt.Rows[i]["WareCode"].ToString()))
                        {
                            return "第【" + (i + 1).ToString() + "】行商品编码不能为空！";
                        }
                        else if (string.IsNullOrEmpty(dt.Rows[i]["WareName"].ToString()))
                        {
                            return "第【" + (i + 1).ToString() + "】行商品名称不能为空！";
                        }
                    }
                }
            }
            else if (functionid == "2008")//销售退货入库订单
            {
                if (dbtype == "1" || dbtype == "2")//新增、更新
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (string.IsNullOrEmpty(dt.Rows[i]["OrgGUID"].ToString()))
                        {
                            return "第【" + (i + 1).ToString() + "】行机构不能为空！";
                        }
                        else if (string.IsNullOrEmpty(dt.Rows[i]["DmsCompanyGUID"].ToString()))
                        {
                            return "第【" + (i + 1).ToString() + "】行往来单位不能为空！";
                        }
                        else if (string.IsNullOrEmpty(dt.Rows[i]["OrderType"].ToString()))
                        {
                            return "第【" + (i + 1).ToString() + "】行订单类型不能为空！";
                        }
                        else if (string.IsNullOrEmpty(dt.Rows[i]["OrderNo"].ToString()))
                        {
                            return "第【" + (i + 1).ToString() + "】行订单编号不能为空！";
                        }
                        else if (string.IsNullOrEmpty(dt.Rows[i]["SaleOutstoreOrderGUID"].ToString()))
                        {
                            return "第【" + (i + 1).ToString() + "】行销售出库订单不能为空！";
                        }
                    }
                }
            }
            else if (functionid == "2009")//销售退货入库订单明细
            {
                if (dbtype == "1" || dbtype == "2")//新增、更新
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (string.IsNullOrEmpty(dt.Rows[i]["OrderGUID"].ToString()))
                        {
                            return "第【" + (i + 1).ToString() + "】行订单号不能为空！";
                        }
                        else if (string.IsNullOrEmpty(dt.Rows[i]["WareGUID"].ToString()))
                        {
                            return "第【" + (i + 1).ToString() + "】行商品ID不能为空！";
                        }
                        else if (string.IsNullOrEmpty(dt.Rows[i]["WareCode"].ToString()))
                        {
                            return "第【" + (i + 1).ToString() + "】行商品编码不能为空！";
                        }
                        else if (string.IsNullOrEmpty(dt.Rows[i]["WareName"].ToString()))
                        {
                            return "第【" + (i + 1).ToString() + "】行商品名称不能为空！";
                        }
                    }
                }
            }
            else if (functionid == "2010")//销售出库订单
            {
                if (dbtype == "1" || dbtype == "2")//新增、更新
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (string.IsNullOrEmpty(dt.Rows[i]["OrgGUID"].ToString()))
                        {
                            return "第【" + (i + 1).ToString() + "】行机构不能为空！";
                        }
                        else if (string.IsNullOrEmpty(dt.Rows[i]["DmsCompanyGUID"].ToString()))
                        {
                            return "第【" + (i + 1).ToString() + "】行往来单位不能为空！";
                        }
                        else if (string.IsNullOrEmpty(dt.Rows[i]["OrderType"].ToString()))
                        {
                            return "第【" + (i + 1).ToString() + "】行订单类型不能为空！";
                        }
                        else if (string.IsNullOrEmpty(dt.Rows[i]["OrderCode"].ToString()))
                        {
                            return "第【" + (i + 1).ToString() + "】行订单编号不能为空！";
                        }
                    }
                }
            }
            else if (functionid == "2011")//销售出库订单明细
            {
                if (dbtype == "1" || dbtype == "2")//新增、更新
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (string.IsNullOrEmpty(dt.Rows[i]["OrderGUID"].ToString()))
                        {
                            return "第【" + (i + 1).ToString() + "】行订单号不能为空！";
                        }
                        else if (string.IsNullOrEmpty(dt.Rows[i]["WareGUID"].ToString()))
                        {
                            return "第【" + (i + 1).ToString() + "】行商品ID不能为空！";
                        }
                        else if (string.IsNullOrEmpty(dt.Rows[i]["WareCode"].ToString()))
                        {
                            return "第【" + (i + 1).ToString() + "】行商品编码不能为空！";
                        }
                        else if (string.IsNullOrEmpty(dt.Rows[i]["WareName"].ToString()))
                        {
                            return "第【" + (i + 1).ToString() + "】行商品名称不能为空！";
                        }
                        else if (string.IsNullOrEmpty(dt.Rows[i]["Batchs"].ToString()))
                        {
                            return "第【" + (i + 1).ToString() + "】行批号不能为空！";
                        }
                        else if (string.IsNullOrEmpty(dt.Rows[i]["ExpDate"].ToString()))
                        {
                            return "第【" + (i + 1).ToString() + "】行截至有效期不能为空！";
                        }
                    }
                }
            }
            else if (functionid == "2012")//购进退货出库订单
            {
                if (dbtype == "1" || dbtype == "2")//新增、更新
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (string.IsNullOrEmpty(dt.Rows[i]["OrgGUID"].ToString()))
                        {
                            return "第【" + (i + 1).ToString() + "】行机构不能为空！";
                        }
                        else if (string.IsNullOrEmpty(dt.Rows[i]["DmsCompanyGUID"].ToString()))
                        {
                            return "第【" + (i + 1).ToString() + "】行往来单位不能为空！";
                        }
                        else if (string.IsNullOrEmpty(dt.Rows[i]["OrderCode"].ToString()))
                        {
                            return "第【" + (i + 1).ToString() + "】行订单编号不能为空！";
                        }
                        else if (string.IsNullOrEmpty(dt.Rows[i]["OrderGUID"].ToString()))
                        {
                            return "第【" + (i + 1).ToString() + "】行采购入库订单不能为空！";
                        }
                    }
                }
            }
            else if (functionid == "2013")//购进退货出库订单明细
            {
                if (dbtype == "1" || dbtype == "2")//新增、更新
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (string.IsNullOrEmpty(dt.Rows[i]["OrderGUID"].ToString()))
                        {
                            return "第【" + (i + 1).ToString() + "】行订单号不能为空！";
                        }
                        else if (string.IsNullOrEmpty(dt.Rows[i]["WareGUID"].ToString()))
                        {
                            return "第【" + (i + 1).ToString() + "】行商品ID不能为空！";
                        }
                        else if (string.IsNullOrEmpty(dt.Rows[i]["WareNo"].ToString()))
                        {
                            return "第【" + (i + 1).ToString() + "】行商品编码不能为空！";
                        }
                        else if (string.IsNullOrEmpty(dt.Rows[i]["WareName"].ToString()))
                        {
                            return "第【" + (i + 1).ToString() + "】行商品名称不能为空！";
                        }
                        else if (string.IsNullOrEmpty(dt.Rows[i]["Batchs"].ToString()))
                        {
                            return "第【" + (i + 1).ToString() + "】行批号不能为空！";
                        }
                        else if (string.IsNullOrEmpty(dt.Rows[i]["ExpDate"].ToString()))
                        {
                            return "第【" + (i + 1).ToString() + "】行截至有效期不能为空！";
                        }
                    }
                }
            }
            else if (functionid == "2014")//机构目录
            {
                if (dbtype == "1" || dbtype == "2")//新增、更新
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (string.IsNullOrEmpty(dt.Rows[i]["OrgCode"].ToString()))
                        {
                            return "第【" + (i + 1).ToString() + "】行机构编码不能为空！";
                        }
                        else if (string.IsNullOrEmpty(dt.Rows[i]["OrgName"].ToString()))
                        {
                            return "第【" + (i + 1).ToString() + "】行机构名称不能为空！";
                        }
                    }
                }
            }
            else if (functionid == "2015")//往来单位目录
            {
                if (dbtype == "1" || dbtype == "2")//新增、更新
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (string.IsNullOrEmpty(dt.Rows[i]["NatureGUID"].ToString()))
                        {
                            return "第【" + (i + 1).ToString() + "】行单位性质不能为空！";
                        }
                        else if (string.IsNullOrEmpty(dt.Rows[i]["KindGUID"].ToString()))
                        {
                            return "第【" + (i + 1).ToString() + "】行单位类别不能为空！";
                        }
                        else if (string.IsNullOrEmpty(dt.Rows[i]["CompanyCode"].ToString()))
                        {
                            return "第【" + (i + 1).ToString() + "】行单位编号不能为空！";
                        }
                        else if (string.IsNullOrEmpty(dt.Rows[i]["CompanyName"].ToString()))
                        {
                            return "第【" + (i + 1).ToString() + "】行单位名称不能为空！";
                        }
                    }
                }
            }
            return "TRUE";
        }

        public string InsertDataTable(DataTable dt)
        {
            return InsertDataTable(dt, dt.TableName);
        }

        public string InsertDataTable(DataTable dt, string TableName)
        {
            string strSql = "";
            string ls_return = "";
            cmd = null;
            if (dt.Rows.Count > 0)
            {
                try
                {
                    Open();
                    BeginTrans();
                    cmd = new Oracle.ManagedDataAccess.Client.OracleCommand();
                    cmd.Connection = this.cn;
                    if (inTransaction)
                    {
                        cmd.Transaction = trans;
                    }
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        strSql = InsertAdapter(dt, TableName, i);
                        cmd.CommandText = strSql;
                        cmd.ExecuteNonQuery();
                    }
                    CommitTrans();
                }
                catch (Exception ex)
                {
                    ls_return = ex.Message.ToString();
                    RollbackTrans();
                    cmd = null;
                    return (ls_return);
                }
                finally
                {
                    cmd = null;
                }
            }
            ls_return = "TRUE";
            return (ls_return);
        }

        public string InsertOrUpdateDataTable(DataTable dt)
        {
            return InsertOrUpdateDataTable(dt, dt.PrimaryKey[0].ToString());
        }

        public string InsertOrUpdateDataTable(DataTable dt, string ls_updatecol)
        {
            return InsertOrUpdateDataTable(dt, dt.TableName, ls_updatecol);
        }

        public string InsertOrUpdateDataTable(DataTable dt, string TableName, string ls_updatecol)
        {
            string strSql = "";
            string ls_return = "";
            string ls_guidvalue = "";
            DataTable dtcur = new DataTable();
            cmd = null;
            if (dt.Rows.Count > 0)
            {
                try
                {
                    Open();
                    BeginTrans();
                    cmd = new Oracle.ManagedDataAccess.Client.OracleCommand();
                    cmd.Connection = this.cn;
                    if (inTransaction)
                    {
                        cmd.Transaction = trans;
                    }
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        ls_guidvalue = dt.Rows[i][ls_updatecol].ToString();
                        dtcur = GetDataTable("select " + ls_updatecol + " from " + TableName + " where " + ls_updatecol + " = '" + ls_guidvalue + "'");
                        if (dtcur.Rows.Count == 0)
                        {
                            strSql = InsertAdapter(dt, TableName, i);
                            cmd.CommandText = strSql;
                            cmd.ExecuteNonQuery();
                        }
                        else
                        {
                            strSql = UpdateAdapter(dt, TableName, i, ls_updatecol);
                            cmd.CommandText = strSql;
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                catch (Exception ex)
                {
                    ls_return = ex.Message.ToString();
                    RollbackTrans();
                    cmd = null;
                    return (ls_return);
                }
                finally
                {
                    cmd = null;
                }
            }
            ls_return = "TRUE";
            return (ls_return);
        }

        public string SqlDataTable(DataTable dt)
        {
            string strSql = "";
            string ls_return = "";
            cmd = null;
            if (dt.Rows.Count > 0)
            {
                try
                {
                    Open();
                    BeginTrans();
                    cmd = new Oracle.ManagedDataAccess.Client.OracleCommand();
                    cmd.Connection = this.cn;
                    if (inTransaction)
                    {
                        cmd.Transaction = trans;
                    }

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        strSql = dt.Rows[i][0].ToString();
                        cmd.CommandText = "begin\r\n" + strSql + "\r\nend;";
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    ls_return = ex.Message.ToString();
                    RollbackTrans();
                    cmd = null;
                    return (ls_return);
                }
                finally
                {
                    cmd = null;
                }
            }
            ls_return = "TRUE";
            return (ls_return);
        }

        public string SqlDataTable(string strSql)
        {
            string ls_return = "";
            cmd = null;
            try
            {
                Open();
                BeginTrans();
                cmd = new Oracle.ManagedDataAccess.Client.OracleCommand();
                cmd.Connection = this.cn;
                if (inTransaction)
                {
                    cmd.Transaction = trans;
                }
                cmd.CommandText = strSql;
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ls_return = ex.Message.ToString();
                RollbackTrans();
                cmd = null;
                return (ls_return);
            }
            finally
            {
                cmd = null;
            }
            return ("TRUE");
        }

        public string DeleteDataTable(DataTable dt)
        {
            return DeleteDataTable(dt, dt.TableName);
        }

        public string DeleteDataTable(DataTable dt, string TableName)
        {
            string strSql = "";
            string ls_return = "";
            cmd = null;
            if (dt.Rows.Count > 0)
            {
                try
                {
                    Open();
                    BeginTrans();
                    cmd = new Oracle.ManagedDataAccess.Client.OracleCommand();
                    cmd.Connection = this.cn;
                    if (inTransaction)
                    {
                        cmd.Transaction = trans;
                    }


                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        strSql = DeleteAdapter(dt, TableName, i);
                        cmd.CommandText = strSql;
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    ls_return = ex.Message.ToString();
                    RollbackTrans();
                    cmd = null;
                    return (ls_return);
                }
                finally
                {
                    cmd = null;
                }
            }
            return (ls_return);
        }

        public string FindDataTable(DataTable dt, string functionid, string dbtype, out string ls_xml)
        {
            string strSql = "";
            string sql = "";
            string ls_return = "";
            DataTable ddt = null;
            ls_xml = "";
            if (dt.Rows.Count > 0)
            {
                try
                {
                    switch (functionid)
                    {
                        case "2005":
                            sql = "select wareguid,warecode,warename ";
                            sql += " from tb_Dms_Dmsware where wareguid in( ";
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                strSql = FindAdapter(dt, i, strSql);
                            }
                            break;
                        case "2017":
                            if (dbtype == "1")
                            {
                                sql = "select orderguid,CheckNo,CheckName,WareSum,NoWareSum,QualSingleSum,NoqualSingleSum,QualPackSum,NoqualPackSum,OperUser,OperDate,Remark ";
                                sql += " from TB_Dms_BUGINSTORECHECK where ORDERGUID in( ";
                                for (int i = 0; i < dt.Rows.Count; i++)
                                {
                                    strSql = FindAdapter(dt, i, strSql);
                                }
                            }
                            else if (dbtype == "2")
                            {
                                sql = "select orderguid,CheckNo,CheckName,WareSum,NoWareSum,QualSingleSum,NoqualSingleSum,QualPackSum,NoqualPackSum,OperUser,OperDate,Remark ";
                                sql += " from Tb_Dms_SaleReturnInstoreCheck where ORDERGUID in( ";
                                for (int i = 0; i < dt.Rows.Count; i++)
                                {
                                    strSql = FindAdapter(dt, i, strSql);
                                }
                            }
                            break;
                        case "2018":
                            if (dbtype == "1")
                            {
                                sql = "select (select orderguid from TB_Dms_BUGINSTORECHECK where checkguid = t.checkguid) as orderguid,CheckGUID,WareGUID,WareName,Units,FactArea,Specs,Batch,ExpDate,ProductDate,QualSingleNum,QualPackNum,NoqualSingleNum,NoqualPackNum,Remark ";
                                sql += " from TB_Dms_BUGINSTORECHECKdetail t where CheckGUID in(select checkguid from TB_Dms_BUGINSTORECHECK where orderguid in(";
                                for (int i = 0; i < dt.Rows.Count; i++)
                                {
                                    strSql = FindAdapter(dt, i, strSql);
                                }
                            }
                            else if (dbtype == "2")
                            {
                                sql = "select (select orderguid from Tb_Dms_SaleReturnInstoreCheck where checkguid = t.checkguid) as orderguid,CheckGUID,WareGUID,WareName,Units,FactArea,Specs,Batch,ExpDate,ProductDate,QualSingleNum,QualPackNum,NoqualSingleNum,NoqualPackNum,Remark ";
                                sql += " from Tb_Dms_SaleReturnInstoreCheckd t where CheckGUID in(select checkguid from Tb_Dms_SaleReturnInstoreCheck where orderguid in(";
                                for (int i = 0; i < dt.Rows.Count; i++)
                                {
                                    strSql = FindAdapter(dt, i, strSql);
                                }
                            }
                            break;
                        case "2019":
                            if (dbtype == "1")
                            {
                                sql = "select INVENTCONFIRMGUID,ORGGUID,STOREAREAGUID,FROMTYPE,OPERUSER,OPERDATE,CHECKUSER,CHECKDATA,REMARK ";
                                sql += " from TB_Dms_INVENTADD where InventConfirmGUID in( select InventConfirmGUID from Tb_Dms_InventConfirm where InventGUID in( select InventGUID from Tb_Dms_Invent where StartInventGUID in(";
                                for (int i = 0; i < dt.Rows.Count; i++)
                                {
                                    strSql = FindAdapter(dt, i, strSql);
                                }
                            }
                            else if (dbtype == "2")
                            {
                                sql = "select INVENTCONFIRMGUID,ORGGUID,STOREAREAGUID,FROMTYPE,OPERUSER,OPERDATE,CHECKUSER,CHECKDATA,REMARK ";
                                sql += " from TB_Dms_INVENTdel where InventConfirmGUID in( select InventConfirmGUID from Tb_Dms_InventConfirm where InventGUID in( select InventGUID from Tb_Dms_Invent where StartInventGUID in(";
                                for (int i = 0; i < dt.Rows.Count; i++)
                                {
                                    strSql = FindAdapter(dt, i, strSql);
                                }
                            }
                            break;
                        case "2020":
                            if (dbtype == "1")
                            {
                                sql = "select SheLveGUID,LocationGUID,WareGUID,batchs,InventaddPackNum,InventaddSingleNum,Remark ";
                                sql += " from Tb_Dms_InventAddDetail t where InventAddGUID in(select InventAddGUID from TB_Dms_INVENTADD where InventConfirmGUID in( select InventConfirmGUID from Tb_Dms_InventConfirm where InventGUID in( select InventGUID from Tb_Dms_Invent where StartInventGUID in(";
                                for (int i = 0; i < dt.Rows.Count; i++)
                                {
                                    strSql = FindAdapter(dt, i, strSql);
                                }
                            }
                            else if (dbtype == "2")
                            {
                                sql = "select SheLveGUID,LocationGUID,WareGUID,batchs,InventdelPackNum,InventdelSingleNum,Remark ";
                                sql += " from Tb_Dms_InventdelDetail t where InventdelGUID in(select InventdelGUID from TB_Dms_INVENTdel where InventConfirmGUID in( select InventConfirmGUID from Tb_Dms_InventConfirm where InventGUID in( select InventGUID from Tb_Dms_Invent where StartInventGUID in(";
                                for (int i = 0; i < dt.Rows.Count; i++)
                                {
                                    strSql = FindAdapter(dt, i, strSql);
                                }
                            }
                            break;
                    }
                    strSql = strSql.Substring(0, strSql.Length - 1) + " ";
                    switch (functionid)
                    {
                        case "2018":
                            sql += strSql + " )) and batch is not null";
                            break;
                        case "2019":
                            sql += strSql + " ))) ";
                            break;
                        case "2020":
                            sql += strSql + " )))) ";
                            break;
                        default:
                            sql += strSql + " ) ";
                            break;
                    }
                    ddt = GetDataTable(sql);
                    //WriteXml(ddt);
                    //ls_xml = doc.InnerXml;
                }
                catch (Exception ex)
                {
                    ls_return = ex.Message.ToString();
                    return (ls_return);
                }
                finally
                {

                }
            }
            return ("TRUE");
        }

        public string FindDataTable(DataTable dt, out string ls_xml)
        {
            string strSql = "";
            string ls_return = "";
            ls_xml = "";
            DataTable ddt = null;
            try
            {
                strSql = dt.Rows[0][0].ToString();
                ddt = GetDataTable(strSql);
                //WriteXml(ddt);
                //ls_xml = doc.InnerXml;
            }
            catch (Exception ex)
            {
                ls_return = ex.Message.ToString();
                return (ls_return);
            }
            ls_return = "TRUE";
            return (ls_return);
        }

        public string UpdateDataTable(DataTable dt, string ls_upcol)
        {
            return UpdateDataTable(dt, dt.TableName, ls_upcol);
        }

        public string UpdateDataTable(DataTable dt, string TableName, string ls_updatecol)
        {
            string strSql = "";
            string ls_return = "";
            cmd = null;
            if (dt.Rows.Count > 0)
            {
                try
                {
                    Open();
                    BeginTrans();
                    cmd = new Oracle.ManagedDataAccess.Client.OracleCommand();
                    cmd.Connection = this.cn;
                    if (inTransaction)
                    {
                        cmd.Transaction = trans;
                    }

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        strSql = UpdateAdapter(dt, TableName, i, ls_updatecol);
                        cmd.CommandText = strSql;
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    ls_return = ex.Message.ToString();
                    RollbackTrans();
                    cmd = null;
                    return (ls_return);
                }
                finally
                {
                    cmd = null;
                }
            }
            ls_return = "TRUE";
            return (ls_return);
        }

        public void Open()
        {
            //if(cn == null)
            //{
            //    cn = new Oracle.ManagedDataAccess.Client.OracleConnection();
            //}
            if (cn.State.ToString().ToUpper() != "OPEN")
                this.cn.Open();
        }

        public void Close()
        {
            if (cn.State.ToString().ToUpper() == "OPEN")
            {
                this.cn.Close();
            }
        }

        public void DisPose()
        {
            if (cn.State.ToString().ToUpper() == "OPEN")
            {
                this.cn.Close();
            }

            this.cn.Dispose();
            this.cn = null;
        }

        public void BeginTrans()
        {
            if (trans == null)
            {
                trans = null;
                trans = cn.BeginTransaction();
                inTransaction = true;
            }
        }

        public void CommitTrans()
        {
            if (trans != null)
            {
                try
                {
                    trans.Commit();
                    inTransaction = false;
                    Close();
                }
                catch { }
            }
            else
            {
                if (cn != null)
                {
                    Close();
                }
            }
        }

        public void RollbackTrans()
        {
            if (trans != null)
            {
                try
                {
                    trans.Rollback();
                    inTransaction = false;
                    Close();
                }
                catch { }
            }
            else
            {
                if (cn != null)
                {
                    Close();
                }
            }
        }
        #endregion        
    }
}