using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MASM.Models;
using MASM.Models.Clinic;

namespace MASM.DataAccess.Data
{
	public class ApplicationDbContext : IdentityDbContext<User>
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
		}

		public DbSet<Clinic> Clinics { get; set; }
		public DbSet<UserClinic> UserClinics { get; set; }
		public DbSet<Appointment> Appointments { get; set; } // Add Appointment table

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			// Unique Constraints for Clinic
			modelBuilder.Entity<Clinic>()
				.HasIndex(c => c.Telephone)
				.IsUnique();

			modelBuilder.Entity<Clinic>()
				.HasIndex(c => c.Phone)
				.IsUnique();

			modelBuilder.Entity<Clinic>()
				.HasIndex(c => c.Email)
				.IsUnique();

			// UserClinic Relationship (Many-to-Many)
			modelBuilder.Entity<UserClinic>()
				.HasKey(uc => new { uc.UserId, uc.ClinicId });

			modelBuilder.Entity<UserClinic>()
				.HasOne(uc => uc.User)
				.WithMany(u => u.UserClinics)
				.HasForeignKey(uc => uc.UserId);

			modelBuilder.Entity<UserClinic>()
				.HasOne(uc => uc.Clinic)
				.WithMany(c => c.UserClinics)
				.HasForeignKey(uc => uc.ClinicId);

			// Appointment Relationships
			modelBuilder.Entity<Appointment>()
				.HasOne(a => a.Doctor)
				.WithMany()
				.HasForeignKey(a => a.DoctorId)
				.OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete

			modelBuilder.Entity<Appointment>()
				.HasOne(a => a.Patient)
				.WithMany()
				.HasForeignKey(a => a.PatientId)
				.OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete

			modelBuilder.Entity<Appointment>()
				.HasOne(a => a.Confirmee)
				.WithMany()
				.HasForeignKey(a => a.ConfirmeeId)
				.OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete

			modelBuilder.Entity<Appointment>()
				.HasOne(a => a.Clinic)
				.WithMany()
				.HasForeignKey(a => a.ClinicId)
				.OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete

			modelBuilder.Entity<Appointment>()
				.HasOne(a => a.CreatedBy)
				.WithMany()
				.HasForeignKey(a => a.CreatedById)
				.OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete

			modelBuilder.Entity<Appointment>()
				.HasOne(a => a.ModifiedBy)
				.WithMany()
				.HasForeignKey(a => a.ModifiedById)
				.OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete
		}
	}
}