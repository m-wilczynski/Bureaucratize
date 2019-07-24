using Bureaucratize.ImageProcessing.Core.Document;
using Bureaucratize.Templating.Core.Template;

namespace Bureaucratize.Web.ViewModels
{
    public class DocumentProcessingViewModel
    {
        public DocumentToProcess Document { get; set; }
        public TemplateDefinition DocumentTemplate { get; set; }
    }
}
