using MASM.DataAccess.Data;
using MASM.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class UserRepository : IUserRepository
{
	private readonly ApplicationDbContext _context;
	private readonly UserManager<User> _userManager;

	public UserRepository(ApplicationDbContext context, UserManager<User> userManager)
	{
		_context = context;
		_userManager = userManager;
	}

	public async Task<User> GetUserByIdAsync(string id)
	{
		return await _userManager.FindByIdAsync(id);
	}

	public async Task<User> GetUserByEmailAsync(string email)
	{
		return await _userManager.FindByEmailAsync(email);
	}

	public async Task<bool> CreateUserAsync(User user, string password)
	{
		var result = await _userManager.CreateAsync(user, password);
		return result.Succeeded;
	}

	public async Task<bool> UpdateUserAsync(User user)
	{
		var result = await _userManager.UpdateAsync(user);
		return result.Succeeded;
	}

	public async Task<bool> CheckPasswordAsync(User user, string password)
	{
		return await _userManager.CheckPasswordAsync(user, password);
	}

	public async Task<List<User>> GetAllUsersAsync()
	{
		return await _userManager.Users.ToListAsync();
	}

	public async Task<List<User>> SearchUsersByNameAndRoleAsync(string searchTerm, string role)
	{
		return await _userManager.Users
			.Where(u => (u.FirstName.Contains(searchTerm) || u.LastName.Contains(searchTerm)) && _userManager.GetRolesAsync(u).Result.Contains(role))
			.ToListAsync();
	}

	public async Task<List<User>> SearchUsersByEmailAndRoleAsync(string searchTerm, string role)
	{
		return await _userManager.Users
			.Where(u => u.Email.Contains(searchTerm) && _userManager.GetRolesAsync(u).Result.Contains(role))
			.ToListAsync();
	}
}