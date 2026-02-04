using Microsoft.EntityFrameworkCore;
using SGA_Smash.Data;
using SGA_Smash.Repositories;
using SGA_Smash.Services;
using Rotativa.AspNetCore;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSession();

//Registra el contexto de base de datos en el contenedor de dependencias
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

//Referencias a repositorios
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IEmpleadoRepository, EmpleadoRepository>();
builder.Services.AddScoped<IProveedorRepository, ProveedorRepository>();
builder.Services.AddScoped<IPlanillaRepository, PlanillaRepository>();
builder.Services.AddScoped<IVacacionRepository, VacacionRepository>();
builder.Services.AddScoped<IPlanillaReportService, PlanillaReportService>();
builder.Services.AddScoped<IHistorialPagosService, HistorialPagosService>();
builder.Services.AddScoped<IConceptoNominaRepository, ConceptoNominaRepository>();
builder.Services.AddScoped<IPlanillaCalculoService, PlanillaCalculoService>();
builder.Services.AddScoped<IVacacionRepository, VacacionRepository>();
builder.Services.AddScoped<IContratoProveedorRepository, ContratoProveedorRepository>();

// Reservaciones
builder.Services.AddScoped<IReservacionRepository, ReservacionRepository>();
builder.Services.AddScoped<IReservacionReportService, ReservacionReportService>();

// Services
builder.Services.AddScoped<IVacacionPolicyService, VacacionPolicyService>();
builder.Services.AddScoped<INotificationService, EmailNotificationService>();
builder.Services.AddScoped<IVacacionPolicyService, VacacionPolicyService>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseRotativa();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
