using System;
using System.Collections.Generic;
using Data = System.Data;
using System.IO;
using System.Linq;
using System.Text;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;
using System.Data;
using System.Data.SqlClient;

namespace SQLTest.ReadFile
{
    interface IFileOperation
    {
        DataTable Read(string readPath);
        bool Write(DataTable dt,string savePath);
    }

    class CsvFileOperation : IFileOperation
    {
        /// <summary>
        /// 读取CSV文件
        /// </summary>
        /// <param name="strPath">CSV文件地址</param>
        /// <returns>读取成功返回true</returns>
        public DataTable Read(string strPath)
        {
            DataTable dt = new DataTable();

            using (StreamReader sreader = new StreamReader(strPath, Encoding.GetEncoding("UTF-8")))
            {
                string strTemp = string.Empty;
                bool title = true;
                int dataStart = 0;
                while ((strTemp = sreader.ReadLine()) != null)
                {
                    // 不移除csv中的空数据
                    // 表头是 31 个
                    // 表内容是 249 个
                    // 第一行是表头，之后才是数据
                    if (title)
                    {

                        List<string> context = strTemp.Split(new char[] { ',' }, StringSplitOptions.None).ToList();
                        dataStart = context.IndexOf("Sample Data");
                        for (int i = 0; i < context.Capacity; i++)
                        {
                            if (i < dataStart)
                            {
                                dt.Columns.Add(context[i], typeof(string));
                            }
                            else
                            {
                                dt.Columns.Add($"Sample Data_{i}");
                            }
                        }
  
                        title = false;
                    }
                    else  
                    {
                        List<string> context = strTemp.Split(new char[] { ',' }, StringSplitOptions.None).ToList();
                        // 样式是按照 dt 中的 rows 生成的
                        DataRow dr = dt.NewRow();
                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            //dr[i] = i < dt.Columns.Count ? context[i] : "";
                            //dr[i] = i < context.Capacity ? context[i] : 0;
                            if (i < context.Capacity)
                            {
                                dr[i] = context[i];
                            }
                            else
                            {
                                dr[i] = 0.0;
                            }


                        }
                        // 将生成的 rows 增加到 dt 中
                        dt.Rows.Add(dr);
                    }

                }
            }

            return dt;
        }

        public DataTable Read_Backup(string strPath)
        {
            DataTable dt = new DataTable();

            using (StreamReader sreader = new StreamReader(strPath, Encoding.GetEncoding("UTF-8")))
            {
                string strTemp = string.Empty;
                bool title = true;
                while ((strTemp = sreader.ReadLine()) != null)
                {
                    List<string> context = strTemp.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    if (context.Capacity == 23)
                    {
                        Console.WriteLine();
                    }
                    // 第一行是表头，之后才是数据
                    if (title)
                    {

                        foreach (var column in context)
                        {
                            dt.Columns.Add(column, typeof(string));
                        }

                        List<string> titleList = new List<string>();

                        for (int i = 0; i < 210; i++)
                        {
                            dt.Columns.Add($"SampleData_{i}", typeof(int));
                        }

                        title = false;
                    }
                    else
                    {
                        // 样式是按照 dt 中的 rows 生成的
                        DataRow dr = dt.NewRow();
                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            //dr[i] = i < dt.Columns.Count ? context[i] : "";
                            //dr[i] = i < context.Capacity ? context[i] : 0;
                            if (i < context.Capacity)
                            {
                                dr[i] = context[i];
                            }
                            else
                            {
                                dr[i] = 0;
                            }


                        }
                        // 将生成的 rows 增加到 dt 中
                        dt.Rows.Add(dr);
                    }

                }
            }

            return dt;
        }

        /// <summary>
        ///     按照特定规则读取项目数据
        /// </summary>
        /// <param name="strPath"></param>
        /// <returns></returns>
        public void ReadFileWithSpecialRule(string strPath,ref DataTable dt1,ref DataTable dt2)
        {
            long counter = 0;
            using (StreamReader sreader = new StreamReader(strPath, Encoding.GetEncoding("UTF-8")))
            {
                string strTemp = string.Empty;
                bool title = true;
                int dataLocation = 0;
                int titleLength = 0;
                int dataStart = 0;
                while ((strTemp = sreader.ReadLine()) != null)
                {
                    //string[] context = strTemp.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    // 第一行是表头，之后才是数据
                    List<string> context = strTemp.Split(new char[] { ',' }, StringSplitOptions.None).ToList();
                    if (title)
                    {
                        titleLength = context.Capacity;
                        dataLocation = context.IndexOf("Record Type");
                        dataStart = context.IndexOf("Sample Data");
                        List<string> titleList = new List<string>();

                        for (int i = 0; i < context.Capacity - dataStart; i++)
                        {
                            dt1.Columns.Add($"Sample Data_{i}",typeof(double));
                            dt2.Columns.Add($"Sample Data_{i}",typeof(double));
                        }

                        title = false;
                    }
                    else
                    {
                        counter++;

                        List<string> tempList = strTemp.Split(new char[] { ',' }, StringSplitOptions.None).ToList();
                        string label = tempList[dataLocation];
                        dt1.TableName = "DFRM";
                        dt2.TableName = "FREQ";
                        if (label.Equals("DFRM") && counter % 2 == 1)
                        {
                            List<string> resultList = new List<string>();
                            resultList = resultList.Concat(tempList.GetRange(dataStart, tempList.Capacity - dataStart)).ToList<string>();
                            DataRow dr = dt1.NewRow();
                            for (int i = 0; i < dt1.Columns.Count; i++)
                            {
                                dr[i] = i < dt1.Columns.Count ? Convert.ToDouble( resultList[i] ): 0.0;
                            }
                            dt1.Rows.Add(dr);
                        }

                        else if (label.Equals("FREQ") && counter % 2 == 1)
                        {
                            List<string> resultList = new List<string>();
                            resultList = resultList.Concat(tempList.GetRange(dataStart, tempList.Capacity - dataStart)).ToList<string>();
                            DataRow dr = dt2.NewRow();
                            for (int i = 0; i < dt2.Columns.Count; i++)
                            {
                                dr[i] = i < dt2.Columns.Count ? Convert.ToDouble(resultList[i]) : 0.0;
                            }
                            dt2.Rows.Add(dr);
                        }
                    }
                }
            }
        }

        public void ReadFileWithSpecialRule_Backup(string strPath, ref DataTable dt1, ref DataTable dt2)
        {
            long counter = 0;
            using (StreamReader sreader = new StreamReader(strPath, Encoding.GetEncoding("UTF-8")))
            {
                string strTemp = string.Empty;
                bool title = true;
                int dataLocation = 0;
                int titleLength = 0;
                int dataStart = 0;
                while ((strTemp = sreader.ReadLine()) != null)
                {
                    //string[] context = strTemp.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    // 第一行是表头，之后才是数据
                    List<string> context = strTemp.Split(new char[] { ',' }, StringSplitOptions.None).ToList();
                    if (title)
                    {
                        titleLength = context.Capacity;
                        dataLocation = context.IndexOf("Record Type");
                        dataStart = context.IndexOf("Sample Data");
                        List<string> titleList = new List<string>();

                        for (int i = 0; i < context.Capacity - dataStart; i++)
                        {
                            dt1.Columns.Add($"Sample Data_{i}");
                            dt2.Columns.Add($"Sample Data_{i}");
                        }

                        title = false;
                    }
                    else
                    {
                        counter++;

                        List<string> tempList = strTemp.Split(new char[] { ',' }, StringSplitOptions.None).ToList();
                        string label = tempList[dataLocation];
                        dt1.TableName = "DFRM";
                        if (label.Equals("DFRM") && counter % 2 == 1)
                        {
                            List<string> resultList = new List<string>();
                            resultList = resultList.Concat(tempList.GetRange(dataStart, tempList.Capacity - dataStart)).ToList<string>();
                            // 样式是按照 dt 中的 rows 生成的
                            DataRow dr = dt1.NewRow();
                            for (int i = 0; i < dt1.Columns.Count; i++)
                            {
                                dr[i] = i < dt1.Columns.Count ? resultList[i] : "";
                            }
                            // 将生成的 rows 增加到 dt 中
                            dt1.Rows.Add(dr);
                        }
                        else if (label.Equals("FREQ") && counter % 2 == 1)
                        {
                            dt2.TableName = "FREQ";
                            List<string> resultList = new List<string> { label };
                            resultList = resultList.Concat(tempList.GetRange(dataStart, tempList.Capacity - dataStart)).ToList<string>();
                            // 样式是按照 dt 中的 rows 生成的
                            DataRow dr = dt2.NewRow();
                            for (int i = 0; i < dt2.Columns.Count; i++)
                            {
                                dr[i] = i < dt2.Columns.Count ? resultList[i] : "";
                            }
                            // 将生成的 rows 增加到 dt 中
                            dt2.Rows.Add(dr);
                        }
                    }
                }
            }
        }
        /// <summary>
        ///     将 DataTable 数据导出为 CSV 文件
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="savePath"></param>
        /// <returns></returns>
        public bool Write(DataTable dt,string savePath)
        {
            if (!Path.GetExtension(savePath).Equals(".csv") 
                || !Directory.Exists(Path.GetDirectoryName(savePath)))
            {
                return false;
            }
            
            FileStream fs = null;
            StreamWriter sw = null;
            try
            {
                fs = new FileStream(savePath, FileMode.OpenOrCreate, FileAccess.Write);
                sw = new StreamWriter(fs, Encoding.GetEncoding("UTF-8"));
                // 先将表头写入其中
                string strHead = string.Empty;
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    strHead += dt.Columns[i].ColumnName + ",";
                }
                strHead = strHead.Substring(0, strHead.Length - 1);
                sw.WriteLine(strHead);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string strTemp = string.Empty;
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        strTemp += dt.Rows[i][j].ToString() + ",";
                    }
                    strTemp = strTemp.Substring(0, strTemp.Length - 1);
                    sw.WriteLine(strTemp);
                }

                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (sw != null)
                {
                    sw.Close();
                }

                if (fs != null)
                {
                    fs.Close();
                }
               
            }
        }



    }

    class ExcleFileOperation : IFileOperation
    {
        /// <summary>
        /// 导入Excel，使用Datable保存
        /// </summary>
        /// <param name="file">导入路径(包含文件名与扩展名)</param>
        /// <returns></returns>
        public DataTable Read(string file)
        {
            DataTable dt = new DataTable();
            IWorkbook workbook;
            string fileExt = Path.GetExtension(file).ToLower();
            using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                //XSSFWorkbook 适用XLSX格式，HSSFWorkbook 适用XLS格式
                if (fileExt == ".xlsx") { workbook = new XSSFWorkbook(fs); } else if (fileExt == ".xls") { workbook = new HSSFWorkbook(fs); } else { workbook = null; }
                if (workbook == null) { return null; }
                ISheet sheet = workbook.GetSheetAt(0);

                //表头  
                IRow header = sheet.GetRow(sheet.FirstRowNum);
                List<int> columns = new List<int>();
                for (int i = 0; i < header.LastCellNum; i++)
                {
                    object obj = GetValueType(header.GetCell(i));
                    if (obj == null || obj.ToString() == string.Empty)
                    {
                        dt.Columns.Add(new DataColumn("Columns" + i.ToString()));
                    }
                    else
                        dt.Columns.Add(new DataColumn(obj.ToString()));
                    columns.Add(i);
                }
                //数据  
                for (int i = sheet.FirstRowNum + 1; i <= sheet.LastRowNum; i++)
                {
                    DataRow dr = dt.NewRow();
                    bool hasValue = false;
                    foreach (int j in columns)
                    {
                        dr[j] = GetValueType(sheet.GetRow(i).GetCell(j));
                        if (dr[j] != null && dr[j].ToString() != string.Empty)
                        {
                            hasValue = true;
                        }
                    }
                    if (hasValue)
                    {
                        dt.Rows.Add(dr);
                    }
                }
            }
            return dt;
        }

        /// <summary>
        /// 获取单元格类型
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        private object GetValueType(ICell cell)
        {
            if (cell == null)
                return null;
            switch (cell.CellType)
            {
                case CellType.Blank: //BLANK:  
                    return null;
                case CellType.Boolean: //BOOLEAN:  
                    return cell.BooleanCellValue;
                case CellType.Numeric: //NUMERIC:  
                    return cell.NumericCellValue;
                case CellType.String: //STRING:  
                    return cell.StringCellValue;
                case CellType.Error: //ERROR:  
                    return cell.ErrorCellValue;
                case CellType.Formula: //FORMULA:  
                default:
                    return "=" + cell.CellFormula;
            }
        }

        /// <summary>
        ///     Datable导出成Excel
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="savePath">导出路径(包括文件名与扩展名)</param>
        public bool WriteData(DataTable dt, string savePath,string sheetName)
        {
            try
            {
                IWorkbook workbook;
                string fileExt = Path.GetExtension(savePath).ToLower();
                if (fileExt == ".xlsx")
                {
                    workbook = new XSSFWorkbook();
                }
                else if (fileExt == ".xls")
                {
                    workbook = new HSSFWorkbook();
                }
                else
                {
                    workbook = null;
                }

                if (workbook == null)
                {
                    return false;
                }

                ISheet sheet = workbook.CreateSheet(sheetName);

                //表头  
                IRow row = sheet.CreateRow(0);
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    ICell cell = row.CreateCell(i);
                    cell.SetCellValue(dt.Columns[i].ColumnName);
                }

                //数据  
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    IRow row1 = sheet.CreateRow(i + 1);
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        ICell cell = row1.CreateCell(j);
                        if (j == 0)
                        {

                            cell.SetCellValue(dt.Rows[i][j].ToString());
                        }
                        else
                        {
                            cell.SetCellValue(dt.Rows[i][j].ToString());
                        }
                    }
                }

                //转为字节数组  
                MemoryStream stream = new MemoryStream();
                workbook.Write(stream);
                var buf = stream.ToArray();

                //保存为Excel文件  
                using (FileStream fs = new FileStream(savePath, FileMode.Open, FileAccess.Write))
                {
                    fs.Write(buf, 0, buf.Length);
                    fs.Flush();
                }

                return true;
            }
            catch (Exception e)
            {
                throw e;
            }

        }


        /// <summary>
        ///     将DataSet中多个Table保存为Excel中的多个sheet
        /// </summary>
        /// <param name="data"></param>
        /// <param name="savePath">导出路径(包括文件名与扩展名)</param>
        public bool WriteDataSet(DataSet data, string savePath)
        {
            try
            {
                IWorkbook workbook;
                string fileExt = Path.GetExtension(savePath).ToLower();
                if (fileExt == ".xlsx")
                {
                    workbook = new XSSFWorkbook();
                }
                else if (fileExt == ".xls")
                {
                    workbook = new HSSFWorkbook();
                }
                else
                {
                    workbook = null;
                }

                if (workbook == null)
                {
                    return false;
                }

                for (int idx = 0;  idx< data.Tables.Count; ++idx)
                {
                    DataTable dt = data.Tables[idx];
                    ISheet sheet = workbook.CreateSheet(data.Tables[idx].TableName);

                    //表头  
                    IRow row = sheet.CreateRow(0);
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        ICell cell = row.CreateCell(j);
                        cell.SetCellValue(dt.Columns[j].ColumnName);
                    }

                    //数据  
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        IRow row1 = sheet.CreateRow(i + 1);
                        for (int j = 0; j < dt.Columns.Count; j++)
                        {
                            ICell cell = row1.CreateCell(j);
                            if(idx != 0)
                            {
                                double value = 0.0;
                                double.TryParse(dt.Rows[i][j].ToString(), out value);
                                cell.SetCellValue(value);
                            }
                            else
                            {
                                if (j < 30)
                                {
                                    cell.SetCellValue(dt.Rows[i][j].ToString());
                                }
                                else
                                {
                                    // 设置数据格式为 double 类型
                                    double value = 0.0;
                                    double.TryParse(dt.Rows[i][j].ToString(), out value);
                                    cell.SetCellValue(value);
                                }

                            }
                        }
                    }
                }
               

                //转为字节数组  
                MemoryStream stream = new MemoryStream();
                workbook.Write(stream);
                var buf = stream.ToArray();

                //保存为Excel文件  
                using (FileStream fs = new FileStream(savePath, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(buf, 0, buf.Length);
                    fs.Flush();
                }

                return true;
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public bool Write(DataTable dt, string savePath)
        {
            throw new NotImplementedException();
        }
    }

    class ReadFileAndImportDatabase
    {

        public static bool Execute(string fileName)
        {
            try
            {
                IFileOperation readFile = null;
                if (Path.GetExtension(fileName).Equals(".csv"))
                {
                    readFile = new CsvFileOperation();
                }
                else if (Path.GetExtension(fileName).Equals(".xlsx"))
                {
                    readFile = new ExcleFileOperation();
                }

                DataTable dt = readFile.Read(fileName);
                if (readFile != null)
                {
                    string tableName = "[demo].[dbo].[studentInfo]";
                    BatchImport(tableName, ref dt);
                }
                return true;

            }
            catch(Exception e)
            {
                throw e;
            }
            
        }



        /// <summary>
        /// 
        ///        批量导入数据到数据库中
        /// 
        /// </summary>
        /// <param name="tableName">到导入的表名</param>
        /// <param name="dt">要导入的数据，用DataTable保存</param>
        /// <returns>导入数据库成功返回true</returns>
        private static bool BatchImport(string tableName, ref DataTable dt)
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
    }

    class test
    {
        public void execute()
        {
            string fileName = @"C:\Users\jinlong.dai\Desktop\B003_2nd_Oddindex1_95-92%.csv";
            //string savePath = @"C:\Users\jinlong.dai\Desktop\B003_23-10-26_11-08-38.xlsx";
            string savePath = string.Format("C://Users//jinlong.dai//Desktop//B003_{0}.xlsx", DateTime.Now.ToString("yy-MM-dd_HH-mm-ss"));
            CsvFileOperation csvOperation = new CsvFileOperation();

            DataTable dt1 = new DataTable();
            DataTable dt2 = new DataTable();
            DataSet data = new DataSet();

            DataTable dtOld = csvOperation.Read(fileName);
            dtOld.TableName = "原数据";
            data.Tables.Add(dtOld);

            csvOperation.ReadFileWithSpecialRule(fileName, ref dt1, ref dt2);
            data.Tables.Add(dt1);
            data.Tables.Add(dt2);

            ExcleFileOperation excle = new ExcleFileOperation();
            bool state1 = excle.WriteDataSet(data, savePath);
            if (!state1)
            {
                Console.WriteLine();
            }
            Console.WriteLine("emem");
        } 
    }
}
