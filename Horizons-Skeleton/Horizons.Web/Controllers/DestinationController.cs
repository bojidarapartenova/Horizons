using Horizons.Services.Core.Contracts;
using Horizons.Web.ViewModels.Destination;
using Microsoft.AspNetCore.Mvc;

namespace Horizons.Web.Controllers
{
    public class DestinationController : BaseController
    {
        private readonly IDestinationService destinationService;

        public DestinationController(IDestinationService destinationService)
        {
            this.destinationService = destinationService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            string? userId=GetUserId();

            IEnumerable<DestinationIndexViewModel> allDestinations=await
                destinationService.GetAllDestinationsAsync(userId);

            return View(allDestinations);
        }
    }
}
