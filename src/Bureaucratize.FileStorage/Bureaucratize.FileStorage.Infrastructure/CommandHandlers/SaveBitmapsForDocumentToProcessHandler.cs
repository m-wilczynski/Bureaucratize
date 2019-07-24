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
    public class SaveBitmapsForDocumentToProcessHandler : IFileStorageCommandHandler<SaveBitmapsForDocumentToProcess, Nothing>
    {
        private readonly IOptions<PersistenceConfiguration> _configuration;
        private readonly FileStorageContext _context;

        public SaveBitmapsForDocumentToProcessHandler(IOptions<PersistenceConfiguration> configuration, 
            FileStorageContext context)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            if (context == null) throw new ArgumentNullException(nameof(context));

            _configuration = configuration;
            _context = context;
        }

        public Nothing Handle(SaveBitmapsForDocumentToProcess command)
        {
            using (_context)
            {
                Dictionary<int, (string Path, string FileLabel)> savedBitmapsPaths = null;
                try
                {
                    string directory = EnsureDirectoryForDocumentBitmaps(command);
                    savedBitmapsPaths = WriteAllBitmapsToFilesystem(command, directory);
                    UpsertBitmapInfoToDb(command, savedBitmapsPaths);
                }
                catch (Exception ex)
                {
                    foreach (var fileInfo in savedBitmapsPaths.Values)
                    {
                        File.Delete(fileInfo.Path);
                    }
                    throw;
                }
                return new Nothing();
            }
        }

        private void UpsertBitmapInfoToDb(SaveBitmapsForDocumentToProcess command, Dictionary<int, (string Path, string FileLabel)> savedBitmapsPaths)
        {
            foreach (var fileInfo in savedBitmapsPaths)
            {
                var pageForDocument = _context.DocumentBitmaps
                    .SingleOrDefault(bmp => bmp.DocumentToProcessId == command.DocumentToProcessId &&
                                                 bmp.Order == fileInfo.Key);

                if (pageForDocument == null)
                {
                    _context.DocumentBitmaps.Add(new DocumentToProcessBitmap
                    {
                        DocumentToProcessId = command.DocumentToProcessId,
                        FileLabel = fileInfo.Value.FileLabel,
                        BitmapPath = fileInfo.Value.Path,
                        Order = fileInfo.Key,
                    });
                }
                else
                {
                    pageForDocument.BitmapPath = fileInfo.Value.Path;
                    pageForDocument.FileLabel = fileInfo.Value.FileLabel;
                    _context.Update(pageForDocument);
                }
                _context.SaveChanges();
            }
        }

        private string EnsureDirectoryForDocumentBitmaps(SaveBitmapsForDocumentToProcess command)
        {
            var directory = Path.Combine(_configuration.Value.UserDocumentsPath, command.DocumentToProcessId + "/");
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            return directory;
        }

        private static Dictionary<int, (string Path, string FileLabel)> WriteAllBitmapsToFilesystem(SaveBitmapsForDocumentToProcess command, string directory)
        {
            var savedBitmapsPaths = new Dictionary<int, (string Path, string FileLabel)>();

            foreach (var bitmap in command.OrderedBitmaps)
            {
                var path = Path.Combine(directory, bitmap.Order + bitmap.FileType.AsFileExtension());

                File.WriteAllBytes(path, bitmap.FileData);

                savedBitmapsPaths.Add(bitmap.Order, (path, bitmap.FileLabel));
            }

            return savedBitmapsPaths;
        }
    }
}
