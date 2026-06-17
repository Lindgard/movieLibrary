using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using movieLibraryService.Services;
using movieLibraryAPI.Data;
using movieLibraryAPI.Data.Repositories;
using movieLibraryService.Services.Security;
using movieLibraryService.Services.Security.Interfaces;
using movieLibraryAPI.Data.Repositories.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter(namingPolicy: null, allowIntegerValues: false));
        });

builder.Services.AddDbContext<MovieLibraryDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddScoped<MovieService>();
builder.Services.AddScoped<TvShowService>();
builder.Services.AddScoped<HashTokens>();
builder.Services.AddScoped<IPasswordPolicyValidator, PasswordPolicyValidator>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRecoveryTokenRepository, RecoveryTokenRepository>();
builder.Services.AddScoped<LoginService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//app.UseAuthorization();

app.MapControllers();

app.Run();
