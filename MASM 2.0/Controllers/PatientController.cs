using MASM_2._0.Data;
using MASM_2._0.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

namespace MASM_2._0.Controllers
{
	public class PatientController : Controller
	{
		private readonly ApplicationDbContext _db;
		private readonly IPasswordHasher<Patient> _passwordHasher;

		public PatientController(ApplicationDbContext db)
		{
			_db = db;
			_passwordHasher = new PasswordHasher<Patient>();
		}

		public IActionResult Index()
		{
			List<Patient> patients = _db.Patients.ToList();
			return View(patients);
		}

		public IActionResult Register()
		{
			return View();
		}

		[HttpPost]
		public IActionResult Register(PatientViewModel model)
		{
			// Check if the email already exists
			if (_db.Patients.Any(p => p.Email == model.Email))
			{
				ModelState.AddModelError("Email", "Email is already in use.");
			}

			// Enforce password length manually (extra security)
			if (model.Password.Length < 8)
			{
				ModelState.AddModelError("Password", "Password must be at least 8 characters long.");
			}

			if (!ModelState.IsValid)
			{
				return View(model);
			}

			// Hash the password
			var patient = new Patient
			{
				Email = model.Email,
				MobileNumber = model.MobileNumber
			};

			patient.Password = _passwordHasher.HashPassword(patient, model.Password);

			_db.Patients.Add(patient);
			_db.SaveChanges();

			return RedirectToAction("Index");
		}
		public IActionResult Edit(int? id)
		{
			if (id == null || id == 0)
			{
				return NotFound();
			}

			Patient? patient = _db.Patients.Find(id);
			if (patient == null)
			{
				return NotFound();
			}

			var patientEditViewModel = new ViewModels.PatientEditViewModel
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

			return View(patientEditViewModel);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Edit(ViewModels.PatientEditViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var patient = _db.Patients.Find(model.Id);
			if (patient == null)
			{
				return NotFound();
			}

			// Update patient fields
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

			_db.Patients.Update(patient);
			_db.SaveChanges();

			return RedirectToAction("Index");
		}



		public IActionResult Login()
		{
			return View();
		}
	}
}