using System.ComponentModel.DataAnnotations;

namespace MASM.Models
{
	public class PatientLoginViewModel
	{
		[Required, EmailAddress]
		public string Email { get; set; }

		[Required, DataType(DataType.Password)]
		public string Password { get; set; }
	}
}
