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
using System.Drawing;
using Accord.Imaging;
using Accord.Imaging.Filters;
using Bureaucratize.Common.Core.CommonDetails;
using Bureaucratize.ImageProcessing.Contracts.Bitmaps;
using Bureaucratize.ImageProcessing.Contracts.ProcessingMessages;
using Bureaucratize.ImageProcessing.Contracts.ProcessingMessages.Details;
using Bureaucratize.ImageProcessing.Core.Cropping.Contracts;
using Bureaucratize.Templating.Core.Template.Contracts;

namespace Bureaucratize.ImageProcessing.Core.Cropping
{
    public class TemplateAreasCropper : ITemplateAreasCropper
    {
        public ProcessingResult<ICollection<ICroppedArea>> CropUserInput(Bitmap bitmapToCropIntoParts, Guid documentId, 
            ITemplatePageDefinition definitionForCropping)
        {
            if (definitionForCropping == null)
            {
                return ProcessingResult<ICollection<ICroppedArea>>.Failure(new PageDefinitionNotProvided());
            }

            if (documentId == Guid.Empty)
            {
                return ProcessingResult<ICollection<ICroppedArea>>.Failure(new MissingDocumentId());
            }

            //Can this be more granular without downloading bitmap all over again?
            //Maybe parent holding bitmap and just passing it over to all children?
                using (bitmapToCropIntoParts)
                {
                    var results = new List<ICroppedArea>();
                    foreach (var templateArea in definitionForCropping.DefinedAreas)
                    {
                        try
                        {
                            var croppedAreaParts = new List<OrderedBitmap>();

                            if (templateArea.InterestPoints == null || templateArea.InterestPoints.Count == 0)
                            {
                                croppedAreaParts.Add(
                                    new OrderedBitmap(0, new Crop(templateArea.AreaDimension).Apply(bitmapToCropIntoParts)));
                            }
                            else
                            {
                                foreach (var areaPart in templateArea.InterestPoints)
                                {
                                    croppedAreaParts.Add(
                                        new OrderedBitmap(areaPart.OrderInArea, new Crop(areaPart.Dimension).Apply(bitmapToCropIntoParts)));
                                }
                            }

                            results.Add(new CroppedArea(templateArea, croppedAreaParts, documentId));
                        }
                        catch (UnsupportedImageFormatException)
                        {
                            return ProcessingResult<ICollection<ICroppedArea>>.Failure(new UnsupportedImageFormat());
                        }
                        catch (Exception ex)
                        {
                            ProcessingResult<ICollection<ICroppedArea>>.Failure(new UncaughtException(ex));
                        }
                    }
                    return ProcessingResult<ICollection<ICroppedArea>>.Success(results);
            }
        }
    }
}
