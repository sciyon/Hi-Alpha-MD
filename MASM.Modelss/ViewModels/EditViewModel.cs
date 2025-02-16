using System.ComponentModel.DataAnnotations;

namespace MASM.Models.ViewModels
{
	public class EditViewModel
	{
		public string Id { get; set; }  // Ensure it's a string
		public string Email { get; set; }
		public string MobileNumber { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Address { get; set; }
		public DateTime? BirthDate { get; set; }
		public Sex? Sex { get; set; }
		public CivilStatus? CivilStatus { get; set; }
		public BloodType? BloodType { get; set; }
		public string EmergencyContact { get; set; }
		public string EmergencyContactNumber { get; set; }
		public bool EmailVerified { get; set; }
		public UserStatus Status { get; set; } // Ensure it matches the fix above
	}
}
