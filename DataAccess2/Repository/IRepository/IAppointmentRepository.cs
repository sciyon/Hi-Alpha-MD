using MASM.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MASM.DataAccess.Repositories
{
	public interface IAppointmentRepository
	{
		Task<Appointment> GetByIdAsync(Guid id);
		Task<(IEnumerable<Appointment> Appointments, int TotalCount)> GetAllAsync(
			int clinicId,
			string range = "monthly",
			AppointmentStatus? status = null,
			DateTime? startDate = null,
			int page = 1,
			int pageSize = 10,
			string dateFilter = "future",
			string sortBy = "ascending");
		Task CreateAsync(Appointment appointment);
		Task UpdateAsync(Appointment appointment);
		Task DeleteAsync(Guid id);
	}
}