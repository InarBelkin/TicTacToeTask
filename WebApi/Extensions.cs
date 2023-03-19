using Microsoft.AspNetCore.Mvc;

namespace WebApi;

public static class Extensions
{
    public static IActionResult MapToResult(this IEnumerable<Exception> exceptions)
    {
        return new BadRequestResult();
    }
}