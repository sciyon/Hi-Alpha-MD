using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MASM.Models.Clinic;

namespace MASM.Models
{
	public class Appointment
	{
		public Guid Id { get; set; } = Guid.NewGuid(); // Unique identifier for the appointment

		// Foreign key to the doctor (User with Role.Doctor)
		[Required]
		public string DoctorId { get; set; }
		public User Doctor { get; set; } // Navigation property

		// Foreign key to the patient (User with Role.Patient)
		[Required]
		public string PatientId { get; set; }
		public User Patient { get; set; } // Navigation property

		// Foreign key to the confirmee (User with Role.Assistant or Role.Doctor)
		public string ConfirmeeId { get; set; }
		public User Confirmee { get; set; } // Navigation property

		// Foreign key to the clinic
		[Required]
		public int ClinicId { get; set; }
		public MASM.Models.Clinic.Clinic Clinic { get; set; } // Navigation property

		// Appointment status
		[Required]
		public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;

		// Reason for the visit
		[Required]
		[StringLength(500)]
		public string ReasonForVisit { get; set; }

		// Additional information
		[StringLength(2000)]
		public string AdditionalInfo { get; set; }

		// Images (stored as file paths or URLs)
		public List<string> Images { get; set; } = new();

		// Date and time of the appointment
		[Required]
		public DateTime AppointmentDate { get; set; }

		// Timestamps
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
		public string CreatedById { get; set; } // Foreign key to the user who created the appointment
		public User CreatedBy { get; set; } // Navigation property

		public DateTime? ModifiedOn { get; set; }
		public string ModifiedById { get; set; } // Foreign key to the user who modified the appointment
		public User ModifiedBy { get; set; } // Navigation property
	}

	public enum AppointmentStatus
	{
		Pending,
		Confirmed,
		Cancelled,
		Denied
	}
}