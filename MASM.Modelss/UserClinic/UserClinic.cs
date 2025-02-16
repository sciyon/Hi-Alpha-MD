using MASM.Models;
using MASM.Models.Clinic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MASM.Models
{
	public class UserClinic
	{
		public string UserId { get; set; } = null!; // FK to User
		public int ClinicId { get; set; } // FK to Clinic

		// Navigation Properties
		public User User { get; set; } = null!;
		public MASM.Models.Clinic.Clinic Clinic { get; set; } = null!;
	}
}
