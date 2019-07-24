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

using Autofac;
using Bureaucratize.ImageProcessing.Core.Cleaning;
using Bureaucratize.ImageProcessing.Core.Cleaning.Contracts;
using Bureaucratize.ImageProcessing.Core.Cropping;
using Bureaucratize.ImageProcessing.Core.Cropping.Contracts;
using Bureaucratize.ImageProcessing.Core.Extracting;
using Bureaucratize.ImageProcessing.Core.Extracting.Contracts;
using Bureaucratize.ImageProcessing.Core.Recognition;
using Bureaucratize.ImageProcessing.Core.Recognition.Contracts;
using Bureaucratize.ImageProcessing.Host.Actors;

namespace Bureaucratize.ImageProcessing.Host.IoC.Modules
{
    public class ImageProcessingDependenciesModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ImageDifferenceHandwrittenInputExtractor>()
                .As<IHandwrittenInputExtractor>();

            builder.RegisterType<TemplateAreasCropper>()
                .As<ITemplateAreasCropper>();

            builder.RegisterType<BradleyLocalThresholdingCleaner>()
                .As<ICroppedAreaCleaner>();

            builder.RegisterType<EmnistCroppedAreaScaler>()
                .As<ICroppedAreaScaler>();

            builder.RegisterType<RegionOfInterestExtractor>()
                .As<IRegionOfInterestExtractor>();

            //Simple composition of recognition presteps that were registered above
            builder.RegisterType<ImageProcessingPreparationSteps>();

            builder.RegisterType<DeepLearningLabelMapConfiguration>();

            builder.RegisterType<DeepLearningHandwrittenInputRecognizer>()
                .As<IHandwrittenInputRecognizer>();

            builder.RegisterType<FindAnyInputHandwrittenChoiceRecognizer>()
                .As<IHandwrittenChoiceRecognizer>();
        }
    }
}
