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

namespace identity_test
{
    public class Startup
    {

        private IConfiguration Configuration { get; set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<AzureAdB2COptions>(Configuration.GetSection("AzureAdB2C"));

            // services.AddSession(options =>
            // {
            //     options.IdleTimeout = TimeSpan.FromHours(1);
            //     options.Cookie.HttpOnly = true;
            // });


            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddOpenIdConnect(options => {
                    options.ClientId = Configuration["AzureAD:ClientId"];
                    options.Authority = string.Format(CultureInfo.InvariantCulture, Configuration["AzureAd:AadInstance"], "common", "/v2.0");
                    options.ResponseType = OpenIdConnectResponseType.IdToken;
                    options.SignedOutRedirectUri = Configuration["AzureAd:PostLogoutRedirectUri"];
                    options.Events = new OpenIdConnectEvents
                    {
                        OnAuthenticationFailed = RemoteFailure,
                        OnTokenValidated = TokenValidated
                    };
                    // options.TokenValidationParameters = new TokenValidationParameters
                    // {
                    //     // Instead of using the default validation (validating against
                    //     // a single issuer value, as we do in line of business apps), 
                    //     // we inject our own multitenant validation logic
                    //     ValidateIssuer = false,

                    //     NameClaimType = "name"
                    // };
            });

            services.AddMvc(config => 
            {
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
                // app.UseExceptionHandler("/error");
            }
            // app.UseSession();

            app.UseAuthentication();




            // app.UseStatusCodePages(context => {
            //     if (context.HttpContext.Response.StatusCode == 401) {

            //         Console.WriteLine($"🔥: {context.HttpContext.Request.QueryString}");

            //         KeyValuePair<string, StringValues> forgotPasswordRequest = context.HttpContext.Request.Query.FirstOrDefault(kv => 
            //             kv.Value
            //                 .FirstOrDefault()
            //                 ?.StartsWith("AADB2C90118") 
            //                 ?? false
            //         );

            //         if (!forgotPasswordRequest.Equals(default(KeyValuePair<string, StringValues>))) 
            //         {
            //             context.HttpContext.Response.Redirect($"https://login.microsoftonline.com/{Configuration["AzureAdB2C:Tenant"]}/oauth2/v2.0/authorize?p=B2C_1_fp&client_id=25b026f3-9fe9-414a-b2c0-4554b38b17bb&nonce=defaultNonce&redirect_uri=http%3A%2F%2Flocalhost%3A5000&scope=https%3A%2F%2Fsvper.onmicrosoft.com%2Fapi%2Fread&response_type=token&prompt=login");
            //         }
            //         else if (context.HttpContext.Request.Query.ContainsKey("code"))
            //         {
            //             Console.WriteLine($"🔥 got us a code ya'll");
            //         }
            //         else
            //         {
            //             //visser.stephen+2@gmail.com
            //             //GTH3352^$#@%%@ff
            //             //D#}0^exa6*V67qJ\z66~%cI^
            //             //AADB2C90091: The user has cancelled entering self-asserted information.
            //             //including CODE!
            //             context.HttpContext.Response.Redirect($"https://login.microsoftonline.com/{Configuration["AzureAdB2C:Tenant"]}/oauth2/v2.0/authorize?p=B2C_1_test&client_id=25b026f3-9fe9-414a-b2c0-4554b38b17bb&nonce=defaultNonce&redirect_uri=http%3A%2F%2Flocalhost%3A5000&scope=https%3A%2F%2Fsvper.onmicrosoft.com%2Fapi%2Fread&response_type=id_token%20token&prompt=login");
            //         }

            //     }
            //     return Task.CompletedTask;
            // });

            //app.UseStatusCodePagesWithRedirects();

            app.UseMvc();
        }

        private Task AuthenticationFailed(AuthenticationFailedContext arg)
        {
            // For debugging purposes only!
            var s = $"AuthenticationFailed: {arg.Exception.Message}";
            arg.Response.ContentLength = s.Length;
            arg.Response.Body.Write(Encoding.UTF8.GetBytes(s), 0, s.Length);
            return Task.FromResult(0);
        }

        private Task TokenValidated(TokenValidatedContext context)
        {
            /* ---------------------
            // Replace this with your logic to validate the issuer/tenant
               ---------------------       
            // Retriever caller data from the incoming principal
            string issuer = context.SecurityToken.Issuer;
            string subject = context.SecurityToken.Subject;
            string tenantID = context.Ticket.Principal.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid").Value;

            // Build a dictionary of approved tenants
            IEnumerable<string> approvedTenantIds = new List<string>
            {
                "<Your tenantID>",
                "9188040d-6c67-4c5b-b112-36a304b66dad" // MSA Tenant
            };

            if (!approvedTenantIds.Contains(tenantID))
                throw new SecurityTokenValidationException();
              --------------------- */

            return Task.FromResult(0);
        }

        // Handle sign-in errors differently than generic errors.
        private Task RemoteFailure(AuthenticationFailedContext context)
        {
            context.HandleResponse();
            context.Response.Redirect("/Home/Error?message=" + context.Exception.Message);
            return Task.FromResult(0);
        }
    }
}
