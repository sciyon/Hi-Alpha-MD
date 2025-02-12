using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MASM_2._0.Models
{
	public class PatientViewModel
	{
		[Required(ErrorMessage = "Email is required.")]
		[EmailAddress(ErrorMessage = "Invalid email address.")]
		[DisplayName("Email")]
		public string Email { get; set; }

		[Required(ErrorMessage = "Mobile number is required.")]
		[Phone(ErrorMessage = "Invalid phone number.")]
		[DisplayName("Mobile Number")]
		public string MobileNumber { get; set; }

		[Required(ErrorMessage = "Password is required.")]
		[DataType(DataType.Password)]
		[DisplayName("Password")]
		public string Password { get; set; }

		[Required(ErrorMessage = "Confirm password is required.")]
		[DataType(DataType.Password)]
		[Compare("Password", ErrorMessage = "Passwords do not match.")]
		[DisplayName("Confirm Password")]
		public string ConfirmPassword { get; set; }
	}
}
