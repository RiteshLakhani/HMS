namespace Hospital_Management_System.Models
{
    public class AdminDashboardModel
    {
        public DashboardCountsModel DashboardCounts { get; set; }
        public RecentEntriesModel RecentEntries { get; set; }
        public MonthlyStatsModel MonthlyStats { get; set; }

        public AdminDashboardModel()
        {
            DashboardCounts = new DashboardCountsModel();
            RecentEntries = new RecentEntriesModel();
            MonthlyStats = new MonthlyStatsModel();
        }
    }
}
