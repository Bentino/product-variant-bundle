using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ProductVariantBundle.Api.DTOs.Common;

namespace ProductVariantBundle.Api.Filters;

public class ValidationFilter : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        Console.WriteLine("=== ValidationFilter.OnActionExecuting ===");
        Console.WriteLine($"ModelState.IsValid: {context.ModelState.IsValid}");
        
        if (!context.ModelState.IsValid)
        {
            Console.WriteLine("ModelState errors:");
            foreach (var kvp in context.ModelState)
            {
                Console.WriteLine($"  {kvp.Key}: {string.Join(", ", kvp.Value?.Errors.Select(e => e.ErrorMessage) ?? new string[0])}");
            }
            
            var errors = context.ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray() ?? Array.Empty<string>()
                );

            var problemDetails = ProblemDetailsResponse.Create(
                status: 400,
                title: "Validation Error",
                detail: "One or more validation errors occurred.",
                instance: context.HttpContext.Request.Path,
                errors: errors);

            context.Result = new BadRequestObjectResult(problemDetails);
        }
    }
}