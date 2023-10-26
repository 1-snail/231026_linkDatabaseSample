using System;
using System.Data;
using System.Windows.Forms;
using DAL;

namespace BLL
{
    class FromOperator
    {
        SqlOperator sqlOperator = new SqlOperator();
        /// <summary>
        /// 获得 DataGridview 鼠标所选行的索引
        /// </summary>
        /// <param name="dv"> 要搜索的 DataGridview </param>
        /// <returns>要搜索到了则返回行数，否则返回 0 </returns>
        public int GetSelectRowIndex(DataGridView dv)
        {
            foreach (DataGridViewRow row in dv.Rows)
            {
                if (row.Selected)
                {
                    return row.Index;
                }
            }
            return 0;
        }


        /// <summary>
        /// 检查 GroupBox 中的 TextBox 是否为空，
        /// </summary>
        /// <param name="groupBox"> 要判断的 groupBox 组件 </param>
        /// <returns>如果为空返回 true</returns>
        public bool CheckControlIsNUll(System.Windows.Forms.GroupBox groupBox, bool allCheck)
        {
            int count = 0;

            foreach (Control control in groupBox.Controls)
            {
                if (control is System.Windows.Forms.TextBox)
                {
                    if (string.IsNullOrEmpty(control.Text.ToString()))
                    {
                        count++;
                    }
                }
            }
            if (allCheck && count == 4)
            {
                return true;
            }
            if (allCheck && count != 4)
            {
                return false;
            }
            if (!allCheck && count != 4)
            {
                return false;
            }


            return true;
        }

        /// <summary>
        /// 输入SQL语句，检查数据库中该变量值是否重复，
        /// 例如检查 id = 1 是否重复  strComm=" select count(*) from [demo].[dbo].[studentInfo]  where id = 1";
        /// </summary>
        /// <param name="strComm">要查询的SQL语句</param>
        /// <returns>如果重复，则返回true</returns>
        public bool CheckValueisPrepeat(string strComm)
        {

            try
            {
                int result = sqlOperator.SqlExectuScalar(strComm);
                if (result == -1)
                {
                    MessageBox.Show("查询错误");
                }
                else if (result == 0)
                {
                    return true;
                }
                return false;
            }
            catch
            {
                throw;
            }

        }

        /// <summary>
        /// 刷新 DataGridView 显示内容
        /// </summary>
        /// <param name="dv">要刷新的 DataGridView </param>
        public void RefreshDataGridView(DataGridView dv)
        {
            string strComm = "select * from [demo].[dbo].[studentInfo]";
            DataTable dt = new DataTable();
            if (sqlOperator.RetrieveSqlOperator(strComm, ref dt))
            {
                dv.DataSource = dt;
            }
        }

        /// <summary>
        /// 业务逻辑层对数据库进行 增加、更新、删除 操作
        /// </summary>
        /// <param name="strComm">要查询的SQL语句</param>
        /// <returns>执行成功返回 true</returns>
        public bool CUDOSqlperator(string strComm)
        {
            try
            {
                return sqlOperator.CUDOSqlperator(strComm);

            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>  
        /// 业务逻辑层调用 DAL ，使用SQslCommand进行查询(
        /// </summary>
        /// <param name="strComm">要查询是SQL语句</param>
        /// <returns>返回获得的结果，如果抛出错误，则表示查询错误</returns>
        public int SqlExectuScalar(string strComm)
        {
            try
            {
                return sqlOperator.SqlExectuScalar(strComm);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 业务逻辑层查询数据库
        /// </summary>
        /// <param name="strComm">要查询的sql语句，</param>
        /// <param name="dt">返回的DataTable</param>
        /// <returns>查询成功返回 true </returns>
        public bool RetrieveSqlOperator(string strComm, ref DataTable dt)
        {
            return sqlOperator.RetrieveSqlOperator(strComm, ref dt);
        }


        /// <summary>
        /// 批量导入数据到数据库中
        /// </summary>
        /// <param name="tableName">到导入的表名</param>
        /// <param name="dt">要导入的数据，用DataTable保存</param>
        /// <returns>导入数据库成功返回true</returns>
        public bool BatchImport(string tableName, ref DataTable dt)
        {
            return sqlOperator.BatchImport(tableName,ref dt);
        }


    }



}
