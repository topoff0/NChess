using Account.Application.Common.Errors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Account.API.Filters;

public sealed class ValidateModelAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            var error = Error.Validation(
                new Dictionary<string, string[]>
                {
                    {
                        ErrorCodes.ModelStateValidationFailed,
                        new[] {ErrorMessages.ModelStateValidationFailed}
                    }
                });


            context.Result = new BadRequestObjectResult(error);
        }
    }
}
