using Microsoft.EntityFrameworkCore;
using MASM_2._0.Models;

namespace MASM_2._0.Data
{
	public class ApplicationDbContext : DbContext
	{
		 public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
		{

		}

		public DbSet<Patient> Patients { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Patient>().HasData(
				new Patient
				{
					Id = 1,
					Email = "antepuesto.erwin@gmail.com",
					MobileNumber = "09123456789",
					Password = "password"
				},
				new Patient
				{
					Id = 2,
					Email = "antepuesto.belle@gmail.com",
					MobileNumber = "09123456788",
					Password = "password"
				},
				new Patient
				{
					Id = 3,
					Email = "antepuesto.aubrey@gmail.com",
					MobileNumber = "09123456787",
					Password = "password"
				}
			);
			{
				modelBuilder.Entity<Patient>()
					.HasIndex(p => p.Email)
					.IsUnique(); // Enforce unique constraint
			}
		}
	}
}
