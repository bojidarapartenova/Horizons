using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Horizons.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace Horizons.Web.ViewModels.Destination
{
    public class DestinationIndexViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? ImageUrl { get; set; }
        public string Terrain {  get; set; }=null!;
        public long FavoritesCount {  get; set; }
        public bool IsPublisher { get; set; }
        public bool IsFavorite { get; set; }
    }
}
