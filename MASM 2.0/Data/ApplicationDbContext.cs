using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MASM_2._0.Models.Patient;

namespace MASM_2._0.Data
{
	public class ApplicationDbContext : IdentityDbContext<PatientUser>
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
		}
	}
}
