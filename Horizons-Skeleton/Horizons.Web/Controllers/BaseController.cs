using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace Horizons.Web.Controllers
{
    public abstract class BaseController : Controller
    {
        protected bool IsUserAuthenticated()
        {
            return User.Identity?.IsAuthenticated ?? false;
        }

        protected string? GetUserId()
        {
            string? userId = null;
            bool isAuthenticated = IsUserAuthenticated();

            if (isAuthenticated)
            {
                userId=User.FindFirstValue(ClaimTypes.NameIdentifier);
            }

            return userId;
        }
    }
}
