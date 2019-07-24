using System;
using System.Collections.Generic;
using System.Linq;
using Bureaucratize.ImageProcessing.Contracts.ProcessingMessages.ProcessingOutcomes;
using Bureaucratize.ImageProcessing.Contracts.Recognition;
using Bureaucratize.Web.ExampleMapping.DomainModels;

namespace Bureaucratize.Web.ExampleMapping
{
    public class DocumentPageMapper
    {
        public MappingResult<CreditInquiry> MapToDomainModel(DocumentPageProcessingCompleted processedPage)
        {
            var result = new MappingResult<CreditInquiry>();
            result.Scope = "Page";

            var inquiry = new CreditInquiry(processedPage.DocumentId);
            inquiry.Customer = new Customer();
            bool anyError = false;

            var inquiryType = processedPage.ProcessedChoiceAreas
                .Where(pca => pca.AreaName.StartsWith("Typ wniosku - ") && pca.RecognitionOutput == true)
                .ToList();

            anyError &= AssignInquiryType(result, inquiry, inquiryType);
            anyError &= AssignCustomerParts(result, inquiry, processedPage);

            result.WasSuccessful = !anyError;
            result.Result = inquiry;

            return result;
        }

        private static bool AssignCustomerParts(MappingResult<CreditInquiry> result, CreditInquiry inquiry,
            DocumentPageProcessingCompleted processedPage)
        {
            var anyError = false;

            inquiry.Customer.Name = processedPage.ProcessedTextAreas.Single(pta => pta.AreaName == "Wnioskodawca 1 - Imie").ResultStringified;
            if (inquiry.Customer.Name == null)
            {
                anyError = true;
                result.Errors.Add("Nie wprowadzono imienia wnioskodawcy");
            }

            inquiry.Customer.Surname = processedPage.ProcessedTextAreas.Single(pta => pta.AreaName == "Wnioskodawca 1 - Nazwisko").ResultStringified;
            if (inquiry.Customer.Surname == null)
            {
                anyError = true;
                result.Errors.Add("Nie wprowadzono nazwiska wnioskodawcy");
            }

            inquiry.Customer.PESEL = processedPage.ProcessedTextAreas
                .Single(pta => pta.AreaName == "Wnioskodawca 1 - PESEL").ResultStringified;

            if (inquiry.Customer.PESEL == null)
            {
                anyError = true;
                result.Errors.Add("Nie wprowadzono nr PESEL wnioskodawcy");
            }
            else if (inquiry.Customer.PESEL.Length < 11)
            {
                anyError = true;
                result.Errors.Add("Nie wprowadzono wszystkich cyfr nr PESEL");
            }
            else
            {
                var peselNumbers = inquiry.Customer.PESEL.Select(num => int.Parse(num.ToString())).ToList();

                int controlSum = 0;

                controlSum += 9 * peselNumbers[0];
                controlSum += 7 * peselNumbers[1];
                controlSum += 3 * peselNumbers[2];
                controlSum += 1 * peselNumbers[3];
                controlSum += 9 * peselNumbers[4];
                controlSum += 7 * peselNumbers[5];
                controlSum += 3 * peselNumbers[6];
                controlSum += 1 * peselNumbers[7];
                controlSum += 9 * peselNumbers[8];
                controlSum += 7 * peselNumbers[9];

                if (controlSum%10 != peselNumbers[10])
                {
                    anyError = true;
                    result.Errors.Add("Nr PESEL jest nieprawidlowy");
                }
            }

            return anyError;
        }

        private static bool AssignInquiryType(MappingResult<CreditInquiry> result, CreditInquiry inquiry, 
            List<RecognizedChoicePart> inquiryType)
        {
            switch (inquiryType.Count)
            {
                case 0:
                    result.Errors.Add("Nie wybrano typu wniosku");
                    return true;
                case 1:
                    switch (inquiryType.Single().AreaName)
                    {
                        case "Typ wniosku - Kredyt hipoteczny":
                            inquiry.Type = CreditType.BudowlanyHipoteczny;
                            break;
                        case "Typ wniosku - Kredyt budowlano-hipoteczny":
                            inquiry.Type = CreditType.BudowlanyHipoteczny;
                            break;
                        case "Typ wniosku - Kredyt mieszkaniowy MDM":
                            inquiry.Type = CreditType.MieszkaniowyMDM;
                            break;
                        default:
                            inquiry.Type = CreditType.EurokontoHipotecznePlus;
                            break;
                    }
                    return false;
                default:
                    result.Errors.Add("Wybrano wiecej niz jeden typ wniosku");
                    return true;
            }
        }
    }
}
