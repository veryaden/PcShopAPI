using Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
using Microsoft.IdentityModel.Tokens;
using PcShop.Areas.IUsers.Interface;
using PcShop.Areas.Users.Controllers;
using PcShop.Areas.Users.Data;
using PcShop.Areas.Users.Interface;
using PcShop.Models;
using System.Text;
using PcShop.Areas.Cart.Repositories;
using PcShop.Areas.Cart.Services;
using PcShop.Areas.Checkout.Repositories;
using PcShop.Areas.Checkout.Services;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

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

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
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
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
        )
    };
}); var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");


builder.Services.AddDbContext<ExamContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Exam")));

builder.Services.AddScoped<IAuthData, AuthData>();
builder.Services.AddScoped<IAuthBus, AuthBus>();
builder.Services.AddScoped<IOAuthData, OAuthData>();
builder.Services.AddScoped<IJwtService, JwtService>();

builder.Services.AddAuthorization();

builder.Services.AddControllers();

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
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<ICheckoutService, CheckoutService>();




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
