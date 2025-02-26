using MASM.Models;
using MASM.Models.Clinic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering; // Add this
using Microsoft.AspNetCore.Identity; // Add this
using System.Linq;

namespace MASM_2._0.Controllers
{
	[Authorize(Roles = "Admin,Doctor")]
	public class UserClinicController : Controller
	{
		private readonly IUserClinicRepository _userClinicRepository;
		private readonly IClinicRepository _clinicRepository; // Inject Clinic Repository
		private readonly IUserRepository _userRepository; // Inject UserRepository
		private readonly UserManager<User> _userManager; // Inject UserManager

		public UserClinicController(IUserClinicRepository userClinicRepository,
									 IClinicRepository clinicRepository,
									 IUserRepository userRepository,
									 UserManager<User> userManager)
		{
			_userClinicRepository = userClinicRepository;
			_clinicRepository = clinicRepository;
			_userRepository = userRepository;
			_userManager = userManager;
		}

		public async Task<IActionResult> Index()
		{
			// Fetch all user-clinic associations
			List<UserClinic> userClinics = await _userClinicRepository.GetAllUserClinicsAsync(); // Assumed method

			// Pass the list to the view
			return View(userClinics);
		}

		//Create a userclinic
		public async Task<IActionResult> Create()
		{
			// Fetch users with the "Doctor" or "Assistant" role
			List<User> staff = new List<User>();
			staff = await _userRepository.GetAllUsersAsync();
			staff = staff.Where(x => _userManager.GetRolesAsync(x).Result.Any(r => r == "Assistant" || r == "Doctor")).ToList(); // Fetch users

			ViewBag.StaffList = staff;

			// Fetch all clinics
			List<Clinic> clinics = await _clinicRepository.GetAllClinicsAsync();
			ViewBag.ClinicList = clinics; // Pass the list to the view

			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(string selectedUserId, int selectedClinicId)
		{
			if (string.IsNullOrEmpty(selectedUserId) || selectedClinicId == 0)
			{
				ViewBag.ErrorMessage = "Please select a user and a clinic.";
				return View();
			}

			// Create UserClinic object
			UserClinic userClinic = new UserClinic
			{
				UserId = selectedUserId,
				ClinicId = selectedClinicId
			};

			// Call the Create method in the repository
			bool success = await _userClinicRepository.CreateUserClinicAsync(userClinic);

			if (success)
			{
				// Redirect to the index page
				return RedirectToAction("Index");
			}
			else
			{
				ViewBag.ErrorMessage = "Failed to create UserClinic.";
				return View();
			}
		}


		// GET: UserClinic/Edit/5
		public async Task<IActionResult> Edit(string userId, int clinicId)
		{
			if (string.IsNullOrEmpty(userId) || clinicId == 0)
			{
				return NotFound();
			}

			// Fetch the UserClinic object based on UserId and ClinicId
			UserClinic userClinic = await _userClinicRepository.GetUserClinicAsync(userId, clinicId);

			if (userClinic == null)
			{
				return NotFound();
			}

			// Pass the UserClinic object to the Edit view
			return View(userClinic);
		}

		// POST: UserClinic/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(string userId, int clinicId, UserClinic userClinic)
		{
			if (userId != userClinic.UserId || clinicId != userClinic.ClinicId)
			{
				return BadRequest();
			}

			if (ModelState.IsValid)
			{
				bool success = await _userClinicRepository.UpdateUserClinicAsync(userClinic);

				if (success)
				{
					return RedirectToAction(nameof(Index));
				}
				else
				{
					return View(userClinic);
				}
			}

			return View(userClinic);
		}

		[HttpPost("Delete/{userId}/{clinicId}")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Delete(string userId, int clinicId)
		{
			if (string.IsNullOrEmpty(userId) || clinicId == 0)
			{
				return NotFound();
			}

			bool success = await _userClinicRepository.DeleteUserClinicAsync(userId, clinicId);
			if (success)
				return RedirectToAction(nameof(Index));
			else
				return NotFound();
		}
	}
}