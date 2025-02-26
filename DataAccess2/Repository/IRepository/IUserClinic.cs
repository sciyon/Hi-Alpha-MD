using MASM.Models;
using MASM.Models.Clinic;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IUserClinicRepository
{
	Task<bool> CreateUserClinicAsync(UserClinic userClinic);
	Task<bool> UpdateUserClinicAsync(UserClinic userClinic);
	Task<bool> DeleteUserClinicAsync(string userId, int clinicId);

	Task<List<Clinic>> GetClinicsByUserIdAsync(string userId);
	Task<List<User>> GetUsersByClinicIdAsync(int clinicId);

	// Get all UserClinics
	Task<List<UserClinic>> GetAllUserClinicsAsync();

	// Get UserClinic using user and clinic id
	Task<UserClinic> GetUserClinicAsync(string userId, int clinicId);

	// NEW: Get staff by clinic ID and optional role
	Task<List<User>> GetStaff(int clinicId, string role = null);
}