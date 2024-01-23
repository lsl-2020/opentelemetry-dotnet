// Copyright The OpenTelemetry Authors
// SPDX-License-Identifier: Apache-2.0

using System.Diagnostics;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Exporter.Geneva;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace FunctionApp;

internal class Program
{
    private static void Main(string[] args)
    {
        // test trace
        var host = new HostBuilder()
            .ConfigureFunctionsWorkerDefaults()
            .ConfigureLogging(builder =>
            {
                builder.ClearProviders();
                builder.AddConsole();
                /*
                builder.AddOpenTelemetry(options =>
                {
                    options
                        .AddGenevaLogExporter(genevaExporterOptions =>
                        {
                            genevaExporterOptions.ConnectionString = "EtwSession=OpenTelemetry";
                        })
                        .AddConsoleExporter()
                        .AddOtlpExporter();
                });
                */
            })
            .ConfigureServices(s =>
            {
                s.AddSingleton(new ActivitySource("FunctionApp"));

                s.AddOpenTelemetry()
                    .WithTracing(builder =>
                    {
                        builder
                            .AddSource("FunctionApp")
                            .AddSource("StaticActivitySource")
                            .SetResourceBuilder(ResourceBuilder.CreateDefault())
                            .AddHttpClientInstrumentation()
                            .AddAspNetCoreInstrumentation()
                            .SetSampler(new AlwaysOnSampler())
                            .AddGenevaTraceExporter(options =>
                            {
                                options.ConnectionString = "EtwSession=OpenTelemetry";
                            })
                            .AddConsoleExporter()
                            .AddOtlpExporter();
                    });
            })
            .Build();

        using var activity = host.Services.GetRequiredService<ActivitySource>().StartActivity("Main");
        using var activity2 = TestActivitySource.ActivitySource.StartActivity("Main2");
        host.Services.GetRequiredService<ILoggerFactory>().CreateLogger<Program>()
            .LogInformation("Hello from {name} {price}.", "tomato", 2.99);

        host.Run();
    }
}
