using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;

namespace Horizons.Data
{ 
    public class HorizonDbContextFactory : IDesignTimeDbContextFactory<HorizonDbContext>
    {
        public HorizonDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<HorizonDbContext>();

            // Replace with your actual connection string
            optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=HorizonsDb;Trusted_Connection=True;");

            return new HorizonDbContext(optionsBuilder.Options);
        }
    }
}
