using MASM.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IPatientRepository
{
	Task<IEnumerable<PatientUser>> GetAllPatientsAsync();
	Task<PatientUser?> GetPatientByIdAsync(string id);
	Task<PatientUser?> GetPatientByEmailAsync(string email);
	Task<bool> CreatePatientAsync(PatientUser patient, string password);
	Task<bool> UpdatePatientAsync(PatientUser patient);
	Task<bool> AddToRoleAsync(PatientUser patient, string role);
	Task<bool> CheckPasswordAsync(PatientUser patient, string password);
}
