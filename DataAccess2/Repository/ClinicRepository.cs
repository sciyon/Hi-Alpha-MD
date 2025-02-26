using MASM.DataAccess.Data;
using MASM.Models.Clinic;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class ClinicRepository : IClinicRepository
{
	private readonly ApplicationDbContext _context;

	public ClinicRepository(ApplicationDbContext context)
	{
		_context = context;
	}

	public async Task<Clinic> GetClinicByIdAsync(string id)
	{
		return await _context.Clinics.FindAsync(int.Parse(id));
	}

	public async Task<bool> CreateClinicAsync(Clinic clinic)
	{
		_context.Clinics.Add(clinic);
		return await _context.SaveChangesAsync() > 0;
	}

	public async Task<bool> UpdateClinicAsync(Clinic clinic)
	{
		_context.Clinics.Update(clinic);
		return await _context.SaveChangesAsync() > 0;
	}

	public async Task<bool> DeleteClinicAsync(string id)
	{
		var clinic = await _context.Clinics.FindAsync(int.Parse(id));
		if (clinic == null) return false;

		_context.Clinics.Remove(clinic);
		return await _context.SaveChangesAsync() > 0;
	}

	public async Task<List<Clinic>> GetAllClinicsAsync()
	{
		return await _context.Clinics.ToListAsync();
	}

	//Search Clinic by Name
	public async Task<List<Clinic>> SearchClinicsByNameAsync(string searchTerm)
	{
		return await _context.Clinics
			.Where(c => c.Name.Contains(searchTerm))
			.ToListAsync();
	}

	//Search Clinic by Email
	public async Task<List<Clinic>> SearchClinicsByEmailAsync(string searchTerm)
	{
		return await _context.Clinics
			.Where(c => c.Email.Contains(searchTerm))
			.ToListAsync();
	}
}