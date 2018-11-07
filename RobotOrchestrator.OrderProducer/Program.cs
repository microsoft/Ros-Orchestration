// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace RobotOrchestrator.OrderProducer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    var builtConfig = config.Build();

                    if (!string.IsNullOrWhiteSpace(builtConfig["Vault"]))
                    {
                        config.AddAzureKeyVault(
                            $"https://{builtConfig["Vault"]}.vault.azure.net/",
                            builtConfig["ClientId"],
                            builtConfig["ClientSecret"]
                            );
                    }
                })
                .UseStartup<Startup>();
    }
}
