using System;

namespace Bureaucratize.Web.ExampleMapping.DomainModels
{
    public class CreditInquiry
    {
        private Guid documentId;

        public CreditInquiry(Guid documentId)
        {
            this.documentId = documentId;
        }

        public CreditType? Type { get; set; }
        public Customer Customer { get; set; }
    }

    public enum CreditType
    {
        Hipoteczny,
        BudowlanyHipoteczny,
        MieszkaniowyMDM,
        EurokontoHipotecznePlus
    }
}
