using System.ComponentModel.DataAnnotations;

namespace MASM_2._0.Models
{
	public class Patient
	{
		public int Id { get; set; }

		[Required]
		public string FirstName { get; set; }

		[Required]
		public string LastName { get; set; }

		[Required]
		public string Address { get; set; }

		[Required]
		public DateTime BirthDate { get; set; }

		[Required]
		public string Sex { get; set; }

		[Required]
		public string MobileNumber { get; set; }

		[Required]
		public string Email { get; set; }

		[Required]
		public string Password { get; set; }

		[Required]
		public bool EmailVerified { get; set; }

		[Required]
		public string Status { get; set; }

		[Required]
		public string CivilStatus { get; set; }

		[Required]
		public string BloodType { get; set; }

		[Required]
		public string EmergencyContact { get; set; }

		[Required]
		public string EmergencyContactNumber { get; set; }
	}
}
