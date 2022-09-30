using ClosedXML.Excel;
using EFlowerShopAdminApplication.Models;
using GemBox.Document;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace EFlowerShopAdminApplication.Controllers
{
    public class OrderController : Controller
    {
        public OrderController()
        {
            ComponentInfo.SetLicense("FREE-LIMITED-KEY");
        }
        public IActionResult Index()
        {
            HttpClient client = new HttpClient();

            string URL = "https://localhost:44324/api/Admin/GetOrders";

            HttpResponseMessage response = client.GetAsync(URL).Result;

            var result = response.Content.ReadAsAsync<List<Order>>().Result;

            return View(result);
        }

        public IActionResult Details(Guid id)
        {
            HttpClient client = new HttpClient();

            string URL = "https://localhost:44324/api/Admin/GetDetailsForOrder";

            var model = new
            {
                Id = id
            };


            HttpContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");


            HttpResponseMessage response = client.PostAsync(URL, content).Result;

            var result = response.Content.ReadAsAsync<Order>().Result;

            return View(result);
        }

        public FileContentResult CreateInvoice(Guid id)
        {
            HttpClient client = new HttpClient();

            string URL = "https://localhost:44324/api/Admin/GetDetailsForOrder";

            var model = new
            {
                Id = id
            };

            HttpContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

            HttpResponseMessage response = client.PostAsync(URL, content).Result;

            var result = response.Content.ReadAsAsync<Order>().Result;

            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Invoice.docx");

            var document = DocumentModel.Load(templatePath);

            document.Content.Replace("{{OrderNumber}}", result.Id.ToString());
            document.Content.Replace("{{CustomerEmail}}", result.User.Email);

            StringBuilder sb = new StringBuilder();

            var total = 0.0;

            foreach (var item in result.FlowerInOrders)
            {
                total += item.Quantity * item.Flower.FlowerPrice;
                sb.AppendLine(item.Flower.FlowerName + " with quantity of: " + item.Quantity + " and price of: $" + item.Flower.FlowerPrice);
            }

            document.Content.Replace("{{AllFlowers}}", sb.ToString());
            document.Content.Replace("{{TotalPrice}}", "$" + total.ToString());

            var stream = new MemoryStream();

            document.Save(stream, new PdfSaveOptions());


            return File(stream.ToArray(), new PdfSaveOptions().ContentType, "ExportInvoice.pdf");
        }

        [HttpGet]
        public FileContentResult ExportAllOrders()
        {
            HttpClient client = new HttpClient();

            string URL = "https://localhost:44324/api/Admin/GetOrders";

            HttpResponseMessage response = client.GetAsync(URL).Result;

            var result = response.Content.ReadAsAsync<List<Order>>().Result;

            string fileName = "Orders.xlsx";
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            using (var workBook = new XLWorkbook())
            {
                IXLWorksheet worksheet = workBook.Worksheets.Add("All Orders");

                worksheet.Cell(1, 1).Value = "Order Id";
                worksheet.Cell(1, 2).Value = "First Name";
                worksheet.Cell(1, 3).Value = "Last Name";
                worksheet.Cell(1, 4).Value = "Email";

                for (int i = 1; i <= result.Count(); i++)
                {
                    var item = result[i - 1];

                    worksheet.Cell(i + 1, 1).Value = item.Id.ToString();
                    worksheet.Cell(i + 1, 2).Value = item.User.FirstName;
                    worksheet.Cell(i + 1, 3).Value = item.User.LastName;
                    worksheet.Cell(i + 1, 4).Value = item.User.Email;

                    for (int f = 1; f <= item.FlowerInOrders.Count(); f++)
                    {
                        worksheet.Cell(1, f + 4).Value = "Flower - " + (f);
                        worksheet.Cell(i + 1, f + 4).Value = item.FlowerInOrders.ElementAt(f - 1).Flower.FlowerName;
                    }

                }

                using (var stream = new MemoryStream())
                {
                    workBook.SaveAs(stream);

                    var content = stream.ToArray();

                    return File(content, contentType, fileName);
                }
            }

        }

    }
}
