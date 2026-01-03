// Reference: Udemy Course: Build ASP.NET Core Web API - Scratch To Finish (.NET8 API)

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NZWalks.API.Data;
using NZWalks.API.Mappings;
using NZWalks.API.Repositories;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure the DbContext with PostgreSQL database connection
builder.Services.AddDbContext<NZWalksDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("NZWalksConnectionString")));

// Configure the Auth DbContext with PostgreSQL database connection
builder.Services.AddDbContext<NZWalksAuthDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("NZWalksAuthConnectionString")));

// Configure Dependency Injection for Repositories
builder.Services.AddScoped<IRegionRepository, SQLRegionRepository>();
builder.Services.AddScoped<IWalkRepository, SQLWalkRepository>();

// AutoMapper Configuration
builder.Services.AddAutoMapper(typeof(AutoMapperProfiles));

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

app.UseHttpsRedirection();

app.UseAuthentication(); // JWT Authentication Configuration: Add Authentication Middleware
app.UseAuthorization();

app.MapControllers();

app.Run();
