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
    [Route("api/user-documents")]
    public class UserDocumentsController : Controller
    {
        private readonly IFileStorageCommandHandler<SaveBitmapsForDocumentToProcess, Nothing>
            _saveDocumentBitmapsHandler;
        private readonly IFileStorageCommandHandler<SavePageBitmapForDocumentToProcess, Nothing> 
            _saveDocumentPageHandler;
        private readonly IFileStorageQueryHandler<GetBitmapsForDocumentToProcess, ICollection<OrderedBitmapResource>> 
            _getDocumentsBitmapHandler;

        public UserDocumentsController(
            IFileStorageCommandHandler<SaveBitmapsForDocumentToProcess, Nothing> saveDocumentBitmapsHandler,
            IFileStorageCommandHandler<SavePageBitmapForDocumentToProcess, Nothing> saveDocumentPageHandler,
            IFileStorageQueryHandler<GetBitmapsForDocumentToProcess, ICollection<OrderedBitmapResource>> getDocumentsBitmapHandler)
        {
            if (saveDocumentBitmapsHandler == null) throw new ArgumentNullException(nameof(saveDocumentBitmapsHandler));
            if (saveDocumentPageHandler == null) throw new ArgumentNullException(nameof(saveDocumentPageHandler));
            if (getDocumentsBitmapHandler == null) throw new ArgumentNullException(nameof(getDocumentsBitmapHandler));

            _saveDocumentBitmapsHandler = saveDocumentBitmapsHandler;
            _saveDocumentPageHandler = saveDocumentPageHandler;
            _getDocumentsBitmapHandler = getDocumentsBitmapHandler;
        }

        [HttpPost]
        [Route("document-to-process")]
        public IActionResult SaveDocumentToProcessResources([FromBody]SaveBitmapsForDocumentToProcess expectedData)
        {
            try
            {
                _saveDocumentBitmapsHandler.Handle(expectedData);
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

        [HttpPost]
        [Route("document-to-process-page")]
        public IActionResult SavePageForDocumentToProcess([FromBody]SavePageBitmapForDocumentToProcess expectedData)
        {
            try
            {
                _saveDocumentPageHandler.Handle(expectedData);
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

        [HttpGet("document-to-process/{documentId:guid}")]
        public IActionResult GetDocumentToProcessResources([FromRoute] Guid documentId)
        {
            try
            {
                var queryResult =
                    _getDocumentsBitmapHandler.Handle(new GetBitmapsForDocumentToProcess {DocumentId = documentId});
                
                return new JsonResult(new FileStorageRequestResult<ICollection<OrderedBitmapResource>>
                {
                    Success = true,
                    Result = queryResult
                });
            }
            catch
            {
                return new JsonResult(new FileStorageRequestResult<ICollection<OrderedBitmapResource>>
                {
                    Success = false
                });
            }
        }
    }
}
