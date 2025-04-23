namespace Gdn.Web.Api.Vs.Endpoints;

public static class ResultHelper
{
    public static IResult NoContent() => TypedResults.NoContent();
    public static IResult Ok<T>(T data) where T : class => TypedResults.Ok(data);
    public static IResult Created<T>(T data) where T : class => TypedResults.Created(string.Empty, data);
    public static IResult BadRequest<T>(T data) where T : class => TypedResults.BadRequest(data);
    public static IResult NotFound() => TypedResults.NotFound();
    public static IResult NotFound(Error error) => TypedResults.NotFound(error);
}
