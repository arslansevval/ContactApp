using ContactApp.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using FluentValidation;
using FluentValidation.AspNetCore;
using ContactApp.Infrastructure.Data; // AppDbContext burada tanımlıysa
using ContactApp.Core.Interfaces;      // IUnitOfWork
using ContactApp.Infrastructure.Repositories; // UnitOfWork
using ContactApp.Application.Services;  // EmployeeService, ContactInfoService, CompanyService
using ContactApp.Application.Mapping;
using ContactApp.Infrastructure.Services;
using ContactApp.Application.Validators;

var builder = WebApplication.CreateBuilder(args);

// PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// In-Memory Cache
builder.Services.AddMemoryCache();

// FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<EmployeeValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CompanyValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<ContactInfoValidator>();
// Otomatik validation ve client-side adapter ekleme
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();

// Validator sınıflarını assembly’den ekle
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

//AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ContactApp API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "JWT token'ınızı giriniz",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});
// UnitOfWork ve diğer servisler - dependency injection
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<EmployeeService>();
builder.Services.AddScoped<ContactInfoService>();
builder.Services.AddScoped<CompanyService>();
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<AuthService>();

// JWT
var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrEmpty(jwtKey))
{
    throw new Exception("JWT Key configuration is missing!");
}
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtSettings = builder.Configuration.GetSection("Jwt");
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!))
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddControllers();

var app = builder.Build();

// Middleware
app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
