using Garage.Contracts.Permissions;
using Garage.Domain.Users.Permissions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Garage.Api.Controllers;

using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public sealed class PermissionsController : ControllerBase
{
    [HttpGet("grouped")]
    public ActionResult<Dictionary<string, List<string>>> GetGrouped()
    {
        var result = Permission.All
            .Select(p =>
            {
                var parts = p.Split('.', StringSplitOptions.RemoveEmptyEntries);
                var module = parts.ElementAtOrDefault(0) ?? "";
                var action = parts.ElementAtOrDefault(1) ?? "";
                return new { module, action };
            })
            .Where(x => !string.IsNullOrWhiteSpace(x.module) && !string.IsNullOrWhiteSpace(x.action))
            .GroupBy(x => x.module, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(
                g => ToPascal(g.Key),
                g => g.Select(x => ToPascal(x.action))
                      .Distinct(StringComparer.OrdinalIgnoreCase)
                      .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
                      .ToList(),
                StringComparer.OrdinalIgnoreCase
            );

        return Ok(result);
    }

    private static string ToPascal(string value)
        => string.IsNullOrWhiteSpace(value)
            ? value
            : char.ToUpperInvariant(value[0]) + value[1..];
}

