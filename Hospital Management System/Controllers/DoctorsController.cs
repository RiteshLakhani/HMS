using Hospital_Management_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Hospital_Management_System.Controllers
{
    public class DoctorsController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public DoctorsController(IConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(_configuration["WebApiBaseUrl"])
            };
        }

        #region List of Doctors
        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("api/Doctors");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var doctors = JsonConvert.DeserializeObject<List<DoctorModel>>(data);
                return View(doctors);
            }
            TempData["ErrorMessage"] = "Failed to load doctors list.";
            return View(new List<DoctorModel>());
        }
        #endregion

        #region Add/Edit Doctor
        public async Task<IActionResult> Add(int? DoctorID)
        {
            if (DoctorID.HasValue)
            {
                var response = await _httpClient.GetAsync($"api/Doctors/{DoctorID}");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var doctor = JsonConvert.DeserializeObject<DoctorModel>(data);
                    ViewData["PageTitle"] = "Edit Doctor";
                    return View(doctor);
                }
                else
                {
                    TempData["ErrorMessage"] = "Doctor not found.";
                    return RedirectToAction("Index");
                }
            }

            ViewData["PageTitle"] = "Add New Doctor";
            return View(new DoctorModel());
        }
        #endregion

        #region Save Doctor
        [HttpPost]
        public async Task<IActionResult> Save(DoctorModel doctor)
        {
            if (ModelState.IsValid)
            {
                var content = new MultipartFormDataContent();

                // Add doctor properties as form fields
                content.Add(new StringContent(doctor.DoctorID.ToString()), "DoctorID");
                content.Add(new StringContent(doctor.DoctorName ?? ""), "DoctorName");
                content.Add(new StringContent(doctor.DateOfBirth.ToString("yyyy-MM-dd")), "DateOfBirth");
                content.Add(new StringContent(doctor.Age.ToString()), "Age");
                content.Add(new StringContent(doctor.Specialization ?? ""), "Specialization");
                content.Add(new StringContent(doctor.Experience.ToString()), "Experience");
                content.Add(new StringContent(doctor.ContactNumber ?? ""), "ContactNumber");
                content.Add(new StringContent(doctor.Email ?? ""), "Email");
                content.Add(new StringContent(doctor.Gender ?? ""), "Gender");
                content.Add(new StringContent(doctor.DoctorDetail ?? ""), "DoctorDetail");
                content.Add(new StringContent(doctor.Address ?? ""), "Address");
                content.Add(new StringContent(doctor.IsConfirmed.ToString()), "IsConfirmed");

                // Add file if it exists
                if (doctor.file != null && doctor.file.Length > 0)
                {
                    var streamContent = new StreamContent(doctor.file.OpenReadStream());
                    streamContent.Headers.ContentType = new MediaTypeHeaderValue(doctor.file.ContentType);
                    content.Add(streamContent, "file", doctor.file.FileName);
                }

                // Send request to API
                HttpResponseMessage response;
                if (doctor.DoctorID != 0)
                {
                    response = await _httpClient.PutAsync($"api/Doctors/{doctor.DoctorID}", content);
                }
                else
                {
                    response = await _httpClient.PostAsync("api/Doctors/InsertDoctor", content);
                }
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = doctor.DoctorID == 0
                        ? "Doctor added successfully!"
                        : "Doctor updated successfully!";
                    return RedirectToAction("Index");
                }
            }
                

            ViewData["PageTitle"] = doctor.DoctorID == 0 ? "Add New Doctor" : "Edit Doctor";
            return View("Add", doctor);
        }


        #endregion

        #region Delete Doctor
        public async Task<IActionResult> Delete(int DoctorID)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/Doctors/{DoctorID}");

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Doctor deleted successfully!";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to delete doctor. Please try again.";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
                return RedirectToAction("Index");
            }
        }
        #endregion

    }
}
