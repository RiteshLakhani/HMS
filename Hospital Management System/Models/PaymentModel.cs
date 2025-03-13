using System;
using System.ComponentModel.DataAnnotations;

namespace Hospital_Management_System.Models
{
    public class PaymentModel
    {
        public int PaymentID { get; set; }

        [Required(ErrorMessage = "Patient ID is required.")]
        public int PatientID { get; set; }

        public string? PatientName { get; set; }

        [Required(ErrorMessage = "Doctor ID is required.")]
        public int DoctorID { get; set; }

        public string? DoctorName { get; set; }

        [Required(ErrorMessage = "Department is required.")]
        public string Department { get; set; }

        [Required(ErrorMessage = "Service Name is required.")]
        public string ServiceName { get; set; }

        [Required(ErrorMessage = "Cost of Treatment is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Cost must be a positive number.")]
        public decimal CostOfTreatment { get; set; }

        [Required(ErrorMessage = "Admission Date is required.")]
        public DateTime? AdmissionDate { get; set; }

        public DateTime? DischargeDate { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Advanced Paid must be a positive number.")]
        public decimal? AdvancedPaid { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Discount must be a positive number.")]
        public decimal? Discount { get; set; }

        [Required(ErrorMessage = "Amount is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Amount must be a positive number.")]
        public decimal? Amount { get; set; }

        [Required(ErrorMessage = "Payment Date is required.")]
        public DateTime PaymentDate { get; set; }

        [Required(ErrorMessage = "Payment Method is required.")]
        public string PaymentMethod { get; set; }  // Values: "Cash", "Card", "Check"

        [Required(ErrorMessage = "Payment Type is required.")]
        public string PaymentType { get; set; }

        [RequiredIfPaymentMethodIsCardOrCheck(ErrorMessage = "The Card or Check No field is required when the payment method is Card or Check.")]
        public string? CardOrCheckNo { get; set; }

        [Required(ErrorMessage = "Payment Status is required.")]
        public string PaymentStatus { get; set; }

        public bool IsConfirmed { get; set; }
    }

    // Custom validation attribute to make CardOrCheckNo required only if the PaymentMethod is Card or Check
    public class RequiredIfPaymentMethodIsCardOrCheckAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var paymentModel = (PaymentModel)validationContext.ObjectInstance;

        
            if (paymentModel.PaymentMethod == "Card" || paymentModel.PaymentMethod == "Check")
            {
                if (string.IsNullOrEmpty(value?.ToString()))
                {
                    return new ValidationResult(ErrorMessage ?? "The Card or Check No field is required when the payment method is Card or Check.");
                }
            }
            return ValidationResult.Success;
        }
    }
}