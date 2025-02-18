using MASM.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MASM_2._0.Controllers
{
	[Authorize(Roles = "Admin,Doctor")]
	public class UserClinicController : Controller
	{
		private readonly IUserClinicRepository _userClinicRepository;

		public UserClinicController(IUserClinicRepository userClinicRepository)
		{
			_userClinicRepository = userClinicRepository;
		}

		// Create UserClinic
		[HttpPost]
		public async Task<IActionResult> Create(UserClinic userClinic)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			bool success = await _userClinicRepository.CreateUserClinicAsync(userClinic);
			if (success)
				return Ok();

			return BadRequest("Failed to create UserClinic.");
		}

		// Update UserClinic
		[HttpPut]
		public async Task<IActionResult> Update(UserClinic userClinic)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			bool success = await _userClinicRepository.UpdateUserClinicAsync(userClinic);
			if (success)
				return Ok();

			return BadRequest("Failed to update UserClinic.");
		}

		// Delete UserClinic
		[HttpDelete("{userId}/{clinicId}")]
		public async Task<IActionResult> Delete(string userId, int clinicId)
		{
			bool success = await _userClinicRepository.DeleteUserClinicAsync(userId, clinicId);
			if (success)
				return Ok();

			return BadRequest("Failed to delete UserClinic.");
		}

		// Get UserClinic by UserId and ClinicId
		[HttpGet("{userId}/{clinicId}")]
		public async Task<IActionResult> Get(string userId, int clinicId)
		{
			var userClinic = await _userClinicRepository.GetUserClinicAsync(userId, clinicId);
			if (userClinic == null)
				return NotFound();

			return Ok(userClinic);
		}

		// Get all clinics associated with a user
		[HttpGet("clinics/{userId}")]
		public async Task<IActionResult> GetClinicsByUserId(string userId)
		{
			var clinics = await _userClinicRepository.GetClinicsByUserIdAsync(userId);
			return Ok(clinics);
		}

		// Get all users associated with a clinic
		[HttpGet("users/{clinicId}")]
		public async Task<IActionResult> GetUsersByClinicId(int clinicId)
		{
			var users = await _userClinicRepository.GetUsersByClinicIdAsync(clinicId);
			return Ok(users);
		}
	}
}
