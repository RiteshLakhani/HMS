namespace Hospital_Management_System.Models
{
    public class MonthlyStatsModel
    {
        public List<MonthlyAdmissions> Admissions { get; set; } = new List<MonthlyAdmissions>();
        public List<MonthlyPayments> Payments { get; set; } = new List<MonthlyPayments>();
    }

    public class MonthlyAdmissions
    {
        public string Month { get; set; }
        public int TotalAdmissions { get; set; }
    }

    public class MonthlyPayments
    {
        public string Month { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}
