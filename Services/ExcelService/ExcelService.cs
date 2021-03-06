/*
Manual Whisker Annotator - A program to manually annotate whiskers and analyse them
Copyright (C) 2015 Brett Michael Hewitt

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Office.Interop.Excel;
using RobynsWhiskerTracker.Extension;
using Application = Microsoft.Office.Interop.Excel.Application;

namespace RobynsWhiskerTracker.Services.ExcelService
{
    public class ExcelService
    {
        public static string[,] GetRawData(string fileName)
        {
            Application excelApp = new Application();
            string[,] result;

            try
            {
                Workbook workBook = excelApp.Workbooks.Open(fileName);

                Worksheet worksheet = workBook.Sheets[1];
                Range xlRange = worksheet.UsedRange;

                int rowCount = xlRange.Rows.Count;
                int colCount = xlRange.Columns.Count;

                result = new string[rowCount, colCount];

                for (int i = 1; i <= rowCount; i++)
                {
                    for (int j = 1; j <= colCount; j++)
                    {
                        result[i - 1, j - 1] = xlRange.Cells[i, j].Value2;
                    }
                }
                workBook.Close(false, fileName, null);
                Marshal.ReleaseComObject(workBook);
                excelApp.Quit();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }

            return result;
        }

        public static T?[,] GetData<T>(string fileName) where T : struct
        {
            Application excelApp = new Application();
            T?[,] result;

            try
            {
                Workbook workBook = excelApp.Workbooks.Open(fileName);

                Worksheet worksheet = workBook.Sheets[1];
                Range xlRange = worksheet.UsedRange;

                int rowCount = xlRange.Rows.Count;
                int colCount = xlRange.Columns.Count;

                result = new T?[rowCount,colCount];

                for (int i = 1; i <= rowCount; i++)
                {
                    for (int j = 1; j <= colCount; j++)
                    {
                        if (xlRange.Cells[i, j].Value2 is T)
                        {
                            result[i - 1, j - 1] = (T)xlRange.Cells[i, j].Value2;
                        }
                        else
                        {
                            result[i - 1, j - 1] = null;
                        }
                    }
                }
                workBook.Close(false, fileName, null);
                Marshal.ReleaseComObject(workBook);
                excelApp.Quit();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }

            return result;
        }

        public static T?[,] GetData<T>(string fileName, out int rowCount, out int columnCount) where T : struct
        {
            Application excelApp = new Application();
            T?[,] result;
            rowCount = 0;
            columnCount = 0;

            try
            {
                Workbook workBook = excelApp.Workbooks.Open(fileName);

                Worksheet worksheet = workBook.Sheets[1];
                Range xlRange = worksheet.UsedRange;

                rowCount = xlRange.Rows.Count;
                columnCount = xlRange.Columns.Count;

                result = new T?[rowCount, columnCount];

                for (int i = 1; i <= rowCount; i++)
                {
                    for (int j = 1; j <= columnCount; j++)
                    {
                        if (xlRange.Cells[i, j].Value is T)
                        {
                            result[i - 1, j - 1] = (T)xlRange.Cells[i, j].Value;
                        }
                        else
                        {
                            result[i - 1, j - 1] = null;
                        }
                    }
                }
                workBook.Close(false, fileName, null);
                Marshal.ReleaseComObject(workBook);
                excelApp.Quit();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }

            return result;
        }

        public static void WriteData<T>(T?[,] data, string fileName) where T : struct
        {
            Application excelApp = new Application();

            if (excelApp == null)
            {
                Console.WriteLine("Failed to open excel");
                return;
            }

            Workbook workBook = excelApp.Workbooks.Add(XlWBATemplate.xlWBATWorksheet);
            Worksheet ws = (Worksheet)workBook.Worksheets[1];

            int rowCount = data.GetLength(0);
            int columnCount = data.GetLength(1);

            for (int i = 1; i <= rowCount; i++)
            {
                for (int j = 1; j <= columnCount; j++)
                {
                    if (data[i - 1, j - 1].HasValue)
                    {
                        ws.Cells[i, j] = data[i - 1, j - 1].Value;
                    }
                }
            }

            workBook.SaveAs(fileName);
            excelApp.Quit();
            Marshal.ReleaseComObject(workBook);
            Marshal.ReleaseComObject(ws);
        }

        public static void WriteData<T>(T[,] data, string fileName, bool displayWarnings = true) where T : class
        {
            Application excelApp = new Application();

            if (excelApp == null)
            {
                Console.WriteLine("Failed to open excel");
                return;
            }

            excelApp.DisplayAlerts = displayWarnings;
            Workbook workBook = excelApp.Workbooks.Add(XlWBATemplate.xlWBATWorksheet);
            Worksheet ws = (Worksheet)workBook.Worksheets[1];

            int rowCount = data.GetLength(0);
            int columnCount = data.GetLength(1);

            for (int i = 1; i <= rowCount; i++)
            {
                for (int j = 1; j <= columnCount; j++)
                {
                    ws.Cells[i, j] = data[i - 1, j - 1];
                }
            }

            workBook.SaveAs(fileName);
            excelApp.Quit();
            Marshal.ReleaseComObject(workBook);
            Marshal.ReleaseComObject(ws);
        }
    }
}
