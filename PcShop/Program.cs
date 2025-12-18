using Google;
using Microsoft.EntityFrameworkCore;
using PcShop.Areas.Faqs.Repositories;
using PcShop.Areas.Faqs.Repositories.Interfaces;
using PcShop.Areas.Faqs.Services;
using PcShop.Areas.Faqs.Services.Interfaces;
using PcShop.Areas.Games.Factories;
using PcShop.Areas.Games.Repositories;
using PcShop.Areas.Games.Repositories.Interfaces;
using PcShop.Areas.Games.Services;
using PcShop.Areas.Games.Strategies;
using PcShop.Areas.IUsers.Interface;
using PcShop.Areas.Users.Controllers;
using PcShop.Areas.Users.Data;
using PcShop.Areas.Users.Interface;
using PcShop.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddAuthorization();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular",
        policy =>
        {
            policy
                .WithOrigins("http://localhost:4200")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");


builder.Services.AddDbContext<ExamContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Exam")));


builder.Services.AddScoped<IAuthData, AuthData>();
builder.Services.AddScoped<IAuthBus, AuthBus>();
builder.Services.AddScoped<IJwtService, JwtService>();

// Add services to the container.
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])
            ),

            // ? 很重要：把 sub 自動對應成 NameIdentifier
            NameClaimType = JwtRegisteredClaimNames.Sub
        };
    });

//Faq Services and Repositories
builder.Services.AddScoped<IFaqRepository, FaqRepository>();
//「當有人要 IFaqRepository 時，請給他一個 FaqRepository」
builder.Services.AddScoped<IFaqService, FaqService>();
//Game Services and Repositories
builder.Services.AddScoped<IGameRepository, GameRepository>();
builder.Services.AddScoped<IGamePointRepository, GamePointRepository>();
builder.Services.AddScoped<IRecordRepository, RecordRepository>();
builder.Services.AddScoped<GameService>();
// 各遊戲 Strategy（每個遊戲一個）
builder.Services.AddScoped<DinoPointCalculator>();
builder.Services.AddScoped<SnakePointCalculator>();
// Factory（負責「選誰來算」）
builder.Services.AddScoped<GamePointCalculatorFactory>();




var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("AllowAngular");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
