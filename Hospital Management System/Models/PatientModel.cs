using System.ComponentModel.DataAnnotations;

namespace Hospital_Management_System.Models
{
    public class PatientModel
    {
        public int PatientID { get; set; }

        [Required]
        public string PatientName { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Range(0, 120)]
        public int Age { get; set; }

        [Required]
        [Phone]
        public string Phone { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string Gender { get; set; }

        [Required]
        public string Address { get; set; }

        public bool IsConfirmed { get; set; }
    }

    public class PatientsDropDown
    {
        public int PatientID { get; set; }
        public string PatientName { get; set; }
    }
}
