// Copyright The OpenTelemetry Authors
// SPDX-License-Identifier: Apache-2.0

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using OpenTelemetry.Exporter.Geneva;
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
            .ConfigureServices(s =>
            {
                s.AddOpenTelemetry()
                    .WithTracing(builder =>
                    {
                        builder
                           .SetResourceBuilder(ResourceBuilder.CreateDefault())
                           .AddHttpClientInstrumentation()
                           .AddAspNetCoreInstrumentation()
                           .SetSampler(new AlwaysOnSampler())
                           .AddGenevaTraceExporter(options =>
                           {
                               options.ConnectionString = "EtwSession=OpenTelemetry";
                           })
                           .AddConsoleExporter();
                    });
            })
            .Build();

        host.Services.GetRequiredService<ILoggerFactory>()?.CreateLogger<Program>()
            .LogInformation("Hello from {name} {price}.", "tomato", 2.99);

        host.Run();
    }
}
