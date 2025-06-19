using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Horizons.Data;
using Horizons.Data.Models;
using Horizons.Services.Core.Contracts;
using Horizons.Web.ViewModels.Destination;
using Microsoft.EntityFrameworkCore;
using static Horizons.GCommon.ValidationConstants.Destination;

namespace Horizons.Services.Core
{
    public class DestinationService : IDestinationService
    {
        private readonly HorizonDbContext dbContext;

        public DestinationService(HorizonDbContext dbContext)
        {
            this.dbContext = dbContext;
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
    }
}
