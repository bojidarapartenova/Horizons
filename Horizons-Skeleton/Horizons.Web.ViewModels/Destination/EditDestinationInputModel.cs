using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Horizons.GCommon.ValidationConstants.Destination;

namespace Horizons.Web.ViewModels.Destination
{
    public class EditDestinationInputModel: AddDestinationInputModel
    {
        public int Id { get; set; }

        [Required]
        public string PublisherId {  get; set; }= null!;
    }
}
