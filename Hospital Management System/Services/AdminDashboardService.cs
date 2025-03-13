using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Hospital_Management_System.Models;

public class AdminDashboardService
{
    private readonly HttpClient _httpClient;

    public AdminDashboardService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<AdminDashboardModel> GetDashboardCounts()
    {
        var response = await _httpClient.GetAsync("api/admin/dashboardcounts");
        if (response.IsSuccessStatusCode)
        {
            var jsonString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<AdminDashboardModel>(jsonString);
        }
        return null;
    }

    public async Task<RecentEntriesModel> GetRecentEntries()
    {
        var response = await _httpClient.GetAsync("api/admin/recententries");
        if (response.IsSuccessStatusCode)
        {
            var jsonString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<RecentEntriesModel>(jsonString);
        }
        return null;
    }

    public async Task<MonthlyStatsModel> GetMonthlyStats()
    {
        var response = await _httpClient.GetAsync("api/admin/monthlystats");
        if (response.IsSuccessStatusCode)
        {
            var jsonString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<MonthlyStatsModel>(jsonString);
        }
        return null;
    }
}
