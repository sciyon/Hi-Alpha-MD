using MASM.Models.Clinic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Globalization;

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

			// Validate time format
			if (!string.IsNullOrEmpty(model.StartingTime) && !string.IsNullOrEmpty(model.ClosingTime))
			{
				if (!TimeSpan.TryParse(model.StartingTime, out TimeSpan startingTime) || !TimeSpan.TryParse(model.ClosingTime, out TimeSpan closingTime))
				{
					ModelState.AddModelError("", "Invalid time format.");
					return View(model);
				}
			}

			var clinic = new Clinic
			{
				Name = model.Name,
				Location = model.Location,
				Telephone = model.Telephone,
				Phone = model.Phone,
				Email = model.Email,
				StartingTime = model.StartingTime,
				ClosingTime = model.ClosingTime,
			};

			// If the user is an Admin, allow them to set the status
			if (User.IsInRole("Admin"))
			{
				clinic.Status = model.Status;
			}
			else if (User.IsInRole("Doctor"))
			{
				clinic.Status = ClinicStatus.Pending;
			}

			bool success = await _clinicRepository.CreateClinicAsync(clinic);
			if (success)
				return RedirectToAction(nameof(Index));

			ModelState.AddModelError("", "Failed to create clinic.");
			return View(model);
		}


		public async Task<IActionResult> Details(int id)
		{
			var clinic = await _clinicRepository.GetClinicByIdAsync(id.ToString());
			if (clinic == null)
				return NotFound();

			return View(clinic);
		}

		public async Task<IActionResult> Edit(int id)
		{
			var clinic = await _clinicRepository.GetClinicByIdAsync(id.ToString());
			if (clinic == null)
			{
				return NotFound();
			}

			return View(clinic);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, Clinic clinic)
		{
			if (id != clinic.Id)
			{
				return NotFound();
			}

			//Explicitly Validate Model
			TryValidateModel(clinic);

			if (ModelState.IsValid)
			{
				try
				{
					// If the user is a Doctor, ensure the Status is not modified
					if (User.IsInRole("Doctor"))
					{
						var existingClinic = await _clinicRepository.GetClinicByIdAsync(id.ToString());
						if (existingClinic != null)
						{
							clinic.Status = existingClinic.Status;
						}
						else
						{
							ModelState.AddModelError("", "Existing clinic not found.");
							return View(clinic);
						}
					}

					await _clinicRepository.UpdateClinicAsync(clinic);
				}
				catch (Exception ex)
				{
					// Log the exception (VERY IMPORTANT!)
					Console.WriteLine(ex); // Replace with a proper logger
					ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
					return View(clinic);
				}

				return RedirectToAction(nameof(Index));
			}

			return View(clinic);
		}

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
