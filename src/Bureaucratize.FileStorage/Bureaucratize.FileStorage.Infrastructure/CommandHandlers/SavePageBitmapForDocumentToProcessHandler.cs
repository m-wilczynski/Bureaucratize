using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bureaucratize.FileStorage.Contracts;
using Bureaucratize.FileStorage.Contracts.Commands;
using Bureaucratize.FileStorage.Infrastructure.EntityFramework;
using Bureaucratize.FileStorage.Infrastructure.EntityFramework.PersistenceModels;
using Microsoft.Extensions.Options;

namespace Bureaucratize.FileStorage.Infrastructure.CommandHandlers
{
    public class SavePageBitmapForDocumentToProcessHandler
        : IFileStorageCommandHandler<SavePageBitmapForDocumentToProcess, Nothing>
    {
        private readonly IOptions<PersistenceConfiguration> _configuration;
        private readonly FileStorageContext _context;

        public SavePageBitmapForDocumentToProcessHandler(IOptions<PersistenceConfiguration> configuration,
            FileStorageContext context)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            if (context == null) throw new ArgumentNullException(nameof(context));

            _configuration = configuration;
            _context = context;
        }

        public Nothing Handle(SavePageBitmapForDocumentToProcess command)
        {
            using (_context)
            {
                var savedBitmapsPath = default((string Path, string FileLabel));
                try
                {
                    string directory = EnsureDirectoryForDocumentBitmaps(command);
                    savedBitmapsPath = WriteBitmapToFilesystem(command, directory);
                    UpsertBitmapInfoToDb(command, savedBitmapsPath);
                }
                catch (Exception ex)
                {
                    File.Delete(savedBitmapsPath.Path);
                    throw;
                }
                return new Nothing();
            }
        }

        private void UpsertBitmapInfoToDb(SavePageBitmapForDocumentToProcess command, 
            (string Path, string FileLabel) savedBitmapPath)
        {
            var pageForDocument = _context.DocumentBitmaps
                .SingleOrDefault(bmp => bmp.DocumentToProcessId == command.DocumentId &&
                                        bmp.Order == command.PageNumber);

            if (pageForDocument == null)
            {
                _context.DocumentBitmaps.Add(new DocumentToProcessBitmap
                {
                    DocumentToProcessId = command.DocumentId,
                    FileLabel = savedBitmapPath.FileLabel,
                    BitmapPath = savedBitmapPath.Path,
                    Order = command.PageNumber
                });
            }
            else
            {
                pageForDocument.BitmapPath = savedBitmapPath.Path;
                pageForDocument.FileLabel = savedBitmapPath.FileLabel;
                _context.Update(pageForDocument);
            }
            _context.SaveChanges();
        }

        private string EnsureDirectoryForDocumentBitmaps(SavePageBitmapForDocumentToProcess command)
        {
            var directory = Path.Combine(_configuration.Value.UserDocumentsPath, command.DocumentId + "/");
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            return directory;
        }

        private static (string Path, string FileLabel) WriteBitmapToFilesystem
            (SavePageBitmapForDocumentToProcess command, string directory)
        {
            var path = Path.Combine(directory, command.PageNumber + command.FileType.AsFileExtension());
            File.WriteAllBytes(path, command.FileData);
            return (path, command.FileLabel);
        }


    }
}
