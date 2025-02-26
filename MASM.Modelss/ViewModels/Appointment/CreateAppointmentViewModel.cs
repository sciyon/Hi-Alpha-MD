using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MASM.Models.ViewModels.Appointment
{
	public class CreateAppointmentViewModel
	{
		[Required(ErrorMessage = "Reason for visit is required.")]
		[StringLength(500, ErrorMessage = "Reason for visit cannot exceed 500 characters.")]
		public string ReasonForVisit { get; set; }

		[StringLength(2000, ErrorMessage = "Additional information cannot exceed 2000 characters.")]
		public string AdditionalInfo { get; set; }

		[Required(ErrorMessage = "Appointment date and time are required.")]
		[DataType(DataType.DateTime)]
		[Display(Name = "Appointment Date and Time")]
		public DateTime AppointmentDateTime { get; set; }

		[Required(ErrorMessage = "Clinic is required.")]
		public int ClinicId { get; set; }

		[Required(ErrorMessage = "Doctor is required.")]
		public string DoctorId { get; set; }

		[Required(ErrorMessage = "Patient is required.")]
		public string PatientId { get; set; }

		[Required(ErrorMessage = "Status is required.")]
		public AppointmentStatus Status { get; set; }
	}
}