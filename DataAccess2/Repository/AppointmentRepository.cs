using MASM.DataAccess.Data;
using MASM.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MASM.DataAccess.Repositories
{
	public class AppointmentRepository : IAppointmentRepository
	{
		private readonly ApplicationDbContext _context;

		public AppointmentRepository(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<Appointment> GetByIdAsync(Guid id)
		{
			return await _context.Appointments
				.Include(a => a.Doctor)
				.Include(a => a.Patient)
				.Include(a => a.Clinic)
				.FirstOrDefaultAsync(a => a.Id == id);
		}

		public async Task<IEnumerable<Appointment>> GetAllAsync(int clinicId, string range = "monthly", AppointmentStatus? status = null)
		{
			var query = _context.Appointments
				.Include(a => a.Doctor)
				.Include(a => a.Patient)
				.Include(a => a.Clinic)
				.Where(a => a.ClinicId == clinicId);

			if (status.HasValue)
			{
				query = query.Where(a => a.Status == status.Value);
			}

			if (range == "weekly")
			{
				var startOfWeek = DateTime.UtcNow.Date.AddDays(-(int)DateTime.UtcNow.DayOfWeek);
				var endOfWeek = startOfWeek.AddDays(7);
				query = query.Where(a => a.AppointmentDate >= startOfWeek && a.AppointmentDate <= endOfWeek);
			}
			else // Default to monthly
			{
				var startOfMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
				var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);
				query = query.Where(a => a.AppointmentDate >= startOfMonth && a.AppointmentDate <= endOfMonth);
			}

			return await query.ToListAsync();
		}

		public async Task CreateAsync(Appointment appointment)
		{
			_context.Appointments.Add(appointment);
			await _context.SaveChangesAsync();
		}

		public async Task UpdateAsync(Appointment appointment)
		{
			_context.Appointments.Update(appointment);
			await _context.SaveChangesAsync();
		}

		public async Task DeleteAsync(Guid id)
		{
			var appointment = await _context.Appointments.FindAsync(id);
			if (appointment != null)
			{
				_context.Appointments.Remove(appointment);
				await _context.SaveChangesAsync();
			}
		}
	}
}