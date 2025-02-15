using MASM.Models;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class PatientRepository : IPatientRepository
{
	private readonly UserManager<PatientUser> _userManager;

	public PatientRepository(UserManager<PatientUser> userManager)
	{
		_userManager = userManager;
	}

	public async Task<IEnumerable<PatientUser>> GetAllPatientsAsync()
	{
		return await Task.FromResult(_userManager.Users.ToList());
	}

	public async Task<PatientUser?> GetPatientByIdAsync(string id)
	{
		return await _userManager.FindByIdAsync(id);
	}

	public async Task<PatientUser?> GetPatientByEmailAsync(string email)
	{
		return await _userManager.FindByEmailAsync(email);
	}

	public async Task<bool> CreatePatientAsync(PatientUser patient, string password)
	{
		var result = await _userManager.CreateAsync(patient, password);
		return result.Succeeded;
	}

	public async Task<bool> UpdatePatientAsync(PatientUser patient)
	{
		var result = await _userManager.UpdateAsync(patient);
		return result.Succeeded;
	}

	public async Task<bool> AddToRoleAsync(PatientUser patient, string role)
	{
		var result = await _userManager.AddToRoleAsync(patient, role);
		return result.Succeeded;
	}

	public async Task<bool> CheckPasswordAsync(PatientUser patient, string password)
	{
		return await _userManager.CheckPasswordAsync(patient, password);
	}
}
