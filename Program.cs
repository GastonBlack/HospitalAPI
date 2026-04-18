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

// //////////////////////////////////////////
// Framework services
// //////////////////////////////////////////
builder.Services.AddOpenApi();
builder.Services.AddControllersWithViews();
builder.Services.AddSwaggerGen();
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.SectionName));
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
// Application services
// //////////////////////////////////////////
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IMedicService, MedicService>();
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddScoped<AuthCookieFactory>();
builder.Services.AddScoped<JwtTokenGenerator>();

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
app.UseAuthentication();
app.UseAuthorization();

// //////////////////////////////////////////
// Endpoints
// //////////////////////////////////////////
app.MapControllers();

app.Run();
