using System;
using System.IO;
using System.Linq;
using Bureaucratize.FileStorage.Contracts;
using Bureaucratize.FileStorage.Contracts.Commands;
using Bureaucratize.FileStorage.Infrastructure.EntityFramework;
using Bureaucratize.FileStorage.Infrastructure.EntityFramework.PersistenceModels;
using Microsoft.Extensions.Options;

namespace Bureaucratize.FileStorage.Infrastructure.CommandHandlers
{
    public class SaveBitmapForTemplatePageCanvasDefinitionHandler 
        : IFileStorageCommandHandler<SaveBitmapForTemplatePageCanvasDefinition, Nothing>
    {
        private readonly IOptions<PersistenceConfiguration> _configuration;
        private readonly FileStorageContext _context;

        public SaveBitmapForTemplatePageCanvasDefinitionHandler(IOptions<PersistenceConfiguration> configuration,
            FileStorageContext context)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            if (context == null) throw new ArgumentNullException(nameof(context));

            _configuration = configuration;
            _context = context;
        }

        public Nothing Handle(SaveBitmapForTemplatePageCanvasDefinition command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));
            if (command.TemplateId == Guid.Empty)
                throw new ArgumentNullException(nameof(command.TemplateId));
            if (command.TemplatePageId == Guid.Empty)
                throw new ArgumentNullException(nameof(command.TemplatePageId));

            using (_context)
            {
                var savedBitmapsPath = default((string Path, string FileLabel));
                try
                {
                    string directory = EnsureDirectoryForTemplate(command);
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

        private void UpsertBitmapInfoToDb(SaveBitmapForTemplatePageCanvasDefinition command,
            (string Path, string FileLabel) savedBitmapPath)
        {
            var pageCanvas = _context.PageCanvasBitmaps
                .SingleOrDefault(bmp => bmp.TemplatePageId == command.TemplatePageId);

            if (pageCanvas == null)
            {
                _context.PageCanvasBitmaps.Add(new PageCanvasBitmap
                {
                    TemplatePageId = command.TemplatePageId,
                    TemplateId = command.TemplateId,
                    FileLabel = savedBitmapPath.FileLabel,
                    BitmapPath = savedBitmapPath.Path,
                });
            }
            else
            {
                pageCanvas.BitmapPath = savedBitmapPath.Path;
                pageCanvas.FileLabel = savedBitmapPath.FileLabel;
                _context.Update(pageCanvas);
            }
            _context.SaveChanges();
        }

        private string EnsureDirectoryForTemplate(SaveBitmapForTemplatePageCanvasDefinition command)
        {
            var directory = Path.Combine(_configuration.Value.TemplateFilesPath, "pages/" + command.TemplatePageId + "/");
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            return directory;
        }

        private static (string Path, string FileLabel) WriteBitmapToFilesystem
            (SaveBitmapForTemplatePageCanvasDefinition command, string directory)
        {
            var path = Path.Combine(directory, "canvas" + command.FileType.AsFileExtension());
            File.WriteAllBytes(path, command.FileData);
            return (path, command.FileLabel);
        }
    }
}
