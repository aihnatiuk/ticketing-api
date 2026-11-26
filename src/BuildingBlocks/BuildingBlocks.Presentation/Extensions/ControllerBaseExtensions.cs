using BuildingBlocks.Domain.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BuildingBlocks.Presentation.Extensions;

public static class ControllerBaseExtensions
{
    public static ActionResult ToProblem(this ControllerBase controller, Error error)
    {
        return error.Type switch
        {
            ErrorType.NotFound => controller.Problem(
                title: "Not Found",
                detail: error.Description,
                statusCode: StatusCodes.Status404NotFound,
                extensions: DetailedExtensions(error)
            ),
            ErrorType.Validation => controller.Problem(
                title: "Bad Request",
                detail: error.Description,
                statusCode: StatusCodes.Status400BadRequest,
                extensions: DetailedExtensions(error)
            ),
            ErrorType.Conflict => controller.Problem(
                title: "Conflict",
                detail: error.Description,
                statusCode: StatusCodes.Status409Conflict,
                extensions: DetailedExtensions(error)
            ),
            _ => controller.Problem(
                title: "Internal Server Error",
                detail: error.Description,
                statusCode: StatusCodes.Status500InternalServerError,
                extensions: DefaultExtensions(error)
            ),
        };

        static Dictionary<string, object?> DefaultExtensions(Error error) =>
            new() { ["errorCode"] = error.Code };

        static Dictionary<string, object?> DetailedExtensions(Error error)
        {
            Dictionary<string, object?> extensions = DefaultExtensions(error);
            if (error.Details is not null)
            {
                extensions.Add("errorDetails", error.Details);
            }

            return extensions;
        }
    }
}
