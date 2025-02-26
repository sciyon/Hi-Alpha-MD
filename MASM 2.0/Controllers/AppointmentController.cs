using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MASM.DataAccess.Repositories;
using MASM.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using MASM.Models.ViewModels.Appointment;
using MASM.Models.Clinic;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using Newtonsoft.Json;
using System.Globalization;
using Microsoft.Extensions.Logging; // Added for logging

namespace MASM.Controllers
{
	[Authorize]
	public class AppointmentController : Controller
	{
		private readonly IAppointmentRepository _appointmentRepository;
		private readonly UserManager<User> _userManager;
		private readonly IClinicRepository _clinicRepository;
		private readonly IUserClinicRepository _userClinicRepository;
		private readonly IUserRepository _userRepository;
		private readonly ILogger<AppointmentController> _logger; // Added logger

		public AppointmentController
			(
				IAppointmentRepository appointmentRepository,
				UserManager<User> userManager,
				IClinicRepository clinicRepository,
				IUserClinicRepository userClinicRepository,
				IUserRepository userRepository,
				ILogger<AppointmentController> logger // Inject logger
			)
		{
			_appointmentRepository = appointmentRepository;
			_userManager = userManager;
			_clinicRepository = clinicRepository;
			_userClinicRepository = userClinicRepository;
			_userRepository = userRepository;
			_logger = logger; // Assign logger
		}

		// GET: Appointment/Index
		[HttpGet]
		public async Task<IActionResult> Index(
			int clinicId = 0, // Make nullable
			string range = "monthly",
			AppointmentStatus? status = null,
			DateTime? startDate = null,
			string dateFilter = "future", // Default to "future"
			string sortBy = "ascending",   // Default to "ascending"
			int page = 1,
			int pageSize = 10)
		{
			var (appointments, totalCount) = await _appointmentRepository.GetAllAsync(
				clinicId, range, status, startDate, page, pageSize, dateFilter, sortBy);

			ViewBag.ClinicId = clinicId;
			ViewBag.Range = range;
			ViewBag.Status = status;
			ViewBag.StartDate = startDate;
			ViewBag.DateFilter = dateFilter;
			ViewBag.SortBy = sortBy;
			ViewBag.Page = page;
			ViewBag.PageSize = pageSize;
			ViewBag.TotalCount = totalCount;

			// Pass clinics to the view
			var clinics = await _clinicRepository.GetAllClinicsAsync();
			ViewBag.Clinics = clinics;

			return View(appointments);
		}

		public async Task<IActionResult> Create()
		{
			// 1. Fetch Clinics
			var clinics = await _clinicRepository.GetAllClinicsAsync();
			ViewBag.ClinicList = new SelectList(clinics, "Id", "Name");

			// Loads the patient
			var patients = await _userRepository.GetPatients();
			var patientSelectList = new SelectList(patients.Select(p => new
			{
				Id = p.Id,
				Name = (string.IsNullOrEmpty(p.LastName) || string.IsNullOrEmpty(p.FirstName)) ?
				   "No Name" :
				   $"{p.LastName}, {p.FirstName?.Substring(0, 1)}." // Format Patient Name and check if p.FirstName is null, ensure FirstName not null
			}), "Id", "Name");

			ViewBag.PatientList = patientSelectList;

			// 2. Default Clinic and Users to empty lists
			ViewBag.DoctorList = new SelectList(Enumerable.Empty<SelectListItem>(), "Value", "Text");

			//3. Set Status
			ViewBag.StatusList = new SelectList(new List<AppointmentStatus> { AppointmentStatus.Confirmed, AppointmentStatus.Pending }, AppointmentStatus.Pending);

			return View();
		}

		[HttpPost]
		public async Task<IActionResult> LoadDoctors(int clinicId)
		{
			//Gets the doctor from that clinic through the clinicId
			var doctors = await _userClinicRepository.GetStaff(clinicId, "Doctor");

			//Converts the data into the format needed
			var doctorSelectList = doctors.Select(d => new
			{
				Id = d.Id,
				Name = (string.IsNullOrEmpty(d.LastName) || string.IsNullOrEmpty(d.FirstName)) ?
				   "No Name" :
				   $"Dr. {d.LastName}, {d.FirstName?.Substring(0, 1)}." // Format Doctor Name, ensure FirstName not null
			}).ToList();

			return Json(doctorSelectList);
		}

		[HttpPost]
		public async Task<IActionResult> GetClinicDetails(string clinicId)
		{
			var clinic = await _clinicRepository.GetClinicByIdAsync(clinicId);
			if (clinic == null)
			{
				return NotFound();
			}

			return Json(new { clinic.StartingTime, clinic.ClosingTime });
		}

		// POST: Appointment/Create
		[HttpPost]
		[Authorize(Roles = "Doctor,Assistant,Admin")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(CreateAppointmentViewModel model)
		{
			_logger.LogInformation("Attempting to create appointment.");
			_logger.LogInformation($"Model values: {JsonConvert.SerializeObject(model)}"); // Log the model

			if (ModelState.IsValid)
			{
				try
				{
					var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
					_logger.LogInformation($"User ID: {userId}");

					var appointment = new Appointment
					{
						DoctorId = model.DoctorId, // Doctor can be selected
						PatientId = model.PatientId, // Patient can be selected
						ReasonForVisit = model.ReasonForVisit,
						AdditionalInfo = model.AdditionalInfo,
						AppointmentDate = model.AppointmentDateTime,
						ClinicId = model.ClinicId,
						Status = model.Status,
						CreatedById = userId,
						ConfirmeeId = null,
						ModifiedById = null
					};

					_logger.LogInformation($"Appointment object created: {JsonConvert.SerializeObject(appointment)}");

					await _appointmentRepository.CreateAsync(appointment);
					_logger.LogInformation("Appointment created successfully in the database.");

					TempData["SuccessMessage"] = "Appointment created successfully!";  // Optional success message
					return RedirectToAction(nameof(Index), new { clinicId = model.ClinicId });
				}
				catch (Exception ex)
				{
					// Log the exception
					_logger.LogError(ex, "Error creating appointment.");
					ModelState.AddModelError("", "An error occurred while creating the appointment. Please try again.");
					TempData["ErrorMessage"] = "An error occurred while creating the appointment.";  // Optional error message
					return View(model);
				}
			}

			// If we got this far, something failed, redisplay form
			_logger.LogWarning("Model state is invalid. Redisplaying the form.");
			foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
			{
				_logger.LogError($"Model Error: {error.ErrorMessage}");
			}

			var clinics = await _clinicRepository.GetAllClinicsAsync();
			ViewBag.ClinicList = new SelectList(clinics, "Id", "Name", model.ClinicId);

			// Populate DoctorList again in case of validation errors
			// Ensure to only populate based on the selected clinic for DoctorList
			var doctors = await _userClinicRepository.GetStaff(model.ClinicId, "Doctor");


			var doctorSelectList = new SelectList(doctors.Select(d => new
			{
				Id = d.Id,
				Name = (string.IsNullOrEmpty(d.LastName) || string.IsNullOrEmpty(d.FirstName)) ?
				   "No Name" :
				   $"Dr. {d.LastName}, {d.FirstName?.Substring(0, 1)}." // Format Doctor Name, ensure FirstName not null
			}), "Id", "Name", model.DoctorId);  //Preserve selection

			ViewBag.DoctorList = doctorSelectList;

			var users = _userManager.Users.ToList();
			var patients = new List<User>();
			foreach (var user in users)
			{
				var roles = await _userManager.GetRolesAsync(user);
				if (roles.Contains("Patient"))
				{
					patients.Add(user);
				}

			}

			// Convert to SelectList
			var patientSelectList = new SelectList(patients.Select(p => new {
				Id = p.Id,
				Name = (string.IsNullOrEmpty(p.LastName) || string.IsNullOrEmpty(p.FirstName)) ?
				   "No Name" :
				   $"{p.LastName}, {p.FirstName?.Substring(0, 1)}." // Format Patient Name and check if p.FirstName is null, ensure FirstName not null
			}), "Id", "Name", model.PatientId); //Preserve selection

			ViewBag.PatientList = patientSelectList;
			ViewBag.StatusList = new SelectList(Enum.GetValues(typeof(AppointmentStatus)).Cast<AppointmentStatus>(), model.Status); //Correct and Preserve selection


			return View(model);
		}
	}
}