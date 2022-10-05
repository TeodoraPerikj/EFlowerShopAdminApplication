using EFlowerShopAdminApplication.Models;
using ExcelDataReader;
using Microsoft.AspNetCore.Http;
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
    public class UserController : Controller
    {
        [HttpGet("[action]")]
        public IActionResult ImportUsers()
        {
            return View();
        }

        [HttpPost("[action]")]
        public IActionResult ImportUsers(IFormFile file)
        {
            string pathToUpload = $"{Directory.GetCurrentDirectory()}\\files\\{file.FileName}";

            using (FileStream fileStream = System.IO.File.Create(pathToUpload))
            {
                file.CopyTo(fileStream);

                fileStream.Flush();
            }

            List<User> users = GetUsersFromExcelFile(file.FileName);

            HttpClient client = new HttpClient();

            string URL = "https://localhost:44324/api/Admin/ImportAllUsers";

            HttpContent content = new StringContent(JsonConvert.SerializeObject(users), Encoding.UTF8, "application/json");

            HttpResponseMessage response = client.PostAsync(URL, content).Result;

            var result = response.Content.ReadAsAsync<bool>().Result;

            return RedirectToAction("Index", "Home");
        }

        private List<User> GetUsersFromExcelFile(string fileName)
        {

            string pathToFile = $"{Directory.GetCurrentDirectory()}\\files\\{fileName}";

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            List<User> userList = new List<User>();

            using (var stream = System.IO.File.Open(pathToFile, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    while (reader.Read())
                    {
                        userList.Add(new Models.User
                        {
                            FirstName = reader.GetValue(0).ToString(),
                            LastName = reader.GetValue(1).ToString(),
                            Email = reader.GetValue(2).ToString(),
                            Password = reader.GetValue(3).ToString(),
                            ConfirmPassword = reader.GetValue(4).ToString()
                        });
                    }
                }
            }

            return userList;
        }
        public IActionResult ManageUsers()
        {
            HttpClient client = new HttpClient();

            string URL = "https://localhost:44324/api/Admin/GetAllUsers";

            HttpResponseMessage response = client.GetAsync(URL).Result;

            var users = response.Content.ReadAsAsync<List<EFlowerShopApplicationUser>>().Result;

            return View(users);
        }

        public IActionResult ChangeRole(string id)
        {
            HttpClient client = new HttpClient();

            string URL = "https://localhost:44324/api/Admin/GetDetailsForUser?";

            string email = id;

            string param = $"email={email}";

            HttpContent content = new StringContent(JsonConvert.SerializeObject(param), Encoding.UTF8, "application/json");

            HttpResponseMessage response = client.PostAsync(URL + param, content).Result;

            var data = response.Content.ReadAsAsync<EFlowerShopApplicationUser>().Result;

            return View(data);
        }

        [HttpPost]
        public IActionResult Edit(string email, EFlowerShopApplicationUser user)
        {

            HttpClient client = new HttpClient();

            string URL = "https://localhost:44324/api/Admin/Edit?";

            string param = $"email={email}&role={user.Role}";

            HttpContent content = new StringContent(JsonConvert.SerializeObject(param), Encoding.UTF8, "application/json");

            HttpResponseMessage response = client.PostAsync(URL + param, content).Result;

            var data = response.Content.ReadAsAsync<bool>().Result;

            if (data)
            {
                return RedirectToAction("ManageUsers", "User");
            }
            return RedirectToAction("ManageUsers", "User");
        }

    }
}
