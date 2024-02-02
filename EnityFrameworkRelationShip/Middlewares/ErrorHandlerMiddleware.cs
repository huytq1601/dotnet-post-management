
using PostManagement.Core.Exceptions;

namespace PostManagement.WebApi.Middlewares
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (NotFoundException ex)
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                await context.Response.WriteAsJsonAsync(new Response<object> { Success = false, Message = ex.Message });
            }
            catch (BadRequestException ex)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsJsonAsync(new Response<object> { Success = false, Message = ex.Message });
            }
            catch(ForbiddenException)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsJsonAsync(new Response<object> { Success = false, Message = "You don't have permission to do this action." });
            }
            catch (UnauthorizedAccessException)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new Response<object>() { Success = false, Message = "Unauthorized access." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unhandled Exception: {ex}");
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsJsonAsync(new Response<object> { Success = false, Message = "An error occurred while processing your request." });
            }
        }
    }
}
 