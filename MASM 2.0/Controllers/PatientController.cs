using MASM_2._0.Models;
using MASM_2._0.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace MASM_2._0.Controllers
{
	public class PatientController : Controller
	{
		private readonly UserManager<PatientUser> _userManager;
		private readonly SignInManager<PatientUser> _signInManager;

		public PatientController(UserManager<PatientUser> userManager, SignInManager<PatientUser> signInManager)
		{
			_userManager = userManager;
			_signInManager = signInManager;
		}

		public async Task<IActionResult> Index()
		{
			var patients = await Task.FromResult(_userManager.Users.ToList());
			return View(patients);
		}

		public IActionResult Register() => View();

		[HttpPost]
		public async Task<IActionResult> Register(PatientRegisterViewModel model)
		{
			if (!ModelState.IsValid) return View(model);

			var patientUser = new PatientUser
			{
				UserName = model.Email,
				Email = model.Email,
				PhoneNumber = model.MobileNumber,
				MobileNumber = model.MobileNumber,
				FirstName = model.FirstName,
				LastName = model.LastName,
				Address = model.Address,
				BirthDate = model.BirthDate,
				Sex = model.Sex,
				CivilStatus = model.CivilStatus,
				BloodType = model.BloodType,
				EmergencyContact = model.EmergencyContact,
				EmergencyContactNumber = model.EmergencyContactNumber,
				EmailVerified = false,
				Status = PatientStatus.Active,
				CreatedAt = DateTime.UtcNow
			};

			var result = await _userManager.CreateAsync(patientUser, model.Password);

			if (result.Succeeded)
			{
				await _signInManager.SignInAsync(patientUser, isPersistent: false);
				return RedirectToAction("Index", "Patient");
			}

			foreach (var error in result.Errors)
			{
				ModelState.AddModelError(string.Empty, error.Description);
			}

			return View(model);
		}

		public IActionResult Login() => View();

		[HttpPost]
		public async Task<IActionResult> Login(PatientLoginViewModel model)
		{
			if (!ModelState.IsValid) return View(model);

			var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);
			if (result.Succeeded)
			{
				var user = await _userManager.FindByEmailAsync(model.Email);
				if (user != null && await _userManager.IsInRoleAsync(user, "Patient"))
				{
					return RedirectToAction("Index", "Patient"); // Redirect to Patient Index
				}

				return RedirectToAction("Index", "Home"); // Default redirect
			}

			ModelState.AddModelError(string.Empty, "Invalid email or password.");
			return View(model);
		}

		public async Task<IActionResult> Logout()
		{
			await _signInManager.SignOutAsync();
			return RedirectToAction("Index", "Home");
		}

		public async Task<IActionResult> Edit(string id)
		{
			var patient = await _userManager.FindByIdAsync(id);
			if (patient == null) return NotFound();

			var model = new PatientEditViewModel
			{
				Id = patient.Id,
				Email = patient.Email,
				MobileNumber = patient.MobileNumber,
				FirstName = patient.FirstName,
				LastName = patient.LastName,
				Address = patient.Address,
				BirthDate = patient.BirthDate,
				Sex = patient.Sex,
				CivilStatus = patient.CivilStatus,
				BloodType = patient.BloodType,
				EmergencyContact = patient.EmergencyContact,
				EmergencyContactNumber = patient.EmergencyContactNumber,
				EmailVerified = patient.EmailVerified,
				Status = patient.Status
			};

			return View(model);
		}

		[HttpPost]
		public async Task<IActionResult> Edit(PatientEditViewModel model)
		{
			if (!ModelState.IsValid) return View(model);

			var patient = await _userManager.FindByIdAsync(model.Id);
			if (patient == null) return NotFound();

			patient.Email = model.Email;
			patient.MobileNumber = model.MobileNumber;
			patient.FirstName = model.FirstName;
			patient.LastName = model.LastName;
			patient.Address = model.Address;
			patient.BirthDate = model.BirthDate;
			patient.Sex = model.Sex;
			patient.CivilStatus = model.CivilStatus;
			patient.BloodType = model.BloodType;
			patient.EmergencyContact = model.EmergencyContact;
			patient.EmergencyContactNumber = model.EmergencyContactNumber;
			patient.EmailVerified = model.EmailVerified;
			patient.Status = model.Status;

			await _userManager.UpdateAsync(patient);
			return RedirectToAction("Index");
		}
	}
}
