using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MVCWithBlazor.Data;
using MVCWithBlazor.Models;
using MVCWithBlazor.Services;

namespace MVCWithBlazor.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ReportDbContext _context;
        private readonly ReportService _reportService;

        public HomeController(ILogger<HomeController> logger, ReportDbContext context, ReportService reportService)
        {
            _logger = logger;
            _context = context;
            _reportService = reportService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Indexes()
        {
            // Show Indexes between fisrt and last of current Month
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

        [HttpGet]
        public IActionResult UploadDataFromFile()
        {
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
        public IActionResult UploadPrognozaFromFile(List<IFormFile> files)
        {
            // Verificam daca lista de fisiera incarcata  are 0 elemente si returnam msj
            if (files.Count == 0)
            {
                ViewBag.Mesaj2 = "Fisierul nu s-a incarcat";
                return View();
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
            // Redirection la Month Forecast
            return RedirectToAction("UploadDataFromFile", "Home"); // TO DO
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
