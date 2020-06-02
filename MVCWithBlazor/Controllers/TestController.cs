using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MVCWithBlazor.Data;
using MVCWithBlazor.Models;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using MVCWithBlazor.Services;
using System.IO;
using OfficeOpenXml;

namespace MVCWithBlazor.Controllers
{
    public class TestController : Controller
    {
        private readonly ReportDbContext _context;
        private readonly ILogger<TestController> _logger;
        private readonly Auxiliar _auxiliar;

        public TestController(ReportDbContext context, ILogger<TestController> logger, Auxiliar auxiliar)
        {
            _context = context;
            _logger = logger;
            _auxiliar = auxiliar;
            // log
            _logger.LogInformation("{data}<=>{Messege}", DateTime.Now.ToString("dd.MM.yyyy hh:mm:ss"), "TESTCONTROLLER constructor log information");
            _logger.LogError("{data}<=>{Messege}", DateTime.Now.ToString("dd.MM.yyyy hh:mm:ss"), "TESTCONTROLLER constructor log error");
            _logger.LogWarning("{data}<=>{Messege}", DateTime.Now.ToString("dd.MM.yyyy hh:mm:ss"), "TESTCONTROLLER constructor log warning");
        }
        public IActionResult Index()
        {
            return View();
        }

        // Get call from RedirectToActionWithParameters
        // Also was transferd List Of Complex Object by TempData in Json Format
        public IActionResult IndexPlc(string nume, int numar)
        {
            ViewData["Nume primit"] = nume;
            ViewData["Numar primit"] = numar;
            List<PlcModel> plcuri;
            try
            {
                // Receive list of complex objects by TempData
                plcuri = JsonSerializer.Deserialize<List<PlcModel>>((string)(TempData["list"]));
            }
            catch (ArgumentNullException)
            {
                plcuri = _context.Plcs.ToList();
            }
            
            return View(plcuri);
        }

        // Partial View Return
        // POST: ProbaModels 
        // Afisam doar modelele intre anumite date selectate
        [HttpPost]
        public async Task<IActionResult> _IndexPlc(string dataFrom, string dataTo)
        {
            // Modific datele de afisat in functie de selectie din View
            if (dataFrom == null) dataFrom = _auxiliar.GetOneMonthBeforeDate();
            if (dataTo == null) dataTo = _auxiliar.GetTomorrowDate();
            IEnumerable<PlcModel> ListaDeAfisat = await _context.Plcs.ToListAsync();

            return PartialView(ListaDeAfisat.Where(item => _auxiliar.IsDateBetween(item.DataCreation.ToString("dd.MM.yyyy HH:mm:ss"), dataFrom, dataTo)));
            // return View(await _context.ProbaModels.ToListAsync());
        }

        public IActionResult IndexTag()
        {
            // Include => preaia obiect din DbContext si il atribui in acest model
            return View(_context.Tags.Include(t => t.PlcModel).ToList());
        }

        // Populate Plcs DbContext and redirect to action with parameter
        public async Task<IActionResult> PopulatePlcDb()
        {
            if (_context.Plcs.ToList() == null)
                _context.Plcs.AddRange(
                    new PlcModel { Name = "Plc 1", Ip = "192.168.0.1", IsEnable = true, DataCreation = DateTime.Now, PingRequestsFail = 3, Rack = 1, Slot = 1 },
                    new PlcModel { Name = "Plc 2", Ip = "192.168.0.2", IsEnable = true, DataCreation = DateTime.Now, PingRequestsFail = 3, Rack = 1, Slot = 1 },
                    new PlcModel { Name = "Plc 3", Ip = "192.168.0.3", IsEnable = true, DataCreation = DateTime.Now, PingRequestsFail = 3, Rack = 1, Slot = 1 },
                    new PlcModel { Name = "Plc 4", Ip = "192.168.0.4", IsEnable = true, DataCreation = DateTime.Now, PingRequestsFail = 3, Rack = 1, Slot = 1 },
                    new PlcModel { Name = "Plc 5", Ip = "192.168.0.5", IsEnable = true, DataCreation = DateTime.Now, PingRequestsFail = 3, Rack = 1, Slot = 1 },
                    new PlcModel { Name = "Plc 6", Ip = "192.168.0.6", IsEnable = true, DataCreation = DateTime.Now, PingRequestsFail = 3, Rack = 1, Slot = 1 }
                    );
            await _context.SaveChangesAsync();
            List<PlcModel> plcs = await _context.Plcs.ToListAsync();
            // Redirect to Action with a anonimous parameter
            // nu se poate transmite un obiect complex in redirectto action, deoarece urlul ar fi prea lung, poti trimite doar variablie basic: int, string ...
            // transmitem obiect complex prin: Temp data
            TempData["list"] = JsonSerializer.Serialize(plcs);
            return RedirectToAction("IndexPlc", "Test", new
            {
                nume = "Nume din populate PLC DB",
                numar = 3333
            });
        }

        // GET: Plcs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Plcs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PlcModelID,Name,DataCreation,IsEnable,Ip,Rack,Slot")] PlcModel plcModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(plcModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(plcModel);
        }

        // Populate Tags DbContext and redirect to action
        public async Task<IActionResult> PopulateTagDb()
        {
            if (_context.Tags.ToList() == null)
                _context.Tags.AddRange(
                new TagModel { Name = "tag1", Adress = "M0.0", PlcModelID = 1, Value = "0" },
                new TagModel { Name = "tag2", Adress = "M0.0", PlcModelID = 2, Value = "0" },
                new TagModel { Name = "tag3", Adress = "M0.0", PlcModelID = 3, Value = "0" },
                new TagModel { Name = "tag4", Adress = "M0.0", PlcModelID = 4, Value = "0" },
                new TagModel { Name = "tag5", Adress = "M0.0", PlcModelID = 5, Value = "0" },
                new TagModel { Name = "tag6", Adress = "M0.0", PlcModelID = 6, Value = "0" },
                new TagModel { Name = "tag7", Adress = "M0.7", PlcModelID = 1, Value = "0" }
                );
            await _context.SaveChangesAsync();
            // Redirect to Action without parameter
            return RedirectToAction("IndexTag", "Test");
        }

        // Function called by click from forms submit with AntiForgeryToken
        // Delete Range of elements from SQL, started with an ID > id given
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StergereElementeById(int numberId)
        {
            //return Content(numberId.ToString());
            // Stergere range elemente mai mari decat Id-ul introdus
            _context.Tags.RemoveRange(_context.Tags.Where(item => item.TagID > numberId));
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Test");
        }

        // Function export data to excel file
        [Route("/Test/ExportToExcelAsync")]
        public async Task<IActionResult> ExportToExcelAsync(string dataFrom, string dataTo)
        {
            //return Content(dataFrom + "<==>" + dataTo);
            List<PlcModel> listaSql = await _context.Plcs.ToListAsync();
            // Extrage datele cuprinse intre limitele date de operator
            IEnumerable<PlcModel> listaDeAfisat = listaSql.Where(model => _auxiliar.IsDateBetween(model.DataCreation.ToString("dd.MM.yyyy HH:mm:ss"), dataFrom, dataTo));

            var stream = new MemoryStream();

            using (var pck = new ExcelPackage(stream))
            {
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("ModelPLC");
                ws.Cells["A1:Z1"].Style.Font.Bold = true;

                ws.Cells["A1"].Value = "PlcModelID";
                ws.Cells["B1"].Value = "Name";
                ws.Cells["C1"].Value = "DataCreation";
                ws.Cells["D1"].Value = "IsEnable";
                ws.Cells["E1"].Value = "Ip";
                ws.Cells["F1"].Value = "Rack";
                ws.Cells["G1"].Value = "Slot";

                int rowStart = 2;
                foreach (var elem in listaDeAfisat)
                {
                    ws.Cells[string.Format("A{0}", rowStart)].Value = elem.PlcModelID;
                    ws.Cells[string.Format("B{0}", rowStart)].Value = elem.Name;
                    ws.Cells[string.Format("C{0}", rowStart)].Value = elem.DataCreation;
                    ws.Cells[string.Format("D{0}", rowStart)].Value = elem.IsEnable;
                    ws.Cells[string.Format("E{0}", rowStart)].Value = elem.Ip;
                    ws.Cells[string.Format("F{0}", rowStart)].Value = elem.Rack;
                    ws.Cells[string.Format("G{0}", rowStart)].Value = elem.Slot;
                    rowStart++;
                }

                ws.Cells["A:AZ"].AutoFitColumns();

                pck.Save();
            }
            stream.Position = 0;
            string excelName = "RaportPLC.xlsx";
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        }

        // Action import data from excel file
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ImportFile(List<IFormFile> files)
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

            //Cream lista de blumuri din fisier excel
            List<PlcModel> lista = await _auxiliar.GetBlumsListFromExcelFileBySarjaAsync(formFile);

            // Actualizam baza de date cu lista de blumuri din fisier
            if (lista != null)
            {
                foreach (var item in lista)
                {
                    _context.Add(item);
                    _context.SaveChanges();
                }

            }

            // Redirection la Index
            return RedirectToAction("Index", "Test", new
            {
                nume = "Nume din import File",
                numar = 1000
            });
        }

    }
}