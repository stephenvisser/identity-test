using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using System.IO;
using System.Collections;
using Microsoft.Extensions.Primitives;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Globalization;
using Microsoft.AspNetCore.Authentication;

namespace identity_test
{
    public class Startup
    {

        private IConfiguration Configuration { get; set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void SetOptionsForOpenIdConnectPolicy(string policy, AzureAdB2COptions b2cOptions, OpenIdConnectOptions options)
        {
            options.MetadataAddress = $"https://login.microsoftonline.com/{b2cOptions.Tenant}/v2.0/.well-known/openid-configuration?p={policy}";
            options.ClientId = b2cOptions.ClientId;
            options.ResponseType = OpenIdConnectResponseType.IdToken;
            options.CallbackPath = $"/signin/{policy}";
            options.SignedOutCallbackPath = $"/signout/{policy}";
            options.SignedOutRedirectUri = "/";
            options.TokenValidationParameters.NameClaimType = "name";
            options.Events = new OpenIdConnectEvents {
                OnRemoteFailure = context => {
                    context.HandleResponse();
                    context.Response.Redirect("/Auth");
                    return Task.CompletedTask; 
                },
                OnAuthenticationFailed = context => {
                    context.HandleResponse();
                    context.Response.Redirect("/Auth");
                    return Task.CompletedTask;
                },
                OnRemoteSignOut = context => {
                    return context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                },
                OnRedirectToIdentityProviderForSignOut = context =>  {
                    return context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                }
            };
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // Takes configuration from appsettings.json, environment variables, and KeyVault
            // (KeyVault added in Program.cs)
            var b2cOptions = Configuration.GetSection("AzureAdB2C").Get<AzureAdB2COptions>();

            services.AddAuthentication(options => {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = "B2C_1_sign_up_in";
                })
                .AddOpenIdConnect("B2C_1_sign_up_in", options => SetOptionsForOpenIdConnectPolicy("B2C_1_sign_up_in", b2cOptions, options))
                .AddCookie();

            services.AddMvc(config => 
            {
                //Same as adding [Authorize] attribute to all controllers & actions
                var policy = new AuthorizationPolicyBuilder()
                         .RequireAuthenticatedUser()
                         .Build();
                         
                config.Filters.Add(new AuthorizeFilter(policy));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/error");
            }

            app.UseStatusCodePages();

            app.UseAuthentication();

            app.UseMvc();
        }
    }
}
