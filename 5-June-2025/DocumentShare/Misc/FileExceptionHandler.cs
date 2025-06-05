using DocumentShare.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SecondWebApi.Models.Dtos;

namespace DocumentShare.Misc
{
    public class FileExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<FileExceptionFilter> _logger;

        public FileExceptionFilter(ILogger<FileExceptionFilter> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            var ex = context.Exception;

            if (ex is FileNotFoundException ||
                ex is FileConversionException ||
                ex is FileUploadException ||
                ex is InvalidFileFormatException)
            {
                _logger.LogError(ex, "File-specific error occurred: {Message}", ex.Message);

                context.Result = new ObjectResult(new ErrorObjectDto
                {
                    ErrorNumber = 500,
                    ErrorMessage = $"File error: {ex.Message}"
                })
                {
                    StatusCode = 500
                };

                context.ExceptionHandled = true;
            }
        }
    }
}
