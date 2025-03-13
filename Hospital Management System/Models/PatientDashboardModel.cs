using System;
using System.Collections.Generic;

namespace Hospital_Management_System.Models
{
    public class PatientDashboardModel
    {
        public int PatientID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        public List<AppointmentsModel> Appointments { get; set; }
        public List<PatientPaymentModel> Payments { get; set; }
        public List<RoomsModel> Rooms { get; set; }
    }

    public class AppointmentsModel
    {
        public int AppointmentID { get; set; }
        public DateTime AppointmentDate { get; set; }
        public TimeSpan AppointmentTime { get; set; }
        public string Problem { get; set; }
        public string Status { get; set; }
    }

    public class PatientPaymentModel
    {
        public int PaymentID { get; set; }
        public string ServiceName { get; set; }
        public decimal CostOfTreatment { get; set; }
        public string PaymentStatus { get; set; }
    }

    public class RoomsModel
    {
        public int RoomID { get; set; }
        public int RoomNumber { get; set; }
        public string RoomType { get; set; }
        public DateTime AllotmentDate { get; set; }
        public DateTime? DischargeDate { get; set; }
    }
}
