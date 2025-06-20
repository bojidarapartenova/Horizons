using Horizons.Web.ViewModels.Destination;

namespace Horizons.Services.Core.Contracts
{
    public interface IDestinationService
    {
        Task<IEnumerable<DestinationIndexViewModel>> GetAllDestinationsAsync(string? userId);
        Task<DestinationDetailsViewModel?> GetDestinationDetailsAsync(int? id, string? userId);
        Task<bool> AddDestinationAsync(string userId, AddDestinationInputModel inputModel);
        Task<EditDestinationInputModel?> GetDestinationForEditingAsync(string userId, int? dId);
        Task<bool> PersistEditDestinationAsync(EditDestinationInputModel inputModel);
        Task<DeleteDestinationViewModel?> GetDestinationForDeletingAsync(string userId, int? dId);
        Task<bool> SoftDeleteDestinationAsync(string userId, DeleteDestinationViewModel deleteModel);
        Task<IEnumerable<FavoriteDestinationViewModel>?> GetFavoriteDestinationsAsync(string userId);
        Task<bool> AddToFavoritesAsync(string userId, int dId);
        Task<bool> RemoveFromFavoritesAsync(string userId, int dId);
    }
}
