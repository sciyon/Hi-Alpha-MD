using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MASM_2._0.Controllers
{
	public class DashboardController : Controller
	{
		public IActionResult Index()
		{
			var userRole = User.FindFirstValue(ClaimTypes.Role) ?? "Patient";
			var firstName = User.FindFirstValue("FirstName") ?? "Firstname";
			var lastName = User.FindFirstValue("LastName") ?? "Lastname";

			ViewBag.UserRole = userRole;
			ViewBag.FirstName = firstName;
			ViewBag.LastName = lastName;

			return View();
		}
	}
}
