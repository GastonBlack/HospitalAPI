using System.Text;
using HospitalAPI.Features.Auth.IServices;
using HospitalAPI.Features.Auth.Services;
using HospitalAPI.Features.Medics.IServices;
using HospitalAPI.Features.Medics.Services;
using HospitalAPI.Features.Patients.IServices;
using HospitalAPI.Features.Patients.Services;
using HospitalAPI.Features.Tickets.IServices;
using HospitalAPI.Features.Tickets.Services;
using HospitalAPI.Infrastructure.Auth;
using HospitalAPI.Infrastructure.Data;
using HospitalAPI.Infrastructure.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
var jwtOptions = builder.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()
    ?? throw new InvalidOperationException("La configuracion JWT es obligatoria.");
const string FrontendCorsPolicy = "FrontendCorsPolicy";

// //////////////////////////////////////////
// Framework Services
// //////////////////////////////////////////
builder.Services.AddOpenApi();
builder.Services.AddControllersWithViews();
builder.Services.AddSwaggerGen();

// //////////////////////////////////////////
// Options
// //////////////////////////////////////////
builder.Services.Configure<AdminSeedOptions>(builder.Configuration.GetSection(AdminSeedOptions.SectionName));
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.SectionName));

// //////////////////////////////////////////
// Cors
// //////////////////////////////////////////
builder.Services.AddCors(options =>
{
    options.AddPolicy(FrontendCorsPolicy, policy =>
    {
        policy.WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// //////////////////////////////////////////
// Authentication
// //////////////////////////////////////////
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidAudience = jwtOptions.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key)),
            ClockSkew = TimeSpan.Zero
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                context.Token = context.Request.Cookies[AuthConstants.AccessTokenCookieName];
                return Task.CompletedTask;
            }
        };
    });
builder.Services.AddAuthorization();

// //////////////////////////////////////////
// Database
// //////////////////////////////////////////
builder.Services.AddDbContext<HospitalDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// //////////////////////////////////////////
// Application Services
// //////////////////////////////////////////
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IMedicService, MedicService>();
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddScoped<AuthCookieFactory>();
builder.Services.AddScoped<JwtTokenGenerator>();

var app = builder.Build();
await app.SeedAdminAsync();

// //////////////////////////////////////////
// Development Only
// //////////////////////////////////////////
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

// //////////////////////////////////////////
// Request Pipeline
// //////////////////////////////////////////
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseCors(FrontendCorsPolicy);
app.UseAuthentication();
app.UseAuthorization();

// //////////////////////////////////////////
// Endpoints
// //////////////////////////////////////////
app.MapControllers();

app.Run();
