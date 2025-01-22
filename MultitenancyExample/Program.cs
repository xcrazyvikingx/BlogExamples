using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MultitenancyExample.Contexts;
using MultitenancyExample.Helpers;

var builder = WebApplication.CreateBuilder(args);

// Add authentication services
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = false,
            ValidIssuer = "your-issuer",
            ValidAudience = "your-audience",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your-secure-key-that-is-long-enough-to-be-secure-and-should-be-stored-in-a-secure-place-and-random-enough-to-be-secure-and-should-be-kept-secret"))
        };
    });
builder.Services.AddAuthorization();
builder.Services.AddControllers();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddHttpContextAccessor();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<MainContext>(options => 
    options.UseSqlite(builder.Configuration.GetConnectionString("MainContext")));

// Setting a default connection string for the shared context, so we can use migrations
builder.Services.AddDbContext<SharedContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("SharedContext")));

builder.Services.AddScoped<ICurrentUserDataAccessor, CurrentUserDataAccessor>();
builder.Services.AddScoped<ISharedContextAccessor, SharedContextAccessor>();



var app = builder.Build();

// Seed the databases
await app.Services.SeedDatabaseAsync();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();
app.UseHttpsRedirection();

app.Run();