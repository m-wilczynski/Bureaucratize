using System;
using Bureaucratize.FileStorage.Contracts;
using Bureaucratize.FileStorage.Contracts.Queries;
using Bureaucratize.FileStorage.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Bureaucratize.FileStorage.Service.Controllers
{
    [Route("api/image-recognition-trained-files")]
    public class ImageRecognitionTrainedFilesController : Controller
    {
        private readonly IFileStorageQueryHandler<GetImageRecognitionModel, byte[]> _getImageRecognitionModelHandler;
        private readonly IFileStorageQueryHandler<GetImageRecognitionLabelMap, string> _getImageRecognitionLabelMapHandler;

        public ImageRecognitionTrainedFilesController(
            IFileStorageQueryHandler<GetImageRecognitionModel, byte[]> getImageRecognitionModelHandler, 
            IFileStorageQueryHandler<GetImageRecognitionLabelMap, string> getImageRecognitionLabelMapHandler)
        {
            if (getImageRecognitionModelHandler == null)
                throw new ArgumentNullException(nameof(getImageRecognitionModelHandler));
            if (getImageRecognitionLabelMapHandler == null)
                throw new ArgumentNullException(nameof(getImageRecognitionLabelMapHandler));

            _getImageRecognitionModelHandler = getImageRecognitionModelHandler;
            _getImageRecognitionLabelMapHandler = getImageRecognitionLabelMapHandler;
        }

        [HttpGet("image-recognition-model/{expectedData}")]
        public async void GetImageRecognitionModel([FromRoute]ImageRecognitionExpectedData expectedData)
        {
            var bytes = _getImageRecognitionModelHandler.Handle(
                new GetImageRecognitionModel {ExpectedData = expectedData});
            await Response.Body.WriteAsync(bytes, 0, bytes.Length);
        }

        [HttpGet("image-recognition-labelmap/{expectedData}")]
        public IActionResult GetImageRecognitionLabelMap([FromRoute]ImageRecognitionExpectedData expectedData)
        {
            return new ObjectResult(_getImageRecognitionLabelMapHandler.Handle(
                new GetImageRecognitionLabelMap { ExpectedData = expectedData }));
        }
    }
}
