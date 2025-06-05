

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SecondWebApi.Models.Dtos;

namespace SecondWebApi.Misc
{
    public class CustomExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            context.Result = new BadRequestObjectResult(new ErrorObjectDto
            {
                ErrorNumber = 500,
                ErrorMessage = context.Exception.Message
            });
        }
    }
}