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

namespace Horizons.Services.Core
{
    public class TerrainService : ITerrainService
    {
        private readonly HorizonDbContext dbContext;

        public TerrainService(HorizonDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IEnumerable<AddDestinationTerrainDropDownModel>> GetTerrainDropDownAsync()
        {
            IEnumerable<AddDestinationTerrainDropDownModel> allTerrains =
                await dbContext
                .Terrains
                .AsNoTracking()
                .Select(t => new AddDestinationTerrainDropDownModel()
                {
                    Id = t.Id,
                    Name = t.Name,
                })
                .ToArrayAsync();

            return allTerrains;
        }
    }
}
