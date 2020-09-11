using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MVCWithBlazor.Data;
using MVCWithBlazor.Models;
using MVCWithBlazor.Services;

namespace MVCWithBlazor.Controllers
{
    [Authorize(Roles = "Admin")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ReportDbContext _context;
        private readonly ReportService _reportService;
        private readonly IWebHostEnvironment _env;
        private readonly IEmailSender _emailSender;

        public HomeController(ILogger<HomeController> logger, ReportDbContext context, ReportService reportService, IWebHostEnvironment environment, IEmailSender emailSender)
        {
            _logger = logger;
            _context = context;
            _reportService = reportService;
            _env = environment;
            _emailSender = emailSender;
        }

        public IActionResult MailService()
        {
            string filePath = Path.Combine(_env.WebRootPath, "Fisiere\\MailDate.JSON");
            MailModel mailModel = _reportService.GetMailModelAsync(filePath).Result;
            return View(mailModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MailService([Bind("FromAdress,ToAddress,Subjsect,Messaege,FilePathFisierDeTrimis")] MailModel mailModel)
        {
            string filePath = _reportService.WriteToJsonMailData(mailModel, _env);
            //MailModel mailModel1 = _reportService.GetMailModelAsync(filePath).Result;
            return View(mailModel);
        }

        [HttpGet]
        public IActionResult Index()
        {
            DateTime data = DateTime.Now;
            ViewBag.start = data.ToString("yyyy-MM-dd");

            DailyViewModel dvm = new DailyViewModel
            {
                ListaConsumPerZi = _context.Indexes.Where(elem => elem.DataOra.Year == data.Year
                                    && elem.DataOra.Month == data.Month 
                                    && elem.DataOra.Day == data.Day).ToList(),
                ListaPrognozaPerZi = _context.PrognozaEnergieModels.Where(elem => elem.DataOra.Year == data.Year
                                    && elem.DataOra.Month == data.Month
                                    && elem.DataOra.Day == data.Day).ToList()
            };

            List<AxisLabelData> chartData = new List<AxisLabelData>();

            for (int i = 0; i < dvm.ListaPrognozaPerZi.Count; i++)
            {
                chartData.Add(new AxisLabelData { x = dvm.ListaPrognozaPerZi[i].Ora, y = dvm.ListaPrognozaPerZi[i].Valoare, y1 = dvm.ListaConsumPerZi.Count > 1 ? Math.Round(dvm.ListaConsumPerZi[i].ValueEnergyPlusA / 1000, 1): 0});
            }
            dvm.ChartData = chartData;
            ViewBag.dataSource = chartData;
            return View(dvm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(DateTime startDate, string submitBtn, double val1, double val2, double val3, double val4, double val5, double val6, double val7, double val8, double val9, double val10, double val11, double val12, double val13, double val14, double val15, double val16, double val17, double val18, double val19, double val20, double val21, double val22, double val23, double val24)
        {
            // Show Data For previous Day
            if (submitBtn == "Previous")
            {
                startDate = startDate.AddDays(-1);
            } 
            else if (submitBtn == "Next") // Show Data For next  Day
            {
                startDate = startDate.AddDays(1);
            }
            ViewBag.start = startDate.ToString("yyyy-MM-dd");
            DailyViewModel dvm = new DailyViewModel
            {
                ListaConsumPerZi = _context.Indexes.Where(elem => elem.DataOra.Year == startDate.Year
                                    && elem.DataOra.Month == startDate.Month
                                    && elem.DataOra.Day == startDate.Day).ToList(),
                ListaPrognozaPerZi = _context.PrognozaEnergieModels.Where(elem => elem.DataOra.Year == startDate.Year
                                    && elem.DataOra.Month == startDate.Month
                                    && elem.DataOra.Day == startDate.Day).ToList()
            };

            List<AxisLabelData> chartData = new List<AxisLabelData>();

            for (int i = 0; i < dvm.ListaPrognozaPerZi.Count; i++)
            {
                chartData.Add(new AxisLabelData { x = dvm.ListaPrognozaPerZi[i].Ora, y = dvm.ListaPrognozaPerZi[i].Valoare, y1 = dvm.ListaConsumPerZi.Count > 1 ? Math.Round(dvm.ListaConsumPerZi[i].ValueEnergyPlusA / 1000, 1) : 0 });
                
                // Update Elements in Database
                if (submitBtn == "Save data")
                {
                    switch (i)
                    {
                        case 0: dvm.ListaPrognozaPerZi[i].Valoare = val1;
                            break;
                        case 1:
                            dvm.ListaPrognozaPerZi[i].Valoare = val2;
                            break;
                        case 2:
                            dvm.ListaPrognozaPerZi[i].Valoare = val3;
                            break;
                        case 3:
                            dvm.ListaPrognozaPerZi[i].Valoare = val4;
                            break;
                        case 4:
                            dvm.ListaPrognozaPerZi[i].Valoare = val5;
                            break;
                        case 5:
                            dvm.ListaPrognozaPerZi[i].Valoare = val6;
                            break;
                        case 6:
                            dvm.ListaPrognozaPerZi[i].Valoare = val7;
                            break;
                        case 7:
                            dvm.ListaPrognozaPerZi[i].Valoare = val8;
                            break;
                        case 8:
                            dvm.ListaPrognozaPerZi[i].Valoare = val9;
                            break;
                        case 9:
                            dvm.ListaPrognozaPerZi[i].Valoare = val10;
                            break;
                        case 10:
                            dvm.ListaPrognozaPerZi[i].Valoare = val11;
                            break;
                        case 11:
                            dvm.ListaPrognozaPerZi[i].Valoare = val12;
                            break;
                        case 12:
                            dvm.ListaPrognozaPerZi[i].Valoare = val13;
                            break;
                        case 13:
                            dvm.ListaPrognozaPerZi[i].Valoare = val14;
                            break;
                        case 14:
                            dvm.ListaPrognozaPerZi[i].Valoare = val15;
                            break;
                        case 15:
                            dvm.ListaPrognozaPerZi[i].Valoare = val16;
                            break;
                        case 16:
                            dvm.ListaPrognozaPerZi[i].Valoare = val17;
                            break;
                        case 17:
                            dvm.ListaPrognozaPerZi[i].Valoare = val18;
                            break;
                        case 18:
                            dvm.ListaPrognozaPerZi[i].Valoare = val19;
                            break;
                        case 19:
                            dvm.ListaPrognozaPerZi[i].Valoare = val20;
                            break;
                        case 20:
                            dvm.ListaPrognozaPerZi[i].Valoare = val21;
                            break;
                        case 21:
                            dvm.ListaPrognozaPerZi[i].Valoare = val22;
                            break;
                        case 22:
                            dvm.ListaPrognozaPerZi[i].Valoare = val23;
                            break;
                        case 23:
                            dvm.ListaPrognozaPerZi[i].Valoare = val24;
                            break;
                        default:
                            break;
                    }
                    _context.PrognozaEnergieModels.Update(dvm.ListaPrognozaPerZi[i]);
                }
            }
            // Save to Database
            _context.SaveChanges();

            // Add ChartData
            dvm.ChartData = chartData;
            ViewBag.dataSource = chartData;

            // Save to Excel File
            if (submitBtn == "Get File")
            {
                var filePath = _reportService.SaveExcelFileToDisk(dvm, _env, startDate).ToString();
                byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Prognoza zilnic.xlsx");
            }

            // Send Mail With File Per one day Forecast
            if (submitBtn == "Send Mail")
            {
                var fisierDeTrimis = _reportService.SaveExcelFileToDisk(dvm, _env, startDate).ToString();
                string filePathMailModel = Path.Combine(_env.WebRootPath, "Fisiere\\MailDate.JSON");
                MailModel mailModel = _reportService.GetMailModelAsync(filePathMailModel).Result;
                _emailSender.SendEmailAsync(mailModel.FromAdress, mailModel.ToAddress, mailModel.Subjsect, mailModel.Messaege, fisierDeTrimis);
            }

            return View(dvm);
        }

        // Index Consumtion
        public IActionResult Indexes()
        {
            // Show Indexes between first and last of current Month
            ViewBag.Data = DateTime.Now;
            List<IndexModel> lista = _context.Indexes.Where(elem => elem.DataOra.Month == DateTime.Now.Month).ToList();
            return View(lista);
        }
        [HttpPost]
        public IActionResult Indexes(string datepicker)
        {
            DateTime data = Convert.ToDateTime(datepicker);
            ViewBag.Data = data;
            List<IndexModel> lista = _context.Indexes.Where(elem => elem.DataOra.Month == data.Month).ToList();
            return View(lista);
        }

        // Index Forecast
        public IActionResult IndexePrognoza()
        {
            // Show Indexes between fisrt and last of current Month
            DateTime data = DateTime.Now;
            ViewBag.Data = data;
            int zileInLuna = DateTime.DaysInMonth(data.Year, data.Month);
            ViewBag.ZileInLuna = zileInLuna;
            List<PrognozaEnergieModel> lista = _context.PrognozaEnergieModels.Where(elem => elem.DataOra.Month == DateTime.Now.Month).ToList();
            ReportMonthValoriViewModel reportMonthValoriViewModel = _reportService.GetMonthValoriPrognozaFromList(lista, zileInLuna);
            return View(reportMonthValoriViewModel);
        }
        [HttpPost]
        public IActionResult IndexePrognoza(string datepicker)
        {
            DateTime data = Convert.ToDateTime(datepicker);
            ViewBag.Data = data;
            int zileInLuna = DateTime.DaysInMonth(data.Year, data.Month);
            ViewBag.ZileInLuna = zileInLuna;
            List<PrognozaEnergieModel> lista = _context.PrognozaEnergieModels.Where(elem => elem.DataOra.Month == data.Month).ToList();
            ReportMonthValoriViewModel reportMonthValoriViewModel = _reportService.GetMonthValoriPrognozaFromList(lista, zileInLuna);
            return View(reportMonthValoriViewModel);
        }

        [HttpGet]
        public IActionResult UploadDataFromFile()
        {
            ViewBag.Data = DateTime.Now;
            return View();
        }

        // Action import data from excel file
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadDataFromFile(List<IFormFile> files)
        {
            // Verificam daca lista de fisiera incarcata  are 0 elemente si returnam msj
            if (files.Count == 0)
            {
                ViewBag.Hidden = "";
                ViewBag.Mesaj = "Fisierul nu s-a incarcat";
                return View();
            }

            // Cream fisier din primul lelement din lista de fisiere
            IFormFile formFile = files[0];
            // Verificam daca fisierul are extensia .xlsx
            if (!formFile.FileName.EndsWith(".xlsx"))
            {
                ViewBag.Hidden = "";
                ViewBag.Mesaj = "Fisierul nu are extensia .xlsx!";
                return View();
            }

            //Cream lista de Index din fisier excel
            List<IndexModel> lista = await _reportService.GetBlumsListFromExcelFileBySarjaAsync(formFile);

            //Actualizam baza de date cu lista de blumuri din fisier
            if (lista != null)
            {
                foreach (var item in lista)
                {
                    _context.Add(item);
                    _context.SaveChanges();
                }
            }

            // Redirection la Index
            return RedirectToAction("Indexes", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadPrognozaFromFile(List<IFormFile> files, DateTime datepicker)
        {
            DateTime data = Convert.ToDateTime(datepicker);
            ViewBag.Data = data;
            // Verificam daca lista de fisiera incarcata  are 0 elemente si returnam msj
            if (files.Count == 0)
            {
                ViewBag.Mesaj2 = "Fisierul nu s-a incarcat";
                return RedirectToAction("UploadDataFromFile", "Home");
            }

            // Cream fisier din primul lelement din lista de fisiere
            IFormFile formFile = files[0];
            // Verificam daca fisierul are extensia .xlsx
            if (!formFile.FileName.EndsWith(".xlsx"))
            {
                    ViewBag.Hidden = "";
                    ViewBag.Mesaj2 = "Fisierul nu are extensia .xlsx!";
                    return RedirectToAction("UploadDataFromFile", "Home");

            }

            //Cream lista cu prognoza din fisier excel
            List<PrognozaEnergieModel> lista = await _reportService.GetPrognozaForMonthFromFileAsync(formFile, data);

            //Actualizam baza de date cu lista de blumuri din fisier
            if (lista != null)
            {
                foreach (var item in lista)
                {
                    _context.Add(item);
                    _context.SaveChanges();
                }
            }
            // Redirection la Month Forecast
            return RedirectToAction("IndexePrognoza", "Home"); // TO DO
        }
        public IActionResult ViewReportOnMotnh()
        {
            ViewBag.Data = DateTime.Now;
            ReportMonthViewModel raport = _reportService.GetViewModelForSelectedMonth(_context, DateTime.Now);
            return View(raport);
        }
        [HttpPost]
        public IActionResult ViewReportOnMotnh(string datepicker, string submitButon)
        {
            DateTime data = Convert.ToDateTime(datepicker);
            ViewBag.Data = data;
            ReportMonthViewModel raport = _reportService.GetViewModelForSelectedMonth(_context, data);
            if (submitButon == "Show data") // If it is click Show Data Button Return View with data
                return View(raport);

            if (submitButon == "Export To Excel") // Else if it is clicked export to excel return an excel file report 
            {
                List<IndexModel> listaIndex = _context.Indexes.Where(elem => elem.DataOra.Month == data.Month && elem.DataOra.Year == data.Year).ToList();
                return _reportService.GetExcelFileFromReport(listaIndex, raport);
            }
            return View(raport);

        }

        [Authorize(Roles = "Member, Admin")]
        [Authorize(Policy = "Dep")]
        public IActionResult Member()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Admin()
        {
            return View();
        }
        [Authorize]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
