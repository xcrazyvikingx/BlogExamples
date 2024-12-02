using Microsoft.EntityFrameworkCore;
using MultitenancyExample.Contexts;
using MultitenancyExample.Helpers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<MainContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("MainContext"));
});

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