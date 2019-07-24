using System;
using Bureaucratize.FileStorage.Contracts;
using Bureaucratize.FileStorage.Contracts.Commands.Base;
using Bureaucratize.ImageProcessing.Infrastructure;
using Bureaucratize.Templating.Core.Infrastructure.Commands;
using Bureaucratize.Templating.Core.Infrastructure.Queries;
using Bureaucratize.Templating.Core.Template;
using Bureaucratize.Templating.Infrastructure.NetStand;
using Bureaucratize.Templating.Infrastructure.NetStand.CommandHandlers;
using Bureaucratize.Templating.Infrastructure.NetStand.QueryHandlers;
using Bureaucratize.Templating.Infrastructure.NetStand.ResourceCommandHandlers;
using Bureaucratize.Web.Resources;

namespace Bureaucratize.Web.ViewShortcuts
{
    public class PrepareTemplates
    {
        private readonly IImageProcessingPersistenceConfiguration ImageProcessingConfig;
        private readonly ITemplatingPersistenceConfiguration TemplatingConfig;

        public PrepareTemplates(IImageProcessingPersistenceConfiguration imageProcessingConfig,
            ITemplatingPersistenceConfiguration templatingConfig)
        {
            if (imageProcessingConfig == null)
                throw new ArgumentNullException(nameof(imageProcessingConfig));
            if (templatingConfig == null)
                throw new ArgumentNullException(nameof(templatingConfig));

            ImageProcessingConfig = imageProcessingConfig;
            TemplatingConfig = templatingConfig;
        }

        public TemplateDefinition CreateTemplateDefinition()
        {
            var templateDefinition = new CreateTemplateDefinitionHandler(TemplatingConfig)
                .Handle(new CreateTemplateDefinition
                {
                    TemplateCreatorId = Guid.NewGuid(),
                    TemplateName = "Pekao SA - Wniosek kredytowy"
                }).Result;

            var pageDefinitionResult = new AddTemplateDefinitionPageHandler(TemplatingConfig,
                    new SaveBitmapForTemplatePageCanvasHandler(TemplatingConfig))
                .Handle(new AddTemplateDefinitionPage
                {
                    TemplateId = templateDefinition.Id,
                    PageNumber = 1,
                    ReferenceCanvas = new SaveBitmapCommand
                    {
                        FileData = ExampleTemplates.pekao_loan_template,
                        FileLabel = "Pekao - wniosek kredytowy",
                        FileType = BitmapFiletype.Jpg,
                    }
                });

            var pageDefinition = pageDefinitionResult.Result.DefinedPages[1] as TemplatePageDefinition;

            var areaAddResult = new AddTemplatePageAreasHandler(TemplatingConfig)
                .Handle(new AddTemplatePageAreas
                {
                    TemplatePageId = pageDefinition.Id,
                    Areas = new[]
                    {
                        #region Typy wnioskow
                        new AddTemplatePageArea
                        {
                            AreaName = "Typ wniosku - Kredyt hipoteczny",
                            DimensionX = 500,
                            DimensionY = 493,
                            DimensionWidth = 32,
                            DimensionHeight = 24,
                            ExpectedData = TemplatePartExpectedDataType.Choice,
                            TemplatePageId = pageDefinition.Id,
                            AreaParts = new[]
                            {
                                new AddTemplatePageAreaPart
                                {
                                    DimensionX = 500,
                                    DimensionY = 493,
                                    DimensionWidth = 32,
                                    DimensionHeight = 24,
                                    OrderInArea = 1
                                }
                            }
                        },
                        new AddTemplatePageArea
                        {
                            AreaName = "Typ wniosku - Kredyt budowlano-hipoteczny",
                            DimensionX = 500,
                            DimensionY = 525,
                            DimensionWidth = 32,
                            DimensionHeight = 24,
                            ExpectedData = TemplatePartExpectedDataType.Choice,
                            TemplatePageId = pageDefinition.Id,
                            AreaParts = new[]
                            {
                                new AddTemplatePageAreaPart
                                {
                                    DimensionX = 500,
                                    DimensionY = 525,
                                    DimensionWidth = 32,
                                    DimensionHeight = 24,
                                    OrderInArea = 1
                                }
                            }
                        },
                        new AddTemplatePageArea
                        {
                            AreaName = "Typ wniosku - Kredyt mieszkaniowy MDM",
                            DimensionX = 500,
                            DimensionY = 559,
                            DimensionWidth = 32,
                            DimensionHeight = 22,
                            ExpectedData = TemplatePartExpectedDataType.Choice,
                            TemplatePageId = pageDefinition.Id,
                            AreaParts = new[]
                            {
                                new AddTemplatePageAreaPart
                                {
                                    DimensionX = 500,
                                    DimensionY = 559,
                                    DimensionWidth = 32,
                                    DimensionHeight = 22,
                                    OrderInArea = 1
                                }
                            }
                        },
                        new AddTemplatePageArea
                        {
                            AreaName = "Typ wniosku - Eurokonto Hipoteczne Plus",
                            DimensionX = 500,
                            DimensionY = 591,
                            DimensionWidth = 32,
                            DimensionHeight = 24,
                            ExpectedData = TemplatePartExpectedDataType.Choice,
                            TemplatePageId = pageDefinition.Id,
                            AreaParts = new[]
                            {
                                new AddTemplatePageAreaPart
                                {
                                    DimensionX = 500,
                                    DimensionY = 591,
                                    DimensionWidth = 32,
                                    DimensionHeight = 24,
                                    OrderInArea = 1
                                }
                            }
                        },
                        #endregion
                        #region Wnioskodawca - Imie
                        new AddTemplatePageArea
                        {
                            AreaName = "Wnioskodawca 1 - Imie",
                            DimensionX = 365,
                            DimensionY = 821,
                            DimensionWidth = 181,
                            DimensionHeight = 46,
                            ExpectedData = TemplatePartExpectedDataType.Letters,
                            TemplatePageId = pageDefinition.Id,
                            AreaParts = new[]
                            {
                                new AddTemplatePageAreaPart
                                {
                                    DimensionX = 365,
                                    DimensionY = 821,
                                    DimensionWidth = 28,
                                    DimensionHeight = 44,
                                    OrderInArea = 1
                                },
                                new AddTemplatePageAreaPart
                                {
                                    DimensionX = 398,
                                    DimensionY = 821,
                                    DimensionWidth = 24,
                                    DimensionHeight = 44,
                                    OrderInArea = 2
                                },
                                new AddTemplatePageAreaPart
                                {
                                    DimensionX = 427,
                                    DimensionY = 821,
                                    DimensionWidth = 28,
                                    DimensionHeight = 44,
                                    OrderInArea = 3
                                },
                                new AddTemplatePageAreaPart
                                {
                                    DimensionX = 460,
                                    DimensionY = 821,
                                    DimensionWidth = 24,
                                    DimensionHeight = 44,
                                    OrderInArea = 4
                                },
                                new AddTemplatePageAreaPart
                                {
                                    DimensionX = 490,
                                    DimensionY = 821,
                                    DimensionWidth = 24,
                                    DimensionHeight = 44,
                                    OrderInArea = 5
                                },
                                new AddTemplatePageAreaPart
                                {
                                    DimensionX = 520,
                                    DimensionY = 821,
                                    DimensionWidth = 24,
                                    DimensionHeight = 44,
                                    OrderInArea = 6
                                }
                            }
                        },
                        #endregion 
                        #region Wnioskodawca - Nazwisko
                        new AddTemplatePageArea
                        {
                            AreaName = "Wnioskodawca 1 - Nazwisko",
                            DimensionX = 732,
                            DimensionY = 821,
                            DimensionWidth = 422,
                            DimensionHeight = 46,
                            ExpectedData = TemplatePartExpectedDataType.Letters,
                            TemplatePageId = pageDefinition.Id,
                            AreaParts = new[]
                            {
                                new AddTemplatePageAreaPart
                                {
                                    DimensionX = 733,
                                    DimensionY = 821,
                                    DimensionWidth = 24,
                                    DimensionHeight = 44,
                                    OrderInArea = 1
                                },
                                new AddTemplatePageAreaPart
                                {
                                    DimensionX = 761,
                                    DimensionY = 821,
                                    DimensionWidth = 28,
                                    DimensionHeight = 44,
                                    OrderInArea = 2
                                },
                                new AddTemplatePageAreaPart
                                {
                                    DimensionX = 794,
                                    DimensionY = 821,
                                    DimensionWidth = 24,
                                    DimensionHeight = 44,
                                    OrderInArea = 3
                                },
                                new AddTemplatePageAreaPart
                                {
                                    DimensionX = 824,
                                    DimensionY = 821,
                                    DimensionWidth = 24,
                                    DimensionHeight = 44,
                                    OrderInArea = 4
                                },
                                new AddTemplatePageAreaPart
                                {
                                    DimensionX = 856,
                                    DimensionY = 821,
                                    DimensionWidth = 24,
                                    DimensionHeight = 44,
                                    OrderInArea = 5
                                },
                                new AddTemplatePageAreaPart
                                {
                                    DimensionX = 886,
                                    DimensionY = 821,
                                    DimensionWidth = 24,
                                    DimensionHeight = 44,
                                    OrderInArea = 6
                                },
                                new AddTemplatePageAreaPart
                                {
                                    DimensionX = 916,
                                    DimensionY = 821,
                                    DimensionWidth = 24,
                                    DimensionHeight = 44,
                                    OrderInArea = 7
                                },
                                new AddTemplatePageAreaPart
                                {
                                    DimensionX = 946,
                                    DimensionY = 821,
                                    DimensionWidth = 24,
                                    DimensionHeight = 44,
                                    OrderInArea = 8
                                },
                                new AddTemplatePageAreaPart
                                {
                                    DimensionX = 976,
                                    DimensionY = 821,
                                    DimensionWidth = 24,
                                    DimensionHeight = 44,
                                    OrderInArea = 9
                                },
                                new AddTemplatePageAreaPart
                                {
                                    DimensionX = 1006,
                                    DimensionY = 821,
                                    DimensionWidth = 24,
                                    DimensionHeight = 44,
                                    OrderInArea = 10
                                },
                                new AddTemplatePageAreaPart
                                {
                                    DimensionX = 1036,
                                    DimensionY = 821,
                                    DimensionWidth = 24,
                                    DimensionHeight = 44,
                                    OrderInArea = 11
                                },
                                new AddTemplatePageAreaPart
                                {
                                    DimensionX = 1051,
                                    DimensionY = 821,
                                    DimensionWidth = 24,
                                    DimensionHeight = 44,
                                    OrderInArea = 12
                                },
                                new AddTemplatePageAreaPart
                                {
                                    DimensionX = 1066,
                                    DimensionY = 821,
                                    DimensionWidth = 24,
                                    DimensionHeight = 44,
                                    OrderInArea = 13
                                },
                                new AddTemplatePageAreaPart
                                {
                                    DimensionX = 1128,
                                    DimensionY = 821,
                                    DimensionWidth = 23,
                                    DimensionHeight = 44,
                                    OrderInArea = 14
                                }
                            }
                        },
                        #endregion 

                        #region Wnioskodawca - PESEL
                        new AddTemplatePageArea
                        {
                            AreaName = "Wnioskodawca 1 - PESEL",
                            DimensionX = 1159,
                            DimensionY = 821,
                            DimensionWidth = 330,
                            DimensionHeight = 46,
                            ExpectedData = TemplatePartExpectedDataType.Digits,
                            TemplatePageId = pageDefinition.Id,
                            AreaParts = new[]
                            {
                                new AddTemplatePageAreaPart
                                {
                                    DimensionX = 1159,
                                    DimensionY = 821,
                                    DimensionWidth = 28,
                                    DimensionHeight = 44,
                                    OrderInArea = 1
                                },
                                new AddTemplatePageAreaPart
                                {
                                    DimensionX = 1192,
                                    DimensionY = 821,
                                    DimensionWidth = 28,
                                    DimensionHeight = 44,
                                    OrderInArea = 2
                                },
                                new AddTemplatePageAreaPart
                                {
                                    DimensionX = 1222,
                                    DimensionY = 821,
                                    DimensionWidth = 24,
                                    DimensionHeight = 44,
                                    OrderInArea = 3
                                },
                                new AddTemplatePageAreaPart
                                {
                                    DimensionX = 1252,
                                    DimensionY = 821,
                                    DimensionWidth = 24,
                                    DimensionHeight = 44,
                                    OrderInArea = 4
                                },
                                new AddTemplatePageAreaPart
                                {
                                    DimensionX = 1282,
                                    DimensionY = 821,
                                    DimensionWidth = 24,
                                    DimensionHeight = 44,
                                    OrderInArea = 5
                                },
                                new AddTemplatePageAreaPart
                                {
                                    DimensionX = 1313,
                                    DimensionY = 821,
                                    DimensionWidth = 24,
                                    DimensionHeight = 44,
                                    OrderInArea = 6
                                },
                                new AddTemplatePageAreaPart
                                {
                                    DimensionX = 1342,
                                    DimensionY = 821,
                                    DimensionWidth = 24,
                                    DimensionHeight = 44,
                                    OrderInArea = 7
                                },
                                new AddTemplatePageAreaPart
                                {
                                    DimensionX = 1372,
                                    DimensionY = 821,
                                    DimensionWidth = 24,
                                    DimensionHeight = 44,
                                    OrderInArea = 8
                                },
                                new AddTemplatePageAreaPart
                                {
                                    DimensionX = 1402,
                                    DimensionY = 821,
                                    DimensionWidth = 24,
                                    DimensionHeight = 44,
                                    OrderInArea = 9
                                },
                                new AddTemplatePageAreaPart
                                {
                                    DimensionX = 1433,
                                    DimensionY = 821,
                                    DimensionWidth = 24,
                                    DimensionHeight = 44,
                                    OrderInArea = 10
                                },
                                new AddTemplatePageAreaPart
                                {
                                    DimensionX = 1464,
                                    DimensionY = 821,
                                    DimensionWidth = 24,
                                    DimensionHeight = 44,
                                    OrderInArea = 11
                                }
                            }
                        }
                        #endregion 
                    }
                });

            return new GetTemplateDefinitionByIdHandler(TemplatingConfig)
                .Handle(new GetTemplateDefinitionById
                {
                    TemplateId = templateDefinition.Id
                }).Result;
        }
    }
}
