using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Horizons.Data.Models
{
    public class UserDestinationConfiguration : IEntityTypeConfiguration<UserDestination>
    {
        public void Configure(EntityTypeBuilder<UserDestination> entity)
        {
            entity.HasKey(e => new { e.UserId, e.DestinationId });

            entity.HasQueryFilter(e => e.Destination.IsDeleted == false);

            entity
                .HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId);

            entity.HasOne(e => e.Destination)
                .WithMany(e=>e.UsersDestinations)
                .HasForeignKey(e => e.DestinationId);
        }
    }
}
