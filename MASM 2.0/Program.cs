using MASM.DataAccess.Data;
using MASM.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add database context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Identity Services
builder.Services.AddIdentity<PatientUser, IdentityRole>()
	.AddEntityFrameworkStores<ApplicationDbContext>()
	.AddDefaultTokenProviders();

//  Fix: Add Cookie Authentication Scheme
builder.Services.ConfigureApplicationCookie(options =>
{
	options.LoginPath = "/Patient/Login";  // Redirect unauthorized users to login
	options.AccessDeniedPath = "/Home/AccessDenied"; // Redirect if access is denied
	options.Cookie.HttpOnly = true;
	options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
});

builder.Services.AddScoped<IPatientRepository, PatientRepository>();


//  Fix: Explicitly Add Authentication Middleware
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
	.AddCookie(options =>
	{
		options.LoginPath = "/Patient/Login";
		options.AccessDeniedPath = "/Home/AccessDenied";
	});


var app = builder.Build();

// Ensure roles are created at startup
using (var scope = app.Services.CreateScope())
{
	var services = scope.ServiceProvider;
	var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
	await EnsureRolesAsync(roleManager);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Add Authentication & Authorization
app.UseAuthentication(); //  Ensure authentication middleware is enabled!
app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

// Method to Ensure Roles Exist
async Task EnsureRolesAsync(RoleManager<IdentityRole> roleManager)
{
	string[] roles = { "Patient", "Admin", "Doctor" };

	foreach (var role in roles)
	{
		if (!await roleManager.RoleExistsAsync(role))
		{
			await roleManager.CreateAsync(new IdentityRole(role));
		}
	}
}
