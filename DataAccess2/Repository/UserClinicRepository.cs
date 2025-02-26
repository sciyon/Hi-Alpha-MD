using MASM.DataAccess.Data;
using MASM.Models;
using MASM.Models.Clinic;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class UserClinicRepository : IUserClinicRepository
{
	private readonly ApplicationDbContext _context;

	public UserClinicRepository(ApplicationDbContext context)
	{
		_context = context;
	}

	// Create
	public async Task<bool> CreateUserClinicAsync(UserClinic userClinic)
	{
		try
		{
			_context.UserClinics.Add(userClinic);
			await _context.SaveChangesAsync();
			return true;
		}
		catch
		{
			return false;
		}
	}

	// Update
	public async Task<bool> UpdateUserClinicAsync(UserClinic userClinic)
	{
		try
		{
			_context.UserClinics.Update(userClinic);
			await _context.SaveChangesAsync();
			return true;
		}
		catch
		{
			return false;
		}
	}

	// Delete
	public async Task<bool> DeleteUserClinicAsync(string userId, int clinicId)
	{
		try
		{
			var userClinic = await _context.UserClinics
				.FirstOrDefaultAsync(uc => uc.UserId == userId && uc.ClinicId == clinicId);

			if (userClinic == null)
				return false;

			_context.UserClinics.Remove(userClinic);
			await _context.SaveChangesAsync();
			return true;
		}
		catch
		{
			return false;
		}
	}

	// Get all clinics associated with a user
	public async Task<List<Clinic>> GetClinicsByUserIdAsync(string userId)
	{
		return await _context.UserClinics
			.Where(uc => uc.UserId == userId)
			.Select(uc => uc.Clinic)
			.ToListAsync();
	}

	// Get all users associated with a clinic
	public async Task<List<User>> GetUsersByClinicIdAsync(int clinicId)
	{
		return await _context.UserClinics
			.Where(uc => uc.ClinicId == clinicId)
			.Select(uc => uc.User)
			.ToListAsync();
	}

	//Get All UserClinics
	public async Task<List<UserClinic>> GetAllUserClinicsAsync()
	{
		return await _context.UserClinics
			.Include(uc => uc.Clinic)  // Eagerly load the Clinic
			.Include(uc => uc.User)    // Eagerly load the User
			.ToListAsync();
	}

	//Get UserClinic
	public async Task<UserClinic> GetUserClinicAsync(string userId, int clinicId)
	{
		return await _context.UserClinics
			.Include(uc => uc.Clinic)
			.Include(uc => uc.User)
			.FirstOrDefaultAsync(uc => uc.UserId == userId && uc.ClinicId == clinicId);
	}

}