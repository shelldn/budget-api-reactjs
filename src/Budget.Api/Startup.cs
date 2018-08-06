using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Budget.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services
                .AddMvc(c => {

                    var policy = new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .Build();

                    c.Filters.Add(new AuthorizeFilter(policy));
                })

                .AddJsonOptions(o =>
                {
                    o.SerializerSettings.Converters.Add(new StringEnumConverter(true));
                });

            services
                .AddAuthentication("Bearer")
                .AddIdentityServerAuthentication(o => {
                    o.Authority = "http://localhost:5001";
                    o.RequireHttpsMetadata = false;
                    o.ApiName = "api1";
                });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(b => b
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod()
            );

            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
