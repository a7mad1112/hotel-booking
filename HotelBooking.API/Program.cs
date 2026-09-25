using Elastic.Ingest.Elasticsearch;
using Elastic.Ingest.Elasticsearch.DataStreams;
using HotelBooking.API.Authorization;
using HotelBooking.API.Extensions;
using HotelBooking.API.Middleware;
using HotelBooking.API.Swagger;
using HotelBooking.Application;
using HotelBooking.Infrastructure;
using Elastic.Serilog.Sinks;
using HotelBooking.Domain.Entities;
using HotelBooking.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "HotelBooking.API")
    .Enrich.WithProperty("Environment", builder.Environment.EnvironmentName)
    .WriteTo.Elasticsearch(
        new[]
        {
            new Uri(builder.Configuration["Elasticsearch:Url"] ?? "http://localhost:9200")
        },
        options =>
        {
            options.DataStream = new DataStreamName("logs", "hotelbooking-api",
                builder.Environment.EnvironmentName.ToLowerInvariant());

            options.BootstrapMethod = BootstrapMethod.Failure;
        })
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddApplicationServices();

builder.Services.AddInfrastructure(
    builder.Configuration);

builder.Services.AddJwtAuthentication(builder.Configuration, builder.Environment);

builder.Services.AddHotelBookingAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddValidation();

builder.Services
    .AddControllers(options => { options.Filters.Add<FluentValidationFilter>(); })
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors =
                context.ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .ToDictionary(
                        x => x.Key,
                        x => x.Value!.Errors
                            .Select(e => e.ErrorMessage)
                            .ToArray());

            return new BadRequestObjectResult(new
            {
                message = "Validation failed.",
                errors
            });
        };
    });

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Enter a valid JWT token."
        });

    options.OperationFilter<AuthorizeCheckOperationFilter>();
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider
        .GetRequiredService<ApplicationDbContext>();

    var passwordHasher = scope.ServiceProvider
        .GetRequiredService<IPasswordHasher<User>>();

    await DatabaseSeeder.SeedAsync(
        dbContext,
        passwordHasher);
}

app.UseSerilogRequestLogging(options =>
{
    options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";

    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
    {
        diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);

        diagnosticContext.Set("UserId",
            httpContext.User.FindFirst(
                System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
            ?? "anonymous");

        diagnosticContext.Set("UserRole",
            httpContext.User.FindFirst(
                System.Security.Claims.ClaimTypes.Role)?.Value ?? "anonymous");
    };
});

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("Frontend");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

try
{
    Log.Information("Hotel Booking API started.");

    app.Run();
}
catch (Exception exception)
{
    Log.Fatal(exception, "Hotel Booking API terminated unexpectedly.");
}
finally
{
    Log.CloseAndFlush();
}