using System;
using Bureaucratize.Common.Core.CommonDetails;
using Bureaucratize.Common.Core.Infrastructure.Common;
using Bureaucratize.Common.Core.Infrastructure.Common.Shortcuts;
using Bureaucratize.Common.Core.Infrastructure.ResultMessages;
using Bureaucratize.ImageProcessing.Contracts.ProcessingMessages.ProcessingOutcomes.Models;
using Bureaucratize.ImageProcessing.Core.Document;
using Bureaucratize.ImageProcessing.Infrastructure.EntityFramework;

namespace Bureaucratize.ImageProcessing.Infrastructure.CommandHandlers
{
    public class SaveProcessedDocumentPageHandler : ICommandHandler<SaveDomainObject<ProcessedDocumentPage>>
    {
        private readonly IImageProcessingPersistenceConfiguration _persistenceConfiguration;

        public SaveProcessedDocumentPageHandler(IImageProcessingPersistenceConfiguration persistenceConfiguration)
        {
            if (persistenceConfiguration == null) throw new ArgumentNullException(nameof(persistenceConfiguration));

            _persistenceConfiguration = persistenceConfiguration;
        }

        public OperationResult<Nothing> Handle(SaveDomainObject<ProcessedDocumentPage> command)
        {
            using (var context = new ImageProcessingContext(_persistenceConfiguration))
            {
                try
                {
                    context.ProcessedPages.Add(command.ObjectToSave);
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
