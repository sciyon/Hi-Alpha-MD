using MASM.Models;
using MASM.Models.Clinic;
using System;
using System.ComponentModel.DataAnnotations;

namespace MASM.Models.ViewModels.Appointment
{
	public class ViewAppointmentViewModel
	{
		public string Id { get; set; } // Change to string for the UI
		[Display(Name = "Clinic")]
		public string ClinicName { get; set; }

		[Display(Name = "Doctor")]
		public string DoctorName { get; set; }

		[Display(Name = "Patient")]
		public string PatientName { get; set; }

		[Display(Name = "Status")]
		public AppointmentStatus Status { get; set; }

		[Display(Name = "Reason For Visit")]
		public string ReasonForVisit { get; set; }

		[Display(Name = "Additional Information")]
		public string AdditionalInfo { get; set; }

		[Display(Name = "Appointment Date")]
		public DateTime AppointmentDateTime { get; set; }
	}
}