using System;
using System.ComponentModel.DataAnnotations;

namespace Hospital_Management_System.Models
{
    public class DoctorModel
    {
        public int DoctorID { get; set; }

        [Required]
        [StringLength(100)]
        public string DoctorName { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Range(0, 120)]
        public int Age { get; set; }

        [Required]
        public string Specialization { get; set; }

        [Range(0, int.MaxValue)]
        public int Experience { get; set; }

        [Required]
        [Phone]
        public string ContactNumber { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string Gender { get; set; }

        [StringLength(500, ErrorMessage = "Doctor details cannot exceed 500 characters.")]
        public string DoctorDetail { get; set; }
        public string Address { get; set; }

        public bool IsConfirmed { get; set; }

        // Image Properties
        public string? ImagePath { get; set; }

        [Display(Name = "Upload Image")]
        public IFormFile file { get; set; }
    }


    public class DoctorsDropDown
    {
        public int DoctorID { get; set; }
        public string DoctorName { get; set; }
    }

}
