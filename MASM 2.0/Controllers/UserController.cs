using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using MASM.Models;
using MASM.Models.ViewModels;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace MASM.Controllers
{
	public class UserController : Controller
	{
		private readonly IUserRepository _userRepository;
		private readonly SignInManager<User> _signInManager;
		private readonly UserManager<User> _userManager; // Add UserManager
		private readonly RoleManager<IdentityRole> _roleManager;

		public UserController(IUserRepository userRepository, SignInManager<User> signInManager, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
		{
			_userRepository = userRepository;
			_signInManager = signInManager;
			_userManager = userManager; // Initialize UserManager
			_roleManager = roleManager;
		}

		public async Task<IActionResult> Edit(Guid id) // Ensure parameter matches route
		{
			var user = await _userRepository.GetUserByIdAsync(id.ToString()); // Convert Guid to string
			if (user == null)
			{
				return NotFound();
			}

			var model = new EditViewModel
			{
				Id = user.Id, // Ensure EditViewModel.Id is a string, not Guid
				Email = user.Email,
				MobileNumber = user.MobileNumber,
				FirstName = user.FirstName,
				LastName = user.LastName,
				Address = user.Address,
				BirthDate = user.BirthDate,
				Sex = user.Sex,
				CivilStatus = user.CivilStatus,
				BloodType = user.BloodType,
				EmergencyContact = user.EmergencyContact,
				EmergencyContactNumber = user.EmergencyContactNumber,
				EmailVerified = user.EmailVerified,
				Role = user.Role,
				Status = user.Status  // Ensure a valid default value
			};

			return View(model);
		}

		[HttpPost]
		public async Task<IActionResult> Edit(EditViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var user = await _userRepository.GetUserByIdAsync(model.Id);
			if (user == null)
			{
				return NotFound();
			}

			// Update user details
			user.Email = model.Email;
			user.MobileNumber = model.MobileNumber;
			user.FirstName = model.FirstName;
			user.LastName = model.LastName;
			user.Address = model.Address;
			user.BirthDate = model.BirthDate;
			user.Sex = model.Sex;
			user.CivilStatus = model.CivilStatus;
			user.BloodType = model.BloodType;
			user.EmergencyContact = model.EmergencyContact;
			user.EmergencyContactNumber = model.EmergencyContactNumber;
			user.EmailVerified = model.EmailVerified;
			user.Status = model.Status;

			// Ensure role exists before assigning
			if (!await _roleManager.RoleExistsAsync(model.Role.ToString()))
			{
				ModelState.AddModelError("Role", "Selected role does not exist.");
				return View(model);
			}

			// Remove old roles first
			var currentRoles = await _userManager.GetRolesAsync(user);
			var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);

			if (!removeResult.Succeeded)
			{
				ModelState.AddModelError("Role", "Failed to remove existing roles.");
				return View(model);
			}

			// Add new role
			var addResult = await _userManager.AddToRoleAsync(user, model.Role.ToString());
			if (!addResult.Succeeded)
			{
				ModelState.AddModelError("Role", "Failed to update user role.");
				return View(model);
			}

			// Explicitly update the user's Role property
			user.Role = model.Role;
			await _userRepository.UpdateUserAsync(user);

			// Refresh authentication session only if the edited user is the currently logged-in user
			var currentUserId = _userManager.GetUserId(User); // Get the ID of the currently logged-in user
			if (user.Id == currentUserId)
			{
				await _signInManager.RefreshSignInAsync(user);
			}

			return RedirectToAction("Index");
		}



		public async Task<IActionResult> Index()
		{
			var users = await _userRepository.GetAllUsersAsync();
			return View(users);
		}

		public IActionResult Register() => View();

		[HttpPost]
		public async Task<IActionResult> Register(RegisterViewModel model)
		{
			if (!ModelState.IsValid) return View(model);

			var user = new User
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
				Role = model.Role,  // Role should be set via the form
				CreatedAt = DateTime.UtcNow
			};

			var result = await _userRepository.CreateUserAsync(user, model.Password);
			if (result)
			{
				return RedirectToAction("Index");
			}

			ModelState.AddModelError("", "Registration failed.");
			return View(model);
		}

		public async Task<IActionResult> Login() => View();

		[HttpPost]
		public async Task<IActionResult> Login(LoginViewModel model)
		{
			if (!ModelState.IsValid) return View(model);

			var user = await _userRepository.GetUserByEmailAsync(model.Email);
			if (user == null || !await _userRepository.CheckPasswordAsync(user, model.Password))
			{
				ModelState.AddModelError("", "Invalid login attempt.");
				return View(model);
			}

			await _signInManager.SignInAsync(user, isPersistent: false);

			return RedirectToAction("Index", "Dashboard");
		}

		public async Task<IActionResult> Logout()
		{
			await _signInManager.SignOutAsync();
			return RedirectToAction("Index", "Home");
		}
	}
}
