/*
   Copyright (c) 2018 Michał Wilczyński

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using Bureaucratize.Common.Core;
using Bureaucratize.ImageProcessing.Contracts.Bitmaps;
using Bureaucratize.ImageProcessing.Contracts.ProcessingMessages;
using Bureaucratize.ImageProcessing.Contracts.ProcessingMessages.Details;
using Bureaucratize.ImageProcessing.Core.Common;
using Bureaucratize.Templating.Core.Template.Contracts;

namespace Bureaucratize.ImageProcessing.Core.Document
{
    public class DocumentToProcess : Identifiable
    {
        private readonly IDictionary<int, OrderedBitmap> _documentPages = new Dictionary<int, OrderedBitmap>();

        public DocumentToProcess(Guid requesterIdentifier, ITemplateDefinition templateDefinition,
            Guid? id = null) : base(id)
        {
            if (requesterIdentifier == Guid.Empty)
                throw new ArgumentNullException(nameof(requesterIdentifier));
            if (templateDefinition == null)
                throw new ArgumentNullException(nameof(templateDefinition));

            RequesterIdentifier = requesterIdentifier;
            TemplateDefinition = templateDefinition;
        }

        public Guid RequesterIdentifier { get; private set; }
        public ITemplateDefinition TemplateDefinition { get; private set; }
        public IReadOnlyCollection<OrderedBitmap> DocumentPages => _documentPages.Values.ToList();

        public bool CanDocumentBeProcessed
        {
            get
            {
                return TemplateDefinition.DefinedPages.Select(page => page.Key)
                    .SequenceEqual(DocumentPages.Select(page => (int)page.Order));
            }
        }

        public DocumentModificationResult AddDocumentPageBitmap(OrderedBitmap bitmap)
        {
            if (_documentPages.ContainsKey((int)bitmap.Order))
                return DocumentModificationResult.Failure(new AddedBitmapForPageTwice((int)bitmap.Order));

            return ReplaceDocumentPageBitmap(bitmap);
        }

        public DocumentModificationResult ReplaceDocumentPageBitmap(OrderedBitmap bitmap)
        {
            if (!TemplateDefinition.DefinedPages.ContainsKey((int)bitmap.Order))
                return DocumentModificationResult.Failure(new AddedBitmapForNonexistentPage((int)bitmap.Order));

            if (_documentPages.ContainsKey((int)bitmap.Order))
            {
                _documentPages[(int)bitmap.Order] = bitmap;
            }
            else
            {
                _documentPages.Add((int)bitmap.Order, bitmap);
            }

            return DocumentModificationResult.Success();
        }
    }
}
