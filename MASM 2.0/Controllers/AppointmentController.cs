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
using Microsoft.Extensions.Logging;

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
		private readonly ILogger<AppointmentController> _logger;

		public AppointmentController(
			IAppointmentRepository appointmentRepository,
			UserManager<User> userManager,
			IClinicRepository clinicRepository,
			IUserClinicRepository userClinicRepository,
			IUserRepository userRepository,
			ILogger<AppointmentController> logger
		)
		{
			_appointmentRepository = appointmentRepository;
			_userManager = userManager;
			_clinicRepository = clinicRepository;
			_userClinicRepository = userClinicRepository;
			_userRepository = userRepository;
			_logger = logger;
		}

		// GET: Appointment/Index
		[HttpGet]
		public async Task<IActionResult> Index(AppointmentIndexViewModel model)
		{
			var (appointments, totalCount) = await _appointmentRepository.GetAllAsync(
				model.ClinicId, model.Status, model.StartDate, model.Page, model.PageSize, model.DateFilter, model.SortBy);

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
				   $"{p.LastName}, {p.FirstName?.Substring(0, 1)}."
			}), "Id", "Name");

			ViewBag.PatientList = patientSelectList;

			// 2. Default Clinic and Users to empty lists
			ViewBag.DoctorList = new SelectList(Enumerable.Empty<SelectListItem>(), "Value", "Text");

			// 3. Set Status
			ViewBag.StatusList = new SelectList(new List<AppointmentStatus> { AppointmentStatus.Confirmed, AppointmentStatus.Pending }, AppointmentStatus.Pending);

			return View();
		}

		[HttpPost]
		public async Task<IActionResult> LoadDoctors(string clinicId)
		{
			// Gets the doctor from that clinic through the clinicId
			if (!int.TryParse(clinicId, out int clinicIdInt))
			{
				return BadRequest("Invalid Clinic ID format.");
			}

			var doctors = await _userClinicRepository.GetStaff(clinicIdInt, "Doctor");

			// Converts the data into the format needed
			var doctorSelectList = doctors.Select(d => new
			{
				Id = d.Id,
				Name = (string.IsNullOrEmpty(d.LastName) || string.IsNullOrEmpty(d.FirstName)) ?
				   "No Name" :
				   $"Dr. {d.LastName}, {d.FirstName?.Substring(0, 1)}."
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
			_logger.LogInformation($"Model values: {JsonConvert.SerializeObject(model)}");

			if (ModelState.IsValid)
			{
				try
				{
					var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
					_logger.LogInformation($"User ID: {userId}");

					var appointment = new Appointment
					{
						DoctorId = model.DoctorId,
						PatientId = model.PatientId,
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

					TempData["SuccessMessage"] = "Appointment created successfully!";
					return RedirectToAction(nameof(Index), new { clinicId = model.ClinicId });
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Error creating appointment.");
					ModelState.AddModelError("", "An error occurred while creating the appointment. Please try again.");
					TempData["ErrorMessage"] = "An error occurred while creating the appointment.";
					return View(model);
				}
			}

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
				   $"Dr. {d.LastName}, {d.FirstName?.Substring(0, 1)}."
			}), "Id", "Name", model.DoctorId);

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

			var patientSelectList = new SelectList(patients.Select(p => new {
				Id = p.Id,
				Name = (string.IsNullOrEmpty(p.LastName) || string.IsNullOrEmpty(p.FirstName)) ?
				   "No Name" :
				   $"{p.LastName}, {p.FirstName?.Substring(0, 1)}."
			}), "Id", "Name", model.PatientId);

			ViewBag.PatientList = patientSelectList;
			ViewBag.StatusList = new SelectList(Enum.GetValues(typeof(AppointmentStatus)).Cast<AppointmentStatus>(), model.Status);

			return View(model);
		}

		//GET: Appointment/View/{appointmentId}
		public async Task<IActionResult> View(Guid appointmentId)
		{
			var appointment = await _appointmentRepository.GetByIdAsync(appointmentId);
			if (appointment == null)
			{
				return NotFound("Appointment not found.");
			}

			// Use ViewAppointmentViewModel to display appointment details
			var viewModel = new ViewAppointmentViewModel
			{
				Id = appointment.Id.ToString(),
				ClinicName = appointment.Clinic.Name,
				DoctorName = appointment.Doctor.FirstName + " " + appointment.Doctor.LastName,
				PatientName = appointment.Patient.FirstName + " " + appointment.Patient.LastName,
				Status = appointment.Status,
				ReasonForVisit = appointment.ReasonForVisit,
				AdditionalInfo = appointment.AdditionalInfo,
				AppointmentDateTime = appointment.AppointmentDate
			};

			return View(viewModel);
		}

		// GET: Appointment/Edit/{appointmentId}
		[HttpGet]
		public async Task<IActionResult> Edit(Guid appointmentId)
		{
			var appointment = await _appointmentRepository.GetByIdAsync(appointmentId);
			if (appointment == null)
			{
				return NotFound("Appointment not found.");
			}

			// Fetch clinics for the dropdown
			var clinics = await _clinicRepository.GetAllClinicsAsync();
			ViewBag.ClinicList = new SelectList(clinics, "Id", "Name", appointment.ClinicId);

			// Fetch doctors for the selected clinic
			var doctors = await _userClinicRepository.GetStaff(appointment.ClinicId, "Doctor");
			var doctorSelectList = new SelectList(doctors.Select(d => new
			{
				Id = d.Id,
				Name = (string.IsNullOrEmpty(d.LastName) || string.IsNullOrEmpty(d.FirstName)) ?
					"No Name" :
					$"Dr. {d.LastName}, {d.FirstName?.Substring(0, 1)}."
			}), "Id", "Name", appointment.DoctorId);

			ViewBag.DoctorList = doctorSelectList;

			// Fetch patients
			var patients = await _userRepository.GetPatients();
			var patientSelectList = new SelectList(patients.Select(p => new
			{
				Id = p.Id,
				Name = (string.IsNullOrEmpty(p.LastName) || string.IsNullOrEmpty(p.FirstName)) ?
					"No Name" :
					$"{p.LastName}, {p.FirstName?.Substring(0, 1)}."
			}), "Id", "Name", appointment.PatientId);

			ViewBag.PatientList = patientSelectList;

			// Set status list
			ViewBag.StatusList = new SelectList(Enum.GetValues(typeof(AppointmentStatus)).Cast<AppointmentStatus>(), appointment.Status);

			var viewModel = new EditAppointmentViewModel
			{
				Id = appointment.Id.ToString(),
				ClinicId = appointment.ClinicId, // Now matches the type
				DoctorId = appointment.DoctorId,
				PatientId = appointment.PatientId,
				Status = appointment.Status,
				ReasonForVisit = appointment.ReasonForVisit,
				AdditionalInfo = appointment.AdditionalInfo,
				AppointmentDateTime = appointment.AppointmentDate
			};

			return View(viewModel);
		}

		// POST: Appointment/Edit/{appointmentId}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(EditAppointmentViewModel model)
		{
			if (ModelState.IsValid)
			{
				try
				{
					var appointment = await _appointmentRepository.GetByIdAsync(new Guid(model.Id));
					if (appointment == null)
					{
						return NotFound("Appointment not found.");
					}

					appointment.ClinicId = model.ClinicId; // Now matches the type
					appointment.DoctorId = model.DoctorId;
					appointment.PatientId = model.PatientId;
					appointment.Status = model.Status;
					appointment.ReasonForVisit = model.ReasonForVisit;
					appointment.AdditionalInfo = model.AdditionalInfo;
					appointment.AppointmentDate = model.AppointmentDateTime;

					await _appointmentRepository.UpdateAsync(appointment);

					TempData["SuccessMessage"] = "Appointment updated successfully!";
					return RedirectToAction(nameof(Index));
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Error updating appointment.");
					ModelState.AddModelError("", "An error occurred while updating the appointment. Please try again.");
					TempData["ErrorMessage"] = "An error occurred while updating the appointment.";
				}
			}

			// If we got this far, something failed; redisplay form
			var clinics = await _clinicRepository.GetAllClinicsAsync();
			ViewBag.ClinicList = new SelectList(clinics, "Id", "Name", model.ClinicId);

			var doctors = await _userClinicRepository.GetStaff(model.ClinicId, "Doctor");
			var doctorSelectList = new SelectList(doctors.Select(d => new
			{
				Id = d.Id,
				Name = (string.IsNullOrEmpty(d.LastName) || string.IsNullOrEmpty(d.FirstName)) ?
					"No Name" :
					$"Dr. {d.LastName}, {d.FirstName?.Substring(0, 1)}."
			}), "Id", "Name", model.DoctorId);

			ViewBag.DoctorList = doctorSelectList;

			var patients = await _userRepository.GetPatients();
			var patientSelectList = new SelectList(patients.Select(p => new
			{
				Id = p.Id,
				Name = (string.IsNullOrEmpty(p.LastName) || string.IsNullOrEmpty(p.FirstName)) ?
					"No Name" :
					$"{p.LastName}, {p.FirstName?.Substring(0, 1)}."
			}), "Id", "Name", model.PatientId);

			ViewBag.PatientList = patientSelectList;

			ViewBag.StatusList = new SelectList(Enum.GetValues(typeof(AppointmentStatus)).Cast<AppointmentStatus>(), model.Status);

			return View(model);
		}
	}
}