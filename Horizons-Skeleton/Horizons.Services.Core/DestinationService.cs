using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using Horizons.Data;
using Horizons.Data.Models;
using Horizons.Services.Core.Contracts;
using Horizons.Web.ViewModels.Destination;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static Horizons.GCommon.ValidationConstants.Destination;

namespace Horizons.Services.Core
{
    public class DestinationService : IDestinationService
    {
        private readonly HorizonDbContext dbContext;
        private readonly UserManager<IdentityUser> userManager;

        public DestinationService(HorizonDbContext dbContext, UserManager<IdentityUser> userManager)
        {
            this.dbContext = dbContext;
            this.userManager = userManager;
        }
        public async Task<IEnumerable<DestinationIndexViewModel>> GetAllDestinationsAsync(string? userId)
        {
            bool isGuidValid=!String.IsNullOrEmpty(userId);
            IEnumerable<DestinationIndexViewModel> allDestinations = await
                dbContext
                .Destinations
                .AsNoTracking()
                .Select(d => new DestinationIndexViewModel
                {
                    Id = d.Id,
                    Name = d.Name,
                    ImageUrl = d.ImageUrl,
                    Terrain = d.Terrain.Name,
                    FavoritesCount = d.UsersDestinations.Count,
                    IsPublisher = isGuidValid ? d.PublisherId.ToLower() == userId!.ToLower() : false,
                    IsFavorite = isGuidValid ? d.UsersDestinations.Any(d => d.UserId.ToLower() == userId!.ToLower()) : false,
                })
                .ToArrayAsync();

            return allDestinations;
        }

        public async Task<DestinationDetailsViewModel?> GetDestinationDetailsAsync(int? id, string? userId)
        {
            DestinationDetailsViewModel? detailsVm = null;

            if(id.HasValue)
            {
                Destination? destination= await dbContext
                    .Destinations
                    .Include(d=>d.Publisher)
                    .Include(d=>d.Terrain)
                    .Include(d=>d.UsersDestinations)
                    .AsNoTracking()
                    .SingleOrDefaultAsync(d=>d.Id== id.Value);

                if(destination!=null)
                {
                    detailsVm = new DestinationDetailsViewModel
                    {
                        Id = destination.Id,
                        Name = destination.Name,
                        ImageUrl = destination.ImageUrl,
                        Description = destination.Description,
                        Terrain = destination.Terrain.Name,
                        PublishedOn = destination.PublishedOn.ToString(DateFormat),
                        Publisher = destination.Publisher.UserName,
                        IsPublisher = userId != null ?
                        destination.Publisher.Id.ToLower() == userId!.ToLower() : false,
                        IsFavorite = userId != null ?
                        destination.UsersDestinations.Any(d => d.UserId.ToLower() == userId!.ToLower()) : false
                    };
                }
            }

            return detailsVm;
        }

        public async Task<bool> AddDestinationAsync(string userId, AddDestinationInputModel inputModel)
        {
            bool result = false;

            IdentityUser? user = await userManager.FindByIdAsync(userId);

            Terrain? terrain = await dbContext.Terrains.FindAsync(inputModel.TerrainId);
            bool isPublishedOnValid = DateTime
                .TryParseExact(inputModel.PublishedOn, DateFormat,
                CultureInfo.InvariantCulture, DateTimeStyles.None,
                out DateTime publishedOn);

            if (user != null && terrain != null && isPublishedOnValid)
            {
                Destination destination = new Destination()
                {
                    Name = inputModel.Name,
                    Description = inputModel.Description,
                    ImageUrl = inputModel.ImageUrl,
                    PublishedOn = publishedOn,
                    PublisherId = userId,
                    TerrainId = inputModel.TerrainId
                };

                await dbContext.Destinations.AddAsync(destination);
                await dbContext.SaveChangesAsync();

                result = true;
            }
            return result;
        }

        public async Task<EditDestinationInputModel?> GetDestinationForEditingAsync(string userId, int? dId)
        {
            EditDestinationInputModel? editModel = null;

            if(dId!=null)
            {
                Destination? destination = await dbContext
                    .Destinations
                    .AsNoTracking()
                    .SingleOrDefaultAsync(d => d.Id == dId);

                if(destination!=null && destination.PublisherId.ToLower()==userId.ToLower())
                {
                    editModel = new EditDestinationInputModel()
                    {
                        Id = destination.Id,
                        Name = destination.Name,
                        Description = destination.Description,
                        ImageUrl = destination.ImageUrl,
                        PublishedOn = destination.PublishedOn.ToString(DateFormat),
                        PublisherId = destination.PublisherId,
                        TerrainId=destination.TerrainId
                    };
                }
            }
            return editModel;
        }

        public async Task<bool> PersistEditDestinationAsync(EditDestinationInputModel inputModel)
        {
            bool result = false;
            Terrain? terrain = await dbContext.Terrains.FindAsync(inputModel.TerrainId);
            bool isPublishedOnValid = DateTime
                .TryParseExact(inputModel.PublishedOn, DateFormat,
                CultureInfo.InvariantCulture, DateTimeStyles.None,
                out DateTime publishedOn);
            Destination? updatedDestination = await dbContext.Destinations
                .FindAsync(inputModel.Id);

            if(updatedDestination!=null && terrain!=null && isPublishedOnValid)
            {
                updatedDestination.Name = inputModel.Name;
                updatedDestination.Description = inputModel.Description;
                updatedDestination.ImageUrl = inputModel.ImageUrl;
                updatedDestination.PublishedOn = publishedOn;
                updatedDestination.TerrainId = inputModel.TerrainId;

                await dbContext.SaveChangesAsync();
                result = true;
            }

            return result;
        }

        public async Task<DeleteDestinationViewModel?> GetDestinationForDeletingAsync(string userId, int? dId)
        {
            DeleteDestinationViewModel? deleteModel = null;

            if(dId!=null)
            {
                Destination? destination = await dbContext
                    .Destinations
                    .Include(d=>d.Publisher)
                    .AsNoTracking()
                    .SingleOrDefaultAsync(d => d.Id == dId);

                if(destination!=null &&
                    destination.PublisherId.ToLower()==userId.ToLower())
                {
                    deleteModel = new DeleteDestinationViewModel()
                    {
                        Id = destination.Id,
                        Name = destination.Name,
                        Publisher=destination.Publisher.NormalizedUserName,
                        PublisherId = destination.PublisherId
                    };
                }
            }
            return deleteModel;
        }

        public async Task<bool> SoftDeleteDestinationAsync(string userId, DeleteDestinationViewModel deleteModel)
        {
            bool result = false;

            IdentityUser? user=await userManager.FindByIdAsync(userId);
            Destination? destination = await dbContext.Destinations.FindAsync(deleteModel.Id);

            if(user!=null && destination!=null && 
                destination.PublisherId.ToLower()==userId.ToLower())
            {
                destination.IsDeleted = true;

                await dbContext.SaveChangesAsync();
                result = true;
            }

            return result;
        }

        public async Task<IEnumerable<FavoriteDestinationViewModel>?> GetFavoriteDestinationsAsync(string userId)
        {
            IEnumerable<FavoriteDestinationViewModel>? favDestinations = null;

            IdentityUser? user = await userManager.FindByIdAsync(userId);

            if (user != null)
            {
                favDestinations = await dbContext
                    .UsersDestinations
                    .Include(ud=>ud.Destination)
                    .ThenInclude(d=>d.Terrain)
                    .Where(ud => ud.UserId.ToLower() == userId.ToLower())
                    .Select(ud => new FavoriteDestinationViewModel()
                    {
                        Id = ud.DestinationId,
                        Name = ud.Destination.Name,
                        ImageUrl = ud.Destination.ImageUrl,
                        Terrain = ud.Destination.Terrain.Name
                    })
                    .ToArrayAsync();
            }

            return favDestinations;
        }

        public async Task<bool> AddToFavoritesAsync(string userId, int dId)
        {
            bool result = false;

            IdentityUser? user=await userManager.FindByIdAsync(userId);
            Destination? destinations = await dbContext
                .Destinations
                .FindAsync(dId);

            if(user!=null && destinations!=null &&
                destinations.PublisherId.ToLower()!=userId.ToLower())
            {
                UserDestination? userDestination = await dbContext
                    .UsersDestinations
                    .SingleOrDefaultAsync(ud => ud.UserId.ToLower() == userId.ToLower() &&
                    ud.DestinationId == dId);

                if(userDestination==null)
                {
                    userDestination = new UserDestination()
                    {
                        UserId = userId,
                        DestinationId = dId
                    };

                    await dbContext.UsersDestinations.AddAsync(userDestination);
                    await dbContext.SaveChangesAsync();

                    result = true;
                }
            }

            return result;
        }

        public async Task<bool> RemoveFromFavoritesAsync(string userId, int dId)
        {
            bool result = false;

            IdentityUser? user= await userManager.FindByIdAsync(userId);
            Destination? destination= await dbContext
                .Destinations
                .FindAsync(dId);

            if(user!=null && destination!=null)
            {
                UserDestination? userDestination = await dbContext
                    .UsersDestinations
                    .SingleOrDefaultAsync(ud => ud.UserId.ToLower() == userId.ToLower()
                    && ud.DestinationId == dId);

                if(userDestination!=null)
                {
                    dbContext.UsersDestinations.Remove(userDestination);
                    await dbContext.SaveChangesAsync();

                    result = true;
                }
            }
            return result;
        }
    }
}
