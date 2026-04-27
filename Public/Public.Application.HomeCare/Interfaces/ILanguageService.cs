using Public.Domain.HomeCare.DataModels.Response.ServicePartners;
using Shared.HomeCare.Entities;
using Shared.Interfaces.Services;

namespace Public.Application.HomeCare.Interfaces
{
    public interface ILanguageService : IGenericService<Language>
    {
        Task<IEnumerable<GetLanguageResponseModel>> GetAllLanguagesAsync();
    }
}