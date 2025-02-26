using MASM.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MASM.DataAccess.Repositories
{
	public interface IAppointmentRepository
	{
		Task<Appointment> GetByIdAsync(Guid id);
		Task<IEnumerable<Appointment>> GetAllAsync(int clinicId, string range = "monthly", AppointmentStatus? status = null);
		Task CreateAsync(Appointment appointment);
		Task UpdateAsync(Appointment appointment);
		Task DeleteAsync(Guid id);
	}
}