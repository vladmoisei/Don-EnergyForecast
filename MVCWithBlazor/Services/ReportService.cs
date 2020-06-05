using Microsoft.AspNetCore.Http;
using MVCWithBlazor.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualBasic;

namespace MVCWithBlazor.Services
{
    public class ReportService
    {

        // Task returnare lista index contor gaz din fisier excel 
        public async Task<List<IndexModel>> GetBlumsListFromExcelFileBySarjaAsync(IFormFile formFile)
        //public static List<Blum> GetBlumsListFromFileAsync(IFormFile formFile)
        {
            var list = new List<IndexModel>();

            using (var stream = new MemoryStream())
            {
                await formFile.CopyToAsync(stream);

                using (var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    var rowCount = worksheet.Dimension.Rows;

                    for (int row = 3; row <= rowCount; row++)
                    {
                        DateTime dateTime = ReturnareDataFromExcel(worksheet.Cells[row, 1].Value.ToString().Trim());
                        if (list.LastOrDefault() != null && list.LastOrDefault().DataOra == dateTime)
                            continue;
                        if (IsFixHour(dateTime))
                        {
                            list.Add(new IndexModel
                            {
                                DataOra = ReturnareDataFromExcel(worksheet.Cells[row, 1].Value.ToString().Trim()),
                                EdisStatus = Convert.ToInt32(worksheet.Cells[row, 2].Value.ToString().Trim()),
                                IndexEnergyPlusA = Convert.ToDouble(worksheet.Cells[row, 3].Value.ToString().Trim()),
                                IndexEnergyMinusA = Convert.ToDouble(worksheet.Cells[row, 4].Value.ToString().Trim()),
                                IndexEnergyPlusRi = Convert.ToDouble(worksheet.Cells[row, 5].Value.ToString().Trim()),
                                IndexEnergyPlusRc = Convert.ToDouble(worksheet.Cells[row, 6].Value.ToString().Trim()),
                                IndexEnergyMinusRi = Convert.ToDouble(worksheet.Cells[row, 7].Value.ToString().Trim()),
                                IndexEnergyMinusRc = Convert.ToDouble(worksheet.Cells[row, 8].Value.ToString().Trim()),
                            });
                        }
                    }
                }
            }

            return list;
        }

        // Auxiliar Functions
        // Functie convertire din string in dateTime format excel "serial number",
        public DateTime ReturnareDataFromExcel(string dateToParse)
        {
            double dateValue = Convert.ToDouble(dateToParse);  // not "Text"
            DateTime dateTime = DateTime.FromOADate(dateValue);
            return dateTime;
        }

        // Check if it is Fix Hour
        public bool IsFixHour(DateTime dateTime)
        {
            string timp = dateTime.TimeOfDay.ToString().Substring(3,2);
            if (timp == "00") return true;
            return false;
        }
        // Check if is hour 00:00 
        public bool IsTimeOfDayZeroZero(DateTime dateTime)
        {
            if (dateTime.TimeOfDay == new TimeSpan(0)) return true;
            return false;
        }
    }
}
