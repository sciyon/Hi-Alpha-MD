using MASM_2._0.Models.Patient;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
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
				await _userManager.AddToRoleAsync(patientUser, "Patient"); 
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

		public async Task<IActionResult> Login(PatientLoginViewModel model)
		{
			if (!ModelState.IsValid) return View(model);

			var user = await _userManager.FindByEmailAsync(model.Email);
			if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
			{
				ModelState.AddModelError("", "Invalid login attempt.");
				return View(model);
			}

			// Get the user's roles (returns a list)
			var roles = await _userManager.GetRolesAsync(user);
			var role = roles.FirstOrDefault() ?? "Patient"; // Default to "Patient" if no role is assigned

			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
				new Claim(ClaimTypes.Role, role), // Store role correctly
				new Claim("FirstName", user.FirstName ?? ""),
				new Claim("LastName", user.LastName ?? "")
			};

			var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
			var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

			await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

			return RedirectToAction("Dashboard", "Index");
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
