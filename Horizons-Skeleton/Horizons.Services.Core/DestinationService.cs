using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Horizons.Data;
using Horizons.Services.Core.Contracts;
using Horizons.Web.ViewModels.Destination;
using Microsoft.EntityFrameworkCore;

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
    }
}
