using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using SGA_Smash.Data;
using SGA_Smash.Repositories;
using SGA_Smash.Services;

var builder = WebApplication.CreateBuilder(args);

// DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Repositorios
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IEmpleadoRepository, EmpleadoRepository>();
builder.Services.AddScoped<IProveedorRepository, ProveedorRepository>();
builder.Services.AddScoped<IPlanillaRepository, PlanillaRepository>();
builder.Services.AddScoped<IVacacionRepository, VacacionRepository>();
builder.Services.AddScoped<IConceptoNominaRepository, ConceptoNominaRepository>();
builder.Services.AddScoped<IContratoProveedorRepository, ContratoProveedorRepository>();

// Servicios

builder.Services.AddScoped<IHistorialPagosService, HistorialPagosService>();
builder.Services.AddScoped<PlanillaCalculoService>();
builder.Services.AddScoped<IVacacionPolicyService, VacacionPolicyService>();
builder.Services.AddScoped<INotificationService, EmailNotificationService>();

builder.Services.AddScoped<IPlanillaReportService, PlanillaReportService>();

builder.Services.AddScoped<ReportService>();  

// MVC
builder.Services.AddControllersWithViews();

// === Cookie Authentication ===
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(opt =>
    {
        opt.LoginPath = "/Auth/Login";       // ajusta si tu login es otro
        opt.AccessDeniedPath = "/Auth/Denied";
        opt.SlidingExpiration = true;
        opt.ExpireTimeSpan = TimeSpan.FromHours(8);
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// IMPORTANTE: Autenticación antes de autorización
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
