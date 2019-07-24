///*
//   Copyright (c) 2018 Michał Wilczyński

//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at

//       http://www.apache.org/licenses/LICENSE-2.0

//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
//*/

//using System;
//using System.Drawing;
//using System.Threading.Tasks;
//using Bureaucratize.Common.Core.Infrastructure.Common;
//using Bureaucratize.Common.Core.Infrastructure.FileStore;
//using Bureaucratize.Common.Core.Infrastructure.ResultMessages;
//using Bureaucratize.Common.Core.Utils;
//using Bureaucratize.FileStorage.Contracts.Commands;
//using Bureaucratize.FileStorage.Contracts.Commands.Base;
//using Bureaucratize.Templating.Core.Infrastructure.Commands;
//using Bureaucratize.Templating.Core.Infrastructure.Queries;
//using Bureaucratize.Templating.Core.ResultMessages;
//using Bureaucratize.Templating.Core.Template;
//using Bureaucratize.Templating.Core.Template.Contracts;

//namespace Bureaucratize.Templating.Infrastructure.CommandHandlers
//{
//    public class CreatePageCanvasBasedOnBitmapHandler : ICommandHandler<CreatePageCanvasBasedOnBitmap, TemplatePageDefinition>
//    {
//        private readonly IResourceCommandHandler<SaveBitmapForTemplatePageCanvasDefinition, Nothing> _saveImage;
//        private readonly IQueryHandler<GetPageDefinitionByIdQuery, ITemplatePageDefinition> _getTemplatePage;
//        private readonly IPersistPageCanvasDefinition _persistCanvasDefinition;

//        public CreatePageCanvasBasedOnBitmapHandler(
//            IResourceCommandHandler<SaveBitmapForTemplatePageCanvasDefinition, Nothing> saveImage,
//            IQueryHandler<GetPageDefinitionByIdQuery, ITemplatePageDefinition> getTemplatePage,
//            IPersistPageCanvasDefinition persistCanvasDefinition)
//        {
//            if (saveImage == null)
//                throw new ArgumentNullException(nameof(saveImage));
//            if (getTemplatePage == null)
//                throw new ArgumentNullException(nameof(getTemplatePage));
//            if (persistCanvasDefinition == null)
//                throw new ArgumentNullException(nameof(persistCanvasDefinition));

//            _saveImage = saveImage;
//            _getTemplatePage = getTemplatePage;
//            _persistCanvasDefinition = persistCanvasDefinition;
//        }

//        //TODO: Return some sort of message instead of simply returning object?
//        private TemplateModificationResult<TemplatePageCanvasDefinition> Convert
//            (SaveBitmapCommand imageSentByUser, Guid pageThatWillUseCanvas)
//        {
//            var persistImageResult = _saveImage.Handle(new SaveBitmapForTemplatePageCanvasDefinition
//            {
//                FileData = imageSentByUser.FileData,
//                FileLabel = imageSentByUser.FileLabel,
//                FileType = imageSentByUser.FileType
//            });

//            //if (!persistImageResult.Successful)
//            //{
//            //    return TemplateModificationResult<TemplatePageCanvasDefinition>.Failure(persistImageResult.Details);
//            //}

//            var getCanvasPageResult = _getTemplatePage.Handle(new GetPageDefinitionByIdQuery { PageId = pageThatWillUseCanvas });
//            if (!getCanvasPageResult.Successful)
//            {
//                return TemplateModificationResult<TemplatePageCanvasDefinition>.Failure(getCanvasPageResult.Details);
//            }

//            using (var bitmap = imageSentByUser.FileData.AsBitmap())
//            {
//                //TODO: Persistence layer should know of concrete class, not interface
//                var modificationResult = ((TemplatePageDefinition)getCanvasPageResult.Result)
//                    .ModifyReferenceCanvas(new Rectangle(0, 0, bitmap.Width, bitmap.Height));

//                return modificationResult;
//            }
//        }

//        public OperationResult<Nothing> Handle(CreatePageCanvasBasedOnBitmap command)
//        {
//            var canvasConvertResult = Convert(command.CanvasBitmap, command.PageId);

//            if (canvasConvertResult.Successful)
//            {
//                _persistCanvasDefinition.Canvas = canvasConvertResult.Result;
//                //TODO: Probably should give command a hint if this is insert or update?
//                var modificationPersistResult = _persistCanvasDefinition.ExecuteAsync();

//                if (!modificationPersistResult.Successful)
//                {
//                    return TemplateModificationResult<TemplatePageCanvasDefinition>.Failure(modificationPersistResult
//                        .Details);
//                }

//                //TODO: Consider - Could use some normalization (to usable proportions, rounding etc)?
//                return TemplateModificationResult<TemplatePageCanvasDefinition>.Success(canvasConvertResult.Result);
//            }
//            else
//            {
//                return TemplateModificationResult<TemplatePageCanvasDefinition>.Failure(canvasConvertResult.Details);
//            }
//        }
//    }
//}
