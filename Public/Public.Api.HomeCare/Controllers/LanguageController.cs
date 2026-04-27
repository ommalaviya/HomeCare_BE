using Microsoft.AspNetCore.Mvc;
using Public.Application.HomeCare.Interfaces;
using Shared.Helpers;

namespace Public.Api.HomeCare.Controllers
{
    [Route("api/language")]
    [ApiController]
    public class LanguageController(ILanguageService languageService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var result = await languageService.GetAllLanguagesAsync();
            return Ok(ResponseHelper.SuccessResponse(result));
        }
    }
}
