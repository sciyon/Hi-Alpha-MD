using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MASM.DataAccess.Repositories;
using MASM.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace MASM.Controllers
{
	[Authorize]
	public class AppointmentController : Controller
	{
		private readonly IAppointmentRepository _appointmentRepository;
		private readonly UserManager<User> _userManager;

		public AppointmentController(IAppointmentRepository appointmentRepository, UserManager<User> userManager)
		{
			_appointmentRepository = appointmentRepository;
			_userManager = userManager;
		}

		// GET: Appointment/GetAll
		public async Task<IActionResult> GetAll(int clinicId, string range = "monthly", AppointmentStatus? status = null)
		{
			var appointments = await _appointmentRepository.GetAllAsync(clinicId, range, status);
			return View(appointments);
		}

		// GET: Appointment/PatientCreate
		[Authorize(Roles = "Patient")]
		public IActionResult PatientCreate()
		{
			return View();
		}

		// POST: Appointment/PatientCreate
		[HttpPost]
		[Authorize(Roles = "Patient")]
		public async Task<IActionResult> PatientCreate(Appointment appointment)
		{
			if (ModelState.IsValid)
			{
				var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
				appointment.PatientId = userId;
				appointment.Status = AppointmentStatus.Pending;
				appointment.CreatedById = userId;

				await _appointmentRepository.CreateAsync(appointment);
				return RedirectToAction(nameof(GetAll), new { clinicId = appointment.ClinicId });
			}
			return View(appointment);
		}

		// GET: Appointment/StaffCreate
		[Authorize(Roles = "Doctor,Assistant")]
		public IActionResult StaffCreate()
		{
			return View();
		}

		// POST: Appointment/StaffCreate
		[HttpPost]
		[Authorize(Roles = "Doctor,Assistant")]
		public async Task<IActionResult> StaffCreate(Appointment appointment)
		{
			if (ModelState.IsValid)
			{
				var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
				appointment.CreatedById = userId;
				appointment.Status = AppointmentStatus.Confirmed;

				await _appointmentRepository.CreateAsync(appointment);
				return RedirectToAction(nameof(GetAll), new { clinicId = appointment.ClinicId });
			}
			return View(appointment);
		}

		// GET: Appointment/Edit/{id}
		[Authorize(Roles = "Doctor,Assistant")]
		public async Task<IActionResult> Edit(Guid id)
		{
			var appointment = await _appointmentRepository.GetByIdAsync(id);
			if (appointment == null)
			{
				return NotFound();
			}
			return View(appointment);
		}

		// POST: Appointment/Edit/{id}
		[HttpPost]
		[Authorize(Roles = "Doctor,Assistant")]
		public async Task<IActionResult> Edit(Guid id, Appointment appointment)
		{
			if (id != appointment.Id)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				var existingAppointment = await _appointmentRepository.GetByIdAsync(id);
				if (existingAppointment == null)
				{
					return NotFound();
				}

				// Only allow status changes from Pending to Confirmed, Cancelled, or Denied
				if (existingAppointment.Status == AppointmentStatus.Pending &&
					(appointment.Status == AppointmentStatus.Confirmed ||
					 appointment.Status == AppointmentStatus.Cancelled ||
					 appointment.Status == AppointmentStatus.Denied))
				{
					existingAppointment.Status = appointment.Status;
					existingAppointment.ModifiedById = User.FindFirstValue(ClaimTypes.NameIdentifier);
					existingAppointment.ModifiedOn = DateTime.UtcNow;

					await _appointmentRepository.UpdateAsync(existingAppointment);
					return RedirectToAction(nameof(GetAll), new { clinicId = existingAppointment.ClinicId });
				}
				else
				{
					ModelState.AddModelError("Status", "Invalid status transition.");
				}
			}
			return View(appointment);
		}
	}
}