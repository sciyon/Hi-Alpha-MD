using System.ComponentModel.DataAnnotations;

namespace MASM_2._0.ViewModels
{
	public class PatientLoginViewModel
	{
		[Required, EmailAddress]
		public string Email { get; set; }

		[Required, DataType(DataType.Password)]
		public string Password { get; set; }
	}
}
