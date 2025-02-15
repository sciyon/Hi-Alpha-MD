using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MASM.Models;

namespace MASM.DataAccess.Data
{
	public class ApplicationDbContext : IdentityDbContext<PatientUser>
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
		}
	}
}
