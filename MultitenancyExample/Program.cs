using Microsoft.EntityFrameworkCore;
using MultitenancyExample.Contexts;
using MultitenancyExample.Helpers;

var builder = WebApplication.CreateBuilder(args);

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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();