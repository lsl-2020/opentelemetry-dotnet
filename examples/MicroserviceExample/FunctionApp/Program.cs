// Copyright The OpenTelemetry Authors
// SPDX-License-Identifier: Apache-2.0

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

internal class Program
{
    private static void Main(string[] args)
    {
        var host = new HostBuilder()
            .ConfigureFunctionsWorkerDefaults()
            .ConfigureServices(s =>
            {
                s.AddOpenTelemetry()
                    .WithTracing(builder =>
                    {
                        builder
                           .AddSource("Microsoft.Azure.Functions.Worker")
                           .SetResourceBuilder(ResourceBuilder.CreateDefault())
                           .AddAspNetCoreInstrumentation()
                           .SetSampler(new ParentBasedSampler(new AlwaysOnSampler()))
                           .AddConsoleExporter()
                           .AddOtlpExporter();
                    });
            })
            .Build();

        host.Run();
    }
}
