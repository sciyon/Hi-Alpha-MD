using MASM.Models;
using MASM.Models.Clinic;

public interface IUserClinicRepository
{
	Task<bool> CreateUserClinicAsync(UserClinic userClinic);
	Task<bool> UpdateUserClinicAsync(UserClinic userClinic);
	Task<bool> DeleteUserClinicAsync(string userId, int clinicId);
	Task<UserClinic> GetUserClinicAsync(string userId, int clinicId);

	Task<List<Clinic>> GetClinicsByUserIdAsync(string userId);
	Task<List<User>> GetUsersByClinicIdAsync(int clinicId);
}
