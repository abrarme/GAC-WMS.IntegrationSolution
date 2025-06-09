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
using Microsoft.EntityFrameworkCore;
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
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

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

builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
});

#endregion

var app = builder.Build();

#region Middleware Pipeline

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
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
