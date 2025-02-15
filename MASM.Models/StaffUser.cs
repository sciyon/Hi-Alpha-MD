using Microsoft.AspNetCore.Mvc;

namespace MASM.Models
{
	public class StaffUser : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
