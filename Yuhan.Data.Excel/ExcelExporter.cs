using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using Yuhan.Common.Events;

namespace Yuhan.Data.Excel
{
    public class ExcelExporter : DataExporter
        
    {
        protected int OLE_HEADER_FONT_COLOR = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
        protected int OLE_HEADER_CELL_COLOR = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(31, 73, 125));

        protected int OLE_EVEN_ROW_CELL_COLOR = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(238, 236, 225));
        protected int OLE_ODD_ROW_CELL_COLOR = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(197, 217, 241));

        protected int START_ROW_INDEX = 1;
        protected int START_COLUMN_INDEX = 1;

        protected static String EXCEL_APPLICATION_ID = "Excel.Application";

        private static Type OfficeType
        {
            get { return Type.GetTypeFromProgID(EXCEL_APPLICATION_ID); }
        }

        public static Boolean InstalledExcel
        {
            get { return OfficeType == null ? false : true; }
        }

        public Boolean IsAsync { get; set; }

        #region Export Generic
        public override void Export<T>(IEnumerable<T> collection)
        {
            if (IsAsync)
                ExecuteExportAsync<T>(collection);
            else
                ExecuteExport<T>(collection);
        }

        protected virtual void ExecuteExport<T>(IEnumerable<T> collection)
        {
            if (InstalledExcel)
            {
                #region Initialization
                Microsoft.Office.Interop.Excel.Application xlexcel;
                Microsoft.Office.Interop.Excel.Workbook xlWorkBook;
                Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet;
                Object misValue = System.Reflection.Missing.Value;
                xlexcel = new Microsoft.Office.Interop.Excel.Application();
                xlWorkBook = xlexcel.Workbooks.Add(misValue);
                xlWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
                xlWorkSheet.Name = typeof(T).Name;

                int xlRowIndex = START_ROW_INDEX;
                int xlColumnIndex = START_COLUMN_INDEX;
                #endregion
                var properties = typeof(T).GetProperties();
                List<PropertyInfo> valiedProperties = new List<PropertyInfo>();
                foreach (PropertyInfo property in properties)
                {
                    ExcelAttribute attribute = property.GetCustomAttributes(typeof(ExcelAttribute), true).FirstOrDefault() as ExcelAttribute;
                    if (attribute != null)
                    {
                        if (!attribute.IsDisplay)
                            continue;
                        valiedProperties.Add(property);
                    }
                }

                #region Generation Column Header

                foreach (var item in valiedProperties)
                {
                    var headerName = (item.GetCustomAttributes(typeof(ExcelAttribute), true).FirstOrDefault() as ExcelAttribute).HeaderName;
                    if (String.IsNullOrEmpty(headerName))
                        headerName = item.Name;
                    xlWorkSheet.Cells[xlRowIndex, xlColumnIndex].Font.Bold = true;
                    xlWorkSheet.Cells[xlRowIndex, xlColumnIndex].Font.Color = OLE_HEADER_FONT_COLOR;
                    xlWorkSheet.Cells[xlRowIndex, xlColumnIndex].Interior.Color = OLE_HEADER_CELL_COLOR;
                    xlWorkSheet.Cells[xlRowIndex, xlColumnIndex] = headerName;
                    xlColumnIndex++;
                }
                #endregion
                #region Generation Data Row
                xlRowIndex++;
                foreach (var item in collection)
                {
                    xlColumnIndex = START_COLUMN_INDEX;
                    foreach (var property in valiedProperties)
                    {
                        var value = property.GetValue(item, null);
                        if (value == null)
                            value = String.Empty;
                        xlWorkSheet.Cells[xlRowIndex, xlColumnIndex] = value.ToString();
                        if (xlRowIndex % 2 == 0)
                            xlWorkSheet.Cells[xlRowIndex, xlColumnIndex].Interior.Color = OLE_EVEN_ROW_CELL_COLOR;
                        else
                            xlWorkSheet.Cells[xlRowIndex, xlColumnIndex].Interior.Color = OLE_ODD_ROW_CELL_COLOR;
                        xlColumnIndex++;
                    }
                    xlRowIndex++;
                }
                #endregion
                xlexcel.Visible = true;
                xlWorkSheet.Columns.AutoFit();
                OnExportCompleted(collection);
            }
        }
        protected void ExecuteExportAsync<T>(IEnumerable<T> collection)
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (sender, e) =>
            {
                ExecuteExport<T>(collection);
            };
            worker.RunWorkerCompleted += (sender, e) =>
            {
                if (e.Error != null)
                {
                    OnExportFailed(e.Error);
                }
            };
            worker.RunWorkerAsync();
        }
        #endregion

        #region Export DataSet
        public void ExportDataSet(DataSet dataSet)
        {
            if (IsAsync)
                ExecuteExportDataSetAsync(dataSet);
            else
                ExecuteExportDataSet(dataSet);
        }

        protected virtual void ExecuteExportDataSet(DataSet dataSet)
        {
            if (InstalledExcel)
            {
                #region Initialization
                Microsoft.Office.Interop.Excel.Application xlexcel;
                Microsoft.Office.Interop.Excel.Workbook xlWorkBook;
                Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet;
                Object misValue = System.Reflection.Missing.Value;
                xlexcel = new Microsoft.Office.Interop.Excel.Application();
                xlWorkBook = xlexcel.Workbooks.Add(misValue);

                #endregion
                for (int tableIdx = 1; tableIdx <= dataSet.Tables.Count; tableIdx++)
                {
                    xlWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.Worksheets.get_Item(tableIdx);
                    xlWorkSheet.Name = dataSet.Tables[tableIdx].TableName;
                    ExportDataTable(dataSet.Tables[tableIdx], xlexcel, xlWorkBook, xlWorkSheet);
                }
                xlexcel.Visible = true;
                OnExportCompleted(dataSet);
            }
        }
        protected void ExecuteExportDataSetAsync(DataSet dataSet)
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (sender, e) =>
            {
                ExecuteExportDataSet(dataSet);
            };
            worker.RunWorkerCompleted += (sender, e) =>
            {
                if (e.Error != null)
                {
                    OnExportFailed(e.Error);
                }
            };
            worker.RunWorkerAsync();
        }

        #endregion

        #region Export DataTable
        public void ExportDataTable(DataTable dataTable)
        {
            if (IsAsync)
                ExecuteExportDataTableAsync(dataTable);
            else
                ExecuteExportDataTable(dataTable);
        }

        protected virtual void ExecuteExportDataTable(DataTable dataTable)
        {
            if (InstalledExcel)
            {
                #region Initialization
                Microsoft.Office.Interop.Excel.Application xlexcel;
                Microsoft.Office.Interop.Excel.Workbook xlWorkBook;
                Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet;
                Object misValue = System.Reflection.Missing.Value;

                xlexcel = new Microsoft.Office.Interop.Excel.Application();
                xlWorkBook = xlexcel.Workbooks.Add(misValue);
                xlWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

                ExportDataTable(dataTable, xlexcel, xlWorkBook, xlWorkSheet);
                #endregion
                xlexcel.Visible = true;
                OnExportCompleted(dataTable);
            }
        }

        protected void ExecuteExportDataTableAsync(DataTable dataTable)
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (sender, e) =>
            {
                ExecuteExportDataTable(dataTable);
            };
            worker.RunWorkerCompleted += (sender, e) =>
            {
                if (e.Error != null)
                {
                    OnExportFailed(e.Error);
                }
            };
            worker.RunWorkerAsync();
        }
        #endregion

        private void ExportDataTable(DataTable dataTable,
            Microsoft.Office.Interop.Excel.Application xlexcel,
            Microsoft.Office.Interop.Excel.Workbook xlWorkBook,
            Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet)
        {
            if (InstalledExcel)
            {
                int xlRowIndex = START_ROW_INDEX;
                int xlColumnIndex = START_COLUMN_INDEX;
                #region Generation Column Header
                var dataColumns = dataTable.Columns.OfType<DataColumn>();
                foreach (var column in dataColumns)
                {
                    xlWorkSheet.Cells[xlRowIndex, xlColumnIndex].Font.Bold = true;
                    xlWorkSheet.Cells[xlRowIndex, xlColumnIndex].Font.Color = OLE_HEADER_FONT_COLOR;
                    xlWorkSheet.Cells[xlRowIndex, xlColumnIndex].Interior.Color = OLE_HEADER_CELL_COLOR;
                    xlWorkSheet.Cells[xlRowIndex, xlColumnIndex] = column.ColumnName;
                    xlColumnIndex++;
                }
                #endregion
                #region Generation Data Row
                xlRowIndex++;
                foreach (var row in dataTable.Rows.OfType<DataRow>())
                {
                    xlColumnIndex = START_COLUMN_INDEX;
                    foreach (var column in dataColumns)
                    {
                        xlWorkSheet.Cells[xlRowIndex, xlColumnIndex] = row[column.ColumnName].ToString();
                        if (xlRowIndex % 2 == 0)
                            xlWorkSheet.Cells[xlRowIndex, xlColumnIndex].Interior.Color = OLE_EVEN_ROW_CELL_COLOR;
                        else
                            xlWorkSheet.Cells[xlRowIndex, xlColumnIndex].Interior.Color = OLE_ODD_ROW_CELL_COLOR;
                        xlColumnIndex++;
                    }
                    xlRowIndex++;
                }
                #endregion
                xlWorkSheet.Columns.AutoFit();
            }
        }

        public ExcelExporter() : base() { }
    }
}
