using AutoMapper;

namespace RealEstateApp.Core.Application.Mappings;

/// <summary>
/// Perfil base de AutoMapper (Entities, ViewModels y DTOs).
/// Cada desarrollador agrega sus mapeos dentro de su propia región para evitar conflictos de merge.
/// </summary>
public class GeneralProfile : Profile
{
    public GeneralProfile()
    {
        #region Módulos Dev A (Propiedades, Admin, Ofertas)
        // CreateMap<Property, PropertyViewModel>().ReverseMap();
        #endregion

        #region Módulos Dev B (Catálogos, Favoritos, Chat)
        // CreateMap<PropertyType, PropertyTypeViewModel>().ReverseMap();
        #endregion

        #region DTOs (WebApi)
        // CreateMap<Property, PropertyDto>().ReverseMap();
        #endregion
    }
}
