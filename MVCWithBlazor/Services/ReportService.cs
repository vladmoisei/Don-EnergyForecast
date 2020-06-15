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
        public  ReportMonthViewModel GetNewReporthValoriVieModelByDate(DateTime data)
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
                        var variabila = context.Indexes.Where(elem =>
                            elem.DataOra.Year == data.Year &&
                            elem.DataOra.Month == data.Month &&
                            elem.DataOra.Day == i &&
                            elem.DataOra.Hour == j
                        ).Select(x => new ElemSelectieModel
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
                    raport.TabeleValori[3].TotalperZi[i] += raport.TabeleValori[3].Valori[i, j];
                    raport.TabeleValori[4].TotalperZi[i] += raport.TabeleValori[4].Valori[i, j];
                    raport.TabeleValori[5].TotalperZi[i] += raport.TabeleValori[5].Valori[i, j];
                }

                // For cos Fi afisam media aritmetica
                raport.TabeleValori[3].TotalperZi[i] = Math.Round(raport.TabeleValori[3].TotalperZi[i] / 24, 2);
                raport.TabeleValori[4].TotalperZi[i] = Math.Round(raport.TabeleValori[4].TotalperZi[i] / 24, 2);

                raport.TabeleValori[0].TotalperLuna += raport.TabeleValori[0].TotalperZi[i];
                raport.TabeleValori[1].TotalperLuna += raport.TabeleValori[1].TotalperZi[i];
                raport.TabeleValori[2].TotalperLuna += raport.TabeleValori[2].TotalperZi[i];
                raport.TabeleValori[3].TotalperLuna += raport.TabeleValori[3].TotalperZi[i];
                raport.TabeleValori[4].TotalperLuna += raport.TabeleValori[4].TotalperZi[i];
                raport.TabeleValori[5].TotalperLuna += raport.TabeleValori[5].TotalperZi[i];
            }
            // For cos Fi afisam media aritmetica
            raport.TabeleValori[3].TotalperLuna = raport.TabeleValori[3].TotalperLuna / raport.ZileInLuna;
            raport.TabeleValori[4].TotalperLuna = raport.TabeleValori[4].TotalperLuna / raport.ZileInLuna;
            return raport;
        }

    }
}
