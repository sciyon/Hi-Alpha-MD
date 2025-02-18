using MASM.Models.Clinic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace MASM_2._0.Controllers
{
	[Authorize(Roles = "Admin,Doctor")] 
	public class ClinicController : Controller
	{
		private readonly IClinicRepository _clinicRepository;

		public ClinicController(IClinicRepository clinicRepository)
		{
			_clinicRepository = clinicRepository;
		}

		public async Task<IActionResult> Index()
		{
			var clinics = await _clinicRepository.GetAllClinicsAsync();
			return View(clinics);
		}

		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = "Admin,Doctor")]
		public async Task<IActionResult> Create(Clinic model)
		{
			if (!ModelState.IsValid)
				return View(model);

			var clinic = new Clinic
			{
				Name = model.Name,
				Location = model.Location,
				Telephone = model.Telephone,
				Phone = model.Phone,
				Email = model.Email,
				StartingTime = model.StartingTime,
				ClosingTime = model.ClosingTime
			};

			// If the user is an Admin, allow them to set the status
			if (User.IsInRole("Admin"))
			{
				clinic.Status = model.Status; // Admins can set the status directly
			}
			else if (User.IsInRole("Doctor"))
			{
				clinic.Status = ClinicStatus.Pending; // Doctors' clinics start with Pending status
			}

			bool success = await _clinicRepository.CreateClinicAsync(clinic);
			if (success)
				return RedirectToAction(nameof(Index));

			ModelState.AddModelError("", "Failed to create clinic.");
			return View(model);
		}



		// Show Clinic details
		public async Task<IActionResult> Details(int id)
		{
			var clinic = await _clinicRepository.GetClinicByIdAsync(id.ToString());
			if (clinic == null)
				return NotFound();

			return View(clinic);
		}

		// Show Clinic edit form
		public async Task<IActionResult> Edit(int id)
		{
			var clinic = await _clinicRepository.GetClinicByIdAsync(id.ToString());
			if (clinic == null)
				return NotFound();

			return View(clinic);
		}

		// Handle Clinic update
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(Clinic clinic)
		{
			if (!ModelState.IsValid)
				return View(clinic);

			bool success = await _clinicRepository.UpdateClinicAsync(clinic);
			if (success)
				return RedirectToAction(nameof(Index));

			ModelState.AddModelError("", "Failed to update clinic.");
			return View(clinic);
		}

		// Handle Clinic deletion
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Delete(int id)
		{
			bool success = await _clinicRepository.DeleteClinicAsync(id.ToString());
			if (success)
				return RedirectToAction(nameof(Index));

			ModelState.AddModelError("", "Failed to delete clinic.");
			return RedirectToAction(nameof(Index));
		}
	}
}
