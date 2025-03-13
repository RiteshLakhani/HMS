using System.ComponentModel.DataAnnotations;

namespace Hospital_Management_System.Models
{
    public class RoomModel
    {
        public int RoomID { get; set; }

        [Required(ErrorMessage = "Room number is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Room number must be a positive number.")]
        public int RoomNumber { get; set; }

        [Required(ErrorMessage = "Room type is required.")]
        [StringLength(100, ErrorMessage = "Room type must be less than 100 characters.")]
        public string RoomType { get; set; }

        [Required(ErrorMessage = "Patient ID is required.")]
        public int PatientID { get; set; }

        public string? PatientName { get; set; }  // Added for display  

        [Required(ErrorMessage = "Allotment date is required.")]
        [DataType(DataType.Date, ErrorMessage = "Please enter a valid date.")]
        [Display(Name = "Allotment Date")]
        public DateTime? AllotmentDate { get; set; }

        [Required(ErrorMessage = "Discharge date is required.")]
        [DataType(DataType.Date, ErrorMessage = "Please enter a valid date.")]
        [Display(Name = "Discharge Date")]
        public DateTime? DischargeDate { get; set; }

        [Required(ErrorMessage = "Doctor ID is required.")]
        public int DoctorID { get; set; }

        public string? DoctorName { get; set; }

        public bool IsConfirmed { get; set; }
    }
}
