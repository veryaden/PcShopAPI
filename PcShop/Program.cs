using Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PcShop.Areas.Ads.Repositories;
using PcShop.Areas.Ads.Repositories.Interfaces;
using PcShop.Areas.Ads.Services;
using PcShop.Areas.Ads.Services.Interfaces;
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
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using PcShop.Areas.Cart.Repositories;
using PcShop.Areas.Cart.Services;
using PcShop.Areas.Checkout.Repositories;
using PcShop.Areas.Checkout.Services;
using Microsoft.AspNetCore.Identity.UI.Services;
using PcShop.Areas.ECPay.Repositories;
using PcShop.Areas.ECPay.Services;
using PcShop.Areas.OrderItems.Repositories;
using PcShop.Areas.OrderItems.Services;


var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    WebRootPath = "wwwroot"
});


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
                    //這樣可以不用開全域呼叫
                .WithOrigins("http://localhost:4200" , "https://localhost:4200")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");


builder.Services.AddDbContext<ExamContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Exam")));

builder.Services.AddScoped<IAuthData, AuthData>();
builder.Services.AddScoped<IAuthServices, AuthServices>();
builder.Services.AddScoped<IOAuthData, OAuthData>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<ISendEmailService, SendEmailServices>();
builder.Services.AddScoped<IMemberCenterData, MeMberCenterData>();
builder.Services.AddScoped<IMemberCenterService, MemberCenterService>();
builder.Services.AddScoped<IOrderData, OrderData>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IAdminData, AdminData>();
builder.Services.AddScoped<IAdminServices, AdminServices>();
builder.Services.AddMemoryCache();
builder.Services.AddAuthorization();

// Add services to the container.
builder.Services
    .AddAuthentication(options =>
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
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])
            ),

            NameClaimType = "sub" // 讓 Controller 可透過 User.Identity.Name 拿到 userId
        };
        // -------- 修改點：取消 JWT Claim 自動映射，保留原始 claim --------
        options.MapInboundClaims = false;
        // 這樣 User.FindFirst("sub") 才能正確抓到 token 的 sub claim
    });


//Faq Services and Repositories
builder.Services.AddScoped<IFaqRepository, FaqRepository>();
//?𣬚訜?劐犖閬?IFaqRepository ?�?隢讠策隞碶???FaqRepository??
builder.Services.AddScoped<IFaqService, FaqService>();
//Game Services and Repositories
builder.Services.AddScoped<IGameRepository, GameRepository>();
builder.Services.AddScoped<IGamePointRepository, GamePointRepository>();
builder.Services.AddScoped<IRecordRepository, RecordRepository>();
builder.Services.AddScoped<GameService>();
// ?�???Strategy嚗�??钅??脖??页?
builder.Services.AddScoped<DinoPointCalculator>();
builder.Services.AddScoped<SnakePointCalculator>();
// Factory嚗�?鞎研�屸�隤唬?蝞𨰜�㵪?
builder.Services.AddScoped<GamePointCalculatorFactory>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<ICheckoutService, CheckoutService>();
builder.Services.AddScoped<IOrderItemsService, OrderItemsService>();

builder.Services.AddScoped<IAdRepository, AdRepository>();
builder.Services.AddScoped<IPositionRepository, PositionRepository>();
builder.Services.AddScoped<IAdService, AdService>();

// ECPay Services
builder.Services.AddScoped<IECPayService, ECPayService>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
var app = builder.Build();
// ---- 新增：確認目前連線的資料庫 ----
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ExamContext>();
    Console.WriteLine("Current DB Connection: " + context.Database.GetDbConnection().ConnectionString);
}
// --------------------------------------


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    //app.UseSwagger();
    //app.UseSwaggerUI(); // ?躰??齿糓?见? UI 隞钅𢒰
}

app.UseHttpsRedirection();
app.UseCors("AllowAngular");
app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();

app.MapControllers();

app.Run();
