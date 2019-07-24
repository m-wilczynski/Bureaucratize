using System.ComponentModel.DataAnnotations;
using Bureaucratize.Web.ViewModels.Validation;
using Microsoft.AspNetCore.Http;

namespace Bureaucratize.Web.ViewModels
{
    public class UploadInputFromUserViewModel
    {
        [Required]
        [JpgOnlyBitmap]
        public IFormFile UserUpload { get; set; }
    }
}
