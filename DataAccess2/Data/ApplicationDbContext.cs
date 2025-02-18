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
		}
	}
}
