using System.ComponentModel.DataAnnotations;
using System.IO;
using Bureaucratize.FileStorage.Contracts;

namespace Bureaucratize.Web.ViewModels.Validation
{
    public class JpgOnlyBitmapAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var model = validationContext.ObjectInstance as UploadInputFromUserViewModel;
            if (model == null)
                return new ValidationResult("Przekazano pusta wiadomosc");

            var file = model.UserUpload;
            if (file == null)
                return new ValidationResult("Nie wyslano zadnego pliku!");

            if (Path.GetExtension(file.FileName).AsBitmapFiletype() != BitmapFiletype.Jpg)
                return new ValidationResult("Plik nie ma rozszerzenia .jpg");

            return ValidationResult.Success;

        }
    }
}
