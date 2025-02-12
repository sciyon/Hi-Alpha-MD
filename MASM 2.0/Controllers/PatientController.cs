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

		public IActionResult Login()
		{
			return View();
		}
	}
}