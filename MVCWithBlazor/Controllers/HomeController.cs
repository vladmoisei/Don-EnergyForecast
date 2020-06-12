﻿using System;
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
            var startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var endDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month + 1, 1);
            ViewBag.start = startDate;
            ViewBag.end = endDate;

            List<IndexModel> lista = _context.Indexes.Where(elem => elem.DataOra >= startDate && elem.DataOra <= endDate).ToList();
            return View(lista);
        }
        [HttpPost]
        public IActionResult Indexes(DateTime daterangepicker, string startDate, string endDate)
        {
            DateTime StartDate = Convert.ToDateTime(startDate.Substring(0, 15));
            DateTime EndDate = Convert.ToDateTime(endDate.Substring(0, 15));
            ViewBag.start = StartDate;
            ViewBag.end = EndDate;
            List<IndexModel> lista = _context.Indexes.Where(elem => elem.DataOra >= StartDate && elem.DataOra <= EndDate).ToList();
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

        public IActionResult ViewReportOnMotnh()
        {
            ViewBag.Data = DateTime.Now;
            return View();
        }
        [HttpPost]
        public IActionResult ViewReportOnMotnh(string datepicker)
        {
            DateTime data = Convert.ToDateTime(datepicker);
            ViewBag.Data = data;
            _reportService.GetViewModelForSelectedMonth(_context, data);
            return View();
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
