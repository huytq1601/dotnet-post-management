using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PostManagement.WebApi.Filters;

public class ValidateModelAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            var errors = new Dictionary<string, string[]>();
            foreach (var (key, value) in context.ModelState)
            {
                if (value.Errors.Any())
                {
                    errors[key] = value.Errors.Select(e => e.ErrorMessage).ToArray();
                }
            }

            var response = new Response<object>
            {
                Success = false,
                Message = "Validation failed",
                Errors = errors
            };

            context.Result = new BadRequestObjectResult(response);
        }
    }
}
