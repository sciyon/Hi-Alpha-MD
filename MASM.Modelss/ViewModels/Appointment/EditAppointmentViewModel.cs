using MASM.Models;
using MASM.Models.Clinic;
using System;
using System.ComponentModel.DataAnnotations;

namespace MASM.Models.ViewModels.Appointment
{
	public class EditAppointmentViewModel
	{
		public string Id { get; set; } // Keep as string for the UI

		[Display(Name = "Clinic")]
		[Required(ErrorMessage = "Please select a Clinic")]
		public int ClinicId { get; set; } // Change to int

		[Display(Name = "Doctor")]
		[Required(ErrorMessage = "Please select a Doctor")]
		public string DoctorId { get; set; }

		[Display(Name = "Patient")]
		[Required(ErrorMessage = "Please select a Patient")]
		public string PatientId { get; set; }

		[Display(Name = "Status")]
		[Required(ErrorMessage = "Please select a Status")]
		public AppointmentStatus Status { get; set; }

		[Display(Name = "Reason For Visit")]
		[Required(ErrorMessage = "Please input the Reason for the Visit")]
		public string ReasonForVisit { get; set; }

		[Display(Name = "Additional Information")]
		public string AdditionalInfo { get; set; }

		[Required(ErrorMessage = "Please input a valid Date")]
		[Display(Name = "Appointment Date")]
		public DateTime AppointmentDateTime { get; set; }
	}
}