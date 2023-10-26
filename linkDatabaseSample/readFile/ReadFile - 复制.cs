//using System;
//using System.Collections.Generic;
//using Data = System.Data;
//using System.IO;
//using System.Linq;
//using System.Text;
//using NPOI.SS.UserModel;
//using NPOI.XSSF.UserModel;
//using NPOI.HSSF.UserModel;
//using System.Data;
//using System.Data.SqlClient;

//namespace SQLTest.ReadFile
//{
//    interface IReadFile
//    {
//        DataTable Read(string strPath);
//    }

//    class ReadCsv : IReadFile
//    {
//        /// <summary>
//        /// 读取CSV文件
//        /// </summary>
//        /// <param name="strPath">CSV文件地址</param>
//        /// <returns>读取成功返回true</returns>
//        public DataTable Read(string strPath)
//        {
//            DataTable dt = new DataTable();

//            using (StreamReader sreader = new StreamReader(strPath, Encoding.GetEncoding("UTF-8")))
//            {
//                string strTemp = string.Empty;
//                bool title = true;
//                while ((strTemp = sreader.ReadLine()) != null)
//                {
//                    string[] context = strTemp.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
//                    // 第一行是表头，之后才是数据
//                    if (title)
//                    {
//                        foreach (var colum in context)
//                        {
//                            dt.Columns.Add(colum);
//                        }
//                        title = false;
//                    }
//                    else  
//                    {
//                        // 样式是按照 dt 中的 rows 生成的
//                        DataRow dr = dt.NewRow();
//                        for (int i = 0; i < dt.Columns.Count; i++)
//                        {

//                            dr[i] = i < dt.Columns.Count ? context[i] : "";

//                        }
//                        // 将生成的 rows 增加到 dt 中
//                        dt.Rows.Add(dr);
//                    }

//                }
//            }

//            return dt;
//        }

//        public DataTable ReadNew(string strPath)
//        {
//            DataTable dt = new DataTable();
//            long counter = 0;
//            using (StreamReader sreader = new StreamReader(strPath, Encoding.GetEncoding("UTF-8")))
//            {
//                string strTemp = string.Empty;
//                bool title = true;
//                while ((strTemp = sreader.ReadLine()) != null)
//                {
//                    //string[] context = strTemp.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
//                    // 第一行是表头，之后才是数据
//                    List<string> context = strTemp.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                   
//                    if (title)
//                    {

//                        List<string> titleList = new List<string>
//                        {
//                            context[2]
//                        };
//                        for (int i = 0; i < 210; i++)
//                        {
//                            titleList.Add($"SampleData_{i}");
//                        }

//                        foreach (var colum in titleList)
//                        {
//                            dt.Columns.Add(colum);
//                        }
//                        title = false;
//                    }
//                    else
//                    {
//                        counter++;

//                        List<string> tempList = strTemp.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
//                        string label = tempList[2];
                        

//                        if ((label.Equals("DFRM") || label.Equals("FREQ")) && counter %2 == 1)
//                        {
//                            List<string> resultList = new List<string> { label };
//                            int dataLocation = context.IndexOf("Record Type");
//                            resultList = resultList.Concat(tempList.GetRange(22, tempList.Capacity - 22)).ToList<string>();
//                            // 样式是按照 dt 中的 rows 生成的
//                            DataRow dr = dt.NewRow();
                            
//                            for (int i = 0; i < dt.Columns.Count; i++)
//                            {
//                                try
//                                {
//                                    dr[i] = i < dt.Columns.Count ? resultList[i] : "";

//                                }
//                                catch (ArgumentOutOfRangeException)
//                                {

//                                    throw;
//                                }
//                            }
//                            // 将生成的 rows 增加到 dt 中
//                            dt.Rows.Add(dr);
//                        }
//                    }
//                }
//            }

//            return dt;
//        }

//        public bool DataToExcel(DataTable dt,string savePath)
//        {
//            try
//            {

//                TableToExcel(dt, savePath);
//                return true; 
//            }
//            catch (Exception)
//            {
//                return false;
//                throw;
//            }
//        }

//        /// <summary>
//        /// Datable导出成Excel
//        /// </summary>
//        /// <param name="dt"></param>
//        /// <param name="file">导出路径(包括文件名与扩展名)</param>
//        private void TableToExcel(DataTable dt, string file)
//        {
//            IWorkbook workbook;
//            string fileExt = Path.GetExtension(file).ToLower();
//            if (fileExt == ".xlsx") { workbook = new XSSFWorkbook(); } else if (fileExt == ".xls") { workbook = new HSSFWorkbook(); } else { workbook = null; }
//            if (workbook == null) { return; }
//            ISheet sheet = string.IsNullOrEmpty(dt.TableName) ? workbook.CreateSheet("Sheet1") : workbook.CreateSheet(dt.TableName);

//            //表头  
//            IRow row = sheet.CreateRow(0);
//            for (int i = 0; i < dt.Columns.Count; i++)
//            {
//                ICell cell = row.CreateCell(i);
//                cell.SetCellValue(dt.Columns[i].ColumnName);
//            }

//            //数据  
//            for (int i = 0; i < dt.Rows.Count; i++)
//            {
//                IRow row1 = sheet.CreateRow(i + 1);
//                for (int j = 0; j < dt.Columns.Count; j++)
//                {
//                    ICell cell = row1.CreateCell(j);
//                    if (j == 0)
//                    {

//                        cell.SetCellValue(dt.Rows[i][j].ToString());
//                    }
//                    else
//                    {
//                        cell.SetCellValue(Convert.ToInt32(dt.Rows[i][j]));
//                    }
//                }
//            }

//            //转为字节数组  
//            MemoryStream stream = new MemoryStream();
//            workbook.Write(stream);
//            var buf = stream.ToArray();

//            //保存为Excel文件  
//            using (FileStream fs = new FileStream(file, FileMode.Create, FileAccess.Write))
//            {
//                fs.Write(buf, 0, buf.Length);
//                fs.Flush();
//            }
//        }



//    }

//    class ReadExcle : IReadFile
//    {
//        /// <summary>
//        /// 导入Excel，使用Datable保存
//        /// </summary>
//        /// <param name="file">导入路径(包含文件名与扩展名)</param>
//        /// <returns></returns>
//        public DataTable Read(string file)
//        {
//            DataTable dt = new DataTable();
//            IWorkbook workbook;
//            string fileExt = Path.GetExtension(file).ToLower();
//            using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read))
//            {
//                //XSSFWorkbook 适用XLSX格式，HSSFWorkbook 适用XLS格式
//                if (fileExt == ".xlsx") { workbook = new XSSFWorkbook(fs); } else if (fileExt == ".xls") { workbook = new HSSFWorkbook(fs); } else { workbook = null; }
//                if (workbook == null) { return null; }
//                ISheet sheet = workbook.GetSheetAt(0);

//                //表头  
//                IRow header = sheet.GetRow(sheet.FirstRowNum);
//                List<int> columns = new List<int>();
//                for (int i = 0; i < header.LastCellNum; i++)
//                {
//                    object obj = GetValueType(header.GetCell(i));
//                    if (obj == null || obj.ToString() == string.Empty)
//                    {
//                        dt.Columns.Add(new DataColumn("Columns" + i.ToString()));
//                    }
//                    else
//                        dt.Columns.Add(new DataColumn(obj.ToString()));
//                    columns.Add(i);
//                }
//                //数据  
//                for (int i = sheet.FirstRowNum + 1; i <= sheet.LastRowNum; i++)
//                {
//                    DataRow dr = dt.NewRow();
//                    bool hasValue = false;
//                    foreach (int j in columns)
//                    {
//                        dr[j] = GetValueType(sheet.GetRow(i).GetCell(j));
//                        if (dr[j] != null && dr[j].ToString() != string.Empty)
//                        {
//                            hasValue = true;
//                        }
//                    }
//                    if (hasValue)
//                    {
//                        dt.Rows.Add(dr);
//                    }
//                }
//            }
//            return dt;
//        }

//        /// <summary>
//        /// 获取单元格类型
//        /// </summary>
//        /// <param name="cell"></param>
//        /// <returns></returns>
//        private object GetValueType(ICell cell)
//        {
//            if (cell == null)
//                return null;
//            switch (cell.CellType)
//            {
//                case CellType.Blank: //BLANK:  
//                    return null;
//                case CellType.Boolean: //BOOLEAN:  
//                    return cell.BooleanCellValue;
//                case CellType.Numeric: //NUMERIC:  
//                    return cell.NumericCellValue;
//                case CellType.String: //STRING:  
//                    return cell.StringCellValue;
//                case CellType.Error: //ERROR:  
//                    return cell.ErrorCellValue;
//                case CellType.Formula: //FORMULA:  
//                default:
//                    return "=" + cell.CellFormula;
//            }
//        }

//    }

//    class ReadFileAndImportDatabase
//    {

//        public static bool Execute(string fileName)
//        {
//            try
//            {
//                IReadFile readFile = null;
//                if (Path.GetExtension(fileName).Equals(".csv"))
//                {
//                    readFile = new ReadCsv();
//                }
//                else if (Path.GetExtension(fileName).Equals(".xlsx"))
//                {
//                    readFile = new ReadExcle();
//                }

//                DataTable dt = readFile.Read(fileName);
//                if (readFile != null)
//                {
//                    string tableName = "[demo].[dbo].[studentInfo]";
//                    BatchImport(tableName, ref dt);
//                }
//                return true;

//            }
//            catch(Exception e)
//            {
//                throw e;
//            }
            
//        }



//        /// <summary>
//        /// 
//        ///        批量导入数据到数据库中
//        /// 
//        /// </summary>
//        /// <param name="tableName">到导入的表名</param>
//        /// <param name="dt">要导入的数据，用DataTable保存</param>
//        /// <returns>导入数据库成功返回true</returns>
//        private static bool BatchImport(string tableName, ref DataTable dt)
//        {
//            string strConn = "Server=localhost; Database=demo; uid = sa; pwd = Jqs123456; Trusted_Connection = False";
//            SqlConnection sqlConn = new SqlConnection(strConn);
//            try
//            {
//                if (sqlConn.State == ConnectionState.Closed)
//                {
//                    sqlConn.Open();
//                }
//                using (var bulk = new SqlBulkCopy(sqlConn))
//                {
//                    bulk.DestinationTableName = tableName;
//                    bulk.WriteToServer(dt);
//                }
//                return true;
//            }
//            catch (Exception)
//            {
//                throw;
//            }
//            finally
//            {
//                sqlConn.Close();
//            }
//        }
//    }
//}
