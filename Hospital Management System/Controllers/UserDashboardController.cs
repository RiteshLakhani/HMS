using Hospital_Management_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Hospital_Management_System.Controllers
{
    public class UserDashboardController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public UserDashboardController(IConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(_configuration["WebApiBaseUrl"])
            };
        }

        #region User Dashboard View
        public async Task<IActionResult> Index(int patientId)
        {
            var dashboardModel = new PatientDashboardModel();

            try
            {
                var response = await _httpClient.GetAsync(string.Format("api/UserDashboard/GetPatientDashboard/{0}", patientId));
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    dashboardModel = JsonConvert.DeserializeObject<PatientDashboardModel>(data);
                }
                else
                {
                    TempData["ErrorMessage"] = "Error fetching patient dashboard data.";
                }
            }
            catch
            {
                TempData["ErrorMessage"] = "An error occurred while fetching the dashboard data.";
            }

            return View(dashboardModel);
        }
        #endregion
    }
}
