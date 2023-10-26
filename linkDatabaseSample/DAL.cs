using System;
using Data = System.Data;
using System.Data.SqlClient;
using System.Data;

namespace DAL
{

    /// <summary>
    /// 对 SQL 相关操作
    /// </summary>
    class SqlOperator
    {
        
        readonly static string strConn = "Server=localhost; Database=demo; uid = sa; pwd = Jqs123456; Trusted_Connection = False";
        static SqlConnection sqlConn = new SqlConnection(strConn);
        private SqlConnection GetConnection()
        {
            return new SqlConnection("Server=localhost; Database=demo; uid = sa; pwd = Jqs123456; Trusted_Connection = False");
        }
        /// <summary>
        /// 对数据库进行 增加、更新、删除 操作，
        /// </summary>
        /// <param name="strComm">要执行的 sql 语句</param>
        /// <returns> 执行成功返回 true </returns>
        public bool CUDOSqlperator(string strComm)
        {
            try
            {
                if (sqlConn.State == ConnectionState.Closed)
                {
                    sqlConn.Open();
                }

                SqlCommand sqlComm = new SqlCommand(strComm, sqlConn);
                int result = sqlComm.ExecuteNonQuery();
                if (result == 0)
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                sqlConn.Close();
            }
        }



        /// <summary>  
        /// 使用SQslCommand进行查询(
        /// </summary>
        /// <param name="strComm">要查询是SQL语句</param>
        /// <returns>返回获得的结果，如果抛出错误，则表示查询错误</returns>
        public int SqlExectuScalar(string strComm)
        {
            try
            {
                if (sqlConn.State == ConnectionState.Closed)
                {
                    sqlConn.Open();
                }
                SqlCommand sqlComm = new SqlCommand(strComm, sqlConn);
                return Convert.ToInt32(sqlComm.ExecuteScalar());

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                sqlConn.Close();  // 最后关闭数据库连接
            }


        }

        /// <summary>
        /// 查询数据库
        /// </summary>
        /// <param name="strComm">要查询的sql语句，</param>
        /// <param name="dt">返回的DataTable</param>
        /// <returns>查询成功返回 true </returns>
        public bool RetrieveSqlOperator(string strComm, ref DataTable dt)
        {
            try
            {
                if (sqlConn.State == ConnectionState.Closed)
                {
                    sqlConn.Open();
                }
                SqlCommand sqlComm = new SqlCommand(strComm, sqlConn);
                SqlDataAdapter sda = new SqlDataAdapter(sqlComm);
                sda.Fill(dt);
                return true;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                sqlConn.Close();
            }
        }




        /// <summary>
        /// 批量导入数据到数据库中
        /// </summary>
        /// <param name="tableName">到导入的表名</param>
        /// <param name="dt">要导入的数据，用DataTable保存</param>
        /// <returns>导入数据库成功返回true</returns>
        public bool BatchImport(string tableName, ref System.Data.DataTable dt)
        {
            string strConn = "Server=localhost; Database=demo; uid = sa; pwd = Jqs123456; Trusted_Connection = False";
            SqlConnection sqlConn = new SqlConnection(strConn);
            try
            {
                if (sqlConn.State == Data.ConnectionState.Closed)
                {
                    sqlConn.Open();
                }
                using (var bulk = new SqlBulkCopy(sqlConn))
                {
                    bulk.DestinationTableName = tableName;
                    bulk.WriteToServer(dt);
                }
                return true;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                sqlConn.Close();
            }
        }

    }


}
