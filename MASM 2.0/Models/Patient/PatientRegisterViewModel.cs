using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MASM_2._0.Models.Patient
{
	public class PatientRegisterViewModel
	{
		[Required]
		[EmailAddress]
		[DisplayName("Email Address")]
		public string Email { get; set; }

		[Required]
		[Phone]
		[DisplayName("Mobile Number")]
		public string MobileNumber { get; set; }

		[Required]
		[DataType(DataType.Password)]
		[DisplayName("Password")]
		public string Password { get; set; }

		[Required]
		[DataType(DataType.Password)]
		[DisplayName("Confirm Password")]
		[Compare("Password", ErrorMessage = "Passwords do not match.")]
		public string ConfirmPassword { get; set; }

		public string? FirstName { get; set; }
		public string? LastName { get; set; }
		public string? Address { get; set; }
		public DateTime? BirthDate { get; set; }
		public Sex? Sex { get; set; }
		public CivilStatus? CivilStatus { get; set; }
		public BloodType? BloodType { get; set; }
		public string? EmergencyContact { get; set; }
		public string? EmergencyContactNumber { get; set; }
	}
}
