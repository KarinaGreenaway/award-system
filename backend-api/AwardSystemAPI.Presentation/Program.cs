using AwardSystemAPI.Application.BackgroundServices;
using AwardSystemAPI.Application.Mappings;
using AwardSystemAPI.Application.Options;
using AwardSystemAPI.Application.Services;
using AwardSystemAPI.Extensions;
using Microsoft.EntityFrameworkCore;
using AwardSystemAPI.Infrastructure;
using AwardSystemAPI.Infrastructure.Repositories;
using AwardSystemAPI.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IAwardProcessService, AwardProcessService>();
builder.Services.AddScoped<IAwardProcessRepository, AwardProcessRepository>();
builder.Services.AddScoped<IAwardCategoryRepository, AwardCategoryRepository>();
builder.Services.AddScoped<IAwardCategoryService, AwardCategoryService>();
builder.Services.AddScoped<IRsvpService, RsvpService>();
builder.Services.AddScoped<IRsvpRepository, RsvpRepository>();
builder.Services.AddScoped<IRsvpFormQuestionsRepository, RsvpFormQuestionsRepository>();
builder.Services.AddScoped<IFeedbackService, FeedbackService>();
builder.Services.AddScoped<IFeedbackRepository, FeedbackRepository>();
builder.Services.AddScoped<IFeedbackFormQuestionRepository, FeedbackFormQuestionRepository>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IMobileUserSettingsService, MobileUserSettingsService>();
builder.Services.AddScoped<IJudgingRoundService, JudgingRoundService>();
builder.Services.AddScoped<IAwardEventService, AwardEventService>();
builder.Services.AddScoped<IAwardEventRepository, AwardEventRepository>();
builder.Services.AddScoped<INominationService, NominationService>();
builder.Services.AddScoped<INominationRepository, NominationRepository>();
builder.Services.AddScoped<INomineeSummaryService, NomineeSummaryService>();
builder.Services.AddScoped<INomineeSummaryRepository, NomineeSummaryRepository>();
builder.Services.AddScoped<IAiSummaryService, AiSummaryService>();
builder.Services.AddScoped<IVertexAiService, VertexAiService>();
builder.Services.AddScoped<INominationQuestionRepository, NominationQuestionRepository>();
builder.Services.AddScoped<INominationQuestionService, NominationQuestionService>();
builder.Services.AddScoped<IAuthorizationHandler, CategoryOwnerHandler>();
builder.Services.AddScoped<IDeviceTokenRepository, DeviceTokenRepository>();
builder.Services.AddScoped<IAnnouncementRepository, AnnouncementRepository>();
builder.Services.AddScoped<IAnnouncementService, AnnouncementService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IBlobService, BlobService>();
builder.Services.AddScoped<IPushNotificationService, PushNotificationService>();
builder.Services.AddSingleton<IFirebaseNotificationService, FirebaseNotificationService>();
// builder.Services.AddSingleton<IFirebaseMessagingClient, FirebaseMessagingClient>();

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}
builder.Services.Configure<VertexAiOptions>(
    builder.Configuration.GetSection("VertexAi")
);
builder.Services.AddScoped<IVertexAiService, VertexAiService>();



builder.Services.AddHostedService<AnnouncementPublisherService>();



builder.Services.AddAutoMapper(typeof(AwardProcessProfile));
builder.Services.AddAutoMapper(typeof(AwardCategoryProfile));
builder.Services.AddAutoMapper(typeof(NotificationProfile));
builder.Services.AddAutoMapper(typeof(MobileUserSettingsProfile));
builder.Services.AddAutoMapper(typeof(JudgingRoundProfile));
builder.Services.AddAutoMapper(typeof(AwardEventProfile));
builder.Services.AddAutoMapper(typeof(NominationProfile));
builder.Services.AddAutoMapper(typeof(NomineeSummaryProfile));
builder.Services.AddAutoMapper(typeof(TeamMemberProfile));
builder.Services.AddAutoMapper(typeof(NominationQuestionProfile));
builder.Services.AddAutoMapper(typeof(AnnouncementProfile));
builder.Services.AddAutoMapper(typeof(RsvpProfile));
builder.Services.AddAutoMapper(typeof(FeedbackFormQuestionProfile));
builder.Services.AddAutoMapper(typeof(FeedbackProfile));
builder.Services.AddAutoMapper(typeof(UserProfile));

builder.Services.AddControllers();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(
        "CategoryOwnerPolicy",
        policy => policy
            .RequireAuthenticatedUser()
            .AddRequirements(new CategoryOwnerRequirement()));
    
    options.AddPolicy("SponsorOrAdminPolicy", policy =>
        policy.RequireRole("Sponsor", "Admin"));
    
    options.AddPolicy("AdminOnlyPolicy", policy =>
        policy.RequireRole("Admin"));
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5174", "http://localhost:5173", "http://localhost:5175")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddHttpContextAccessor();



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

app.UseCors("AllowFrontend");
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