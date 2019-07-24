using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Bureaucratize.FileStorage.Contracts;
using Bureaucratize.FileStorage.Contracts.Commands.Base;
using Bureaucratize.FileStorage.Contracts.Queries;
using Bureaucratize.ImageProcessing.Contracts.RemotingMessages;
using Bureaucratize.ImageProcessing.Core.Commands;
using Bureaucratize.ImageProcessing.Core.Document;
using Bureaucratize.ImageProcessing.Core.Queries;
using Bureaucratize.ImageProcessing.Infrastructure;
using Bureaucratize.ImageProcessing.Infrastructure.CommandHandlers;
using Bureaucratize.ImageProcessing.Infrastructure.QueryHandlers;
using Bureaucratize.ImageProcessing.Infrastructure.ResourceCommandHandlers;
using Bureaucratize.ImageProcessing.Infrastructure.ResourceQueryHandlers;
using Bureaucratize.Templating.Core.Infrastructure.Commands;
using Bureaucratize.Templating.Core.Infrastructure.Queries;
using Bureaucratize.Templating.Core.Template;
using Bureaucratize.Templating.Infrastructure;
using Bureaucratize.Templating.Infrastructure.NetStand;
using Bureaucratize.Templating.Infrastructure.NetStand.CommandHandlers;
using Bureaucratize.Templating.Infrastructure.NetStand.QueryHandlers;
using Bureaucratize.Templating.Infrastructure.NetStand.ResourceCommandHandlers;
using Bureaucratize.Web.Config;
using Bureaucratize.Web.ImageUtils;
using Bureaucratize.Web.Resources;
using Bureaucratize.Web.ViewModels;
using Bureaucratize.Web.ViewShortcuts;
using Microsoft.AspNetCore.Mvc;

namespace Bureaucratize.Web.Controllers
{
    public class HomeController : Controller
    {

        private readonly IImageProcessingPersistenceConfiguration ImageProcessingConfig;
        private readonly ITemplatingPersistenceConfiguration TemplatingConfig;
        private readonly PrepareTemplates _prepareTemplates;

        public HomeController(IImageProcessingPersistenceConfiguration imageProcessingConfig, 
            ITemplatingPersistenceConfiguration templatingConfig,
            PrepareTemplates prepareTemplates)
        {
            if (imageProcessingConfig == null) throw new ArgumentNullException(nameof(imageProcessingConfig));
            if (templatingConfig == null) throw new ArgumentNullException(nameof(templatingConfig));
            if (prepareTemplates == null) throw new ArgumentNullException(nameof(prepareTemplates));

            ImageProcessingConfig = imageProcessingConfig;
            TemplatingConfig = templatingConfig;
            _prepareTemplates = prepareTemplates;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult DocumentProcessing(DocumentProcessingViewModel viewModel)
        {
            return View(viewModel);
        }

        public IActionResult StartDocumentProcessing(Guid id)
        {
            DocumentSystemActors.RemoteImageProcessing.Ask<ProcessDocumentOfIdRequest>(
                new ProcessDocumentOfIdRequest(
                    id,
                Guid.NewGuid()));

            return Ok(new { Success = true });
        }

        public IActionResult DocumentProcessed()
        {
            return View(Guid.NewGuid());
        }

        public IActionResult GetReferenceImage()
        {
            return File(ExampleTemplates.pekao_loan_template, "image/jpeg");
        }

        public IActionResult GetDocumentImage(Guid id)
        {
            var result = new GetDocumentToProcessResourcesHandler(ImageProcessingConfig)
                .Handle(new GetBitmapsForDocumentToProcess
                {
                    DocumentId = id
                }).Result?.FirstOrDefault();

            if (result == null)
                return NotFound();

            return File(result.FileData, result.Filetype.AsMimeType());
        }

        public IActionResult UseValidWorkingFile()
        {
            var template = _prepareTemplates.CreateTemplateDefinition();

            var document = AddDocument(template.Id, ExampleUserInputs.pekao_loan_input_3, 
                nameof(ExampleUserInputs.pekao_loan_input_3) + ".jpg", BitmapFiletype.Jpg);

            return View("DocumentProcessing", new DocumentProcessingViewModel
            {
                Document = document,
                DocumentTemplate = template
            });
        }

        [HttpPost]
        public async Task<IActionResult> UploadCustomUserData(UploadInputFromUserViewModel userInput)
        {
            if (!ModelState.IsValid)
            {
                return View("Index");
            }

            var template = _prepareTemplates.CreateTemplateDefinition();

            byte[] data;
            using (var memoryStream = new MemoryStream())
            {
                await userInput.UserUpload.CopyToAsync(memoryStream);
                data = memoryStream.ToArray();
            }

            var document = AddDocument(template.Id, data, userInput.UserUpload.FileName, BitmapFiletype.Jpg);

            return View("DocumentProcessing", new DocumentProcessingViewModel
            {
                Document = document,
                DocumentTemplate = template
            });
        }

        public DocumentToProcess AddDocument(Guid templateId, byte[] fileData, string fileLabel, BitmapFiletype fileType)
        {
            var createDocument = new CreateDocumentToProcess
            {
                Id = Guid.NewGuid(),
                RequesterIdentifier = Guid.NewGuid(),
                TemplateDefinitionIdentifier = templateId
            };

            var createDocumentResult = new CreateDocumentToProcessHandler(ImageProcessingConfig).Handle(createDocument);

            var addBitmapToDocument = new AddBitmapForDocumentToProcess
            {
                DocumentId = createDocument.Id,
                OrderedBitmap = new OrderedBitmapToSave
                {
                    FileData = fileData,
                    Order = 1,
                    FileLabel = fileLabel,
                    FileType = fileType
                }
            };

            var getDocumentHandler = new GetDocumentToProcessHandler(ImageProcessingConfig,
                new GetDocumentToProcessResourcesHandler(ImageProcessingConfig),
                new GetTemplateDefinitionByIdHandler(TemplatingConfig));

            var addBitmapToDocumentResult = new AddBitmapForDocumentToProcessHandler
                    (getDocumentHandler, new SavePageBitmapForDocumentToProcessHandler(ImageProcessingConfig))
                .Handle(addBitmapToDocument);

            return getDocumentHandler.Handle(new GetDocumentToProcess
            {
                DocumentId = createDocument.Id
            }).Result;
        }
    }
}
