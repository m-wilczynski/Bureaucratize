using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bureaucratize.FileStorage.Contracts;
using Bureaucratize.FileStorage.Contracts.Models;
using Bureaucratize.FileStorage.Contracts.Queries;
using Bureaucratize.FileStorage.Infrastructure.EntityFramework;
using Microsoft.Extensions.Options;

namespace Bureaucratize.FileStorage.Infrastructure.QueryHandlers
{
    public class GetCanvasBitmapForTemplatePageHandler 
        : IFileStorageQueryHandler<GetCanvasBitmapForTemplatePage, TemplatePageCanvasBitmapResource>
    {
        private readonly IOptions<PersistenceConfiguration> _configuration;
        private readonly FileStorageContext _context;

        public GetCanvasBitmapForTemplatePageHandler(IOptions<PersistenceConfiguration> configuration,
            FileStorageContext context)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            if (context == null) throw new ArgumentNullException(nameof(context));

            _configuration = configuration;
            _context = context;
        }

        public TemplatePageCanvasBitmapResource Handle(GetCanvasBitmapForTemplatePage query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));
            if (query.TemplatePageId == Guid.Empty)
                throw new ArgumentNullException(nameof(query.TemplatePageId));

            using (_context)
            {
                var canvas = _context.PageCanvasBitmaps
                    .SingleOrDefault(c => c.TemplatePageId == query.TemplatePageId);

                return new TemplatePageCanvasBitmapResource
                {
                    FileData = File.ReadAllBytes(canvas.BitmapPath),
                    Filetype = Path.GetFileName(canvas.BitmapPath).AsBitmapFiletype(),
                    TemplatePageId = canvas.TemplatePageId
                };
            }
        }
    }
}
