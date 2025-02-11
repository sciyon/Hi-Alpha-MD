using Microsoft.AspNetCore.Mvc;

namespace MASM_2._0.Controllers
{
	public class PatientController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}