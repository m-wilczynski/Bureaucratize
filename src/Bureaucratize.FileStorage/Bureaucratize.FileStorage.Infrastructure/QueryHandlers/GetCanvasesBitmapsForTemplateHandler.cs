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
    public class GetCanvasesBitmapsForTemplateHandler 
        : IFileStorageQueryHandler<GetCanvasesBitmapsForTemplate, ICollection<TemplatePageCanvasBitmapResource>>
    {
        private readonly IOptions<PersistenceConfiguration> _configuration;
        private readonly FileStorageContext _context;

        public GetCanvasesBitmapsForTemplateHandler(IOptions<PersistenceConfiguration> configuration,
            FileStorageContext context)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            if (context == null) throw new ArgumentNullException(nameof(context));

            _configuration = configuration;
            _context = context;
        }

        public ICollection<TemplatePageCanvasBitmapResource> Handle(GetCanvasesBitmapsForTemplate query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));
            if (query.TemplateId == Guid.Empty)
                throw new ArgumentNullException(nameof(query.TemplateId));

            using (_context)
            {
                var pageCanvases = _context.PageCanvasBitmaps
                    .Where(doc => doc.TemplateId == query.TemplateId)
                    .ToList();

                var bitmaps = new List<TemplatePageCanvasBitmapResource>();

                foreach (var pageCanvas in pageCanvases)
                {
                    var bitmapBytes = File.ReadAllBytes(pageCanvas.BitmapPath);
                    bitmaps.Add(new TemplatePageCanvasBitmapResource
                    {
                        FileData = bitmapBytes,
                        Filetype = Path.GetFileName(pageCanvas.BitmapPath).AsBitmapFiletype(),
                        TemplatePageId = pageCanvas.TemplatePageId
                    });
                }

                return bitmaps;
            }
        }
    }
}
