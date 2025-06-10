using FluentValidation;
using FluentValidation.AspNetCore;
using GAC_WMS.IntegrationSolution.Clients;
using GAC_WMS.IntegrationSolution.Data;
using GAC_WMS.IntegrationSolution.Helper;
using GAC_WMS.IntegrationSolution.Jobs;
using GAC_WMS.IntegrationSolution.Middleware;
using GAC_WMS.IntegrationSolution.Repositories.Implementation;
using GAC_WMS.IntegrationSolution.Repositories.Interface;
using GAC_WMS.IntegrationSolution.Services;
using GAC_WMS.IntegrationSolution.Services.Implementation;
using GAC_WMS.IntegrationSolution.Services.Interface;
using GAC_WMS.IntegrationSolution.Validator;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Quartz;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

#region Logging - Serilog

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("Logs/GAC-WMS_logs.log", rollingInterval: RollingInterval.Day)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

#endregion

#region Services - DbContext & Retry Policy

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,               // number of retries
                maxRetryDelay: TimeSpan.FromSeconds(10), // delay between retries
                errorNumbersToAdd: null         // add specific SQL error codes if needed
            );
        }));

builder.Services.AddScoped<RetryPolicyHandler>();

#endregion

#region Services - File Processing

builder.Services.AddScoped<IWmsClient, WmsClient>();

builder.Services.AddHttpClient<WmsClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["WMS:BaseUrl"]);
}).AddHttpMessageHandler<RetryPolicyHandler>();

builder.Services.AddScoped<IFileProcessorFactory, FileProcessorFactory>();
builder.Services.AddScoped<IXmlParser, XmlParser>();
builder.Services.AddScoped<IFileErrorHandler, FileErrorHandler>();
builder.Services.AddScoped<CsvFileProcessor>();
builder.Services.AddScoped<JsonFileProcessor>();
builder.Services.AddScoped<XmlFileProcessor>();

#endregion

#region Services - Email

builder.Services.AddScoped<IEmailService, EmailService>();

#endregion

#region Repositories

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IPurchaseOrderRepository, PurchaseOrderRepository>();
builder.Services.AddScoped<ISalesOrderRepository, SalesOrderRepository>();

#endregion

#region Jobs - Quartz

builder.Services.AddScoped<FilePollingJob>();

builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();
    q.ScheduleJob<FilePollingJob>(trigger => trigger
        .WithIdentity("FilePolling")
        .WithCronSchedule(builder.Configuration["FilePolling:Schedule"] ?? "0 */2 * ? * *")
        .WithDescription("Poll files every 2 hours"));
});

builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

#endregion

#region Authentication Services

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]))
        };
    });

builder.Services.AddAuthorization();

#endregion

#region FluentValidation

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<ProductDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CustomerDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<SalesOrderDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<PurchaseOrderDtoValidator>();

#endregion

#region AutoMapper

builder.Services.AddAutoMapper(typeof(Program));

#endregion

#region Controllers & Swagger

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
        options.JsonSerializerOptions.WriteIndented = true;
    });

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.EnableAnnotations();

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

#endregion

var app = builder.Build();

#region Database Migrations

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate(); // Applies pending migrations and creates DB
}

#endregion

#region Middleware Pipeline

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage(); // Shows detailed error
}
else
{
    app.UseGlobalExceptionHandler(); // Custom middleware
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

#endregion

app.Run();
