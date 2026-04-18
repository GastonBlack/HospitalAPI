using HospitalAPI.Features.Medics.IServices;
using HospitalAPI.Features.Medics.Services;
using HospitalAPI.Features.Patients.IServices;
using HospitalAPI.Features.Patients.Services;
using HospitalAPI.Features.Tickets.IServices;
using HospitalAPI.Features.Tickets.Services;
using HospitalAPI.Infrastructure.Data;
using HospitalAPI.Infrastructure.Middleware;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// //////////////////////////////////////////
// Framework services
// //////////////////////////////////////////
builder.Services.AddOpenApi();
builder.Services.AddControllersWithViews();
builder.Services.AddSwaggerGen();

// //////////////////////////////////////////
// Database
// //////////////////////////////////////////
builder.Services.AddDbContext<HospitalDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// //////////////////////////////////////////
// Application services
// //////////////////////////////////////////
builder.Services.AddScoped<IMedicService, MedicService>();
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<ITicketService, TicketService>();

var app = builder.Build();

// //////////////////////////////////////////
// Development only
// //////////////////////////////////////////
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
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