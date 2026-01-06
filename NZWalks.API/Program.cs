// Reference: Udemy Course: Build ASP.NET Core Web API - Scratch To Finish (.NET8 API)

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NZWalks.API.Data;
using NZWalks.API.Mappings;
using NZWalks.API.Middlewares;
using NZWalks.API.Repositories;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Serilog Configuration
var logger = new LoggerConfiguration()
    .WriteTo.Console()  // Log to console
    .WriteTo.File("Logs/NZWalks_log.txt", rollingInterval: RollingInterval.Day) // Log file per day
    //.MinimumLevel.Information()   // Set minimum log level to Information
    .MinimumLevel.Warning()         // Set minimum log level to Warning
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger); // Integrate Serilog with ASP.NET Core Logging

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor(); // To access HttpContext in repositories

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// Swagger Configuration with JWT Authentication
builder.Services.AddSwaggerGen(options =>
{
    // Configure Swagger document
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "NZ Walks API",
        Version = "v1"
    });

    // Add JWT Authentication to Swagger
    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
    });

    // Define the security requirement
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = JwtBearerDefaults.AuthenticationScheme
                },
            Scheme = "Oauth2",
            Name = JwtBearerDefaults.AuthenticationScheme,
            In = ParameterLocation.Header,
        },
        new List<string>()
        }
    });
});

// Configure the DbContext with PostgreSQL database connection
builder.Services.AddDbContext<NZWalksDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("NZWalksConnectionString")));

// Configure the Auth DbContext with PostgreSQL database connection
builder.Services.AddDbContext<NZWalksAuthDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("NZWalksAuthConnectionString")));

// Configure Dependency Injection for Repositories
builder.Services.AddScoped<IRegionRepository, SQLRegionRepository>();
builder.Services.AddScoped<IWalkRepository, SQLWalkRepository>();
builder.Services.AddScoped<ITokenRepository, TokenRepository>();
builder.Services.AddScoped<IImageRepository, LocalImageRepository>();

// AutoMapper Configuration
builder.Services.AddAutoMapper(typeof(AutoMapperProfiles));

// Identity Configuration
builder.Services.AddIdentityCore<IdentityUser>()
    .AddRoles<IdentityRole>()
    .AddTokenProvider<DataProtectorTokenProvider<IdentityUser>>("NZWalks")
    .AddEntityFrameworkStores<NZWalksAuthDbContext>()
    .AddDefaultTokenProviders();

// Identity Options Configuration
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 10;
    options.Password.RequiredUniqueChars = 1;
    //options.User.RequireUniqueEmail = true;
});

// JWT Authentication Configuration
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        });

var app = builder.Build();

// Configure the HTTP request pipeline. //Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Custom Middleware for Global Exception Handling
app.UseMiddleware<ExceptionHanderMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication(); // JWT Authentication Configuration: Add Authentication Middleware
app.UseAuthorization();

app.MapControllers();

app.Run();
