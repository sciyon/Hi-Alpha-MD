using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace MASM.Models
{
	public class User : IdentityUser
	{
		public string? FirstName { get; set; }
		public string? LastName { get; set; }
		public string? Address { get; set; }
		public DateTime? BirthDate { get; set; }
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
		public Sex? Sex { get; set; }
		public CivilStatus? CivilStatus { get; set; }
		public BloodType? BloodType { get; set; }
		public string MobileNumber { get; set; }
		public string? EmergencyContact { get; set; }
		public string? EmergencyContactNumber { get; set; }
		public bool EmailVerified { get; set; } = false;

		public Role Role { get; set; } = Role.Patient; // Default to Patient
		public UserStatus Status { get; set; } = UserStatus.Active;

		// Many-to-Many Relationship
		public List<UserClinic> UserClinics { get; set; } = new(); // Navigation property
	}

	public enum Sex
	{
		Male,
		Female,
		Other
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

	public enum Role
	{
		Patient,
		Doctor,
		Assistant,
		Admin
	}

	public enum UserStatus
	{
		Active,
		Deleted,
		Blocked
	}
}
