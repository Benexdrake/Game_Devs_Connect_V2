namespace GameDevsConnect.Api.Shared.Endpoints;

public static class ResultExtensions
{
    public static IResult ToHttpResult<T>(this Result<T> result) => result.Status switch
    {
        ResultStatus.Success => Results.Ok(result.Value),
        ResultStatus.NotFound => Problem(result, StatusCodes.Status404NotFound),
        ResultStatus.Conflict => Problem(result, StatusCodes.Status409Conflict),
        ResultStatus.Forbidden => Problem(result, StatusCodes.Status403Forbidden),
        ResultStatus.Unauthorized => Problem(result, StatusCodes.Status401Unauthorized),
        ResultStatus.ValidationError => Problem(result, StatusCodes.Status400BadRequest),
        _ => Results.StatusCode(StatusCodes.Status500InternalServerError),
    };

    public static IResult ToCreatedHttpResult<T>(this Result<T> result, Func<T, string> locationFactory) =>
        result.Status == ResultStatus.Success
            ? Results.Created(locationFactory(result.Value!), result.Value)
            : result.ToHttpResult();

    private static IResult Problem<T>(Result<T> result, int statusCode) =>
        Results.Problem(detail: result.Error, statusCode: statusCode);
}
