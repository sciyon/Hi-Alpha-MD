using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace MASM.Models
{
	public class PatientUser : IdentityUser
	{
		// Optional fields
		public string? FirstName { get; set; }
		public string? LastName { get; set; }
		public string? Address { get; set; }
		public DateTime? BirthDate { get; set; }
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

		public Sex? Sex { get; set; }
		public string MobileNumber { get; set; }
		public PatientStatus? Status { get; set; } = PatientStatus.Active;
		public CivilStatus? CivilStatus { get; set; }
		public BloodType? BloodType { get; set; }
		public bool EmailVerified { get; set; } = false;


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
		[Display(Name = "A+")] A_Positive,
		[Display(Name = "A-")] A_Negative,
		[Display(Name = "B+")] B_Positive,
		[Display(Name = "B-")] B_Negative,
		[Display(Name = "O+")] O_Positive,
		[Display(Name = "O-")] O_Negative,
		[Display(Name = "AB+")] AB_Positive,
		[Display(Name = "AB-")] AB_Negative
	}
}
