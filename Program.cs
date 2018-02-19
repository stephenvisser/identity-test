using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration.AzureKeyVault;

namespace identity_test
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    var builtConfig = config.Build();
                    config.AddAzureKeyVault(
                        $"https://{builtConfig["KeyVault:Name"]}.vault.azure.net/",
                        builtConfig["KeyVault:ApplicationId"],
                        builtConfig["KeyVault:ClientSecret"]);
                })
                .UseStartup<Startup>()
                .Build();

    }
}
