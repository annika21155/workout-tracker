using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace WorkoutTracker.Api.Helpers;

public static class ControllerExtensions
{
    public static int GetUserId(this ControllerBase controller)
    {
        var idClaim = controller.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? controller.User.FindFirst("sub")?.Value;
        return int.Parse(idClaim!);
    }
}