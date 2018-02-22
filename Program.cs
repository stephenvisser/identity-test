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

    public class KeyVaultConfiguration
    {
        public string Name { get; set; }
        public string ApplicationId { get; set; }
        public string ClientSecret { get; set; }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        
        public static KeyVaultConfiguration GetKeyVaultConfiguration(IHostingEnvironment env) 
        {
            //KeyVault configuration can be found in appsettings or environment variables
            //We won't need to do things this way once .NET Core 2.1 comes out; it will have
            //chained config as per https://github.com/aspnet/Configuration/issues/630
            var config = new ConfigurationBuilder();

            var keyVaultConfig = new KeyVaultConfiguration();

            var rootConfig = config
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            return rootConfig.GetSection("KeyVault").Get<KeyVaultConfiguration>();
        }


        public static IWebHost BuildWebHost(string[] args) {

            return WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    var keyVaultConfig = GetKeyVaultConfiguration(context.HostingEnvironment);

                    config.AddAzureKeyVault(
                        $"https://{keyVaultConfig.Name}.vault.azure.net/",
                        keyVaultConfig.ApplicationId,
                        keyVaultConfig.ClientSecret);
                })
                .UseStartup<Startup>()
                .Build();
        }

    }
}
