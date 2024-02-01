using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Globalization;
using System.Text;
using TarjetaCreditoWebApp.Models;

namespace TarjetaCreditoWebApp.Controllers
{
    public class HomeController : Controller
    {
        readonly Uri baseAddress = new("https://localhost:44348/api");
        private readonly HttpClient _httpClient;

        public HomeController()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = baseAddress
            };
        }

        [HttpGet]
        public IActionResult Index(string sortOrder, string currentFilter, string searchString, int? pageNumber, int? pageSize)
        {
            pageSize ??= 5;
            try
            {
                ViewData["CurrentSort"] = sortOrder;
                ViewData["NumTarjetaSort"] = sortOrder == "NumTarjeta" ? "NumTarjeta_desc" : "NumTarjeta";
                ViewData["NombreSort"] = sortOrder == "Nombres" ? "Nombres_desc" : "Nombres";
                ViewData["ApellidoSort"] = sortOrder == "Apellidos" ? "Apellidos_desc" : "Apellidos";

                if (searchString != null)
                {
                    pageNumber = 1;
                }
                else
                {
                    searchString = currentFilter;
                }

                ViewData["CurrentFilter"] = searchString;

                HttpResponseMessage response = _httpClient.GetAsync($"{_httpClient.BaseAddress}/TarjetaCredito/GetTarjetasCredito").Result;
                string data = response.Content.ReadAsStringAsync().Result;
                List<TarjetaCredito> tarjetaList = JsonConvert.DeserializeObject<List<TarjetaCredito>>(data);

                foreach (var item in tarjetaList)
                {
                    item.NumeroTarjetaMask = $"{item.NumeroTarjeta[..4]} **** **** {item.NumeroTarjeta[12..]}";
                }
                if (!String.IsNullOrEmpty(searchString))
                {
                    tarjetaList = tarjetaList.Where(s => s.Nombres.ToUpper().Contains(searchString.ToUpper()) || s.Apellidos.ToUpper().Contains(searchString.ToUpper())).ToList();
                }

                tarjetaList = sortOrder switch
                {
                    "NumTarjeta" => [.. tarjetaList.OrderBy(s => s.NumeroTarjeta)],
                    "NumTarjeta_desc" => [.. tarjetaList.OrderByDescending(s => s.NumeroTarjeta)],
                    "Nombres" => [.. tarjetaList.OrderBy(s => s.Nombres)],
                    "Nombres_desc" => [.. tarjetaList.OrderByDescending(s => s.Nombres)],
                    "Apellidos" => [.. tarjetaList.OrderBy(s => s.Apellidos)],
                    "Apellidos_desc" => [.. tarjetaList.OrderByDescending(s => s.Apellidos)],
                    _ => tarjetaList,
                };

                ViewData["PageSize"] = pageSize;
                return View(PaginatedList<TarjetaCredito>.Create(tarjetaList, pageNumber ?? 1, pageSize ?? 3));
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpGet]
        public IActionResult EstadoCuenta(int id)
        {
            HttpResponseMessage response = _httpClient.GetAsync($"{_httpClient.BaseAddress}/TarjetaCredito/GetTarjetaCredito?id=" + id).Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                List<TarjetaCredito> tarjetaList = JsonConvert.DeserializeObject<List<TarjetaCredito>>(data);

                tarjetaList[0].SaldoActual = 0;
                tarjetaList[0].ComprasPrevio = 0;
                tarjetaList[0].ComprasActual = 0;
                tarjetaList[0].NombreMesPrev = DateTime.Now.AddDays(-DateTime.Now.Day).ToString("MMMM", new CultureInfo("es-ES"));
                tarjetaList[0].NombreMesAct = DateTime.Now.ToString("MMMM", new CultureInfo("es-ES"));
                tarjetaList[0].NumeroTarjetaMask = $"{tarjetaList[0].NumeroTarjeta[..4]} **** **** {tarjetaList[0].NumeroTarjeta[12..]}";

                foreach (var item in tarjetaList[0].Transacciones)
                {
                    if (item.AbonoCargo == "C")
                    {
                        tarjetaList[0].SaldoActual += item.Monto;
                        if (item.Fecha.Year == DateTime.Now.Year && item.Fecha.Month == DateTime.Now.Month)
                        {
                            tarjetaList[0].ComprasActual += item.Monto;
                        }
                        else if (item.Fecha.Month == DateTime.Now.AddDays(-DateTime.Now.Day).Month && item.Fecha.Year == DateTime.Now.AddDays(-DateTime.Now.Day).Year)
                        {
                            tarjetaList[0].ComprasPrevio += item.Monto;
                        }
                    }
                    else
                    {
                        tarjetaList[0].SaldoActual -= item.Monto;
                    }
                    item.FechaText = item.Fecha.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    item.NumeroAutorizacion = item.Id.ToString().PadLeft(6, '0');
                    item.MontoText = item.Monto.ToString("C");
                }

                tarjetaList[0].SaldoDisponible = tarjetaList[0].Limite - tarjetaList[0].SaldoActual;
                tarjetaList[0].InteresBonificable = tarjetaList[0].ComprasActual * tarjetaList[0].PorcInteres / 100;
                tarjetaList[0].Transacciones =
                [
                    .. tarjetaList[0].Transacciones.Where(s =>
                                    s.AbonoCargo == "C" && s.Fecha.Year == DateTime.Now.Year && s.Fecha.Month == DateTime.Now.Month).OrderByDescending(s => s.Fecha),
                ];

                return View(tarjetaList);
            }

            return View();
        }

        [HttpGet]
        public IActionResult RegistraCompra(int id)
        {
            HttpResponseMessage response = _httpClient.GetAsync($"{_httpClient.BaseAddress}/TarjetaCredito/GetTarjetaCredito?id=" + id).Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                var tarjetaList = JsonConvert.DeserializeObject<List<TarjetaCredito>>(data);

                tarjetaList[0].Transacciones[0] = new Transacciones();
                tarjetaList[0].Transacciones[0].Fecha = DateTime.Parse(DateTime.Now.ToString(), CultureInfo.InvariantCulture);
                tarjetaList[0].NombreCompleto = $"{tarjetaList[0].Nombres} {tarjetaList[0].Apellidos}";
                tarjetaList[0].NumeroTarjetaMask = $"{tarjetaList[0].NumeroTarjeta[..4]} **** **** {tarjetaList[0].NumeroTarjeta[12..]}";

                return View(tarjetaList[0]);
            }

            return View();
        }

        [HttpPost]
        public IActionResult RegistraCompra(TarjetaCredito model)
        {
            HttpResponseMessage result = _httpClient.GetAsync($"{_httpClient.BaseAddress}/TarjetaCredito/GetTarjetaCredito?id=" + model.Id).Result;
            if (result.IsSuccessStatusCode)
            {
                string value = result.Content.ReadAsStringAsync().Result;
                var tarjeta = JsonConvert.DeserializeObject<List<TarjetaCredito>>(value)[0];

                model.NumeroTarjeta = tarjeta.NumeroTarjeta;
                model.Transacciones[0].AbonoCargo = "C";

                string data = JsonConvert.SerializeObject(model);
                StringContent content = new(data, Encoding.UTF8, "application/json");
                HttpResponseMessage response = _httpClient.PutAsync($"{_httpClient.BaseAddress}/Transacciones/InsertTransaccion", content).Result;

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
            }

            return View();
        }

        [HttpGet]
        public IActionResult RegistraPago(int id)
        {
            HttpResponseMessage response = _httpClient.GetAsync($"{_httpClient.BaseAddress}/TarjetaCredito/GetTarjetaCredito?id=" + id).Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                var tarjetaList = JsonConvert.DeserializeObject<List<TarjetaCredito>>(data);

                tarjetaList[0].Transacciones[0] = new Transacciones();
                tarjetaList[0].Transacciones[0].Fecha = DateTime.Parse(DateTime.Now.ToString(), CultureInfo.InvariantCulture);
                tarjetaList[0].NombreCompleto = $"{tarjetaList[0].Nombres} {tarjetaList[0].Apellidos}";
                tarjetaList[0].NumeroTarjetaMask = $"{tarjetaList[0].NumeroTarjeta[..4]} **** **** {tarjetaList[0].NumeroTarjeta[12..]}";

                return View(tarjetaList[0]);
            }

            return View();
        }

        [HttpPost]
        public IActionResult RegistraPago(TarjetaCredito model)
        {
            HttpResponseMessage result = _httpClient.GetAsync($"{_httpClient.BaseAddress}/TarjetaCredito/GetTarjetaCredito?id=" + model.Id).Result;
            if (result.IsSuccessStatusCode)
            {
                string value = result.Content.ReadAsStringAsync().Result;
                var tarjeta = JsonConvert.DeserializeObject<List<TarjetaCredito>>(value)[0];

                model.NumeroTarjeta = tarjeta.NumeroTarjeta;
                model.Transacciones[0].Descripcion = "Pago de TC";
                model.Transacciones[0].AbonoCargo = "A";

                string data = JsonConvert.SerializeObject(model);
                StringContent content = new(data, Encoding.UTF8, "application/json");
                HttpResponseMessage response = _httpClient.PutAsync($"{_httpClient.BaseAddress}/Transacciones/InsertTransaccion", content).Result;

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
            }

            return View();
        }

        [HttpGet]
        public IActionResult Transacciones(int id)
        {
            HttpResponseMessage response = _httpClient.GetAsync($"{_httpClient.BaseAddress}/TarjetaCredito/GetTarjetaCredito?id=" + id).Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                List<TarjetaCredito> tarjetaList = JsonConvert.DeserializeObject<List<TarjetaCredito>>(data);
                tarjetaList[0].NumeroTarjetaMask = $"{tarjetaList[0].NumeroTarjeta[..4]} **** **** {tarjetaList[0].NumeroTarjeta[12..]}";

                foreach (var item in tarjetaList[0].Transacciones)
                {
                    if (item.AbonoCargo == "C")
                    {
                        item.MontoCargo = item.Monto.ToString("C");
                    }
                    else
                    {
                        item.MontoAbono = item.Monto.ToString("C");
                    }
                    item.FechaText = item.Fecha.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    item.NumeroAutorizacion = item.Id.ToString().PadLeft(6, '0');
                }

                tarjetaList[0].SaldoDisponible = tarjetaList[0].Limite - tarjetaList[0].SaldoActual;
                tarjetaList[0].InteresBonificable = tarjetaList[0].ComprasActual * tarjetaList[0].PorcInteres / 100;
                tarjetaList[0].Transacciones =
                [
                    .. tarjetaList[0].Transacciones.Where(s =>
                                    s.Fecha.Year == DateTime.Now.Year && s.Fecha.Month == DateTime.Now.Month).OrderByDescending(s => s.Fecha),
                ];

                return View(tarjetaList);
            }

            return View();
        }

        [HttpGet]
        public IActionResult GenerarExcel(int id)
        {
            try
            {
                HttpResponseMessage response = _httpClient.GetAsync($"{_httpClient.BaseAddress}/TarjetaCredito/GetTarjetaCredito?id=" + id).Result;

                string data = response.Content.ReadAsStringAsync().Result;
                List<TarjetaCredito> tarjetaList = JsonConvert.DeserializeObject<List<TarjetaCredito>>(data);

                tarjetaList[0].SaldoActual = 0;
                tarjetaList[0].ComprasPrevio = 0;
                tarjetaList[0].ComprasActual = 0;
                tarjetaList[0].NombreMesPrev = DateTime.Now.AddDays(-DateTime.Now.Day).ToString("MMMM", new CultureInfo("es-ES"));
                tarjetaList[0].NombreMesAct = DateTime.Now.ToString("MMMM", new CultureInfo("es-ES"));
                tarjetaList[0].NumeroTarjetaMask = $"{tarjetaList[0].NumeroTarjeta[..4]} **** **** {tarjetaList[0].NumeroTarjeta[12..]}";

                foreach (var item in tarjetaList[0].Transacciones)
                {
                    if (item.AbonoCargo == "C")
                    {
                        tarjetaList[0].SaldoActual += item.Monto;
                        if (item.Fecha.Year == DateTime.Now.Year && item.Fecha.Month == DateTime.Now.Month)
                        {
                            tarjetaList[0].ComprasActual += item.Monto;
                        }
                        else if (item.Fecha.Month == DateTime.Now.AddDays(-DateTime.Now.Day).Month && item.Fecha.Year == DateTime.Now.AddDays(-DateTime.Now.Day).Year)
                        {
                            tarjetaList[0].ComprasPrevio += item.Monto;
                        }
                    }
                    else
                    {
                        tarjetaList[0].SaldoActual -= item.Monto;
                    }
                    item.FechaText = item.Fecha.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    item.NumeroAutorizacion = item.Id.ToString().PadLeft(6, '0');
                    item.MontoText = item.Monto.ToString("C");
                }

                tarjetaList[0].SaldoDisponible = tarjetaList[0].Limite - tarjetaList[0].SaldoActual;
                tarjetaList[0].InteresBonificable = tarjetaList[0].ComprasActual * tarjetaList[0].PorcInteres / 100;
                tarjetaList[0].Transacciones =
                [
                    .. tarjetaList[0].Transacciones.Where(s =>
                                    s.AbonoCargo == "C" && s.Fecha.Year == DateTime.Now.Year && s.Fecha.Month == DateTime.Now.Month).OrderByDescending(s => s.Fecha),
                ];

                var numrow = 1;


                using var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Compras");
                worksheet.Cell(numrow, 1).Value = "Número de Autorización";
                worksheet.Cell(numrow, 2).Value = "Fecha";
                worksheet.Cell(numrow, 3).Value = "Descripción";
                worksheet.Cell(numrow, 4).Value = "Monto";

                foreach (var item in tarjetaList[0].Transacciones)
                {
                    numrow++;
                    worksheet.Cell(numrow, 1).Value = item.NumeroAutorizacion;
                    worksheet.Cell(numrow, 2).Value = item.Fecha.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    worksheet.Cell(numrow, 3).Value = item.Descripcion;
                    worksheet.Cell(numrow, 4).Value = item.Monto.ToString("C");
                }

                worksheet.Columns().AdjustToContents();

                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                var content = stream.ToArray();

                return File(content, "aplication/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Compras.xlsx");
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}

