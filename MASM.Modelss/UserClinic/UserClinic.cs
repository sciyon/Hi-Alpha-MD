using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MASM.Models.Clinic; // Make sure this is present!

namespace MASM.Models
{
	public class UserClinic
	{
		[Key]
		[Column(Order = 1)]
		public string UserId { get; set; } = null!;

		[Key]
		[Column(Order = 2)]
		public int ClinicId { get; set; }

		[ForeignKey("UserId")]
		public User User { get; set; } = null!;

		[ForeignKey("ClinicId")]
		public MASM.Models.Clinic.Clinic Clinic { get; set; } = null!; // Fully qualify the Clinic type
	}
}