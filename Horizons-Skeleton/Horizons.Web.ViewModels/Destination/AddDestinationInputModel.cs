using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Horizons.GCommon.ValidationConstants.Destination;

namespace Horizons.Web.ViewModels.Destination
{
    public class AddDestinationInputModel
    {
        [Required]
        [MinLength(NameMinLength)]
        [MaxLength(NameMaxLength)]
        public string Name { get; set; } = null!;

        [Required]
        public int TerrainId {  get; set; }

        public IEnumerable<AddDestinationTerrainDropDownModel>? Terrains { get; set; }

        [Required]
        [MinLength(DescriptionMinLength)]
        [MaxLength(DescriptionMaxLength)]
        public string Description {  get; set; } = null!;

        public string? ImageUrl {  get; set; }

        [Required]
        public string PublishedOn { get; set; } = null!;

    }
}
