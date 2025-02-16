using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MASM.Models.Clinic
{
	public class Clinic
	{
		public int Id { get; set; }
		public string? Location { get; set; }
		public string? Telephone { get; set; }
		public string? Phone { get; set; }
		public string? Email { get; set; }
		public string? StartingTime { get; set; }
		public string? ClosingTime { get; set; }
		public ClinicStatus? Status { get; set; }

		// Many-to-Many Relationship
		public List<UserClinic> UserClinics { get; set; } = new();
	}

	public enum ClinicStatus
	{
		Active,
		Closed,
		Restricted,
		Banned
	}
}
