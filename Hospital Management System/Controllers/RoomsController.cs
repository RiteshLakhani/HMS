using Hospital_Management_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Hospital_Management_System.Controllers
{
    public class RoomsController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public RoomsController(IConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = new HttpClient
            {
                BaseAddress = new System.Uri(_configuration["WebApiBaseUrl"])
            };
        }

        #region List of Rooms
        public async Task<IActionResult> Index(string roomNumber, string roomType, string patientName, DateTime? allotmentDate, DateTime? dischargeDate, string doctorName, bool? isConfirmed)
        {
            try
            {
                var queryParams = new List<string>();

                if (!string.IsNullOrEmpty(roomNumber))
                    queryParams.Add($"roomNumber={Uri.EscapeDataString(roomNumber)}");
                if (!string.IsNullOrEmpty(roomType))
                    queryParams.Add($"roomType={Uri.EscapeDataString(roomType)}");
                if (!string.IsNullOrEmpty(patientName))
                    queryParams.Add($"patientName={Uri.EscapeDataString(patientName)}");
                if (allotmentDate.HasValue)
                    queryParams.Add($"allotmentDate={allotmentDate.Value.ToString("yyyy-MM-dd")}");
                if (dischargeDate.HasValue)
                    queryParams.Add($"dischargeDate={dischargeDate.Value.ToString("yyyy-MM-dd")}");
                if (!string.IsNullOrEmpty(doctorName))
                    queryParams.Add($"doctorName={Uri.EscapeDataString(doctorName)}");
                if (isConfirmed.HasValue)  // Fixed this condition
                    queryParams.Add($"isConfirmed={isConfirmed.Value.ToString().ToLower()}"); // Convert to lowercase for API compatibility

                string queryString = queryParams.Any() ? "?" + string.Join("&", queryParams) : "";

                // Make API call with filters
                var response = await _httpClient.GetAsync($"api/Rooms{queryString}");

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var rooms = JsonConvert.DeserializeObject<List<RoomModel>>(data) ?? new List<RoomModel>();
                    return View(rooms);
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to load rooms.";
                    return View(new List<RoomModel>());
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
                return View(new List<RoomModel>());
            }
        }

        #endregion

        #region Add/Edit Room
        public async Task<IActionResult> Add(int? RoomID)
        {
            await LoadDoctorListAsync();
            await LoadPatientListAsync();
            if (RoomID.HasValue)
            {
                var response = await _httpClient.GetAsync($"api/Rooms/{RoomID}");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var room = JsonConvert.DeserializeObject<RoomModel>(data);
                    ViewData["PageTitle"] = "Edit Room";
                    return View(room);
                }
                else
                {
                    TempData["ErrorMessage"] = "Room not found.";
                    return RedirectToAction("Index");
                }
            }

            ViewData["PageTitle"] = "Add New Room";
            return View(new RoomModel());
        }
        #endregion

        #region Save Room
        public async Task<IActionResult> Save(RoomModel room)
        {
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(room);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response;

                try
                {
                    if (room.RoomID != 0)
                    {
                        response = await _httpClient.PutAsync($"api/Rooms/{room.RoomID}", content);
                    }
                    else // Add new room
                    {
                        response = await _httpClient.PostAsync("api/Rooms", content);
                    }

                    if (response.IsSuccessStatusCode)
                    {
                        TempData["SuccessMessage"] = room.RoomID == 0
                            ? "Room added successfully!"
                            : "Room updated successfully!";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        var errorMessage = await response.Content.ReadAsStringAsync();
                        TempData["ErrorMessage"] = !string.IsNullOrEmpty(errorMessage)
                            ? errorMessage
                            : "An error occurred while saving the room.";
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Exception: {ex.Message}";
                }
            }
            await LoadDoctorListAsync();
            await LoadPatientListAsync();
            return View("Add", room);
        }
        #endregion

        #region Delete Room
        public async Task<IActionResult> Delete(int RoomID)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/Rooms/{RoomID}");

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Room deleted successfully!";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to delete room. Please try again.";
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

        #region Doctor Dropdown
        private async Task LoadDoctorListAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/Rooms/doctors");
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
                var response = await _httpClient.GetAsync("api/Rooms/patients");
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
