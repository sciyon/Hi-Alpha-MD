using System.ComponentModel.DataAnnotations;
using MASM_2._0.Models; // Ensure you reference the enums

namespace MASM_2._0.ViewModels
{
	public class PatientEditViewModel
	{
		public int Id { get; set; }

		[Required(ErrorMessage = "Email is required.")]
		[EmailAddress(ErrorMessage = "Invalid email address.")]
		public string Email { get; set; }

		[Required(ErrorMessage = "Mobile number is required.")]
		[Phone(ErrorMessage = "Invalid phone number.")]
		public string MobileNumber { get; set; }

		public string? FirstName { get; set; }
		public string? LastName { get; set; }
		public string? Address { get; set; }
		public DateTime? BirthDate { get; set; }
		public Sex? Sex { get; set; }
		public CivilStatus? CivilStatus { get; set; }
		public BloodType? BloodType { get; set; }
		public string? EmergencyContact { get; set; }
		public string? EmergencyContactNumber { get; set; }
		public bool EmailVerified { get; set; }
		public PatientStatus? Status { get; set; }
	}
}
