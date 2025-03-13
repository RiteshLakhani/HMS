using System;
using System.ComponentModel.DataAnnotations;

namespace Hospital_Management_System.Models
{
    public class AppointmentModel
    {
        public int AppointmentID { get; set; }

        [Required]
        public int PatientID { get; set; }

        public string? PatientName { get; set; }  // Added for display  

        [Required]
        public int DoctorID { get; set; }

        public string? DoctorName { get; set; }  // Added for display  

        [Required]
        [DataType(DataType.Date, ErrorMessage = "Invalid date format.")]
        public DateTime? AppointmentDate { get; set; }

        [Required]
        [DataType(DataType.Time, ErrorMessage = "Invalid time format.")]
        public TimeSpan? AppointmentTime { get; set; }

        public string? TokenNumber { get; set; }

        [StringLength(500, ErrorMessage = "Problem description cannot exceed 500 characters.")]
        public string Problem { get; set; }

        [StringLength(20, ErrorMessage = "Status cannot exceed 20 characters.")]
        public string Status { get; set; }
        public bool IsConfirmed { get; set; }
    }

}
