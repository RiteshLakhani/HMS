using Hospital_Management_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System;

namespace Hospital_Management_System.Controllers
{
    public class AppointmentsController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public AppointmentsController(IConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(_configuration["WebApiBaseUrl"])
            };
        }

        #region List of Appointments
        public async Task<IActionResult> Index(string patientName, string doctorName, DateTime? appointmentDate, string status)
        {
            try
            {
                var queryParams = new List<string>();
                if (!string.IsNullOrEmpty(patientName))
                    queryParams.Add($"patientName={Uri.EscapeDataString(patientName)}");
                if (!string.IsNullOrEmpty(doctorName))
                    queryParams.Add($"doctorName={Uri.EscapeDataString(doctorName)}");
                if (appointmentDate.HasValue)
                    queryParams.Add($"appointmentDate={appointmentDate.Value.ToString("yyyy-MM-dd")}");
                if (!string.IsNullOrEmpty(status))
                    queryParams.Add($"status={Uri.EscapeDataString(status)}");

                string queryString = queryParams.Any() ? "?" + string.Join("&", queryParams) : "";

                // Make API call with filters
                var response = await _httpClient.GetAsync($"api/Appointments{queryString}");

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var appointments = JsonConvert.DeserializeObject<List<AppointmentModel>>(data);
                    return View(appointments);
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to load appointments.";
                    return View(new List<AppointmentModel>());
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
                return View(new List<AppointmentModel>());
            }
        }
        #endregion


        #region Add/Edit Appointment
        public async Task<IActionResult> Add(int? AppointmentID)
        {
            await LoadDoctorListAsync();
            await LoadPatientListAsync();

            if (AppointmentID.HasValue && AppointmentID.Value > 0)
            {
                try
                {
                    var response = await _httpClient.GetAsync($"api/Appointments/{AppointmentID.Value}");
                    if (response.IsSuccessStatusCode)
                    {
                        var data = await response.Content.ReadAsStringAsync();
                        var appointment = JsonConvert.DeserializeObject<AppointmentModel>(data);
                        ViewData["PageTitle"] = "Edit Appointment";
                        return View(appointment);
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Appointment not found.";
                    }
                }
                catch (HttpRequestException ex)
                {
                    TempData["ErrorMessage"] = $"Error fetching appointment: {ex.Message}";
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"An unexpected error occurred: {ex.Message}";
                }

                return RedirectToAction("Index");
            }

            ViewData["PageTitle"] = "Add New Appointment";
            return View(new AppointmentModel());
        }
        #endregion

        #region Save Appointment
        [HttpPost]
        public async Task<IActionResult> Save(AppointmentModel appointment)
        {
            if (!ModelState.IsValid)
            {
                await LoadDoctorListAsync();
                await LoadPatientListAsync();
                return View("Add", appointment);
            }

            try
            {
                // Ensure TokenNumber is set correctly on the backend
                if (appointment.AppointmentID == 0)
                {
                    appointment.TokenNumber = string.Empty; // Auto-generated on backend
                }
                else
                {
                    var existingResponse = await _httpClient.GetAsync($"api/Appointments/{appointment.AppointmentID}");
                    if (existingResponse.IsSuccessStatusCode)
                    {
                        var existingData = await existingResponse.Content.ReadAsStringAsync();
                        var existingAppointment = JsonConvert.DeserializeObject<AppointmentModel>(existingData);
                        appointment.TokenNumber = existingAppointment.TokenNumber; // Preserve TokenNumber
                    }
                }

                var json = JsonConvert.SerializeObject(appointment);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response;

                if (appointment.AppointmentID > 0) // Update existing appointment
                {
                    response = await _httpClient.PutAsync($"api/Appointments/{appointment.AppointmentID}", content);
                }
                else // Add new appointment
                {
                    response = await _httpClient.PostAsync("api/Appointments", content);
                }

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = appointment.AppointmentID == 0
                        ? "Appointment added successfully!"
                        : "Appointment updated successfully!";
                    return RedirectToAction("Index");
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    TempData["ErrorMessage"] = !string.IsNullOrEmpty(errorMessage)
                        ? errorMessage
                        : "An error occurred while saving the appointment.";
                }
            }
            catch (HttpRequestException ex)
            {
                TempData["ErrorMessage"] = $"Request failed: {ex.Message}";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An unexpected error occurred: {ex.Message}";
            }

            await LoadDoctorListAsync();
            await LoadPatientListAsync();
            return View("Add", appointment);
        }
        #endregion

        #region Appointment Delete
        public async Task<IActionResult> Delete(int AppointmentID)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/Appointments/{AppointmentID}");

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Appointment deleted successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to delete appointment. Please try again.";
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
            try
            {
                var response = await _httpClient.GetAsync("api/Appointments/doctors");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var doctors = JsonConvert.DeserializeObject<List<DoctorsDropDown>>(data);
                    ViewBag.DoctorList = doctors ?? new List<DoctorsDropDown>();
                }
                else
                {
                    ViewBag.DoctorList = new List<DoctorsDropDown>();
                }
            }
            catch (Exception ex)
            {
                ViewBag.DoctorList = new List<DoctorsDropDown>();
                TempData["ErrorMessage"] = $"Error loading doctors: {ex.Message}";
            }
        }
        #endregion

        #region Patient Dropdown
        private async Task LoadPatientListAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/Appointments/patients");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var patients = JsonConvert.DeserializeObject<List<PatientsDropDown>>(data);
                    ViewBag.PatientList = patients ?? new List<PatientsDropDown>();
                }
                else
                {
                    ViewBag.PatientList = new List<PatientsDropDown>();
                }
            }
            catch (Exception ex)
            {
                ViewBag.PatientList = new List<PatientsDropDown>();
                TempData["ErrorMessage"] = $"Error loading patients: {ex.Message}";
            }
        }
        #endregion
    }
}
