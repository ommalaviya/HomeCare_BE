using AutoMapper;
using Public.Application.HomeCare.Interfaces;
using Public.Domain.HomeCare.DataModels.Response.ServicePartners;
using Public.Domain.HomeCare.Interface;
using Shared.HomeCare.Entities;
using Shared.HomeCare.Interfaces.Repositories;
using Shared.HomeCare.Services;
using System.Security.Claims;

namespace Public.Application.HomeCare.Services
{
    public class LanguageService(
        ILanguageRepository languageRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ClaimsPrincipal principal)
        : GenericService<Language>(languageRepository, unitOfWork, mapper, principal), ILanguageService
    {
        public async Task<IEnumerable<GetLanguageResponseModel>> GetAllLanguagesAsync()
        {
            var result = await GetAllAsync(x => !x.IsDeleted);
            return ToResponseModel<IEnumerable<GetLanguageResponseModel>>(result.Records);
        }
    }
}