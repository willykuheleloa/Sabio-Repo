using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sabio.Web.Core;
using Sabio.Web.StartUp;

namespace Sabio.Web.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureAppSettings(services);

            DependencyInjection.ConfigureServices(services, Configuration);

            Cors.ConfigureServices(services);

            Authentication.ConfigureServices(services, Configuration);

            MVC.ConfigureServices(services);

            SPA.ConfigureServices(services);
        }

        private void ConfigureAppSettings(IServiceCollection services)
        {
            services.AddOptions();
            services.Configure<SecurityConfig>(Configuration.GetSection("Security"));
            services.Configure<JsonWebTokenConfig>(Configuration.GetSection("JsonWebToken"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseDeveloperExceptionPage();
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            Cors.Configure(app, env);

            Authentication.Configure(app, env);

            MVC.Configure(app, env);

            SPA.Configure(app, env);

            StaticFiles.Configure(app, env);
        }
    }
}