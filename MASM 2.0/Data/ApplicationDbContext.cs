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
	}
}
