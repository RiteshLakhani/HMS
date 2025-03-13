using Hospital_Management_System.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace HospitalWeb.Controllers
{
    public class AuthController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl = "http://localhost:5202/api/UserAuth";

        public AuthController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        // Login View
        public IActionResult Login()
        {
            return View();
        }

        //[HttpPost]
        //public async Task<IActionResult> Login(LoginModel loginModel)
        //{
        //    if (!ModelState.IsValid)
        //        return View(loginModel);

        //    var jsonContent = JsonConvert.SerializeObject(loginModel);
        //    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        //    var response = await _httpClient.PostAsync($"{_apiBaseUrl}/Login", content);
        //    if (response.IsSuccessStatusCode)
        //    {
        //        var result = await response.Content.ReadAsStringAsync();
        //        var responseData = JsonConvert.DeserializeObject<dynamic>(result);

        //        // Ensure responseData.token is converted to string
        //        string token = responseData.token?.ToString() ?? string.Empty;

        //        if (!string.IsNullOrEmpty(token))
        //        {
        //            HttpContext.Session.SetString("Token", token);
        //            return RedirectToAction("Index", "Home"); // Redirect after login
        //        }
        //    }

        //    ModelState.AddModelError(string.Empty, "Invalid Email or Password");
        //    return View(loginModel);
        //}

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            if (!ModelState.IsValid)
                return View(loginModel);

            var jsonContent = JsonConvert.SerializeObject(loginModel);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_apiBaseUrl}/Login", content);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                var responseData = JsonConvert.DeserializeObject<dynamic>(result);

                // Extract and store token
                string token = responseData.token?.ToString() ?? string.Empty;

                // Extract and store user name
                string userName = responseData.userName?.ToString() ?? "Guest";

                if (!string.IsNullOrEmpty(token))
                {
                    HttpContext.Session.SetString("Token", token);
                    HttpContext.Session.SetString("UserName", userName); // Store user name in session

                    return RedirectToAction("Index", "Home"); // Redirect after login
                }
            }

            ModelState.AddModelError(string.Empty, "Invalid Email or Password");
            return View(loginModel);
        }


        // Register View
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel registerModel)
        {
            if (!ModelState.IsValid)
                return View(registerModel);

            var jsonContent = JsonConvert.SerializeObject(registerModel);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_apiBaseUrl}/Register", content);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Login");
            }

            ModelState.AddModelError(string.Empty, "Registration Failed");
            return View(registerModel);
        }

        // Logout
        //public IActionResult Logout()
        //{
        //    HttpContext.Session.Remove("Token");
        //    return RedirectToAction("Login");
        //}

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("Token");
            HttpContext.Session.Remove("UserName"); // Remove stored user name
            return RedirectToAction("Login");
        }

    }
}
