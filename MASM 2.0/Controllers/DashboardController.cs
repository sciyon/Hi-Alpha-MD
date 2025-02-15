using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace MASM_2._0.Controllers
{
	[Authorize]
	public class DashboardController : Controller
	{
		public IActionResult Index()
		{
			if (!User.Identity.IsAuthenticated)
			{
				return RedirectToAction("Login", "Patient");
			}

			var user = User;
			ViewBag.UserRole = user.FindFirstValue(ClaimTypes.Role) ?? "Unknown";
			ViewBag.FirstName = user.FindFirstValue("FirstName") ?? "NoFirstName";
			ViewBag.LastName = user.FindFirstValue("LastName") ?? "NoLastName";

			Console.WriteLine($"Authenticated User: {user.Identity.Name}");
			Console.WriteLine($"Role: {ViewBag.UserRole}");
			Console.WriteLine($"First Name: {ViewBag.FirstName}");
			Console.WriteLine($"Last Name: {ViewBag.LastName}");

			return View();
		}
	}
}
