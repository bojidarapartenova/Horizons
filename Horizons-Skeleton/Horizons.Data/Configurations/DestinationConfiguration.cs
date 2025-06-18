using Horizons.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static Horizons.GCommon.ValidationConstants.Destination;

namespace Horizons.Data.Configurations
{
    public class DestinationConfiguration : IEntityTypeConfiguration<Destination>
    {
        public void Configure(EntityTypeBuilder<Destination> entity)
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(NameMaxLength);

            entity.Property(e => e.Description)
                .IsRequired()
                .HasMaxLength(DescriptionMaxLength);

            entity.Property(e => e.ImageUrl)
                .IsRequired(false);

            entity.Property(e => e.PublisherId)
                .IsRequired();

            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false);

            entity.HasQueryFilter(e => e.IsDeleted == false);

            entity
                .HasOne(e => e.Publisher)
                .WithMany()
                .HasForeignKey(e => e.PublisherId)
                .OnDelete(DeleteBehavior.Restrict);

            entity
                .HasOne(e => e.Terrain)
                .WithMany(e => e.Destinations)
                .HasForeignKey(e => e.TerrainId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasData(GenerateSeedDestinations());
        }

        private List<Destination> GenerateSeedDestinations()
        {
            List<Destination> seedDestinations = new List<Destination>()
            {
                new Destination
                {
                    Id = 1,
                    Name = "Rila Monastery",
                    Description = "A stunning historical landmark nestled in the Rila Mountains.",
                    ImageUrl = "https://img.etimg.com/thumb/msid-112831459,width-640,height-480,imgsize-2180890,resizemode-4/rila-monastery-bulgaria.jpg",
                    PublisherId = "7699db7d-964f-4782-8209-d76562e0fece",
                    PublishedOn = DateTime.Now,
                    TerrainId = 1,
                    IsDeleted = false
                },
                new Destination
                {
                    Id = 2,
                    Name = "Durankulak Beach",
                    Description = "The sand at Durankulak Beach showcases a pristine golden color, creating a beautiful contrast against the azure waters of the Black Sea.",
                    ImageUrl = "https://travelplanner.ro/blog/wp-content/uploads/2023/01/durankulak-beach-1-850x550.jpg.webp",
                    PublisherId = "7699db7d-964f-4782-8209-d76562e0fece",
                    PublishedOn = DateTime.Now,
                    TerrainId = 2,
                    IsDeleted = false
                },
                new Destination
                {
                    Id = 3,
                    Name = "Devil's Throat Cave",
                    Description = "A mysterious cave located in the Rhodope Mountains.",
                    ImageUrl = "https://detskotobnr.binar.bg/wp-content/uploads/2017/11/Diavolsko_garlo_17.jpg",
                    PublisherId = "7699db7d-964f-4782-8209-d76562e0fece",
                    PublishedOn = DateTime.Now,
                    TerrainId = 7,
                    IsDeleted = false
                }
            };
            return seedDestinations;
        }
    }
}
    
