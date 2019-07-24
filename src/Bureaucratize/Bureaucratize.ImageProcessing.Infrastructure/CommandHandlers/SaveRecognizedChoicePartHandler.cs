using System;
using Bureaucratize.Common.Core.CommonDetails;
using Bureaucratize.Common.Core.Infrastructure.Common;
using Bureaucratize.Common.Core.Infrastructure.Common.Shortcuts;
using Bureaucratize.Common.Core.Infrastructure.ResultMessages;
using Bureaucratize.ImageProcessing.Contracts.Recognition;
using Bureaucratize.ImageProcessing.Core.Recognition;
using Bureaucratize.ImageProcessing.Infrastructure.EntityFramework;

namespace Bureaucratize.ImageProcessing.Infrastructure.CommandHandlers
{
    public class SaveRecognizedChoicePartHandler : ICommandHandler<SaveDomainObject<RecognizedChoicePart>>
    {
        private readonly IImageProcessingPersistenceConfiguration _persistenceConfiguration;

        public SaveRecognizedChoicePartHandler(IImageProcessingPersistenceConfiguration persistenceConfiguration)
        {
            if (persistenceConfiguration == null)
                throw new ArgumentNullException(nameof(persistenceConfiguration));
            _persistenceConfiguration = persistenceConfiguration;
        }

        public OperationResult<Nothing> Handle(SaveDomainObject<RecognizedChoicePart> command)
        {
            using (var context = new ImageProcessingContext(_persistenceConfiguration))
            {
                try
                {
                    context.Set<RecognizedChoicePart>().Add(command.ObjectToSave);
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
