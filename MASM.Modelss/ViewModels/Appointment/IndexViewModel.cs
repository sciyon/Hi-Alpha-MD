using System;
using System.ComponentModel.DataAnnotations;

namespace MASM.Models.ViewModels.Appointment
{
	public class AppointmentIndexViewModel
	{
		public int ClinicId { get; set; } = 0;
		public AppointmentStatus? Status { get; set; } = null;
		public DateTime? StartDate { get; set; } = null;
		public string DateFilter { get; set; } = "future";
		public string SortBy { get; set; } = "ascending";
		public int Page { get; set; } = 1;
		public int PageSize { get; set; } = 10;
	}
}