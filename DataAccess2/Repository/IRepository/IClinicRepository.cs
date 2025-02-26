using MASM.Models.Clinic;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IClinicRepository
{
	Task<Clinic> GetClinicByIdAsync(string id);
	Task<bool> CreateClinicAsync(Clinic clinic);
	Task<bool> UpdateClinicAsync(Clinic clinic);
	Task<bool> DeleteClinicAsync(string id);
	Task<List<Clinic>> GetAllClinicsAsync();

	//Search Clinics by Name
	Task<List<Clinic>> SearchClinicsByNameAsync(string searchTerm);

	//Search Clinics by Email
	Task<List<Clinic>> SearchClinicsByEmailAsync(string searchTerm);
}