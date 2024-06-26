﻿using System.Diagnostics;

using Encamina.Enmarcha.AI.OpenAI.Azure;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.ApplicationInsights;

using GlobalAzureSpain.Demo.Services.Common;
using GlobalAzureSpain.Demo.Services.VacationPlanner.Extensions;
using GlobalAzureSpain.Demo.Services.VacationPlanner.Models;
using GlobalAzureSpain.Demo.Services.VacationPlanner.Services;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions()
{
    Args = args,
    ContentRootPath = Directory.GetCurrentDirectory(),
});

builder.Configuration.SetBasePath(Directory.GetCurrentDirectory());

var isDevelopment = builder.Environment.IsDevelopment();

/* Load Configuration */

if (Debugger.IsAttached)
{
    builder.Configuration.AddJsonFile(@"appsettings.debug.json", optional: true, reloadOnChange: true);
}

builder.Configuration.AddJsonFile($@"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                     .AddJsonFile($@"appsettings.{Environment.UserName}.json", optional: true, reloadOnChange: true)
                     .AddEnvironmentVariables();

/* Logging Configuration */

if (isDevelopment)
{
    builder.Logging.AddConsole();

    if (Debugger.IsAttached)
    {
        builder.Logging.AddDebug();
    }
}

var applicationInsightsConnectionString = builder.Configuration.GetConnectionString(@"ApplicationInsights");

builder.Logging.AddApplicationInsights((telemetryConfiguration) => telemetryConfiguration.ConnectionString = applicationInsightsConnectionString, (_) => { })
               .AddFilter<ApplicationInsightsLoggerProvider>(string.Empty, LogLevel.Trace)
               ;

/* Options Configuration */

builder.Services.AddOptionsWithValidateOnStart<AzureOpenAIOptions>().BindConfiguration(nameof(AzureOpenAIOptions)).ValidateDataAnnotations();

/* Application Services */

builder.AddServiceDefaults();

builder.Services.AddDaprClient();

builder.Services.AddApplicationInsightsTelemetry(builder.Configuration)
                .AddRouting()
                .AddEndpointsApiExplorer()
                .AddSwaggerGen()
                .AddAzureAIServices()
                .AddScoped<VacationPlannerService>()
                .AddHealthChecks()
                ;

/* Application Middleware Configuration */

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapPost(@"/vacations/plan", async Task<Results<Ok<VacationPlannerResponse>, ProblemHttpResult>> (VacationPlannerRequest request, [FromServices] VacationPlannerService vacationPlannerService, CancellationToken cancellationToken) =>
{
    try
    {
        var response = await vacationPlannerService.ResponseAsync(request, cancellationToken);

        return TypedResults.Ok(response);
    }
    catch (Exception exception)
    {
        return TypedResults.Problem(exception.Message);
    }
})
.WithName(@"GetVacationPlan")
.WithOpenApi();

app.Run();