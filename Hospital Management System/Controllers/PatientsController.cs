using Hospital_Management_System.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Hospital_Management_System.Controllers
{
    public class PatientsController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public PatientsController(IConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = new HttpClient
            {
                BaseAddress = new System.Uri(_configuration["WebApiBaseUrl"])
            };
        }

        #region List of Patients
        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/Patients");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var patients = JsonConvert.DeserializeObject<List<PatientModel>>(data);
                    return View(patients);
                }
                TempData["ErrorMessage"] = "Failed to fetch patients.";
            }
            catch
            {
                TempData["ErrorMessage"] = "Error fetching data from the server.";
            }

            return View(new List<PatientModel>());
        }
        #endregion

        #region Add/Edit
        public async Task<IActionResult> Add(int? PatientID)
        {

            if (PatientID.HasValue)
            {
                var response = await _httpClient.GetAsync($"api/Patients/{PatientID}");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var patient = JsonConvert.DeserializeObject<PatientModel>(data);
                    ViewData["PageTitle"] = "Edit Patient";
                    return View(patient);
                }
            }

            ViewData["PageTitle"] = "Add New Patient";
            return View(new PatientModel());
        }
        #endregion

        #region Save
        public async Task<IActionResult> Save(PatientModel patient)
        {
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(patient);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response;

                try
                {
                    if (patient.PatientID != 0)
                    { 
                        response = await _httpClient.PutAsync($"api/Patients/{patient.PatientID}", content);
                    }
                    else
                    {
                        response = await _httpClient.PostAsync("api/Patients", content);
                    }

                    if (response.IsSuccessStatusCode)
                    {
                        TempData["SuccessMessage"] = patient.PatientID == 0
                            ? "Patient added successfully!"
                            : "Patient updated successfully!";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Failed to save patient.";
                    }
                }
                catch
                {
                    TempData["ErrorMessage"] = "An error occurred while saving.";
                }
            }

            return View("Add", patient);
        }
        #endregion

        #region Delete
        public async Task<IActionResult> Delete(int PatientID)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/Patients/{PatientID}");

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Patient deleted successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to delete patient.";
                }
            }
            catch
            {
                TempData["ErrorMessage"] = "An error occurred while deleting.";
            }

            return RedirectToAction("Index");
        }
        #endregion
    }
}
