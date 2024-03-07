using Microsoft.AspNetCore.Mvc;

namespace Gdn.Web.Api.Controllers;


public class CrudController : ControllerBase
{
	protected string? GetEntityLocation<TId>(TId entityId) where TId : struct
	{
		string? location = default;
		string? controllerName = ControllerContext?.RouteData?.Values["controller"]?.ToString();
		if (!string.IsNullOrEmpty(controllerName))
			location = Url.Action("Get", controllerName, new { id = entityId }, Request.Scheme);

		return location;
	}
}
