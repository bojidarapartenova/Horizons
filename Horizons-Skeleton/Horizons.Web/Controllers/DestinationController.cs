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
            try
            {
                AddDestinationInputModel inputModel = new AddDestinationInputModel()
                {
                    PublishedOn=DateTime.UtcNow.ToString(DateFormat),
                    Terrains = await terrainService.GetTerrainDropDownAsync()
                };

                return View(inputModel);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddDestinationInputModel inputModel)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return RedirectToAction(nameof(Add));
                }

                bool result= await destinationService.AddDestinationAsync(GetUserId()!, inputModel);

                if(result==false)
                {
                    return RedirectToAction(nameof(Add));
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                string userId = GetUserId()!;
                EditDestinationInputModel? inputModel = await
                    destinationService.GetDestinationForEditingAsync(userId, id);

                if(inputModel == null)
                {
                    return RedirectToAction(nameof(Index));
                }
                inputModel.Terrains = await terrainService.GetTerrainDropDownAsync();

                return View(inputModel);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditDestinationInputModel inputModel)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return View(inputModel);
                }
                bool result=await destinationService.PersistEditDestinationAsync(inputModel);

                if(result==false)
                {
                    return View(inputModel);
                }

                return RedirectToAction(nameof(Details));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                string userId = GetUserId()!;

                DeleteDestinationViewModel? deleteModel = await destinationService.GetDestinationForDeletingAsync(userId, id);

                if(deleteModel==null)
                {
                    return RedirectToAction(nameof(Index));
                }

                return View(deleteModel);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(DeleteDestinationViewModel deleteModel)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return View(deleteModel);
                }

                bool result = await destinationService.SoftDeleteDestinationAsync(GetUserId()!, deleteModel);

                if(result==false)
                {
                    return View(deleteModel);
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> Favorites()
        {
            try
            {
                string userId = GetUserId()!;

                IEnumerable<FavoriteDestinationViewModel>? destinations=await 
                    destinationService.GetFavoriteDestinationsAsync(userId);

                if(destinations==null)
                {
                    return RedirectToAction(nameof(Index));
                }

                return View(destinations);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddToFavorites(int? id)
        {
            try
            {
                string userId=GetUserId()!;

                if(id==null)
                {
                    return RedirectToAction(nameof(Index));
                }
                bool result = await destinationService.AddToFavoritesAsync(userId, id.Value);

                if(result==false)
                {
                    return RedirectToAction(nameof(Index));
                }

                return RedirectToAction(nameof(Favorites));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
