using MASM.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IUserRepository
{
	Task<IEnumerable<User>> GetAllUsersAsync();
	Task<User?> GetUserByIdAsync(string id);
	Task<User?> GetUserByEmailAsync(string email);
	Task<bool> CreateUserAsync(User user, string password);
	Task<bool> UpdateUserAsync(User user);
	Task<bool> CheckPasswordAsync(User user, string password);
}
