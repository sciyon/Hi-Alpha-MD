using Microsoft.EntityFrameworkCore;

namespace MASM_2._0.Data
{
	public class ApplicationDbContext : DbContext
	{
		 public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
		{
		}
	}
}
