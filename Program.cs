using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Email_Scheduler_WebApi.Data;
using Email_Scheduler_WebApi.Services;
using Email_Scheduler_WebApi.Configuration;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;

var builder = WebApplication.CreateBuilder(args);


builder.Services.Configure<SmtpConfigs>(configs =>
{
    configs.Host = builder.Configuration["SMTP:Host"];

    configs.Port = int.Parse(builder.Configuration["SMTP:Port"]);

    configs.Username = builder.Configuration["SMTP:Username"];

    configs.Password = builder.Configuration["SMTP:Password"];

    configs.From = builder.Configuration["SMTP:From"];
});

builder.Services.Configure<JwtConfigs>(configs =>
    configs.Secret = Encoding.ASCII.GetBytes(builder.Configuration["JWT:Secret"]));

// Add services to the container.

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(jwt =>
    {
        var key = Encoding.ASCII.GetBytes(builder.Configuration["JWT:Secret"]);

        jwt.SaveToken = true;
        jwt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            RequireExpirationTime = true
        };
    });

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllers();

// Register Email Service
builder.Services.AddSingleton<SmtpConfigs>();
builder.Services.AddTransient<EmailService>();

// Register Quartz Background Service
builder.Services.AddHostedService<QuartzHostedService>();

// Register Quartz Service Dependencies
builder.Services.AddSingleton<EmailScheduleHandler>();
builder.Services.AddSingleton<IJobFactory, SingletonJobFactory>();
builder.Services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
builder.Services.AddTransient<EmailScheduleJob>();

// Register Schedule Loader Background Service
builder.Services.AddHostedService<ScheduleLoaderService>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();