using BuildingBlocks.Application.Results;
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
                extensions: DefaultExtensions(error)
            ),
            ErrorType.Validation => controller.Problem(
                title: "Bad Request",
                detail: error.Description,
                statusCode: StatusCodes.Status400BadRequest,
                extensions: DefaultExtensions(error)
            ),
            ErrorType.Conflict => controller.Problem(
                title: "Conflict",
                detail: error.Description,
                statusCode: StatusCodes.Status409Conflict,
                extensions: DefaultExtensions(error)
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
    }
}
