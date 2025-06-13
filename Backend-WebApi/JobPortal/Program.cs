using System.Text;
using JobPortal.Contexts;
using JobPortal.Interfaces;
using JobPortal.Models;
using JobPortal.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using log4net;
using log4net.Config;
using System.Reflection;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Microsoft.AspNetCore.Mvc.Versioning;
using Prometheus;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

var logRepo = LogManager.GetRepository(Assembly.GetEntryAssembly());
XmlConfigurator.Configure(logRepo, new FileInfo("log4net.config"));
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5001, listenOptions =>
    {
        listenOptions.UseHttps(); 
    });
});
builder.Services.AddOpenApi();

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
});

builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});
builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy("PerHourPolicy", httpContext =>
        RateLimitPartition.Get(
            httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            key => new FixedWindowRateLimiter(
                new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 1000,
                    Window = TimeSpan.FromHours(1),
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    QueueLimit = 0
                }
            )
        )
    );
});


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "JobPortal", Version = "v1" });
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
    opt.ExampleFilters();
});

builder.Services.AddSignalR();
builder.Services.AddSwaggerExamplesFromAssemblyOf<Program>();

builder.Services.AddDbContext<JobContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
            );

builder.Services.AddControllers().AddJsonOptions(options =>
 {
     options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
     options.JsonSerializerOptions.WriteIndented = true;
     
 });

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = false,
                        ValidateIssuer = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Keys:JwtTokenKey"]!)),
                        ClockSkew = TimeSpan.Zero
                    };
                });
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://127.0.0.1:5500")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

#region Repositories
builder.Services.AddTransient<IRepository<Guid, User>, UserRepository>();
builder.Services.AddTransient<IRepository<Guid, Company>, CompanyRepository>();
builder.Services.AddTransient<IRepository<Guid, Address>, AddressRepository>();
builder.Services.AddTransient<IRepository<Guid, IndustryType>, IndustryTypeRepository>();
builder.Services.AddTransient<IRepository<Guid, JobApplication>, JobApplicantRepository>();
builder.Services.AddTransient<IRepository<Guid, JobPost>, JobPostRepository>();
builder.Services.AddTransient<IRepository<Guid, PostSkills>, PostSkillsRepository>();
builder.Services.AddTransient<IRepository<Guid, Recruiter>, RecruiterRepository>();
builder.Services.AddTransient<IRepository<Guid, Requirements>, RequirementRepository>();
builder.Services.AddTransient<IRepository<Guid, Responsibilities>, ResponsibilityRepository>();
builder.Services.AddTransient<IRepository<Guid, Seeker>, SeekerRepository>();
builder.Services.AddTransient<IRepository<Guid, SeekerSkills>, SeekerSkillsRepository>();
builder.Services.AddTransient<IRepository<Guid, Skill>, SkillsRepository>();
builder.Services.AddTransient<IRepository<Guid, FileModel>, FileRepository>();
#endregion

#region Services
builder.Services.AddTransient<IAuthenticationService, AuthenticationService>();
builder.Services.AddTransient<ICompanyService, CompanyService>();
builder.Services.AddTransient<IEncryptionService, EncryptionService>();
builder.Services.AddTransient<IRecruiterService, RecruiterService>();
builder.Services.AddTransient<ISeekerService, SeekerService>();
builder.Services.AddTransient<ISkillsService, SkillsService>();
builder.Services.AddTransient<ITokenService, TokenService>();
builder.Services.AddTransient<IIndustryTypeService, IndustryTypeService>();
builder.Services.AddTransient<IJobApplicationPaged, JobApplicationPagedGet>();
builder.Services.AddTransient<IJobPostPagedGet, JobPostPagedGet>();
builder.Services.AddTransient<IJobApplicantService, JobApplicantService>();
builder.Services.AddTransient<IJobPostService, JobPostService>();
builder.Services.AddTransient<ITransactionalSeekerService, TransactionalSeekerService>();
builder.Services.AddTransient<ISeekerPagedGet, SeekerPagedGet>();
builder.Services.AddTransient<ITransactionalRecruiterRegister, TransactionalRecruiterRegister>();
builder.Services.AddTransient<ITransactionalJobPostService, TransactionalJobPostService>();
builder.Services.AddTransient<IFileService, FileService>();
builder.Services.AddTransient<ISeekerNotificationService, SeekerNotificationService>();
#endregion

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseCors();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.UseMetricServer(); 
app.UseHttpMetrics();
app.MapHub<NotificationHub>("/notificationHub");
app.MapControllers();
app.Run();
