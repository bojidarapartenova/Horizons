using Horizons.Services.Core.Contracts;
using Horizons.Web.ViewModels.Destination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Horizons.GCommon.ValidationConstants.Destination;

namespace Horizons.Web.Controllers
{
    public class DestinationController : BaseController
    {
        private readonly IDestinationService destinationService;
        private readonly ITerrainService terrainService;

        public DestinationController(IDestinationService destinationService,
            ITerrainService terrainService)
        {
            this.destinationService = destinationService;
            this.terrainService = terrainService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            try
            {
                string? userId = GetUserId();

                IEnumerable<DestinationIndexViewModel> allDestinations = await
                    destinationService.GetAllDestinationsAsync(userId);

                return View(allDestinations);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return RedirectToAction(nameof(Index), "Home");
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            try
            {
                string? userId = GetUserId();

                DestinationDetailsViewModel? destinationDetails = await destinationService
                    .GetDestinationDetailsAsync(id, userId);

                if (destinationDetails == null)
                {
                    return RedirectToAction(nameof(Index));
                }

                return View(destinationDetails);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return RedirectToAction(nameof(Index), "Home");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            AddDestinationInputModel inputModel = new AddDestinationInputModel()
            {
                PublishedOn = DateTime.UtcNow.ToString(DateFormat),
                Terrains = await terrainService.GetTerrainDropDownAsync()
            };

            return View(inputModel);
        }
    }
}
