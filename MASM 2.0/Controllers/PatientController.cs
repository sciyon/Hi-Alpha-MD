using MASM_2._0.Data;
using MASM_2._0.Models;
using Microsoft.AspNetCore.Mvc;

namespace MASM_2._0.Controllers
{
	public class PatientController : Controller
	{
		private readonly ApplicationDbContext _db;
		public PatientController(ApplicationDbContext db)
		{
			_db = db;
		}
		public IActionResult Index()
		{
			List<Patient> patients = _db.Patients.ToList();
			return View(patients);
		}

		public IActionResult Register()
		{
			return View();
		}
		public IActionResult Login()
		{
			return View();
		}
	}
}