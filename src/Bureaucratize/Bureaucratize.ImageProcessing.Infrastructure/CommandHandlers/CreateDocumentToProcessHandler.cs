using System;
using Bureaucratize.Common.Core.CommonDetails;
using Bureaucratize.Common.Core.Infrastructure;
using Bureaucratize.Common.Core.Infrastructure.Common;
using Bureaucratize.Common.Core.Infrastructure.ResultMessages;
using Bureaucratize.ImageProcessing.Core.Commands;
using Bureaucratize.ImageProcessing.Core.Document;
using Bureaucratize.ImageProcessing.Infrastructure.EntityFramework;
using Bureaucratize.ImageProcessing.Infrastructure.EntityFramework.PersistenceModels;

namespace Bureaucratize.ImageProcessing.Infrastructure.CommandHandlers
{
    public class CreateDocumentToProcessHandler : ICommandHandler<CreateDocumentToProcess, Nothing>
    {
        private readonly IImageProcessingPersistenceConfiguration _persistenceConfiguration;

        public CreateDocumentToProcessHandler(IImageProcessingPersistenceConfiguration persistenceConfiguration)
        {
            if (persistenceConfiguration == null) throw new ArgumentNullException(nameof(persistenceConfiguration));

            _persistenceConfiguration = persistenceConfiguration;
        }

        public OperationResult<Nothing> Handle(CreateDocumentToProcess command)
        {
            using (var context = new ImageProcessingContext(_persistenceConfiguration))
            {
                try
                {
                    var document = new DocumentToProcessPersistenceModel
                    {
                        Id = command.Id,
                        RequesterIdentifier = command.RequesterIdentifier,
                        TemplateDefinitionIdentifier = command.TemplateDefinitionIdentifier
                    };

                    context.DocumentsToProcess.Add(document);
                    context.SaveChanges();

                    return OperationResult<Nothing>.Success(new Nothing());
                }
                catch (Exception ex)
                {
                    return OperationResult<Nothing>.Failure(new UncaughtException(ex));
                }
            }
        }
    }
}
