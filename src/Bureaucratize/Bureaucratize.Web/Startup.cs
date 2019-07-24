using Bureaucratize.ImageProcessing.Infrastructure;
using Bureaucratize.Templating.Infrastructure;
using Bureaucratize.Templating.Infrastructure.NetStand;
using Bureaucratize.Web.Config;
using Bureaucratize.Web.ViewShortcuts;
using Bureaucratize.Web.WebSockets;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bureaucratize.Web
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
            services.AddMvc();

            services.AddTransient<IImageProcessingPersistenceConfiguration, ImageProcessingPersistenceConfiguration>();
            services.AddTransient<ITemplatingPersistenceConfiguration, TemplatingPersistenceConfiguration>();
            services.AddTransient<PrepareTemplates, PrepareTemplates>();
            services.AddSingleton<DocumentHubManager>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();
            app.UseWebSockets();
            app.UseMiddleware<DocumentHubManagerMiddleware>();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            SetupAkka.BootstrapActorSystem(Configuration);
        }
    }
}
