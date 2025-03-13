using System.Collections.Generic;

namespace Hospital_Management_System.Models
{
    public class RecentEntriesModel
    {
        public List<PatientModel> RecentPatients { get; set; }
        public List<DoctorModel> RecentDoctors { get; set; }
        public List<PaymentModel> RecentPayments { get; set; }

        public RecentEntriesModel()
        {
            RecentPatients = new List<PatientModel>();
            RecentDoctors = new List<DoctorModel>();
            RecentPayments = new List<PaymentModel>();
        }
    }
}
