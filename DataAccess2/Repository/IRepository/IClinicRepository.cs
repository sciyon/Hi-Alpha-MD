using MASM.Models.Clinic;

public interface IClinicRepository
{
	Task<IEnumerable<Clinic>> GetAllClinicsAsync();
	Task<Clinic?> GetClinicByIdAsync(string id);
	Task<bool> CreateClinicAsync(Clinic clinic);
	Task<bool> UpdateClinicAsync(Clinic clinic);
	Task<bool> DeleteClinicAsync(string id);
}