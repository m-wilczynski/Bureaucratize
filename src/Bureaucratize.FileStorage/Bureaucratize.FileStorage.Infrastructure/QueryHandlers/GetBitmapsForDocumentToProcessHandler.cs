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
    public class GetBitmapsForDocumentToProcessHandler 
        : IFileStorageQueryHandler<GetBitmapsForDocumentToProcess, ICollection<OrderedBitmapResource>>
    {
        private readonly IOptions<PersistenceConfiguration> _configuration;
        private readonly FileStorageContext _context;

        public GetBitmapsForDocumentToProcessHandler(IOptions<PersistenceConfiguration> configuration,
            FileStorageContext context)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            if (context == null) throw new ArgumentNullException(nameof(context));

            _configuration = configuration;
            _context = context;
        }

        public ICollection<OrderedBitmapResource> Handle(GetBitmapsForDocumentToProcess query)
        {
            if (query.DocumentId == Guid.Empty)
                throw new ArgumentNullException(nameof(query.DocumentId));

            using (_context)
            {
                var bitmapDefinitions = _context.DocumentBitmaps
                    .Where(doc => doc.DocumentToProcessId == query.DocumentId)
                    .ToList();

                var bitmaps = new List<OrderedBitmapResource>();

                foreach (var bitmapDefinition in bitmapDefinitions)
                {
                    var bitmapBytes = File.ReadAllBytes(bitmapDefinition.BitmapPath);
                    bitmaps.Add(new OrderedBitmapResource
                    {
                        FileData = bitmapBytes,
                        Filetype = Path.GetFileName(bitmapDefinition.BitmapPath).AsBitmapFiletype(),
                        Order = bitmapDefinition.Order
                    });
                }

                return bitmaps;
            }
        }
    }
}
