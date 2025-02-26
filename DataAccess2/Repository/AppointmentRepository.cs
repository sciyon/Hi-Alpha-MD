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

		public async Task<(IEnumerable<Appointment> Appointments, int TotalCount)> GetAllAsync(
			int clinicId,
			string range = "monthly",
			AppointmentStatus? status = null,
			DateTime? startDate = null,
			int page = 1,
			int pageSize = 10,
			string dateFilter = "future",
			string sortBy = "ascending")
		{
			IQueryable<Appointment> query = _context.Appointments
				.Include(a => a.Doctor)
				.Include(a => a.Patient)
				.Include(a => a.Clinic);

			if (clinicId != 0)
			{
				query = query.Where(a => a.ClinicId == clinicId);
			}

			if (status.HasValue)
			{
				query = query.Where(a => a.Status == status.Value);
			}

			DateTime today = DateTime.UtcNow.Date;

			if (dateFilter == "past")
			{
				query = query.Where(a => a.AppointmentDate < today);
			}
			else // "future" or default
			{
				query = query.Where(a => a.AppointmentDate >= today);
			}

			// Sorting
			if (sortBy == "ascending")
			{
				query = query.OrderBy(a => a.AppointmentDate);
			}
			else // "descending"
			{
				query = query.OrderByDescending(a => a.AppointmentDate);
			}

			DateTime rangeStart, rangeEnd;

			if (range == "weekly")
			{
				// Calculate the start and end of the week
				rangeStart = startDate?.Date ?? DateTime.UtcNow.Date.AddDays(-(int)DateTime.UtcNow.DayOfWeek);
				rangeEnd = rangeStart.AddDays(7);
			}
			else // Default to monthly
			{
				// Calculate the start and end of the month
				rangeStart = startDate?.Date ?? new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
				rangeEnd = rangeStart.AddMonths(1).AddDays(-1);
			}

			query = query.Where(a => a.AppointmentDate >= rangeStart && a.AppointmentDate <= rangeEnd);

			// Get the total count for pagination
			var totalCount = await query.CountAsync();

			// Apply pagination
			var appointments = await query
				.Skip((page - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync();

			return (appointments, totalCount);
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