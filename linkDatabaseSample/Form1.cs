using BLL;
using System;

using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms;
using SQLTest.ReadFile;


namespace linkDatabaseSample
{
    public partial class Form1 : Form
    {
        FromOperator fromoperator = new FromOperator();
        public Form1()
        {
            InitializeComponent();


        }

        /// <summary>
        /// 新增内容到数据库中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void insetBtn_Click(object sender, EventArgs e)
        {
            string strCommIdIsRepeat = @"select count(*) from [demo].[dbo].[studentInfo] 
                                            where id = '" + this.idTextBox.Text.ToString().Trim() + "'";
            if (!string.IsNullOrEmpty(this.idTextBox.Text.ToString().Trim()) && !fromoperator.CheckValueisPrepeat(strCommIdIsRepeat))
            {
                MessageBox.Show("ID 值重复，请重新输入");
                return;
            }
            if (!fromoperator.CheckControlIsNUll(this.groupBox1, true))
            {
                string strComm = @"insert into  [demo].[dbo].[studentInfo] (id, name,age,score) 
                    values('" + this.idTextBox.Text.ToString() + "','" + this.nameTextBox.Text.ToString() + "','" + this.ageTextBox.Text.ToString() + "','" + this.scoreTextBox.Text.ToString() + "')";
                if (fromoperator.CUDOSqlperator(strComm))
                {
                    fromoperator.RefreshDataGridView(this.dataGridView);
                }
            }
            else
            {
                MessageBox.Show("请输入完整数据");
            }


        }

        /// <summary>
        /// 删除表格中所选择的数据行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteBtn_Click(object sender, EventArgs e)
        {
            int index = fromoperator.GetSelectRowIndex(this.dataGridView);
            string strComm = "delete from [demo].[dbo].[studentInfo] where  ";
            string[] context = { "id", "name", "age", "score" };
            for (int i = 0; i < this.dataGridView.ColumnCount; i++)
            {
                if (context[i].Equals("name"))
                {
                    strComm += string.Format("{0} ='{1}'", context[i], this.dataGridView.Rows[index].Cells[i].Value.ToString().Trim());
                }
                else
                {
                    strComm += string.Format("{0} = {1}", context[i], this.dataGridView.Rows[index].Cells[i].Value.ToString().Trim());
                }

                if (i != this.dataGridView.ColumnCount - 1)
                {
                    strComm += " and ";
                }
            }
            if (fromoperator.CUDOSqlperator(strComm))
            {
                fromoperator.RefreshDataGridView(this.dataGridView);
            }

            Console.WriteLine("pause");

        }

        /// <summary>
        /// 使用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void testBtn_Click(object sender, EventArgs e)
        {
            int index = fromoperator.GetSelectRowIndex(this.dataGridView);
            string strComm = "delete from [demo].[dbo].[studentInfo]";
            if (fromoperator.CUDOSqlperator(strComm))
            {
                fromoperator.RefreshDataGridView(this.dataGridView);
            }
        }

        private void findBtn_Click(object sender, EventArgs e)
        {
            string[] context = new string[] { "id", "name", "age", "score" };
            string strComm = "select * from [demo].[dbo].[studentInfo] where ";
            if (!fromoperator.CheckControlIsNUll(this.groupBox1, false))
            {
                foreach (Control control in this.groupBox1.Controls)
                {
                    if (control is System.Windows.Forms.TextBox)
                    {
                        if (!string.IsNullOrEmpty(control.Text.ToString()))
                        {
                            string oneOfContext = control.Name.Substring(0, control.Name.Length - 7);

                            bool state = Array.Exists(context, element => element == oneOfContext);
                            if (state)
                            {
                                if (oneOfContext.Equals("name"))
                                {
                                    strComm += string.Format("{0} = '{1}' and", oneOfContext, control.Text.ToString().Trim());
                                }
                                else
                                {
                                    strComm += string.Format("{0} = {1} and", oneOfContext, control.Text.ToString().Trim());
                                }
                            }
                        }
                    }
                }
                strComm = strComm.Substring(0, strComm.Length - 3);
            }
            else
            {
                strComm = "select * from [demo].[dbo].[studentInfo]";
            }


            DataTable dt = new DataTable();
            fromoperator.RetrieveSqlOperator(strComm, ref dt);
            dataGridView.DataSource = dt;

        }

        private void importDataBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();  //显示选择文件对话框
            openFileDialog1.InitialDirectory = @"C:\Users\jinlong.dai\Desktop\data";
            openFileDialog1.Filter = "All files (.csv;.xlsx)|*.csv;*.xlsx|csv files (*.csv)|*.csv|Excel 工作簿 (*.xlsx)|*.xlsx"; //所有的文件格式
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = false;
            string fileNmae = string.Empty;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                fileNmae = openFileDialog1.FileName;   //显示文件路径
            }

            // 按照给定的路径读取文件
            if (!ReadFileAndImportDatabase.Execute(fileNmae))
            {
                return;
            }

            DataTable dt = new DataTable();
            string strComm = "select * from [demo].[dbo].[studentInfo]";
            fromoperator.RetrieveSqlOperator(strComm, ref dt);
            dataGridView.DataSource = dt;
        }


        /// <summary>
        /// 批量导入数据到数据库中
        /// </summary>
        /// <param name="tableName">到导入的表名</param>
        /// <param name="dt">要导入的数据，用DataTable保存</param>
        /// <returns>导入数据库成功返回true</returns>
        public bool BatchImport(string tableName, ref DataTable dt)
        {
            string strConn = "Server=localhost; Database=demo; uid = sa; pwd = Jqs123456; Trusted_Connection = False";
            SqlConnection sqlConn = new SqlConnection(strConn);
            try
            {
                if (sqlConn.State == ConnectionState.Closed)
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

        /// <summary>
        /// 使用 Entity Framework 框架读对数据库进行操作
        /// </summary>
        /// <returns>读取成功返回true</returns>
        private bool EFreadSql()
        {
            try
            {
                using (demoEntities de = new demoEntities())
                {
                    de.studentInfo.Add(new studentInfo()
                    {
                        id = Convert.ToInt32(this.idTextBox.Text),
                        name = this.nameTextBox.Text.ToString(),
                        age = Convert.ToInt32(this.ageTextBox.Text),
                        score = Convert.ToInt32(this.scoreTextBox.Text)

                    });
                    de.SaveChanges();
                }
                return true;

            }
            catch
            {
                return false;
            }
        }
    }


}
