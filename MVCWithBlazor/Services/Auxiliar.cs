using Microsoft.AspNetCore.Http;
using MVCWithBlazor.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MVCWithBlazor.Services
{
    public class Auxiliar
    {

        // Functie reutrn clock de pe server in view
        public string GetClock()
        {
            return DateTime.Now.ToString("HH:mm:ss");
        }

        // Functie convertire din string in dateTime format
        public DateTime ReturnareDataFromString(string dateToParse)
        {

            DateTime parsedDate = DateTime.ParseExact(dateToParse,
                                                      "dd.MM.yyyy HH:mm:ss",
                                                      CultureInfo.InvariantCulture);
            return parsedDate;
        }

        // Functie verificare data este din ziua de azi
        public bool IsCurrentDay(DateTime data)
        {
            if (data.Day == DateTime.Now.Day) return true;
            return false;
        }

        // Functie verificare data este din luna curenta
        public bool IsCurrentMonth(DateTime data)
        {
            if (data.Month == DateTime.Now.Month) return true;
            return false;
        }

        // Functie verificare data cuprinse intre 2 date (format string) 
        public bool IsDateBetween(string dataItemString, string dataFromString, string dataToString)
        {
            try
            {
                // Convert string data received from View to DateTime format
                DateTime dataItem = ReturnareDataFromString(dataItemString);
                DateTime dataFrom = ReturnareDataFromString(dataFromString + " 00:00:00");
                DateTime dataTo = ReturnareDataFromString(dataToString + " 00:00:00");
                if (dataItem.CompareTo(dataFrom) >= 0)
                {
                    if (dataItem.CompareTo(dataTo) <= 0)
                    {
                        return true;
                    }
                }
            }
            catch (Exception)
            {

                return false; ;
            }


            return false;
        }

        // Functie returnare data maine format scurt + 1 zi
        public string GetTomorrowDate()
        {
            return DateTime.Now.AddDays(1).ToString("dd.MM.yyyy");
        }

        // Functie returnare data cu o luna in urma
        public string GetOneMonthBeforeDate()
        {
            return DateTime.Now.AddMonths(-1).ToString("dd.MM.yyyy");
        }

        // Task returnare lista blumuri din fisier excel 
        // Introducere in fisier sarja de blumuri, si extrag de acolo blum cu blum
        public async Task<List<PlcModel>> GetBlumsListFromExcelFileBySarjaAsync(IFormFile formFile)
        //public static List<Blum> GetBlumsListFromFileAsync(IFormFile formFile)
        {
            var list = new List<PlcModel>();

            using (var stream = new MemoryStream())
            {
                await formFile.CopyToAsync(stream);

                using (var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    var rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++)
                    {
                        int plcModelID = int.TryParse(worksheet.Cells[row, 1].Value.ToString().Trim(), out int d) ? d : 0;
                        string name = worksheet.Cells[row, 2].Value.ToString().Trim();
                        DateTime dataCreation = ReturnareDataFromString(worksheet.Cells[row, 3].Value.ToString().Trim());
                        bool isEnable = Convert.ToBoolean(worksheet.Cells[row, 4].Value.ToString().Trim());
                        string ip = worksheet.Cells[row, 5].Value.ToString().Trim();
                        short rack = Convert.ToInt16(worksheet.Cells[row, 6].Value.ToString().Trim());
                        short slot = Convert.ToInt16(worksheet.Cells[row, 7].Value.ToString().Trim());


                        list.Add(new PlcModel
                            {
                                //PlcModelID = plcModelID,
                                Name = name,
                                DataCreation = dataCreation,
                                IsEnable = isEnable,
                                Ip = ip,
                                Rack = rack,
                                Slot = slot
                            });
                        
                    }
                }
            }

            return list;
        }
    }
}
