using HospitalAPI.Features.Patients.IServices;
using HospitalAPI.Features.Patients.Services;
using HospitalAPI.Infrastructure.Data;
using HospitalAPI.Infrastructure.Middleware;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// //////////////////////////////////////////
// Framework services
// //////////////////////////////////////////
builder.Services.AddOpenApi();
builder.Services.AddControllersWithViews();

// //////////////////////////////////////////
// Database
// //////////////////////////////////////////
builder.Services.AddDbContext<HospitalDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// //////////////////////////////////////////
// Application services
// //////////////////////////////////////////
builder.Services.AddScoped<IPatientService, PatientService>();

var app = builder.Build();

// //////////////////////////////////////////
// Development only
// //////////////////////////////////////////
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// //////////////////////////////////////////
// Middleware
// //////////////////////////////////////////
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();

// //////////////////////////////////////////
// Endpoints
// //////////////////////////////////////////
app.MapControllers();

app.Run();