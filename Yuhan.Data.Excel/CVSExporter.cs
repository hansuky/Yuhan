using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Yuhan.Data.Excel
{
    public class CVSExporter : DataExporter
    {
        /// <summary>
        /// 파일 저장 경로를 설정하거나 가져옵니다.
        /// </summary>
        public String FilePath { get; set; }
        private const String DELIMITER = ",";

        private void WriteToFile(String text)
        {
            using (StreamWriter file = new StreamWriter(FilePath, false, Encoding.UTF8))
            {
                file.WriteLine(text);
                file.Close();
            }
        }

        public override void Export<T>(IEnumerable<T> collection)
        {
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

            StringBuilder output = new StringBuilder();

            #region Generation Column Header
            foreach (var item in valiedProperties)
            {
                var headerName = (item.GetCustomAttributes(typeof(ExcelAttribute), true).FirstOrDefault() as ExcelAttribute).HeaderName;
                if (String.IsNullOrEmpty(headerName))
                    headerName = item.Name;

                output.Append(headerName);
                if(valiedProperties.IndexOf(item) != valiedProperties.IndexOf(valiedProperties.Last()))
                    output.Append(DELIMITER);
            }
            output.AppendLine();
            #endregion

            #region Generation Data Row
            foreach (var item in collection)
            {
                foreach (var property in valiedProperties)
                {
                    var value = property.GetValue(item, null);
                    if (value == null)
                        value = String.Empty;
                    output.Append(value);
                    if (valiedProperties.IndexOf(property) != valiedProperties.IndexOf(valiedProperties.Last()))
                        output.Append(DELIMITER);
                }
                output.AppendLine();
            }
            #endregion

            WriteToFile(output.ToString());
            OnExportCompleted(collection);
        }

        public void ExportDataTable(DataTable dataTable)
        {
            #region Generation Column Header
            var dataColumns = dataTable.Columns.OfType<DataColumn>();
            StringBuilder output = new StringBuilder();
            for (int columnIndex = 0; columnIndex < dataColumns.Count(); columnIndex++)
            {
                output.Append(dataColumns.ElementAt(columnIndex).ColumnName);
                if (columnIndex < dataColumns.Count())
                    output.Append(DELIMITER);
            }
            output.AppendLine();
            #endregion
            #region Generation Data Row
            foreach (var row in dataTable.Rows.OfType<DataRow>())
            {
                for (int columnIndex = 0; columnIndex < dataColumns.Count(); columnIndex++)
                {
                    output.Append(row[columnIndex].ToString());
                    if (columnIndex < dataColumns.Count())
                        output.Append(DELIMITER);
                }
                output.AppendLine();
            }
            #endregion
            WriteToFile(output.ToString());
            OnExportCompleted(dataTable);
        }
    }
}
