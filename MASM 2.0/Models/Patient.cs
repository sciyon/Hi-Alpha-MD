using System.ComponentModel.DataAnnotations;

namespace MASM_2._0.Models
{
	public class Patient
	{
		public int Id { get; set; }

		// Required fields for sign-up
		[Required(ErrorMessage = "Email is required.")]
		[EmailAddress(ErrorMessage = "Invalid email address.")]
		public string Email { get; set; }

		[Required(ErrorMessage = "Mobile number is required.")]
		[Phone(ErrorMessage = "Invalid phone number.")]
		public string MobileNumber { get; set; }

		[Required(ErrorMessage = "Password is required.")]
		[DataType(DataType.Password)]
		public string Password { get; set; }

		// Optional fields (can be filled later)
		public string? FirstName { get; set; }

		public string? LastName { get; set; }

		public string? Address { get; set; }

		public DateTime? BirthDate { get; set; } // Nullable DateTime

		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

		public Sex? Sex { get; set; } // Enum

		public bool EmailVerified { get; set; } = false; // Default value

		public PatientStatus? Status { get; set; } = PatientStatus.Active;

		public CivilStatus? CivilStatus { get; set; } // Enum

		public BloodType? BloodType { get; set; } // Enum

		public string? EmergencyContact { get; set; }

		public string? EmergencyContactNumber { get; set; }
	}

	// Enum for Sex
	public enum Sex
	{
		Male,
		Female,
		Other
	}

	public enum PatientStatus
	{
		Active,
		Deleted,
		Blocked
	}

	public enum CivilStatus
	{
		Single,
		Married,
		Divorced,
		Widowed
	}

	public enum BloodType
	{
		A,
		B,
		AB,
		O
	}
}