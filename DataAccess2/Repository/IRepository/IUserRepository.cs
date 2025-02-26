using MASM.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IUserRepository
{
	Task<User> GetUserByIdAsync(string id);
	Task<User> GetUserByEmailAsync(string email);
	Task<bool> CreateUserAsync(User user, string password);
	Task<bool> UpdateUserAsync(User user);
	Task<bool> CheckPasswordAsync(User user, string password);
	Task<List<User>> GetAllUsersAsync();
	Task<List<User>> SearchUsersByNameAndRoleAsync(string searchTerm, string role);
	Task<List<User>> SearchUsersByEmailAndRoleAsync(string searchTerm, string role);

	// NEW: Get all patients
	Task<List<User>> GetPatients();
}