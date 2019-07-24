using System.Collections.Generic;
using Bureaucratize.FileStorage.Contracts;
using Bureaucratize.FileStorage.Contracts.Commands;
using Bureaucratize.FileStorage.Contracts.Models;
using Bureaucratize.FileStorage.Contracts.Queries;
using Bureaucratize.FileStorage.Infrastructure;
using Bureaucratize.FileStorage.Infrastructure.CommandHandlers;
using Bureaucratize.FileStorage.Infrastructure.EntityFramework;
using Bureaucratize.FileStorage.Infrastructure.QueryHandlers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bureaucratize.FileStorage.Service
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
            services.Configure<PersistenceConfiguration>(Configuration.GetSection("FileStores"));
            RegisterHandlers(services);

            services.AddDbContext<FileStorageContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("FileStoreConnection")));

            services.AddMvc();
            services.AddRouting();
        }

        private static void RegisterHandlers(IServiceCollection services)
        {
            services
                .AddTransient<IFileStorageQueryHandler<GetImageRecognitionModel, byte[]>,
                    GetImageRecognitionModelHandler>()
                .AddTransient<IFileStorageQueryHandler<GetImageRecognitionLabelMap, string>,
                    GetImageRecognitionLabelMapHandler>()
                .AddTransient<IFileStorageCommandHandler<SaveBitmapsForDocumentToProcess, Nothing>,
                    SaveBitmapsForDocumentToProcessHandler>()
                .AddTransient<IFileStorageQueryHandler<GetBitmapsForDocumentToProcess, ICollection<OrderedBitmapResource>>,
                    GetBitmapsForDocumentToProcessHandler>()
                .AddTransient<IFileStorageCommandHandler<SavePageBitmapForDocumentToProcess, Nothing>,
                    SavePageBitmapForDocumentToProcessHandler>()
                .AddTransient<IFileStorageCommandHandler<SaveBitmapForTemplatePageCanvasDefinition, Nothing>,
                    SaveBitmapForTemplatePageCanvasDefinitionHandler>()
                .AddTransient<IFileStorageQueryHandler<GetCanvasesBitmapsForTemplate, ICollection<TemplatePageCanvasBitmapResource>>,
                    GetCanvasesBitmapsForTemplateHandler>()
                .AddTransient<IFileStorageQueryHandler<GetCanvasBitmapForTemplatePage, TemplatePageCanvasBitmapResource>,
                    GetCanvasBitmapForTemplatePageHandler>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
