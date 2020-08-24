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
using MVCWithBlazor.Data;
using Microsoft.AspNetCore.Mvc;
using MVCWithBlazor.Controllers;
using OfficeOpenXml.Style;

namespace MVCWithBlazor.Services
{
    public class ReportService
    {
        // Get ReportMonthValoriViewModel For Forecast Index View by selected Month
        public ReportMonthValoriViewModel GetMonthValoriPrognozaFromList(List<PrognozaEnergieModel> lista, int zleInLuna)
        {
            ReportMonthValoriViewModel valoriPeLuna = new ReportMonthValoriViewModel() { Valori = new double[zleInLuna + 1, 24], TotalperZi = new double[zleInLuna + 1]};
            for (int i = 1; i <= zleInLuna; i++)
            {
                for (int j = 0; j < 24; j++)
                {
                    //raport.TabeleValori[0].Valori[i, j]
                    var variabila = lista.Where(elem =>
                        elem.DataOra.Day == i &&
                        elem.DataOra.Hour == j
                    ).Select(x => x.Valoare
                    ).FirstOrDefault();
                    if (variabila == null) continue;
                    valoriPeLuna.Valori[i, j] = variabila;
                }
                for (int j = 0; j < 24; j++)
                {
                    valoriPeLuna.TotalperZi[i] += valoriPeLuna.Valori[i, j];
                    valoriPeLuna.TotalperZi[i] = Math.Round(valoriPeLuna.TotalperZi[i], 1);
                }

                valoriPeLuna.TotalperLuna += valoriPeLuna.TotalperZi[i];
                valoriPeLuna.TotalperLuna = Math.Round(valoriPeLuna.TotalperLuna);

            }
            return valoriPeLuna;
        }
        // Task returnare lista prognoza energie din fisier excel
        public async Task<List<PrognozaEnergieModel>> GetPrognozaForMonthFromFileAsync(IFormFile formFile, DateTime data)
        {
            var list = new List<PrognozaEnergieModel>();

            using (var stream = new MemoryStream())
            {
                await formFile.CopyToAsync(stream);

                using (var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    var rowCount = worksheet.Dimension.Rows;

                    for (int row = 4; row <= 27; row++)
                    {
                        for (int j = 0; j < DateTime.DaysInMonth(data.Year, data.Month); j++)
                        {
                            list.Add(new PrognozaEnergieModel
                            {
                                DataOra = new DateTime(data.Year, data.Month, j+1, row - 4, 0, 0),
                                Ora = row - 3,
                                Valoare = Convert.ToDouble(worksheet.Cells[row, j+2].Value.ToString().Trim())
                            });
                        }
                    }
                }
            }

            return list;
        }
        // Task returnare lista index consum energie din fisier excel 
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

                    for (int row = 3; row <= (rowCount + 1); row++)
                    {
                        DateTime dateTime = ReturnareDataFromExcel(worksheet.Cells[row, 1].Value.ToString().Trim());
                        if (list.LastOrDefault() != null && list.LastOrDefault().DataOra == dateTime)
                            continue;
                        if (IsFixHour(dateTime))
                        {
                            list.Add(new IndexModel
                            {
                                DataOra = dateTime,
                                EdisStatus = Convert.ToInt32(worksheet.Cells[row, 2].Value.ToString().Trim()),
                                IndexEnergyPlusA = Convert.ToDouble(worksheet.Cells[row, 3].Value.ToString().Trim()),
                                IndexEnergyMinusA = Convert.ToDouble(worksheet.Cells[row, 4].Value.ToString().Trim()),
                                IndexEnergyPlusRi = Convert.ToDouble(worksheet.Cells[row, 5].Value.ToString().Trim()),
                                IndexEnergyPlusRc = Convert.ToDouble(worksheet.Cells[row, 6].Value.ToString().Trim()),
                                IndexEnergyMinusRi = Convert.ToDouble(worksheet.Cells[row, 7].Value.ToString().Trim()),
                                IndexEnergyMinusRc = Convert.ToDouble(worksheet.Cells[row, 8].Value.ToString().Trim()),
                                Ora = dateTime.Hour + 1,
                            });
                            if (list.Count > 1)
                            {
                                list[list.Count - 2].ValueEnergyPlusA = GetCalculateValueEnergyPlusA(list[list.Count - 1], list[list.Count - 2]);
                                list[list.Count - 2].ValueEnergyPlusRi = GetCalculateValueEnergyPlusRi(list[list.Count - 1], list[list.Count - 2]);
                                list[list.Count - 2].ValueEnergyMinusRc = GetCalculateValueEnergyMinusRc(list[list.Count - 1], list[list.Count - 2]);
                                list[list.Count - 2].CosFiInductiv = GetCalculateValueCosFiInductiv(list[list.Count - 2]);
                                list[list.Count - 2].CosFiCapacitiv = GetCalculateValueCosFiCapacitiv(list[list.Count - 2]);
                                list[list.Count - 2].EnergiiOrareFacturareRiPlus = GetCalculateValueEnergiiOrareFacturareRiPlus(list[list.Count - 2]);
                            }

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

        // Check if it is Fix Hour hh:00:00
        public bool IsFixHour(DateTime dateTime)
        {
            string timp = dateTime.TimeOfDay.ToString().Substring(3, 2);
            if (timp == "00") return true;
            return false;
        }
        // Check if is hour 00:00:00 
        public bool IsTimeOfDayZeroZero(DateTime dateTime)
        {
            if (dateTime.TimeOfDay == new TimeSpan(0)) return true;
            return false;
        }

        // Get Calculate Value Energy +A
        public double GetCalculateValueEnergyPlusA(IndexModel elemMinusUnu, IndexModel elemMinusDoi)
        {
            return Math.Round(elemMinusUnu.IndexEnergyPlusA - elemMinusDoi.IndexEnergyPlusA);
        }
        // Get Calculate Value Energy +Ri
        public double GetCalculateValueEnergyPlusRi(IndexModel elemMinusUnu, IndexModel elemMinusDoi)
        {
            return Math.Round(elemMinusUnu.IndexEnergyPlusRi - elemMinusDoi.IndexEnergyPlusRi);
        }
        // Get Calculate Value Energy -Rc
        public double GetCalculateValueEnergyMinusRc(IndexModel elemMinusUnu, IndexModel elemMinusDoi)
        {
            return Math.Round(elemMinusUnu.IndexEnergyMinusRc - elemMinusDoi.IndexEnergyMinusRc);
        }
        // Get Calculate Value Cos Fi Inductiv
        public double GetCalculateValueCosFiInductiv(IndexModel elemMinusDoi)
        {
            return Math.Round(elemMinusDoi.ValueEnergyPlusA / Math.Sqrt(Math.Pow(elemMinusDoi.ValueEnergyPlusA, 2) + Math.Pow(elemMinusDoi.ValueEnergyPlusRi, 2)), 2);
        }
        // Get Calculate Value Cos Fi Capacitiv
        public double GetCalculateValueCosFiCapacitiv(IndexModel elemMinusDoi)
        {
            return Math.Round(elemMinusDoi.ValueEnergyPlusA / Math.Sqrt(Math.Pow(elemMinusDoi.ValueEnergyPlusA, 2) + Math.Pow(elemMinusDoi.ValueEnergyMinusRc, 2)), 2);
        }
        // Get Calculate Value Energii Orare Facturare Ri+
        public double GetCalculateValueEnergiiOrareFacturareRiPlus(IndexModel elemMinusDoi)
        {
            return Math.Round(elemMinusDoi.ValueEnergyPlusRi - Math.Tan(Math.Acos(0.9)) * elemMinusDoi.ValueEnergyPlusA, 2) > 0 ? Math.Round(elemMinusDoi.ValueEnergyPlusRi - Math.Tan(Math.Acos(0.9)) * elemMinusDoi.ValueEnergyPlusA) : 0;
        }

        // Get New Report ReportMonthValoriViewModel
        public ReportMonthViewModel GetNewReporthValoriVieModelByDate(DateTime data)
        {
            int zileInLuna = DateTime.DaysInMonth(data.Year, data.Month);
            return new ReportMonthViewModel
            {
                Luna = data.Month,
                An = data.Year,
                ZileInLuna = zileInLuna,
                TabeleValori = new ReportMonthValoriViewModel[]{
                    new ReportMonthValoriViewModel { Denumire = "energyPlusA" , Valori = new double[zileInLuna + 1, 24] , TotalperZi = new double[zileInLuna + 1], TotalperLuna = 0},
                    new ReportMonthValoriViewModel { Denumire = "energyPlusRi" , Valori = new double[zileInLuna + 1, 24] , TotalperZi = new double[zileInLuna + 1], TotalperLuna = 0},
                    new ReportMonthValoriViewModel { Denumire = "energyMinusRc" , Valori = new double[zileInLuna + 1, 24] , TotalperZi = new double[zileInLuna + 1], TotalperLuna = 0},
                    new ReportMonthValoriViewModel { Denumire = "cosFiInductiv" , Valori = new double[zileInLuna + 1, 24] , TotalperZi = new double[zileInLuna + 1], TotalperLuna = 0},
                    new ReportMonthValoriViewModel { Denumire = "cosFiCapacitiv" , Valori = new double[zileInLuna + 1, 24] , TotalperZi = new double[zileInLuna + 1], TotalperLuna = 0},
                    new ReportMonthValoriViewModel { Denumire = "RiPlusEnergiiOrare" , Valori = new double[zileInLuna +1, 24] , TotalperZi = new double[zileInLuna + 1], TotalperLuna = 0}
                }
            };
        }

        // Get Values For Selected Month
        public ReportMonthViewModel GetViewModelForSelectedMonth(ReportDbContext context, DateTime data)
        {
            ReportMonthViewModel raport = GetNewReporthValoriVieModelByDate(data);

            for (int i = 1; i <= raport.ZileInLuna; i++)
            {
                for (int j = 0; j < 24; j++)
                {
                    //raport.TabeleValori[0].Valori[i, j]
                    var variabile = context.Indexes.Where(elem =>
                        elem.DataOra.Year == data.Year &&
                        elem.DataOra.Month == data.Month &&
                        elem.DataOra.Day == i &&
                        elem.DataOra.Hour == j
                    ).ToList();
                    if (variabile == null) continue;

                    var variabila = variabile.Select(x => new ElemSelectieModel
                    {
                        EnergyPlusA = x.ValueEnergyPlusA,
                        EnergyPlusRi = x.ValueEnergyPlusRi,
                        EnergyMinusRc = x.ValueEnergyMinusRc,
                        CosFiInductiv = x.CosFiInductiv,
                        CosFiCapacitiv = x.CosFiCapacitiv,
                        RiPlusEnergiiOrare = x.EnergiiOrareFacturareRiPlus
                    }
                    ).FirstOrDefault();
                    if (variabila == null) continue;

                    raport.TabeleValori[0].Valori[i, j] = variabila.EnergyPlusA;
                    raport.TabeleValori[1].Valori[i, j] = variabila.EnergyPlusRi;
                    raport.TabeleValori[2].Valori[i, j] = variabila.EnergyMinusRc;
                    raport.TabeleValori[3].Valori[i, j] = variabila.CosFiInductiv;
                    raport.TabeleValori[4].Valori[i, j] = variabila.CosFiCapacitiv;
                    raport.TabeleValori[5].Valori[i, j] = variabila.RiPlusEnergiiOrare;
                }
                for (int j = 0; j < 24; j++)
                {
                    raport.TabeleValori[0].TotalperZi[i] += raport.TabeleValori[0].Valori[i, j];
                    raport.TabeleValori[1].TotalperZi[i] += raport.TabeleValori[1].Valori[i, j];
                    raport.TabeleValori[2].TotalperZi[i] += raport.TabeleValori[2].Valori[i, j];
                    raport.TabeleValori[5].TotalperZi[i] += raport.TabeleValori[5].Valori[i, j];
                }

                // For cos Fi Total for each day
                raport.TabeleValori[3].TotalperZi[i] = GetCalculateValueCosFiInductiv(new IndexModel { ValueEnergyPlusA = raport.TabeleValori[0].TotalperZi[i], ValueEnergyPlusRi = raport.TabeleValori[1].TotalperZi[i] });
                raport.TabeleValori[4].TotalperZi[i] = GetCalculateValueCosFiCapacitiv(new IndexModel { ValueEnergyPlusA = raport.TabeleValori[0].TotalperZi[i], ValueEnergyMinusRc = raport.TabeleValori[2].TotalperZi[i] });

                raport.TabeleValori[0].TotalperLuna += raport.TabeleValori[0].TotalperZi[i];
                raport.TabeleValori[1].TotalperLuna += raport.TabeleValori[1].TotalperZi[i];
                raport.TabeleValori[2].TotalperLuna += raport.TabeleValori[2].TotalperZi[i];
                raport.TabeleValori[5].TotalperLuna += raport.TabeleValori[5].TotalperZi[i];
            }
            // For cos Fi afisam total per motnh
            raport.TabeleValori[3].TotalperLuna = GetCalculateValueCosFiInductiv(new IndexModel { ValueEnergyPlusA = raport.TabeleValori[0].TotalperLuna, ValueEnergyPlusRi = raport.TabeleValori[1].TotalperLuna });
            raport.TabeleValori[4].TotalperLuna = GetCalculateValueCosFiCapacitiv(new IndexModel { ValueEnergyPlusA = raport.TabeleValori[0].TotalperLuna, ValueEnergyMinusRc = raport.TabeleValori[2].TotalperLuna });

            return raport;
        }

        // Get Excel File From Report
        public FileStreamResult GetExcelFileFromReport(List<IndexModel> listOfIndexes, ReportMonthViewModel report)
        {
            var controler = new ProbaController(); // Initialize new controller to use File Method for Excel File
            var stream = new MemoryStream();

            using (var pck = new ExcelPackage(stream))
            {
                // EXCEL WORKSHEET FOR INDEXES
                ExcelWorksheet wsIndex = pck.Workbook.Worksheets.Add($"{report.An}.{report.Luna} Indexe orare");
                wsIndex.Cells["A1:Z1"].Style.Font.Bold = true;

                wsIndex.Cells["A1"].Value = "Clock";
                wsIndex.Cells["B1"].Value = "EDIS status";
                wsIndex.Cells["C1"].Value = "Energy +A";
                wsIndex.Cells["D1"].Value = "Energy -A";
                wsIndex.Cells["E1"].Value = "Energy +Ri";
                wsIndex.Cells["F1"].Value = "Energy +Rc";
                wsIndex.Cells["G1"].Value = "Energy -Ri";
                wsIndex.Cells["H1"].Value = "Energy -Rc";

                int rowStart = 2;
                foreach (var elem in listOfIndexes)
                {
                    wsIndex.Cells[string.Format("A{0}", rowStart)].Value = elem.DataOra;
                    wsIndex.Cells[string.Format("B{0}", rowStart)].Value = elem.EdisStatus;
                    wsIndex.Cells[string.Format("C{0}", rowStart)].Value = elem.IndexEnergyPlusA;
                    wsIndex.Cells[string.Format("D{0}", rowStart)].Value = elem.IndexEnergyMinusA;
                    wsIndex.Cells[string.Format("E{0}", rowStart)].Value = elem.IndexEnergyPlusRi;
                    wsIndex.Cells[string.Format("F{0}", rowStart)].Value = elem.IndexEnergyPlusRc;
                    wsIndex.Cells[string.Format("G{0}", rowStart)].Value = elem.IndexEnergyMinusRi;
                    wsIndex.Cells[string.Format("H{0}", rowStart)].Value = elem.IndexEnergyMinusRc;
                    // Set background Yellow color for fix Hour when it starts a new day
                    if (elem.DataOra.Hour == 0) 
                    {
                        wsIndex.Cells[string.Format("A{0}:Z{0}", rowStart)].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        wsIndex.Cells[string.Format("A{0}:Z{0}", rowStart)].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Yellow);
                    }
                    rowStart++;
                }
                wsIndex.Cells["A:AZ"].AutoFitColumns();

                // Another SHEET REport
                // EXCEL WORKSHEET FOR A+ Energii orare
                ExcelWorksheet wsEnergyPlusA = pck.Workbook.Worksheets.Add($"{report.An}.{report.Luna} A+ energii orare");
                // Set Head of table header Style
                wsEnergyPlusA.Cells["A1:AG1"].Style.Font.Bold = true;
                // Set Values of Table Header
                wsEnergyPlusA.Cells["A1"].Value = "Ora/ Zi";
                for (int i = 1; i <= report.ZileInLuna; i++)
                {
                    wsEnergyPlusA.Cells[$"{GetCharStringsFromNumber(65 + i)}1"].Value = i;
                }

                // Set Values from report in excel file
                for (int j = 0; j < 24; j++)
                {
                    wsEnergyPlusA.Cells[$"A{j + 2}"].Style.Font.Bold = true;
                    wsEnergyPlusA.Cells[$"A{j + 2}"].Value = j + 1;
                    for (int i = 1; i <= report.ZileInLuna; i++)
                    {
                        wsEnergyPlusA.Cells[$"{GetCharStringsFromNumber(65 + i)}{j + 2}"].Value = report.TabeleValori[0].Valori[i, j];
                    }
                }
                // Show Values per each day of mounth
                wsEnergyPlusA.Cells[$"A{27}"].Value = " Total: ";
                for (int i = 1; i <= (report.ZileInLuna); i++)
                {
                    wsEnergyPlusA.Cells[$"{GetCharStringsFromNumber(65 + i)}27"].Value = report.TabeleValori[0].TotalperZi[i];
                }
                wsEnergyPlusA.Cells[$"A27:AG27"].Style.Font.Bold = true;
                wsEnergyPlusA.Cells[$"{GetCharStringsFromNumber(66 + report.ZileInLuna)}27"].Value = $"{report.TabeleValori[0].TotalperLuna} kWh";

                wsEnergyPlusA.Cells["A:AZ"].AutoFitColumns();

                // Another SHEET REport
                // EXCEL WORKSHEET FOR Ri+ Energii orare
                ExcelWorksheet wsEnergyPlusRi = pck.Workbook.Worksheets.Add($"{report.An}.{report.Luna} Ri+ energii orare");
                // Set Head of table header Style
                wsEnergyPlusRi.Cells["A1:AG1"].Style.Font.Bold = true;
                // Set Values of Table Header
                wsEnergyPlusRi.Cells["A1"].Value = "Ora/ Zi";
                for (int i = 1; i <= report.ZileInLuna; i++)
                {
                    wsEnergyPlusRi.Cells[$"{GetCharStringsFromNumber(65 + i)}1"].Value = i;
                }

                // Set Values from report in excel file
                for (int j = 0; j < 24; j++)
                {
                    wsEnergyPlusRi.Cells[$"A{j + 2}"].Style.Font.Bold = true;
                    wsEnergyPlusRi.Cells[$"A{j + 2}"].Value = j + 1;
                    for (int i = 1; i <= report.ZileInLuna; i++)
                    {
                        wsEnergyPlusRi.Cells[$"{GetCharStringsFromNumber(65 + i)}{j + 2}"].Value = report.TabeleValori[1].Valori[i, j];
                    }
                }
                // Show Values per each day of mounth
                wsEnergyPlusRi.Cells[$"A{27}"].Value = " Total: ";
                for (int i = 1; i <= (report.ZileInLuna); i++)
                {
                    wsEnergyPlusRi.Cells[$"{GetCharStringsFromNumber(65 + i)}27"].Value = report.TabeleValori[1].TotalperZi[i];
                }
                wsEnergyPlusRi.Cells[$"A27:AG27"].Style.Font.Bold = true;
                wsEnergyPlusRi.Cells[$"{GetCharStringsFromNumber(66 + report.ZileInLuna)}27"].Value = $"{report.TabeleValori[1].TotalperLuna} kVArh";

                wsEnergyPlusRi.Cells["A:AZ"].AutoFitColumns();

                // Another SHEET REport
                // EXCEL WORKSHEET FOR Rc- Energii orare
                ExcelWorksheet wsEnergyMinusRc = pck.Workbook.Worksheets.Add($"{report.An}.{report.Luna} Rc- energii orare");
                // Set Head of table header Style
                wsEnergyMinusRc.Cells["A1:AG1"].Style.Font.Bold = true;
                // Set Values of Table Header
                wsEnergyMinusRc.Cells["A1"].Value = "Ora/ Zi";
                for (int i = 1; i <= report.ZileInLuna; i++)
                {
                    wsEnergyMinusRc.Cells[$"{GetCharStringsFromNumber(65 + i)}1"].Value = i;
                }

                // Set Values from report in excel file
                for (int j = 0; j < 24; j++)
                {
                    wsEnergyMinusRc.Cells[$"A{j + 2}"].Style.Font.Bold = true;
                    wsEnergyMinusRc.Cells[$"A{j + 2}"].Value = j + 1;
                    for (int i = 1; i <= report.ZileInLuna; i++)
                    {
                        wsEnergyMinusRc.Cells[$"{GetCharStringsFromNumber(65 + i)}{j + 2}"].Value = report.TabeleValori[2].Valori[i, j];
                    }
                }
                // Show Values per each day of mounth
                wsEnergyMinusRc.Cells[$"A{27}"].Value = " Total: ";
                for (int i = 1; i <= (report.ZileInLuna); i++)
                {
                    wsEnergyMinusRc.Cells[$"{GetCharStringsFromNumber(65 + i)}27"].Value = report.TabeleValori[2].TotalperZi[i];
                }
                wsEnergyMinusRc.Cells[$"A27:AG27"].Style.Font.Bold = true;
                wsEnergyMinusRc.Cells[$"{GetCharStringsFromNumber(66 + report.ZileInLuna)}27"].Value = $"{report.TabeleValori[2].TotalperLuna} kVArh";

                wsEnergyMinusRc.Cells["A:AZ"].AutoFitColumns();

                // Another SHEET REport
                // EXCEL WORKSHEET FOR Cos Fi Inductiv Energii orare
                ExcelWorksheet wsCosFiInductiv = pck.Workbook.Worksheets.Add($"{report.An}.{report.Luna} Cos Fi inductiv");
                // Set Head of table header Style
                wsCosFiInductiv.Cells["A1:AG1"].Style.Font.Bold = true;
                // Set Values of Table Header
                wsCosFiInductiv.Cells["A1"].Value = "Ora/ Zi";
                for (int i = 1; i <= report.ZileInLuna; i++)
                {
                    wsCosFiInductiv.Cells[$"{GetCharStringsFromNumber(65 + i)}1"].Value = i;
                }

                // Set Values from report in excel file
                for (int j = 0; j < 24; j++)
                {
                    wsCosFiInductiv.Cells[$"A{j + 2}"].Style.Font.Bold = true;
                    wsCosFiInductiv.Cells[$"A{j + 2}"].Value = j + 1;
                    for (int i = 1; i <= report.ZileInLuna; i++)
                    {
                        wsCosFiInductiv.Cells[$"{GetCharStringsFromNumber(65 + i)}{j + 2}"].Value = report.TabeleValori[3].Valori[i, j];
                    }
                }
                // Show Values per each day of mounth
                wsCosFiInductiv.Cells[$"A{27}"].Value = " Total: ";
                for (int i = 1; i <= (report.ZileInLuna); i++)
                {
                    wsCosFiInductiv.Cells[$"{GetCharStringsFromNumber(65 + i)}27"].Value = report.TabeleValori[3].TotalperZi[i];
                }
                wsCosFiInductiv.Cells[$"A27:AG27"].Style.Font.Bold = true;
                wsCosFiInductiv.Cells[$"{GetCharStringsFromNumber(66 + report.ZileInLuna)}27"].Value = $"{report.TabeleValori[3].TotalperLuna}";

                wsCosFiInductiv.Cells["A:AZ"].AutoFitColumns();

                // Another SHEET REport
                // EXCEL WORKSHEET FOR Cos Fi Capacitiv Energii orare
                ExcelWorksheet wsCosFiCapacitiv = pck.Workbook.Worksheets.Add($"{report.An}.{report.Luna} Cos Fi capacitiv");
                // Set Head of table header Style
                wsCosFiCapacitiv.Cells["A1:AG1"].Style.Font.Bold = true;
                // Set Values of Table Header
                wsCosFiCapacitiv.Cells["A1"].Value = "Ora/ Zi";
                for (int i = 1; i <= report.ZileInLuna; i++)
                {
                    wsCosFiCapacitiv.Cells[$"{GetCharStringsFromNumber(65 + i)}1"].Value = i;
                }

                // Set Values from report in excel file
                for (int j = 0; j < 24; j++)
                {
                    wsCosFiCapacitiv.Cells[$"A{j + 2}"].Style.Font.Bold = true;
                    wsCosFiCapacitiv.Cells[$"A{j + 2}"].Value = j + 1;
                    for (int i = 1; i <= report.ZileInLuna; i++)
                    {
                        wsCosFiCapacitiv.Cells[$"{GetCharStringsFromNumber(65 + i)}{j + 2}"].Value = report.TabeleValori[4].Valori[i, j];
                    }
                }

                // Show Values per each day of mounth
                wsCosFiCapacitiv.Cells[$"A{27}"].Value = " Total: ";
                for (int i = 1; i <= (report.ZileInLuna); i++)
                {
                    wsCosFiCapacitiv.Cells[$"{GetCharStringsFromNumber(65 + i)}27"].Value = report.TabeleValori[4].TotalperZi[i];
                }
                wsCosFiCapacitiv.Cells[$"A27:AG27"].Style.Font.Bold = true;
                wsCosFiCapacitiv.Cells[$"{GetCharStringsFromNumber(66 + report.ZileInLuna)}27"].Value = $"{report.TabeleValori[4].TotalperLuna}";

                wsCosFiCapacitiv.Cells["A:AZ"].AutoFitColumns();

                // Another SHEET REport
                // EXCEL WORKSHEET FOR Ri+ Energii orare Facturare
                ExcelWorksheet wsEnergyEnergiiFacturare = pck.Workbook.Worksheets.Add($"{report.An}.{report.Luna} Ri+ energii orare fact");
                // Set Head of table header Style
                wsEnergyEnergiiFacturare.Cells["A1:AG1"].Style.Font.Bold = true;
                // Set Values of Table Header
                wsEnergyEnergiiFacturare.Cells["A1"].Value = "Ora/ Zi";
                for (int i = 1; i <= report.ZileInLuna; i++)
                {
                    wsEnergyEnergiiFacturare.Cells[$"{GetCharStringsFromNumber(65 + i)}1"].Value = i;
                }

                // Set Values from report in excel file
                for (int j = 0; j < 24; j++)
                {
                    wsEnergyEnergiiFacturare.Cells[$"A{j + 2}"].Style.Font.Bold = true;
                    wsEnergyEnergiiFacturare.Cells[$"A{j + 2}"].Value = j + 1;
                    for (int i = 1; i <= report.ZileInLuna; i++)
                    {
                        wsEnergyEnergiiFacturare.Cells[$"{GetCharStringsFromNumber(65 + i)}{j + 2}"].Value = report.TabeleValori[5].Valori[i, j];
                    }
                }

                // Show Values per each day of mounth
                wsEnergyEnergiiFacturare.Cells[$"A{27}"].Value = " Total: ";
                for (int i = 1; i <= (report.ZileInLuna); i++)
                {
                    wsEnergyEnergiiFacturare.Cells[$"{GetCharStringsFromNumber(65 + i)}27"].Value = report.TabeleValori[5].TotalperZi[i];
                }
                wsEnergyEnergiiFacturare.Cells[$"A27:AG27"].Style.Font.Bold = true;
                wsEnergyEnergiiFacturare.Cells[$"{GetCharStringsFromNumber(66 + report.ZileInLuna)}27"].Value = $"{report.TabeleValori[5].TotalperLuna} kVArh";

                wsEnergyEnergiiFacturare.Cells["A:AZ"].AutoFitColumns();

                // Final part Excel File
                pck.Save();
            }
            stream.Position = 0;
            string excelName = $"{report.An}.{report.Luna}_RaportEnergy.xlsx";

            return controler.File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);

        }

        public string GetCharStringsFromNumber(int nr)
        {
            if (nr < 91) return Convert.ToChar(nr).ToString();

            switch (nr)
            {
                case 91: return "AA";
                case 92: return "AB";
                case 93: return "AC";
                case 94: return "AD";
                case 95: return "AE";
                case 96: return "AF";
                case 97: return "AG";
                default: break;
            }
            return "";
        }
    }
}
