using System;
using System.Collections.Generic;
using System.Linq;
using Bureaucratize.Common.Core.CommonDetails;
using Bureaucratize.Common.Core.Infrastructure;
using Bureaucratize.Common.Core.Infrastructure.Common;
using Bureaucratize.Common.Core.Infrastructure.FileStore;
using Bureaucratize.Common.Core.Infrastructure.ResultMessages;
using Bureaucratize.FileStorage.Contracts;
using Bureaucratize.FileStorage.Contracts.Models;
using Bureaucratize.FileStorage.Contracts.Queries;
using Bureaucratize.ImageProcessing.Contracts.Bitmaps;
using Bureaucratize.ImageProcessing.Core.Common;
using Bureaucratize.ImageProcessing.Core.Document;
using Bureaucratize.ImageProcessing.Core.Queries;
using Bureaucratize.ImageProcessing.Infrastructure.EntityFramework;
using Bureaucratize.ImageProcessing.Infrastructure.EntityFramework.PersistenceModels;
using Bureaucratize.Templating.Core.Infrastructure.Queries;
using Bureaucratize.Templating.Core.Template;

namespace Bureaucratize.ImageProcessing.Infrastructure.QueryHandlers
{
    public class GetDocumentToProcessHandler : IQueryHandler<GetDocumentToProcess, DocumentToProcess>
    {
        private readonly IImageProcessingPersistenceConfiguration _persistenceConfiguration;
        private readonly IResourceQueryHandler<GetBitmapsForDocumentToProcess, 
            FileStorageRequestResult<ICollection<OrderedBitmapResource>>> _getDocumentBitmapsHandler;
        private readonly IQueryHandler<GetTemplateDefinitionById, TemplateDefinition> _getTemplateById;

        public GetDocumentToProcessHandler(IImageProcessingPersistenceConfiguration persistenceConfiguration,
            IResourceQueryHandler<GetBitmapsForDocumentToProcess, 
                FileStorageRequestResult<ICollection<OrderedBitmapResource>>> getDocumentBitmapsHandler,
            IQueryHandler<GetTemplateDefinitionById, TemplateDefinition> getTemplateById)
        {
            if (persistenceConfiguration == null) throw new ArgumentNullException(nameof(persistenceConfiguration));
            if (getDocumentBitmapsHandler == null) throw new ArgumentNullException(nameof(getDocumentBitmapsHandler));
            if (getTemplateById == null) throw new ArgumentNullException(nameof(getTemplateById));

            _persistenceConfiguration = persistenceConfiguration;
            _getDocumentBitmapsHandler = getDocumentBitmapsHandler;
            _getTemplateById = getTemplateById;
        }

        public OperationResult<DocumentToProcess> Handle(GetDocumentToProcess query)
        {
            if (query == null || query.DocumentId == Guid.Empty)
                return OperationResult<DocumentToProcess>.Failure(new EmptyInput());
         
            DocumentToProcessPersistenceModel documentToProcessPersistenceModel;
            using (var context = new ImageProcessingContext(_persistenceConfiguration))
            {
                documentToProcessPersistenceModel =
                    context.DocumentsToProcess.Single(doc => doc.Id == query.DocumentId);
            }

            //TODO: Separating .Templating and .ImageProcessing into microservices will force this query to be HTTP request, not DB call
            var getTemplateResult = _getTemplateById.Handle(new GetTemplateDefinitionById
                {TemplateId = documentToProcessPersistenceModel.TemplateDefinitionIdentifier});

            if (!getTemplateResult.Successful)
            {
                return OperationResult<DocumentToProcess>.Failure(getTemplateResult.Details);
            }

            TemplateDefinition templateDefinition = getTemplateResult.Result;

            var bitmaps = _getDocumentBitmapsHandler.Handle(new GetBitmapsForDocumentToProcess
            {
                DocumentId = query.DocumentId
            });

            if (!bitmaps.Success)
            {
                //TODO: FileStorageQueryFailed
                return OperationResult<DocumentToProcess>.Failure(null);
            }

            var documentToProcess = new DocumentToProcess(documentToProcessPersistenceModel.RequesterIdentifier,
                templateDefinition, documentToProcessPersistenceModel.Id);

            foreach (var bitmap in bitmaps.Result)
            {
                documentToProcess.AddDocumentPageBitmap(bitmap.AsOrderedBitmap());
            }

            return OperationResult<DocumentToProcess>.Success(documentToProcess);
        }
    }
}
