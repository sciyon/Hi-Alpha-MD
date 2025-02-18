using MASM.DataAccess.Data;
using MASM.Models.Clinic;
using Microsoft.EntityFrameworkCore;
using System;

public class ClinicRepository : IClinicRepository
{
	private readonly ApplicationDbContext _context;

	public ClinicRepository(ApplicationDbContext context)
	{
		_context = context;
	}

	public async Task<IEnumerable<Clinic>> GetAllClinicsAsync()
	{
		return await _context.Clinics.ToListAsync();
	}

	public async Task<Clinic?> GetClinicByIdAsync(string id)
	{
		if (!int.TryParse(id, out int clinicId))
			return null;

		return await _context.Clinics.FindAsync(clinicId);
	}

	public async Task<bool> CreateClinicAsync(Clinic clinic)
	{
		// Ensure uniqueness before adding
		if (await _context.Clinics.AnyAsync(c =>
			c.Telephone == clinic.Telephone ||
			c.Phone == clinic.Phone ||
			c.Email == clinic.Email))
		{
			return false; // Duplicate entry found
		}

		_context.Clinics.Add(clinic);
		return await _context.SaveChangesAsync() > 0;
	}

	public async Task<bool> UpdateClinicAsync(Clinic clinic)
	{
		var existingClinic = await _context.Clinics.FindAsync(clinic.Id);
		if (existingClinic == null)
			return false;

		// Check if new values are unique
		if (await _context.Clinics.AnyAsync(c =>
			(c.Telephone == clinic.Telephone || c.Phone == clinic.Phone || c.Email == clinic.Email)
			&& c.Id != clinic.Id)) // Exclude itself
		{
			return false; // Duplicate found
		}

		_context.Entry(existingClinic).CurrentValues.SetValues(clinic);
		return await _context.SaveChangesAsync() > 0;
	}

	public async Task<bool> DeleteClinicAsync(string id)
	{
		if (!int.TryParse(id, out int clinicId))
			return false;

		var clinic = await _context.Clinics.FindAsync(clinicId);
		if (clinic == null)
			return false;

		_context.Clinics.Remove(clinic);
		return await _context.SaveChangesAsync() > 0;
	}
}
