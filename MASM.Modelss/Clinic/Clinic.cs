using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MASM.Models.Clinic
{
	[Index(nameof(Telephone), IsUnique = true)]
	[Index(nameof(Phone), IsUnique = true)]
	[Index(nameof(Email), IsUnique = true)]
	public class Clinic
	{
		public int Id { get; set; }

		[Required]
		[DisplayName("Clinic Name")]
		public string Name { get; set; }

		[Required]
		[DisplayName("Clinic Location")]
		public string Location { get; set; }

		[Required]
		[DisplayName("Clinic Telephone Number")]
		public string Telephone { get; set; }

		[Required]
		[DisplayName("Clinic Phone Number")]
		public string Phone { get; set; }

		[Required]
		[DisplayName("Clinic Email Address")]
		public string Email { get; set; }

		[Required]
		[DisplayName("Clinic Starting Time")]
		public string StartingTime { get; set; }

		[Required]
		[DisplayName("Clinic Closing Time")]
		public string ClosingTime { get; set; }

		[Required]
		[DisplayName("Status")]
		public ClinicStatus Status { get; set; }

		public List<UserClinic> UserClinics { get; set; } = new();
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