using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using MASM.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace MASM_2._0.Controllers
{
	public class PatientController : Controller
	{
		private readonly IPatientRepository _patientRepository;
		private readonly SignInManager<PatientUser> _signInManager;

		public PatientController(IPatientRepository patientRepository, SignInManager<PatientUser> signInManager)
		{
			_patientRepository = patientRepository;
			_signInManager = signInManager;
		}

		public async Task<IActionResult> Index()
		{
			var patients = await _patientRepository.GetAllPatientsAsync();
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

			var result = await _patientRepository.CreatePatientAsync(patientUser, model.Password);
			if (result)
			{
				await _patientRepository.AddToRoleAsync(patientUser, "Patient");
				ViewBag.RegistrationSuccess = true; // Flag for showing a message
				ModelState.Clear(); // Clears the form fields
			}

			ModelState.AddModelError("", "Registration failed.");
			return View(model);
		}

		[HttpGet]
		public IActionResult Login() => View();

		[HttpPost]
		public async Task<IActionResult> Login(PatientLoginViewModel model)
		{
			if (!ModelState.IsValid) return View(model);

			var user = await _patientRepository.GetPatientByEmailAsync(model.Email);
			if (user == null || !await _patientRepository.CheckPasswordAsync(user, model.Password))
			{
				ModelState.AddModelError("", "Invalid login attempt.");
				return View(model);
			}

			await _signInManager.SignInAsync(user, isPersistent: false);

			Console.WriteLine("User logged in successfully!");

			return RedirectToAction("Index", "Dashboard");
		}

		public async Task<IActionResult> Logout()
		{
			await _signInManager.SignOutAsync();
			return RedirectToAction("Index", "Home");
		}

		public async Task<IActionResult> Edit(string id)
		{
			var patient = await _patientRepository.GetPatientByIdAsync(id);
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

			var patient = await _patientRepository.GetPatientByIdAsync(model.Id);
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

			await _patientRepository.UpdatePatientAsync(patient);
			return RedirectToAction("Index");
		}
	}
}
