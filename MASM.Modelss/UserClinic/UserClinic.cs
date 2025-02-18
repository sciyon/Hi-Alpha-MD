using MASM.Models;
using MASM.Models.Clinic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MASM.Models
{
	public class UserClinic
	{
		public string UserId { get; set; } = null!;
		public int ClinicId { get; set; } 

		public User User { get; set; } = null!;
		public MASM.Models.Clinic.Clinic Clinic { get; set; } = null!;
	}
}
