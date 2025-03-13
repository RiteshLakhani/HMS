using Hospital_Management_System.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Hospital_Management_System.Controllers
{
    public class AdminDashboardController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public AdminDashboardController(IConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = new HttpClient
            {
                BaseAddress = new System.Uri(_configuration["WebApiBaseUrl"])
            };
        }

        #region Admin Dashboard View
        public async Task<IActionResult> Index()
        {
            var dashboardModel = new AdminDashboardModel();

            try
            {
                // Fetch Dashboard Counts
                var response = await _httpClient.GetAsync("api/AdminDashboard/dashboardcounts");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    dashboardModel.DashboardCounts = JsonConvert.DeserializeObject<DashboardCountsModel>(data);
                }

                // Fetch Recent Entries
                response = await _httpClient.GetAsync("api/AdminDashboard/recententries");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    dashboardModel.RecentEntries = JsonConvert.DeserializeObject<RecentEntriesModel>(data);
                }

                // Fetch Monthly Stats
                response = await _httpClient.GetAsync("api/AdminDashboard/monthlystats");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    dashboardModel.MonthlyStats = JsonConvert.DeserializeObject<MonthlyStatsModel>(data);
                }
            }
            catch
            {
                TempData["ErrorMessage"] = "Error fetching dashboard data.";
            }

            return View(dashboardModel);
        }
        #endregion

        #region Update Dashboard Data (Admin Only)
        [HttpPost]
        public async Task<IActionResult> UpdateDashboard()
        {
            try
            {
                var response = await _httpClient.PostAsync("api/AdminDashboard/updatedashboard", null);
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Dashboard data updated successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to update dashboard data.";
                }
            }
            catch
            {
                TempData["ErrorMessage"] = "An error occurred while updating dashboard data.";
            }

            return RedirectToAction("Index");
        }
        #endregion
    }
}
