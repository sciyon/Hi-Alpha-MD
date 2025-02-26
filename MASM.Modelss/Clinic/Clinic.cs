using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace MASM.Models.Clinic
{
	[Index(nameof(Telephone), IsUnique = true)]
	[Index(nameof(Phone), IsUnique = true)]
	[Index(nameof(Email), IsUnique = true)]
	public class Clinic
	{
		public int Id { get; set; }

		[Required(ErrorMessage = "Clinic Name is required.")]
		[DisplayName("Clinic Name")]
		public string Name { get; set; }

		[Required(ErrorMessage = "Clinic Location is required.")]
		[DisplayName("Clinic Location")]
		public string Location { get; set; }

		[Required(ErrorMessage = "Clinic Telephone Number is required.")]
		[DisplayName("Clinic Telephone Number")]
		[RegularExpression(@"^\d{3}-\d{4}$", ErrorMessage = "Telephone number must be in the format XXX - XXXX.")]
		public string Telephone { get; set; }

		[Required(ErrorMessage = "Clinic Phone Number is required.")]
		[DisplayName("Clinic Phone Number")]
		[RegularExpression(@"^(09|\+?639)\d{9}$|^9\d{9}$", ErrorMessage = "Phone number must start with 09, 9, 63, or +63 and be followed by 9 digits.")]
		public string Phone { get; set; }

		[Required(ErrorMessage = "Clinic Email Address is required.")]
		[DisplayName("Clinic Email Address")]
		[EmailAddress(ErrorMessage = "Invalid Email Address")]
		public string Email { get; set; }

		[Required(ErrorMessage = "Clinic Starting Time is required.")]
		[DisplayName("Clinic Starting Time")]
		public string StartingTime { get; set; } // Keep as string to match your view

		[Required(ErrorMessage = "Clinic Closing Time is required.")]
		[DisplayName("Clinic Closing Time")]
		public string ClosingTime { get; set; } // Keep as string to match your view

		[Required(ErrorMessage = "Status is required.")]
		[DisplayName("Status")]
		public ClinicStatus Status { get; set; }

		public List<UserClinic> UserClinics { get; set; } = new();

		public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			if (!string.IsNullOrEmpty(StartingTime) && !string.IsNullOrEmpty(ClosingTime))
			{
				if (TimeSpan.TryParse(StartingTime, out TimeSpan startTime) && TimeSpan.TryParse(ClosingTime, out TimeSpan endTime))
				{
					if (startTime >= endTime)
					{
						yield return new ValidationResult("Starting Time must be earlier than Closing Time.", new[] { nameof(StartingTime), nameof(ClosingTime) });
					}
				}
				else
				{
					yield return new ValidationResult("Invalid time format.", new[] { nameof(StartingTime), nameof(ClosingTime) });
				}
			}
		}
	}

	public enum ClinicStatus
	{
		Pending,
		Active,
		Closed,
		Restricted,
		Banned
	}
}