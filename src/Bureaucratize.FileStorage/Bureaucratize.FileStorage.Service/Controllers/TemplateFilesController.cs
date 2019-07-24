using System;
using System.Collections.Generic;
using Bureaucratize.FileStorage.Contracts;
using Bureaucratize.FileStorage.Contracts.Commands;
using Bureaucratize.FileStorage.Contracts.Models;
using Bureaucratize.FileStorage.Contracts.Queries;
using Bureaucratize.FileStorage.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Bureaucratize.FileStorage.Service.Controllers
{
    [Route("api/template-files")]
    public class TemplateFilesController : Controller
    {
        private readonly IFileStorageCommandHandler<SaveBitmapForTemplatePageCanvasDefinition, Nothing> _savePageCanvasBitmapHandler;
        private readonly IFileStorageQueryHandler<GetCanvasesBitmapsForTemplate, 
            ICollection<TemplatePageCanvasBitmapResource>> _getCanvasesForTemplateHandler;
        private readonly IFileStorageQueryHandler<GetCanvasBitmapForTemplatePage, 
            TemplatePageCanvasBitmapResource> _getCanvaseForTemplatePageHandler;

        public TemplateFilesController(
            IFileStorageCommandHandler<SaveBitmapForTemplatePageCanvasDefinition, Nothing> savePageCanvasBitmapHandler,
            IFileStorageQueryHandler<GetCanvasesBitmapsForTemplate, ICollection<TemplatePageCanvasBitmapResource>> getCanvasesForTemplateHandler,
            IFileStorageQueryHandler<GetCanvasBitmapForTemplatePage, TemplatePageCanvasBitmapResource> getCanvaseForTemplatePageHandler)
        {
            if (savePageCanvasBitmapHandler == null)
                throw new ArgumentNullException(nameof(savePageCanvasBitmapHandler));
            if (getCanvasesForTemplateHandler == null)
                throw new ArgumentNullException(nameof(getCanvasesForTemplateHandler));
            if (getCanvaseForTemplatePageHandler == null)
                throw new ArgumentNullException(nameof(getCanvaseForTemplatePageHandler));

            _savePageCanvasBitmapHandler = savePageCanvasBitmapHandler;
            _getCanvasesForTemplateHandler = getCanvasesForTemplateHandler;
            _getCanvaseForTemplatePageHandler = getCanvaseForTemplatePageHandler;
        }

        [HttpPost]
        [Route("page-canvas-bitmap")]
        public IActionResult SavePageForDocumentToProcess([FromBody]SaveBitmapForTemplatePageCanvasDefinition pageCanvas)
        {
            try
            {
                _savePageCanvasBitmapHandler.Handle(pageCanvas);
                return new JsonResult(new FileStorageRequestResult
                {
                    Success = true
                });
            }
            catch
            {
                return new JsonResult(new FileStorageRequestResult
                {
                    Success = false
                });
            }
        }

        [HttpGet("template-definition/{templateId:guid}/canvases")]
        public IActionResult GetTemplateCanvasesBitmaps([FromRoute] Guid templateId)
        {
            try
            {
                var queryResult =
                    _getCanvasesForTemplateHandler.Handle(new GetCanvasesBitmapsForTemplate { TemplateId = templateId });

                return new JsonResult(new FileStorageRequestResult<ICollection<TemplatePageCanvasBitmapResource>>
                {
                    Success = true,
                    Result = queryResult
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new FileStorageRequestResult<ICollection<TemplatePageCanvasBitmapResource>>
                {
                    Success = false
                });
            }
        }

        [HttpGet("template-page/{templatePageId:guid}/canvas")]
        public IActionResult GetTemplatePageCanvasBitmap([FromRoute] Guid templatePageId)
        {
            try
            {
                var queryResult =
                    _getCanvaseForTemplatePageHandler.Handle(new GetCanvasBitmapForTemplatePage { TemplatePageId = templatePageId });

                return new JsonResult(new FileStorageRequestResult<TemplatePageCanvasBitmapResource>
                {
                    Success = true,
                    Result = queryResult
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new FileStorageRequestResult<TemplatePageCanvasBitmapResource>
                {
                    Success = false
                });
            }
        }

    }
}
