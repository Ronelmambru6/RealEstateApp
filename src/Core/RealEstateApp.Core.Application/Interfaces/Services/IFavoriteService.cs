namespace RealEstateApp.Core.Application.Interfaces.Services;

public interface IFavoriteService
{
    /// <summary>Ids de las propiedades marcadas como favoritas por el cliente.</summary>
    Task<HashSet<int>> GetFavoritePropertyIdsAsync(string clientId);

    /// <summary>Marca o desmarca una propiedad como favorita. Devuelve true si quedó marcada.</summary>
    Task<bool> ToggleFavoriteAsync(string clientId, int propertyId);
}
