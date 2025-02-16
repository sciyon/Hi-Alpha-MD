using MASM.Models;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class UserRepository : IUserRepository
{
	private readonly UserManager<User> _userManager;

	public UserRepository(UserManager<User> userManager)
	{
		_userManager = userManager;
	}

	public async Task<IEnumerable<User>> GetAllUsersAsync()
	{
		return await Task.FromResult(_userManager.Users.ToList());
	}

	public async Task<User?> GetUserByIdAsync(string id)
	{
		return await _userManager.FindByIdAsync(id);
	}

	public async Task<User?> GetUserByEmailAsync(string email)
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
}
