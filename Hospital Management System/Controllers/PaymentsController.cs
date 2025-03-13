using Hospital_Management_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Hospital_Management_System.Controllers
{
    public class PaymentsController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public PaymentsController(IConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = new HttpClient
            {
                BaseAddress = new System.Uri(_configuration["WebApiBaseUrl"])
            };
        }

        #region List of Payments
        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("api/Payments");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var payments = JsonConvert.DeserializeObject<List<PaymentModel>>(data);
                return View(payments);
            }
            return View(new List<PaymentModel>());
        }
        #endregion

        #region Add/Edit Payment
        public async Task<IActionResult> Add(int? PaymentID)
        {
            await LoadDoctorListAsync();
            await LoadPatientListAsync();

            if (PaymentID.HasValue)
            {
                var response = await _httpClient.GetAsync($"api/Payments/{PaymentID}");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var payment = JsonConvert.DeserializeObject<PaymentModel>(data);
                    ViewData["PageTitle"] = "Edit Payment";
                    return View(payment);
                }

                TempData["ErrorMessage"] = "Payment not found.";
                return RedirectToAction("Index");
            }

            ViewData["PageTitle"] = "Add New Payment";
            return View(new PaymentModel());
        }
        #endregion

        #region Save Payment
        [HttpPost]
        public async Task<IActionResult> Save(PaymentModel payment)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please correct the form errors and try again.";
                await LoadDoctorListAsync();
                await LoadPatientListAsync();
                return View("Add", payment);
            }

            var json = JsonConvert.SerializeObject(payment);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response;

            try
            {
                if (payment.PaymentID != 0)
                {
                    response = await _httpClient.PutAsync($"api/Payments/{payment.PaymentID}", content);
                }
                else
                {
                    response = await _httpClient.PostAsync("api/Payments", content);
                }

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = payment.PaymentID == 0
                        ? "Payment added successfully!"
                        : "Payment updated successfully!";
                    return RedirectToAction("Index");
                }

                var errorMessage = await response.Content.ReadAsStringAsync();
                TempData["ErrorMessage"] = string.IsNullOrEmpty(errorMessage)
                    ? "An error occurred while saving the payment."
                    : errorMessage;
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
            }

            await LoadDoctorListAsync();
            await LoadPatientListAsync();
            return View("Add", payment);
        }
        #endregion

        #region Delete Payment
        public async Task<IActionResult> Delete(int PaymentID)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/Payments/{PaymentID}");
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Payment deleted successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to delete payment. Please try again.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
            }

            return RedirectToAction("Index");
        }
        #endregion

        #region Doctor Dropdown
        private async Task LoadDoctorListAsync()
        {
            ViewBag.DoctorList = new List<DoctorsDropDown>();
            try
            {
                var response = await _httpClient.GetAsync("api/Payments/doctors");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    ViewBag.DoctorList = JsonConvert.DeserializeObject<List<DoctorsDropDown>>(data) ?? new List<DoctorsDropDown>();
                }
            }
            catch
            {
                TempData["ErrorMessage"] = "Error loading doctors.";
            }
        }
        #endregion

        #region Patient Dropdown
        private async Task LoadPatientListAsync()
        {
            ViewBag.PatientList = new List<PatientsDropDown>();
            try
            {
                var response = await _httpClient.GetAsync("api/Payments/patients");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    ViewBag.PatientList = JsonConvert.DeserializeObject<List<PatientsDropDown>>(data) ?? new List<PatientsDropDown>();
                }
            }
            catch
            {
                TempData["ErrorMessage"] = "Error loading patients.";
            }
        }
        #endregion
    }
}
