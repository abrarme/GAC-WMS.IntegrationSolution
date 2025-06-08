using GAC_WMS.IntegrationSolution.Clients;
using GAC_WMS.IntegrationSolution.Data;
using GAC_WMS.IntegrationSolution.Jobs;
using GAC_WMS.IntegrationSolution.Middleware;
using GAC_WMS.IntegrationSolution.Repositories.Implementation;
using GAC_WMS.IntegrationSolution.Repositories.Interface;
using GAC_WMS.IntegrationSolution.Services;
using GAC_WMS.IntegrationSolution.Services.Implementation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Quartz;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Register DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register Retry Policy
builder.Services.AddTransient<RetryPolicyHandler>();

// Register File Processing Services

builder.Services.AddScoped<IWmsClient, WmsClient>();
builder.Services.AddHttpClient<WmsClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["WMS:BaseUrl"]);
}).AddHttpMessageHandler<RetryPolicyHandler>();

builder.Services.AddScoped<IFileProcessorFactory, FileProcessorFactory>();
builder.Services.AddScoped<CsvFileProcessor>();
builder.Services.AddScoped<JsonFileProcessor>();
builder.Services.AddScoped<XmlFileProcessor>();

//  Register repositories
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();


builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();
    q.ScheduleJob<FilePollingJob>(trigger => trigger
        .WithIdentity("FilePolling")
        .WithCronSchedule(builder.Configuration["FilePolling:Schedule"] ?? "0 */2 * ? * *")
        .WithDescription("Poll files every 1 mins"));
});
builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

// Register the job class
builder.Services.AddScoped<FilePollingJob>();

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("Logs/GAC-WMS_logs.log", rollingInterval: RollingInterval.Day)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog(); // Use Serilog for logging


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseGlobalExceptionHandler();  // Register custom error middleware
app.UseAuthorization();

app.MapControllers();

app.Run();
