using AwardSystemAPI.Application.Mappings;
using AwardSystemAPI.Application.Services;
using AwardSystemAPI.Extensions;
using Microsoft.EntityFrameworkCore;
using AwardSystemAPI.Infrastructure;
using AwardSystemAPI.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IAwardProcessService, AwardProcessService>();
builder.Services.AddScoped<IAwardCategoryRepository, AwardCategoryRepository>();
builder.Services.AddScoped<IAwardCategoryService, AwardCategoryService>();
builder.Services.AddScoped<INotificationService, NotificationService>();

builder.Services.AddAutoMapper(typeof(AwardProcessProfile));
builder.Services.AddAutoMapper(typeof(AwardCategoryProfile));
builder.Services.AddAutoMapper(typeof(NotificationProfile));

builder.Services.AddControllers();
builder.Services.AddAuthorization(); 
builder.Services.AddProblemDetails();

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOpenApi();

builder.Services.AddAuthentication("FakeScheme")
    .AddScheme<AuthenticationSchemeOptions, FakeAuthenticationHandler>("FakeScheme", options => { });

builder.Services.Configure<AuthenticationOptions>(options =>
{
    options.DefaultAuthenticateScheme = "FakeScheme";
    options.DefaultChallengeScheme = "FakeScheme";
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "AwardSystem API V1");
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

var logger = app.Services.GetRequiredService<ILogger<Program>>();
var env = app.Services.GetRequiredService<IWebHostEnvironment>();
//app.ConfigureExceptionHandler(logger, env);

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.Run();